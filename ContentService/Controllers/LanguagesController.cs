using ContentService.Data.Entity;
using ContentService.Models.RequestModels;
using ContentService.Services.Interface;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace ContentService.Controllers
{
    public class LanguagesController : BaseController
    {
        private readonly ILanguageService _languageService;

        public LanguagesController(ILanguageService languageService)
        {
            _languageService = languageService;
        }

        /// <summary>
        /// Retrieves all available languages.
        /// </summary>
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Language>>> GetLanguages()
        {
            var languages = await _languageService.GetAllLanguagesAsync();
            return Ok(languages);
        }

        /// <summary>
        /// Retrieves a specific language by its ID.
        /// </summary>
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
        [HttpPost]
        [Authorize(Roles = "Admin")]
        public async Task<ActionResult<Language>> CreateLanguage([FromBody] LanguageRequestModel language)
        {
            var created = await _languageService.CreateLanguageAsync(language);
            return CreatedAtAction(nameof(GetLanguage), new { id = created.Id }, created);
        }

        /// <summary>
        /// Updates an existing language.
        /// </summary>
        [HttpPut("{id:int}")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> UpdateLanguage(int id, [FromBody] LanguageRequestModel language)
        {
            await _languageService.UpdateLanguageAsync(id, language);
            return NoContent();
        }

        /// <summary>
        /// Deletes/deactivates a language.
        /// </summary>
        [HttpDelete("{id:int}")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> DeleteLanguage(int id)
        {
            await _languageService.DeleteLanguageAsync(id);
            return NoContent();
        }

        /// <summary>
        /// Uploads a flag image for a language.
        /// </summary>
        [HttpPost("{id:int}/upload-flag")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> UploadFlag(int id, IFormFile flagFile)
        {
            var flagUrl = await _languageService.UploadFlagAsync(id, flagFile);
            return Ok(new { FlagUrl = flagUrl });
        }
    }
}
