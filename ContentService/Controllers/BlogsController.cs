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
    /// Controller for managing blog content.
    /// </summary>
    public class BlogsController : BaseController
    {
        private readonly IBlogService _blogService;

        /// <summary>
        /// Initializes a new instance of the <see cref="BlogsController"/> class.
        /// </summary>
        /// <param name="blogService">Service for blog operations.</param>
        public BlogsController(IBlogService blogService)
        {
            _blogService = blogService;
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
        /// Creates a new blog.
        /// </summary>
        /// <param name="request">The blog creation request model.</param>
        /// <returns>The created blog response model.</returns>
        [Authorize(Roles = "Admin")]
        [HttpPost]
        public async Task<ActionResult<BlogResponseModel>> CreateBlog([FromBody] BlogRequestModel request)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var result = await _blogService.CreateAsync(request);
            return CreatedAtAction(nameof(GetBlogById), new { blogId = result.BlogId }, result);
        }

        /// <summary>
        /// Retrieves all blogs for a specific language.
        /// </summary>
        /// <returns>List of blog response models.</returns>
        [HttpGet]
        public async Task<ActionResult<IEnumerable<BlogResponseModel>>> GetAllBlogs()
        {
            var languageId = GetLanguageId();
            var blogs = await _blogService.GetAllAsync(languageId);
            return Ok(blogs);
        }

        /// <summary>
        /// Retrieves a blog by its unique ID and language.
        /// </summary>
        /// <param name="blogId">The blog ID.</param>
        /// <returns>The blog response model.</returns>
        [HttpGet("{blogId}")]
        public async Task<ActionResult<BlogResponseModel>> GetBlogById(Guid blogId)
        {
            try
            {
                var languageId = GetLanguageId();
                var blog = await _blogService.GetByIdAsync(blogId, languageId);
                return Ok(blog);
            }
            catch (KeyNotFoundException ex)
            {
                return NotFound(ex.Message);
            }
        }

        /// <summary>
        /// Updates an existing blog.
        /// </summary>
        /// <param name="request">The update request model.</param>
        /// <returns>The updated blog response model.</returns>
        [Authorize(Roles = "Admin")]
        [HttpPut]
        public async Task<ActionResult<BlogResponseModel>> UpdateBlog([FromBody] BlogUpdateModel request)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            try
            {
                var updated = await _blogService.UpdateAsync(request);
                return Ok(updated);
            }
            catch (KeyNotFoundException ex)
            {
                return NotFound(ex.Message);
            }
        }

        /// <summary>
        /// Deletes a blog by its ID.
        /// </summary>
        /// <param name="blogId">The blog ID.</param>
        /// <returns>NoContent if deleted, NotFound if missing.</returns>
        [Authorize(Roles = "Admin")]
        [HttpDelete("{blogId}")]
        public async Task<IActionResult> DeleteBlog(Guid blogId)
        {
            try
            {
                await _blogService.DeleteAsync(blogId);
                return NoContent();
            }
            catch (KeyNotFoundException ex)
            {
                return NotFound(ex.Message);
            }
        }

        /// <summary>
        /// Uploads a media file for blog usage.
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

            var mediaUrl = await _blogService.UploadBlogMediaAsync(request.MediaFile);
            return Ok(mediaUrl);
        }

        /// <summary>
        /// Retrieves blogs by a list of tag IDs for a given language.
        /// </summary>
        /// <param name="tagIds">List of tag IDs.</param>
        /// <returns>List of matching blog response models.</returns>
        [HttpPost("by-tags")]
        public async Task<ActionResult<IEnumerable<BlogResponseModel>>> GetBlogsByTags([FromBody] List<Guid> tagIds)
        {
            var languageId = GetLanguageId();
            var blogs = await _blogService.GetByTagIdsAsync(tagIds, languageId);
            return Ok(blogs);
        }
    }
}
