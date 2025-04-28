using ContentService.Models.RequestModels;
using ContentService.Services.Interface;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace ContentService.Controllers
{
    public class PageController : BaseController
    {
        private readonly IPageService _pageService;

        public PageController(IPageService pageService)
        {
            _pageService = pageService;
        }

        /// <summary>
        /// Gets all pages.
        /// </summary>
        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            return Ok(await _pageService.GetAllAsync());
        }

        /// <summary>
        /// Gets a page by ID.
        /// </summary>
        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(int id)
        {
            var result = await _pageService.GetByIdAsync(id);
            return result == null ? NotFound() : Ok(result);
        }

        /// <summary>
        /// Creates a new page.
        /// </summary>
        [Authorize(Roles = "Admin")]
        [HttpPost]
        public async Task<IActionResult> Create([FromBody] PageRequestModel model)
        {
            var result = await _pageService.CreateAsync(model);
            return CreatedAtAction(nameof(GetById), new { id = result.Id }, result);
        }

        /// <summary>
        /// Updates an existing page.
        /// </summary>
        [Authorize(Roles = "Admin")]
        [HttpPut]
        public async Task<IActionResult> Update([FromBody] PageUpdateModel model)
        {
            var result = await _pageService.UpdateAsync(model);
            return result == null ? NotFound() : Ok(result);
        }

        /// <summary>
        /// Deletes a page by ID.
        /// </summary>
        [Authorize(Roles = "Admin")]
        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            var success = await _pageService.DeleteAsync(id);
            return success ? NoContent() : NotFound();
        }
    }
}
