using LibraryManagementSystemAPIDataAccessLayer;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Data.SqlClient;
using static SharedClasses.AuthorDTOs;
using SharedClasses;

namespace LibraryManagementSystemAPIDataAccessLayer
{
   
    public class clsAuthorData
    {
        public static async Task<Result<AuthorDTO>> GetAuthorInfoByIDAsync(int AuthorID)
        {
            using (SqlConnection connection = new SqlConnection(clsDataAccessSettings.ConnectionString))
            {
                string query = "SELECT * FROM Authors WHERE AuthorID = @AuthorID";
                using (SqlCommand command = new SqlCommand(query, connection))
                {
                    command.Parameters.AddWithValue("@AuthorID", AuthorID);
                    try
                    {
                        await connection.OpenAsync();
                        using (var reader = await command.ExecuteReaderAsync())
                        {
                            if (await reader.ReadAsync())
                            {
                                int authorID = reader.GetInt32(reader.GetOrdinal("AuthorID"));
                                int personID = reader.GetInt32(reader.GetOrdinal("PersonID"));

                                DateTime createdDate = reader.GetDateTime(reader.GetOrdinal("CreatedDate"));
                                AuthorDTO author = new AuthorDTO(authorID, personID, createdDate );
                                return new Result<AuthorDTO>(true, "Author retrieved successfully.", author);
                            }
                            else
                            {
                                return new Result<AuthorDTO>(false, "Author not found.", null, 404);
                            }
                        }
                    }
                    catch (Exception ex)
                    {
                        return new Result<AuthorDTO>(false, "An unexpected error occurred on the server.", null, 500);
                    }
                }
            }
        }

        public static async Task<Result<int>> AddNewAuthorAsync(int PersonID)
        {
            int AuthorID = -1;
            using (var connection = new SqlConnection(clsDataAccessSettings.ConnectionString))
            using (var command = new SqlCommand("sp_AddNewAuthor", connection))
            {
                command.CommandType = CommandType.StoredProcedure;
                command.Parameters.AddWithValue("@PersonID", PersonID);

                SqlParameter returnedValue = new SqlParameter("@ReturnValue", SqlDbType.Int)
                {
                    Direction = ParameterDirection.ReturnValue
                };
                command.Parameters.Add(returnedValue);

                try
                {
                    await connection.OpenAsync();
                    await command.ExecuteNonQueryAsync();
                    AuthorID = (int)returnedValue.Value;
                    if (AuthorID > 0)
                    {
                        return new Result<int>(true, "Author added successfully.", AuthorID);
                    }
                    else
                    {
                        return new Result<int>(false, "Failed to add author.", -1);
                    }
                }
                catch (Exception ex)
                {
                    return new Result<int>(false, "An unexpected error occurred on the server.", -1, 500);
                }
            }
        }

        public static async Task<Result<bool>> DeleteAuthorAsync(int AuthorID)
        {
            int rowsAffected = 0;
            using (SqlConnection connection = new SqlConnection(clsDataAccessSettings.ConnectionString))
            {
                string query = "DELETE FROM Authors WHERE AuthorID = @AuthorID";
                using (SqlCommand command = new SqlCommand(query, connection))
                {
                    command.Parameters.AddWithValue("@AuthorID", AuthorID);
                    try
                    {
                        await connection.OpenAsync();
                        rowsAffected = await command.ExecuteNonQueryAsync();
                        if (rowsAffected > 0)
                        {
                            return new Result<bool>(true, "Author deleted successfully.", true);
                        }
                        else
                        {
                            return new Result<bool>(false, "Author not found.", false, 404);
                        }
                    }
                    catch (Exception ex)
                    {
                        return new Result<bool>(false, "An unexpected error occurred on the server.", false, 500);
                    }
                }
            }
        }

        public static async Task<Result<List<AuthorViewDTO>>> GetAllAuthorsAsync()
        {
            List<AuthorViewDTO> authorList = new List<AuthorViewDTO>();
            using (SqlConnection connection = new SqlConnection(clsDataAccessSettings.ConnectionString))
            {
                string query = "SELECT * FROM Authors_View";
                using (SqlCommand command = new SqlCommand(query, connection))
                {
                    try
                    {
                        await connection.OpenAsync();
                        using (var reader = await command.ExecuteReaderAsync())
                        {
                            while (await reader.ReadAsync())
                            {
                                authorList.Add(new AuthorViewDTO
                                (
                                    reader.GetInt32(reader.GetOrdinal("AuthorID")),
                                    reader.GetString(reader.GetOrdinal("AuthorName")),
                                    reader.GetString(reader.GetOrdinal("Email")),
                                    reader.GetString(reader.GetOrdinal("Phone")),
                                    reader.GetInt32(reader.GetOrdinal("TotalBooks"))
                                ));
                            }
                        }
                        if (authorList.Count == 0)
                        {
                            return new Result<List<AuthorViewDTO>>(false, "No authors found.", authorList, 404);
                        }
                        return new Result<List<AuthorViewDTO>>(true, "Authors retrieved successfully.", authorList);
                    }
                    catch (Exception ex)
                    {
                        return new Result<List<AuthorViewDTO>>(false, "An unexpected error occurred on the server.", authorList, 500);
                    }
                }
            }
        }

        public static async Task<Result<string>> GetAuthorNameAsync(int AuthorID)
        {
            string AuthorName = string.Empty;
            using (SqlConnection connection = new SqlConnection(clsDataAccessSettings.ConnectionString))
            {
                string query = "SELECT AuthorName FROM Authors_View WHERE AuthorID = @AuthorID";
                using (SqlCommand command = new SqlCommand(query, connection))
                {
                    command.Parameters.AddWithValue("@AuthorID", AuthorID);
                    try
                    {
                        await connection.OpenAsync();
                        object result = await command.ExecuteScalarAsync();
                        if (result != null)
                        {
                            AuthorName = result.ToString();
                        }
                        return new Result<string>(true, "Name retrieved successfully.", AuthorName);
                    }
                    catch (Exception ex)
                    {
                        return new Result<string>(false, "An unexpected error occurred on the server.", "", 500);
                    }
                }
            }
        }

        public static async Task<Result<int>> GetAuthorIDAsync(string AuthorName)
        {
            int AuthorID = -1;
            using (SqlConnection connection = new SqlConnection(clsDataAccessSettings.ConnectionString))
            {
                string query = "SELECT AuthorID FROM Authors_View WHERE AuthorName = @AuthorName";
                using (SqlCommand command = new SqlCommand(query, connection))
                {
                    command.Parameters.AddWithValue("@AuthorName", AuthorName);
                    try
                    {
                        await connection.OpenAsync();
                        object result = await command.ExecuteScalarAsync();
                        if (result != null && int.TryParse(result.ToString(), out int NewAuthorID))
                        {
                            AuthorID = NewAuthorID;
                        }
                        return new Result<int>(true, "Author ID retrieved successfully.", AuthorID);
                    }
                    catch (Exception ex)
                    {
                        return new Result<int>(false, $"Database error: {ex.Message}", AuthorID);
                    }
                }
            }
        }

        public static async Task<Result<int>> GetTotalAuthorsAsync()
        {
            int TotalAuthors = 0;
            using (SqlConnection connection = new SqlConnection(clsDataAccessSettings.ConnectionString))
            {
                string query = "SELECT COUNT(AuthorID) FROM Authors";
                using (SqlCommand command = new SqlCommand(query, connection))
                {
                    try
                    {
                        await connection.OpenAsync();
                        object result = await command.ExecuteScalarAsync();
                        if (result != null && int.TryParse(result.ToString(), out int authorCount))
                        {
                            TotalAuthors = authorCount;
                        }
                        return new Result<int>(true, "Total authors retrieved successfully.", TotalAuthors);
                    }
                    catch (Exception ex)
                    {
                        return new Result<int>(false, $"Database error: {ex.Message}", TotalAuthors);
                    }
                }
            }
        }

    }

}
