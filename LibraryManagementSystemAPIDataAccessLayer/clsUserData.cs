using System;
using System.Collections.Generic;
using System.Data;
using Microsoft.Data.SqlClient;
using LibraryManagementSystemAPIDataAccessLayer;
using System.Text.Json.Serialization; // Assuming this namespace
using static SharedClasses.UserDTOS;
using SharedClasses;
using static SharedClasses.BookDTOs;
namespace LibraryManagementSystemAPIDataAccessLayer
{
    public class clsUserData
    {
        public static async Task<Result<UserDTO>> GetUserInfoByUserIDAsync(int userID)
        {
            using (SqlConnection connection = new SqlConnection(clsDataAccessSettings.ConnectionString))
            {
                string query = "SELECT * FROM Users WHERE UserID = @UserID";
                using (SqlCommand command = new SqlCommand(query, connection))
                {
                    command.Parameters.AddWithValue("@UserID", userID);
                    try
                    {
                        await connection.OpenAsync();
                        using (SqlDataReader reader = await command.ExecuteReaderAsync())
                        {
                            if (reader.Read())
                            {
                                int personID = reader.GetInt32(reader.GetOrdinal("PersonID"));
                                var user = new UserDTO(
                                    reader.GetInt32(reader.GetOrdinal("UserID")),
                                    personID,
                                    reader.GetString(reader.GetOrdinal("UserName")),
                                    reader.GetString(reader.GetOrdinal("Password")),
                                    reader.GetInt32(reader.GetOrdinal("UserRoles")),
                                    reader.GetBoolean(reader.GetOrdinal("IsActive"))
                                );
                                return new Result<UserDTO>(true, "User found successfully.", user);
                            }
                            else
                            {
                                return new Result<UserDTO>(false, "User not found.", null, 404);
                            }
                        }
                    }
                    catch (Exception ex)
                    {
                        return new Result<UserDTO>(false, "An unexpected error occurred on the server.", null, 500);
                    }
                }
            }
        }

        public static async Task<Result<UserDTO>> GetUserInfoByUserNameAsync(string userName)
        {
            using (SqlConnection connection = new SqlConnection(clsDataAccessSettings.ConnectionString))
            {
                string query = "SELECT * FROM Users WHERE UserName = @UserName";
                using (SqlCommand command = new SqlCommand(query, connection))
                {
                    command.Parameters.AddWithValue("@UserName", userName);
                    try
                    {
                        await connection.OpenAsync();
                        using (SqlDataReader reader = await command.ExecuteReaderAsync())
                        {
                            if (reader.Read())
                            {
                                int personID = reader.GetInt32(reader.GetOrdinal("PersonID"));
                                var user = new UserDTO(
                                    reader.GetInt32(reader.GetOrdinal("UserID")),
                                    personID,
                                    reader.GetString(reader.GetOrdinal("UserName")),
                                    reader.GetString(reader.GetOrdinal("Password")),
                                    reader.GetInt32(reader.GetOrdinal("UserRoles")),
                                    reader.GetBoolean(reader.GetOrdinal("IsActive"))
                                );
                                return new Result<UserDTO>(true, "User found successfully.", user);
                            }
                            else
                            {
                                return new Result<UserDTO>(false, "User not found.", null, 404);
                            }
                        }
                    }
                    catch (Exception ex)
                    {
                        return new Result<UserDTO>(false, "An unexpected error occurred on the server.", null, 500);
                    }
                }
            }
        }

        public static async Task<Result<int>> AddNewUserAsync(UserDTO userDTO)
        {
            int userID = -1;
            using (SqlConnection connection = new SqlConnection(clsDataAccessSettings.ConnectionString))
            using (SqlCommand command = new SqlCommand("sp_AddNewUser", connection))
            {
                command.CommandType = CommandType.StoredProcedure;
                command.Parameters.AddWithValue("@PersonID", userDTO.PersonID);
                command.Parameters.AddWithValue("@UserName", userDTO.UserName);
                command.Parameters.AddWithValue("@Password", userDTO.Password);
                command.Parameters.AddWithValue("@UserRoles", userDTO.Role);
                command.Parameters.AddWithValue("@IsActive", userDTO.IsActive);
                SqlParameter returnedValue = new SqlParameter("@ReturnValue", SqlDbType.Int)
                {
                    Direction = ParameterDirection.ReturnValue
                };
                command.Parameters.Add(returnedValue);

                try
                {
                    await connection.OpenAsync();
                    await command.ExecuteNonQueryAsync();
                    userID = (int)returnedValue.Value;
                    if (userID > 0)
                    {
                        return new Result<int>(true, "User added successfully.", userID);
                    }
                    else
                    {
                        return new Result<int>(false, "An unexpected error occurred on the server.", -1, 500);
                    }
                }
                catch (Exception ex)
                {
                    return new Result<int>(false, "An unexpected error occurred on the server.", -1, 500);
                }
            }
        }

        public static async Task<Result<int>> UpdateUserAsync(UserDTO userDTO)
        {
            int rowsAffected = 0;
            using (SqlConnection connection = new SqlConnection(clsDataAccessSettings.ConnectionString))
            {
                string query = @"UPDATE Users SET 
                          PersonID = @PersonID, 
                          UserName = @UserName, 
                          Password = @Password, 
                          UserRoles = @UserRoles, 
                          IsActive = @IsActive
                          WHERE UserID = @UserID";

                using (SqlCommand command = new SqlCommand(query, connection))
                {
                    command.Parameters.AddWithValue("@UserID", userDTO.UserID);
                    command.Parameters.AddWithValue("@PersonID", userDTO.PersonID);
                    command.Parameters.AddWithValue("@UserName", userDTO.UserName);
                    command.Parameters.AddWithValue("@Password", userDTO.Password);
                    command.Parameters.AddWithValue("@UserRoles", userDTO.Role);
                    command.Parameters.AddWithValue("@IsActive", userDTO.IsActive);

                    try
                    {
                        await connection.OpenAsync();
                        rowsAffected = await command.ExecuteNonQueryAsync();
                        if (rowsAffected > 0)
                        {
                            return new Result<int>(true, "User updated successfully.", rowsAffected);
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

        public static async Task<Result<bool>> DeleteUserAsync(int userID)
        {
            int rowsAffected = 0;
            using (SqlConnection connection = new SqlConnection(clsDataAccessSettings.ConnectionString))
            {
                string query = "DELETE FROM Users WHERE UserID = @UserID";
                using (SqlCommand command = new SqlCommand(query, connection))
                {
                    command.Parameters.AddWithValue("@UserID", userID);
                    try
                    {
                        await connection.OpenAsync();
                        rowsAffected = await command.ExecuteNonQueryAsync();
                        if (rowsAffected > 0)
                        {
                            return new Result<bool>(true, "User deleted successfully.", true);
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

        public static async Task<Result<List<UserViewDTO>>> GetAllUsersAsync()
        {
            List<UserViewDTO> allUsers = new List<UserViewDTO>();
            using (SqlConnection connection = new SqlConnection(clsDataAccessSettings.ConnectionString))
            using (SqlCommand command = new SqlCommand("SELECT * FROM Users_View", connection))
            {
                try
                {
                    await connection.OpenAsync();
                    using (SqlDataReader reader = await command.ExecuteReaderAsync())
                    {
                        while (await reader.ReadAsync())
                        {
                            allUsers.Add(new UserViewDTO(
                                reader.GetInt32(reader.GetOrdinal("UserID")),
                                reader.GetString(reader.GetOrdinal("FullName")),
                                reader.GetString(reader.GetOrdinal("UserName")),
                                reader.GetString(reader.GetOrdinal("UserRols")),
                                reader.GetBoolean(reader.GetOrdinal("IsActive"))
                            ));
                        }
                    }
                    return new Result<List<UserViewDTO>>(true, "Users retrieved successfully.", allUsers);
                }
                catch (Exception ex)
                {
                    return new Result<List<UserViewDTO>>(false, "An unexpected error occurred on the server.", allUsers, 500);
                }
            }
        }

        public static async Task<Result<bool>> IsUserExistAsync(int userID)
        {
            bool isFound = false;
            using (SqlConnection connection = new SqlConnection(clsDataAccessSettings.ConnectionString))
            {
                string query = "SELECT UserID FROM Users WHERE UserID = @UserID";
                using (SqlCommand command = new SqlCommand(query, connection))
                {
                    command.Parameters.AddWithValue("@UserID", userID);
                    try
                    {
                        await connection.OpenAsync();
                        using (SqlDataReader reader = await command.ExecuteReaderAsync())
                        {
                            isFound = reader.HasRows;
                        }
                        return new Result<bool>(true, "User existence check completed.", isFound);
                    }
                    catch (Exception ex)
                    {
                        return new Result<bool>(false, "An unexpected error occurred on the server.", false, 500);
                    }
                }
            }
        }

        public static async Task<Result<bool>> IsUserExistAsync(string UserName)
        {
            bool isFound = false;
            using (SqlConnection connection = new SqlConnection(clsDataAccessSettings.ConnectionString))
            {
                string query = "SELECT UserID FROM Users WHERE UserName = @UserName";
                using (SqlCommand command = new SqlCommand(query, connection))
                {
                    command.Parameters.AddWithValue("@UserName", UserName);
                    try
                    {
                        await connection.OpenAsync();
                        using (SqlDataReader reader = await command.ExecuteReaderAsync())
                        {
                            isFound = reader.HasRows;
                        }
                        return new Result<bool>(true, "User existence check completed.", isFound);
                    }
                    catch (Exception ex)
                    {
                        return new Result<bool>(false, "An unexpected error occurred on the server.", false, 500);
                    }
                }
            }
        }

        public static async Task<Result<int>> GetTotalUsersAsync()
        {
            int TotalUsers = 0;
            string query = @"select TotalUsers = Count(UserID) from Users";

            using (SqlConnection connection = new SqlConnection(clsDataAccessSettings.ConnectionString))
            using (SqlCommand command = new SqlCommand(query, connection))
            {
                try
                {
                    await connection.OpenAsync();
                    object result = await command.ExecuteScalarAsync();
                    if (result != null && int.TryParse(result.ToString(), out int UsersCount))
                    {
                        TotalUsers = UsersCount;
                    }
                    return new Result<int>(true, "Total users retrieved successfully.", TotalUsers);
                }
                catch (Exception ex)
                {
                    return new Result<int>(false, "An unexpected error occurred on the server.", -1, 500);
                }
            }
        }

        public static async Task<Result<bool>> IsThereOtherAdminsAsync(int CurrentUserID)
        {
            bool isFound = false;
            using (SqlConnection connection = new SqlConnection(clsDataAccessSettings.ConnectionString))
            {
                string query = @"select top 1 Found = 1 from Users where not UserID = @CurrentUserID and  UserRoles = 0 and IsActive = 1";
                using (SqlCommand command = new SqlCommand(query, connection))
                {
                    command.Parameters.AddWithValue("@CurrentUserID", CurrentUserID);
                    try
                    {
                        await connection.OpenAsync();
                        using (SqlDataReader reader = await command.ExecuteReaderAsync())
                        {
                            isFound = reader.HasRows;
                        }
                        return new Result<bool>(true, "Other admins check complete.", isFound);
                    }
                    catch (Exception ex)
                    {
                        return new Result<bool>(false, "An unexpected error occurred on the server.", false, 500);
                    }
                }
            }
        }

        public static async Task<Result<bool>> ChangePasswordAsync(ChangePasswordDTO changePasswordDTO)
        {
            int rowsAffected = 0;
            using (SqlConnection connection = new SqlConnection(clsDataAccessSettings.ConnectionString))
            {
                string query = @"Update  Users  
                     set   Password = @Password                               
                         where UserID = @UserID";

                using (SqlCommand command = new SqlCommand(query, connection))
                {
                    command.Parameters.AddWithValue("@UserID", changePasswordDTO.UserID);
                    command.Parameters.AddWithValue("@Password", changePasswordDTO.Password);

                    try
                    {
                        await connection.OpenAsync();
                        rowsAffected = await command.ExecuteNonQueryAsync();
                        if (rowsAffected > 0)
                        {
                            return new Result<bool>(true, "Password updated successfully.", true);
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
        public static async Task<Result<string>> GetUserNameAsync(int userID)
        {
            string AuthorName = string.Empty;
            using (SqlConnection connection = new SqlConnection(clsDataAccessSettings.ConnectionString))
            {
                string query = "select username from Users where UserID = @userID";
                using (SqlCommand command = new SqlCommand(query, connection))
                {
                    command.Parameters.AddWithValue("@userID", userID);
                    try
                    {
                        await connection.OpenAsync();
                        object result = await command.ExecuteScalarAsync();
                        if (result != null)
                        {
                            AuthorName = result.ToString();
                        }
                        return new Result<string>(true, "UserName retrieved successfully.", AuthorName);
                    }
                    catch (Exception ex)
                    {
                        return new Result<string>(false, "An unexpected error occurred on the server.", "", 500);
                    }
                }
            }
        }
    }

}
