using Microsoft.AspNetCore.Identity;
using System.ComponentModel.DataAnnotations;
using System.Data.SqlClient;

namespace PROG6212_POE.Models
{
    public class Claims_Queries
    {
        public int claimID { get; set; }
        public int lecturerID { get; set; }
        public int pcID { get; set; }
        public int pmID { get; set; }
        public string pcName { get; set; }
        public string pmName { get; set; }
        public string lecturerName { get; set; }

        [Required]
        public int sessions { get; set; }
        [Required]
        public decimal hoursWorked { get; set; }
        [Required]
        public decimal hourlyRate { get; set; }
        [Required]
        public string document { get; set; }

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
                                                    Name Varchar(50) NOT NULL,
                                                    Sessions INT,
                                                    HoursWorked INT,
                                                    HourlyRate INT,
                                                    Document VARCHAR(100)
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
    }
}

