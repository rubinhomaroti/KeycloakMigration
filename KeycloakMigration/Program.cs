using KeycloakMigration;
using Microsoft.Extensions.Configuration;
using System.Data;

await Run();

async Task Run()
{
    try
    {
        Logger.StartLog();

        IConfiguration config = new ConfigurationBuilder()
                        .AddJsonFile("appSettings.json")
                        .Build();

        using (DbClient dbClient = new DbClient(config.GetConnectionString("LegacyMySql")))
        {
            dbClient.TestConnection();
            Logger.Log("MySQL connection OK!");

            KeycloakClient keycloakClient = new KeycloakClient(config.GetRequiredSection("Keycloak:BaseUrl").Value,
                                                config.GetRequiredSection("Keycloak:Realm").Value,
                                                config.GetRequiredSection("Keycloak:ClientId").Value,
                                                config.GetRequiredSection("Keycloak:ClientSecret").Value);

            await keycloakClient.GenerateToken();
            Logger.Log("Keycloak authentication OK!");

            List<string>? usersInKeycloak = await keycloakClient.GetUsers();
            if (usersInKeycloak != null)
            {
                string usersInKeycloakStr = string.Join(",", usersInKeycloak);
                usersInKeycloakStr = "'" + usersInKeycloakStr.Replace(",", "','") + "'";
                string query = config.GetRequiredSection("Query").Value;
                using (DataTable userDbData = dbClient.DoQuery(query.Replace("{EXISTING_USERS}", usersInKeycloakStr)))
                {
                    Logger.Log($"{userDbData.Rows.Count} usuários a serem importados");

                    for (int i = 0; i < userDbData.Rows.Count; i++)
                    {
                        string? id = userDbData.Rows[i]["id"].ToString();
                        string? email = userDbData.Rows[i]["email"].ToString();
                        string? name = userDbData.Rows[i]["name"].ToString();

                        if (id != null && email != null && name != null)
                        {
                            Console.WriteLine();
                            User user = new User(email!, name!);
                            Logger.Log($"Starting user import:\n" +
                                user.ToString());

                            string userId = await keycloakClient.CreateUser(user);
                        }
                    }
                }
            }
        }
    }
    catch (Exception ex)
    {
        Logger.Log(ex.GetExceptionMessages());
    }
    finally
    {
        Logger.FinishLog();
    }
}