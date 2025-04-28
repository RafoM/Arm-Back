using ContentService.Models.RequestModels;
using ContentService.Services.Interface;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace ContentService.Controllers
{
    [Authorize]
    public class TutorialsController :BaseController
    {
        private readonly ITutorialService _tutorialService;

        public TutorialsController(ITutorialService tutorialService)
        {
            _tutorialService = tutorialService;
        }

        /// <summary>
        /// Creates a new tutorial.
        /// </summary>
        /// <param name="request">Tutorial creation data.</param>
        /// <returns>The created tutorial.</returns>
        [Authorize(Roles = "Admin")]
        [HttpPost]
        public async Task<IActionResult> CreateTutorial([FromBody] TutorialRequestModel request)
            => Ok(await _tutorialService.CreateTutorialAsync(request));

        /// <summary>
        /// Retrieves all tutorials.
        /// </summary>
        /// <returns>List of tutorials.</returns>
        [HttpGet]
        public async Task<IActionResult> GetAllTutorials()
            => Ok(await _tutorialService.GetAllTutorialsAsync());

        /// <summary>
        /// Retrieves a tutorial by its ID.
        /// </summary>
        /// <param name="id">Tutorial ID.</param>
        /// <returns>The requested tutorial.</returns>
        [HttpGet("{id}")]
        public async Task<IActionResult> GetTutorialById(int id)
            => Ok(await _tutorialService.GetTutorialByIdAsync(id));

        /// <summary>
        /// Updates a tutorial.
        /// </summary>
        /// <param name="id">Tutorial ID.</param>
        /// <param name="request">Updated tutorial data.</param>
        [Authorize(Roles = "Admin")]
        [HttpPut]
        public async Task<IActionResult> UpdateTutorial([FromBody] TutorialUpdateModel request)
        {
            await _tutorialService.UpdateTutorialAsync(request);
            return NoContent();
        }

        /// <summary>
        /// Deletes a tutorial.
        /// </summary>
        /// <param name="id">Tutorial ID.</param>
        [Authorize(Roles = "Admin")]
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteTutorial(int id)
        {
            await _tutorialService.DeleteTutorialAsync(id);
            return NoContent();
        }
    }
}
