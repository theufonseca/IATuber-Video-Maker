using Domain.Dto;
using Domain.Interfaces;
using Infra.ExternalServices.Storage;
using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;

namespace Infra.ExternalServices.Image
{
    public class ImageService : IImageService
    {
        private readonly IHttpClientFactory httpClientFactory;
        private readonly IConfiguration configuration;
        private readonly ICloudStorage googleCloudStorage;

        public ImageService(IHttpClientFactory httpClientFactory, IConfiguration configuration,
            ICloudStorage googleCloudStorage)
        {
            this.httpClientFactory = httpClientFactory;
            this.configuration = configuration;
            this.googleCloudStorage = googleCloudStorage;
        }

        public async Task<FileResponseDto?> GenerateImage(string keyWord, int videoId, int index)
        {
            keyWord = keyWord.Replace(".", "").Replace("-", "");
            string base64 = string.Empty;
            var client = httpClientFactory.CreateClient();
            var body = GetBody(keyWord);
            var url = configuration.GetSection("StableDiffusion:Url").Value;
            client.Timeout = TimeSpan.FromHours(1);
            var response = await client.PostAsync(url, body);

            if (response.IsSuccessStatusCode)
            {
                var stringContent = await response.Content.ReadAsStringAsync();
                var imageResponse = JsonConvert.DeserializeObject<ImageResponse>(stringContent);
                base64 = imageResponse?.Images?.FirstOrDefault() ?? "";

                if (string.IsNullOrEmpty(base64))
                    throw new ArgumentException($"Image not generated, details: {stringContent}");
            }
            else
            {
                var stringContent = await response.Content.ReadAsStringAsync();
                if (string.IsNullOrEmpty(base64))
                    throw new ArgumentException($"Image not generated, details: {stringContent}");
            }


            var urlResponse = await UploadImage(keyWord, base64, videoId, index);
            return urlResponse;
        }

        private async Task<FileResponseDto?> UploadImage(string keyword, string base64, int videoId, int index)
        {
            var name = $"image-{videoId}-{index}-{keyword}.png";
            byte[] bytes = Convert.FromBase64String(base64);
            using var imageStream = new MemoryStream(bytes);
            var urlResponse = await googleCloudStorage.UploadFileAsync(name, imageStream);

            if (string.IsNullOrEmpty(urlResponse))
                throw new ArgumentException("Not found Url repsonse when uploading file to google");

            return new FileResponseDto
            {
                Url = urlResponse,
                FileName = name
            };
        }

        private HttpContent GetBody(string keyWord)
        {
            var widthSection = configuration.GetSection("StableDiffusion:Width").Value!;
            var heighSection = configuration.GetSection("StableDiffusion:Height").Value!;

            var width = string.IsNullOrEmpty(widthSection) ? 500 : Convert.ToInt32(widthSection);
            var height = string.IsNullOrEmpty(heighSection) ? 500 : Convert.ToInt32(heighSection);

            var body = new
            {
                prompt = keyWord,
                steps = Convert.ToInt32(configuration.GetSection("StableDiffusion:Steps").Value),
                width = width,
                height = height
            };

            var stringContent = new StringContent(JsonConvert.SerializeObject(body));
            stringContent.Headers.ContentType = new MediaTypeHeaderValue("application/json");
            return stringContent;
        }
    }
}
