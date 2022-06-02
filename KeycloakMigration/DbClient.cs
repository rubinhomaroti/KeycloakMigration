using MySql.Data.MySqlClient;
using System.Data;
using System.Data.Common;

namespace KeycloakMigration
{
    internal class DbClient : IDisposable
    {
        private MySqlConnection _connection { get; set; }
        private string _connectionString { get; set; }

        public DbClient(string connectionString)
        {
            _connectionString = connectionString;
            _connection = GetDbConnection();
        }

        private MySqlConnection GetDbConnection()
        {
            try
            {
                return new MySqlConnection(_connectionString);
            }
            catch (Exception ex)
            {
                throw new Exception("Error during database connection creation", ex);
            }
        }

        public bool TestConnection()
        {
            try
            {
                _connection.Open();
                _connection.Close();
                return true;
            }
            catch (Exception ex)
            {
                throw new Exception("Error while testing database connection", ex);
            }
        }

        public DataTable DoQuery(string query)
        {
            DbDataAdapter? cmd = null;

            try
            {
                _connection.Open();
                cmd = new MySqlDataAdapter(query, _connection);
                DataTable table = new DataTable();
                cmd.Fill(table);
                return table;
            }
            catch (Exception ex)
            {
                throw new Exception("Error during the query execution", ex);
            }
            finally
            {
                cmd?.Dispose();
                _connection.Close();
            }
        }

        public void Dispose()
        {
            _connection?.Close();
            _connection?.Dispose();
        }
    }
}
