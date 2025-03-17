using LanguageService.Data.Entity;
using LanguageService.Services.Interface;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace LanguageService.Controllers
{
    public class TranslationsController : BaseController
    {
        private readonly ITranslationService _translationService;

        public TranslationsController(ITranslationService translationService)
        {
            _translationService = translationService;
        }

        /// <summary>
        /// Retrieves all translations along with their associated language details.
        /// </summary>
        /// <returns>A list of translations.</returns>
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Translation>>> GetTranslations()
        {
            var translations = await _translationService.GetAllTranslationsAsync();
            return Ok(translations);
        }

        /// <summary>
        /// Retrieves a specific translation by its ID.
        /// </summary>
        /// <param name="id">The ID of the translation.</param>
        /// <returns>The translation with the specified ID.</returns>
        [HttpGet("{id:int}")]
        public async Task<ActionResult<Translation>> GetTranslation(int id)
        {
            var translation = await _translationService.GetTranslationByIdAsync(id);
            if (translation == null)
                return NotFound();

            return Ok(translation);
        }

        /// <summary>
        /// Retrieves translations for a specific language by its ID.
        /// </summary>
        /// <param name="languageId">The ID of the language.</param>
        /// <returns>A list of translations for the specified language.</returns>
        [HttpGet("by-language/{languageId:int}")]
        public async Task<ActionResult<IEnumerable<Translation>>> GetTranslationsByLanguageId(int languageId)
        {
            var translations = await _translationService.GetTranslationsByLanguageIdAsync(languageId);
            return Ok(translations);
        }

        /// <summary>
        /// Creates a new translation.
        /// </summary>
        /// <param name="translation">The translation data to create.</param>
        /// <returns>The newly created translation.</returns>
        [HttpPost]
        [Authorize(Roles = "Admin")]
        public async Task<ActionResult<Translation>> CreateTranslation([FromBody] Translation translation)
        {
            var created = await _translationService.CreateTranslationAsync(translation);
            return CreatedAtAction(nameof(GetTranslation), new { id = created.Id }, created);
        }

        /// <summary>
        /// Updates an existing translation.
        /// </summary>
        /// <param name="id">The ID of the translation to update.</param>
        /// <param name="translation">The updated translation data.</param>
        /// <returns>No content if update is successful.</returns>
        [HttpPut("{id:int}")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> UpdateTranslation(int id, [FromBody] Translation translation)
        {
            await _translationService.UpdateTranslationAsync(id, translation);
            return NoContent();
        }

        /// <summary>
        /// Deletes an existing translation.
        /// </summary>
        /// <param name="id">The ID of the translation to delete.</param>
        /// <returns>No content if deletion is successful.</returns>
        [HttpDelete("{id:int}")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> DeleteTranslation(int id)
        {
            await _translationService.DeleteTranslationAsync(id);
            return NoContent();
        }
    }
}
