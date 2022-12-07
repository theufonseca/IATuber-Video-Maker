using Domain.Dto;
using Domain.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Infra.ExternalServices.Music
{
    public class MusicService : IMusicService
    {
        public Task<FileResponseDto> GenerateMusic(string text)
        {
            return Task.FromResult(new FileResponseDto
            {
                Url = "https://storage.googleapis.com/teste8-182316.appspot.com/Morning_Light_-_Yigit_Atilla.mp3",
                FileName = "Morning_Light_-_Yigit_Atilla.mp3"
            });
        }
    }
}
