using LibraryManagementSystemAPIBussinesLayer;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using SharedClasses;
using static SharedClasses.BorrowDTOs;
using static SharedClasses.UserDTOS;

namespace LibraryManagementSystemAPI.Controllers
{
    [Route("api/borrows")]
    [ApiController]
    public class BorrowsController : ControllerBase
    {
        [HttpGet("{id}", Name = "GetBorrowByID")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult<FullBorrowDTO>> GetBorrowByID(int id)
        {
            Result<FullBorrowDTO> result = await clsBorrow.FindAsync(id); 
            if (result.Success)
            {
                return Ok(result.Data);
            }
            return StatusCode(result.ErrorCode, result.Message);
        }

        [HttpGet("by-copy/{id}", Name = "GetBorrowByCopyID")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult<FullBorrowDTO>> GetBorrowByCopyID(int id)
        {
            Result<FullBorrowDTO> result = await clsBorrow.FindByCopyIDAsync(id); 

            if (result.Success)
            {
                return Ok(result.Data);
            }
            return StatusCode(result.ErrorCode, result.Message);
        }

        [HttpPost("borrow-book", Name = "BorrowBook")]
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        [ProducesResponseType(StatusCodes.Status422UnprocessableEntity)]
        public async Task<ActionResult<BorrowedBookDTO>> BorrowBook(BorrowDTO borrowDTO)
        {
            Result<BorrowedBookDTO> result = await clsBorrow.BorrowBookAsync(borrowDTO); 

            if (result.Success)
            {
                return CreatedAtRoute("GetBorrowByID", new { id = result.Data.BorrowID }, result.Data);
            }
            else
            {
                return StatusCode(result.ErrorCode, result.Message);
            }
        }

        [HttpPost("return-book", Name = "ReturnBook")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        [ProducesResponseType(StatusCodes.Status422UnprocessableEntity)]
        public async Task<ActionResult<ReturnedBookDTO>> ReturnBook(ReturnBookDTO returnBookDTO)
        {
            Result<ReturnedBookDTO> result = await clsBorrow.ReturnBookAsync(returnBookDTO); 

            if (result.Success)
            {
                return Ok(result.Data);
            }
            return StatusCode(result.ErrorCode, result.Message);
        }

        [HttpGet("total-issued-books", Name = "GetTotalIssuedBooks")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult<int>> GetTotalIssuedBooks()
        {
            Result<int> result = await clsBorrow.GetTotalIssuedBooksAsync();

            if (result.Success)
            {
                return Ok(result.Data);
            }

            return StatusCode(result.ErrorCode, result.Message);
        }

        [HttpGet("all", Name = "GetBorrows")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult<IEnumerable<BorrowViewDTO>>> GetAllBorrows()
        {
            Result<List<BorrowViewDTO>> result = await clsBorrow.GetAllBorrowsAsync(); 

            if (result.Success)
            {
                return Ok(result.Data);
            }
            return StatusCode(result.ErrorCode, result.Message);
        }



    }
}
