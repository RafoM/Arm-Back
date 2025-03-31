using ContentService.Models.RequestModels;
using ContentService.Services.Interface;
using Microsoft.AspNetCore.Mvc;

namespace ContentService.Controllers
{
    public class LocalizationController : BaseController
    {
        private readonly ILocalizationService _service;

        public LocalizationController(ILocalizationService service)
        {
            _service = service;
        }

        /// <summary>
        /// Gets all localizations.
        /// </summary>
        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            return Ok(await _service.GetAllAsync());
        }

        /// <summary>
        /// Gets a localization by ID.
        /// </summary>
        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(int id)
        {
            var result = await _service.GetByIdAsync(id);
            return result == null ? NotFound() : Ok(result);
        }

        /// <summary>
        /// Creates a new localization key.
        /// </summary>
        [HttpPost]
        public async Task<IActionResult> Create([FromBody] LocalizationRequestModel model)
        {
            var result = await _service.CreateAsync(model);
            return CreatedAtAction(nameof(GetById), new { id = result.Id }, result);
        }

        /// <summary>
        /// Updates an existing localization key.
        /// </summary>
        [HttpPut]
        public async Task<IActionResult> Update([FromBody] LocalizationUpdateModel model)
        {
            var result = await _service.UpdateAsync(model);
            return result == null ? NotFound() : Ok(result);
        }

        /// <summary>
        /// Deletes a localization by ID.
        /// </summary>
        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            var success = await _service.DeleteAsync(id);
            return success ? NoContent() : NotFound();
        }
    }
}
