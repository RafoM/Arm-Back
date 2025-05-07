using ContentService.Data.Entity;
using ContentService.Models.RequestModels;
using ContentService.Models.ResponseModels;
using ContentService.Services.Implementation;
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
        /// Gets all translations.
        /// </summary>
        [HttpGet]
        public async Task<ActionResult<IEnumerable<TranslationResponseModel>>> GetAll()
        {
            return Ok(await _service.GetAllAsync());
        }

        /// <summary>
        /// Gets a translation by ID.
        /// </summary>
        [HttpGet("{id}")]
        public async Task<ActionResult<TranslationResponseModel>> GetById(int id)
        {
            var result = await _service.GetByIdAsync(id);
            return result == null ? NotFound() : Ok(result);
        }
        /// <summary>
        /// Get all translations by language ID as a dictionary (Key: Localization.Key, Value: Translation.Value).
        /// </summary>
        [HttpGet("by-language/{languageId}")]
        public async Task<ActionResult<Dictionary<string, string>>> GetTranslationsByLanguageId(int languageId)
        {
            var result = await _service.GetTranslationsByLanguageIdAsync(languageId);
            return Ok(result);
        }
        /// <summary>
        /// Creates a new translation.
        /// </summary>
        [Authorize(Roles = "Admin")]
        [HttpPost]
        public async Task<ActionResult<TranslationResponseModel>> Create([FromBody] TranslationRequestModel model)
        {
            var result = await _service.CreateAsync(model);
            return CreatedAtAction(nameof(GetById), new { id = result.Id }, result);
        }

        /// <summary>
        /// Updates an existing translation.
        /// </summary>
        [Authorize(Roles = "Admin")]
        [HttpPut("{id}")]
        public async Task<ActionResult<TranslationResponseModel>> Update([FromBody] TranslationUpdateModel model)
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
