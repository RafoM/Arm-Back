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
    /// Controller for managing blog tags.
    /// </summary>
    public class BlogTagsController : BaseController
    {
        private readonly IBlogTagService _tagService;

        /// <summary>
        /// Initializes a new instance of the <see cref="BlogTagsController"/> class.
        /// </summary>
        /// <param name="tagService">Service for blog tag operations.</param>
        public BlogTagsController(IBlogTagService tagService)
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
        /// Creates a new blog tag.
        /// </summary>
        /// <param name="request">The tag creation request model.</param>
        /// <returns>The created tag.</returns>
        [Authorize(Roles = "Admin")]
        [HttpPost]
        public async Task<ActionResult<BlogTagResponseModel>> CreateTag([FromBody] BlogTagRequestModel request)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var createdTag = await _tagService.CreateAsync(request);
            return CreatedAtAction(nameof(GetTagById), new { tagId = createdTag.TagId }, createdTag);
        }

        /// <summary>
        /// Retrieves all blog tags in the requested language.
        /// </summary>
        /// <returns>List of blog tags.</returns>
        [HttpGet]
        public async Task<ActionResult<IEnumerable<BlogTagResponseModel>>> GetAllTags()
        {
            var languageId = GetLanguageId();
            var tags = await _tagService.GetAllAsync(languageId);
            return Ok(tags);
        }

        /// <summary>
        /// Retrieves a single blog tag by ID in the requested language.
        /// </summary>
        /// <param name="tagId">The ID of the blog tag.</param>
        /// <returns>The requested blog tag.</returns>
        [HttpGet("{tagId}")]
        public async Task<ActionResult<BlogTagResponseModel>> GetTagById(Guid tagId)
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
        /// Updates an existing blog tag.
        /// </summary>
        /// <param name="request">The tag update model.</param>
        /// <returns>The updated tag.</returns>
        [Authorize(Roles = "Admin")]
        [HttpPut]
        public async Task<ActionResult<BlogTagResponseModel>> UpdateTag([FromBody] BlogTagUpdateModel request)
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
        /// Deletes a blog tag by ID.
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
