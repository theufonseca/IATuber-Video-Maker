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
        private readonly IVideoEditor videoEditor;
        private readonly IWebHostEnvironment environment;

        public TesteController(IVoiceService voiceService, ITranslateService translateService,
            IVideoEditor videoEditor, IWebHostEnvironment environment)
        {
            this.voiceService = voiceService;
            this.translateService = translateService;
            this.videoEditor = videoEditor;
            this.environment = environment;
        }

        [HttpGet("GenerateVoice")]
        public async Task<IActionResult> Get()
        {
            await voiceService.GenerateVoice("Olá mundo", 1);
            return Ok();
        }

        [HttpGet("Translate")]
        public async Task<IActionResult> Get2()
        {
            await translateService.GetTranslate("Olá mundo!");
            return Ok();
        }

        [HttpGet("Editor")]
        public async Task<IActionResult> Get3()
        {
            await videoEditor.Edit(environment.WebRootPath);
            return Ok();
        }
    }
}
