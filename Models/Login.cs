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

        public bool getUser(string email, string password, string role)
        {
            bool found = false;
            try
            {
                using (SqlConnection connect = new SqlConnection(connection))
                {
                    connect.Open();
                    string tableName = "";

                    switch (role.ToLower())
                    {
                        case "lecturer":
                            tableName = "Lecturer";
                        break;

                        case "programcoordinator":
                            tableName = "PC";
                        break;

                        case "programmanager":
                            tableName = "PM";
                        break;

                        default:
                            Console.WriteLine("Invalid role specified.");
                        break;

                    }

                    string query = @"SELECT * FROM "+tableName+" WHERE Email='"+email+"' AND Password='"+password+"' AND Role='"+role+"'";

                    if (tableName == "Lecturer")
                    {
                        try
                        {
                            using (SqlCommand command = new SqlCommand(query, connect))
                            {
                                using (SqlDataReader read = command.ExecuteReader())
                                {
                                    while (read.Read())
                                    {
                                        found = true;
                                        Console.WriteLine("Lecturer found" + read["Name"] + " " + read["Surname"]);
                                    }
                                }
                            }
                        }
                        catch (Exception error)
                        {
                            Console.WriteLine("Could not find Lecturer" + error.Message);
                        }

                    }
                    else if (tableName == "PC")
                    {
                        try
                        {
                            using (SqlCommand command = new SqlCommand(query, connect))
                            {
                                using (SqlDataReader read = command.ExecuteReader())
                                {
                                    while(read.Read())
                                    {
                                        found = true;
                                        Console.WriteLine("Program Coordinator found" + read["Name"] + " " + read["Surname"]);
                                    }
                                }
                            }
                        }
                        catch (Exception error)
                        {
                            Console.WriteLine("Could not find Program Coordinator" + error.Message);
                        }
                    }
                    else if(tableName == "PM")
                    {
                        try
                        {
                            using (SqlCommand command = new SqlCommand(query, connect))
                            {
                                using (SqlDataReader read = command.ExecuteReader())
                                {
                                    while (read.Read())
                                    {
                                        found = true;
                                        Console.WriteLine("Program Manager found" + read["Name"] + " " + read["Surname"]);
                                    }
                                }
                            }
                        }
                        catch (Exception error)
                        {
                            Console.WriteLine("Could not find Program Manager" + error.Message);
                        }
                    }
                }
            }
            catch (Exception error)
            {
                Console.WriteLine("User not found" + error.Message);
            }


            return found;
        }//end getUser
    }
}
