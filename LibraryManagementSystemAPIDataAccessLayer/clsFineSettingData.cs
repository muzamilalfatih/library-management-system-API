using Azure.Core.GeoJson;
using Microsoft.Data.SqlClient;
using SharedClasses;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static SharedClasses.FineDTOs;
using static SharedClasses.FineSettingsDTOs;
using static SharedClasses.UserDTOS;

namespace LibraryManagementSystemAPIDataAccessLayer
{
    public class clsFineSettingData
    {
        public static async Task<Result<List<FineSettingsDTO>>> GetFineSettingsInfoAsync()
        {
            List<FineSettingsDTO> FineSettings = new List<FineSettingsDTO>();
            using (SqlConnection connection = new SqlConnection(clsDataAccessSettings.ConnectionString))
            using (SqlCommand command = new SqlCommand("Select * from LibrarySettings", connection))
            {
                try
                {
                    await connection.OpenAsync();
                    using (SqlDataReader reader = await command.ExecuteReaderAsync())
                    {
                        while (await reader.ReadAsync())
                        {
                            FineSettings.Add(new FineSettingsDTO(
                                Convert.ToSingle(reader.GetDecimal("FineAmountPerDay")),
                                Convert.ToSingle(reader.GetDecimal("FineForDamagedBook"))
                            ));
                        }
                    }
                    return new Result<List<FineSettingsDTO>>(true, "Fine settings retrieved successfully.", FineSettings);
                }
                catch (Exception ex)
                {
                    return new Result<List<FineSettingsDTO>>(false, "An unexpected error occurred on the server.", FineSettings, 500);
                }
            }
        }

        public static async Task<Result<bool>> UpdateFineSettingsAsync(FineSettingsDTO fineSettingsDTO)
        {
            int rowsAffected = 0;
            using (SqlConnection connection = new SqlConnection(clsDataAccessSettings.ConnectionString))
            {
                string query = @"UPDATE LibrarySettings
                         SET FineAmountPerDay = @FineAmountPerDay,
                             FineForDamagedBook = @FineForDamagedBook";

                using (SqlCommand command = new SqlCommand(query, connection))
                {
                    command.Parameters.AddWithValue("@FineAmountPerDay", fineSettingsDTO.FineAmountPerDay);
                    command.Parameters.AddWithValue("@FineForDamagedBook", fineSettingsDTO.FineForDamagedBook);

                    try
                    {
                        await connection.OpenAsync();
                        rowsAffected = await command.ExecuteNonQueryAsync();
                        if (rowsAffected == 0)
                        {
                            return new Result<bool>(false, "An unexpected error occurred on the server.", false, 500);
                        }
                        return new Result<bool>(true, "Fine Settings updated successfully.", true);
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
