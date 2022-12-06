using Domain.Interfaces;
using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;

namespace Infra.ExternalServices.Translate
{
    public class TranslateService : ITranslateService
    {
        private readonly IHttpClientFactory httpClientFactory;
        private readonly IConfiguration configuration;

        public TranslateService(IHttpClientFactory httpClientFactory, IConfiguration configuration)
        {
            this.httpClientFactory = httpClientFactory;
            this.configuration = configuration;
        }

        public async Task<string> GetTranslate(string text)
        {
            string result = string.Empty;
            var client = httpClientFactory.CreateClient();
            client.DefaultRequestHeaders.Add("Authorization", $"Basic {configuration.GetSection("IBMTranslate:Token").Value}");

            var body = GetBody(text);
            var response = await client.PostAsync(configuration.GetSection("IBMTranslate:Url").Value, body);

            if (response.IsSuccessStatusCode)
            {
                var objectResponse = await response.Content.ReadAsStringAsync()!;
                var translateResponse = JsonConvert.DeserializeObject<TranslateResponse>(objectResponse);
                result = translateResponse?.translations?.FirstOrDefault()?.translation ?? "";
            }

            return result;
        }

        private HttpContent GetBody(string text)
        {
            var body = new
            {
                text = new List<string> { text },
                model_id = configuration.GetSection("IBMTranslate:model_id").Value
            };

            var encodedBody = new StringContent(JsonConvert.SerializeObject(body));
            encodedBody.Headers.ContentType = new MediaTypeHeaderValue("application/json");
            return encodedBody;
        }

    }
}
