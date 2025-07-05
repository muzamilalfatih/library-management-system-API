using LibraryManagementSystemAPIBussinesLayer;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using SharedClasses;
using static SharedClasses.AuthorDTOs;
using static SharedClasses.BookDTOs;
using static SharedClasses.PersonDTOs;

namespace LibraryManagementSystemAPI.Controllers
{
    [Route("api/people")]
    [ApiController]
    public class PeopleController : ControllerBase
    {
        [HttpGet("{id}", Name = "GetPersonByID")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult<PersonDTO>> GetPersonByID(int id)
        {
            Result<clsPerson> result = await clsPerson.FindAsync(id);

            if (result.Success)
            {
                return Ok(result.Data.PDTO);
            }
            return result.ErrorCode == 400 ? BadRequest(result.Message) : NotFound(result.Message);
        }

        [HttpPost(Name = "AddPerson")]
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult<PersonDTO>> AddPerson(PersonDTO personDTO)
        {

            Result<bool> result = await clsPerson.ValidateDataAsync(personDTO);
            if (!result.Success)
            {
                return StatusCode(result.ErrorCode, result.Message);
            }
            clsPerson newPerson = new clsPerson(personDTO, clsPerson.enMode.AddNew);
            Result<int> savingResult = await newPerson.SaveAsync();

            if (savingResult.Success)
            {
                return CreatedAtRoute("GetAuthorByID", new { id = personDTO.PersonID }, newPerson.PDTO);
            }
            else
            {
                return StatusCode(savingResult.ErrorCode, savingResult.Message);
            }
        }

        [HttpPut("{id}", Name = "UpdatePerson")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult<PersonDTO>> UpdatePerson(int id, [FromBody] PersonDTO personDTO)
        {

            Result<clsPerson> findResult = await clsPerson.FindAsync(id);   
            if (!findResult.Success)
            {
                return StatusCode(findResult.ErrorCode, findResult.Message);
            }

            Result<int> savingResult = await findResult.Data.SaveAsync();

            if (savingResult.Success)
            {
                return Ok(findResult.Data.PDTO);
            }
            return StatusCode(savingResult.ErrorCode, savingResult.Message);
        }

        [HttpDelete("{id}", Name = "DeletePerson")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult<bool>> DeletePerson(int id)
        {
            Result<bool> result = await clsPerson.DeleteAsync(id);
            if (result.Success)
            {
                return Ok(result.Message);
            }

            return StatusCode(result.ErrorCode, result.Message);
        }
    }
}
