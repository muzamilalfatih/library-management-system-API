﻿using LibraryManagementSystemAPIDataAccessLayer;
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
        public clsPerson PersonInfo { get; set; }

        public clsAuthor(AuthorDTO auhtorDTO, enMode mode = enMode.AddNew)
        {
            this.ID = auhtorDTO.AuthorID;
            this.PersonID = auhtorDTO.PersonID;
            this.PersonInfo = new clsPerson(auhtorDTO.PersonInfoDTO, (clsPerson.enMode)mode);
            this.CreatedDate = auhtorDTO.CreatedDate;
            _Mode = mode;
        }

        public AuthorDTO ADTO
        {
            get
            {
                return new AuthorDTO(this.ID, this.PersonID, this.CreatedDate, this.PersonInfo.PDTO);
            }
        }

        static public async Task<Result<clsAuthor>> FindAsync(int authorID)
        {
            if (authorID <= 0)
            {
                return new Result<clsAuthor>(false, new { error = new { header = "Bad Request", body = "The request is invalid. Please check the input and try again." } }, null, 400);
            }

            Result<AuthorDTO> result = await clsAuthorData.GetAuthorInfoByIDAsync(authorID); 
            if (result.Success)
            {
                return new Result<clsAuthor>(true, new { success = new { header = "Success", body = "Author Found." } }, new clsAuthor(result.Data, enMode.Update));
            }

            return new Result<clsAuthor>(result.Success, result.Message, null, result.ErrorCode);
        }

        private async Task<Result<int>> _AddNewAuthorAsync()
        {
            Result<int> result = await PersonInfo.SaveAsync();  // Make it async
            if (!result.Success)
            {
                return result;
            }

            this.PersonID = result.Data;

            return await clsAuthorData.AddNewAuthorAsync(this.PersonID); // Make it async
        }
        public static async Task<Result<bool>> VaidateData(AuthorDTO authorDTO)
        {
            return await clsPerson.ValidateDataAsync(authorDTO.PersonInfoDTO);
        }
        public static async Task<Result<bool>> VaidateData(AuthorDTO authorDTO, string currentNationalNumber)
        {
            return await clsPerson.ValidateDataAsync(authorDTO.PersonInfoDTO, currentNationalNumber);
        }
        private async Task<Result<int>> _UpdateAuthorAsync( )
        {
            return await PersonInfo.SaveAsync();
        }
        public static async Task<Result<bool>> DeleteAuthorAsync(int AuthorID)
        {
            if (AuthorID <= 0)
            {
                return new Result<bool>(false, new { error = new { header = "Bad Request", body = "The request is invalid. Please check the input and try again." } }, false, 400);
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
                return new Result<List<BookViewDTO>>(false, new { error = new { header = "Bad Request", body = "The request is invalid. Please check the input and try again." } }, new List<BookViewDTO>(), 400);
            }

            Result<clsAuthor> result = await FindAsync(authorID); // Make it async
            if (!result.Success)
            {
                return new Result<List<BookViewDTO>>(false, new { error = new { header = "Not Found", body = "Author not found." } }, new List<BookViewDTO>(), 404);
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
                    return await _UpdateAuthorAsync(); 

                default:
                    return new Result<int>(false, new { error = new { header = "Server Error", body = "An unexpected error occurred on the server." } }, -1, 500);
            }
        }

    }
}
