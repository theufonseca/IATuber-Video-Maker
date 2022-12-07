using Domain.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Infra.ExternalServices.VideoEditor
{
    public class VideoEditor : IVideoEditor
    {
        public Task Edit(string webRootPath)
        {
            var fileName = $"{webRootPath}\\videos\\{Guid.NewGuid()}.wmv";
            using var fs = new FileStream(fileName, FileMode.Create);

            return Task.CompletedTask;
        }
    }
}
