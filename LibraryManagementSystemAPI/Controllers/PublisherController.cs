using LibraryManagementSystemAPIBussinesLayer;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using SharedClasses;
using static SharedClasses.AuthorDTOs;
using static SharedClasses.BookDTOs;
using static SharedClasses.PublisherDTOs;

namespace LibraryManagementSystemAPI.Controllers
{
    [Route("api/publishers")]
    [ApiController]
    public class PublisherController : ControllerBase
    {
        [HttpGet("{id}", Name = "GetPublisherByID")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult<PublisherDTO>> GetPublisherByID(int id)
        {
            Result<clsPublisher> result = await clsPublisher.FindAsync(id);  // Assuming async version of Find

            if (result.Success)
            {
                return Ok(result.Data.PDTO);
            }
            return StatusCode(result.ErrorCode, result.Message);
        }

        [HttpPost(Name = "AddPublisher")]
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult<PublisherDTO>> AddPublisher(PublisherDTO publisherDTO)
        {
            clsPublisher newPublisher = new clsPublisher(publisherDTO, clsPublisher.enMode.AddNew);

            Result<int> result = await newPublisher.SaveAsync();  // Assuming async version of Save

            if (result.Success)
            {
                return CreatedAtRoute("GetPublisherByID", new { id = newPublisher.ID }, newPublisher.PDTO);
            }
            else
            {
                return StatusCode(result.ErrorCode, result.Message);
            }
        }

        [HttpPut("{id}", Name = "UpdatePublisher")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult<PublisherDTO>> UpdatePublisher(int id, [FromBody] PublisherDTO publisherDTO)
        {
            Result<clsPublisher> result = await clsPublisher.FindAsync(id);  

            if (!result.Success)
            {
                return StatusCode(result.ErrorCode, result.Message);
            }

            string originalNationalNumber = result.Data.PDTO.PersonInfoDTO.NationalNo;

            result.Data.PersonInfo.NationalNo = publisherDTO.PersonInfoDTO.NationalNo;
            result.Data.PersonInfo.FirstName = publisherDTO.PersonInfoDTO.FirstName;
            result.Data.PersonInfo.SecondName = publisherDTO.PersonInfoDTO.SecondName;
            result.Data.PersonInfo.ThirdName = publisherDTO.PersonInfoDTO.ThirdName;
            result.Data.PersonInfo.LastName = publisherDTO.PersonInfoDTO.LastName;
            result.Data.PersonInfo.Gender = (clsPerson.enGender)publisherDTO.PersonInfoDTO.Gender;
            result.Data.PersonInfo.Email = publisherDTO.PersonInfoDTO.Email;
            result.Data.PersonInfo.Phone = publisherDTO.PersonInfoDTO.Phone;

            Result<int> newResult = await result.Data.SaveAsync(originalNationalNumber); 

            if (newResult.Success)
            {
                return Ok(result.Data.PDTO);
            }
            return StatusCode(newResult.ErrorCode, newResult.Message);
        }

        [HttpDelete("{id}", Name = "DeletePublisher")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult> DeletePublisher(int id)
        {
            Result<bool> result = await clsPublisher.DeletePublisherAsync(id);  
            if (result.Success)
            {
                return Ok($"Publisher with ID {id} has been deleted.");
            }

            return StatusCode(result.ErrorCode, result.Message);
        }

        [HttpGet("TotalPublishers", Name = "GetTotalPublishers")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult<int>> GetTotalPublisher()
        {
            Result<int> result = await clsPublisher.GetTotalPublishersAsync();  
            if (result.Success)
            {
                return Ok(result.Data);
            }

            return StatusCode(result.ErrorCode, result.Message);
        }

        [HttpGet(Name = "GetPublishers")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult<IEnumerable<PublisherViewDTO>>> GetAllPublishers()
        {
            Result<List<PublisherViewDTO>> result = await clsPublisher.GetAllPublishersAsync();  

            if (result.Success)
            {
                return Ok(result.Data);
            }
            return StatusCode(result.ErrorCode, result.Message);
        }

        [HttpGet("AllBooks", Name = "GetPublisherBooks")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult<IEnumerable<BookViewDTO>>> GetAllBooks(int publisherID)
        {
            Result<List<BookViewDTO>> result = await clsPublisher.GetAllBooksAsync(publisherID);  

            if (result.Success)
            {
                return Ok(result.Data);
            }
            return StatusCode(result.ErrorCode, result.Message);
        }

        [HttpGet("publisher/name", Name = "GetPublisherName")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult<string>> GetPublisherName([FromQuery] int id)
        {
            Result<string> result = await clsPublisher.GetPublisherNameAsync(id);  
            if (result.Success)
            {
                return Ok(result.Data);
            }
            return StatusCode(result.ErrorCode, result.Message);
        }

        [HttpGet("publisher/id", Name = "GetpublisherID")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult<int>> GetpublisherID([FromQuery] string name)
        {
            Result<int> result = await clsPublisher.GetPublisherIDAsync(name);  
            if (result.Success)
            {
                return Ok(result.Data);
            }
            return StatusCode(result.ErrorCode, result.Message);
        }

    }
}
