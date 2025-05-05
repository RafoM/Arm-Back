using ContentService.Models.RequestModels;
using ContentService.Models.ResponseModels;
using ContentService.Services.Interface;
using Microsoft.AspNetCore.Authorization;
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
        public async Task<ActionResult<IEnumerable<LocalizationResponseModel>>> GetAll()
        {
            return Ok(await _service.GetAllAsync());
        }

        /// <summary>
        /// Gets a localization by ID.
        /// </summary>
        [HttpGet("{id}")]
        public async Task<ActionResult<LocalizationResponseModel>> GetById(int id)
        {
            var result = await _service.GetByIdAsync(id);
            return result == null ? NotFound() : Ok(result);
        }

        /// <summary>
        /// Creates a new localization key.
        /// </summary>
        [Authorize(Roles = "Admin")]
        [HttpPost]
        public async Task<ActionResult<LocalizationResponseModel>> Create([FromBody] LocalizationRequestModel model)
        {
            var result = await _service.CreateAsync(model);
            return CreatedAtAction(nameof(GetById), new { id = result.Id }, result);
        }

        /// <summary>
        /// Updates an existing localization key.
        /// </summary>
        [Authorize(Roles = "Admin")]
        [HttpPut]
        public async Task<ActionResult<LocalizationResponseModel>> Update([FromBody] LocalizationUpdateModel model)
        {
            var result = await _service.UpdateAsync(model);
            return result == null ? NotFound() : Ok(result);
        }

        /// <summary>
        /// Deletes a localization by ID.
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
