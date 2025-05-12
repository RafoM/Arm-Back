using ContentService.Models.RequestModels;
using ContentService.Models.ResponseModels;
using ContentService.Services.Interface;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Threading.Tasks;
using System;
using Microsoft.AspNetCore.Authorization;
using ContentService.Services.Implementation;

namespace ContentService.Controllers
{
   
    public class CasesController : BaseController
    {
        private readonly ICaseService _CaseService;

        /// <summary>
        /// Initializes a new instance of the <see cref="CasesController"/> class.
        /// </summary>
        /// <param name="CaseService">Service handling Case-related operations.</param>
        public CasesController(ICaseService CaseService)
        {
            _CaseService = CaseService;
        }

        /// <summary>
        /// Creates a new Case using the provided request model.
        /// </summary>
        /// <param name="request">The Case data to create.</param>
        /// <returns>
        /// A newly created <see cref="CaseResponseModel"/>, along with a 201 Created response.
        /// </returns>
        [HttpPost]
        public async Task<ActionResult<CaseResponseModel>> CreateCase([FromBody] CaseRequestModel request)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var result = await _CaseService.CreateAsync(request);
            return CreatedAtAction(nameof(GetCaseById), new { CaseId = result.CaseId }, result);
        }

        /// <summary>
        /// Retrieves a list of all existing Cases.
        /// </summary>
        /// <returns>
        /// A collection of <see cref="CaseResponseModel"/>.
        /// </returns>
        [HttpGet]
        public async Task<ActionResult<IEnumerable<CaseResponseModel>>> GetAllCases()
        {
            var Cases = await _CaseService.GetAllAsync();
            return Ok(Cases);
        }

        /// <summary>
        /// Retrieves a specific Case by its unique identifier.
        /// </summary>
        /// <param name="CaseId">The unique identifier for the Case.</param>
        /// <returns>
        /// A single <see cref="CaseResponseModel"/> if found; otherwise 404 Not Found.
        /// </returns>
        [Authorize(Roles = "Admin")]
        [HttpGet("{CaseId}")]
        public async Task<ActionResult<CaseResponseModel>> GetCaseById(int CaseId)
        {
            try
            {
                var Case = await _CaseService.GetByIdAsync(CaseId);
                return Ok(Case);
            }
            catch (KeyNotFoundException ex)
            {
                return NotFound(ex.Message);
            }
        }

        /// <summary>
        /// Updates an existing Case using the provided update model.
        /// </summary>
        /// <param name="request">The update model containing the new values.</param>
        /// <returns>
        /// The updated <see cref="CaseResponseModel"/> if found; otherwise 404 Not Found.
        /// </returns>
        [Authorize(Roles = "Admin")]
        [HttpPut]
        public async Task<ActionResult<CaseResponseModel>> UpdateCase([FromBody] CaseUpdateModel request)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            try
            {
                var updated = await _CaseService.UpdateAsync(request);
                return Ok(updated);
            }
            catch (KeyNotFoundException ex)
            {
                return NotFound(ex.Message);
            }
        }

        /// <summary>
        /// Deletes an existing Case by its unique identifier.
        /// </summary>
        /// <param name="CaseId">The ID of the Case to be deleted.</param>
        /// <returns>
        /// A 204 No Content response if the Case was deleted; 404 Not Found if it does not exist.
        /// </returns>
        [Authorize(Roles = "Admin")]
        [HttpDelete("{CaseId}")]
        public async Task<IActionResult> DeleteCase(int CaseId)
        {
            try
            {
                await _CaseService.DeleteAsync(CaseId);
                return NoContent();
            }
            catch (KeyNotFoundException ex)
            {
                return NotFound(ex.Message);
            }
        }


        /// <summary>
        /// Uploads a media file for case content.
        /// </summary>
        /// <param name="request">The media upload request.</param>
        /// <returns>The URL of the uploaded media.</returns>
        [HttpPost("upload-media")]
        [Authorize(Roles = "Admin")]
        [Consumes("multipart/form-data")]
        [ProducesResponseType(typeof(string), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<ActionResult<string>> UploadMedia([FromForm] UploadMediaRequest request)
        {
            if (request.MediaFile == null || request.MediaFile.Length == 0)
                return BadRequest("Invalid media file.");

            var mediaUrl = await _CaseService.UploadCaseMediaAsync(request.MediaFile);
            return Ok(mediaUrl);
        }
    }
}
