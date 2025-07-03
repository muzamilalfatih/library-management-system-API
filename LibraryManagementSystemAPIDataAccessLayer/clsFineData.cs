using LibraryManagementSystemAPIDataAccessLayer;
using System;
using System.Collections.Generic;
using System.Data;
using Microsoft.Data.SqlClient;
using SharedClasses;
using static SharedClasses.FineDTOs;
using static SharedClasses.BookDTOs;
using static SharedClasses.BorrowDTOs;

namespace LibraryManagementSystemAPIDataAccessLayer
{
    

    public class clsFineData
    {
        public static async Task<Result<FullFineDTO>> GetFineInfoByIDAsync(int fineID)
        {
            using (SqlConnection connection = new SqlConnection(clsDataAccessSettings.ConnectionString))
            {
                string query = "SELECT * FROM Fines WHERE FineID = @FineID";
                using (SqlCommand command = new SqlCommand(query, connection))
                {
                    command.Parameters.AddWithValue("@FineID", fineID);
                    try
                    {
                        await connection.OpenAsync();
                        using (SqlDataReader reader = await command.ExecuteReaderAsync())
                        {
                            if (reader.Read())
                            {
                                var fine = new FullFineDTO(
                                    reader.GetInt32(reader.GetOrdinal("FineID")),
                                    reader.GetInt32(reader.GetOrdinal("MemberID")),
                                    reader.GetInt32(reader.GetOrdinal("BorrowID")),
                                    Convert.ToSingle(reader.GetDecimal(reader.GetOrdinal("FineAmount"))),
                                    Convert.ToSingle(reader.GetDecimal(reader.GetOrdinal("PaidAmount"))),
                                    reader.GetBoolean(reader.GetOrdinal("IsPaid")),
                                    reader.GetDateTime(reader.GetOrdinal("FineDate")),
                                    reader.GetInt32(reader.GetOrdinal("Reason")) == 0 ? "Over Due Date" : "Damaged Book"
                                );
                                return new Result<FullFineDTO>(true, "Fine found successfully.", fine);
                            }
                            else
                            {
                                return new Result<FullFineDTO>(false, "Fine not found..", null, 404);
                            }
                        }
                    }
                    catch (Exception ex)
                    {
                        return new Result<FullFineDTO>(false, "An unexpected error occurred on the server.", null, 500);
                    }
                }
            }
        }

        public static async Task<Result<bool>> PayFineAsync(PayFineDTO payFineDTO)
        {
            using (SqlConnection connection = new SqlConnection(clsDataAccessSettings.ConnectionString))
            using (SqlCommand command = new SqlCommand("sp_PayFine", connection))
            {
                command.CommandType = CommandType.StoredProcedure;
                command.Parameters.AddWithValue("@FineID", payFineDTO.fineID);
                command.Parameters.AddWithValue("@PaidByUserID", payFineDTO.paidByUserID);
                command.Parameters.AddWithValue("@PaidAmount", payFineDTO.paidAmount);

                SqlParameter returnedValue = new SqlParameter("@ReturnValue", SqlDbType.Int)
                {
                    Direction = ParameterDirection.ReturnValue
                };
                command.Parameters.Add(returnedValue);

                try
                {
                    await connection.OpenAsync();
                    await command.ExecuteNonQueryAsync();
                    bool isSuccess = (int)returnedValue.Value > 0;
                    if (isSuccess)
                    {
                        return new Result<bool>(true, "Fine paid successfully.", true);
                    }
                    return new Result<bool>(false, "An unexpected error occurred on the server.", false, 500);
                }
                catch (Exception ex)
                {
                    return new Result<bool>(false, "An unexpected error occurred on the server.", false, 500);
                }
            }
        }

        public static async Task<Result<bool>> DoesMemberHaveUnpaidFinesAsync(int memberID)
        {
            using (SqlConnection connection = new SqlConnection(clsDataAccessSettings.ConnectionString))
            {
                string query = "SELECT FineID FROM Fines WHERE MemberID = @MemberID AND IsPaid = 0";
                using (SqlCommand command = new SqlCommand(query, connection))
                {
                    command.Parameters.AddWithValue("@MemberID", memberID);
                    try
                    {
                        await connection.OpenAsync();
                        using (SqlDataReader reader = await command.ExecuteReaderAsync())
                        {
                            bool hasUnpaidFines = reader.HasRows;
                            return new Result<bool>(true, "Check for fine complete.", hasUnpaidFines);
                        }
                    }
                    catch (Exception ex)
                    {
                        return new Result<bool>(false, "An unexpected error occurred on the server.", false, 500);
                    }
                }
            }
        }

        public static async Task<Result<List<FineViewDTO>>> GetAllFinesAsync()
        {
            var fines = new List<FineViewDTO>();
            using (SqlConnection connection = new SqlConnection(clsDataAccessSettings.ConnectionString))
            {
                string query = "SELECT * FROM Fines_View";
                using (SqlCommand command = new SqlCommand(query, connection))
                {
                    try
                    {
                        await connection.OpenAsync();
                        using (SqlDataReader reader = await command.ExecuteReaderAsync())
                        {
                            while (await reader.ReadAsync())
                            {
                                fines.Add(new FineViewDTO(
                                    reader.GetInt32(reader.GetOrdinal("FineID")),
                                    reader.GetInt32(reader.GetOrdinal("MemberID")),
                                    reader.GetInt32(reader.GetOrdinal("BorrowID")),
                                    Convert.ToSingle(reader.GetDecimal(reader.GetOrdinal("FineAmount"))),
                                    Convert.ToSingle(reader.GetDecimal(reader.GetOrdinal("PaidAmount"))),
                                    reader.GetDateTime(reader.GetOrdinal("FineDate")),
                                    reader.GetBoolean(reader.GetOrdinal("IsPaid")),
                                    reader.GetString(reader.GetOrdinal("FineReason"))
                                ));
                            }
                            return new Result<List<FineViewDTO>>(true, "All fines retrieved successfully.", fines);
                        }
                    }
                    catch (Exception ex)
                    {
                        return new Result<List<FineViewDTO>>(false, "An unexpected error occurred on the server.", fines, 500);
                    }
                }
            }
        }

    }

}
