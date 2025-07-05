using LibraryManagementSystemAPIBussinesLayer;
using LibraryManagementSystemAPIDataAccessLayer;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using SharedClasses;
using static SharedClasses.AuthorDTOs;
using static SharedClasses.BookDTOs;
using static SharedClasses.UserDTOS;

namespace LibraryManagementSystemAPI.Controllers
{
    [Route("api/authors")]
    [ApiController]
    public class AuthorsController : ControllerBase
    {
        [HttpGet("{id}", Name = "GetAuthorByID")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult<AuthorDTO>> GetAuthorByID(int id)
        {
            Result<clsAuthor> result = await clsAuthor.FindAsync(id);  

            if (result.Success)
            {
                return Ok(result.Data.ADTO);
            }
            return result.ErrorCode == 400 ? BadRequest(result.Message) : NotFound(result.Message);
        }

        [HttpPost(Name = "AddAuthor")]
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult<AuthorDTO>> AddAuthor(AuthorDTO authorDTO)
        {
            
            clsAuthor newAuthor = new clsAuthor(authorDTO, clsAuthor.enMode.AddNew);
            Result<int> savingResult = await newAuthor.SaveAsync();  

            if (savingResult.Success)
            {
                return CreatedAtRoute("GetAuthorByID", new { id = newAuthor.ID }, newAuthor.ADTO);
            }
            else
            {
                return StatusCode(savingResult.ErrorCode, savingResult.Message);
            }
        }

        [HttpPut("{id}", Name = "UpdateAuthor")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult<AuthorDTO>> UpdateAuthor(int id, [FromBody] AuthorDTO authorDTO)
        {
            
            Result<clsAuthor> findAuthorResult = await clsAuthor.FindAsync(id);  

            if (!findAuthorResult.Success)
            {
                return StatusCode(findAuthorResult.ErrorCode, findAuthorResult.Message);
            }
            

            

            Result<int> savingResult = await findAuthorResult.Data.SaveAsync();  

            if (savingResult.Success)
            {
                return Ok(findAuthorResult.Data.ADTO);
            }
            return StatusCode(savingResult.ErrorCode, savingResult.Message);
        }

        [HttpDelete("{id}", Name = "DeleteAuthor")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult<bool>> DeleteAuthor(int id)
        {
            Result<bool> result = await clsAuthor.DeleteAuthorAsync(id);  
            if (result.Success)
            {
                return Ok($"Author with ID {id} has been deleted.");
            }

            return StatusCode(result.ErrorCode, result.Message);
        }

        [HttpGet("TotalAuthors", Name = "GetTotalAuthors")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult<int>> GetTotalAuthors()
        {
            Result<int> result = await clsAuthor.GetTotalAuthorsAsync();  
            if (result.Success)
            {
                return Ok(result.Data);
            }

            return StatusCode(result.ErrorCode, result.Message);
        }

        [HttpGet("All", Name = "GetAuthors")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult<IEnumerable<AuthorViewDTO>>> GetAllAuthors()
        {
            Result<List<AuthorViewDTO>> result = await clsAuthor.GetAllAuthorsAsync();  

            if (result.Success)
            {
                return Ok(result.Data);
            }
            return StatusCode(result.ErrorCode, result.Message);
        }

        [HttpGet("AllBooks", Name = "GetAuthorBooks")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult<IEnumerable<BookViewDTO>>> GetAllBooks(int authorID)
        {
            Result<List<BookViewDTO>> result = await clsAuthor.GetAllBooksAsync(authorID);  

            if (result.Success)
            {
                return Ok(result.Data);
            }
            return StatusCode(result.ErrorCode, result.Message);
        }

        [HttpGet("author/name", Name = "GetAuthorName")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult<string>> GetAuthorName([FromQuery] int id)
        {
            Result<string> result = await clsAuthor.GetAuthorNameAsync(id);  
            if (result.Success)
            {
                return Ok(result.Data);
            }
            return StatusCode(result.ErrorCode, result.Message);
        }

        [HttpGet("author/id", Name = "GetAuthorID")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult<int>> GetAuthorID([FromQuery] string name)
        {
            Result<int> result = await clsAuthor.GetAuthorIDAsync(name);  
            if (result.Success)
            {
                return Ok(result.Data);
            }
            return StatusCode(result.ErrorCode, result.Message);
        }

    }
}
