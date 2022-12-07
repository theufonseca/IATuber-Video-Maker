﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Interfaces
{
    public interface IImageService
    {
        Task<string> GenerateImage(string keyWord, int videoId, int index);
    }
}
