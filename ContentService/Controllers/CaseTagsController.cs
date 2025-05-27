using ContentService.Models.RequestModels;
using ContentService.Models.ResponseModels;
using ContentService.Services.Interface;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Threading.Tasks;
using System;

namespace ContentService.Controllers
{
    /// <summary>
    /// Controller for managing Case tags.
    /// </summary>
    public class CaseTagsController : BaseController
    {
        private readonly ICaseTagService _tagService;

        /// <summary>
        /// Initializes a new instance of the <see cref="CaseTagsController"/> class.
        /// </summary>
        /// <param name="tagService">Service for Case tag operations.</param>
        public CaseTagsController(ICaseTagService tagService)
        {
            _tagService = tagService;
        }

        /// <summary>
        /// Retrieves the language ID from the request headers (set by middleware).
        /// </summary>
        /// <returns>Language ID as an integer.</returns>
        /// <exception cref="Exception">Thrown when language ID is missing.</exception>
        private int GetLanguageId()
        {
            return HttpContext.Items.TryGetValue("LanguageId", out var value) && value is int id
                ? id
                : throw new Exception("LanguageId header is missing or invalid.");
        }

        /// <summary>
        /// Creates a new Case tag.
        /// </summary>
        /// <param name="request">The tag creation request model.</param>
        /// <returns>The created tag.</returns>
        [Authorize(Roles = "Admin")]
        [HttpPost]
        public async Task<ActionResult<CaseTagResponseModel>> CreateTag([FromBody] CaseTagRequestModel request)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var createdTag = await _tagService.CreateAsync(request);
            return CreatedAtAction(nameof(GetTagById), new { tagId = createdTag.TagId }, createdTag);
        }

        /// <summary>
        /// Retrieves all Case tags in the requested language.
        /// </summary>
        /// <returns>List of Case tags.</returns>
        [HttpGet]
        public async Task<ActionResult<IEnumerable<CaseTagResponseModel>>> GetAllTags()
        {
            var languageId = GetLanguageId();
            var tags = await _tagService.GetAllAsync(languageId);
            return Ok(tags);
        }

        /// <summary>
        /// Retrieves a single Case tag by ID in the requested language.
        /// </summary>
        /// <param name="tagId">The ID of the Case tag.</param>
        /// <returns>The requested Case tag.</returns>
        [HttpGet("{tagId}")]
        public async Task<ActionResult<CaseTagResponseModel>> GetTagById(Guid tagId)
        {
            try
            {
                var languageId = GetLanguageId();
                var tag = await _tagService.GetByIdAsync(tagId, languageId);
                return Ok(tag);
            }
            catch (KeyNotFoundException ex)
            {
                return NotFound(ex.Message);
            }
        }

        /// <summary>
        /// Updates an existing Case tag.
        /// </summary>
        /// <param name="request">The tag update model.</param>
        /// <returns>The updated tag.</returns>
        [Authorize(Roles = "Admin")]
        [HttpPut]
        public async Task<ActionResult<CaseTagResponseModel>> UpdateTag([FromBody] CaseTagUpdateModel request)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            try
            {
                var updatedTag = await _tagService.UpdateAsync(request);
                return Ok(updatedTag);
            }
            catch (KeyNotFoundException ex)
            {
                return NotFound(ex.Message);
            }
        }

        /// <summary>
        /// Deletes a Case tag by ID.
        /// </summary>
        /// <param name="tagId">The ID of the tag to delete.</param>
        /// <returns>HTTP 204 No Content if successful.</returns>
        [Authorize(Roles = "Admin")]
        [HttpDelete("{tagId}")]
        public async Task<IActionResult> DeleteTag(Guid tagId)
        {
            try
            {
                await _tagService.DeleteAsync(tagId);
                return NoContent();
            }
            catch (KeyNotFoundException ex)
            {
                return NotFound(ex.Message);
            }
        }
    }
}
