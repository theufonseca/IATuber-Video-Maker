﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Aggregates
{
    public class Image
    {
        public int Id { get; set; }
        public int VideoId { get; set; }
        public string ImageFileName { get; set; }
        public DateTime CreateDate { get; set; }

        public Video Video { get; set; }

        public static Image New(int videoId, string imageFileName)
        {
            return new Image
            {
                VideoId = videoId,
                ImageFileName = imageFileName,
                CreateDate = DateTime.Now
            };
        }
    }
}
