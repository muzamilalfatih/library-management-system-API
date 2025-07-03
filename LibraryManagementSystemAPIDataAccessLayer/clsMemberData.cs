using System;
using System.Collections.Generic;
using System.Data;
using LibraryManagementSystemAPIDataAccessLayer;
using Microsoft.Data.SqlClient;
using SharedClasses;
using static SharedClasses.AuthorDTOs;
using static SharedClasses.FineDTOs;
using static SharedClasses.MemberDTOs;

namespace LibraryManagementSystemAPIDataAccessLayer
{
    

    public class clsMemberData
    {
        public class AddNewMemberDTO
        {
            public AddNewMemberDTO(int personID, int createdByUserID, bool isActive)
            {
                PersonID = personID;
                CreatedByUserID = createdByUserID;
                IsActive = isActive;
            }

            public int PersonID { get; set; }
            public int CreatedByUserID { get; set; }
            public bool IsActive { get; set; }
        }
        public class UpdateMemberDTO
        {
            public UpdateMemberDTO(int memberID, bool isActive)
            {
                MemberID = memberID;
                IsActive = isActive;
            }

            public int MemberID { get; set; }
            public bool IsActive { get; set; }
        }
        public static async Task<Result<FullMemberDTO>> GetMemberInfoByIDAsync(int memberID)
        {
            using (SqlConnection connection = new SqlConnection(clsDataAccessSettings.ConnectionString))
            {
                string query = "SELECT * FROM Members WHERE MemberID = @MemberID";
                using (SqlCommand command = new SqlCommand(query, connection))
                {
                    command.Parameters.AddWithValue("@MemberID", memberID);
                    try
                    {
                        await connection.OpenAsync();
                        using (SqlDataReader reader = await command.ExecuteReaderAsync())
                        {
                            if (reader.Read())
                            {
                                int personID = reader.GetInt32("PersonID");
                                Result<PersonDTOs.PersonDTO> personResult = await clsPersonData.GetPersonInfoByIDAsync(personID);
                                if (!personResult.Success)
                                {
                                    return new Result<FullMemberDTO>(false, personResult.Message, null, personResult.ErrorCode);
                                }

                                int createdByUserID = reader.GetInt32("CreatedByUserID");
                                Result<UserDTOS.UserDTO> userResult = await clsUserData.GetUserInfoByUserIDAsync(createdByUserID);
                                if (!userResult.Success)
                                {
                                    return new Result<FullMemberDTO>(false, userResult.Message, null, userResult.ErrorCode);
                                }
                                Result<MembershipDTOs.FullMembershipDTO> membershipResult = await clsMembershipData.GetMembershipByMemberIDAsync(memberID);
                                if (!membershipResult.Success)
                                {
                                    return new Result<FullMemberDTO>(false, membershipResult.Message, null, membershipResult.ErrorCode);
                                }
                                var member = new FullMemberDTO(
                                    reader.GetInt32(reader.GetOrdinal("MemberID")),
                                    personID,
                                    reader.GetDateTime(reader.GetOrdinal("CreatedDate")),
                                    reader.GetBoolean(reader.GetOrdinal("IsActive")),
                                    createdByUserID,
                                    personResult.Data,
                                    userResult.Data,
                                    membershipResult.Data
                                );
                                return new Result<FullMemberDTO>(true, "Member found successfully.", member);
                            }
                            else
                            {
                                return new Result<FullMemberDTO>(false, "Member not found.", null, 404);
                            }
                        }
                    }
                    catch (Exception ex)
                    {
                        return new Result<FullMemberDTO>(false, "An unexpected error occurred on the server.", null, 500);
                    }
                }
            }
        }

        public static async Task<Result<int>> AddNewMemberAsync(AddNewMemberDTO addNewMemberDTO)
        {
            string query = @"
                                INSERT INTO Members
                           (
                           PersonID
                           ,CreatedByUserID 
                            ,IsActive
                           )
                     VALUES
                           (
                            @PersonID, 
                            @CreatedByUserID,
                            @IsActive
                           );
                            SELECT SCOPE_IDENTITY();
                ";
            using (SqlConnection connection = new SqlConnection(clsDataAccessSettings.ConnectionString))
            using (SqlCommand command = new SqlCommand("query", connection))
            {
                command.Parameters.AddWithValue("@PersonID", addNewMemberDTO.PersonID);
                command.Parameters.AddWithValue("@CreatedByUserID", addNewMemberDTO.CreatedByUserID);
                command.Parameters.AddWithValue("@IsActive", addNewMemberDTO.IsActive);
                try
                {
                    await connection.OpenAsync();
                    object result = await command.ExecuteScalarAsync();

                    if (result != null && int.TryParse(result.ToString(), out int NewReservationID))
                    {
                        return new Result<int>(true, "Member Added successfully.", NewReservationID);
                    }
                    return new Result<int>(false, "An unexpected error occurred on the server.", -1, 500);
                }
                catch (Exception ex)
                {
                    return new Result<int>(false, "An unexpected error occurred on the server.", -1, 500);
                }
            }
        }

        public static async Task<Result<int>> UpdateMemberAsync(UpdateMemberDTO updateMemberDTO)
        {
            using (SqlConnection connection = new SqlConnection(clsDataAccessSettings.ConnectionString))
            {
                string query = @"UPDATE Members SET IsActive = @IsActive WHERE MemberID = @MemberID";
                using (SqlCommand command = new SqlCommand(query, connection))
                {
                    command.Parameters.AddWithValue("@MemberID", updateMemberDTO.MemberID);
                    command.Parameters.AddWithValue("@IsActive", updateMemberDTO.IsActive);

                    try
                    {
                        await connection.OpenAsync();
                        int rowsAffected = await command.ExecuteNonQueryAsync();
                        if (rowsAffected > 0)
                        {
                            return new Result<int>(true, "Member updated successfully.", rowsAffected);
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

        public static async Task<Result<bool>> DeleteMemberAsync(int memberID)
        {
            using (SqlConnection connection = new SqlConnection(clsDataAccessSettings.ConnectionString))
            {
                string query = @"DELETE FROM Members WHERE MemberID = @MemberID";
                using (SqlCommand command = new SqlCommand(query, connection))
                {
                    command.Parameters.AddWithValue("@MemberID", memberID);

                    try
                    {
                        await connection.OpenAsync();
                        int rowsAffected = await command.ExecuteNonQueryAsync();
                        if (rowsAffected > 0)
                        {
                            return new Result<bool>(true, "Member deleted successfully.", true);
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

        public static async Task<Result<bool>> IsMemberExistAsync(int memberID)
        {
            using (SqlConnection connection = new SqlConnection(clsDataAccessSettings.ConnectionString))
            {
                string query = "SELECT MemberID FROM Members WHERE MemberID = @MemberID";
                using (SqlCommand command = new SqlCommand(query, connection))
                {
                    command.Parameters.AddWithValue("@MemberID", memberID);

                    try
                    {
                        await connection.OpenAsync();
                        using (SqlDataReader reader = await command.ExecuteReaderAsync())
                        {
                            bool isFound = reader.HasRows;
                            return new Result<bool>(true, "Member existence check complete", isFound);
                        }
                    }
                    catch (Exception ex)
                    {
                        return new Result<bool>(false, "An unexpected error occurred on the server.", false, 500);
                    }
                }
            }
        }

        public static async Task<Result<List<MemberViewDTO>>> GetAllMembersAsync()
        {
            var members = new List<MemberViewDTO>();
            string query = "SELECT * FROM Members_View ORDER BY MemberID";

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
                            members.Add(new MemberViewDTO(
                                reader.GetInt32(reader.GetOrdinal("MemberID")),
                                reader.GetString(reader.GetOrdinal("FullName")),
                                reader.GetDateTime(reader.GetOrdinal("MembershipStartDate")),
                                reader.GetDateTime(reader.GetOrdinal("MembershipExpirationDate")),
                                reader.GetString(reader.GetOrdinal("MembershipClassName")),
                                reader.GetInt32(reader.GetOrdinal("TotalBorrowedBooks")),
                                reader.GetBoolean(reader.GetOrdinal("IsActive"))
                            ));
                        }
                        return new Result<List<MemberViewDTO>>(true, "All members retrieved successfully.", members);
                    }
                }
                catch (Exception ex)
                {
                    return new Result<List<MemberViewDTO>>(false, "An unexpected error occurred on the server.", members, 500);
                }
            }
        }

        public static async Task<Result<int>> GetTotalMembersAsync()
        {
            int totalMembers = 0;
            string query = @"SELECT COUNT(MemberID) FROM Members";

            using (SqlConnection connection = new SqlConnection(clsDataAccessSettings.ConnectionString))
            using (SqlCommand command = new SqlCommand(query, connection))
            {
                try
                {
                    await connection.OpenAsync();
                    object result = await command.ExecuteScalarAsync();
                    if (result != null && int.TryParse(result.ToString(), out int MemberCount))
                    {
                        totalMembers = MemberCount;
                    }
                    return new Result<int>(true, "Total members retrieved successfully.", totalMembers);
                }
                catch (Exception ex)
                {
                    return new Result<int>(false, "An unexpected error occurred on the server.", totalMembers, 500);
                }
            }
        }
    }

}
