﻿using System;
using System.Collections.Generic;
using System.Data;
using Microsoft.Data.SqlClient;
using LibraryManagementSystemAPIDataAccessLayer;
using System.Data.SqlTypes;
using SharedClasses;
using static SharedClasses.PublisherDTOs;
using static SharedClasses.PersonDTOs;

namespace LibraryManagementSystemAPIDataAccessLayer
{
   
    public class clsPublisherData
    {
        public static async Task<Result<PublisherDTO>> GetPublisherInfoByIDAsync(int PublisherID)
        {
            using (SqlConnection connection = new SqlConnection(clsDataAccessSettings.ConnectionString))
            {
                string query = "SELECT * FROM Publishers WHERE PublisherID = @PublisherID";
                using (SqlCommand command = new SqlCommand(query, connection))
                {
                    command.Parameters.AddWithValue("@PublisherID", PublisherID);

                    try
                    {
                        await connection.OpenAsync();
                        using (SqlDataReader reader = await command.ExecuteReaderAsync())
                        {
                            if (reader.Read())
                            {
                                int personID = reader.GetInt32(reader.GetOrdinal("PersonID"));
                                Result<PersonDTO> result = await clsPersonData.GetPersonInfoByIDAsync(personID);
                                if (!result.Success)
                                {
                                    return new Result<PublisherDTO>(false, result.Message, null, result.ErrorCode);
                                }
                                var publisher = new PublisherDTO(
                                    reader.GetInt32(reader.GetOrdinal("PublisherID")),
                                    personID,
                                    reader.GetDateTime(reader.GetOrdinal("CreatedDate")),
                                    result.Data
                                );
                                return new Result<PublisherDTO>(true, new { success = new { header = "Success", body = "Publisher found successfully." } }, publisher);
                            }
                            else
                            {
                                return new Result<PublisherDTO>(false, new { error = new { header = "Not Found", body = "Publisher not found." } }, null, 404);
                            }
                        }
                    }
                    catch (Exception ex)
                    {
                        return new Result<PublisherDTO>(false, new { error = new { header = "Server Error", body = "An unexpected error occurred on the server." } }, null, 500);
                    }
                }
            }
        }

        public static async Task<Result<int>> AddNewPublisherAsync(int personID)
        {
            int publisherID = -1;
            using (SqlConnection connection = new SqlConnection(clsDataAccessSettings.ConnectionString))
            using (SqlCommand command = new SqlCommand("sp_AddNewPublisher", connection))
            {
                command.CommandType = CommandType.StoredProcedure;
                command.Parameters.AddWithValue("@PersonID", personID);
                SqlParameter returnValue = new SqlParameter("@ReturnValue", SqlDbType.Int)
                {
                    Direction = ParameterDirection.ReturnValue
                };
                command.Parameters.Add(returnValue);

                try
                {
                    await connection.OpenAsync();
                    await command.ExecuteNonQueryAsync();
                    publisherID = (int)returnValue.Value;
                    return new Result<int>(true, new { success = new { header = "Success", body = "Publisher added successfully" } }, publisherID);
                }
                catch (Exception ex)
                {
                    return new Result<int>(false, new { error = new { header = "Server Error", body = "An unexpected error occurred on the server." } }, -1, 500);
                }
            }
        }

        public static async Task<Result<bool>> DeletePublisherAsync(int PublisherID)
        {
            int rowsAffected = 0;
            using (SqlConnection connection = new SqlConnection(clsDataAccessSettings.ConnectionString))
            {
                string query = "DELETE FROM Publishers WHERE PublisherID = @PublisherID";
                using (SqlCommand command = new SqlCommand(query, connection))
                {
                    command.Parameters.AddWithValue("@PublisherID", PublisherID);

                    try
                    {
                        await connection.OpenAsync();
                        rowsAffected = await command.ExecuteNonQueryAsync();
                        if (rowsAffected > 0)
                        {
                            return new Result<bool>(true, new { success = new { header = "Success", body = "Publisher deleted successfully" } }, true);
                        }
                        return new Result<bool>(false, new { error = new { header = "Server Error", body = "An unexpected error occurred on the server." } }, false, 500);
                    }
                    catch (Exception ex)
                    {
                        return new Result<bool>(false, new { error = new { header = "Server Error", body = "An unexpected error occurred on the server." } }, false, 500);
                    }
                }
            }
        }

        public static async Task<Result<List<PublisherViewDTO>>> GetAllPublishersAsync()
        {
            List<PublisherViewDTO> allPublishers = new List<PublisherViewDTO>();
            using (SqlConnection connection = new SqlConnection(clsDataAccessSettings.ConnectionString))
            using (SqlCommand command = new SqlCommand("SELECT * FROM Publishers_View", connection))
            {
                try
                {
                    await connection.OpenAsync();
                    using (SqlDataReader reader = await command.ExecuteReaderAsync())
                    {
                        while (await reader.ReadAsync())
                        {
                            allPublishers.Add(new PublisherViewDTO(
                                reader.GetInt32(reader.GetOrdinal("PublisherID")),
                                reader.GetString(reader.GetOrdinal("PublisherName")),
                                reader.GetString(reader.GetOrdinal("Email")),
                                reader.GetString(reader.GetOrdinal("Phone")),
                                reader.GetInt32(reader.GetOrdinal("TotalBooks"))
                            ));
                        }
                        return new Result<List<PublisherViewDTO>>(true, new { success = new { header = "Success", body = "All publishers retrieved successfully." } }, allPublishers);
                    }
                }
                catch (Exception ex)
                {
                    return new Result<List<PublisherViewDTO>>(false, new { error = new { header = "Server Error", body = "An unexpected error occurred on the server." } }, allPublishers, 500);
                }
            }
        }

        public static async Task<Result<string>> GetPublisherNameAsync(int PublisherID)
        {
            string publisherName = string.Empty;
            using (SqlConnection connection = new SqlConnection(clsDataAccessSettings.ConnectionString))
            {
                string query = "SELECT PublisherName FROM Publishers_View WHERE PublisherID = @PublisherID";
                using (SqlCommand command = new SqlCommand(query, connection))
                {
                    command.Parameters.AddWithValue("@PublisherID", PublisherID);

                    try
                    {
                        await connection.OpenAsync();
                        publisherName = (string)await command.ExecuteScalarAsync();
                        if (publisherName != string.Empty)
                        {
                            return new Result<string>(true, new { success = new { header = "Success", body = "Publisher name retrieved successfully." } }, publisherName);
                        }
                        return new Result<string>(false, new { error = new { header = "Not Found", body = "Publisher not found.." } }, null, 404);
                    }
                    catch (Exception ex)
                    {
                        return new Result<string>(false, new { error = new { header = "Server Error", body = "An unexpected error occurred on the server." } }, publisherName, 500);
                    }
                }
            }
        }

        public static async Task<Result<int>> GetPublisherIDAsync(string PublisherName)
        {
            int publisherID = -1;
            using (SqlConnection connection = new SqlConnection(clsDataAccessSettings.ConnectionString))
            {
                string query = "SELECT PublisherID FROM Publishers_View WHERE PublisherName = @PublisherName";
                using (SqlCommand command = new SqlCommand(query, connection))
                {
                    command.Parameters.AddWithValue("@PublisherName", PublisherName);

                    try
                    {
                        await connection.OpenAsync();
                        publisherID = (int)await command.ExecuteScalarAsync();
                        return new Result<int>(true, new { success = new { header = "Success", body = "Publisher ID retrieved successfully." } }, publisherID);
                    }
                    catch (Exception ex)
                    {
                        return new Result<int>(false, new { error = new { header = "Server Error", body = "An unexpected error occurred on the server." } }, -1, 500);
                    }
                }
            }
        }

        public static async Task<Result<int>> GetTotalPublishersAsync()
        {
            int totalPublishers = 0;
            using (SqlConnection connection = new SqlConnection(clsDataAccessSettings.ConnectionString))
            {
                string query = "SELECT COUNT(PublisherID) FROM Publishers";
                using (SqlCommand command = new SqlCommand(query, connection))
                {
                    try
                    {
                        await connection.OpenAsync();
                        totalPublishers = (int)await command.ExecuteScalarAsync();
                        return new Result<int>(true, new { success = new { header = "Success", body = "Total publishers count retrieved successfully." } }, totalPublishers);
                    }
                    catch (Exception ex)
                    {
                        return new Result<int>(false, new { error = new { header = "Server Error", body = "An unexpected error occurred on the server." } }, -1, 500);
                    }
                }
            }
        }
    }

}
