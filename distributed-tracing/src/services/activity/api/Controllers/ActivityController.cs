using application;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;

namespace api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ActivityController : ControllerBase
    {
        private readonly IMediator _mediator;
        public ActivityController(IMediator mediator)
        {
            this._mediator = mediator;
        }

        [Route("GetByDesc")]
        [HttpGet]
        public async Task<IActionResult> GetActivityByDescription([FromQuery] GetActivity.Command request)
        {
            return Ok(await this._mediator.Send(request));
        }

        [Route("Create")]
        [HttpPost]
        public async Task<IActionResult> CreateNewActivity(CreateActivity.Command request)
        {
            return Ok(await this._mediator.Send(request));
        }

        [Route("AddAttendant")]
        [HttpPost]
        public async Task<IActionResult> AddAttendantToActivity(AddAttendant.Command request)
        {  
            return Ok(await this._mediator.Send(request));
        }
    }
}
