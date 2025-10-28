using System.ComponentModel.DataAnnotations;
using System.Data.SqlClient;

namespace PROG6212_POE.Models
{
    public class Register
    {
        [Required]
        public string name { get; set; }
        [Required]
        public string surname { get; set; }
        [Required]
        public string email { get; set; }
        [Required]
        public string password { get; set; }
        [Required]
        public string role { get; set; }

        private string connection = @"Server=(localdb)\claim_system;Database=claims_database";

        public void CreateUserTables()
        {
            try
            {
                using (SqlConnection connect = new SqlConnection(connection))
                {
                    connect.Open();

                    //SQL query to create Lectures table
                    string LecturerTable = @"IF OBJECT_ID('dbo.Lecturer','U') IS NULL
                                            BEGIN
                                            CREATE TABLE Lecturer (
                                            LecturerID INT PRIMARY KEY IDENTITY(1,1),
                                            Name VARCHAR(50) NOT NULL,
                                            Surname VARCHAR(50) NOT NULL,
                                            Email VARCHAR(100) NOT NULL UNIQUE,
                                            Password VARCHAR(75) NOT NULL,
                                            Role VARCHAR(50) NOT NULL,
                                            ClaimID INT,
                                            FOREIGN KEY (ClaimID) REFERENCES Claim(ClaimID)
                                           );
                                            END";

                    //SQL query to create PC table
                    string PCTable = @"IF OBJECT_ID('dbo.PC','U') IS NULL
                                        BEGIN
                                        CREATE TABLE PC (
                                        PCID INT PRIMARY KEY IDENTITY(1,1),
                                        Name VARCHAR(50) NOT NULL,
                                        Surname VARCHAR(50) NOT NULL,
                                        Email VARCHAR(100) NOT NULL UNIQUE,
                                        Password VARCHAR(75) NOT NULL,
                                        Role VARCHAR(50) NOT NULL,
                                        ClaimID INT,
                                        FOREIGN KEY (ClaimID) REFERENCES Claim(ClaimID)
                                       );
                                        END";
                    //SQL query to create PM table
                    string PMTable = @"IF OBJECT_ID('dbo.PM','U') IS NULL
                                        BEGIN
                                        CREATE TABLE PM (
                                        PMID INT PRIMARY KEY IDENTITY(1,1),
                                        Name VARCHAR(50) NOT NULL,
                                        Surname VARCHAR(50) NOT NULL,
                                        Email VARCHAR(100) NOT NULL UNIQUE,
                                        Password VARCHAR(75) NOT NULL,
                                        Role VARCHAR(50) NOT NULL,
                                        ClaimID INT,
                                        FOREIGN KEY (ClaimID) REFERENCES Claim(ClaimID)
                                       );
                                        END";

                    //using command to execute create LecturerTable query
                    using (SqlCommand createLecturer = new SqlCommand(LecturerTable, connect))
                    {
                        try
                        {
                            //executing query 
                            createLecturer.ExecuteNonQuery();
                            //after query is executed print success message
                            Console.WriteLine("Lecturer table created successfully.");
                        }
                        catch (Exception error)
                        {

                            //if table creation fails print failure message
                            Console.WriteLine("Lecturer table creation failed" + error.Message);
                        }
                    }

                    //using command to execute create PCTable query
                    using (SqlCommand createPC = new SqlCommand(PCTable, connect))
                    {
                        try
                        {
                            //executing query
                            createPC.ExecuteNonQuery();
                            Console.WriteLine("PC table created succesfully.");
                        }
                        catch (Exception error)
                        {
                            Console.WriteLine("PC table creation failed" + error.Message);
                        }

                    }
                    //using command to execute create PMTable query
                    using (SqlCommand createPM = new SqlCommand(PMTable, connect))
                    {
                        try
                        {
                            //executing query
                            createPM.ExecuteNonQuery();
                            Console.WriteLine("PM table created succesfully.");
                        }
                        catch (Exception error)
                        {
                            Console.WriteLine("PM table creation failed" + error.Message);
                        }
                    }
                }
            }
            catch (Exception error)
            {
                Console.WriteLine("Cannot create tables(Lecturer, PC, PM) " + error.Message);
            }
        }//end of CreateUserTables method

        public void storeLecturer(string name, string surname, string email, string password, string role)
        {
            try
            {
                using (SqlConnection connect = new SqlConnection(connection))
                {
                    connect.Open();
                    //SQL query to insert lecturer data into Lecturer table
                    string insertLecturer = @"INSERT INTO Lecturer
                                                VALUES
                                                ('"+name+"','"+surname+"','"+email+"','"+password+"','"+role+"');";

                    //using command to execute insertLecturer query
                    using (SqlCommand insert = new SqlCommand(insertLecturer, connect))
                    {
                        try
                        {
                            //executing query
                            insert.ExecuteNonQuery();
                            Console.WriteLine("Lecturer data inserted successfully.");
                        }
                        catch (Exception error)
                        {
                            Console.WriteLine("Lecturer data insertion failed" + error.Message);
                        }
                    }
                }
            }
            catch (Exception error)
            {
                Console.WriteLine("Cannot insert lecturer data " + error.Message);
            }
        }//end of storeLecturer method

        public void storePC(string name, string surname, string email, string password, string role)
        {
            try
            {
                using (SqlConnection connect = new SqlConnection(connection))
                {
                    connect.Open();
                    //SQL query to insert PC data into PC table
                    string insertPC = @"INSERT INTO PC
                                                VALUES
                                                ('" + name + "','" + surname + "','" + email + "','" + password + "','" + role + "');";
                    //using command to execute insertPC query
                    using (SqlCommand insert = new SqlCommand(insertPC, connect))
                    {
                        try
                        {
                            //executing query
                            insert.ExecuteNonQuery();
                            Console.WriteLine("PC data inserted successfully.");
                        }
                        catch (Exception error)
                        {
                            Console.WriteLine("PC data insertion failed" + error.Message);
                        }
                    }
                }
            }
            catch (Exception error)
            {
                Console.WriteLine("Cannot insert PC data " + error.Message);
            }
        }//end of storePC method

        public void storePM(string name, string surname, string email, string password, string role)
        {
            try
            {
                using (SqlConnection connect = new SqlConnection(connection))
                {
                    connect.Open();
                    //SQL query to insert PM data into PM table
                    string insertPM = @"INSERT INTO PM
                                                VALUES
                                                ('" + name + "','" + surname + "','" + email + "','" + password + "','" + role + "');";
                    //using command to execute insertPM query
                    using (SqlCommand insert = new SqlCommand(insertPM, connect))
                    {
                        try
                        {
                            //executing query
                            insert.ExecuteNonQuery();
                            Console.WriteLine("PM data inserted successfully.");
                        }
                        catch (Exception error)
                        {
                            Console.WriteLine("PM data insertion failed" + error.Message);
                        }
                    }
                }
            }
            catch (Exception error)
            {
                Console.WriteLine("Cannot insert PM data " + error.Message);
            }
        }//end of storePM method
    }
}
