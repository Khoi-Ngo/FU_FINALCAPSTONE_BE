using System.Text;
using System.Text.Json;
using AISEA.ApiService.SHARED.DTOs.Requests.CheckPoint;
using AISEA.ApiService.SHARED.Interfaces;
using AISEA.ApiService.SHARED.PropConfigs;
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

        public async Task<List<CommandCheckpointRequest>> GenerateCheckpoints(string userPrompt)
        {
            _httpClient.DefaultRequestHeaders.Authorization =
                new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", _chatBotSettings.ApiKey);

            var request = new
            {
                model = _chatBotSettings.Model,
                messages = new object[]
                {
            new { role = "system", content = "You are a strict API that only outputs JSON matching the schema." },
            new { role = "user", content = userPrompt }
                },
                response_format = new
                {
                    type = "json_schema",
                    json_schema = new
                    {
                        name = "command_checkpoint_list",
                        schema = new
                        {
                            type = "object",
                            properties = new
                            {
                                checkpoints = new
                                {
                                    type = "array",
                                    items = new
                                    {
                                        type = "object",
                                        properties = new
                                        {
                                            Title = new { type = "string" },
                                            Content = new { type = "string" },
                                            Note = new { type = "string" },
                                            Link1 = new { type = "string" },
                                            Link2 = new { type = "string" },
                                            Link3 = new { type = "string" },
                                            Link4 = new { type = "string" },
                                            Link5 = new { type = "string" },
                                            Deadline = new { type = "string", format = "date-time" }
                                        },
                                        required = new[] { "Title", "Content", "Deadline" }
                                    }
                                }
                            },
                            required = new[] { "checkpoints" }
                        }
                    }
                }
            };

            var jsonRequest = System.Text.Json.JsonSerializer.Serialize(request);
            var jsonContent = new StringContent(jsonRequest, Encoding.UTF8, "application/json");

            try
            {
                var response = await _httpClient.PostAsync(_chatBotSettings.ApiUrl, jsonContent);
                response.EnsureSuccessStatusCode();
                var jsonResponse = await response.Content.ReadAsStringAsync();

                using var doc = JsonDocument.Parse(jsonResponse);
                var root = doc.RootElement;

                // Safely navigate JSON structure
                if (root.TryGetProperty("choices", out var choices) &&
                    choices.ValueKind == JsonValueKind.Array && choices.GetArrayLength() > 0 &&
                    choices[0].TryGetProperty("message", out var message) &&
                    message.TryGetProperty("content", out var contentElement))
                {
                    var content = contentElement.GetString();
                    if (string.IsNullOrEmpty(content))
                        return new List<CommandCheckpointRequest>();

                    var parsed = System.Text.Json.JsonSerializer.Deserialize<CheckpointsWrapper>(
                        content,
                        new JsonSerializerOptions { PropertyNameCaseInsensitive = true });

                    return parsed?.Checkpoints ?? new List<CommandCheckpointRequest>();
                }

                return new List<CommandCheckpointRequest>();
            }
            catch (Exception)
            {
                return new List<CommandCheckpointRequest>();
            }
        }
        private class CheckpointsWrapper
        {
            public List<CommandCheckpointRequest> Checkpoints { get; set; }
        }
    }
}