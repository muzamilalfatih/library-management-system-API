using System;
using System.Collections.Generic;
using Microsoft.Data.SqlClient;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using LibraryManagementSystemAPIDataAccessLayer;
using System.Net;
using static System.Runtime.InteropServices.JavaScript.JSType;
using SharedClasses;
using static SharedClasses.BookCopyDTOs;
using static SharedClasses.BookCategoryDTOs;

namespace LibraryManagementSystemAPIDataAccessLayer
{
    
    public class clsBookCopyData
    {
        public static async Task<Result<BookCopyDTO>> GetBookCopyInfoByIDAsync(int BookCopyID)
        {
            using (SqlConnection connection = new SqlConnection(clsDataAccessSettings.ConnectionString))
            {
                string query = "SELECT * FROM BookCopies WHERE BookCopyID = @BookCopyID";
                using (SqlCommand command = new SqlCommand(query, connection))
                {
                    command.Parameters.AddWithValue("@BookCopyID", BookCopyID);
                    try
                    {
                        await connection.OpenAsync();
                        using (var reader = await command.ExecuteReaderAsync())
                        {
                            if (await reader.ReadAsync())
                            {
                                BookCopyDTO bookCopy = new BookCopyDTO(
                                    reader.GetInt32(reader.GetOrdinal("BookCopyID")),
                                    reader.GetInt32(reader.GetOrdinal("BookID")),
                                    reader.IsDBNull(reader.GetOrdinal("ReservedForMemberID")) ? -1 : reader.GetInt32("ReservedForMemberID"),
                                    reader.GetBoolean(reader.GetOrdinal("IsAvailable")),
                                    reader.GetBoolean(reader.GetOrdinal("IsDamaged"))
                                );
                                return new Result<BookCopyDTO>(true, "Copy found successfully", bookCopy);
                            }
                            else
                            {
                                return new Result<BookCopyDTO>(false, "Copy not found.", null, 404);
                            }
                        }
                    }
                    catch (Exception ex)
                    {
                        return new Result<BookCopyDTO>(false, "An unexpected error occurred on the server.", null, 500);
                    }
                }
            }
        }

        public static async Task<Result<List<BookCopyDTO>>> GetAllCopiesAsync(int BookID)
        {
            string query = "SELECT * FROM BookCopies WHERE BookID = @BookID";
            List<BookCopyDTO> allCopies = new List<BookCopyDTO>();
            using (SqlConnection connection = new SqlConnection(clsDataAccessSettings.ConnectionString))
            using (SqlCommand command = new SqlCommand(query, connection))
            {
                command.Parameters.AddWithValue("@BookID", BookID);
                try
                {
                    await connection.OpenAsync();
                    using (SqlDataReader reader = await command.ExecuteReaderAsync())
                    {
                        while (await reader.ReadAsync())
                        {
                            allCopies.Add(new BookCopyDTO(
                                reader.GetInt32(reader.GetOrdinal("BookCopyID")),
                                reader.GetInt32(reader.GetOrdinal("BookID")),
                                reader.IsDBNull(reader.GetOrdinal("ReservedForMemberID")) ? -1 : reader.GetInt32("ReservedForMemberID"),
                                reader.GetBoolean(reader.GetOrdinal("IsAvailable")),
                                reader.GetBoolean(reader.GetOrdinal("IsDamaged"))
                            ));
                        }
                    }
                    if (allCopies.Count == 0)
                    {
                        return new Result<List<BookCopyDTO>>(false, "No books found.", allCopies, 404);
                    }
                    return new Result<List<BookCopyDTO>>(true, "Books retrieved successfully.", allCopies);
                }
                catch (Exception ex)
                {
                    return new Result<List<BookCopyDTO>>(false, "An unexpected error occurred on the server.", allCopies, 500);
                }
            }
        }

        public static async Task<Result<int>> AddNewBookCopyAsync(BookCopyDTO bookCopyDTO)
        {
            string query = @"
        INSERT INTO BookCopies
        (BookID, IsAvailable, IsDamaged)
        VALUES
        (@BookID, @IsAvailable, @IsDamaged);
        SELECT SCOPE_IDENTITY();";
            using (SqlConnection connection = new SqlConnection(clsDataAccessSettings.ConnectionString))
            using (SqlCommand command = new SqlCommand(query, connection))
            {
                command.Parameters.AddWithValue("@BookID", bookCopyDTO.BookID);
                command.Parameters.AddWithValue("@IsAvailable", bookCopyDTO.IsAvailabe);
                command.Parameters.AddWithValue("@IsDamaged", bookCopyDTO.IsDamaged);
                try
                {
                    await connection.OpenAsync();
                    object result = await command.ExecuteScalarAsync();

                    if (result != null && int.TryParse(result.ToString(), out int newBookCopyID))
                    {
                        return new Result<int>(true, "Copy added successfully.", newBookCopyID);
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

        public static async Task<Result<List<int>>> InsertNumberOfCopiesForBookIDAsync(InsertCopiesDTO insertCopiesDTO)
        {
            List<int> reservationsList = new List<int>();
            string storedProcedureName = @"sp_AddNumberOfCopies";
            using (SqlConnection connection = new SqlConnection(clsDataAccessSettings.ConnectionString))
            using (SqlCommand command = new SqlCommand(storedProcedureName, connection))
            {
                command.CommandType = CommandType.StoredProcedure;
                command.Parameters.AddWithValue("@BookID", insertCopiesDTO.BookID);
                command.Parameters.AddWithValue("@NumberOFCopies", insertCopiesDTO.NumberOFCopies);
                SqlParameter outPutParameter = new SqlParameter("@RowAffected", SqlDbType.Int) { Direction = ParameterDirection.Output };
                command.Parameters.Add(outPutParameter);

                try
                {
                    await connection.OpenAsync();
                    using (SqlDataReader reader = await command.ExecuteReaderAsync())
                    {
                        while (await reader.ReadAsync())
                        {
                            reservationsList.Add(reader.GetInt32(reader.GetOrdinal("MemberID")));
                        }
                    }
                    if ((int)outPutParameter.Value > 0)
                    {
                        return new Result<List<int>>(true, "Copies inserted successfully.", reservationsList);
                    }
                    else
                    {
                        return new Result<List<int>>(false, "An unexpected error occurred on the server.", reservationsList, 500);
                    }
                }
                catch (Exception ex)
                {
                    return new Result<List<int>>(false, "An unexpected error occurred on the server.", reservationsList, 500);
                }
            }
        }
        public static async Task<Result<bool>> ISReservationAvailableForBookIDAsync(IsReservationAvailabeDTO IsReservationAvailabeDTO)
        {
            bool IsFound = false;

            using (SqlConnection connection = new SqlConnection(clsDataAccessSettings.ConnectionString))
            {
                string query = @"select top 1 Found = 1 from BookCopies where BookID = @bookID and ReservedForMemberID = @memberID";

                using (SqlCommand command = new SqlCommand(query, connection))
                {
                    command.Parameters.AddWithValue("@memberID", IsReservationAvailabeDTO.memberID);
                    command.Parameters.AddWithValue("@bookID", IsReservationAvailabeDTO.bookID);
                    try
                    {
                        await connection.OpenAsync();
                        using (SqlDataReader reader = await command.ExecuteReaderAsync())
                        {
                            IsFound = reader.HasRows;
                        }
                        return new Result<bool>(true, "Reservation check complete.", IsFound);
                    }
                    catch (Exception ex)
                    {
                        return new Result<bool>(false, "An unexpected error occurred on the server.", IsFound, 500);
                    }
                }
            }
        }

        public static async Task<Result<int>> TotalCopiesAsync(int BookID)
        {
            int totalCopies = 0;

            using (SqlConnection connection = new SqlConnection(clsDataAccessSettings.ConnectionString))
            {
                string query = @" Select TotalCopies = Count(BookCopyID) from BookCopies where BookID = @BookID";

                using (SqlCommand command = new SqlCommand(query, connection))
                {
                    command.Parameters.AddWithValue("@BookID", BookID);
                    try
                    {
                        await connection.OpenAsync();
                        object result = await command.ExecuteScalarAsync();

                        if (result != null && int.TryParse(result.ToString(), out int Copies))
                        {
                            totalCopies = Copies;
                        }
                        return new Result<int>(true, "Total copies received successfully!", totalCopies);
                    }
                    catch (Exception ex)
                    {
                        return new Result<int>(false, "An unexpected error occurred on the server.", -1, 500);
                    }
                }
            }
        }

        public static async Task<Result<int>> AvailableCopiesAsync(int BookID)
        {
            int availableCopies = 0;

            using (SqlConnection connection = new SqlConnection(clsDataAccessSettings.ConnectionString))
            {
                string query = @" Select TotalCopies = Count(BookCopyID) from BookCopies 
                          where BookID = @BookID and IsAvailable = 1 and ReservedForMemberID is NULL and IsDamaged = 0";

                using (SqlCommand command = new SqlCommand(query, connection))
                {
                    command.Parameters.AddWithValue("@BookID", BookID);
                    try
                    {
                        await connection.OpenAsync();
                        object result = await command.ExecuteScalarAsync();

                        if (result != null && int.TryParse(result.ToString(), out int Copies))
                        {
                            availableCopies = Copies;
                        }
                        return new Result<int>(true, "Available copies received successfully", availableCopies);
                    }
                    catch (Exception ex)
                    {
                        return new Result<int>(false, "An unexpected error occurred on the server.", -1, 500);
                    }
                }
            }
        }

        public static async Task<Result<int>> GetTotalBooksAsync()
        {
            int totalBooks = 0;

            using (SqlConnection connection = new SqlConnection(clsDataAccessSettings.ConnectionString))
            {
                string query = @"select TotalBooks = count(BookCopyID) from BookCopies";

                using (SqlCommand command = new SqlCommand(query, connection))
                {
                    try
                    {
                        await connection.OpenAsync();
                        object result = await command.ExecuteScalarAsync();

                        if (result != null && int.TryParse(result.ToString(), out int Copies))
                        {
                            totalBooks = Copies;
                        }
                        return new Result<int>(true, "Total received successfully", totalBooks);
                    }
                    catch (Exception ex)
                    {
                        return new Result<int>(false, "An unexpected error occurred on the server.", -1, 500);
                    }
                }
            }
        }

        public static async Task<Result<int>> RepairBookCopyAsync(RepairCopyDTO repairCopyDTO)
        {
            int rowsAffected = 0;

            using (SqlConnection connection = new SqlConnection(clsDataAccessSettings.ConnectionString))
            {
                string procedureName = @"sp_RepairBookCopy";

                using (SqlCommand command = new SqlCommand(procedureName, connection))
                {
                    command.CommandType = CommandType.StoredProcedure;
                    command.Parameters.AddWithValue("@CopyID", repairCopyDTO.bookCopyID);
                    command.Parameters.AddWithValue("@Description", repairCopyDTO.Description);
                    command.Parameters.AddWithValue("@Date", repairCopyDTO.Date);
                    command.Parameters.AddWithValue("@Cost", repairCopyDTO.Cost);
                    SqlParameter returnedValue = new SqlParameter();
                    returnedValue.Direction = ParameterDirection.ReturnValue;
                    SqlParameter reservedForMemberID = new SqlParameter("@ReservedForMemberID", SqlDbType.Int)
                    {
                        Direction = ParameterDirection.Output
                    };
                    command.Parameters.Add(returnedValue);
                    command.Parameters.Add(reservedForMemberID);

                    try
                    {
                        await connection.OpenAsync();
                        rowsAffected = await command.ExecuteNonQueryAsync();

                        if (rowsAffected > 0)
                        {
                            return new Result<int>(true, "Copy repaired successfully.",
                                reservedForMemberID.Value == DBNull.Value ? -1 :
                                Convert.ToInt32(reservedForMemberID.Value));
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
        }

    }
}
