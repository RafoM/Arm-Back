using ContentService.Models.RequestModels;
using ContentService.Models.ResponseModels;
using ContentService.Services.Interface;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Threading.Tasks;
using System;
using Microsoft.AspNetCore.Authorization;

namespace ContentService.Controllers
{
    /// <summary>
    /// Controller for managing Case content.
    /// </summary>
    public class CasesController : BaseController
    {
        private readonly ICaseService _CaseService;

        /// <summary>
        /// Initializes a new instance of the <see cref="CasesController"/> class.
        /// </summary>
        /// <param name="CaseService">Service for Case operations.</param>
        public CasesController(ICaseService CaseService)
        {
            _CaseService = CaseService;
        }

        /// <summary>
        /// Gets the language ID from middleware-inserted HTTP context.
        /// </summary>
        /// <returns>The language ID.</returns>
        /// <exception cref="Exception">Thrown if header is missing.</exception>
        private int GetLanguageId()
        {
            return HttpContext.Items.TryGetValue("LanguageId", out var value) && value is int languageId
                ? languageId
                : throw new Exception("LanguageId header is required.");
        }

        /// <summary>
        /// Creates a new Case.
        /// </summary>
        /// <param name="request">The Case creation request model.</param>
        /// <returns>The created Case response model.</returns>
        [Authorize(Roles = "Admin")]
        [HttpPost]
        public async Task<ActionResult<CaseResponseModel>> CreateCase([FromBody] CaseRequestModel request)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var result = await _CaseService.CreateAsync(request);
            return CreatedAtAction(nameof(GetCaseById), new { CaseId = result.CaseId }, result);
        }

        /// <summary>
        /// Retrieves all Cases for a specific language.
        /// </summary>
        /// <returns>List of Case response models.</returns>
        [HttpGet]
        public async Task<ActionResult<IEnumerable<CaseResponseModel>>> GetAllCases()
        {
            var languageId = GetLanguageId();
            var Cases = await _CaseService.GetAllAsync(languageId);
            return Ok(Cases);
        }

        /// <summary>
        /// Retrieves a Case by its unique ID and language.
        /// </summary>
        /// <param name="CaseId">The Case ID.</param>
        /// <returns>The Case response model.</returns>
        [HttpGet("{CaseId}")]
        public async Task<ActionResult<CaseResponseModel>> GetCaseById(Guid CaseId)
        {
            try
            {
                var languageId = GetLanguageId();
                var Case = await _CaseService.GetByIdAsync(CaseId, languageId);
                return Ok(Case);
            }
            catch (KeyNotFoundException ex)
            {
                return NotFound(ex.Message);
            }
        }

        /// <summary>
        /// Updates an existing Case.
        /// </summary>
        /// <param name="request">The update request model.</param>
        /// <returns>The updated Case response model.</returns>
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
        /// Deletes a Case by its ID.
        /// </summary>
        /// <param name="CaseId">The Case ID.</param>
        /// <returns>NoContent if deleted, NotFound if missing.</returns>
        [Authorize(Roles = "Admin")]
        [HttpDelete("{CaseId}")]
        public async Task<IActionResult> DeleteCase(Guid CaseId)
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
        /// Uploads a media file for Case usage.
        /// </summary>
        /// <param name="request">The upload request containing the media file.</param>
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

        /// <summary>
        /// Retrieves Cases by a list of tag IDs for a given language.
        /// </summary>
        /// <param name="tagIds">List of tag IDs.</param>
        /// <returns>List of matching Case response models.</returns>
        [HttpPost("by-tags")]
        public async Task<ActionResult<IEnumerable<CaseResponseModel>>> GetCasesByTags([FromBody] List<Guid> tagIds)
        {
            var languageId = GetLanguageId();
            var Cases = await _CaseService.GetByTagIdsAsync(tagIds, languageId);
            return Ok(Cases);
        }
    }
}
