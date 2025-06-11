using LibraryManagementSystemAPIDataAccessLayer;
using System;
using System.Collections.Generic;
using Microsoft.Data.SqlClient;
using SharedClasses;
using static SharedClasses.MembershipDTOs;
using static SharedClasses.MembershipClassDTOs;

namespace LibraryManagementSystemAPIDataAccessLayer
{
    

    public static class clsMembershipData
    {
        public static async Task<Result<FullMembershipDTO>> GetMembershipByIDAsync(int membershipID)
        {
            using (SqlConnection connection = new SqlConnection(clsDataAccessSettings.ConnectionString))
            {
                using (SqlCommand command = new SqlCommand("sp_GetMembershipByID", connection))
                {
                    command.CommandType = System.Data.CommandType.StoredProcedure;
                    command.Parameters.AddWithValue("@MembershipID", membershipID);

                    try
                    {
                        await connection.OpenAsync();
                        using (SqlDataReader reader = await command.ExecuteReaderAsync())
                        {
                            if (await reader.ReadAsync())
                            {
                                int membershipClassID = (int)reader["MembershipClassID"];
                                Result<MembershipClassDTO> membershipClassResult = await clsMembershipClassData.GetMembershipClassInfoByIDAsync(membershipClassID);
                                if (!membershipClassResult.Success)
                                {
                                    return new Result<FullMembershipDTO>(false, membershipClassResult.Message, null, membershipClassResult.ErrorCode);
                                }
                                int createdByUserID = (int)reader["CreatedByUserID"];
                                Result<UserDTOS.UserDTO> userResult = await clsUserData.GetUserInfoByUserIDAsync(createdByUserID);
                                if (!userResult.Success)
                                {
                                    return new Result<FullMembershipDTO>(false, userResult.Message, null, userResult.ErrorCode);
                                }
                                var membership = new FullMembershipDTO(
                                    (int)reader["MembershipID"],
                                    (int)reader["MemberID"],
                                    membershipClassID,
                                    (DateTime)reader["MembershipStartDate"],
                                    (DateTime)reader["MembershipExpirationDate"],
                                    Convert.ToSingle(reader["PaidFees"]),
                                    createdByUserID,
                                    membershipClassResult.Data,
                                    userResult.Data.UserName
                                );
                                return new Result<FullMembershipDTO>(true, new { success = new { header = "Success", body = "Membership retrieved successfully." } }, membership);
                            }
                            else
                            {
                                return new Result<FullMembershipDTO>(false, new { error = new { header = "Not Found", body = "Membership not found." } }, null, 404);
                            }
                        }
                    }
                    catch (Exception ex)
                    {
                        return new Result<FullMembershipDTO>(false, new { error = new { header = "Server Error", body = "An unexpected error occurred on the server." } }, null, 500);
                    }
                }
            }
        }

        public static async Task<Result<FullMembershipDTO>> GetMembershipByMemberIDAsync(int memberID)
        {
            using (SqlConnection connection = new SqlConnection(clsDataAccessSettings.ConnectionString))
            {
                string query = @"Select top 1 * FROM Memberships WHERE MemberID = @MemberID
                        ORDER BY MembershipID DESC";

                using (SqlCommand command = new SqlCommand(query, connection))
                {
                    command.Parameters.AddWithValue("@memberID", memberID);

                    try
                    {
                        await connection.OpenAsync();
                        using (SqlDataReader reader = await command.ExecuteReaderAsync())
                        {
                            if (await reader.ReadAsync())
                            {
                                int membershipClassID = (int)reader["MembershipClassID"];
                                Result<MembershipClassDTO> membershipClassResult = await clsMembershipClassData.GetMembershipClassInfoByIDAsync(membershipClassID);
                                if (!membershipClassResult.Success)
                                {
                                    return new Result<FullMembershipDTO>(false, membershipClassResult.Message, null, membershipClassResult.ErrorCode);
                                }
                                int createdByUserID = (int)reader["CreatedByUserID"];
                                Result<UserDTOS.UserDTO> userResult = await clsUserData.GetUserInfoByUserIDAsync(createdByUserID);
                                if (!userResult.Success)
                                {
                                    return new Result<FullMembershipDTO>(false, userResult.Message, null, userResult.ErrorCode);
                                }
                                var membership = new FullMembershipDTO(
                                    (int)reader["MembershipID"],
                                    (int)reader["MemberID"],
                                    membershipClassID,
                                    (DateTime)reader["MembershipStartDate"],
                                    (DateTime)reader["MembershipExpirationDate"],
                                    Convert.ToSingle(reader["PaidFees"]),
                                    createdByUserID,
                                    membershipClassResult.Data,
                                    userResult.Data.UserName
                                );
                                return new Result<FullMembershipDTO>(true, new { success = new { header = "Success", body = "Membership retrieved successfully." } }, membership);
                            }
                            else
                            {
                                return new Result<FullMembershipDTO>(false, new { error = new { header = "Not Found", body = "Membership not found." } }, null, 404);
                            }
                        }
                    }
                    catch (Exception ex)
                    {
                        return new Result<FullMembershipDTO>(false, new { error = new { header = "Server Error", body = "An unexpected error occurred on the server." } }, null, 500);
                    }
                }
            }
        }

        public static async Task<Result<int>> AddNewMembershipAsync(MembershipDTO membership)
        {
            int membershipID = -1;
            string query = @"
        INSERT INTO Memberships
      (
       MemberID,
       MembershipClassID,
       MembershipStartDate,
       MembershipExpirationDate,
       PaidFees,
       CreatedByUserID
      )
      VALUES
      (
       @MemberID,
       @MembershipClassID, 
       @MembershipStartDate, 
       @MembershipExpirationDate,
       @PaidFees,
       @CreatedByUserID
      );
      SELECT SCOPE_IDENTITY();
    ";

            using (SqlConnection connection = new SqlConnection(clsDataAccessSettings.ConnectionString))
            {
                SqlCommand command = new SqlCommand(query, connection);
                command.Parameters.AddWithValue("@MemberID", membership.MemberID);
                command.Parameters.AddWithValue("@MembershipClassID", membership.MembershipClassID);
                command.Parameters.AddWithValue("@MembershipStartDate", membership.MembershipStartDate);
                command.Parameters.AddWithValue("@MembershipExpirationDate", membership.MembershipExpirationDate);
                command.Parameters.AddWithValue("@PaidFees", membership.PaidFees);
                command.Parameters.AddWithValue("@CreatedByUserID", membership.CreatedByUserID);

                try
                {
                    await connection.OpenAsync();
                    object result = await command.ExecuteScalarAsync();
                    if (result != null && int.TryParse(result.ToString(), out int newMembershipID))
                    {
                        membershipID = newMembershipID;
                        return new Result<int>(true, new { success = new { header = "Success", body = "New membership added successfully." } }, membershipID);
                    }
                    else
                    {
                        return new Result<int>(false, new { error = new { header = "Server Error", body = "An unexpected error occurred on the server." } }, -1, 500);
                    }
                }
                catch (Exception ex)
                {
                    return new Result<int>(false, new { error = new { header = "Server Error", body = "An unexpected error occurred on the server." } }, membershipID, 500);
                }
            }
        }

        public static async Task<Result<int>> UpdateMembershipAsync(FullMembershipDTO membership)
        {
            int rowsAffected = 0;
            using (SqlConnection connection = new SqlConnection(clsDataAccessSettings.ConnectionString))
            {
                string query = @"UPDATE Memberships  
                        SET MemberID = @MemberID, 
                            MembershipClassID = @MembershipClassID, 
                            MembershipStartDate = @MembershipStartDate,       
                            MembershipEndDate = @MembershipEndDate, 
                            PaidFees = @PaidFees 
                        WHERE MembershipID = @MembershipID";

                using (SqlCommand command = new SqlCommand(query, connection))
                {
                    command.Parameters.AddWithValue("@MembershipID", membership.MembershipID);
                    command.Parameters.AddWithValue("@MemberID", membership.MemberID);
                    command.Parameters.AddWithValue("@MembershipClassID", membership.MembershipClassID);
                    command.Parameters.AddWithValue("@MembershipStartDate", membership.MembershipStartDate);
                    command.Parameters.AddWithValue("@MembershipExpirationDate", membership.MembershipExpirationDate);
                    command.Parameters.AddWithValue("@PaidFees", membership.PaidFees);

                    try
                    {
                        await connection.OpenAsync();
                        rowsAffected = await command.ExecuteNonQueryAsync();
                        if (rowsAffected > 0)
                        {
                            return new Result<int>(true, new { success = new { header = "Success", body = "Membership updated successfully." } }, rowsAffected);
                        }
                        return new Result<int>(false, new { error = new { header = "Server Error", body = "An unexpected error occurred on the server." } }, -1, 500);
                    }
                    catch (Exception ex)
                    {
                        return new Result<int>(false, new { error = new { header = "Server Error", body = "An unexpected error occurred on the server." } }, -1, 500);
                    }
                }
            }
        }

        public static async Task<Result<bool>> DeleteMembershipAsync(int membershipID)
        {
            int rowsAffected = 0;
            using (SqlConnection connection = new SqlConnection(clsDataAccessSettings.ConnectionString))
            {
                SqlCommand command = new SqlCommand("sp_DeleteMembership", connection);
                command.CommandType = System.Data.CommandType.StoredProcedure;
                command.Parameters.AddWithValue("@MembershipID", membershipID);

                try
                {
                    await connection.OpenAsync();
                    rowsAffected = await command.ExecuteNonQueryAsync();
                    if (rowsAffected > 0)
                    {
                        return new Result<bool>(true, new { success = new { header = "Success", body = "Membership deleted successfully." } }, true);
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

}
