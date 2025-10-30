using System.ComponentModel.DataAnnotations;
using System.Data.SqlClient;

namespace PROG6212_POE.Models
{
    public class Login
    {
        [Required]
        public string email { get; set; }
        [Required]
        public string password { get; set; }
        [Required]
        public string role { get; set; }

        private string connection = @"Server=(localdb)\claim_system;Database=claims_database;";

        public int GetUserID(string email, string password, string role)
        {
            int userId = 0;
            try
            {
                using (SqlConnection connect = new SqlConnection(connection))
                {
                    connect.Open();
                    string tableName = role.ToLower() switch
                    {
                        "lecturer" => "Lecturer",
                        "programcoordinator" => "PC",
                        "programmanager" => "PM",
                        _ => ""
                    };
                    if (string.IsNullOrEmpty(tableName))
                        return 0;
                    string query = @"SELECT * FROM " + tableName + " WHERE Email='" + email + "' AND Password='" + password + "' AND Role='" + role + "'";


                    using (SqlCommand command = new SqlCommand(query, connect))
                    {
                        using (SqlDataReader read = command.ExecuteReader())
                        {
                            if (read.Read())
                            {
                                userId = Convert.ToInt32(read[tableName + "ID"]);
                                Console.WriteLine(role + " found" + read["Name"] + " " + read["Surname"]);
                            }
                        }
                    }
                }
            }
            catch (Exception error) 
            { 
            
                Console.WriteLine("Could not find Lecturer" + error.Message);
            }
            return userId;
        }//end getUser
    } 
}
    