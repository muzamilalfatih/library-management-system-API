using LibraryManagementSystemAPIDataAccessLayer;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static SharedClasses.AuthorDTOs;
using SharedClasses;
using static SharedClasses.BookDTOs;

namespace LibraryManagementSystemAPIBussinesLayer
{
    public class clsAuthor
    {
        public enum enMode { AddNew = 0, Update = 1 };
        private enMode _Mode;
        public int ID { get; set; }
        public int PersonID { get; set; }
        public DateTime CreatedDate { get; set; }

        public clsAuthor(AuthorDTO auhtorDTO, enMode mode = enMode.AddNew)
        {
            this.ID = auhtorDTO.AuthorID;
            this.PersonID = auhtorDTO.PersonID;
            this.CreatedDate = auhtorDTO.CreatedDate;
            _Mode = mode;
        }

        public AuthorDTO ADTO
        {
            get
            {
                return new AuthorDTO(this.ID, this.PersonID, this.CreatedDate);
            }
        }

        static public async Task<Result<clsAuthor>> FindAsync(int authorID)
        {
            if (authorID <= 0)
            {
                return new Result<clsAuthor>(false, "The request is invalid. Please check the input and try again.", null, 400);
            }

            Result<AuthorDTO> result = await clsAuthorData.GetAuthorInfoByIDAsync(authorID); 
            if (result.Success)
            {
                return new Result<clsAuthor>(true, "Author Found.", new clsAuthor(result.Data, enMode.Update));
            }

            return new Result<clsAuthor>(result.Success, result.Message, null, result.ErrorCode);
        }

        private async Task<Result<int>> _AddNewAuthorAsync()
        {

            return await clsAuthorData.AddNewAuthorAsync(this.PersonID); // Make it async
        }
        
       
        //private async Task<Result<int>> _UpdateAuthorAsync( )
        //{
            
        //}
        public static async Task<Result<bool>> DeleteAuthorAsync(int AuthorID)
        {
            if (AuthorID <= 0)
            {
                return new Result<bool>(false, "The request is invalid. Please check the input and try again.", false, 400);
            }
            Result<AuthorDTO> result = await clsAuthorData.GetAuthorInfoByIDAsync(AuthorID);
            if (!result.Success)
            {
                new Result<bool>(false, result.Message, false, result.ErrorCode);
            }
            return await clsAuthorData.DeleteAuthorAsync(AuthorID); // Make it async
        }

        static public async Task<Result<List<AuthorViewDTO>>> GetAllAuthorsAsync()
        {
            return await clsAuthorData.GetAllAuthorsAsync(); // Make it async
        }

        public static async Task<Result<string>> GetAuthorNameAsync(int AuthorID)
        {
            return await clsAuthorData.GetAuthorNameAsync(AuthorID); // Make it async
        }

        public static async Task<Result<int>> GetAuthorIDAsync(string AuthorName)
        {
            return await clsAuthorData.GetAuthorIDAsync(AuthorName); // Make it async
        }

        public static async Task<Result<int>> GetTotalAuthorsAsync()
        {
            return await clsAuthorData.GetTotalAuthorsAsync(); // Make it async
        }

        public static async Task<Result<List<BookViewDTO>>> GetAllBooksAsync(int authorID)
        {
            if (authorID <= 0)
            {
                return new Result<List<BookViewDTO>>(false, "The request is invalid. Please check the input and try again.", new List<BookViewDTO>(), 400);
            }

            Result<clsAuthor> result = await FindAsync(authorID); // Make it async
            if (!result.Success)
            {
                return new Result<List<BookViewDTO>>(false, "Author not found.", new List<BookViewDTO>(), 404);
            }

            return await clsBookData.GetAllBooksForAuthorIDAsync(authorID); // Make it async
        }

        public async Task<Result<int>> SaveAsync()
        {
            switch (_Mode)
            {
                case enMode.AddNew:
                    Result<int> result = await _AddNewAuthorAsync(); 
                    if (result.Success)
                    {
                        this.ID = result.Data;
                        _Mode = enMode.Update;
                    }
                    return result;

                case enMode.Update:
                    //return await _UpdateAuthorAsync(); 

                default:
                    return new Result<int>(false, "An unexpected error occurred on the server.", -1, 500);
            }
        }

    }
}
