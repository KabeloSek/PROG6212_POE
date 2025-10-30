using Microsoft.AspNetCore.Identity;
using System.ComponentModel.DataAnnotations;
using System.Data.SqlClient;

namespace PROG6212_POE.Models
{
    public class Claims_Queries
    {

        public int claimID { get; set; }
        [Required]
        public string name { get; set; }
        [Required]
        public int sessions { get; set; }
        [Required]
        public int hoursWorked { get; set; }
        [Required]
        public int hourlyRate { get; set; }
        
        public string? document { get; set; }

        private string connection = @"Server=(localdb)\claim_system;Database=claims_database;";

        public void CreateClaimsTable()
        {
            try
            {
                using (SqlConnection connect = new SqlConnection(connection))
                {
                    connect.Open();
                    string claimsTable = @"IF OBJECT_ID('dbo.Claims','U') IS NULL
                                           BEGIN
                                                CREATE TABLE Claims(
                                                    ClaimID INT PRIMARY KEY IDENTITY(1,1),
                                                    LecturerID INT,
                                                    Name Varchar(50) NOT NULL,
                                                    Sessions INT,
                                                    HoursWorked INT,
                                                    HourlyRate INT,
                                                    Document VARCHAR(100),
                                                    FOREIGN KEY (LecturerID) REFERENCES Lecturer(LecturerID)
                                                    );               
                                           END";
                    using (SqlCommand create = new SqlCommand(claimsTable, connect))
                    {
                        create.ExecuteNonQuery();
                        Console.WriteLine("Claims table created successfully.");
                    }
                }
            }
            catch (Exception error)
            {
                Console.WriteLine("Error creating Claims table: " + error.Message);
            }
        }//end CreateClaimsTable

        public void storeClaim(int lecturerID, string name, int sessions, int hoursWorked, int hourlyRate, string document)
        {
            try
            {
                using (SqlConnection connect = new SqlConnection(connection))
                {
                    connect.Open();

                    string insertClaim = @"
                INSERT INTO Claims (LecturerID, Name, Sessions, HoursWorked, HourlyRate, Document)
                VALUES (@LecturerID, @Name, @Sessions, @HoursWorked, @HourlyRate, @Document)";

                    using (SqlCommand insert = new SqlCommand(insertClaim, connect))
                    {
                        insert.Parameters.AddWithValue("@LecturerID", lecturerID);
                        insert.Parameters.AddWithValue("@Name", name);
                        insert.Parameters.AddWithValue("@Sessions", sessions);
                        insert.Parameters.AddWithValue("@HoursWorked", hoursWorked);
                        insert.Parameters.AddWithValue("@HourlyRate", hourlyRate);
                        insert.Parameters.AddWithValue("@Document", document ?? "No document uploaded");

                        insert.ExecuteNonQuery();
                        Console.WriteLine("Claim stored successfully.");
                    }
                }
            }
            catch (Exception error)
            {
                Console.WriteLine("Error storing claim: " + error.Message);
            }
        }

        public List<Claims_Queries> GetAllClaims(int lecturerID)
        {
            List<Claims_Queries> claimsList = new List<Claims_Queries>();
            try
            {
                using (SqlConnection connect = new SqlConnection(connection))
                {
                    connect.Open();
                    string query = @"SELECT 
                    ClaimID,
                    Name,
                    Sessions,
                    HoursWorked,
                    HourlyRate,
                    Document
                 FROM Claims
                    WHERE LecturerID = @LecturerID;";

                    using (SqlCommand command = new SqlCommand(query, connect))
                    {
                        command.Parameters.AddWithValue("@LecturerID", lecturerID);
                        using (SqlDataReader read = command.ExecuteReader())
                        {
                            while(read.Read())
                            {
                                claimsList.Add(new Claims_Queries
                                {
                                    claimID = Convert.ToInt32(read["ClaimID"]),
                                    name = read["Name"].ToString(),
                                    sessions = Convert.ToInt32(read["Sessions"]),
                                    hoursWorked = Convert.ToInt32(read["HoursWorked"]),
                                    hourlyRate = Convert.ToInt32(read["HourlyRate"]),
                                    document = read["Document"].ToString()
                                });
                                
                            }
                        }
                    }
                }
            }
            catch (Exception error)
            {
                Console.WriteLine("Error retrieving claims: " + error.Message); 
            }
            return claimsList;
        }//end GetAllClaims
    }
}

