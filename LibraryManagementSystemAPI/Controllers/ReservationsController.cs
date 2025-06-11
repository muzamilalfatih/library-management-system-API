using LibraryManagementSystemAPIBussinesLayer;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using SharedClasses;
using static SharedClasses.BorrowDTOs;
using static SharedClasses.FineDTOs;
using static SharedClasses.ReservationDTOs;

namespace LibraryManagementSystemAPI.Controllers
{
    [Route("api/reservations")]
    [ApiController]
    public class ReservationsController : ControllerBase
    {
        [HttpGet("{id}", Name = "GetReservationByID")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult<ReservationDTO>> GetReservationByID(int id)
        {
            Result<ReservationDTO> result = await clsReservation.FindAsync(id);  

            if (result.Success)
            {
                return Ok(result.Data);
            }
            return StatusCode(result.ErrorCode, result.Message);
        }

        [HttpPost("reserve-book", Name = "ReserveBook")]
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        [ProducesResponseType(StatusCodes.Status422UnprocessableEntity)]
        public async Task<ActionResult<int>> ReserveBook([FromBody] AddNewReservationDTO addNewReservationDTO)
        {
            Result<int> result = await clsReservation.ReserveBookAsync(addNewReservationDTO);  

            if (result.Success)
            {
                return CreatedAtRoute("GetReservationByID", new { id = result.Data }, result.Data);
            }
            else
            {
                return StatusCode(result.ErrorCode, result.Message);
            }
        }

        [HttpGet("all", Name = "GetReservations")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult<IEnumerable<ReservationViewDTO>>> GetAllReservations()
        {
            Result<List<ReservationViewDTO>> result = await clsReservation.GetAllReservationsAsync();  

            if (result.Success)
            {
                return Ok(result.Data);
            }
            return StatusCode(result.ErrorCode, result.Message);
        }

        [HttpPost("{reservationID}/cancel", Name = "CancelReservation")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult<int>> CancelReservation(int reservationID)
        {
            Result<int> result = await clsReservation.CancelReservationAsync(reservationID); 

            if (result.Success)
            {
                return Ok(result.Message);
            }
            return StatusCode(result.ErrorCode, result.Message);
        }

    }
}
