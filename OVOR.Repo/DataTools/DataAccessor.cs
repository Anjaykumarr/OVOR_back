using System.Data;
using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;
using MySql.Data.MySqlClient;
using OVOR.Repo.validations;

namespace OVOR.Repo.DataTools
{

    public static class DataAccessor
    {
        private static string _connectionString;

        public static void Initialize(IConfiguration configuration)
        {
            _connectionString = configuration.GetConnectionString("Default");
        }

        // To return no value just update db.
        public static void ExecuteNonQuery(string spName, List<MySqlParameter>? parameters = null)
        {
            using (MySqlConnection con = new MySqlConnection(_connectionString))
            using (MySqlCommand cmd = new MySqlCommand(spName, con))
            {
                cmd.CommandType = CommandType.StoredProcedure;
                if (parameters != null)
                    cmd.Parameters.AddRange(parameters.ToArray());

                con.Open();
                cmd.ExecuteNonQuery();
            }
        }


        // To return multiple rows from db.
        public static DataTable GetDataTable(string spName, List<MySqlParameter>? parameters = null)
        {
            DataTable dt = new DataTable();

            try
            {
                using (MySqlConnection con = new MySqlConnection(_connectionString))
                using (MySqlCommand cmd = new MySqlCommand(spName, con))
                {
                    cmd.CommandType = CommandType.StoredProcedure;
                    if (parameters != null)
                        cmd.Parameters.AddRange(parameters.ToArray());

                    using (MySqlDataAdapter da = new MySqlDataAdapter(cmd))
                    {
                        con.Open();
                        da.Fill(dt);
                    }
                }
                
            }
            catch (Exception ex)
            {
                ErrorValidations.ThrowException("Failed to load data", ex);
            }

            return dt;
        }


        public static async Task<DataTable> GetDataTableAsync(string spName, List<MySqlParameter>? parameters = null)
        {
            DataTable dt = new DataTable();

            try
            {
                using (var con = new MySqlConnection(_connectionString))
                using (var cmd = new MySqlCommand(spName, con))
                {
                    cmd.CommandType = CommandType.StoredProcedure;

                    if (parameters != null)
                        cmd.Parameters.AddRange(parameters.ToArray());

                    await con.OpenAsync();

                    using (var reader = await cmd.ExecuteReaderAsync())
                    {
                        dt.Load(reader);
                    }
                }
            }
            catch (Exception ex)
            {
                ErrorValidations.ThrowException("Failed to load data", ex);
            }

            return dt;
        }

    }
}
