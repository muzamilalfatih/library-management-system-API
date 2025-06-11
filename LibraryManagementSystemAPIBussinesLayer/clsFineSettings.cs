using LibraryManagementSystemAPIDataAccessLayer;
using SharedClasses;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static SharedClasses.FineSettingsDTOs;

namespace LibraryManagementSystemAPIBussinesLayer
{
    public class clsFineSettings
    {
        public static async Task<Result<List<FineSettingsDTO>>> GetFineSettingInfoAsync()
        {
            return await clsFineSettingData.GetFineSettingsInfoAsync();
        }
        public static async Task<Result<bool>> UpdateFineSettingsAsync(FineSettingsDTO fineSettingsDTO)
        {
            if (fineSettingsDTO.FineAmountPerDay < 0 || fineSettingsDTO.FineForDamagedBook < 0)
            {
                return new Result<bool>(false, new { error = new { header = "Bad Request", body = "The request is invalid. Please check the input and try again." } }, false, 400);
            }
            return await clsFineSettingData.UpdateFineSettingsAsync(fineSettingsDTO);
        }

    }
}
