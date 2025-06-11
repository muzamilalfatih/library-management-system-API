using LibraryManagementSystemAPIDataAccessLayer;
using SharedClasses;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static SharedClasses.AuthorDTOs;
using static SharedClasses.BookDTOs;
using static SharedClasses.PublisherDTOs;

namespace LibraryManagementSystemAPIBussinesLayer
{
    public class clsPublisher
    {
        public enum enMode { AddNew = 0, Update = 1 }
        private enMode _Mode;

        public int ID { get; set; }
        public int PersonID { get; set; }
        public clsPerson PersonInfo { get; set; }
        public DateTime CreatedDate { get; set; }
        public clsPublisher(PublisherDTO publisherDTO, enMode mode = enMode.AddNew)
        {
            this.ID = publisherDTO.PublisherID;
            this.PersonID = publisherDTO.PersonID;
            this.PersonInfo = new clsPerson(publisherDTO.PersonInfoDTO, (clsPerson.enMode)mode);
            this.CreatedDate = publisherDTO.CreatedDate;
            _Mode = mode;
        }

        public PublisherDTO PDTO
        {
            get
            {
                return new PublisherDTO(this.ID, this.PersonID, this.CreatedDate, this.PersonInfo.PDTO);
            }
        }
        public static async Task<Result<clsPublisher>> FindAsync(int PublisherID)
        {
            if (PublisherID <= 0)
            {
                return new Result<clsPublisher>(
                    false,
                    new { error = new { header = "Bad Request", body = "The request is invalid. Please check the input and try again." } },
                    null,
                    400
                );
            }

            Result<PublisherDTO> result = await clsPublisherData.GetPublisherInfoByIDAsync(PublisherID);

            if (result.Success)
            {
                return new Result<clsPublisher>(true, result.Message, new clsPublisher(result.Data, enMode.Update));
            }

            return new Result<clsPublisher>(result.Success, result.Message, null, result.ErrorCode);
        }
        private async Task<Result<int>> _AddNewPublisherAsync()
        {
            Result<int> result = await PersonInfo.SaveAsync();

            if (!result.Success)
            {
                return result;
            }

            this.PersonID = result.Data;
            return await clsPublisherData.AddNewPublisherAsync(this.PersonID);
        }
        private async Task<Result<int>> _UpdatePublisherAsync(string originalNationalNumber)
        {
            return await PersonInfo.SaveAsync();
        }
        public static async Task<Result<bool>> DeletePublisherAsync(int PublisherID)
        {
            if (PublisherID <= 0)
            {
                return new Result<bool>(
                    false,
                    new { error = new { header = "Bad Request", body = "The request is invalid. Please check the input and try again." } },
                    false,
                    400
                );
            }
            Result<PublisherDTO> result = await clsPublisherData.GetPublisherInfoByIDAsync(PublisherID);
            if (!result.Success)
            {
                return new Result<bool>(false, result.Message, false, result.ErrorCode);
            }
            return await clsPublisherData.DeletePublisherAsync(PublisherID);
        }
        public static async Task<Result<List<PublisherViewDTO>>> GetAllPublishersAsync()
        {
            return await clsPublisherData.GetAllPublishersAsync();
        }
        public static async Task<Result<string>> GetPublisherNameAsync(int PublisherID)
        {
            return await clsPublisherData.GetPublisherNameAsync(PublisherID);
        }
        public static async Task<Result<int>> GetPublisherIDAsync(string PublisherName)
        {
            return await clsPublisherData.GetPublisherIDAsync(PublisherName);
        }
        public static async Task<Result<int>> GetTotalPublishersAsync()
        {
            return await clsPublisherData.GetTotalPublishersAsync();
        }
        public static async Task<Result<List<BookViewDTO>>> GetAllBooksAsync(int PublisherID)
        {
            return await clsBookData.GetAllBooksForPublisherIDAsync(PublisherID);
        }
        public async Task<Result<int>> SaveAsync(string originalNationalNumber = "")
        {
            switch (_Mode)
            {
                case enMode.AddNew:
                    Result<int> addResult = await _AddNewPublisherAsync();

                    if (addResult.Success)
                    {
                        this.ID = addResult.Data;
                        _Mode = enMode.Update;
                    }

                    return addResult;

                case enMode.Update:
                    return await _UpdatePublisherAsync(originalNationalNumber);

                default:
                    return new Result<int>(
                        false,
                        new { error = new { header = "Server Error", body = "An unexpected error occurred on the server." } },
                        -1,
                        500
                    );
            }
        }

    }
}
