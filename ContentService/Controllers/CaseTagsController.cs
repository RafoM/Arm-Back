using ContentService.Models.RequestModels;
using ContentService.Models.ResponseModels;
using ContentService.Services.Interface;
using Microsoft.AspNetCore.Mvc;

namespace ContentService.Controllers
{
    public class CaseTagsController : BaseController
    {
        private readonly ICaseTagService _tagService;

        /// <summary>
        /// Initializes a new instance of the <see cref="CaseTagsController"/> class.
        /// </summary>
        /// <param name="tagService">The service handling Case tag operations.</param>
        public CaseTagsController(ICaseTagService tagService)
        {
            _tagService = tagService;
        }

        /// <summary>
        /// Creates a new CaseTag record based on the provided request.
        /// </summary>
        /// <param name="request">Request model containing Case tag data.</param>
        /// <returns>
        /// The newly created <see cref="CaseTagResponseModel"/> with its assigned ID.
        /// </returns>
        [HttpPost]
        public async Task<ActionResult<CaseTagResponseModel>> CreateTag([FromBody] CaseTagRequestModel request)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var createdTag = await _tagService.CreateAsync(request);
            return CreatedAtAction(nameof(GetTagById), new { tagId = createdTag.TagId }, createdTag);
        }

        /// <summary>
        /// Retrieves all existing tags.
        /// </summary>
        /// <returns>A list of <see cref="CaseTagResponseModel"/> objects.</returns>
        [HttpGet]
        public async Task<ActionResult<IEnumerable<CaseTagResponseModel>>> GetAllTags()
        {
            var tags = await _tagService.GetAllAsync();
            return Ok(tags);
        }

        /// <summary>
        /// Retrieves a specific CaseTag by its unique identifier.
        /// </summary>
        /// <param name="tagId">ID of the tag to be retrieved.</param>
        /// <returns>
        /// The requested <see cref="CaseTagResponseModel"/> if found; otherwise 404 Not Found.
        /// </returns>
        [HttpGet("{tagId}")]
        public async Task<ActionResult<CaseTagResponseModel>> GetTagById(int tagId)
        {
            try
            {
                var tag = await _tagService.GetByIdAsync(tagId);
                return Ok(tag);
            }
            catch (KeyNotFoundException ex)
            {
                return NotFound(ex.Message);
            }
        }

        /// <summary>
        /// Updates an existing CaseTag with new data.
        /// </summary>
        /// <param name="request">The update model containing new tag data and its ID.</param>
        /// <returns>
        /// The updated <see cref="CaseTagResponseModel"/> if successful; otherwise 404 Not Found.
        /// </returns>
        [HttpPut]
        public async Task<ActionResult<CaseTagResponseModel>> UpdateTag([FromBody] CaseTagUpdateModel request)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            try
            {
                var updatedTag = await _tagService.UpdateAsync(request);
                return Ok(updatedTag);
            }
            catch (KeyNotFoundException ex)
            {
                return NotFound(ex.Message);
            }
        }

        /// <summary>
        /// Deletes a specific CaseTag by its unique identifier.
        /// </summary>
        /// <param name="tagId">ID of the tag to be deleted.</param>
        /// <returns>No content if successful, or 404 Not Found if the tag does not exist.</returns>
        [HttpDelete("{tagId}")]
        public async Task<IActionResult> DeleteTag(int tagId)
        {
            try
            {
                await _tagService.DeleteAsync(tagId);
                return NoContent();
            }
            catch (KeyNotFoundException ex)
            {
                return NotFound(ex.Message);
            }
        }
    }
}
