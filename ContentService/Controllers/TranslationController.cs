using ContentService.Data.Entity;
using ContentService.Models.RequestModels;
using ContentService.Services.Interface;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace ContentService.Controllers
{
    public class TranslationController : BaseController
    {
        private readonly ITranslationService _service;

        public TranslationController(ITranslationService service)
        {
            _service = service;
        }

        /// <summary>
        /// Gets all translations for a given page and language.
        /// </summary>
        /// <param name="pageId">The ID of the page.</param>
        /// <param name="languageId">The ID of the language.</param>
        [HttpGet("by-page")]
        public async Task<IActionResult> GetByPage([FromQuery] int pageId, [FromQuery] int languageId)
        {
            if (pageId <= 0 || languageId <= 0)
                return BadRequest("Valid pageId and languageId are required.");

            var result = await _service.GetTranslationsByPageAsync(pageId, languageId);
            return Ok(result);
        }
        
        /// <summary>
        /// Gets all translations.
        /// </summary>
        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            return Ok(await _service.GetAllAsync());
        }

        /// <summary>
        /// Gets a translation by ID.
        /// </summary>
        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(int id)
        {
            var result = await _service.GetByIdAsync(id);
            return result == null ? NotFound() : Ok(result);
        }

        /// <summary>
        /// Creates a new translation.
        /// </summary>
        [Authorize(Roles = "Admin")]
        [HttpPost]
        public async Task<IActionResult> Create([FromBody] TranslationRequestModel model)
        {
            var result = await _service.CreateAsync(model);
            return CreatedAtAction(nameof(GetById), new { id = result.Id }, result);
        }

        /// <summary>
        /// Updates an existing translation.
        /// </summary>
        [Authorize(Roles = "Admin")]
        [HttpPut("{id}")]
        public async Task<IActionResult> Update([FromBody] TranslationUpdateModel model)
        {
            var result = await _service.UpdateAsync(model);
            return result == null ? NotFound() : Ok(result);
        }

        /// <summary>
        /// Deletes a translation by ID.
        /// </summary>
        [Authorize(Roles = "Admin")]
        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            var success = await _service.DeleteAsync(id);
            return success ? NoContent() : NotFound();
        }
    }
}
