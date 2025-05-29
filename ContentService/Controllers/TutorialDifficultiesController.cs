using ContentService.Models.RequestModels;
using ContentService.Models.ResponseModels;
using ContentService.Services.Interface;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace ContentService.Controllers
{
    [Authorize]
    public class TutorialDifficultiesController : BaseController
    {
        private readonly ITutorialDifficultyService _difficultyService;

        public TutorialDifficultiesController(ITutorialDifficultyService difficultyService)
        {
            _difficultyService = difficultyService;
        }

        private int GetLanguageId()
        {
            return HttpContext.Items.TryGetValue("LanguageId", out var value) && value is int id
                ? id
                : throw new Exception("LanguageId header is missing or invalid.");
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<TutorialDifficultyResponseModel>>> GetAll()
        {
            var languageId = GetLanguageId();
            var result = await _difficultyService.GetAllAsync(languageId);
            return Ok(result);
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<TutorialDifficultyResponseModel>> GetById(Guid id)
        {
            var languageId = GetLanguageId();
            var result = await _difficultyService.GetByIdAsync(id, languageId);
            return Ok(result);
        }

        [HttpPost]
        [Authorize(Roles = "Admin")]
        public async Task<ActionResult<TutorialDifficultyResponseModel>> Create([FromBody] TutorialDifficultyRequestModel request)
        {
            request.LanguageId = GetLanguageId();
            var result = await _difficultyService.CreateAsync(request);
            return CreatedAtAction(nameof(GetById), new { id = result.Id }, result);
        }

        [HttpPut]
        [Authorize(Roles = "Admin")]
        public async Task<ActionResult<TutorialDifficultyResponseModel>> Update([FromBody] TutorialDifficultyUpdateModel request)
        {
            request.LanguageId = GetLanguageId();
            var result = await _difficultyService.UpdateAsync(request);
            return Ok(result);
        }

        [HttpDelete("{id}")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Delete(Guid id)
        {
            await _difficultyService.DeleteAsync(id);
            return NoContent();
        }
    }
}

