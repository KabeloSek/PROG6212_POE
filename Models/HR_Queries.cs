using System.Data.SqlClient;

namespace PROG6212_POE.Models
{
    public class LecturerDTO
    {
        public int LecturerID { get; set; }
        public string Name { get; set; } = "";
        public string Surname { get; set; } = "";
        public string Email { get; set; } = "";
    }

    public class ClaimDTO
    {
        public int ClaimID { get; set; }
        public int Sessions { get; set; }
        public int HoursWorked { get; set; }
        public int HourlyRate { get; set; }
        public string? Document { get; set; }
        public string? ClaimStatus { get; set; }
        public decimal Amount => (decimal)HoursWorked * HourlyRate;
    }

    public class HR_Queries
    {
        private readonly string connection = @"Server=(localdb)\claim_system;Database=claims_database;";

        public List<LecturerDTO> GetAllLecturers()
        {
            var list = new List<LecturerDTO>();
            try
            {
                using (var conn = new SqlConnection(connection))
                {
                    conn.Open();
                    string sql = "SELECT LecturerID, Name, Surname, Email FROM Lecturer";
                    using (var cmd = new SqlCommand(sql, conn))
                    using (var r = cmd.ExecuteReader())
                    {
                        while (r.Read())
                        {
                            list.Add(new LecturerDTO
                            {
                                LecturerID = Convert.ToInt32(r["LecturerID"]),
                                Name = r["Name"].ToString() ?? "",
                                Surname = r["Surname"].ToString() ?? "",
                                Email = r["Email"].ToString() ?? ""
                            });
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("GetAllLecturers error: " + ex.Message);
            }
            return list;
        }

        public LecturerDTO? GetLecturerById(int lecturerId)
        {
            try
            {
                using (var conn = new SqlConnection(connection))
                {
                    conn.Open();
                    string sql = "SELECT LecturerID, Name, Surname, Email FROM Lecturer WHERE LecturerID = @LecturerID";
                    using (var cmd = new SqlCommand(sql, conn))
                    {
                        cmd.Parameters.AddWithValue("@LecturerID", lecturerId);
                        using (var r = cmd.ExecuteReader())
                        {
                            if (r.Read())
                            {
                                return new LecturerDTO
                                {
                                    LecturerID = Convert.ToInt32(r["LecturerID"]),
                                    Name = r["Name"].ToString() ?? "",
                                    Surname = r["Surname"].ToString() ?? "",
                                    Email = r["Email"].ToString() ?? ""
                                };
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("GetLecturerById error: " + ex.Message);
            }
            return null;
        }

        public bool UpdateLecturer(int lecturerId, string name, string surname, string email)
        {
            try
            {
                using (var conn = new SqlConnection(connection))
                {
                    conn.Open();
                    string sql = @"UPDATE Lecturer
                                   SET Name = @Name, Surname = @Surname, Email = @Email
                                   WHERE LecturerID = @LecturerID";
                    using (var cmd = new SqlCommand(sql, conn))
                    {
                        cmd.Parameters.AddWithValue("@Name", name);
                        cmd.Parameters.AddWithValue("@Surname", surname);
                        cmd.Parameters.AddWithValue("@Email", email);
                        cmd.Parameters.AddWithValue("@LecturerID", lecturerId);
                        int rows = cmd.ExecuteNonQuery();
                        return rows > 0;
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("UpdateLecturer error: " + ex.Message);
                return false;
            }
        }

        public List<ClaimDTO> GetApprovedClaimsForLecturer(int lecturerId)
        {
            var list = new List<ClaimDTO>();
            try
            {
                using (var conn = new SqlConnection(connection))
                {
                    conn.Open();
                    string sql = @"
                        SELECT ClaimID, Sessions, HoursWorked, HourlyRate, Document, ClaimStatus
                        FROM Claims
                        WHERE LecturerID = @LecturerID AND ClaimStatus = 'Approved'
                        ORDER BY ClaimID";
                    using (var cmd = new SqlCommand(sql, conn))
                    {
                        cmd.Parameters.AddWithValue("@LecturerID", lecturerId);
                        using (var r = cmd.ExecuteReader())
                        {
                            while (r.Read())
                            {
                                list.Add(new ClaimDTO
                                {
                                    ClaimID = Convert.ToInt32(r["ClaimID"]),
                                    Sessions = r["Sessions"] == DBNull.Value ? 0 : Convert.ToInt32(r["Sessions"]),
                                    HoursWorked = r["HoursWorked"] == DBNull.Value ? 0 : Convert.ToInt32(r["HoursWorked"]),
                                    HourlyRate = r["HourlyRate"] == DBNull.Value ? 0 : Convert.ToInt32(r["HourlyRate"]),
                                    Document = r["Document"] == DBNull.Value ? null : r["Document"].ToString(),
                                    ClaimStatus = r["ClaimStatus"] == DBNull.Value ? null : r["ClaimStatus"].ToString()
                                });
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("GetApprovedClaimsForLecturer error: " + ex.Message);
            }
            return list;
        }
    }
}