using Microsoft.AspNetCore.Mvc;
using System.Text.Json;

namespace notification.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class NotificationController : ControllerBase
    {
        [HttpPost]
        public IActionResult SendNotification(NotificationRequest request)
        {
            Console.WriteLine(JsonSerializer.Serialize(request));
            return Ok(new NotificiationResponse { IsSuccess = true });
        }

        public class NotificationRequest
        {
            public string From { get; set; }
            public string To { get; set; }
            public string Message { get; set; }
        }

        public class NotificiationResponse
        {
            public bool IsSuccess { get; set; }
        }
    }
}
