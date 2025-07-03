using LibraryManagementSystemAPIDataAccessLayer;
using System;
using System.Data;
using System.Data.SqlClient;
using Microsoft.Data.SqlClient;
using SharedClasses;
using static SharedClasses.BookCategoryDTOs;
using static SharedClasses.AuthorDTOs;

namespace LibraryManagementSystemAPIDataAccessLayer
{
    

    public class clsBookCategoryData
    {
        public static async Task<Result<BookCategoryDTO>> GetCategoryInfoByIDAsync(int categoryID)
        {
            using (SqlConnection connection = new SqlConnection(clsDataAccessSettings.ConnectionString))
            {
                string query = "SELECT * FROM BookCategories WHERE CategoryID = @CategoryID";
                using (SqlCommand command = new SqlCommand(query, connection))
                {
                    command.Parameters.AddWithValue("@CategoryID", categoryID);
                    try
                    {
                        await connection.OpenAsync();
                        using (SqlDataReader reader = await command.ExecuteReaderAsync())
                        {
                            if (await reader.ReadAsync())
                            {
                                var category = new BookCategoryDTO(
                                    reader.GetInt32(reader.GetOrdinal("CategoryID")),
                                    reader.GetString(reader.GetOrdinal("CategoryName"))
                                );
                                return new Result<BookCategoryDTO>(true, "Category found successfully.", category);
                            }
                            return new Result<BookCategoryDTO>(false, "Category not found.", null, 404);
                        }
                    }
                    catch (Exception ex)
                    {
                        return new Result<BookCategoryDTO>(false, "An unexpected error occurred on the server.", null, 500);
                    }
                }
            }
        }

        public static async Task<Result<int>> GetCategoryIDByNameAsync(string categoryName)
        {
            using (var connection = new SqlConnection(clsDataAccessSettings.ConnectionString))
            {
                string query = "SELECT CategoryID FROM BookCategories WHERE CategoryName = @CategoryName";
                using (var command = new SqlCommand(query, connection))
                {
                    command.Parameters.AddWithValue("@CategoryName", categoryName);
                    try
                    {
                        await connection.OpenAsync();
                        object result = await command.ExecuteScalarAsync();
                        if (result != null)
                        {
                            return new Result<int>(true, "Category ID retrieved successfully.", Convert.ToInt32(result));
                        }
                        return new Result<int>(false, "Category not found.", -1, 404);
                    }
                    catch (Exception ex)
                    {
                        return new Result<int>(false, "An unexpected error occurred on the server.", -1, 500);
                    }
                }
            }
        }

        public static async Task<Result<string>> GetCategoryNameByIDAsync(int categoryID)
        {
            using (var connection = new SqlConnection(clsDataAccessSettings.ConnectionString))
            {
                string query = "SELECT CategoryName FROM BookCategories WHERE CategoryID = @CategoryID";
                using (var command = new SqlCommand(query, connection))
                {
                    command.Parameters.AddWithValue("@CategoryID", categoryID);
                    try
                    {
                        await connection.OpenAsync();
                        object result = await command.ExecuteScalarAsync();
                        if (result != null)
                        {
                            return new Result<string>(true, "Category name retrieved successfully.", result.ToString());
                        }
                        return new Result<string>(false, "Category not found.", "", 404);
                    }
                    catch (Exception ex)
                    {
                        return new Result<string>(false, "An unexpected error occurred on the server.", "", 500);
                    }
                }
            }
        }

        public static async Task<Result<int>> AddNewCategoryAsync(string categoryName)
        {
            using (var connection = new SqlConnection(clsDataAccessSettings.ConnectionString))
            using (var command = new SqlCommand("sp_AddNewCategory", connection))
            {
                command.CommandType = CommandType.StoredProcedure;
                command.Parameters.AddWithValue("@CategoryName", categoryName);

                var returnValue = new SqlParameter("@ReturnValue", SqlDbType.Int)
                {
                    Direction = ParameterDirection.ReturnValue
                };
                command.Parameters.Add(returnValue);

                try
                {
                    await connection.OpenAsync();
                    await command.ExecuteNonQueryAsync();
                    int categoryId = (int)returnValue.Value;
                    if (categoryId > 0)
                    {
                        return new Result<int>(true, "Category added successfully", categoryId);
                    }
                    return new Result<int>(false, "An unexpected error occurred on the server.", -1, 500);
                }
                catch (Exception ex)
                {
                    return new Result<int>(false, "An unexpected error occurred on the server.", -1, 500);
                }
            }
        }

        public static async Task<Result<int>> UpdateCategoryAsync(BookCategoryDTO category)
        {
            using (var connection = new SqlConnection(clsDataAccessSettings.ConnectionString))
            {
                string query = @"
        UPDATE BookCategories 
        SET CategoryName = @CategoryName 
        WHERE CategoryID = @CategoryID";
                using (var command = new SqlCommand(query, connection))
                {
                    command.Parameters.AddWithValue("@CategoryID", category.CategoryID);
                    command.Parameters.AddWithValue("@CategoryName", category.CategoryName);

                    try
                    {
                        await connection.OpenAsync();
                        int rowsAffected = await command.ExecuteNonQueryAsync();
                        if (rowsAffected > 0)
                        {
                            return new Result<int>(true, "Category updated successfully.", rowsAffected);
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

        public static async Task<Result<bool>> DeleteCategoryAsync(int categoryID)
        {
            using (var connection = new SqlConnection(clsDataAccessSettings.ConnectionString))
            {
                string query = "DELETE FROM BookCategories WHERE CategoryID = @CategoryID";
                using (var command = new SqlCommand(query, connection))
                {
                    command.Parameters.AddWithValue("@CategoryID", categoryID);
                    try
                    {
                        await connection.OpenAsync();
                        int rowsAffected = await command.ExecuteNonQueryAsync();
                        if (rowsAffected > 0)
                        {
                            return new Result<bool>(true, "Category deleted successfully.", true);
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

        public static async Task<Result<List<BookCategoryDTO>>> GetAllCategoriesAsync()
        {
            string query = "SELECT * FROM BookCategories";
            List<BookCategoryDTO> allCategories = new List<BookCategoryDTO>();
            using (SqlConnection connection = new SqlConnection(clsDataAccessSettings.ConnectionString))
            using (SqlCommand command = new SqlCommand(query, connection))
            {
                try
                {
                    await connection.OpenAsync();
                    using (SqlDataReader reader = await command.ExecuteReaderAsync())
                    {
                        while (await reader.ReadAsync())
                        {
                            allCategories.Add(new BookCategoryDTO(
                                reader.GetInt32(reader.GetOrdinal("CategoryID")),
                                reader.GetString(reader.GetOrdinal("CategoryName"))
                            ));
                        }
                    }
                    return new Result<List<BookCategoryDTO>>(true, "Categories retrieved successfully.", allCategories);
                }
                catch (Exception ex)
                {
                    return new Result<List<BookCategoryDTO>>(false, "An unexpected error occurred on the server.", allCategories, 500);
                }
            }
        }

    }

}
