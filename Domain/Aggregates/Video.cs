using Domain.Enum;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Aggregates
{
    public class Video
    {
        public int Id { get; set; }
        public string Theme { get; set; }
        public VIDEO_STATUS Status { get; set; }
        public DateTime CreateDate { get; set; }
        public bool? Success { get; set; }
        public DateTime? LastUpdate { get; set; }
        public DateTime? UploadDate { get; set; }
        public string? Title { get; set; }
        public string? Text { get; set; }
        public string? Keywords { get; set; }
        public string? VoiceFileName { get; set; }
        public string? MusicFileName { get; set; }
        public string? ImageFileName { get; set; }
        public string? EditedVideoFileName { get; set; }
        public string? ErrorDetail { get; set; }
    }
}
