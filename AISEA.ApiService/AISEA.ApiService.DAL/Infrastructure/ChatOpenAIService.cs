using System.Text;
using AISEA.ApiService.SHARED.DTOs.Requests.ChatBot;
using AISEA.ApiService.SHARED.DTOs.Responses.ChatBot;
using AISEA.ApiService.SHARED.Interfaces;
using AISEA.ApiService.SHARED.PropConfigs;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;

namespace AISEA.ApiService.DAL.Infrastructure
{
    public class ChatOpenAIService : IChatOpenAIService
    {
        private readonly ChatBotSettings _chatBotSettings;
        private readonly HttpClient _httpClient;

        public ChatOpenAIService(ChatBotSettings chatBotSettings)
        {
            _chatBotSettings = chatBotSettings;
            _httpClient = new HttpClient();
        }

        public async Task<string> SendMsgAsync(string prompt)
        {
            var _apiKey = _chatBotSettings.ApiKey;
            var _apiUrl = _chatBotSettings.ApiUrl;
            var _model = _chatBotSettings.Model;

            _httpClient.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", _apiKey);

            var requestBody = new
            {
                model = _model,
                messages = new[]
                {
                    new { role = "user", content = prompt }
                }
            };

            var jsonContent = new StringContent(JsonConvert.SerializeObject(requestBody), Encoding.UTF8, "application/json");

            var response = await _httpClient.PostAsync(_apiUrl, jsonContent);

            var resContent = await response.Content.ReadAsStringAsync();
            var data = JsonConvert.DeserializeObject<dynamic>(resContent);
            return data?.choices?[0]?.message?.content?.ToString()?.Trim() ?? "No response received.";

        }
    }
}