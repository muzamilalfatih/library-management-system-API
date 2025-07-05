using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using LibraryManagementSystemAPIDataAccessLayer;
using Microsoft.Data.SqlClient;
using static SharedClasses.PersonDTOs;
using SharedClasses;
using static SharedClasses.MembershipDTOs;

namespace LibraryManagementSystemAPIDataAccessLayer
{
   
    public class clsPersonData
    {
        public static async Task<Result<PersonDTO>> GetPersonInfoByIDAsync(int PersonID)
        {
            using (SqlConnection connection = new SqlConnection(clsDataAccessSettings.ConnectionString))
            {
                string query = "Select * FROM People WHERE PersonID = @PersonID";
                using (SqlCommand command = new SqlCommand(query, connection))
                {
                    command.Parameters.AddWithValue("@PersonID", PersonID);
                    try
                    {
                        await connection.OpenAsync();
                        using (var reader = await command.ExecuteReaderAsync())
                        {
                            if (await reader.ReadAsync())
                            {
                                var personDTO = new PersonDTO
                                (
                                    reader.GetInt32(reader.GetOrdinal("PersonID")),
                                    reader.GetString(reader.GetOrdinal("NationalNo")),
                                    reader.GetString(reader.GetOrdinal("FirstName")),
                                    reader.GetString(reader.GetOrdinal("SecondName")),
                                    reader.IsDBNull(reader.GetOrdinal("ThirdName")) ? null : reader.GetString(reader.GetOrdinal("ThirdName")),
                                    reader.GetString(reader.GetOrdinal("LastName")),
                                    (enGender)reader.GetInt32(reader.GetOrdinal("Gender")),
                                    reader.GetString(reader.GetOrdinal("Email")),
                                    reader.GetString(reader.GetOrdinal("Phone")),
                                    reader.GetString(reader.GetOrdinal("Address"))
                                );
                                return new Result<PersonDTO>(true, "Person found successfully", personDTO);
                            }
                            else
                            {
                                return new Result<PersonDTO>(false, "Person not found.", null, 404);
                            }
                        }
                    }
                    catch (Exception ex)
                    {
                        return new Result<PersonDTO>(false, "An unexpected error occurred on the server.", null, 500);
                    }
                }
            }
        }

        public static async Task<Result<int>> AddNewPersonAsync(PersonDTO personDTO)
        {
            using (var connection = new SqlConnection(clsDataAccessSettings.ConnectionString))
            using (var command = new SqlCommand("sp_AddNewPerson", connection))
            {
                command.CommandType = CommandType.StoredProcedure;

                command.Parameters.AddWithValue("@NationalNo", personDTO.NationalNo);
                command.Parameters.AddWithValue("@FirstName", personDTO.FirstName);
                command.Parameters.AddWithValue("@SecondName", personDTO.SecondName);
                command.Parameters.Add(new SqlParameter("@ThirdName", personDTO.ThirdName ?? (object)DBNull.Value));
                command.Parameters.AddWithValue("@LastName", personDTO.LastName);
                command.Parameters.AddWithValue("@Gender", (byte)personDTO.Gender);
                command.Parameters.AddWithValue("@Email", personDTO.Email);
                command.Parameters.AddWithValue("@Phone", personDTO.Phone);
                command.Parameters.AddWithValue("@Address", personDTO.Address);

                SqlParameter returnedValue = new SqlParameter("@ReturnValue", SqlDbType.Int);
                returnedValue.Direction = ParameterDirection.ReturnValue;
                command.Parameters.Add(returnedValue);

                try
                {
                    await connection.OpenAsync();
                    await command.ExecuteNonQueryAsync();
                    int PersonID = (int)returnedValue.Value;

                    if (PersonID > 0)
                    {
                        return new Result<int>(true, "Person added successfully.", PersonID);
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

        public static async Task<Result<int>> UpdatePersonAsync(PersonDTO personDTO)
        {
            using (SqlConnection connection = new SqlConnection(clsDataAccessSettings.ConnectionString))
            {
                string query = @"Update People  
                     set NationalNo = @NationalNo,
                         FirstName = @FirstName, 
                         SecondName = @SecondName, 
                         ThirdName = @ThirdName, 
                         LastName = @LastName, 
                         Gender = @Gender,
                         Address = @Address,
                         Phone = @Phone,
                         Email = @Email
                     where PersonID = @PersonID";
                using (var command = new SqlCommand(query, connection))
                {
                    command.Parameters.AddWithValue("@PersonID", personDTO.PersonID);
                    command.Parameters.AddWithValue("@NationalNo", personDTO.NationalNo);
                    command.Parameters.AddWithValue("@FirstName", personDTO.FirstName);
                    command.Parameters.AddWithValue("@SecondName", personDTO.SecondName);
                    command.Parameters.Add(new SqlParameter("@ThirdName", personDTO.ThirdName ?? (object)DBNull.Value));
                    command.Parameters.AddWithValue("@LastName", personDTO.LastName);
                    command.Parameters.AddWithValue("@Gender",(byte) personDTO.Gender);
                    command.Parameters.AddWithValue("@Email", personDTO.Email);
                    command.Parameters.AddWithValue("@Phone", personDTO.Phone);
                    command.Parameters.AddWithValue("@Address", personDTO.Address);

                    try
                    {
                        await connection.OpenAsync();
                        int rowsAffected = await command.ExecuteNonQueryAsync();
                        if (rowsAffected > 0)
                        {
                            return new Result<int>(true, "Person updated successfully.", rowsAffected);
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

        public static async Task<Result<bool>> DeletePersonAsync(int PersonID)
        {
            using (SqlConnection connection = new SqlConnection(clsDataAccessSettings.ConnectionString))
            {
                string query = @"DELETE FROM People WHERE PersonID = @PersonID";
                using (SqlCommand command = new SqlCommand(query, connection))
                {
                    command.Parameters.AddWithValue("@PersonID", PersonID);
                    try
                    {
                        await connection.OpenAsync();
                        int rowsAffected = await command.ExecuteNonQueryAsync();
                        if (rowsAffected > 0)
                        {
                            return new Result<bool>(true, "Person deleted successfully.", true);
                        }
                        else
                        {
                            return new Result<bool>(false, "An unexpected error occurred on the server.", false, 500);
                        }
                    }
                    catch (Exception ex)
                    {
                        return new Result<bool>(false, "An unexpected error occurred on the server.", false, 500);
                    }
                }
            }
        }

        public static async Task<Result<bool>> IsPersonExistAsync(int PersonID)
        {
            using (SqlConnection connection = new SqlConnection(clsDataAccessSettings.ConnectionString))
            {
                string query = "SELECT PersonID FROM People WHERE PersonID = @PersonID";
                using (SqlCommand command = new SqlCommand(query, connection))
                {
                    command.Parameters.AddWithValue("@PersonID", PersonID);
                    try
                    {
                        await connection.OpenAsync();
                        using (SqlDataReader reader = await command.ExecuteReaderAsync())
                        {
                            bool isFound = reader.HasRows;
                            return new Result<bool>(true, "Person existence check completed", isFound);
                        }
                    }
                    catch (Exception ex)
                    {
                        return new Result<bool>(false, "An unexpected error occurred on the server.", false, 500);
                    }
                }
            }
        }

        public static async Task<Result<bool>> IsPersonExistAsync(string NationalNo)
        {
            using (SqlConnection connection = new SqlConnection(clsDataAccessSettings.ConnectionString))
            {
                string query = "SELECT PersonID FROM People WHERE NationalNo = @NationalNo";
                using (SqlCommand command = new SqlCommand(query, connection))
                {
                    command.Parameters.AddWithValue("@NationalNo", NationalNo);
                    try
                    {
                        await connection.OpenAsync();
                        using (SqlDataReader reader = await command.ExecuteReaderAsync())
                        {
                            bool isFound = reader.HasRows;
                            return new Result<bool>(true, "Person existence check completed", isFound);
                        }
                    }
                    catch (Exception ex)
                    {
                        return new Result<bool>(false, "An unexpected error occurred on the server.", false, 500);
                    }
                }
            }
        }

    }

}
