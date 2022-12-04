using Domain.Interfaces;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace IATuber_Video_Maker.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class TesteController : ControllerBase
    {
        private readonly IVoiceService voiceService;

        public TesteController(IVoiceService voiceService)
        {
            this.voiceService = voiceService;
        }

        [HttpGet]
        public async Task<IActionResult> Get()
        {
            await voiceService.GenerateVoice("Olá mundo");
            return Ok();
        }
    }
}
