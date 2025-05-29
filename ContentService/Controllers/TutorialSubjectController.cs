using ContentService.Models.RequestModels;
using ContentService.Models.ResponseModels;
using ContentService.Services.Interface;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace ContentService.Controllers
{
    [Authorize]
    public class TutorialSubjectController : BaseController
    {
        private readonly ITutorialSubjectService _tutorialSubjectService;

        public TutorialSubjectController(ITutorialSubjectService tutorialSubjectService)
        {
            _tutorialSubjectService = tutorialSubjectService;
        }

        private int GetLanguageId()
        {
            if (HttpContext.Items.TryGetValue("LanguageId", out var value) && value is int id)
                return id;

            throw new Exception("LanguageId header is missing or invalid.");
        }

        /// <summary>
        /// Create a new tutorial subject with translations.
        /// </summary>
        [Authorize(Roles = "Admin")]
        [HttpPost]
        public async Task<ActionResult<TutorialSubjectResponseModel>> Create([FromBody] TutorialSubjectRequestModel request)
        {
            var created = await _tutorialSubjectService.CreateAsync(request);
            return CreatedAtAction(nameof(GetById), new { id = created.Id }, created);
        }

        /// <summary>
        /// Get all tutorial subjects with their translations.
        /// </summary>
        [HttpGet]
        public async Task<ActionResult<List<TutorialSubjectResponseModel>>> GetAll()
        {
            var result = await _tutorialSubjectService.GetAllAsync();
            return Ok(result);
        }

        /// <summary>
        /// Get a tutorial subject by ID.
        /// </summary>
        [HttpGet("{id}")]
        public async Task<ActionResult<TutorialSubjectResponseModel>> GetById(Guid id)
        {
            try
            {
                var languageId = GetLanguageId();
                var subject = await _tutorialSubjectService.GetByIdAsync(id, languageId);
                return Ok(subject);
            }
            catch (KeyNotFoundException ex)
            {
                return NotFound(ex.Message);
            }
        }

        /// <summary>
        /// Update a tutorial subject and its translations.
        /// </summary>
        [Authorize(Roles = "Admin")]
        [HttpPut]
        public async Task<IActionResult> Update([FromBody] TutorialSubjectUpdateModel request)
        {
            try
            {
                await _tutorialSubjectService.UpdateAsync(request);
                return NoContent();
            }
            catch (KeyNotFoundException ex)
            {
                return NotFound(ex.Message);
            }
        }

        /// <summary>
        /// Delete a tutorial subject.
        /// </summary>
        [Authorize(Roles = "Admin")]
        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(Guid id)
        {
            try
            {
                await _tutorialSubjectService.DeleteAsync(id);
                return NoContent();
            }
            catch (KeyNotFoundException ex)
            {
                return NotFound(ex.Message);
            }
        }
    }
}
