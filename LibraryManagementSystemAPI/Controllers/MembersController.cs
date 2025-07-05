using LibraryManagementSystemAPIBussinesLayer;
using LibraryManagementSystemAPIDataAccessLayer;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using static SharedClasses.MemberDTOs;
using SharedClasses;
using static SharedClasses.UserDTOS;
using static SharedClasses.MembershipDTOs;
using Microsoft.IdentityModel.Tokens;

namespace LibraryManagementSystemAPI.Controllers
{
    [Route("api/members")]
    [ApiController]
    public class MembersController : ControllerBase
    {
        [HttpGet("{id}", Name = "GetMemberByID")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult<MemberResponseDataDTO>> GetMemberByID(int id)
        {
            Result<clsMember> result = await clsMember.FindAsync(id);  

            if (result.Success)
            {
                return Ok(result.Data.RespnonsDataDTO);
            }
            return StatusCode(result.ErrorCode, result.Message);
        }

        [HttpGet("{id}/membership", Name = "GetMembershipByMemberID")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult<FullMembershipDTO>> GetMembershipByMemberID(int id)
        {
            Result<clsMembership> result = await clsMembership.FindByMemberIDAsync(id);  
            if (result.Success)
            {
                return Ok(result.Data.FMSDTO);
            }
            return StatusCode(result.ErrorCode, result.Message);
        }

        [HttpPost("{memberID}/renew-membership", Name = "RenewMembershipDTO")]
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult<FullMembershipDTO>> RenewMembership(int memberID, RenewMembershipDTO renewMembershipDTO)
        {
            Result<clsMember> findMemberResult = await clsMember.FindAsync(memberID);  

            if (!findMemberResult.Success)
            {
                return StatusCode(findMemberResult.ErrorCode, findMemberResult.Message);
            }

            Result<FullMembershipDTO> renewMembershipResult = await findMemberResult.Data.RenewMembershipAsync(renewMembershipDTO);  
            if (renewMembershipResult.Success)
            {
                return CreatedAtRoute("GetMembershipByMemberID", new { id = renewMembershipResult.Data.MemberID }, renewMembershipResult.Data);
            }
            return StatusCode(renewMembershipResult.ErrorCode, renewMembershipResult.Message);
        }

        [HttpPost(Name = "AddMember")]
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult<FullMemberDTO>> AddMember(ReceivedDataAddNewMemberDTO AddNewMemberDTO)
        {
            Result<bool> validationResult = await clsMember.ValidateDataAsync(AddNewMemberDTO);
            if (!validationResult.Success)
            {
                return StatusCode(validationResult.ErrorCode, validationResult.Message);
            }
            clsMember newMember = new clsMember(AddNewMemberDTO, clsMember.enMode.AddNew);

            Result<int> result = await newMember.SaveAsync(); 

            if (result.Success)
            {
                return CreatedAtRoute("GetMemberByID", new { id = newMember.MemberID }, newMember.MDTO);
            }
            else
            {
                return StatusCode(result.ErrorCode, result.Message);
            }
        }

        [HttpPut("{id}", Name = "UpdateMember")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult<FullMemberDTO>> UpdateMember(int id, [FromBody] RecivedDataUpdateMemberDTO updateMemberDTO)
        {
            Result<clsMember> findMemberResult = await clsMember.FindAsync(id);  

            if (!findMemberResult.Success)
            {
                return StatusCode(findMemberResult.ErrorCode, findMemberResult.Message);
            }
            findMemberResult.Data.IsActive = updateMemberDTO.IsActive;

            Result<int> newResult = await findMemberResult.Data.SaveAsync();  

            if (newResult.Success)
            {
                return Ok(findMemberResult.Data.MDTO);
            }
            return StatusCode(newResult.ErrorCode, newResult.Message);
        }

        [HttpDelete("{id}", Name = "DeleteMember")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult> DeleteMember(int id)
        {
            Result<bool> result = await clsMember.DeleteMemberAsync(id);  
            if (result.Success)
            {
                return Ok($"Member with ID {id} has been deleted.");
            }
            return StatusCode(result.ErrorCode, result.Message);
        }

        [HttpGet("TotalMembers", Name = "GetTotalMembers")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult<int>> GetTotalMembers()
        {
            Result<int> result = await clsMember.GetTotalMembersAsync(); 
            if (result.Success)
            {
                return Ok(result.Data);
            }
            return StatusCode(result.ErrorCode, result.Message);
        }

        [HttpGet("All", Name = "GetMembers")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult<IEnumerable<MemberViewDTO>>> GetAllMembers()
        {
            Result<List<MemberViewDTO>> result = await clsMember.GetAllMembersAsync();  
            if (result.Success)
            {
                return result.Data.Count > 0 ? Ok(result.Data) : NotFound("No Member found.");
            }
            return StatusCode(result.ErrorCode, result.Message);
        }

    }
}
