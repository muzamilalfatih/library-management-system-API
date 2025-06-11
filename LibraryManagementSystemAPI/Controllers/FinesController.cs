using LibraryManagementSystemAPIBussinesLayer;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using SharedClasses;
using static SharedClasses.FineDTOs;
using static SharedClasses.FineSettingsDTOs;
using static SharedClasses.UserDTOS;

namespace LibraryManagementSystemAPI.Controllers
{
    [Route("api/fines")]
    [ApiController]
    public class FinesController : ControllerBase
    {
        [HttpGet("{id}", Name = "GetFineByID")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult<FullFineDTO>> GetFineByID(int id)
        {
            Result<FullFineDTO> result = await clsFine.FindAsync(id); 

            if (result.Success)
            {
                return Ok(result.Data);
            }
            return StatusCode(result.ErrorCode, result.Message);
        }

        [HttpGet("All", Name = "GetFines")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult<IEnumerable<FineViewDTO>>> GetAllFines()
        {
            Result<List<FineViewDTO>> result = await clsFine.GetAllFinesAsync(); 

            if (result.Success)
            {
                return Ok(result.Data);
            }
            return StatusCode(result.ErrorCode, result.Message);
        }

        [HttpPost("pay", Name = "PayFine")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult<string>> PayFine(PayFineDTO payfineDTO)
        {
            Result<bool> result = await clsFine.PayFineAsync(payfineDTO); 

            if (result.Success)
            {
                return Ok(result.Message);
            }
            return StatusCode(result.ErrorCode, result.Message);
        }

        [HttpGet("settings", Name = "GetFineSetting")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult<IEnumerable<FineSettingsDTO>>> GetFineSettings()
        {
            Result<List<FineSettingsDTO>> result = await clsFineSettings.GetFineSettingInfoAsync(); 

            if (result.Success)
            {
                return Ok(result.Data);
            }
            return StatusCode(result.ErrorCode, result.Message);
        }

        [HttpPut("settings", Name = "UpdateFineSettings")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult<bool>> UpdateFineSettings(FineSettingsDTO fineSettingsDTO)
        {
            Result<bool> result = await clsFineSettings.UpdateFineSettingsAsync(fineSettingsDTO); 

            if (result.Success)
            {
                return Ok(result.Data);
            }
            return StatusCode(result.ErrorCode, result.Message);
        }


    }
}
