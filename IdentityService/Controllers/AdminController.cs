using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace IdentityService.Controllers
{
    [Authorize(Roles = "Admin")]
    public class AdminController : BaseController
    {
        [HttpGet("dashboard")]
        public IActionResult GetDashboard()
        {
            return Ok("Welcome, Admin!");
        }
    }
}
