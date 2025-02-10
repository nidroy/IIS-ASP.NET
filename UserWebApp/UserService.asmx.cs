using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Data;
using System.Linq;
using System.Web;
using System.Web.Services;
using System.IO;

namespace UserWebApp
{
    /// <summary>
    /// Сводное описание для UserService
    /// </summary>
    [WebService(Namespace = "http://tempuri.org/")]
    [WebServiceBinding(ConformsTo = WsiProfiles.BasicProfile1_1)]
    public class UserService : WebService
    {
        private readonly string _connectionString = System.Configuration.ConfigurationManager.ConnectionStrings["UsersDB"].ConnectionString;
        private readonly Logger _logger = new Logger();

        [WebMethod]
        public string GetUsers()
        {
            _logger.Log("GetUsers method called");

            DataTable dt = new DataTable();

            try
            {
                using (SqlConnection connection = new SqlConnection(_connectionString))
                {
                    string query = "SELECT * FROM Users";
                    SqlCommand command = new SqlCommand(query, connection);
                    connection.Open();
                    dt.Load(command.ExecuteReader());
                }
                _logger.Log("Successfully retrieved users");

                // Преобразование DataTable в список строк

                string result = "";

                foreach (DataRow row in dt.Rows)
                {
                    string userString =
                        $"ID: {row["Id"]}, " +
                        $"Name: {row["Name"]}, " +
                        $"Email: {row["Email"]}, " +
                        $"RegDate: {row["RegistrationDate"]}";

                    result += $"{userString}\n";
                }

                _logger.Log($"Successfully retrieved {dt.Rows.Count} users");

                return result;
            }
            catch (Exception ex)
            {
                _logger.Log($"Error: {ex.Message}");
                throw new Exception("Database error", ex);
            }
        }

        [WebMethod]
        public int CreateUser(string name = "", string email = "")
        {
            _logger.Log($"CreateUser called with params: {name}, {email}");

            try
            {
                using (SqlConnection connection = new SqlConnection(_connectionString))
                {
                    SqlCommand command = new SqlCommand("CreateUser", connection);
                    command.CommandType = CommandType.StoredProcedure;

                    command.Parameters.AddWithValue("@Name", name);
                    command.Parameters.AddWithValue("@Email", email);

                    SqlParameter returnParam = command.Parameters.Add("RetVal", SqlDbType.Int);
                    returnParam.Direction = ParameterDirection.ReturnValue;

                    connection.Open();
                    command.ExecuteNonQuery();

                    int result = (int)returnParam.Value;
                    _logger.Log($"CreateUser completed with code: {result}");
                    return result;
                }
            }
            catch (Exception ex)
            {
                _logger.Log($"CreateUser error: {ex.Message}");
                throw new Exception("Service error", ex);
            }
        }
    }

    public class Logger
    {
        private readonly string _logPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Logs");

        public void Log(string message)
        {
            try
            {
                Directory.CreateDirectory(_logPath);
                string fileName = $"Log_{DateTime.Now:yyyy-MM-dd}.txt";
                string fullPath = Path.Combine(_logPath, fileName);

                File.AppendAllText(fullPath, $"{DateTime.Now:yyyy-MM-dd HH:mm:ss} - {message}{Environment.NewLine}");
            }
            catch (Exception ex)
            {
                // Обработка ошибок логирования
            }
        }
    }
}
