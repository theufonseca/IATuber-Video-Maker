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
        public Task<string> GenerateMusic(string text)
        {
            return Task.FromResult("https://storage.googleapis.com/teste8-182316.appspot.com/Morning_Light_-_Yigit_Atilla.mp3");
        }
    }
}
