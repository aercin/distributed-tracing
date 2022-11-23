using Microsoft.AspNetCore.Mvc;

namespace MessageRelayService.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class IndexController : ControllerBase
    {
        [HttpGet("/welcome")]
        public IActionResult Index()
        {
            return Ok("Message relay service is ready..");
        }
    }
}
