using LibraryManagementSystemAPIDataAccessLayer;
using SharedClasses;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static SharedClasses.MembershipClassDTOs;
using static SharedClasses.MembershipDTOs;
using static SharedClasses.UserDTOS;

namespace LibraryManagementSystemAPIBussinesLayer
{
    

    public class clsMembership
    {
        public enum enMode { AddNew = 0, Update = 1 };
        private enMode _Mode;

        public int MembershipID { get; set; }
        public int MembershipClassID { get; set; }
        public int CreatedByUserID { get; set; }
        public string? CreatedByUserName { get; set; }
        public int MemberID { get; set; }
        public clsMembershipClass? MembershipClassInfo { get; set; }
        public DateTime MembershipStartDate { get; set; }
        public DateTime MembershipExpirationDate { get; set; }
        public float PaidFees { get; set; }
        public clsMembership(FullMembershipDTO membershipDTO, enMode mode = enMode.AddNew)
        {
            this.MembershipID = membershipDTO.MembershipID;
            this.MembershipClassID = membershipDTO.MembershipClassID;
            this.MembershipClassInfo = new clsMembershipClass(membershipDTO.MembershipClassInfo, (clsMembershipClass.enMode) mode);
            this.MembershipStartDate = membershipDTO.MembershipStartDate;
            this.MembershipExpirationDate = membershipDTO.MembershipExpirationDate;
            this.PaidFees = membershipDTO.PaidFees;
            this.CreatedByUserID = membershipDTO.CreatedByUserID;
            CreatedByUserName = membershipDTO.CreatedByUserName;
            _Mode = mode;
        }
        public clsMembership(MembershipDTO addNewMembershipDTO, enMode mode = enMode.AddNew)
        {

            this.MemberID = addNewMembershipDTO.MemberID;
            this.MembershipClassID = addNewMembershipDTO.MembershipClassID;
            this.MembershipStartDate = addNewMembershipDTO.MembershipStartDate;
            this.MembershipExpirationDate = addNewMembershipDTO.MembershipExpirationDate;
            this.PaidFees = addNewMembershipDTO.PaidFees;
            this.CreatedByUserID = addNewMembershipDTO.CreatedByUserID;
            this.CreatedByUserName = null;
            this.MembershipClassInfo = null;
            _Mode = mode;
        }
        public static async Task<Result<clsMembership>> CreateAsync(MembershipDTO addNewMembershipDTO, enMode mode = enMode.AddNew)
        {
            clsMembership membership = new clsMembership(addNewMembershipDTO, mode);

            Result<string> userResult = await clsUser.GetUserNameAsync(addNewMembershipDTO.CreatedByUserID);
            if (!userResult.Success)
            {
                return new Result<clsMembership>(false, userResult.Message, null, userResult.ErrorCode);
            }

            Result<clsMembershipClass> classResult = await clsMembershipClass.FindAsync(addNewMembershipDTO.MembershipClassID);
            if (!classResult.Success)
            {
                return new Result<clsMembership>(false, classResult.Message, null, classResult.ErrorCode);
            }
            membership.CreatedByUserName = userResult.Data;
            membership.MembershipClassInfo = classResult.Data;
            return new Result<clsMembership>(true, "Membership object created successfully.", membership);
        }
        public FullMembershipDTO FMSDTO
        {
            get
            {
                return new FullMembershipDTO(this.MembershipID, this.MemberID, this.MembershipClassID, this.MembershipStartDate, this.MembershipExpirationDate,
                    this.PaidFees, this.CreatedByUserID, this.MembershipClassInfo.MSCDTO, this.CreatedByUserName);
            }
        }
        public MembershipDTO MSDTO
        {
            get
            {
                return new MembershipDTO(this.MemberID, this.MembershipClassID, this.MembershipStartDate, this.MembershipExpirationDate, this.PaidFees, this.CreatedByUserID);
            }
        }
        public static async Task<Result<clsMembership>> FindAsync(int MembershipID)
        {
            if (MembershipID <= 0)
            {
                return new Result<clsMembership>(false, "The request is invalid. Please check the input and try again.", null, 400);
            }
            var result = await clsMembershipData.GetMembershipByIDAsync(MembershipID);
            if (result.Success)
            {
                return new Result<clsMembership>(true, result.Message, new clsMembership(result.Data, enMode.Update));
            }
            return new Result<clsMembership>(false, result.Message, null, result.ErrorCode);
        }

        public static async Task<Result<clsMembership>> FindByMemberIDAsync(int MemberID)
        {
            if (MemberID <= 0)
            {
                return new Result<clsMembership>(false, "The request is invalid. Please check the input and try again.", null, 400);
            }
            var result = await clsMembershipData.GetMembershipByMemberIDAsync(MemberID);
            if (result.Success)
            {
                return new Result<clsMembership>(true, result.Message, new clsMembership(result.Data, enMode.Update));
            }
            return new Result<clsMembership>(false, result.Message, null, result.ErrorCode);
        }

        public Result<bool> IsActive()
        {
            bool isActive = this.MembershipExpirationDate >= DateTime.Now;
            return new Result<bool>(isActive, isActive ? "Yes" : "No", isActive);
        }

        private async Task<Result<int>> _AddNewMembershipAsync()
        {
            return await clsMembershipData.AddNewMembershipAsync(this.MSDTO);
        }

        private async Task<Result<int>> _UpdateMembershipAsync()
        {
            return await clsMembershipData.UpdateMembershipAsync(this.FMSDTO);
        }

        public static async Task<Result<bool>> DeleteAsync(int membershipID)
        {
            return await clsMembershipData.DeleteMembershipAsync(membershipID);
        }

        public async Task<Result<int>> SaveAsync()
        {
            switch (_Mode)
            {
                case enMode.AddNew:
                    var result = await _AddNewMembershipAsync();
                    if (result.Success)
                    {
                        this.MembershipID = result.Data;
                        _Mode = enMode.Update;
                    }
                    return result;
                case enMode.Update:
                    return await _UpdateMembershipAsync();
                default:
                    return new Result<int>(false, "An unexpected error occurred on the server.", -1, 500);
            }
        }

    }
    
}
