using Domain.Aggregates;
using Domain.Enum;
using Domain.Helpers;
using Domain.Interfaces;
using MediatR;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.UseCases
{
    public class StartVideoMakerProcessRequest : IRequest<StartVideoMakerProcessResponse>
    {
        public int VideoId { get; set; }
    }

    public class StartVideoMakerProcessResponse
    {
        public bool Sucess { get; set; }
    }

    public class StartVideoMakerProcessRequestHandler : IRequestHandler<StartVideoMakerProcessRequest, StartVideoMakerProcessResponse>
    {
        private readonly IVideoService videoService;
        private readonly ITextService textService;
        private readonly IMessageQueue messageQueue;
        private readonly IConfiguration configuration;
        private readonly IVoiceService voiceService;
        private readonly ITranslateService translateService;
        private readonly IImageService imageService;
        private readonly IImageDbService imageDbService;
        private readonly IMusicService musicService;
        private readonly ICloudStorage cloudStorage;

        public StartVideoMakerProcessRequestHandler(IVideoService videoService,
            ITextService textService, IMessageQueue messageQueue, IConfiguration configuration,
            IVoiceService voiceService, ITranslateService translateService, IImageService imageService,
            IImageDbService imageDbService, IMusicService musicService, ICloudStorage cloudStorage)
        {
            this.videoService = videoService;
            this.textService = textService;
            this.messageQueue = messageQueue;
            this.configuration = configuration;
            this.voiceService = voiceService;
            this.translateService = translateService;
            this.imageService = imageService;
            this.imageDbService = imageDbService;
            this.musicService = musicService;
            this.cloudStorage = cloudStorage;
        }

        public async Task<StartVideoMakerProcessResponse> Handle(StartVideoMakerProcessRequest request, CancellationToken cancellationToken)
        {
            var video = await GetVideo(request);
            try
            {
                var title = await CreateTitle(video);
                var text = await CreateText(video, title);
                var keywords = await CreateKeyWords(video, text);
                var translatedKeyWords = await TranslateKeyWords(video, keywords);
                await CreateImages(video, translatedKeyWords);
                await CreateVoice(video, text);
                await CreateMusic(video, text);
                await DownloadFiles(video.Id);
                await videoService.UpdateStatus(request.VideoId, VIDEO_STATUS.READY_TO_EDIT);

                return new StartVideoMakerProcessResponse
                {
                    Sucess = true
                };
            }
            catch (Exception ex)
            {
                await videoService.SetError(video.Id, $"{ex.Message}###{ex?.InnerException?.Message ?? ""}");
                return new StartVideoMakerProcessResponse
                {
                    Sucess = false
                };
            }
        }

        private async Task DownloadFiles(int id)
        {
            await videoService.UpdateStatus(id, VIDEO_STATUS.DOWNLOADING_FILES);

            var video = await videoService.GetById(id);
            var pathToDownload = configuration.GetSection("DownloadPath").Value!;
            pathToDownload = $"{pathToDownload.Replace("@id", id.ToString())}-{video.Theme.Replace(" ", "")}";

            if (string.IsNullOrEmpty(video?.VoiceFileName))
                throw new ArgumentException("Voice file does not exists");

            await cloudStorage.DownloadFileAsync(video.VoiceFileName, pathToDownload);

            if (string.IsNullOrEmpty(video?.MusicFileName))
                throw new ArgumentException("Music file does not exists");

            await cloudStorage.DownloadFileAsync(video.MusicFileName, pathToDownload);

            foreach (var item in video.Images)
            {
                if (string.IsNullOrEmpty(item.ImageFileName))
                    throw new ArgumentException("Voice file does not exists");
                await cloudStorage.DownloadFileAsync(item.ImageFileName, $"{pathToDownload}\\Images");
            }
        }

        private async Task CreateMusic(Video video, string text)
        {
            await videoService.UpdateStatus(video.Id, VIDEO_STATUS.CREATING_MUSIC);
            var fileResponse = await musicService.GenerateMusic(text);
            await videoService.UpdateMusic(video.Id, fileResponse.FileName, fileResponse.Url);
        }

        private async Task CreateImages(Video video, List<string> keyWords)
        {
            await videoService.UpdateStatus(video.Id, VIDEO_STATUS.CREATING_IMAGES);
            var keyPhrasesWithAuthor = SetArtist(keyWords);

            int i = 0;
            foreach (var item in keyPhrasesWithAuthor)
            {
                i += 1;
                var fileResponse = await imageService.GenerateImage(item, video.Id, i);

                if (fileResponse is null)
                    continue;

                var newImage = Image.New(video.Id, fileResponse.FileName, fileResponse.Url);
                await imageDbService.Create(newImage);
            }
        }

        private List<string> SetArtist(List<string> keyPhrases)
        {
            var keyPhrasesWithAuthor = new List<string>();
            foreach (var item in keyPhrases)
            {
                var phraseWithAuthor = $"{item} made by {Author.GetRandomAuthor()}";
                keyPhrasesWithAuthor.Add(phraseWithAuthor);
            }

            return keyPhrasesWithAuthor;
        }

        private async Task<List<string>> TranslateKeyWords(Video video, List<string> keywords)
        {
            await videoService.UpdateStatus(video.Id, VIDEO_STATUS.TRANSLATING_KEYWORDS);
            List<string> translatedKeywords = new();

            foreach (var item in keywords)
            {
                var translatedKeyword = await translateService.GetTranslate(item);
                if (!string.IsNullOrEmpty(translatedKeyword))
                    translatedKeywords.Add(translatedKeyword);
            }

            return translatedKeywords;
        }

        private async Task CreateVoice(Video video, string text)
        {
            await videoService.UpdateStatus(video.Id, VIDEO_STATUS.CREATING_VOICE);
            var fileResponse = await voiceService.GenerateVoice(text, video.Id);
            await videoService.UpdateVoiceFile(video.Id, fileResponse.FileName, fileResponse.Url);
        }

        private async Task<List<string>> CreateKeyWords(Video video, string text)
        {
            var maxKeywords = Convert.ToInt32(configuration.GetSection("MaxKeyWords").Value);
            var phraseKeyWordsModel = configuration.GetSection("Phrases:Keys").Value!;
            var keyPhrases = new List<string>();

            int i = 1;
            List<string> phrases = new();
            while (text.Contains($"{i}."))
            {
                string phrase = text.Substring(text.IndexOf($"{i}.") + 2);
                phrase = phrase.Substring(0, phrase.IndexOf(".")).Trim();
                phrase = phraseKeyWordsModel.Replace("@text", phrase);
                phrases.Add(phrase);
                i++;
            }

            await videoService.UpdateStatus(video.Id, VIDEO_STATUS.CREATING_KEYWORDS);

            foreach (var item in phrases)
            {
                var response = await textService.GenerateKeyWords(item);
                var responseClean = Helper.CleanKeywordResponse(response);

                if (string.IsNullOrEmpty(responseClean))
                    throw new ArgumentException("Invalid keyword response");

                var splitedKeywords = Helper.SplitKeyword(response);
                var configuredKeywords = Helper.ConfigureKeywords(splitedKeywords);

                var keyPhrase = $"{video.Theme} {string.Join(",", configuredKeywords)}";
                keyPhrases.Add(keyPhrase);
            }
            
            await videoService.UpdateKeywords(video.Id, string.Join("|", keyPhrases));
            return keyPhrases;

        }

        private async Task<string> CreateText(Video video, string title)
        {
            var phraseTextModel = configuration.GetSection("Phrases:Text").Value!;
            var phraseText = phraseTextModel.Replace("@title", title);

            await videoService.UpdateStatus(video.Id, VIDEO_STATUS.CREATING_TEXT);
            var text = await textService.GenerateText(phraseText);
            text = text.Trim().Replace("\"", "");

            if (string.IsNullOrEmpty(text))
                throw new ArgumentException("Invalid text");

            await videoService.UpdateText(video.Id, text);

            return text;

        }

        private async Task<string> CreateTitle(Video video)
        {
            var phraseTitleModel = configuration.GetSection("Phrases:Title").Value!;
            var phraseTitle = phraseTitleModel.Replace("@theme", video.Theme);

            await videoService.UpdateStatus(video.Id, VIDEO_STATUS.CREATING_TITLE);
            var title = await textService.GenerateTitle(phraseTitle);
            title = title.Trim().Replace("\"", "");

            if (string.IsNullOrEmpty(title))
                throw new ArgumentException("Invalid title");

            await videoService.UpdateTitle(video.Id, title);
            return title;

        }

        private async Task<Video> GetVideo(StartVideoMakerProcessRequest request)
        {
            if (request.VideoId == 0)
                throw new ArgumentException("VideoId is invalid");

            var video = await videoService.GetById(request.VideoId);

            if (video is null)
                throw new ArgumentException("Video not found");

            await videoService.UpdateStatus(request.VideoId, VIDEO_STATUS.STARTED);

            return video;
        }
    }
}
