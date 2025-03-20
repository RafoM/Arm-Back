using ContentService.Data.Entity;
using ContentService.Models.RequestModels;
using ContentService.Services.Interface;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace ContentService.Controllers
{
    public class TranslationsController : BaseController
    {
        private readonly ITranslationService _translationService;

        public TranslationsController(ITranslationService translationService)
        {
            _translationService = translationService;
        }

        /// <summary>
        /// Retrieves all translations.
        /// </summary>
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Translation>>> GetTranslations([FromQuery] string languageCode, [FromQuery] string entityName = null, [FromQuery] int? entityId = null, [FromQuery] string group = null)
        {
            var translations = await _translationService.GetTranslationsAsync(languageCode, entityName, entityId, group);
            return Ok(translations);
        }

        /// <summary>
        /// Retrieves a specific translation by its ID.
        /// </summary>
        [HttpGet("{id:int}")]
        public async Task<ActionResult<Translation>> GetTranslation(int id)
        {
            var translation = await _translationService.GetTranslationByIdAsync(id);
            if (translation == null)
                return NotFound();
            return Ok(translation);
        }

        /// <summary>
        /// Creates a new translation.
        /// </summary>
        [HttpPost]
        [Authorize(Roles = "Admin")]
        public async Task<ActionResult<Translation>> CreateTranslation([FromBody] TranslationRequestModel translation)
        {
            var created = await _translationService.CreateTranslationAsync(translation);
            return CreatedAtAction(nameof(GetTranslation), new { id = created.Id }, created);
        }

        /// <summary>
        /// Updates an existing translation.
        /// </summary>
        [HttpPut("{id:int}")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> UpdateTranslation(int id, [FromBody] TranslationRequestModel translation)
        {
            await _translationService.UpdateTranslationAsync(id, translation);
            return NoContent();
        }

        /// <summary>
        /// Deletes a translation.
        /// </summary>
        [HttpDelete("{id:int}")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> DeleteTranslation(int id)
        {
            await _translationService.DeleteTranslationAsync(id);
            return NoContent();
        }
    }
}
