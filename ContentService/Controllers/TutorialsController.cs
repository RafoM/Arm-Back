using ContentService.Models.RequestModels;
using ContentService.Models.ResponseModels;
using ContentService.Services.Interface;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace ContentService.Controllers
{
    [Authorize]
    public class TutorialsController : BaseController
    {
        private readonly ITutorialService _tutorialService;

        public TutorialsController(ITutorialService tutorialService)
        {
            _tutorialService = tutorialService;
        }

        private int GetLanguageId()
        {
            return HttpContext.Items.TryGetValue("LanguageId", out var value) && value is int id
                ? id
                : throw new Exception("LanguageId header is missing or invalid.");
        }

        /// <summary>
        /// Creates a new tutorial.
        /// </summary>
        /// <param name="request">Tutorial creation data.</param>
        /// <returns>The created tutorial.</returns>
        [Authorize(Roles = "Admin")]
        [HttpPost]
        public async Task<ActionResult<TutorialResponseModel>> CreateTutorial([FromBody] TutorialRequestModel request)
        {
            request.LanguageId = GetLanguageId();
            var created = await _tutorialService.CreateTutorialAsync(request);
            return CreatedAtAction(nameof(GetTutorialById), new { id = created.Id }, created);
        }

        /// <summary>
        /// Retrieves all tutorials.
        /// </summary>
        /// <returns>List of tutorials.</returns>
        [HttpGet]
        public async Task<ActionResult<IEnumerable<TutorialResponseModel>>> GetAllTutorials()
        {
            var languageId = GetLanguageId();
            var tutorials = await _tutorialService.GetAllTutorialsAsync(languageId);
            return Ok(tutorials);
        }

        /// <summary>
        /// Retrieves a tutorial by its ID.
        /// </summary>
        /// <param name="id">Tutorial ID.</param>
        /// <returns>The requested tutorial.</returns>
        [HttpGet("{id}")]
        public async Task<ActionResult<TutorialResponseModel>> GetTutorialById(Guid id)
        {
            try
            {
                var languageId = GetLanguageId();
                var tutorial = await _tutorialService.GetTutorialByIdAsync(id, languageId);
                return Ok(tutorial);
            }
            catch (KeyNotFoundException ex)
            {
                return NotFound(ex.Message);
            }
        }

        /// <summary>
        /// Updates a tutorial.
        /// </summary>
        /// <param name="request">Updated tutorial data.</param>
        [Authorize(Roles = "Admin")]
        [HttpPut]
        public async Task<IActionResult> UpdateTutorial([FromBody] TutorialUpdateModel request)
        {
            try
            {
                request.LanguageId = GetLanguageId();
                await _tutorialService.UpdateTutorialAsync(request);
                return NoContent();
            }
            catch (KeyNotFoundException ex)
            {
                return NotFound(ex.Message);
            }
        }

        /// <summary>
        /// Deletes a tutorial.
        /// </summary>
        /// <param name="id">Tutorial ID.</param>
        [Authorize(Roles = "Admin")]
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteTutorial(Guid id)
        {
            try
            {
                await _tutorialService.DeleteTutorialAsync(id);
                return NoContent();
            }
            catch (KeyNotFoundException ex)
            {
                return NotFound(ex.Message);
            }
        }
    }
}
