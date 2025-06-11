using LibraryManagementSystemAPIDataAccessLayer;
using SharedClasses;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static SharedClasses.MembershipClassDTOs;
using static SharedClasses.UserDTOS;
namespace LibraryManagementSystemAPIBussinesLayer
{
    public class clsMembershipClass
    {
        public enum enMode { AddNew = 0, Update = 1 }
        private enMode _Mode;

        public int ID { get; set; }
        public string Name { get; set; }
        public int MaxNumberOfBookCanBorrow { get; set; }
        public float FeesPerDay;

        public clsMembershipClass(MembershipClassDTO membershipClassDTO, enMode mode = enMode.AddNew)
        {
            this.ID = membershipClassDTO.MembershipClassID;
            this.Name = membershipClassDTO.MembershipClassName;
            this.MaxNumberOfBookCanBorrow = membershipClassDTO.MaxNumberOfBooksCanBorrow;
            this.FeesPerDay = membershipClassDTO.FeesPerDay;
            _Mode = mode;
        }

        public MembershipClassDTO MSCDTO
        {
            get
            {
                return new MembershipClassDTO(this.ID, this.Name, this.MaxNumberOfBookCanBorrow, this.FeesPerDay);
            }
        }
        public static async Task<Result<clsMembershipClass>> FindAsync(int MemberShipClassID)
        {
            if (MemberShipClassID <= 0)
            {
                return new Result<clsMembershipClass>(false, new { error = new { header = "Bad Request", body = "The request is invalid. Please check the input and try again." } }, null, 400);
            }

            Result<MembershipClassDTO> result = await clsMembershipClassData.GetMembershipClassInfoByIDAsync(MemberShipClassID);

            if (result.Success)
            {
                return new Result<clsMembershipClass>(true, result.Message, new clsMembershipClass(result.Data, enMode.Update));
            }

            return new Result<clsMembershipClass>(result.Success, result.Message, null, result.ErrorCode);
        }
        public static async Task<Result<List<MembershipClassDTO>>> GetAllMembershipClassesAsync()
        {
            return await clsMembershipClassData.GetAllMembershipClassesAsync();
        }
        private async Task<Result<int>> _AddNewMembershipClassAsync()
        {
            return await clsMembershipClassData.AddNewMembershipClassAsync(this.MSCDTO);
        }
        private async Task<Result<int>> _UpdateMembershipClassAsync()
        {
            return await clsMembershipClassData.UpdateMembershipClassAsync(this.MSCDTO);
        }
        public static async Task<Result<bool>> DeleteAsync(int membershipClassID)
        {
            if (membershipClassID <= 0)
            {
                return new Result<bool>(false, new { error = new { header = "Bad Request", body = "The request is invalid. Please check the input and try again." } }, false, 400);
            }
            Result<MembershipClassDTO> result = await clsMembershipClassData.GetMembershipClassInfoByIDAsync(membershipClassID);
            if (!result.Success)
            {
                return new Result<bool>(false, result.Message, false, result.ErrorCode);
            }
            return await clsMembershipClassData.DeleteMembershipClassAsync(membershipClassID);
        }
        public static async Task<Result<int>> GetMembershipClassIDAsync(string MembershipClassName)
        {
            return await clsMembershipClassData.GetMembershipClassIDByNameAsync(MembershipClassName);
        }
        public static async Task<Result<string>> GetMembershipClassNameAsync(int MembershipClassID)
        {
            return await clsMembershipClassData.GetMembershipClassNameByIDAsync(MembershipClassID);
        }
        public async Task<Result<int>> SaveAsync()
        {
            switch (_Mode)
            {
                case enMode.AddNew:
                    Result<int> addResult = await _AddNewMembershipClassAsync();
                    if (addResult.Success)
                    {
                        this.ID = addResult.Data;
                        _Mode = enMode.Update;
                    }
                    return addResult;

                case enMode.Update:
                    return await _UpdateMembershipClassAsync();

                default:
                    return new Result<int>(false, new { error = new { header = "Server Error", body = "An unexpected error occurred on the server." } }, -1, 500);
            }
        }

    }
}
