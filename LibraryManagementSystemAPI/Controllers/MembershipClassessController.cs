using LibraryManagementSystemAPIBussinesLayer;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using SharedClasses;
using static SharedClasses.MembershipClassDTOs;
using static SharedClasses.UserDTOS;

namespace LibraryManagementSystemAPI.Controllers
{
    [Route("api/membership-Classes")]
    [ApiController]
    public class MembershipClassesController : ControllerBase
    {
        [HttpGet("{id}", Name = "GetMembershipClassByID")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult<MembershipClassDTO>> GetMembershipClass(int id)
        {
            Result<clsMembershipClass> result = await clsMembershipClass.FindAsync(id);  

            if (result.Success)
            {
                return Ok(result.Data.MSCDTO);
            }
            return StatusCode(result.ErrorCode, result.Message);
        }

        [HttpGet("name", Name = "GetMembershipClassName")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult<string>> GetMembershipClassName([FromQuery] int id)
        {
            Result<string> result = await clsMembershipClass.GetMembershipClassNameAsync(id);  

            if (result.Success)
            {
                return Ok(result.Data);
            }
            return StatusCode(result.ErrorCode, result.Message);
        }

        [HttpGet("id", Name = "GetMembershipClassID")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult<int>> GetMembershipClassID([FromQuery] string name)
        {
            Result<int> result = await clsMembershipClass.GetMembershipClassIDAsync(name);  

            if (result.Success)
            {
                return Ok(result.Data);
            }
            return StatusCode(result.ErrorCode, result.Message);
        }

        [HttpPost(Name = "MembershipClass")]
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult<MembershipClassDTO>> AddMembershipClass(MembershipClassDTO membershipClassDTO)
        {
            clsMembershipClass newMembershipClass = new clsMembershipClass(membershipClassDTO, clsMembershipClass.enMode.AddNew);

            Result<int> result = await newMembershipClass.SaveAsync();  

            if (result.Success)
            {
                return CreatedAtRoute("GetMembershipClassByID", new { id = newMembershipClass.ID }, newMembershipClass.MSCDTO);
            }
            else
            {
                return StatusCode(result.ErrorCode, result.Message);
            }
        }

        [HttpPut("{id}", Name = "MembershipClassUser")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult<MembershipClassDTO>> UpdateMembershipClass(int id, [FromBody] MembershipClassDTO membershipClassDTO)
        {
            Result<clsMembershipClass> result = await clsMembershipClass.FindAsync(id);  

            if (!result.Success)
            {
                return StatusCode(result.ErrorCode, result.Message);
            }

            result.Data.Name = membershipClassDTO.MembershipClassName;
            result.Data.MaxNumberOfBookCanBorrow = membershipClassDTO.MaxNumberOfBooksCanBorrow;
            result.Data.FeesPerDay = membershipClassDTO.FeesPerDay;

            Result<int> newResult = await result.Data.SaveAsync();  

            if (newResult.Success)
            {
                return Ok(result.Data.MSCDTO);
            }
            return StatusCode(newResult.ErrorCode, newResult.Message);
        }

        [HttpDelete("{id}", Name = "DeleteMembershipClass")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult> DeleteMembershipClass(int id)
        {
            Result<bool> result = await clsMembershipClass.DeleteAsync(id);  
            if (result.Success)
            {
                return Ok($"Membership Class with ID {id} has been deleted.");
            }
            return StatusCode(result.ErrorCode, result.Message);
        }

        [HttpGet("All", Name = "GetMembershipClasses")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult<IEnumerable<MembershipClassDTO>>> GetAllMembershipClasses()
        {
            Result<List<MembershipClassDTO>> result = await clsMembershipClass.GetAllMembershipClassesAsync();  

            if (result.Success)
            {
                return Ok(result.Data);
            }
            return StatusCode(result.ErrorCode, result.Message);
        }

    }
}
