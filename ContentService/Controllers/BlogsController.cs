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
   
    public class BlogsController : BaseController
    {
        private readonly IBlogService _blogService;

        /// <summary>
        /// Initializes a new instance of the <see cref="BlogsController"/> class.
        /// </summary>
        /// <param name="blogService">Service handling blog-related operations.</param>
        public BlogsController(IBlogService blogService)
        {
            _blogService = blogService;
        }

        /// <summary>
        /// Creates a new blog using the provided request model.
        /// </summary>
        /// <param name="request">The blog data to create.</param>
        /// <returns>
        /// A newly created <see cref="BlogResponseModel"/>, along with a 201 Created response.
        /// </returns>
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
        /// Retrieves a list of all existing blogs.
        /// </summary>
        /// <returns>
        /// A collection of <see cref="BlogResponseModel"/>.
        /// </returns>
        [HttpGet]
        public async Task<ActionResult<IEnumerable<BlogResponseModel>>> GetAllBlogs()
        {
            var blogs = await _blogService.GetAllAsync();
            return Ok(blogs);
        }

        /// <summary>
        /// Retrieves a specific blog by its unique identifier.
        /// </summary>
        /// <param name="blogId">The unique identifier for the blog.</param>
        /// <returns>
        /// A single <see cref="BlogResponseModel"/> if found; otherwise 404 Not Found.
        /// </returns>
        [HttpGet("{blogId}")]
        public async Task<ActionResult<BlogResponseModel>> GetBlogById(int blogId)
        {
            try
            {
                var blog = await _blogService.GetByIdAsync(blogId);
                return Ok(blog);
            }
            catch (KeyNotFoundException ex)
            {
                return NotFound(ex.Message);
            }
        }

        /// <summary>
        /// Updates an existing blog using the provided update model.
        /// </summary>
        /// <param name="request">The update model containing the new values.</param>
        /// <returns>
        /// The updated <see cref="BlogResponseModel"/> if found; otherwise 404 Not Found.
        /// </returns>
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
        /// Deletes an existing blog by its unique identifier.
        /// </summary>
        /// <param name="blogId">The ID of the blog to be deleted.</param>
        /// <returns>
        /// A 204 No Content response if the blog was deleted; 404 Not Found if it does not exist.
        /// </returns>
        [Authorize(Roles = "Admin")]
        [HttpDelete("{blogId}")]
        public async Task<IActionResult> DeleteBlog(int blogId)
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
    }
}
