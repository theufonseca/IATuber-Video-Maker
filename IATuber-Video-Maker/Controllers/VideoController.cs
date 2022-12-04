using Domain.UseCases;
using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace IATuber_Video_Maker.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class VideoController : ControllerBase
    {
        private readonly IMediator mediator;

        public VideoController(IMediator mediator)
        {
            this.mediator = mediator;
        }

        [HttpPost("{id}")]
        public async Task<IActionResult> Post(int id)
        {
            var request = new StartVideoMakerProcessRequest { VideoId = id };
            var response = await mediator.Send(request);
            return Ok(response);
        }
    }
}
