using Microsoft.AspNetCore.Mvc;

namespace api.Controllers
{
    [Route("api")]
    [ApiController]
    public class WebHookController : ControllerBase
    {
        [HttpPost("notification")]
        public IActionResult HandleNotification(NotificationRequest request)
        {
            Console.WriteLine(request.message);

            return Ok();
        } 

        public class NotificationRequest
        {
            public string message { get; set; }
        }
    }
}
