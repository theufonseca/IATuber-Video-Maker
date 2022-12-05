﻿using Domain.Aggregates;
using Domain.Enum;
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

        public StartVideoMakerProcessRequestHandler(IVideoService videoService,
            ITextService textService, IMessageQueue messageQueue, IConfiguration configuration,
            IVoiceService voiceService)
        {
            this.videoService = videoService;
            this.textService = textService;
            this.messageQueue = messageQueue;
            this.configuration = configuration;
            this.voiceService = voiceService;
        }

        public async Task<StartVideoMakerProcessResponse> Handle(StartVideoMakerProcessRequest request, CancellationToken cancellationToken)
        {
            var video = await GetVideo(request);
            var title = await CreateTitle(video);
            var text = await CreateText(video.Id, title);
            var keywords = await CreateKeyWords(video.Id, text);
            await CreateVoice(video.Id, text);

            //Get Key words
            //Translate to english the KeyWords
            //Generate Images to each keyword

            //Generate Music file
            //Edit video with voice, music and video

            //Post to Upload queue
            await messageQueue.PostUploadQueue(request.VideoId);
            await videoService.UpdateStatus(request.VideoId, VIDEO_STATUS.READY_TO_UPLOAD);

            return new StartVideoMakerProcessResponse
            {
                Sucess = true
            };
        }

        private async Task CreateVoice(int videoId, string text)
        {
            await videoService.UpdateStatus(videoId, VIDEO_STATUS.CREATING_VOICE);
            var voiceFileName = await voiceService.GenerateVoice(text);
            await videoService.UpdateVoiceFile(videoId, voiceFileName);
        }

        private async Task<List<string>> CreateKeyWords(int videoId, string text)
        {
            var phraseKeyWordsModel = configuration.GetSection("Phrases:Keys").Value!;
            var phraseKeyWords = phraseKeyWordsModel.Replace("@text", text);

            await videoService.UpdateStatus(videoId, VIDEO_STATUS.CREATING_KEYWORDS);
            var keyWordsPlainText = await textService.GenerateKeyWords(phraseKeyWords);
            keyWordsPlainText = keyWordsPlainText.Trim()
                .Replace("\"", "")
                .Replace("Palavras-chave", "")
                .Replace(":", "");

            if (string.IsNullOrEmpty(keyWordsPlainText))
                throw new ArgumentException("Invalid text");

            List<string> keywords = new();
            List<string> formatedKeys = new();

            if (keyWordsPlainText.Contains("-"))
            {
                keywords = keyWordsPlainText.Split("-").ToList();
                
                foreach (var item in keywords)
                {
                    var formatedItem = item.Trim();
                    if (!string.IsNullOrEmpty(formatedItem))
                        formatedKeys.Add(formatedItem);
                }
            } 
            else if (keyWordsPlainText.Contains(","))
            {
                keywords = keyWordsPlainText.Split(",").ToList();
                foreach (var item in keywords)
                {
                    var formatedItem = item.Trim();
                    if (!string.IsNullOrEmpty(formatedItem))
                        formatedKeys.Add(formatedItem);
                }
            }
            else if (keyWordsPlainText.Contains("1."))
            {
                keywords = keyWordsPlainText.Split(".").ToList();
                foreach (var item in keywords)
                {
                    var formatedItem = item.Trim()
                        .Replace("1", "")
                        .Replace("2", "")
                        .Replace("3", "")
                        .Replace("4", "")
                        .Replace("5", "");

                    if (!string.IsNullOrEmpty(formatedItem))
                        formatedKeys.Add(formatedItem);
                }
            }

            await videoService.UpdateKeywords(videoId, string.Join(",", keywords));

            return formatedKeys;
        }

        private async Task<string> CreateText(int videoId, string title)
        {
            var phraseTextModel = configuration.GetSection("Phrases:Text").Value!;
            var phraseText = phraseTextModel.Replace("@title", title);

            await videoService.UpdateStatus(videoId, VIDEO_STATUS.CREATING_TEXT);
            var text = await textService.GenerateText(phraseText);
            text = text.Trim().Replace("\"", "");

            if (string.IsNullOrEmpty(text))
                throw new ArgumentException("Invalid text");

            await videoService.UpdateText(videoId, text);

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
