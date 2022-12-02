using Domain.Aggregates;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Interfaces
{
    public interface IVideoService
    {
        Task<Video> GetById(int id);
        Task UpdateStatus(int videoId, int newStatus);
        Task UpdateTitle(int videoId, string title);
        Task UpdateText(int videoId, string text);
        Task SetError(int videoId);
    }
}
