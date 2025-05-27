using ContentService.Models.RequestModels;
using ContentService.Models.ResponseModels;
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

        private int GetLanguageId()
        {
            return HttpContext.Items.TryGetValue("LanguageId", out var value) && value is int id
                ? id
                : throw new Exception("LanguageId header is missing or invalid.");
        }

        /// <summary>
        /// Creates a new lesson for a tutorial.
        /// </summary>
        [Authorize(Roles = "Admin")]
        [HttpPost("{tutorialId}")]
        public async Task<ActionResult<LessonResponseModel>> CreateLesson(Guid tutorialId, [FromBody] LessonRequestModel request)
        {
            request.LanguageId = GetLanguageId();
            var createdLesson = await _lessonService.CreateLessonAsync(tutorialId, request);
            return CreatedAtAction(nameof(GetLessonByNumber), new { tutorialId, lessonNumber = createdLesson.LessonNumber }, createdLesson);
        }

        /// <summary>
        /// Retrieves all lessons for a tutorial.
        /// </summary>
        [HttpGet("{tutorialId}")]
        public async Task<ActionResult<IEnumerable<LessonResponseModel>>> GetLessons(Guid tutorialId)
        {
            var languageId = GetLanguageId();
            var lessons = await _lessonService.GetLessonsAsync(tutorialId, languageId);
            return Ok(lessons);
        }

        /// <summary>
        /// Retrieves a specific lesson by lesson number.
        /// </summary>
        [HttpGet("{tutorialId}/lesson/{lessonNumber}")]
        public async Task<ActionResult<LessonResponseModel>> GetLessonByNumber(Guid tutorialId, int lessonNumber)
        {
            var languageId = GetLanguageId();
            var lesson = await _lessonService.GetLessonByNumberAsync(tutorialId, lessonNumber, languageId);
            return Ok(lesson);
        }

        /// <summary>
        /// Updates a specific lesson.
        /// </summary>
        [Authorize(Roles = "Admin")]
        [HttpPut]
        public async Task<IActionResult> UpdateLesson([FromBody] LessonUpdateModel request)
        {
            request.LanguageId = GetLanguageId();
            await _lessonService.UpdateLessonAsync(request);
            return NoContent();
        }

        /// <summary>
        /// Deletes a specific lesson.
        /// </summary>
        [Authorize(Roles = "Admin")]
        [HttpDelete("{tutorialId}/lesson/{lessonNumber}")]
        public async Task<IActionResult> DeleteLesson(Guid tutorialId, int lessonNumber)
        {
            await _lessonService.DeleteLessonAsync(tutorialId, lessonNumber);
            return NoContent();
        }

        /// <summary>
        /// Uploads a media file for lesson content.
        /// </summary>
        [Authorize(Roles = "Admin")]
        [HttpPost("upload-media")]
        [Consumes("multipart/form-data")]
        [ProducesResponseType(typeof(string), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<ActionResult<string>> UploadMedia([FromForm] UploadMediaRequest request)
        {
            if (request.MediaFile == null || request.MediaFile.Length == 0)
                return BadRequest("Invalid media file.");

            var mediaUrl = await _lessonService.UploadMediaAsync(request.MediaFile);
            return Ok(mediaUrl);
        }
    }
}
