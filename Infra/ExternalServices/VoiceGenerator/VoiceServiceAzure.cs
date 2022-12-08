using Domain.Dto;
using Domain.Interfaces;
using Microsoft.CognitiveServices.Speech;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Infra.ExternalServices.VoiceGenerator
{
    public class VoiceServiceAzure : IVoiceService
    {
        private readonly SpeechConfig speechConfig;
        private readonly ICloudStorage cloudStorage;

        public VoiceServiceAzure(IConfiguration configuration, ICloudStorage cloudStorage)
        {
            var token = configuration.GetSection("AzureTextToSpeech:Token").Value;
            var region = configuration.GetSection("AzureTextToSpeech:Region").Value;
            speechConfig = SpeechConfig.FromSubscription(token, region);
            speechConfig.SpeechSynthesisVoiceName = configuration.GetSection("AzureTextToSpeech:VoiceName").Value;
            this.cloudStorage = cloudStorage;
        }

        public async Task<FileResponseDto> GenerateVoice(string text, int videoId)
        {
            var name = $"voice-{videoId}.mp3";

            using var speechSynthesizer = new SpeechSynthesizer(speechConfig);
            var result = await speechSynthesizer.SpeakTextAsync(text);

            if (result.Reason != ResultReason.SynthesizingAudioCompleted)
                throw new ArgumentException("Error when generating voice");

            var audioBytes = result.AudioData;
            var audioStream = new MemoryStream(audioBytes);
            var urlFile = await cloudStorage.UploadFileAsync(name, audioStream);

            return new FileResponseDto
            {
                FileName = name,
                Url = urlFile
            };
        }
    }
}
