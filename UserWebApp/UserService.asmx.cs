using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Data;
using System.Linq;
using System.Web;
using System.Web.Services;
using System.IO;
using System.Xml.Serialization;

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

        [WebMethod]
        public List<string> GetLogs()
        {
            try
            {
                var logs = _logger.GetLogs();
                _logger.Log("Successfully retrieved logs");
                return logs;
            }
            catch (Exception ex)
            {
                _logger.Log($"GetLogs error: {ex.Message}");
                return new List<string> { "Error retrieving logs" };
            }
        }

        [WebMethod]
        public ResultObject TestReturn()
        {
            return new ResultObject
            {
                Retcode = 0,
                Retmess = "Successfully"
            };
        }
    }

    public class Logger
    {
        private readonly string _logPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Logs");
        private readonly object _lock = new object();

        public void Log(string message)
        {
            lock (_lock)
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

        public List<string> GetLogs(int maxLines = 100)
        {
            lock (_lock)
            {
                try
                {
                    string fileName = $"Log_{DateTime.Now:yyyy-MM-dd}.txt";
                    string fullPath = Path.Combine(_logPath, fileName);

                    if (!File.Exists(fullPath))
                        return new List<string>();

                    var lines = File.ReadAllLines(fullPath).Reverse().Take(maxLines).Reverse().ToList();
                    return lines.Count > 0 ? lines : new List<string>();
                }
                catch
                {
                    return new List<string> { "Error reading log file" };
                }
            }
        }
    }

    [XmlRoot(ElementName = "Result")]
    public class ResultObject
    {
        [XmlElement("RETCODE")]
        public int Retcode { get; set; }

        [XmlElement("RETMESS")]
        public string Retmess { get; set; }
    }
}
