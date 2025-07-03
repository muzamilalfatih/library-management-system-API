using Microsoft.Data.SqlClient;
using SharedClasses;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static SharedClasses.MemberDTOs;
using static SharedClasses.MembershipClassDTOs;

namespace LibraryManagementSystemAPIDataAccessLayer
{
    public class clsMembershipClassData
    {
        public static async Task<Result<MembershipClassDTO>> GetMembershipClassInfoByIDAsync(int membershipClassID)
        {
            using (SqlConnection connection = new SqlConnection(clsDataAccessSettings.ConnectionString))
            {
                string query = "SELECT * FROM MembershipClasses WHERE MembershipClassID = @MembershipClassID";
                using (SqlCommand command = new SqlCommand(query, connection))
                {
                    command.Parameters.AddWithValue("@MembershipClassID", membershipClassID);
                    try
                    {
                        await connection.OpenAsync();
                        using (SqlDataReader reader = await command.ExecuteReaderAsync())
                        {
                            if (reader.Read())
                            {
                                var membershipClass = new MembershipClassDTO(
                                    reader.GetInt32(reader.GetOrdinal("MembershipClassID")),
                                    reader.GetString(reader.GetOrdinal("MembershipClassName")),
                                    reader.GetInt32(reader.GetOrdinal("MaxNumberOfBooksCanBorrow")),
                                    (float)reader.GetDecimal(reader.GetOrdinal("FeesPerDay"))
                                );
                                return new Result<MembershipClassDTO>(true, "Membership class found successfully.", membershipClass);
                            }
                            else
                            {
                                return new Result<MembershipClassDTO>(false, "Membership class not found.", null, 404);
                            }
                        }
                    }
                    catch (Exception ex)
                    {
                        return new Result<MembershipClassDTO>(false, "An unexpected error occurred on the server.", null, 500);
                    }
                }
            }
        }

        public static async Task<Result<string>> GetMembershipClassNameByIDAsync(int membershipClassID)
        {
            string membershipClassName = string.Empty;
            using (SqlConnection connection = new SqlConnection(clsDataAccessSettings.ConnectionString))
            {
                string query = "SELECT MembershipClassName FROM MembershipClasses WHERE MembershipClassID = @MembershipClassID";
                using (SqlCommand command = new SqlCommand(query, connection))
                {
                    command.Parameters.AddWithValue("@MembershipClassID", membershipClassID);
                    try
                    {
                        await connection.OpenAsync();
                        object result = await command.ExecuteScalarAsync();
                        if (result != null)
                        {
                            membershipClassName = result.ToString();
                        }
                        return new Result<string>(true, "Membership class name retrieved successfully.", membershipClassName);
                    }
                    catch (Exception ex)
                    {
                        return new Result<string>(false, "An unexpected error occurred on the server.", membershipClassName, 500);
                    }
                }
            }
        }

        public static async Task<Result<int>> GetMembershipClassIDByNameAsync(string membershipClassName)
        {
            int membershipClassID = -1;
            using (SqlConnection connection = new SqlConnection(clsDataAccessSettings.ConnectionString))
            {
                string query = "SELECT MembershipClassID FROM MembershipClasses WHERE MembershipClassName = @MembershipClassName";
                using (SqlCommand command = new SqlCommand(query, connection))
                {
                    command.Parameters.AddWithValue("@MembershipClassName", membershipClassName);
                    try
                    {
                        await connection.OpenAsync();
                        object result = await command.ExecuteScalarAsync();
                        if (result != null && int.TryParse(result.ToString(), out int id))
                        {
                            membershipClassID = id;
                        }
                        return new Result<int>(true, "Membership class ID retrieved successfully.", membershipClassID);
                    }
                    catch (Exception ex)
                    {
                        return new Result<int>(false, "An unexpected error occurred on the server.", membershipClassID, 500);
                    }
                }
            }
        }

        public static async Task<Result<List<MembershipClassDTO>>> GetAllMembershipClassesAsync()
        {
            var membershipClasses = new List<MembershipClassDTO>();
            using (SqlConnection connection = new SqlConnection(clsDataAccessSettings.ConnectionString))
            {
                string query = "SELECT * FROM MembershipClasses";
                using (SqlCommand command = new SqlCommand(query, connection))
                {
                    try
                    {
                        await connection.OpenAsync();
                        using (SqlDataReader reader = await command.ExecuteReaderAsync())
                        {
                            while (await reader.ReadAsync())
                            {
                                membershipClasses.Add(new MembershipClassDTO(
                                    reader.GetInt32(reader.GetOrdinal("MembershipClassID")),
                                    reader.GetString(reader.GetOrdinal("MembershipClassName")),
                                    reader.GetInt32(reader.GetOrdinal("MaxNumberOfBooksCanBorrow")),
                                    (float)reader.GetDecimal(reader.GetOrdinal("FeesPerDay"))
                                ));
                            }
                            return new Result<List<MembershipClassDTO>>(true, "All membership classes retrieved successfully.", membershipClasses);
                        }
                    }
                    catch (Exception ex)
                    {
                        return new Result<List<MembershipClassDTO>>(false, "An unexpected error occurred on the server.", membershipClasses, 500);
                    }
                }
            }
        }

        public static async Task<Result<int>> AddNewMembershipClassAsync(MembershipClassDTO membershipClass)
        {
            int membershipClassID = -1;
            using (SqlConnection connection = new SqlConnection(clsDataAccessSettings.ConnectionString))
            {
                string query = @"
        INSERT INTO MembershipClasses (MembershipClassName, MaxNumberOfBooksCanBorrow, FeesPerDay)
        VALUES (@MembershipClassName, @MaxNumberOfBooksCanBorrow, @FeesPerDay);
        SELECT SCOPE_IDENTITY();
    ";
                using (SqlCommand command = new SqlCommand(query, connection))
                {
                    command.Parameters.AddWithValue("@MembershipClassName", membershipClass.MembershipClassName);
                    command.Parameters.AddWithValue("@MaxNumberOfBooksCanBorrow", membershipClass.MaxNumberOfBooksCanBorrow);
                    command.Parameters.AddWithValue("@FeesPerDay", membershipClass.FeesPerDay);
                    try
                    {
                        await connection.OpenAsync();
                        object result = await command.ExecuteScalarAsync();
                        if (result != null && int.TryParse(result.ToString(), out int id))
                        {
                            membershipClassID = id;
                        }
                        return new Result<int>(true, "Membership class added successfully", membershipClassID);
                    }
                    catch (Exception ex)
                    {
                        return new Result<int>(false, "An unexpected error occurred on the server.", membershipClassID, 500);
                    }
                }
            }
        }

        public static async Task<Result<int>> UpdateMembershipClassAsync(MembershipClassDTO membershipClass)
        {
            int rowsAffected = 0;
            using (SqlConnection connection = new SqlConnection(clsDataAccessSettings.ConnectionString))
            {
                string query = @"
        UPDATE MembershipClasses
        SET MembershipClassName = @MembershipClassName,
            MaxNumberOfBooksCanBorrow = @MaxNumberOfBooksCanBorrow,
            FeesPerDay = @FeesPerDay
        WHERE MembershipClassID = @MembershipClassID
    ";
                using (SqlCommand command = new SqlCommand(query, connection))
                {
                    command.Parameters.AddWithValue("@MembershipClassID", membershipClass.MembershipClassID);
                    command.Parameters.AddWithValue("@MembershipClassName", membershipClass.MembershipClassName);
                    command.Parameters.AddWithValue("@MaxNumberOfBooksCanBorrow", membershipClass.MaxNumberOfBooksCanBorrow);
                    command.Parameters.AddWithValue("@FeesPerDay", membershipClass.FeesPerDay);
                    try
                    {
                        await connection.OpenAsync();
                        rowsAffected = await command.ExecuteNonQueryAsync();
                        if (rowsAffected > 0)
                        {
                            return new Result<int>(true, "Membership class updated successfully.", rowsAffected);
                        }
                        return new Result<int>(false, "An unexpected error occurred on the server.", -1, 500);
                    }
                    catch (Exception ex)
                    {
                        return new Result<int>(false, "An unexpected error occurred on the server.", -1, 500);
                    }
                }
            }
        }

        public static async Task<Result<bool>> DeleteMembershipClassAsync(int membershipClassID)
        {
            int rowsAffected = 0;
            using (SqlConnection connection = new SqlConnection(clsDataAccessSettings.ConnectionString))
            {
                string query = "DELETE FROM MembershipClasses WHERE MembershipClassID = @MembershipClassID";
                using (SqlCommand command = new SqlCommand(query, connection))
                {
                    command.Parameters.AddWithValue("@MembershipClassID", membershipClassID);
                    try
                    {
                        await connection.OpenAsync();
                        rowsAffected = await command.ExecuteNonQueryAsync();
                        if (rowsAffected > 0)
                        {
                            return new Result<bool>(true, "Membership class deleted successfully.", true);
                        }
                        return new Result<bool>(false, "An unexpected error occurred on the server.", false, 500);
                    }
                    catch (Exception ex)
                    {
                        return new Result<bool>(false, "An unexpected error occurred on the server.", false, 500);
                    }
                }
            }
        }
    }
}
