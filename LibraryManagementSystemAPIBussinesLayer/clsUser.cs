using Azure.Core;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Threading.Tasks;
using static SharedClasses.UserDTOS;
using LibraryManagementSystemAPIDataAccessLayer;
using SharedClasses;
using static SharedClasses.ReservationDTOs;
using static SharedClasses.PersonDTOs;

namespace LibraryManagementSystemAPIBussinesLayer
{
    public class ResponseUserDataDTO
    {
        public ResponseUserDataDTO(int userID, string userName, int personID, string userRole, bool isActive, PersonDTO personInfo)
        {
            UserID = userID;
            UserName = userName;
            PersonID = personID;
            PersonInfo = personInfo;
            UserRole = userRole;
            IsActive = isActive;
        }

        public int UserID { get; set; }
        public string UserName { get; set; }
        public int PersonID { get; set; }
        public string UserRole { get; set; }
        public bool IsActive { get; set; }
        public PersonDTO PersonInfo { get; set; }
    }

    public class clsUser
    {
        public enum enMode { AddNew = 0, Update = 1 };
        private enMode _Mode;
        public enum enUserRole { Librarian = 1, Admin = 0 };
        public int UserID { get; set; }
        public int PersonID { get; set; }
        public clsPerson PersonInfo { get; set; }
        public string UserName { get; set; }
        public string Password { get; set; }
        public enUserRole UserRole { get; set; }
        public string UserRoleText()
        {
            string Role = this.UserRole == enUserRole.Admin ? "Admin" : "Librarian";
            return Role;
        }
        public bool IsActive { get; set; }
        public UserDTO UDTO
        {
            get
            {
                return new UserDTO(this.UserID, this.PersonID, this.UserName, this.Password, (int)this.UserRole, this.IsActive, PersonInfo.PDTO);
            }
        }
        public ResponseUserDataDTO ReponseDataDTO
        {
            get
            {
                return new ResponseUserDataDTO(this.UserID, this.UserName, this.PersonID, this.UserRoleText(), this.IsActive, this.PersonInfo.PDTO);
            }
        }
        public clsUser(UserDTO userDTO, enMode mode = enMode.AddNew)
        {
            this.UserID = userDTO.UserID;
            this.PersonID = userDTO.PersonID;
            this.PersonInfo = new clsPerson(userDTO.PersonInfoDTO, (clsPerson.enMode)mode);
            this.UserName = userDTO.UserName;
            if (mode == enMode.AddNew)
            {
                this.Password = Utility.ComputeHash(userDTO.Password);
            }
            else
            {
                this.Password = userDTO.Password;
            }
            this.UserRole = (enUserRole)userDTO.Role;
            this.IsActive = userDTO.IsActive;
            _Mode = mode;
        }

        static public async Task<Result<clsUser>> FindAsync(int UserID)
        {
            if (UserID <= 0)
            {
                return new Result<clsUser>(false, "The request is invalid. Please check the input and try again.", null, 400);
            }
            Result<UserDTO> result = await clsUserData.GetUserInfoByUserIDAsync(UserID);
            if (result.Success)
            {
                return new Result<clsUser>(true, result.Message, new clsUser(result.Data, enMode.Update));
            }
            return new Result<clsUser>(result.Success, result.Message, null, result.ErrorCode);
        }

        static public async Task<Result<clsUser>> FindAsync(string UserName)
        {
            if (string.IsNullOrWhiteSpace(UserName))
            {
                return new Result<clsUser>(false, "The request is invalid. Please check the input and try again.", null, 400);
            }
            Result<UserDTO> result = await clsUserData.GetUserInfoByUserNameAsync(UserName);
            if (result.Success)
            {
                return new Result<clsUser>(true, result.Message, new clsUser(result.Data, enMode.Update));
            }
            return new Result<clsUser>(result.Success, result.Message, null, result.ErrorCode);
        }
        private async Task<Result<int>> _AddNewUserAsync()
        {
            Result<int> result = await PersonInfo.SaveAsync();
            if (!result.Success)
            {
                return result;
            }

            this.PersonID = result.Data;

            return await clsUserData.AddNewUserAsync(UDTO);
        }
        private async Task<Result<int>> _UpdateUserAsync()
        {
            Result<int> result = await PersonInfo.SaveAsync();
            if (!result.Success)
            {
                return result;
            }

            return await clsUserData.UpdateUserAsync(UDTO);
        }
        public static async Task<Result<bool>> DeleteUserAsync(int UserID)
        {
            if (UserID <= 0)
            {
                return new Result<bool>(false, "The request is invalid. Please check the input and try again.", false, 400);
            }
            Result<UserDTO> result = await clsUserData.GetUserInfoByUserIDAsync(UserID);
            if (!result.Success)
            {
                return new Result<bool>(false, result.Message, false, result.ErrorCode);
            }
            return await clsUserData.DeleteUserAsync(UserID);
        }

        public static async Task<Result<List<UserViewDTO>>> GetAllUsersAsync()
        {
            return await clsUserData.GetAllUsersAsync();
        }

        public static async Task<Result<bool>> IsUserExsitAsync(string UserName)
        {
            return await clsUserData.IsUserExistAsync(UserName);
        }

        public static async Task<Result<int>> GetTotalUsersAsync()
        {
            return await clsUserData.GetTotalUsersAsync();
        }

        public static async Task<Result<bool>> IsThereOtherAdminsAsync(int CurrentUserID)
        {
            if (CurrentUserID <= 0)
            {
                return new Result<bool>(false, "The request is invalid. Please check the input and try again.", false, 400);
            }
            return await clsUserData.IsThereOtherAdminsAsync(CurrentUserID);
        }

        public static async Task<Result<ResponseUserDataDTO>> LoginAsync(LoginDTO loginDTO)
        {
            Result<clsUser> result = await FindAsync(loginDTO.UserName);
            if (!result.Success)
            {
                return new Result<ResponseUserDataDTO>(false, "Invalid username or password.", null, 401);
            }
            if (result.Data.Password != Utility.ComputeHash(loginDTO.Password))
            {
                return new Result<ResponseUserDataDTO>(false, "Invalid username or password.", null, 401);
            }
            return new Result<ResponseUserDataDTO>(true, "User logged in successfully.", result.Data.ReponseDataDTO);
        }

        public bool SendEmail(string Body, string Subject)
        {
            return true;
        }

        public static async Task<Result<bool>> ValidateDataAsync(UserDTO userDTO)
        {
            Result<bool> personInfoValidationResult = await clsPerson.ValidateDataAsync(userDTO.PersonInfoDTO);
            if (!personInfoValidationResult.Success)
            {
                return personInfoValidationResult;
            }
            Result<bool> userExestenseResult = await clsUserData.IsUserExistAsync(userDTO.UserName);
            if (!userExestenseResult.Success)
            {
                return userExestenseResult;
            }
            if (userExestenseResult.Data)
            {
                return new Result<bool>(false, "This username is used by another person.", false, 400);
            }
            return new Result<bool>(true, "Validation complete.", true);
        }
        public static async Task<Result<bool>> ValidateDataAsync(UserDTO userDTO, string currentUserName, string currentNationalNumber)
        {
            if (userDTO.UserName == "" || userDTO.Password == "")
            {
                return new Result<bool>(false, "The request is invalid. Please check the input and try again.", false, 400);
            }
            Result<bool> personInfoValidationResult = await clsPerson.ValidateDataAsync(userDTO.PersonInfoDTO, currentNationalNumber);
            if (!personInfoValidationResult.Success)
            {
                return personInfoValidationResult;
            }
            if (userDTO.UserName != currentUserName)
            {
                Result<bool> userExestenseResult = await clsUserData.IsUserExistAsync(userDTO.UserName);
                if (!userExestenseResult.Success)
                {
                    return userExestenseResult;
                }
                if (userExestenseResult.Data)
                {
                    return new Result<bool>(false, "This username is used by another person.", false, 400);
                }
            }
            if ((enUserRole)userDTO.Role != enUserRole.Admin || !userDTO.IsActive)
            {
                Result<bool> checkOtherAdminsResult = await clsUserData.IsThereOtherAdminsAsync(userDTO.UserID);
                if (!checkOtherAdminsResult.Success)
                {
                    return checkOtherAdminsResult;
                }
                if (checkOtherAdminsResult.Data)
                {
                    return new Result<bool>(false, "At least must be one active admin in the system.", false, 400);
                }
            }
            return new Result<bool>(true, "Validation complete.", true);
        }
        public async Task<Result<int>> SaveAsync()
        {
            switch (_Mode)
            {
                case enMode.AddNew:
                    Result<int> result = await _AddNewUserAsync();
                    if (result.Success)
                    {
                        this.UserID = result.Data;
                        _Mode = enMode.Update;
                    }
                    return result;
                case enMode.Update:
                    return await _UpdateUserAsync();
                default:
                    return new Result<int>(false, "An unexpected error occurred on the server.", -1, 500);
            }
        }
        public static async Task<Result<UserDTO>> UpdateUserAsync(int id,  UserDTO userDTO)
        {
            
            Result<clsUser> findUserResult = await FindAsync(id);
            if (!findUserResult.Success)
            {
                return new Result<UserDTO>(false,findUserResult.Message, null, findUserResult.ErrorCode); 
            }
            Result<bool> validationResult = await ValidateDataAsync(userDTO, findUserResult.Data.UserName, findUserResult.Data.PersonInfo.NationalNo);
            if (!validationResult.Success)
            {
                return new Result<UserDTO>(false, validationResult.Message, null, validationResult.ErrorCode);
            }
            findUserResult.Data.UserName = userDTO.UserName;
            findUserResult.Data.Password = Utility.ComputeHash(userDTO.Password);
            findUserResult.Data.UserRole = (enUserRole)userDTO.Role;
            findUserResult.Data.IsActive = userDTO.IsActive;
            findUserResult.Data.PersonInfo.NationalNo = userDTO.PersonInfoDTO.NationalNo;
            findUserResult.Data.PersonInfo.FirstName = userDTO.PersonInfoDTO.FirstName;
            findUserResult.Data.PersonInfo.SecondName = userDTO.PersonInfoDTO.SecondName;
            findUserResult.Data.PersonInfo.ThirdName = userDTO.PersonInfoDTO.ThirdName;
            findUserResult.Data.PersonInfo.LastName = userDTO.PersonInfoDTO.LastName;
            findUserResult.Data.PersonInfo.Gender = (clsPerson.enGender)userDTO.PersonInfoDTO.Gender;
            findUserResult.Data.PersonInfo.Email = userDTO.PersonInfoDTO.Email;
            findUserResult.Data.PersonInfo.Phone = userDTO.PersonInfoDTO.Phone;

            Result<int> saveResult = await findUserResult.Data.SaveAsync();
            if (!saveResult.Success)
            {
                return new Result<UserDTO>(false, findUserResult.Message, null, findUserResult.ErrorCode);
            }
            return new Result<UserDTO>(true, saveResult.Message, findUserResult.Data.UDTO);
        }
        public static async Task<Result<string>> GetUserNameAsync(int  userId)
        {
            return await clsUserData.GetUserNameAsync(userId);
        }
    }

}
