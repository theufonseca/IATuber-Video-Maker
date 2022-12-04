using Domain.Aggregates;
using Domain.Enum;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Interfaces
{
    public interface IVideoService
    {
        Task<Video?> GetById(int id);
        Task UpdateStatus(int videoId, VIDEO_STATUS newStatus);
        Task UpdateTitle(int videoId, string title);
        Task UpdateText(int videoId, string text);
        Task UpdateVoiceFile(int videoId, string fileName);
        Task SetError(int videoId, string errorDetail);
    }
}
