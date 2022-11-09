using Microsoft.AspNetCore.Mvc;

namespace api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class IndexController : ControllerBase
    {
        [HttpGet("/welcome")]
        public IActionResult Index()
        {
            return Ok("Payment service is ready..");
        }
    }
}
