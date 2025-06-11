using LibraryManagementSystemAPIDataAccessLayer;
using SharedClasses;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static SharedClasses.BorrowDTOs;
using static SharedClasses.FineDTOs;
using static SharedClasses.UserDTOS;

namespace LibraryManagementSystemAPIBussinesLayer
{
    public static class clsFine
    {
        public static async Task<Result<FullFineDTO>> FindAsync(int FineID)
        {
            if (FineID <= 0)
            {
                return new Result<FullFineDTO>(false, new { error = new { header = "Bad Request", body = "The request is invalid. Please check the input and try again." } }, null, 400);
            }
            return await clsFineData.GetFineInfoByIDAsync(FineID);
        }

        public static bool ValidateDataAsync(ref PayFineDTO payFineDTO)
        {
            if (payFineDTO.fineID <= 0)
            {
                return false;
            }
            if (payFineDTO.paidByUserID <= 0)
            {
                return false;
            }
            if (payFineDTO.paidAmount <= 0)
            {
                return false;
            }
            return true;
        }

        public static async Task<Result<bool>> PayFineAsync(PayFineDTO payFineDTO)
        {
            if (!ValidateDataAsync(ref payFineDTO))
            {
                return new Result<bool>(false, new { error = new { header = "Bad Request", body = "The request is invalid. Please check the input and try again." } }, false, 400);
            }

            Result<FullFineDTO> result = await FindAsync(payFineDTO.fineID);

            if (result.Success)
            {
                if (result.Data.IsPaid)
                {
                    return new Result<bool>(false, new { error = new { header = "Bad Request", body = "Fine already paid." } }, false, 400);
                }
                if (payFineDTO.paidAmount > (result.Data.PaidAmount + result.Data.FineAmount))
                {
                    return new Result<bool>(false, new { error = new { header = "Bad Request", body = "The amount must be less than or equal to the fine amount." } }, false, 400);
                }
                return await clsFineData.PayFineAsync(payFineDTO);
            }
            return new Result<bool>(false, result.Message, false, result.ErrorCode);
        }

        public static async Task<Result<List<FineViewDTO>>> GetAllFinesAsync()
        {
            return await clsFineData.GetAllFinesAsync();
        }

    }
}
