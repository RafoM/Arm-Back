using ContentService.Models.RequestModels;
using ContentService.Services.Interface;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace ContentService.Controllers
{
    [Authorize]
    public class LessonsController : BaseController
    {
        private readonly ILessonService _lessonService;

        public LessonsController(ILessonService lessonService)
        {
            _lessonService = lessonService;
        }

        /// <summary>
        /// Creates a new lesson for a tutorial.
        /// </summary>
        /// <param name="tutorialId">Tutorial ID.</param>
        /// <param name="request">Lesson creation data.</param>
        /// <returns>The created lesson.</returns>
        [Authorize(Roles = "Admin")]
        [HttpPost]
        public async Task<IActionResult> CreateLesson(int tutorialId, [FromBody] LessonRequestModel request)
            => Ok(await _lessonService.CreateLessonAsync(tutorialId, request));

        /// <summary>
        /// Retrieves all lessons for a tutorial.
        /// </summary>
        /// <param name="tutorialId">Tutorial ID.</param>
        /// <returns>List of lessons.</returns>
        [HttpGet]
        public async Task<IActionResult> GetLessons(int tutorialId)
            => Ok(await _lessonService.GetLessonsAsync(tutorialId));

        /// <summary>
        /// Retrieves a specific lesson by lesson number.
        /// </summary>
        /// <param name="tutorialId">Tutorial ID.</param>
        /// <param name="lessonNumber">Lesson number.</param>
        /// <returns>The requested lesson.</returns>
        [HttpGet("{lessonNumber}")]
        public async Task<IActionResult> GetLessonByNumber(int tutorialId, int lessonNumber)
            => Ok(await _lessonService.GetLessonByNumberAsync(tutorialId, lessonNumber));

        /// <summary>
        /// Updates a specific lesson.
        /// </summary>
        /// <param name="tutorialId">Tutorial ID.</param>
        /// <param name="lessonNumber">Lesson number.</param>
        /// <param name="request">Updated lesson data.</param>
        [Authorize(Roles = "Admin")]
        [HttpPut]
        public async Task<IActionResult> UpdateLesson([FromBody] LessonUpdateModel request)
        {
            await _lessonService.UpdateLessonAsync(request);
            return NoContent();
        }

        /// <summary>
        /// Deletes a specific lesson.
        /// </summary>
        /// <param name="tutorialId">Tutorial ID.</param>
        /// <param name="lessonNumber">Lesson number.</param>
        [Authorize(Roles = "Admin")]
        [HttpDelete("{lessonNumber}")]
        public async Task<IActionResult> DeleteLesson(int tutorialId, int lessonNumber)
        {
            await _lessonService.DeleteLessonAsync(tutorialId, lessonNumber);
            return NoContent();
        }
    }
}
