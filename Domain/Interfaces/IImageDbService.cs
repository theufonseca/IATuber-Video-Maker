using Domain.Aggregates;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Interfaces
{
    public interface IImageDbService
    {
        Task Create(Image image);
        Task<List<Image>> GetByVideoId(int videoId);
    }
}
