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
        public int totalAmount { get; set; }
        public int hourlyRate { get; set; }
        public string claimStatus { get; set; }
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
                                                    ClaimStatus VARCHAR(50) DEFAULT 'Pending',
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
        public List<Claims_Queries> GetClaimsByLecturer(int lecturerID)
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
                                (HoursWorked * HourlyRate) AS Amount,
                                ClaimStatus
                             FROM Claims
                             WHERE LecturerID = @LecturerID";

                    using (SqlCommand command = new SqlCommand(query, connect))
                    {
                        command.Parameters.AddWithValue("@LecturerID", lecturerID);

                        using (SqlDataReader reader = command.ExecuteReader())
                        {
                            while (reader.Read())
                            {
                                claimsList.Add(new Claims_Queries
                                {
                                    claimID = Convert.ToInt32(reader["ClaimID"]),
                                    name = reader["Name"].ToString(),
                                    sessions = Convert.ToInt32(reader["Sessions"]),
                                    hoursWorked = Convert.ToInt32(reader["HoursWorked"]),
                                    hourlyRate = Convert.ToInt32(reader["HourlyRate"]),
                                    totalAmount = Convert.ToInt32(reader["Amount"]),
                                    claimStatus = reader["ClaimStatus"].ToString()
                                });
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("Error retrieving lecturer claims: " + ex.Message);
            }

            return claimsList;
        }//end GetClaimsByLecturer
        public List<Claims_Queries> GetAllClaims()
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
                    ";

                    using (SqlCommand command = new SqlCommand(query, connect))
                    {
                       
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
        public void ApproveClaim(int claimID, string claimStatus)
        {
            try
            {
                using (SqlConnection connect = new SqlConnection(connection))
                {
                    connect.Open();
                    string query = @"UPDATE Claims 
                             SET ClaimStatus = @ClaimStatus 
                             WHERE ClaimID = @ClaimID";

                    using (SqlCommand command = new SqlCommand(query, connect))
                    {
                        command.Parameters.AddWithValue("@ClaimStatus", claimStatus);
                        command.Parameters.AddWithValue("@ClaimID", claimID);

                        int rowsAffected = command.ExecuteNonQuery();
                        Console.WriteLine(rowsAffected > 0
                            ? "Claim status updated successfully."
                            : "No claim found with that ID.");
                    }
                }
            }
            catch (Exception error)
            {
                Console.WriteLine("Error approving claim: " + error.Message);
            }
        }//end ApproveClaim
        public List<Claims_Queries> GetPendingClaims()
        {
            List<Claims_Queries> pendingClaims = new List<Claims_Queries>();
            try
            {
                using (SqlConnection connect = new SqlConnection(connection))
                {
                    connect.Open();
                    string query = @"SELECT ClaimID, LecturerID, Name, Sessions, HoursWorked, HourlyRate, Document, ClaimStatus
                             FROM Claims
                             WHERE ClaimStatus = 'Pending'";

                    using (SqlCommand command = new SqlCommand(query, connect))
                    {
                        using (SqlDataReader read = command.ExecuteReader())
                        {
                            while (read.Read())
                            {
                                pendingClaims.Add(new Claims_Queries
                                {
                                    claimID = Convert.ToInt32(read["ClaimID"]),
                                    name = read["Name"].ToString(),
                                    sessions = Convert.ToInt32(read["Sessions"]),
                                    hoursWorked = Convert.ToInt32(read["HoursWorked"]),
                                    hourlyRate = Convert.ToInt32(read["HourlyRate"]),
                                    document = read["Document"].ToString(),
                                    claimStatus = read["ClaimStatus"].ToString()
                                });
                            }
                        }
                    }
                }
            }
            catch (Exception error)
            {
                Console.WriteLine("Error retrieving pending claims: " + error.Message);
            }

            return pendingClaims;
        }//end GetPendingClaims

        
    }
}

