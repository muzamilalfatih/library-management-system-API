using LibraryManagementSystemAPIDataAccessLayer;
using System;
using System.Collections.Generic;
using System.Data;
using Microsoft.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SharedClasses;
using Microsoft.VisualBasic;
using static SharedClasses.BorrowDTOs;

namespace LibraryManagementSystemAPIDataAccessLayer
{
    
    public class clsBorrowData
    {
        public static async Task<Result<FullBorrowDTO>> GetBorrowInfoByIDAsync(int BorrowID)
        {
            using (SqlConnection connection = new SqlConnection(clsDataAccessSettings.ConnectionString))
            {
                string query = "SELECT * FROM Borrows WHERE BorrowID = @BorrowID";
                using (SqlCommand command = new SqlCommand(query, connection))
                {
                    command.Parameters.AddWithValue("@BorrowID", BorrowID);
                    try
                    {
                        await connection.OpenAsync();
                        using (SqlDataReader reader = await command.ExecuteReaderAsync())
                        {
                            if (await reader.ReadAsync())
                            {
                                int createdByUserID = (int)reader["CreatedByUserID"];

                               Result<string> createdByUserResult = await clsUserData.GetUserNameAsync(createdByUserID);
                                if (!createdByUserResult.Success)
                                {
                                    return new Result<FullBorrowDTO>(false, createdByUserResult.Message, null, createdByUserResult.ErrorCode);
                                }
                                int? returnedByUserID = reader["ReturnedByUserID"] == DBNull.Value ? null : (int?)reader["ReturnedByUserID"];
                                Result<string> returnedByUserResult = null;
                                if (returnedByUserID != null)
                                {
                                    returnedByUserResult = await clsUserData.GetUserNameAsync((int)returnedByUserID);
                                    if (!returnedByUserResult.Success)
                                    {
                                        return new Result<FullBorrowDTO>(false, returnedByUserResult.Message, null, returnedByUserResult.ErrorCode);
                                    }
                                }
                                FullBorrowDTO borrowDTO = new FullBorrowDTO
                                (
                                     (int)reader["BorrowID"],
                                    (int)reader["MemberID"],
                                    (int)reader["CopyID"],
                                     (DateTime)reader["BorrowDate"],
                                     (DateTime)reader["DueDate"],
                                     Convert.ToSingle(reader["PaidFees"]),
                                     reader["ReturnNotes"] as string,
                                     reader["ReturnDate"] == DBNull.Value ? null : (DateTime?)reader["ReturnDate"],
                                     reader["ReturnFees"] == DBNull.Value ? null : Convert.ToSingle(reader["ReturnFees"]),
                                     createdByUserID,
                                     returnedByUserID,
                                     createdByUserResult.Data,
                                     returnedByUserResult?.Data

                                );
                                return new Result<FullBorrowDTO>(true, "Borrow found successfully.", borrowDTO);
                            }
                            return new Result<FullBorrowDTO>(false, "Borrow not found.", null, 404);
                        }
                    }
                    catch (Exception ex)
                    {
                        return new Result<FullBorrowDTO>(false, "An unexpected error occurred on the server.", null, 500);
                    }
                }
            }
        }

        public static async Task<Result<FullBorrowDTO>> GetBorrowInfoByCopyIDAsync(int CopyID)
        {
            using (SqlConnection connection = new SqlConnection(clsDataAccessSettings.ConnectionString))
            {
                string query = "Select * FROM Borrows WHERE CopyID = @CopyID  order by BorrowID desc";
                using (SqlCommand command = new SqlCommand(query, connection))
                {
                    command.Parameters.AddWithValue("@CopyID", CopyID);
                    try
                    {
                        await connection.OpenAsync();
                        using (SqlDataReader reader = await command.ExecuteReaderAsync())
                        {
                            if (await reader.ReadAsync())
                            {
                                int createdByUserID = (int)reader["CreatedByUserID"];

                                Result<string> createdByUserResult = await clsUserData.GetUserNameAsync(createdByUserID);
                                if (!createdByUserResult.Success)
                                {
                                    return new Result<FullBorrowDTO>(false, createdByUserResult.Message, null, createdByUserResult.ErrorCode);
                                }
                                int? returnedByUserID = reader["ReturnedByUserID"] == DBNull.Value ? null : (int?)reader["ReturnedByUserID"];
                                Result<string> returnedByUserResult = null;
                                if (returnedByUserID != null)
                                {
                                    returnedByUserResult = await clsUserData.GetUserNameAsync((int)returnedByUserID);
                                    if (!returnedByUserResult.Success)
                                    {
                                        return new Result<FullBorrowDTO>(false, returnedByUserResult.Message, null, returnedByUserResult.ErrorCode);
                                    }
                                }
                                FullBorrowDTO borrowDTO = new FullBorrowDTO
                                (
                                     (int)reader["BorrowID"],
                                    (int)reader["MemberID"],
                                    (int)reader["CopyID"],
                                     (DateTime)reader["BorrowDate"],
                                     (DateTime)reader["DueDate"],
                                     Convert.ToSingle(reader["PaidFees"]),
                                     reader["ReturnNotes"] as string,
                                     reader["ReturnDate"] == DBNull.Value ? null : (DateTime?)reader["ReturnDate"],
                                     reader["ReturnFees"] == DBNull.Value ? null : Convert.ToSingle(reader["ReturnFees"]),
                                     createdByUserID,
                                     returnedByUserID,
                                     createdByUserResult.Data,
                                     createdByUserResult?.Data

                                );
                                return new Result<FullBorrowDTO>(true, "Borrow found successfully.", borrowDTO);
                            }
                            return new Result<FullBorrowDTO>(false, "Borrow not found.", null, 404);
                        }
                    }
                    catch (Exception ex)
                    {
                        return new Result<FullBorrowDTO>(false, "An unexpected error occurred on the server.", null, 500);
                    }
                }
            }
        }

        public static async Task<Result<BorrowedBookDTO>> AddNewBorrowAsync(BorrowDTO borrowDTO)
        {
            using (var connection = new SqlConnection(clsDataAccessSettings.ConnectionString))
            using (var command = new SqlCommand("sp_BorrowBook", connection))
            {
                command.CommandType = CommandType.StoredProcedure;
                command.Parameters.AddWithValue("@MemberID", borrowDTO.MemberID);
                command.Parameters.AddWithValue("@HasReservation", borrowDTO.HasReserrvation);
                command.Parameters.AddWithValue("@BookID", borrowDTO.BookID);
                command.Parameters.AddWithValue("@DueDate", borrowDTO.DueDate);
                command.Parameters.AddWithValue("@PaidFees", borrowDTO.PaidFees);
                command.Parameters.AddWithValue("@CreatedByUserID", borrowDTO.CreatedByUserID);

                SqlParameter copyIDOutParam = new SqlParameter("@CopyID", SqlDbType.Int) { Direction = ParameterDirection.Output };
                SqlParameter borrowIDOutParam = new SqlParameter("@BorrowID", SqlDbType.Int) { Direction = ParameterDirection.Output };

                command.Parameters.Add(copyIDOutParam);
                command.Parameters.Add(borrowIDOutParam);

                try
                {
                    await connection.OpenAsync();
                    await command.ExecuteNonQueryAsync();
                    return new Result<BorrowedBookDTO>(true, "Book borrowed successfully.",
                        new BorrowedBookDTO((int)copyIDOutParam.Value, (int)borrowIDOutParam.Value));
                }
                catch (Exception ex)
                {
                    return new Result<BorrowedBookDTO>(false, "An unexpected error occurred on the server.", null, 500);
                }
            }
        }

        public static async Task<Result<ReturnedBookDTO>> ReturnBookAsync(ReturnBookDTO returnBook)
        {
            using (var connection = new SqlConnection(clsDataAccessSettings.ConnectionString))
            using (var command = new SqlCommand("sp_ReturnBook", connection))
            {
                command.CommandType = CommandType.StoredProcedure;

                command.Parameters.AddWithValue("@BorrowID", returnBook.BorrowID);
                command.Parameters.AddWithValue("@ReturnNote", returnBook.ReturnNotes);
                command.Parameters.AddWithValue("@IsDamaged", returnBook.IsDamaged);
                command.Parameters.AddWithValue("@ReturnedByUserID", returnBook.ReturnedByUserID);

                SqlParameter returnFeesOutParam = new SqlParameter("@TotalReturnFees", SqlDbType.Float) { Direction = ParameterDirection.Output };
                SqlParameter reservedForMemberIDOutParam = new SqlParameter("@ReservedForMemberID", SqlDbType.Int) { Direction = ParameterDirection.Output };

                command.Parameters.Add(returnFeesOutParam);
                command.Parameters.Add(reservedForMemberIDOutParam);

                try
                {
                    await connection.OpenAsync();
                    await command.ExecuteNonQueryAsync();

                    float ReturnFees = Convert.ToSingle(returnFeesOutParam.Value);
                    int ReservedForMemberID = reservedForMemberIDOutParam.Value != DBNull.Value ? (int)reservedForMemberIDOutParam.Value : -1;
                    return new Result<ReturnedBookDTO>(true, "Book returned successfully.",
                        new ReturnedBookDTO(ReturnFees, ReservedForMemberID));
                }
                catch (Exception ex)
                {
                    return new Result<ReturnedBookDTO>(false, "An unexpected error occurred on the server.", null, 500);
                }
            }
        }

        public static async Task<Result<int>> GetTotalIssuedBooksAsync()
        {
            int TotalIssuedBooks = 0;
            string query = @"select TotalIssuedBooks = count(BorrowID) from Borrows where ReturnDate is Null;";

            using (SqlConnection connection = new SqlConnection(clsDataAccessSettings.ConnectionString))
            using (SqlCommand command = new SqlCommand(query, connection))
            {
                try
                {
                    await connection.OpenAsync();
                    object result = await command.ExecuteScalarAsync();
                    if (result != null && int.TryParse(result.ToString(), out int totalBorrows))
                    {
                        TotalIssuedBooks = totalBorrows;
                    }
                    return new Result<int>(true, "Total borrows retrieved successfully.", TotalIssuedBooks);
                }
                catch (Exception ex)
                {
                    return new Result<int>(false, "An unexpected error occurred on the server.", TotalIssuedBooks, 500);
                }
            }
        }

        public static async Task<Result<int>> GetTotalBorrowedBookAsync(int MemberID)
        {
            int totalBorrowedBooks = 0;
            string query = @"select count(BorrowID) as TotalBorrowedBooks from Borrows where MemberID = @MemberID and ReturnDate is NULL";

            using (SqlConnection connection = new SqlConnection(clsDataAccessSettings.ConnectionString))
            using (SqlCommand command = new SqlCommand(query, connection))
            {
                command.Parameters.AddWithValue("@MemberID", MemberID);
                try
                {
                    await connection.OpenAsync();
                    object result = await command.ExecuteScalarAsync();
                    if (result != null && int.TryParse(result.ToString(), out int totalBorrows))
                    {
                        totalBorrowedBooks = totalBorrows;
                    }
                    return new Result<int>(true, "Total borrows retrieved successfully.", totalBorrowedBooks);
                }
                catch (Exception ex)
                {
                    return new Result<int>(false, "An unexpected error occurred on the server.", totalBorrowedBooks, 500);
                }
            }
        }

        public static async Task<Result<List<BorrowViewDTO>>> GetAllBorrowsAsync()
        {
            string query = "SELECT * FROM Borrows_View";
            List<BorrowViewDTO> allBorrows = new List<BorrowViewDTO>();
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
                            allBorrows.Add(new BorrowViewDTO
                            (
                                reader.GetInt32(reader.GetOrdinal("BorrowID")),
                                reader.GetInt32(reader.GetOrdinal("MemberID")),
                                reader.GetString(reader.GetOrdinal("MemberName")),
                                reader.GetString(reader.GetOrdinal("Title")),
                                reader.GetInt32(reader.GetOrdinal("BookCopyID")),
                                reader.GetBoolean(reader.GetOrdinal("IsReturned"))
                            ));
                        }
                        if (allBorrows.Count > 0)
                        {
                            return new Result<List<BorrowViewDTO>>(true, "All borrows retrieved successfully.", allBorrows);
                        }
                        return new Result<List<BorrowViewDTO>>(false, "No borrows found.", allBorrows, 404);
                    }
                }
                catch (Exception ex)
                {
                    return new Result<List<BorrowViewDTO>>(false, "An unexpected error occurred on the server.", allBorrows, 500);
                }
            }
        }

    }
}
