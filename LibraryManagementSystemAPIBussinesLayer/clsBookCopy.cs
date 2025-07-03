using LibraryManagementSystemAPIDataAccessLayer;
using SharedClasses;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static SharedClasses.AuthorDTOs;
using static SharedClasses.BookCopyDTOs;
using static SharedClasses.UserDTOS;
namespace LibraryManagementSystemAPIBussinesLayer
{
    public class clsBookCopy
    {

        public int BookCopyID { get; set; }
        public int BookID { get; set; }
        public int ReservedForMemberID { get; set; }
        public bool IsAvailable { get; set; }
        public bool IsDamaged { get; set; }

        public BookCopyDTO BCDTO
        {
            get
            {
                return new BookCopyDTO(this.BookCopyID, this.BookID, this.ReservedForMemberID, this.IsAvailable, this.IsDamaged);
            }
        }

        clsBookCopy(BookCopyDTO bookCopyDTO)
        {
            this.BookCopyID = bookCopyDTO.BookCopyID;
            this.BookID = bookCopyDTO.BookID;
            this.ReservedForMemberID = bookCopyDTO.ReservedForMemberID;
            this.IsAvailable = bookCopyDTO.IsAvailabe;
            this.IsDamaged = bookCopyDTO.IsDamaged;
        }

        static public async Task<Result<clsBookCopy>> FindAsync(int BookCopyID)
        {
            if (BookCopyID <= 0)
            {
                return new Result<clsBookCopy>(false, "The request is invalid. Please check the input and try again.", null, 400);
            }
            Result<BookCopyDTO> result = await clsBookCopyData.GetBookCopyInfoByIDAsync(BookCopyID);

            if (result.Success)
            {
                return new Result<clsBookCopy>(true, "Book copy retrieved successfully.", new clsBookCopy(result.Data));
            }
            return new Result<clsBookCopy>(result.Success, result.Message, null, result.ErrorCode);
        }
        public static async Task<Result<List<BookCopyDTO>>> GetAllCopiesAsync(int BookID)
        {
            return await clsBookCopyData.GetAllCopiesAsync(BookID);
        }

        private async Task<Result<int>> _AddNewCopyBookAsync()
        {
            return await clsBookCopyData.AddNewBookCopyAsync(BCDTO);
        }
        public static async Task<Result<int>> RepairAsync(RepairCopyDTO repairCopyDTO)
        {
            Result<clsBookCopy> result = await FindAsync(repairCopyDTO.bookCopyID);

            if (!result.Success)
            {
                return new Result<int>(false, result.Message, -1, result.ErrorCode);
            }
            if (!result.Data.IsDamaged)
            {
                return new Result<int>(false, "This copy is not damaged.", -1, 400);
            }
            return await clsBookCopyData.RepairBookCopyAsync(repairCopyDTO);
        }

    }
}
