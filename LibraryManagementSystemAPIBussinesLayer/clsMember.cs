using Azure.Core;
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
using static SharedClasses.FineSettingsDTOs;
using static SharedClasses.MemberDTOs;
using static SharedClasses.MembershipDTOs;
using static SharedClasses.ReservationDTOs;
using static SharedClasses.UserDTOS;

namespace LibraryManagementSystemAPIBussinesLayer
{
    public class MemberResponseDataDTO
    {
        public MemberResponseDataDTO(int memberID, bool isActive, DateTime createdDate, string createdByUserName,
            string membershipClass, PersonDTOs.PersonDTO personInfoDTO)
        {
            MemberID = memberID;
            IsActive = isActive;
            CreatedDate = createdDate;
            CreatedByUserName = createdByUserName;
            MembershipClass = membershipClass;
            PersonInfoDTO = personInfoDTO;
        }

        public int MemberID { get; set; }
        public bool IsActive { get; set; }
        public DateTime CreatedDate { get; set; }
        public string CreatedByUserName { get; set; }
        public string MembershipClass { get; set; }
        public PersonDTOs.PersonDTO PersonInfoDTO { get; set; }
    }
    public class clsMember
    {
        public enum enMode { AddNew = 0, Update = 1 };
        private enMode _Mode;

        public int MemberID { get; set; }
        public int PersonID { get; set; }
        public bool IsActive { get; set; }
        public DateTime CreatedDate { get; set; }
        public int CreatedByUserID { get; set; }
        public clsUser CreatedByUserInfo { get; set; }
        public clsPerson PersonInfo { get; set; }
        public clsMembership MembershipInfo { get; set; }

        public clsMember(FullMemberDTO memberDTO, enMode mode = enMode.AddNew)
        {
            this.MemberID = memberDTO.MemberID;
            this.IsActive = memberDTO.IsActive;
            this.PersonID = memberDTO.PersonID;
            this.CreatedByUserID = memberDTO.CreatedByUserID;
            this.CreatedDate = memberDTO.CreatedDate;
            this.MembershipInfo = new clsMembership(memberDTO.MembershipInfoDTO, (clsMembership.enMode)mode);
            _Mode = enMode.Update;
        }
        public clsMember(ReceivedDataAddNewMemberDTO memberDTO, enMode mode = enMode.AddNew)
        {
            this.IsActive = memberDTO.IsActive;
            this.PersonInfo = new clsPerson(memberDTO.PersonInfoDTO);
            this.CreatedByUserID = memberDTO.CreatedByUserID;
            this.CreatedByUserInfo = null;
            this.CreatedDate = memberDTO.CreatedDate;
            this.MembershipInfo = new clsMembership(memberDTO.MembershipInofDTO);
            _Mode = enMode.Update;
        }
        public static async Task<Result<clsMember>> CreateAsync(ReceivedDataAddNewMemberDTO addNewMember)
        {
            Result<clsUser> result = await clsUser.FindAsync(addNewMember.CreatedByUserID);
            if (!result.Success)
            {
                return new Result<clsMember>(false, result.Message, null, result.ErrorCode);
            }
            clsMember member = new clsMember(addNewMember);
            member.CreatedByUserInfo = result.Data;
            return new Result<clsMember>(true, "Member created successfully.", member);
        }
        public FullMemberDTO MDTO
        {
            get
            {
                return new FullMemberDTO(this.MemberID, this.PersonID, this.CreatedDate, this.IsActive, this.CreatedByUserID, this.MembershipInfo.FMSDTO);
            }
        }
        public MemberResponseDataDTO RespnonsDataDTO
        {
            get
            {
                return new MemberResponseDataDTO(this.MemberID, this.IsActive, this.CreatedDate, this.CreatedByUserInfo.UserName,
                    this.MembershipInfo.MembershipClassInfo.Name, this.PersonInfo.PDTO);
            }
        }
        public static async Task<Result<clsMember>> FindAsync(int MemberID)
        {
            if (MemberID <= 0)
            {
                return new Result<clsMember>(false, "The request is invalid. Please check the input and try again.", null, 400);
            }
            Result<FullMemberDTO> result = await clsMemberData.GetMemberInfoByIDAsync(MemberID);
            if (result.Success)
            {
                return new Result<clsMember>(true, " Member found.", new clsMember(result.Data, enMode.Update));
            }
            return new Result<clsMember>(result.Success, result.Message, null, result.ErrorCode);
        }

        private async Task<Result<int>> _AddNewMemberAsync()
        {
            Result<int> result = await PersonInfo.SaveAsync();
            if (!result.Success)
            {
                return result;
            }
            this.PersonID = result.Data;
            result = await clsMemberData.AddNewMemberAsync(new clsMemberData.AddNewMemberDTO(this.PersonID, this.CreatedByUserID, this.IsActive));
            if (result.Success)
            {
                this.MemberID = result.Data;
                this.MembershipInfo.MemberID = result.Data;
                return await clsMembershipData.AddNewMembershipAsync(this.MembershipInfo.MSDTO);
            }
            return result;
        }

        private async Task<Result<int>> _UpdateMemberAsync()
        {
            Result<int> result = await PersonInfo.SaveAsync();
            if (!result.Success)
            {
                return result;
            }

            return await clsMemberData.UpdateMemberAsync(new clsMemberData.UpdateMemberDTO(this.MemberID, this.IsActive));
        }

        public static async Task<Result<bool>> DeleteMemberAsync(int MemberID)
        {
            if (MemberID <= 0)
            {
                return new Result<bool>(false, "The request is invalid. Please check the input and try again.", false, 400);
            }
            Result<FullMemberDTO> result = await clsMemberData.GetMemberInfoByIDAsync(MemberID);
            if (!result.Success)
            {
                return new Result<bool>(false, result.Message, false, result.ErrorCode);
            }
            return await clsMemberData.DeleteMemberAsync(MemberID);
        }

        public static async Task<Result<int>> GetTotalMembersAsync()
        {
            return await clsMemberData.GetTotalMembersAsync();
        }

        public async Task<Result<bool>> HasUnpaidFineAsync()
        {
            return await clsFineData.DoesMemberHaveUnpaidFinesAsync(this.MemberID);
        }

        public static async Task<Result<List<MemberViewDTO>>> GetAllMembersAsync()
        {
            return await clsMemberData.GetAllMembersAsync();
        }

        public Result<bool> HasActiveMembership()
        {
            return  this.MembershipInfo.IsActive();
        }

        public async Task<Result<int>> GetNumberOFBorrowedBooksAsync()
        {
            return await clsBorrowData.GetTotalBorrowedBookAsync(this.MemberID);
        }

        public async Task<Result<bool>> DoesHasReservationForBookIDAsync(int BookID)
        {
            return await clsReservationData.DoesHasActiveReservationForBookIDAsync(new HasActiveReservationDTO(this.MemberID, BookID));
        }

        public async Task<Result<bool>> IsReservationAvailableAsync(int bookID)
        {
            return await clsBookCopyData.ISReservationAvailableForBookIDAsync(new IsReservationAvailabeDTO(this.MemberID, bookID));
        }

        public async Task<Result<bool>> DoesHasActiveReservationForBookIDAsync(int bookID)
        {
            return await clsReservationData.DoesHasActiveReservationForBookIDAsync(new HasActiveReservationDTO(this.MemberID, bookID));
        }

        public async Task<Result<bool>> CheckMemberCriteriaAsync()
        {
            if (!this.IsActive)
            {
                return new Result<bool>(false, "This member's account is inactive. Please contact the administrator for assistance.", false, 422);
            }
            Result<bool> result = await this.HasUnpaidFineAsync();
            if (!result.Success)
            {
                return result;
            }
            if (result.Data)
            {
                return new Result<bool>(false, "Your membership has been deactivated due to outstanding fines. Please settle your balance to reactivate your account. Contact the library staff for further assistance.", false, 422);
            }
            result =  this.HasActiveMembership();
            if (!result.Success)
            {
                return result;
            }
            if (!result.Data)
            {
                return new Result<bool>(false, "This membership is currently inactive. Please renew the membership to regain access and try again.", false, 422);
            }
            Result<int> result1 = await GetNumberOFBorrowedBooksAsync();
            if (!result1.Success)
            {
                return new Result<bool>(false, result.Message, false, result.ErrorCode);
            }
            if (result1.Data >= this.MembershipInfo.MembershipClassInfo.MaxNumberOfBookCanBorrow)
            {
                return new Result<bool>(false, "You've reached your borrowing limit. Return items or upgrade your membership for more.", false, 422);
            }
            return new Result<bool>(true, "All criteria have been met. You are eligible to borrow books.", true);
        }

        bool _ValidateDataAsync(ref RenewMembershipDTO renewMembershipDTO)
        {
            return (renewMembershipDTO.MembershipClassID > 0 && renewMembershipDTO.MembershipStartDate > DateTime.Now &&
                renewMembershipDTO.MembershipExpirationDate > renewMembershipDTO.MembershipStartDate && renewMembershipDTO.PaidFees > 0 &&
                renewMembershipDTO.CreatedByUserID > 0);
        }

        public async Task<Result<FullMembershipDTO>> RenewMembershipAsync(RenewMembershipDTO renewMembershipDTO)
        {
            if (!_ValidateDataAsync(ref renewMembershipDTO))
            {
                return new Result<FullMembershipDTO>(false, "The request is invalid. Please check the input and try again.", null, 400);
            }
            if (this.MembershipInfo.MembershipExpirationDate >= DateTime.Now)
            {
                return new Result<FullMembershipDTO>(false, $"This member has an active membership with class {MembershipInfo.MembershipClassInfo.Name}", null, 422);
            }
            MembershipDTO addNewMembershipDTO = new MembershipDTO(this.MemberID, renewMembershipDTO.MembershipClassID, renewMembershipDTO.MembershipStartDate,
                 renewMembershipDTO.MembershipExpirationDate, renewMembershipDTO.PaidFees, renewMembershipDTO.CreatedByUserID);
            Result<clsMembership> createMembershipResult = await clsMembership.CreateAsync(addNewMembershipDTO, clsMembership.enMode.AddNew);
            if (!createMembershipResult.Success)
            {
                return new Result<FullMembershipDTO>(false, createMembershipResult.Message, null, createMembershipResult.ErrorCode);
            }
            Result<int> result = await createMembershipResult.Data.SaveAsync();
            if (!result.Success)
            {
                return new Result<FullMembershipDTO>(false, result.Message, null, result.ErrorCode);
            }
            this.MembershipInfo = createMembershipResult.Data;
            return new Result<FullMembershipDTO>(true, "Membership renewed successfully.", this.MembershipInfo.FMSDTO);
        }

        public static async Task<Result<bool>> ValidateDataAsync(ReceivedDataAddNewMemberDTO addNewMemberDTO)
        {
            if (
                addNewMemberDTO.CreatedByUserID <= 0 ||
                addNewMemberDTO.CreatedDate < DateTime.Now
                )
            {
                return new Result<bool>(false, "The request is invalid. Please check the input and try again.", false, 400);
            }
            return await clsPerson.ValidateDataAsync(addNewMemberDTO.PersonInfoDTO);
        }
        public static async Task<Result<bool>> ValidateDataAsync(RecivedDataUpdateMemberDTO updateMemberDTO, string currentNationalNumber)
        {
            return await clsPerson.ValidateDataAsync(updateMemberDTO.PersonInfoDTO, updateMemberDTO.PersonInfoDTO.NationalNo);
        }
        public async Task<Result<int>> SaveAsync()
        {
            switch (_Mode)
            {
                case enMode.AddNew:
                    Result<int> result = await _AddNewMemberAsync();
                    if (result.Success)
                    {
                        _Mode = enMode.Update;
                    }
                    return result;

                case enMode.Update:
                    return await _UpdateMemberAsync();
                default:
                    return new Result<int>(false, "An unexpected error occurred on the server.", -1, 500);
            }
        }

    }
}
