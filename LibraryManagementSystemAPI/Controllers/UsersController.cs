using Azure.Core;
using LibraryManagementSystemAPIBussinesLayer;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using LibraryManagementSystemAPIDataAccessLayer;
using static SharedClasses.UserDTOS;
using SharedClasses;
namespace LibraryManagementSystemAPI.Controllers
{
    [Route("api/users")]
    [ApiController]
    public class UsersController : ControllerBase
    {
        [HttpGet("{id}", Name = "GetUserByID")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult<UserDTO>> GetUserByIDAsync(int id)
        {
            var result = await clsUser.FindAsync(id);

            if (result.Success)
            {
                return Ok(result.Data.ReponseDataDTO);
            }
            return StatusCode(result.ErrorCode, result.Message);
        }

        [HttpGet(Name = "GetUserByUserName")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult<UserDTO>> GetUserByUserNameAsync([FromQuery] string username)
        {
            var result = await clsUser.FindAsync(username);

            if (result.Success)
            {
                return Ok(result.Data.ReponseDataDTO);
            }
            return StatusCode(result.ErrorCode,result.Message);
        }

        [HttpPost(Name = "AddUser")]
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult<UserDTO>> AddUserAsync(UserDTO userDTO)
        {
            Result<bool> validationResult = await clsUser.ValidateDataAsync(userDTO);
            if (!validationResult.Success)
            {
                return StatusCode(validationResult.ErrorCode, validationResult.Message);
            }
            clsUser newUser = new clsUser(userDTO, clsUser.enMode.AddNew);
            Result<int> savingResult = await newUser.SaveAsync();           
            if (savingResult.Success)
            {
                return CreatedAtRoute("GetUserByID", new { id = newUser.UserID }, newUser.UDTO);
            }
            else
            {
                return StatusCode(savingResult.ErrorCode, savingResult.Message);
            }
        }

        [HttpPut("{id}", Name = "UpdateUser")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult<UserDTO>> UpdateUserAsync(int id, [FromBody] UserDTO userDTO)
        {
            Result<UserDTO> result = await clsUser.UpdateUserAsync(id, userDTO);   
            if (result.Success)
            {
                return Ok(result.Data);
            }
            return StatusCode(result.ErrorCode, result.Message);
        }

        [HttpDelete("{id}", Name = "DeleteUser")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult> DeleteUserAsync(int id)
        {
            var result = await clsUser.DeleteUserAsync(id);

            if (result.Success)
            {
                return Ok($"User with ID {id} has been deleted.");
            }

            return StatusCode(result.ErrorCode, result.Message);
        }

        [HttpGet("TotalUsers", Name = "GetTotalUsers")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult<int>> GetTotalUsersAsync()
        {
            var result = await clsUser.GetTotalUsersAsync();

            if (result.Success)
            {
                return Ok(result.Data);
            }

            return StatusCode(result.ErrorCode, result.Message);
        }

        [HttpGet("CheckForOtherAdmins", Name = "CheckForOtherAdmins")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult<bool>> IsThereOtherAdminsAsync(int currentUserID)
        {
            var result = await clsUser.IsThereOtherAdminsAsync(currentUserID);

            if (result.Success)
            {
                return Ok(result.Data);
            }

            return StatusCode(result.ErrorCode, result.Message);
        }

        [HttpPost("login", Name = "Login")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult<UserDTO>> LoginAsync(LoginDTO loginDTO)
        {
            if (loginDTO == null || string.IsNullOrWhiteSpace(loginDTO.UserName) || string.IsNullOrWhiteSpace(loginDTO.Password))
            {
                return BadRequest("Invalid data.");
            }

            var result = await clsUser.LoginAsync(loginDTO);

            if (result.Success)
            {
                string userToken = TokenService.GenerateJwtToken(result.Data);
                return Ok(new {loggeduser = result.Data, token = userToken });
            }
            else
            {
                return StatusCode(result.ErrorCode, result.Message);
            }
        }

        [HttpGet("All", Name = "GetUsers")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult<IEnumerable<UserViewDTO>>> GetAllUsersAsync()
        {
            var result = await clsUser.GetAllUsersAsync();

            if (result.Success)
            {
                return Ok(result.Data);
            }
            return StatusCode(result.ErrorCode, result.Message);
        }

    }
}
