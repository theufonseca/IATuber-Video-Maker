using Domain.Dto;
using Domain.Interfaces;
using IBM.Cloud.SDK.Core.Authentication.Iam;
using IBM.Watson.TextToSpeech.v1;
using Infra.ExternalServices.Storage;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Infra.ExternalServices.VoiceGenerator
{
    public class VoiceServiceIBM : IVoiceService
    {
        private readonly ICloudStorage googleCloudStorage;
        private readonly IConfiguration configuration;

        public VoiceServiceIBM(ICloudStorage googleCloudStorage,
            IConfiguration configuration)
        {
            this.googleCloudStorage = googleCloudStorage;
            this.configuration = configuration;
        }

        public async Task<FileResponseDto> GenerateVoice(string text, int videoId)
        {
            var name = $"voice-{videoId}.mp3";

            var authenticator = new IamAuthenticator(apikey: "HTE660W-rMUZfJa_Tn2RqedbcgY9bvoQX3wAzcWyhpVy");
            var textToSpeech = new TextToSpeechService(authenticator);
            textToSpeech.SetServiceUrl("https://api.us-south.text-to-speech.watson.cloud.ibm.com/instances/ccffb898-255c-440a-9c67-869000bbb2f3");
            var result = textToSpeech.Synthesize(text, voice: "pt-BR_IsabelaV3Voice");

            if (result.StatusCode != 0)
                throw new ArgumentException("Error when generating voice");

            var stream = result.Result;
            var urlFile = await googleCloudStorage.UploadFileAsync(name, stream);
            return new FileResponseDto
            {
                Url = urlFile,
                FileName = name
            };
        }
    }
}
