using LibraryManagementSystemAPIBussinesLayer;
using LibraryManagementSystemAPIDataAccessLayer;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using SharedClasses;
using static SharedClasses.BookCategoryDTOs;
using static SharedClasses.BookCopyDTOs;
using static SharedClasses.BookDTOs;
using static SharedClasses.UserDTOS;

namespace LibraryManagementSystemAPI.Controllers
{
    [Route("api/Books")]
    [ApiController]
    public class BooksController : ControllerBase
    {
        [HttpGet(Name = "GetBook")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult<FullBookDTO>> FindBook([FromQuery] int? id, [FromQuery] string? query)
        {
            if (id.HasValue)
            {
                Result<clsBook> result = await clsBook.FindAsync(id.Value); 
                if (result.Success)
                {
                    return Ok(result.Data.FBDTO);
                }
                return StatusCode(result.ErrorCode, result.Message);
            }
            else if (!string.IsNullOrEmpty(query))
            {
                Result<clsBook> result1 = await clsBook.FindAsync(query);
                if (result1.Success)
                {
                    return Ok(result1.Data.FBDTO);
                }
                return StatusCode(result1.ErrorCode, result1.Message);
            }
            else
                return BadRequest("Provide either 'id' or 'query' to search for a book.");
        }

        [HttpPost("creat", Name = "AddNewBook")]
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult<FullBookDTO>> AddBook(AddNewBookDTO addNewBookDTO)
        {
            Result<bool> validationResult = await clsBook.ValidateDataAsync(addNewBookDTO);

            if (!validationResult.Success)
            {
                return StatusCode(validationResult.ErrorCode, validationResult.Message);
            }
            clsBook book = new clsBook(addNewBookDTO.bookDTO, clsBook.enMode.AddNew);
            Result<int> result = await book.SaveAsync(addNewBookDTO.numberOfCopies); 
            if (result.Success)
            {
                return CreatedAtRoute("GetBook", new { id = book.BookID }, book.FBDTO);
            }
            else
            {
                return StatusCode(result.ErrorCode, result.Message);
            }
        }

        [HttpPut("{id}", Name = "UpdateBook")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult<FullBookDTO>> UpdateBook(int id, [FromBody] BookDTO bookDTO)
        {
            Result<clsBook> FindBookResult = await clsBook.FindAsync(id); 

            if (!FindBookResult.Success)
            {
                return NotFound(FindBookResult.Message);
            }
            Result<bool> validationResult = await clsBook.ValidateDataAsync(bookDTO, FindBookResult.Data.ISBN);
            if (!validationResult.Success)
            {
                return StatusCode(validationResult.ErrorCode, validationResult.Message);
            }

            FindBookResult.Data.Title = bookDTO.Title;
            FindBookResult.Data.AuthorID = bookDTO.AuthorID;
            FindBookResult.Data.ISBN = bookDTO.ISBN;
            FindBookResult.Data.PublisherID = bookDTO.PublisherID;
            FindBookResult.Data.CategoryID = bookDTO.CategoryID;
            FindBookResult.Data.Year = bookDTO.Year;
            FindBookResult.Data.Location = bookDTO.Location;
            FindBookResult.Data.BorrowFees = bookDTO.BorrowFees;

            Result<int> savingResult = await FindBookResult.Data.SaveAsync(); 

            if (savingResult.Success)
            {
                return Ok(FindBookResult.Data.FBDTO);
            }
            return StatusCode(savingResult.ErrorCode, savingResult.Message);
        }

        [HttpDelete("{id}", Name = "DeleteBook")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult> DeleteBook(int id)
        {
            Result<bool> result = await clsBook.DeleteAsync(id); 
            if (result.Success)
            {
                return Ok($"Book with ID {id} has been deleted.");
            }

            return StatusCode(result.ErrorCode, result.Message);
        }

        [HttpGet("TotalBooks", Name = "GetTotalBooks")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult<int>> GetTotalUsers()
        {
            Result<int> result = await clsBook.GetTotalBooksAsync();
            if (result.Success)
            {
                return Ok(result.Data);
            }

            return StatusCode(result.ErrorCode, result.Message);
        }

        [HttpPost("add-copies", Name = "InsertNumberOfcopies")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult<List<int>>> InsertCopies(InsertCopiesDTO insertCopiesDTO)
        {
            Result<List<int>> result = await clsBook.InserNumberOfCopiesAsync(insertCopiesDTO); 

            if (result.Success)
            {
                return Ok(result.Data);
            }
            return StatusCode(result.ErrorCode, result.Message);
        }

        [HttpGet("all", Name = "GetAllBooks")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult<IEnumerable<BookViewDTO>>> GetAllBooks()
        {
            Result<List<BookViewDTO>> result = await clsBook.GetAllBooksAsync();
            if (result.Success)
            {
                return Ok(result.Data);
            }
            return StatusCode(result.ErrorCode, result.Message);
        }

        [HttpGet("categories/all", Name = "GetAllBookCategory")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult<IEnumerable<BookViewDTO>>> GetAllBookCategory()
        {
            Result<List<BookCategoryDTO>> result = await clsBookCategory.GetAllCategoriesAsync();  
            if (result.Success)
            {
                return Ok(result.Data);
            }
            return StatusCode(result.ErrorCode, result.Message);
        }

        [HttpGet("category/name", Name = "GetCategoryName")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult<string>> GetCategoryName([FromQuery] int id)
        {
            Result<string> result = await clsBookCategory.GetCategoryNameAsync(id);  
            if (result.Success)
            {
                return Ok(result.Data);
            }
            return StatusCode(result.ErrorCode, result.Message);
        }

        [HttpGet("category/id", Name = "GetCategoryID")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult<int>> GetCategoryID([FromQuery] string name)
        {
            Result<int> result = await clsBookCategory.GetCategoryIDAsync(name);  
            if (result.Success)
            {
                return Ok(result.Data);
            }
            return StatusCode(result.ErrorCode, result.Message);
        }

        [HttpGet("{id}/copies/all", Name = "GetAllCopies")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult<int>> GetAllcopies(int id)
        {
            Result<List<BookCopyDTO>> result = await clsBookCopy.GetAllCopiesAsync(id);  
            if (result.Success)
            {
                return Ok(result.Data);
            }
            return StatusCode(result.ErrorCode, result.Message);
        }

        [HttpGet("copies/{CopyID}", Name = "GetBookCopy")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult<int>> GetCopy(int CopyID)
        {
            Result<clsBookCopy> result = await clsBookCopy.FindAsync(CopyID);  
            if (result.Success)
            {
                return Ok(result.Data);
            }
            return StatusCode(result.ErrorCode, result.Message);
        }

        [HttpPost("copies/repair", Name = "RepairBookCopy")]
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult<int>> RepairBookCopy(RepairCopyDTO repairCopyDTO)
        {
            Result<int> result = await clsBookCopy.RepairAsync(repairCopyDTO);  

            if (result.Success)
            {
                return Ok(result.Data);
            }
            else
            {
                return StatusCode(result.ErrorCode, result.Message);
            }
        }

    }
}
