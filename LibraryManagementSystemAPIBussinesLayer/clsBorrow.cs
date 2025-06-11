using Azure.Core;
using LibraryManagementSystemAPIDataAccessLayer;
using SharedClasses;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static SharedClasses.BorrowDTOs;
using static SharedClasses.UserDTOS;

namespace LibraryManagementSystemAPIBussinesLayer
{
    public static class clsBorrow
    {
        public static async Task<Result<FullBorrowDTO>> FindAsync(int BorrowID)
        {
            if (BorrowID <= 0)
            {
                return new Result<FullBorrowDTO>(false, new { error = new { header = "Bad Request", body = "The request is invalid. Please check the input and try again." } }, null, 400);
            }
            return await clsBorrowData.GetBorrowInfoByIDAsync(BorrowID);
        }

        public static async Task<Result<FullBorrowDTO>> FindByCopyIDAsync(int CopyID)
        {
            if (CopyID <= 0)
            {
                return new Result<FullBorrowDTO>(false, new { error = new { header = "Bad Request", body = "The request is invalid. Please check the input and try again." } }, null, 400);
            }
            return await clsBorrowData.GetBorrowInfoByCopyIDAsync(CopyID);
        }

        public static async Task<Result<ReturnedBookDTO>> ReturnBookAsync(ReturnBookDTO returnBookDTO)
        {
            if (returnBookDTO.BorrowID <= 0 || returnBookDTO.ReturnNotes == "" || returnBookDTO.ReturnedByUserID <= 0)
            {
                return new Result<ReturnedBookDTO>(false, new { error = new { header = "Bad Request", body = "The request is invalid. Please check the input and try again." } }, null, 400);
            }
            Result<FullBorrowDTO> result = await FindAsync(returnBookDTO.BorrowID);
            if (!result.Success)
            {
                return new Result<ReturnedBookDTO>(false, result.Message, null, result.ErrorCode);
            }

            if (result.Data.ReturnDate != null)
            {
                return new Result<ReturnedBookDTO>(false, new { error = new { header = "Book Already Returned", body = $"This book is already returned on {result.Data.ReturnDate}" } }, null, 422);
            }
            return await clsBorrowData.ReturnBookAsync(returnBookDTO);
        }

        public static async Task<Result<BorrowedBookDTO>> BorrowBookAsync(BorrowDTO borrowDTO)
        {
            if (borrowDTO.MemberID <= 0 || borrowDTO.BookID <= 0 || borrowDTO.PaidFees <= 0 || borrowDTO.CreatedByUserID <= 0 || borrowDTO.DueDate <= DateTime.Now)
            {
                return new Result<BorrowedBookDTO>(false, new { error = new { header = "Bad Request", body = "The request is invalid. Please check the input and try again." } }, null, 400);
            }

            Result<clsMember> memberResult = await clsMember.FindAsync(borrowDTO.MemberID);
            Result<clsBook> bookResult = await clsBook.FindAsync(borrowDTO.BookID);

            if (!memberResult.Success)
            {
                return new Result<BorrowedBookDTO>(false, memberResult.Message, null, memberResult.ErrorCode);
            }

            Result<bool> memberNewResult = await memberResult.Data.CheckMemberCriteriaAsync();
            if (!memberNewResult.Success)
            {
                return new Result<BorrowedBookDTO>(false, memberNewResult.Message, null, memberNewResult.ErrorCode);
            }

            if (!bookResult.Success)
            {
                return new Result<BorrowedBookDTO>(false, bookResult.Message, null, bookResult.ErrorCode);
            }

            Result<bool> bookNewResult = await bookResult.Data.CheckBookCriteriaAsync(memberResult.Data, borrowDTO.HasReserrvation);
            if (!bookNewResult.Success)
            {
                return new Result<BorrowedBookDTO>(false, bookNewResult.Message, null, bookNewResult.ErrorCode);
            }

            return await clsBorrowData.AddNewBorrowAsync(borrowDTO);
        }

        public static async Task<Result<int>> GetTotalIssuedBooksAsync()
        {
            return await clsBorrowData.GetTotalIssuedBooksAsync();
        }

        public static async Task<Result<List<BorrowViewDTO>>> GetAllBorrowsAsync()
        {
            return await clsBorrowData.GetAllBorrowsAsync();
        }

    }
}
