using Domain.Interfaces;
using Infra.ExternalServices.TextService.TextGenerator;
using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;
using OpenAI_API;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;

namespace Infra.ExternalServices.TextGenerator
{
    public class TextService : ITextService
    {
        private readonly IHttpClientFactory clientFactory;
        private readonly string Token;
        private readonly string CompletionsUri;

        public TextService(IHttpClientFactory clientFactory,
            IConfiguration configuration)
        {
            this.clientFactory = clientFactory;
            Token = configuration.GetSection("OpenAI:Token").Value!;
            CompletionsUri = $"{configuration.GetSection("OpenAI:Url").Value!}{"completions"}";
        }

        public async Task<string> GenerateText(string phrase)
            => await CallOpenApiCompletions(BuildBodyText(phrase));
        
        public async Task<string> GenerateTitle(string phrase)
            => await CallOpenApiCompletions(BuildBodyTitle(phrase));

        public async Task<string> GenerateKeyWords(string phrase)
            => await CallOpenApiCompletions(BuildBodyKeywords(phrase));

        private async Task<string> CallOpenApiCompletions(HttpContent body)
        {
            string result = string.Empty;
            var client = clientFactory.CreateClient();
            client.DefaultRequestHeaders.Add("Authorization", $"Bearer {Token}");
            var response = await client.PostAsync(CompletionsUri, body);

            if (response.IsSuccessStatusCode)
            {
                var objectResponse = await response.Content.ReadAsStringAsync()!;
                var completions = JsonConvert.DeserializeObject<CompletionsResponse>(objectResponse);
                result = completions?.Choices?.FirstOrDefault()?.Text ?? "";
            }

            return result;
        }

        private HttpContent BuildBodyKeywords(string phrase)
        {
            var body = new
            {
                model = "text-davinci-003",
                prompt = phrase,
                temperature = 0.7,
                max_tokens = 3200,
                top_p = 1,
                frequency_penalty = 0,
                presence_penalty = 0
            };

            var content = new StringContent(JsonConvert.SerializeObject(body));
            content.Headers.ContentType = new MediaTypeHeaderValue("application/json");
            return content;
        }

        private HttpContent BuildBodyText(string phrase)
        {
            var body = new
            {
                model = "text-davinci-003",
                prompt = phrase,
                temperature = 0.7,
                max_tokens = 4000,
                top_p = 1,
                frequency_penalty = 0,
                presence_penalty = 0
            };

            var content = new StringContent(JsonConvert.SerializeObject(body));
            content.Headers.ContentType = new MediaTypeHeaderValue("application/json");
            return content;
        }
        private HttpContent BuildBodyTitle(string phrase)
        {
            var body = new
            {
                model = "text-davinci-003",
                prompt = phrase,
                temperature = 0.7,
                max_tokens = 256,
                top_p = 1,
                frequency_penalty = 0,
                presence_penalty = 0
            };

            var content = new StringContent(JsonConvert.SerializeObject(body));
            content.Headers.ContentType = new MediaTypeHeaderValue("application/json");
            return content;
        }
    }
}
