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
        private readonly ITranslateService translateService;

        public TesteController(IVoiceService voiceService, ITranslateService translateService)
        {
            this.voiceService = voiceService;
            this.translateService = translateService;
        }

        [HttpGet("GenerateVoice")]
        public async Task<IActionResult> Get()
        {
            await voiceService.GenerateVoice("Olá mundo");
            return Ok();
        }

        [HttpGet("Translate")]
        public async Task<IActionResult> Get2()
        {
            await translateService.GetTranslate("Olá mundo!");
            return Ok();
        }
    }
}
