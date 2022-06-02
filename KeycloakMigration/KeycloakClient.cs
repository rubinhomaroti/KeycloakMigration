using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using RestSharp;
using System.Net;

namespace KeycloakMigration
{
    internal class KeycloakClient
    {
        private string _baseUrl { get; set; }
        private string _realm { get; set; }
        private string _clientId { get; set; }
        private string _clientSecret { get; set; }
        private string _accessToken { get; set; }

        public KeycloakClient(string baseUrl, string realm, string clientId, string clientSecret)
        {
            _baseUrl = baseUrl;
            _realm = realm;
            _clientId = clientId;
            _clientSecret = clientSecret;
            _accessToken = string.Empty;
        }

        public async Task GenerateToken()
        {
            try
            {
                RestClient client = new RestClient(_baseUrl + $"auth/realms/{_realm}/protocol/openid-connect/token");
                RestRequest request = new RestRequest();
                request.AddParameter("grant_type", "client_credentials", ParameterType.GetOrPost);
                request.AddParameter("client_id", _clientId, ParameterType.GetOrPost);
                request.AddParameter("client_secret", _clientSecret, ParameterType.GetOrPost);

                RestResponse response = await client.PostAsync(request);

                if (response.StatusCode == HttpStatusCode.OK)
                {
                    JObject? responseBody = JsonConvert.DeserializeObject<JObject>(response.Content!);
                    if (responseBody != null)
                    {
                        if (responseBody.TryGetValue("access_token", out JToken? accessToken))
                        {
                            _accessToken = accessToken.ToString();
                        }
                    }
                }
                else
                {
                    throw new Exception("Failed to request access token: " + response.ErrorMessage);
                }
            }
            catch (Exception ex)
            {
                throw new Exception("Error during requesting access token " + ex.Message);
            }
        }

        public async Task<string> CreateUser(User user)
        {
            try
            {
                if (string.IsNullOrEmpty(_accessToken))
                    throw new Exception("Invalid token, you must generate your token before start using Keycloak API");

                RestClient client = new RestClient(_baseUrl + $"auth/admin/realms/{_realm}/users");
                RestRequest request = new RestRequest();
                request.AddHeader("Authorization", $"Bearer {_accessToken}");
                request.AddHeader("Content-Type", @"application/json");

                var body = new
                {
                    enabled = true,
                    username = user.Email,
                    email = user.Email,
                    firstName = user.FirstName,
                    lastName = user.LastName,
                    requiredActions = new string[] { "UPDATE_PASSWORD", "VERIFY_EMAIL" }
                };

                request.AddBody(body);

                RestResponse response = await client.PostAsync(request);
                if (response.StatusCode == HttpStatusCode.Created)
                {
                    string location = response.Headers!.ToList()
                                                       .Find(x => x.Name == "Location")!
                                                       .Value!.ToString()!;
                    Uri locationUri = new Uri(location);
                    string userId = locationUri.Segments.Last();
                    return userId;
                }
            }
            catch (Exception ex)
            {
                Logger.Log($"Failed to create the user {user.Email}: {ex.Message}");
            }

            return "";
        }

        public async Task<List<string>?> GetUsers()
        {
            try
            {
                if (string.IsNullOrEmpty(_accessToken))
                    throw new Exception("Invalid token, you must generate your token before start using Keycloak API");

                RestClient client = new RestClient(_baseUrl + $"auth/admin/realms/{_realm}/users");
                RestRequest request = new RestRequest();
                request.AddHeader("Authorization", $"Bearer {_accessToken}");
                request.AddQueryParameter("max", 50000);

                RestResponse response = await client.GetAsync(request);
                if (response.StatusCode == HttpStatusCode.OK)
                {
                    JArray? responseBody = JsonConvert.DeserializeObject<JArray>(response.Content!);
                    return responseBody!.Select(x => x["email"]!.ToString()).ToList();
                }
            }
            catch (Exception ex)
            {
                throw new Exception("Failed to request Keycloak users: " + ex.Message);
            }

            return null;
        }
    }
}
