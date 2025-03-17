using LanguageService.Data.Entity;
using LanguageService.Services.Interface;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace LanguageService.Controllers
{
    public class LanguagesController : BaseController
    {
        private readonly ILanguageService _languageService;

        public LanguagesController(ILanguageService languageService)
        {
            _languageService = languageService;
        }

        /// <summary>
        /// Retrieves all languages.
        /// </summary>
        /// <returns>A list of all available languages.</returns>
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Language>>> GetLanguages()
        {
            var languages = await _languageService.GetAllLanguagesAsync();
            return Ok(languages);
        }

        /// <summary>
        /// Retrieves a specific language by its ID.
        /// </summary>
        /// <param name="id">The ID of the language.</param>
        /// <returns>The language with the specified ID.</returns>
        [HttpGet("{id:int}")]
        public async Task<ActionResult<Language>> GetLanguage(int id)
        {
            var language = await _languageService.GetLanguageByIdAsync(id);
            if (language == null)
                return NotFound();

            return Ok(language);
        }

        /// <summary>
        /// Creates a new language.
        /// </summary>
        /// <param name="language">The language data to create.</param>
        /// <returns>The newly created language.</returns>
        [HttpPost]
        [Authorize(Roles = "Admin")]
        public async Task<ActionResult<Language>> CreateLanguage([FromBody] Language language)
        {
            var created = await _languageService.CreateLanguageAsync(language);
            return CreatedAtAction(nameof(GetLanguage), new { id = created.Id }, created);
        }

        /// <summary>
        /// Updates an existing language.
        /// </summary>
        /// <param name="id">The ID of the language to update.</param>
        /// <param name="language">The updated language data.</param>
        /// <returns>No content if update is successful.</returns>
        [HttpPut("{id:int}")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> UpdateLanguage(int id, [FromBody] Language language)
        {
            await _languageService.UpdateLanguageAsync(id, language);
            return NoContent();
        }

        /// <summary>
        /// Deletes an existing language.
        /// </summary>
        /// <param name="id">The ID of the language to delete.</param>
        /// <returns>No content if deletion is successful.</returns>
        [HttpDelete("{id:int}")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> DeleteLanguage(int id)
        {
            await _languageService.DeleteLanguageAsync(id);
            return NoContent();
        }
    }
}
