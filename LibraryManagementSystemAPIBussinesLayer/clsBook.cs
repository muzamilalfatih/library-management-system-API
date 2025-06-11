using LibraryManagementSystemAPIDataAccessLayer;
using SharedClasses;
using System;
using System.Collections.Generic;
using System.Data;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static SharedClasses.BookCopyDTOs;
using static SharedClasses.BookDTOs;

namespace LibraryManagementSystemAPIBussinesLayer
{
    public class clsBook
    {
        public enum enMode { AddNew = 0, Update = 1 }
        private enMode _Mode;
        private int _NumberOfCopies;

        public int BookID { get; set; } = -1;
        public string Title { get; set; }
        public int AuthorID { get; set; }
        public string AuthorName { get; set; } = "";
        public string ISBN { get; set; }
        public int PublisherID { get; set; }
        public string PublisherName { get; set; } = "";
        public int CategoryID { get; set; }
        public string CategoryName { get; set; } = "";
        public DateTime Year { get; set; }
        public string Location { get; set; }
        public float BorrowFees { get; set; }

        public FullBookDTO FBDTO
        {
            get
            {
                return new FullBookDTO(this.BookID, this.Title, this.AuthorID, this.ISBN, this.PublisherID, this.CategoryID, this.Year,
                    this.Location, this.BorrowFees, this.AuthorName, this.PublisherName, this.CategoryName);
            }
        }

        public BookDTO BDTO
        {
            get
            {
                return new BookDTO(this.BookID, this.Title, this.AuthorID, this.ISBN, this.PublisherID, this.CategoryID, this.Year,
                    this.Location, this.BorrowFees);
            }
        }

        public clsBook(FullBookDTO fullBookDTO, enMode mode = enMode.AddNew)
        {
            this.BookID = fullBookDTO.BookID;
            this.Title = fullBookDTO.Title;
            this.AuthorID = fullBookDTO.AuthorID;
            this.AuthorName = fullBookDTO.AuthorName;
            this.ISBN = fullBookDTO.ISBN;
            this.PublisherID = fullBookDTO.PublisherID;
            this.PublisherName = fullBookDTO.PublisherName;
            this.CategoryID = fullBookDTO.CategoryID;
            this.CategoryName = fullBookDTO.CategoryName;
            this.Year = fullBookDTO.Year;
            this.Location = fullBookDTO.Location;
            this.BorrowFees = fullBookDTO.BorrowFees;
            this._Mode = mode;
        }

        public clsBook(BookDTO bookDTO, enMode mode = enMode.AddNew)
        {
            this.BookID = bookDTO.BookID;
            this.Title = bookDTO.Title;
            this.AuthorID = bookDTO.AuthorID;
            this.ISBN = bookDTO.ISBN;
            this.PublisherID = bookDTO.PublisherID;
            this.Year = bookDTO.Year;
            this.Location = bookDTO.Location;
            this.BorrowFees = bookDTO.BorrowFees;
            this._Mode = mode;
        }

        static public async Task<Result<clsBook>> FindAsync(int BookID)
        {
            if (BookID <= 0)
            {
                return new Result<clsBook>(false, new { error = new { header = "Bad Request", body = "The request is invalid. Please check the input and try again." } }, null, 400);
            }

            Result<FullBookDTO> result = await clsBookData.GetBookInfoByIDAsync(BookID);
            if (result.Success)
            {
                return new Result<clsBook>(true, result.Message, new clsBook(result.Data, enMode.Update));
            }
            return new Result<clsBook>(result.Success, result.Message, null, result.ErrorCode);
        }

        static public async Task<Result<clsBook>> FindAsync(string bookIdentifier)
        {
            Result<FullBookDTO> result = await clsBookData.GetBookInfoByISBNOrTitleAsync(bookIdentifier);

            if (result.Success)
            {
                return new Result<clsBook>(true, new { success = new { header = "Success", body = "Book found successfully." } }, new clsBook(result.Data, enMode.Update));
            }
            return new Result<clsBook>(false, result.Message, null, result.ErrorCode);
        }

        private async Task<Result<int>> _AddNewBookAsync(int numberOfCopies)
        {
            return await clsBookData.AddNewBookAsync(new AddNewBookDTO(this.BDTO, numberOfCopies));
        }

        private async Task<Result<int>> _UpdateBookAsync()
        {
            return await clsBookData.UpdateBookAsync(this.BDTO);
        }

        public async Task<Result<int>> SaveAsync(int numberOfCopies = 0)
        {
            switch (_Mode)
            {
                case enMode.AddNew:
                    Result<int> result = await _AddNewBookAsync(numberOfCopies);
                    if (result.Success)
                    {
                        this.BookID = result.Data;
                        this._Mode = enMode.Update;
                    }
                    return result;
                case enMode.Update:
                    return await _UpdateBookAsync();
                default:
                    return new Result<int>(false, new { error = new { header = "Server Error", body = "An unexpected error occurred on the server." } }, -1, 500);
            }
        }

        public async Task<Result<bool>> IsAvaiableAsync()
        {
            Result<int> result = await GetCopiesAvilableAsync(this.BookID);
            if (!result.Success)
            {
                return new Result<bool>(false, result.Message, false, result.ErrorCode);
            }
            bool available = result.Data > 0;
            return new Result<bool>(true, new { success = new { header = "Success", body = "Book availability complete." } }, available);
        }

        static public async Task<Result<int>> GetCopiesAvilableAsync(int BookID)
        {
            return await clsBookCopyData.AvailableCopiesAsync(BookID);
        }

        public async Task<Result<bool>> CheckBookCriteriaAsync(clsMember memberInfo, bool hasReservation)
        {
            if (hasReservation)
            {
                Result<bool> reservationAvailabeResult = await memberInfo.IsReservationAvailableAsync(this.BookID);
                if (!reservationAvailabeResult.Success)
                {
                    return new Result<bool>(false,reservationAvailabeResult.Message,false,reservationAvailabeResult.ErrorCode);
                }
                if (!reservationAvailabeResult.Data)
                {
                    return new Result<bool>(false, new { error = new { header = "Not Available", body = "The book is not available yet!" } }, false, 422);
                }
            }

            Result<bool> isAvailabeResult = await this.IsAvaiableAsync();
            if (!isAvailabeResult.Success)
            {
                return isAvailabeResult;
            }
            if (!isAvailabeResult.Data)
            {
                return new Result<bool>(false, new { error = new { header = "Checked Out", body = "This book is checked out!" } }, false, 422);
            }
            return new Result<bool>(true, new { success = new { header = "Success", body = "All book criteria met" } }, true);
        }
        public static async Task<Result<bool>> ValidateDataAsync( AddNewBookDTO addNewBookDTO)
        {
            if (string.IsNullOrEmpty(addNewBookDTO.bookDTO.Title) ||
                addNewBookDTO.bookDTO.AuthorID <= 0 ||
                string.IsNullOrEmpty(addNewBookDTO.bookDTO.ISBN) ||
                !clsBookData.IsISBN(addNewBookDTO.bookDTO.ISBN) ||
                addNewBookDTO.bookDTO.PublisherID <= 0 ||
                addNewBookDTO.bookDTO.CategoryID <= 0 ||
                string.IsNullOrEmpty(addNewBookDTO.bookDTO.Location) ||
                addNewBookDTO.bookDTO.BorrowFees < 0
                || addNewBookDTO.numberOfCopies <= 0 
                )
            {
                return new Result<bool>(false, new { error = new { header = "Bad Request", body = "The request is invalid. Please check the input and try again." } }, false, 400);
            }
            Result<bool> result = await clsBookData.IsBookExistByISBN(addNewBookDTO.bookDTO.ISBN);
            if (!result.Success)
            {
                return result;  
            }
            if (result.Data)
            {
                return new Result<bool>(false, new { error = new { header = "Bad Request", body = "This ISBN is already exist in the system." } }, false, 400);
            }
            return new Result<bool>(true, new { success = new { header = "Success", body = "The data is valid." } }, true);
        }
        public static async Task<Result<bool>> ValidateDataAsync(BookDTO bookDTO, string currentISBN)
        {
            if (string.IsNullOrEmpty(bookDTO.Title) ||
                bookDTO.AuthorID <= 0 ||
                string.IsNullOrEmpty(bookDTO.ISBN) ||
                !clsBookData.IsISBN(bookDTO.ISBN) ||
                bookDTO.PublisherID <= 0 ||
                bookDTO.CategoryID <= 0 ||
                string.IsNullOrEmpty(bookDTO.Location) ||
                bookDTO.BorrowFees < 0
                )
            {
                return new Result<bool>(false, new { error = new { header = "Bad Request", body = "The request is invalid. Please check the input and try again." } }, false, 400);
            }
            if (bookDTO.ISBN != currentISBN)
            {
                Result<bool> result = await clsBookData.IsBookExistByISBN(bookDTO.ISBN);
                if (!result.Success)
                {
                    return result;
                }
                if (result.Data)
                {
                    return new Result<bool>(false, new { error = new { header = "Bad Request", body = "This ISBN is already exist in the system." } }, false, 400);
                }
            }
            return new Result<bool>(true, new { success = new { header = "Success", body = "The data is valid." } }, true);
        }
        public static async Task<Result<List<BookViewDTO>>> GetAllBooksAsync()
        {
            return await clsBookData.GetAllBooksAsync();
        }
        public static async Task<Result<List<int>>>  InserNumberOfCopiesAsync(InsertCopiesDTO insertCopiesDTO)
        {
            Result<clsBook> result = await FindAsync(insertCopiesDTO.BookID);
            if (!result.Success)
            {
                return new Result<List<int>>(false, result.Message, null, result.ErrorCode);
            }
            return await clsBookCopyData.InsertNumberOfCopiesForBookIDAsync(insertCopiesDTO);
        }
       public static async Task<Result<int>> GetTotalBooksAsync()
        {
            return await clsBookCopyData.GetTotalBooksAsync();
        }
        public static async Task<Result<bool>> DeleteAsync(int id)
        {
            if (id <= 0)
            {
                return new Result<bool>(false, new { error = new { header = "Bad Request", body = "The request is invalid. Please check the input and try again." } }, false, 400);
            }
            Result<FullBookDTO> result = await clsBookData.GetBookInfoByIDAsync(id);
            if (!result.Success)
            {
                return new Result<bool>(false, result.Message, false, result.ErrorCode);
            }

            return await clsBookData.DeleteBookAsync(id);
        }
    }
}
