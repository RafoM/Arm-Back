using ContentService.Data.Entity;
using ContentService.Models.RequestModels;
using ContentService.Services.Interface;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Ocelot.Values;

namespace ContentService.Controllers
{
    public class LanguageController : BaseController
    {
        private readonly ILanguageService _languageService;

        public LanguageController(ILanguageService languageService)
        {
            _languageService = languageService;
        }

        /// <summary>
        /// Gets a list of all available languages.
        /// </summary>
        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var result = await _languageService.GetAllAsync();
            return Ok(result);
        }

        /// <summary>
        /// Gets a language by its ID.
        /// </summary>
        /// <param name="id">The ID of the language.</param>
        /// <returns>The language object if found.</returns>
        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(int id)
        {
            var result = await _languageService.GetByIdAsync(id);
            return result == null ? NotFound() : Ok(result);
        }

        /// <summary>
        /// Creates a new language.
        /// </summary>
        /// <param name="model">The language data to create.</param>
        /// <returns>The created language object.</returns>
        [HttpPost]
        public async Task<IActionResult> Create([FromBody] LanguageRequestModel model)
        {
            var result = await _languageService.CreateAsync(model);
            return CreatedAtAction(nameof(GetById), new { id = result }, result);
        }

        /// <summary>
        /// Updates an existing language.
        /// </summary>
        /// <param name="model">The updated language data.</param>
        /// <returns>The updated language object if successful.</returns>
        [HttpPut]
        public async Task<IActionResult> Update([FromBody] LanguageUpdateModel model)
        {
            try
            {
                var result = await _languageService.UpdateAsync(model);
                return result ? Ok() : BadRequest("Update failed.");
            }
            catch (InvalidOperationException ex)
            {
                return NotFound(ex.Message);
            }
        }

        /// <summary>
        /// Deletes a language by ID.
        /// </summary>
        /// <param name="id">The ID of the language to delete.</param>
        /// <returns>No content if deleted successfully.</returns>
        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            var success = await _languageService.DeleteAsync(id);
            return success ? NoContent() : NotFound();
        }
    }
}
