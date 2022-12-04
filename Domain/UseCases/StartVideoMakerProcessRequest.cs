﻿using Domain.Enum;
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
            if (request.VideoId == 0)
                throw new ArgumentException("VideoId is invalid");
            
            var video = await videoService.GetById(request.VideoId);

            if (video is null)
                throw new ArgumentException("Video not found");
            
            await videoService.UpdateStatus(request.VideoId, VIDEO_STATUS.STARTED);

            //Title
            var phraseTitleModel = configuration.GetSection("Phrases:Title").Value!;
            var phraseTitle = phraseTitleModel.Replace("@theme", video.Theme);

            await videoService.UpdateStatus(request.VideoId, VIDEO_STATUS.CREATING_TITLE);
            var title = await textService.GenerateTitle(phraseTitle);
            title = title.Trim().Replace("\"","");

            if (string.IsNullOrEmpty(title))
                throw new ArgumentException("Invalid title");

            await videoService.UpdateTitle(request.VideoId, title);
            //Title

            //Text
            var phraseTextModel = configuration.GetSection("Phrases:Text").Value!;
            var phraseText = phraseTextModel.Replace("@title", title);

            await videoService.UpdateStatus(request.VideoId, VIDEO_STATUS.CREATING_TEXT);
            var text = await textService.GenerateText(phraseText);
            text = text.Trim().Replace("\"", ""); ;

            if (string.IsNullOrEmpty(text))
                throw new ArgumentException("Invalid text");

            await videoService.UpdateText(request.VideoId, text);
            //Text

            //Voice
            await videoService.UpdateStatus(request.VideoId, VIDEO_STATUS.CREATING_VOICE);
            var voiceFileName = await voiceService.GenerateVoice(text);
            await videoService.UpdateVoiceFile(request.VideoId, voiceFileName);
            //Voice

            //Generate voice file
            //Generate Music file
            //Generate Video file
            //Edit video with voice, music and video

            //Post to Upload queue
            await messageQueue.PostUploadQueue(request.VideoId);
            await videoService.UpdateStatus(request.VideoId, VIDEO_STATUS.READY_TO_UPLOAD);

            return new StartVideoMakerProcessResponse
            {
                Sucess = true
            };
        }
    }
}