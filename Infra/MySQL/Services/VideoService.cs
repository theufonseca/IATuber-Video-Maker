using Domain.Aggregates;
using Domain.Enum;
using Domain.Interfaces;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Infra.MySQL.Services
{
    public class VideoService : IVideoService
    {
        private readonly IServiceScopeFactory serviceScopeFactory;

        public VideoService(IServiceScopeFactory serviceScopeFactory)
        {
            this.serviceScopeFactory = serviceScopeFactory;
        }

        public async Task<Video?> GetById(int id)
        {
            using var dataContext = serviceScopeFactory.CreateScope()
                .ServiceProvider.GetRequiredService<DataContext>();

            return await dataContext.Video.FirstOrDefaultAsync(x => x.Id == id) ?? null;
        }

        public async Task SetError(int videoId, string errorDetail)
        {
            using var dataContext = serviceScopeFactory.CreateScope()
                .ServiceProvider.GetRequiredService<DataContext>();

            var video = await dataContext.Video.FirstOrDefaultAsync(x => x.Id == videoId);
            if (video == null)
                return;
            
            video.ErrorDetail = errorDetail.Trim();
            dataContext.Video.Update(video);

            await dataContext.SaveChangesAsync();
        }

        public async Task UpdateKeywords(int videoId, string keywords)
        {
            using var dataContext = serviceScopeFactory.CreateScope()
                .ServiceProvider.GetRequiredService<DataContext>();

            var video = await dataContext.Video.FirstOrDefaultAsync(x => x.Id == videoId);
            if (video == null)
                throw new ArgumentException("Video not found");

            video.Keywords = keywords;
            dataContext.Video.Update(video);

            await dataContext.SaveChangesAsync();
        }

        public async Task UpdateStatus(int videoId, VIDEO_STATUS newStatus)
        {
            using var dataContext = serviceScopeFactory.CreateScope()
                .ServiceProvider.GetRequiredService<DataContext>();

            var video = await dataContext.Video.FirstOrDefaultAsync(x => x.Id == videoId);
            if (video == null)
                throw new ArgumentException("Video not found");

            video.Status = newStatus;
            dataContext.Video.Update(video);

            await dataContext.SaveChangesAsync();
        }

        public async Task UpdateText(int videoId, string text)
        {
            using var dataContext = serviceScopeFactory.CreateScope()
                .ServiceProvider.GetRequiredService<DataContext>();

            var video = await dataContext.Video.FirstOrDefaultAsync(x => x.Id == videoId);
            if (video == null)
                throw new ArgumentException("Video not found");

            video.Text = text;
            dataContext.Video.Update(video);

            await dataContext.SaveChangesAsync();
        }

        public async Task UpdateTitle(int videoId, string title)
        {
            using var dataContext = serviceScopeFactory.CreateScope()
                .ServiceProvider.GetRequiredService<DataContext>();

            var video = await dataContext.Video.FirstOrDefaultAsync(x => x.Id == videoId);
            if (video == null)
                throw new ArgumentException("Video not found");

            video.Title = title;
            dataContext.Video.Update(video);

            await dataContext.SaveChangesAsync();
        }

        public async Task UpdateVoiceFile(int videoId, string fileName)
        {
            using var dataContext = serviceScopeFactory.CreateScope()
                .ServiceProvider.GetRequiredService<DataContext>();

            var video = await dataContext.Video.FirstOrDefaultAsync(x => x.Id == videoId);
            if (video == null)
                throw new ArgumentException("Video not found");

            video.VoiceFileName = fileName;
            dataContext.Video.Update(video);

            await dataContext.SaveChangesAsync();
        }
    }
}
