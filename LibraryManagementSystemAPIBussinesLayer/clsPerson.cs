using Azure.Core;
using LibraryManagementSystemAPIDataAccessLayer;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Runtime.CompilerServices;
using System.Security;
using System.Text;
using System.Threading.Tasks;
using SharedClasses;
using static SharedClasses.PersonDTOs;
using System.Text.RegularExpressions;
namespace LibraryManagementSystemAPIBussinesLayer
{
    public class clsPerson
    {
        public enum enMode { AddNew = 0, Update = 1 }
        public enum enGender { Male = 0, Female = 1 }
        private enMode _Mode;

        public int PersonID { get; set; }
        public string NationalNo { get; set; }
        public string FirstName { get; set; }
        public string SecondName { get; set; }
        public string? ThirdName { get; set; }
        public string LastName { get; set; }
        public string FullName
        {
            get
            {
                return FirstName + " " + SecondName + " " + ThirdName + " " + LastName;
            }
        }
        public enGender Gender { get; set; }
        public string GenderText()
        {
            return this.Gender == enGender.Male ? "Male" : "Female";
        }
        public string Email { get; set; }
        public string Phone { get; set; }
        public string Address { get; set; }

        public PersonDTO PDTO
        {
            get
            {
                return new PersonDTO(this.PersonID, this.NationalNo, this.FirstName, this.SecondName, this.ThirdName, this.LastName,
                    (int)this.Gender, this.Email, this.Phone, this.Address);
            }
        }

        public clsPerson(PersonDTO personDTO, enMode mode = enMode.AddNew)
        {
            this.PersonID = personDTO.PersonID;
            this.NationalNo = personDTO.NationalNo;
            this.FirstName = personDTO.FirstName;
            this.SecondName = personDTO.SecondName;
            this.ThirdName = personDTO.ThirdName;
            this.LastName = personDTO.LastName;
            this.Gender = (enGender)personDTO.Gender;
            this.Email = personDTO.Email;
            this.Phone = personDTO.Phone;
            this.Address = personDTO.Address;
            _Mode = mode;
        }

        public static async Task<Result<clsPerson>> FindAsync(int PersonID)
        {
            if (PersonID <= 0)
            {
                return new Result<clsPerson>(false, "The request is invalid. Please check the input and try again.", null, 400);
            }

            Result<PersonDTO> result = await clsPersonData.GetPersonInfoByIDAsync(PersonID); // Async call
            if (result.Success)
            {
                return new Result<clsPerson>(true, result.Message, new clsPerson(result.Data, enMode.Update));
            }

            return new Result<clsPerson>(result.Success, result.Message, null, result.ErrorCode);
        }

        public static bool IsValidEmail(string email)
        {
            if (string.IsNullOrWhiteSpace(email))
                return false;
            string emailPattern = @"^[^@\s]+@[^@\s]+\.[^@\s]+$";
            return Regex.IsMatch(email, emailPattern);
        }
        public static  async Task<Result<bool>> ValidateDataAsync(PersonDTO personDTO)
        {
            if (string.IsNullOrEmpty(personDTO.NationalNo) ||
                string.IsNullOrEmpty(personDTO.FirstName) ||
                string.IsNullOrEmpty(personDTO.SecondName) ||
                string.IsNullOrEmpty(personDTO.LastName) ||
                (int)personDTO.Gender != 0 && (int)personDTO.Gender != 1 ||
                IsValidEmail(personDTO.Email) ||
                string.IsNullOrEmpty(personDTO.Phone))
            {
                return new Result<bool>(false, "The request is invalid. Please check the input and try again.", false, 400);
            }
            Result<bool> result = await clsPersonData.IsPersonExistAsync(personDTO.NationalNo);
            if (!result.Success)
            {
                return result;
            }
            if (result.Data)
            {
                return new Result<bool>(false, "This national number is used by another person", false, 400);
            }
            return new Result<bool>(true, "Data validattion completed", true);
        }
        public static async Task<Result<bool>> ValidateDataAsync(PersonDTO personDTO, string currentNationalNumber)
        {
            if (string.IsNullOrEmpty(personDTO.NationalNo) ||
                string.IsNullOrEmpty(personDTO.FirstName) ||
                string.IsNullOrEmpty(personDTO.SecondName) ||
                string.IsNullOrEmpty(personDTO.LastName) ||
                (int)personDTO.Gender != 0 && (int)personDTO.Gender != 1 ||
                IsValidEmail(personDTO.Email) ||
                string.IsNullOrEmpty(personDTO.Phone))
            {
                return new Result<bool>(false, "The request is invalid. Please check the input and try again.", false, 400);
            }
            if (personDTO.NationalNo != currentNationalNumber)
            {
                Result<bool> result = await clsPersonData.IsPersonExistAsync(personDTO.NationalNo);
                if (!result.Success)
                {
                    return result;
                }
                if (result.Data)
                {
                    return new Result<bool>(false, "This national number is used by another person", false, 400);
                }
            }          
            return new Result<bool>(true, "Data validattion completed", true);
        }
        private async Task<Result<int>> _AddNewPersonAsync()
        {
            return await clsPersonData.AddNewPersonAsync(PDTO); // Async call
        }

        private async Task<Result<int>> _UpdatePersonAsync()
        {
            return await clsPersonData.UpdatePersonAsync(PDTO); // Async call
        }

        public static async Task<Result<bool>> IsPersonExistAsync(string NationalNo)
        {
            return await clsPersonData.IsPersonExistAsync(NationalNo); // Async call
        }

        public bool SendEmail(string Body, string Subject)
        {
            // Implement the code that sends an email here
            return true;
        }

        public async Task<Result<bool>> DeleteAsync()
        {
            return await clsPersonData.DeletePersonAsync(PersonID); // Async call
        }

        public async Task<Result<int>> SaveAsync()
        {
            switch (_Mode)
            {
                case enMode.AddNew:
                    Result<int> result = await _AddNewPersonAsync();
                    if (result.Success)
                    {
                        this.PersonID = result.Data;
                        _Mode = enMode.Update;
                    }
                    return result;

                case enMode.Update:
                    return await _UpdatePersonAsync(); 

                default:
                    return new Result<int>(false, "An unexpected error occurred on the server.", -1, 500);
            }
        }

    }
}
