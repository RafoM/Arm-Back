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
    public class BlogTagsController : BaseController
    {
        private readonly IBlogTagService _tagService;

        /// <summary>
        /// Initializes a new instance of the <see cref="BlogTagsController"/> class.
        /// </summary>
        /// <param name="tagService">The service handling blog tag operations.</param>
        public BlogTagsController(IBlogTagService tagService)
        {
            _tagService = tagService;
        }

        /// <summary>
        /// Creates a new BlogTag record based on the provided request.
        /// </summary>
        /// <param name="request">Request model containing blog tag data.</param>
        /// <returns>
        /// The newly created <see cref="BlogTagResponseModel"/> with its assigned ID.
        /// </returns>
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
        /// Retrieves all existing tags.
        /// </summary>
        /// <returns>A list of <see cref="BlogTagResponseModel"/> objects.</returns>
        [HttpGet]
        public async Task<ActionResult<IEnumerable<BlogTagResponseModel>>> GetAllTags()
        {
            var tags = await _tagService.GetAllAsync();
            return Ok(tags);
        }

        /// <summary>
        /// Retrieves a specific BlogTag by its unique identifier.
        /// </summary>
        /// <param name="tagId">ID of the tag to be retrieved.</param>
        /// <returns>
        /// The requested <see cref="BlogTagResponseModel"/> if found; otherwise 404 Not Found.
        /// </returns>
        [HttpGet("{tagId}")]
        public async Task<ActionResult<BlogTagResponseModel>> GetTagById(int tagId)
        {
            try
            {
                var tag = await _tagService.GetByIdAsync(tagId);
                return Ok(tag);
            }
            catch (KeyNotFoundException ex)
            {
                return NotFound(ex.Message);
            }
        }

        /// <summary>
        /// Updates an existing BlogTag with new data.
        /// </summary>
        /// <param name="request">The update model containing new tag data and its ID.</param>
        /// <returns>
        /// The updated <see cref="BlogTagResponseModel"/> if successful; otherwise 404 Not Found.
        /// </returns>
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
        /// Deletes a specific BlogTag by its unique identifier.
        /// </summary>
        /// <param name="tagId">ID of the tag to be deleted.</param>
        /// <returns>No content if successful, or 404 Not Found if the tag does not exist.</returns>
        [Authorize(Roles = "Admin")]
        [HttpDelete("{tagId}")]
        public async Task<IActionResult> DeleteTag(int tagId)
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
