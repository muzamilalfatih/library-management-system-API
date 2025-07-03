using LibraryManagementSystemAPIBussinesLayer;
using LibraryManagementSystemAPIDataAccessLayer;
using SharedClasses;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static SharedClasses.AuthorDTOs;
using static SharedClasses.BookCategoryDTOs;
using static SharedClasses.UserDTOS;

namespace LibraryManagementSystemAPIDataAccessLayer
{
    public class clsBookCategory
    {
        public enum enMode { AddNew = 0, Update = 1 }
        private enMode _Mode = enMode.AddNew;

        public int CategoryID { get; set; }
        public string CategoryName { get; set; }

        public clsBookCategory(BookCategoryDTO bookCategoryDTO, enMode mode = enMode.AddNew)
        {
            this.CategoryID = bookCategoryDTO.CategoryID;
            this.CategoryName = bookCategoryDTO.CategoryName;
            _Mode = mode;
        }

        public BookCategoryDTO BCDTO
        {
            get
            {
                return new BookCategoryDTO(this.CategoryID, this.CategoryName);
            }
        }

        public static async Task<Result<int>> GetCategoryIDAsync(string CategoryName)
        {
            if (string.IsNullOrEmpty(CategoryName))
            {
                return new Result<int>(false, "Category name must have a value.", -1, 400);
            }
            return await clsBookCategoryData.GetCategoryIDByNameAsync(CategoryName);
        }
        public static async Task<Result<string>> GetCategoryNameAsync(int CategoryID)
        {
            if (CategoryID <= 0)
            {
                return new Result<string>(false, "The request is invalid. Please check the input and try again.", "", 400);
            }
            return await clsBookCategoryData.GetCategoryNameByIDAsync(CategoryID);
        }

        static public async Task<Result<clsBookCategory>> FindAsync(int CategoryID)
        {
            if (CategoryID <= 0)
            {
                return new Result<clsBookCategory>(false, "The request is invalid. Please check the input and try again.", null, 400);
            }

            Result<BookCategoryDTO> result = await clsBookCategoryData.GetCategoryInfoByIDAsync(CategoryID);
            if (result.Success)
            {
                return new Result<clsBookCategory>(true, "Book category retrieved successfully.", new clsBookCategory(result.Data, enMode.Update));
            }
            return new Result<clsBookCategory>(result.Success, result.Message, null, result.ErrorCode);
        }
        private async Task<Result<int>> _AddNewCategoryAsync()
        {
            return await clsBookCategoryData.AddNewCategoryAsync(this.CategoryName);
        }
        private async Task<Result<int>> _UpdateCategoryAsync()
        {
            return await clsBookCategoryData.UpdateCategoryAsync(BCDTO);
        }

        public static async Task<Result<List<BookCategoryDTO>>> GetAllCategoriesAsync()
        {
            return await clsBookCategoryData.GetAllCategoriesAsync();
        }
        public async Task<Result<int>> SaveAsync()
        {
            if (this.CategoryName == string.Empty)
            {
                return new Result<int>(false, "The request is invalid. Please check the input and try again.", -1, 400);
            }
            switch (_Mode)
            {
                case enMode.AddNew:
                    Result<int> result = await _AddNewCategoryAsync();
                    if (result.Success)
                    {
                        _Mode = enMode.Update;
                        this.CategoryID = result.Data;
                    }
                    return result;

                case enMode.Update:
                    return await _UpdateCategoryAsync();

                default:
                    return new Result<int>(false, "An unexpected error occurred on the server.", -1, 500);
            }
        }

    }
}
