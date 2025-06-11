using Microsoft.Data.SqlClient;
using SharedClasses;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using static SharedClasses.BorrowDTOs;
using static SharedClasses.ReservationDTOs;

namespace LibraryManagementSystemAPIDataAccessLayer
{
    public class clsReservationData
    {
        static string GetStatusText(short status)
        {
            switch (status)
            {
                case 1:
                    return "Pending";
                case 2:
                    return "Available";
                case 3:
                    return "Complete";
                case 4:
                    return "Cancelled";
                default:
                    return "UnKnown";
            }
        }

        public static async Task<Result<ReservationDTO>> GetReservationInfoByIDAsync(int reservationID)
        {
            string query = "Select * FROM Reservations WHERE ReservationID = @ReservationID";
            using (SqlConnection connection = new SqlConnection(clsDataAccessSettings.ConnectionString))
            {
                using (SqlCommand command = new SqlCommand(query, connection))
                {
                    command.Parameters.AddWithValue("@ReservationID", reservationID);
                    try
                    {
                        await connection.OpenAsync();
                        using (SqlDataReader reader = await command.ExecuteReaderAsync())
                        {
                            if (reader.Read())
                            {
                                ReservationDTO reservationDTO = new ReservationDTO
                                (
                                    (int)reader["ReservationID"],
                                    (int)reader["MemberID"],
                                    (int)reader["BookID"],
                                    (DateTime)reader["ReservationDate"],
                                    GetStatusText((short)reader["ReservationStatus"]),
                                    (int)reader["CreatedByUserID"]
                                );
                                return new Result<ReservationDTO>(true, new { success = new { header = "Success", body = "Reservation found successfully." } }, reservationDTO);
                            }
                            return new Result<ReservationDTO>(false, new { error = new { header = "Not Found", body = "Reservation not found." } }, null, 404);
                        }
                    }
                    catch (Exception ex)
                    {
                        return new Result<ReservationDTO>(false, new { error = new { header = "Server Error", body = "An unexpected error occurred on the server." } }, null, 500);
                    }
                }
            }
        }

        public static async Task<Result<int>> GetReservationIDAsync(GetReservationIDDTO getReservationIDDTO)
        {
            int reservationID = -1;
            using (SqlConnection connection = new SqlConnection(clsDataAccessSettings.ConnectionString))
            {
                string query = "Select ReservationID FROM Reservations WHERE MemberID = @MemberID and BookID = @BookID and ReservationStatus = 2";
                using (SqlCommand command = new SqlCommand(query, connection))
                {
                    command.Parameters.AddWithValue("@MemberID", getReservationIDDTO.MemberID);
                    command.Parameters.AddWithValue("@BookID", getReservationIDDTO.BookID);

                    try
                    {
                        await connection.OpenAsync();
                        object result = await command.ExecuteScalarAsync();
                        if (result != null && int.TryParse(result.ToString(), out int returnedID))
                        {
                            return new Result<int>(true, new { success = new { header = "Success", body = "Reservation ID retrieved successfully." } }, returnedID);
                        }
                        return new Result<int>(false, new { error = new { header = "Not Found", body = "Reservation not found." } }, -1, 404);
                    }
                    catch (Exception ex)
                    {
                        return new Result<int>(false, new { error = new { header = "Server Error", body = "An unexpected error occurred on the server." } }, -1, 500);
                    }
                }
            }
        }

        public static async Task<Result<int>> AddNewReservationAsync(AddNewReservationDTO addNewReservationDTO)
        {
            string query = @"
        INSERT INTO Reservations
            (
            MemberID
            ,BookID
            ,CreatedByUserID
            )
              VALUES
            (
             @MemberID, 
            @BookID,
             @CreatedByUserID
            );
     SELECT SCOPE_IDENTITY();
    ";
            using (var connection = new SqlConnection(clsDataAccessSettings.ConnectionString))
            using (var command = new SqlCommand(query, connection))
            {
                command.Parameters.AddWithValue("@MemberID", addNewReservationDTO.MemberID);
                command.Parameters.AddWithValue("@BookID", addNewReservationDTO.BookID);
                command.Parameters.AddWithValue("@CreatedByUserID", addNewReservationDTO.CreatedByUserID);

                try
                {
                    await connection.OpenAsync();
                    object result = await command.ExecuteScalarAsync();

                    if (result != null && int.TryParse(result.ToString(), out int NewReservationID))
                    {
                        return new Result<int>(true, new { success = new { header = "Success", body = "Book reserved successfully." } }, NewReservationID);
                    }
                    return new Result<int>(false, new { error = new { header = "Server Error", body = "An unexpected error occurred on the server." } }, -1, 500);
                }
                catch (Exception ex)
                {
                    return new Result<int>(false, new { error = new { header = "Server Error", body = "An unexpected error occurred on the server." } }, -1, 500);
                }
            }
        }

        public static async Task<Result<int>> CancelReservationAsync(int reservationID)
        {
            int reservedForMemberID = -1;
            string procedureName = @"sp_CancelReservation";
            using (var connection = new SqlConnection(clsDataAccessSettings.ConnectionString))
            using (var command = new SqlCommand(procedureName, connection))
            {
                command.CommandType = CommandType.StoredProcedure;

                command.Parameters.AddWithValue("@ReservationID", reservationID);

                SqlParameter returnedValue = new SqlParameter();
                returnedValue.Direction = ParameterDirection.ReturnValue;
                SqlParameter ReservedForMemberID = new SqlParameter("@ReservedForMemberID", SqlDbType.Int);
                ReservedForMemberID.Direction = ParameterDirection.Output;
                command.Parameters.Add(returnedValue);
                command.Parameters.Add(ReservedForMemberID);

                try
                {
                    await connection.OpenAsync();
                    await command.ExecuteNonQueryAsync();
                    int rowsAffected = (int)returnedValue.Value;
                    if (rowsAffected > 0)
                    {
                        if (ReservedForMemberID.Value != DBNull.Value)
                        {
                            reservedForMemberID = (int)ReservedForMemberID.Value;
                        }
                        return new Result<int>(true, new { success = new { header = "Success", body = "Reservation Cancelled successfully" } }, reservedForMemberID);
                    }
                    return new Result<int>(false, new { error = new { header = "Server Error", body = "An unexpected error occurred on the server." } }, -1, 500);
                }
                catch (Exception ex)
                {
                    return new Result<int>(false, new { error = new { header = "Server Error", body = "An unexpected error occurred on the server." } }, -1, 500);
                }
            }
        }

        static public async Task<Result<bool>> DoesHasActiveReservationForBookIDAsync(HasActiveReservationDTO hasActiveReservationDTO)
        {
            bool IsFound = false;

            using (SqlConnection connection = new SqlConnection(clsDataAccessSettings.ConnectionString))
            {
                string query = @"select Found = 1 from Reservations where BookID = @bookID and MemberID = @memberID and  ReservationStatus not in (3,4)";
                using (SqlCommand command = new SqlCommand(query, connection))
                {
                    command.Parameters.AddWithValue("@memberID", hasActiveReservationDTO.memberID);
                    command.Parameters.AddWithValue("@bookID", hasActiveReservationDTO.bookID);

                    try
                    {
                        await connection.OpenAsync();
                        SqlDataReader reader = await command.ExecuteReaderAsync();
                        IsFound = reader.HasRows;
                        return new Result<bool>(true, new { success = new { header = "Success", body = "Checking completed." } }, IsFound);
                    }
                    catch (Exception ex)
                    {
                        return new Result<bool>(false, new { error = new { header = "Server Error", body = "An unexpected error occurred on the server." } }, false, 500);
                    }
                }
            }
        }

        static public async Task<Result<List<ReservationViewDTO>>> GetAllReservationsAsync()
        {
            string query = @"select * from Reservations_View";
            List<ReservationViewDTO> allReservations = new List<ReservationViewDTO>();
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
                            allReservations.Add(new ReservationViewDTO
                            (
                                reader.GetInt32(reader.GetOrdinal("ReservationID")),
                                reader.GetInt32(reader.GetOrdinal("MemberID")),
                                reader.GetInt32(reader.GetOrdinal("BookID")),
                                reader.GetDateTime(reader.GetOrdinal("ReservationDate")),
                                reader.GetString(reader.GetOrdinal("ReservationStatus"))
                            ));
                        }
                        if (allReservations.Count > 0)
                        {
                            return new Result<List<ReservationViewDTO>>(true, "All books retrieved successfully.", allReservations);
                        }
                        return new Result<List<ReservationViewDTO>>(false, new { error = new { header = "Not Found", body = "No reservation found." } }, allReservations, 404);
                    }
                }
                catch (Exception ex)
                {
                    return new Result<List<ReservationViewDTO>>(false, new { error = new { header = "Server Error", body = "An unexpected error occurred on the server." } }, allReservations, 500);
                }
            }
        }

    }
}
