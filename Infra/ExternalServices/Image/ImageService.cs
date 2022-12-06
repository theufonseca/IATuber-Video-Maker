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
        private readonly GoogleCloudStorage googleCloudStorage;

        public ImageService(IHttpClientFactory httpClientFactory, IConfiguration configuration,
            GoogleCloudStorage googleCloudStorage)
        {
            this.httpClientFactory = httpClientFactory;
            this.configuration = configuration;
            this.googleCloudStorage = googleCloudStorage;
        }

        public async Task<string> GenerateImage(string keyWord)
        {
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
            }

            if (string.IsNullOrEmpty(base64))
                return string.Empty;

            var urlResponse = await UploadImage(base64);
            return urlResponse;
        }

        private async Task<string> UploadImage(string base64)
        {
            var name = Guid.NewGuid().ToString().Replace("-","");
            byte[] bytes = Convert.FromBase64String(base64);
            using var imageStream = new MemoryStream(bytes);
            var urlResponse = await googleCloudStorage.UploadFileAsync($"{name}.png", imageStream);

            if (string.IsNullOrEmpty(urlResponse))
                throw new ArgumentException("Not found Url repsonse when uploading file to google");

            return urlResponse;
        }

        private HttpContent GetBody(string keyWord)
        {
            var body = new
            {
                prompt = keyWord,
                steps = Convert.ToInt32(configuration.GetSection("StableDiffusion:Steps").Value)
            };

            var stringContent = new StringContent(JsonConvert.SerializeObject(body));
            stringContent.Headers.ContentType = new MediaTypeHeaderValue("application/json");
            return stringContent;
        }
    }
}
