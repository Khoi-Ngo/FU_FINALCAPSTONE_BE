using System.Text;
using System.Text.Json;
using AISEA.ApiService.SHARED.DTOs.Requests.CheckPoint;
using AISEA.ApiService.SHARED.DTOs.Responses.SubjectComment;
using AISEA.ApiService.SHARED.DTOs.Roadmap;
using AISEA.ApiService.SHARED.Interfaces;
using AISEA.ApiService.SHARED.PropConfigs;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;

namespace AISEA.ApiService.DAL.Infrastructure;

public class ChatOpenAIService : IChatOpenAIService
{
    #region Init class
    private readonly ChatBotSettings _chatBotSettings;
    private readonly HttpClient _httpClient;
    private readonly ILogger<ChatOpenAIService> _logger;

    public ChatOpenAIService(ChatBotSettings chatBotSettings, ILogger<ChatOpenAIService> logger)
    {
        _chatBotSettings = chatBotSettings;
        _httpClient = new HttpClient();
        _logger = logger;
    }
    #endregion

    #region Checkpoints generation
    public async Task<List<CommandCheckpointRequest>> GenerateCheckpoints(string userPrompt)
    {

        _logger.LogInformation("==== GenerateCheckpoints : OpenAI Request Prompt ====\n{Prompt}", userPrompt);

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

    #endregion

    public async Task<CommentVerificationResult> VerifyCommentAsync(string content)
    {
        _httpClient.DefaultRequestHeaders.Authorization =
            new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", _chatBotSettings.ApiKey);

        var request = new
        {
            model = _chatBotSettings.Model,
            messages = new object[]
            {
        new { role = "system", content = "You are a strict API that only outputs JSON matching the schema." },
        new { role = "user", content = $"Check if this comment violates community rules (spam, rude, offensive, etc.). Comment: \"{content}\"" }
            },
            response_format = new
            {
                type = "json_schema",
                json_schema = new
                {
                    name = "comment_verification",
                    schema = new
                    {
                        type = "object",
                        properties = new
                        {
                            IsBad = new { type = "boolean" },
                            Reason = new { type = "string" }
                        },
                        required = new[] { "IsBad", "Reason" }
                    }
                }
            }
        };

        try
        {
            var response = await _httpClient.PostAsync(
                _chatBotSettings.ApiUrl,
                new StringContent(System.Text.Json.JsonSerializer.Serialize(request), Encoding.UTF8, "application/json"));

            response.EnsureSuccessStatusCode();
            var jsonResponse = await response.Content.ReadAsStringAsync();

            using var doc = JsonDocument.Parse(jsonResponse);
            if (doc.RootElement.GetProperty("choices")[0]
                  .GetProperty("message")
                  .TryGetProperty("content", out var contentElement))
            {
                var contentStr = contentElement.GetString();
                if (!string.IsNullOrWhiteSpace(contentStr))
                {
                    try
                    {
                        return System.Text.Json.JsonSerializer.Deserialize<CommentVerificationResult>(
                            contentStr,
                            new JsonSerializerOptions { PropertyNameCaseInsensitive = true })
                            ?? new CommentVerificationResult { IsBad = false, Reason = "Null parse" };
                    }
                    catch
                    {
                        return new CommentVerificationResult { IsBad = false, Reason = "Invalid JSON" };
                    }
                }
            }
        }
        catch
        {
            // swallow and return safe default
        }

        return new CommentVerificationResult { IsBad = false, Reason = "Error or empty response" };
    }

    #region ROADMAP

    public async Task<string> GetSuggestedComboForStudent(string userPrompt)
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
                    name = "suggested_combo",
                    schema = new
                    {
                        type = "object",
                        properties = new
                        {
                            combo = new { type = "string" }
                        },
                        required = new[] { "combo" },
                        additionalProperties = false
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

            if (root.TryGetProperty("choices", out var choices) &&
                choices.ValueKind == JsonValueKind.Array && choices.GetArrayLength() > 0 &&
                choices[0].TryGetProperty("message", out var message) &&
                message.TryGetProperty("content", out var contentElement))
            {
                var content = contentElement.GetString();
                if (string.IsNullOrEmpty(content))
                    return string.Empty;

                var parsed = System.Text.Json.JsonSerializer.Deserialize<ComboWrapper>(
                    content,
                    new JsonSerializerOptions { PropertyNameCaseInsensitive = true });

                return parsed?.Combo ?? string.Empty;
            }

            return string.Empty;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error in GetSuggestedComboForStudent");
            return string.Empty;
        }
    }

    private class ComboWrapper
    {
        public string Combo { get; set; }
    }

    public async Task<List<CreateNodeDto>> GenExternalSubjectNodesInStudyRoadmap(string userPrompt)
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
                    name = "external_nodes",
                    schema = new
                    {
                        type = "object",
                        properties = new
                        {
                            nodes = new
                            {
                                type = "array",
                                items = new
                                {
                                    type = "object",
                                    properties = new
                                    {
                                        SubjectCode = new { type = "string" },
                                        SemesterNumber = new { type = "integer" },
                                        SubjectName = new { type = "string" },
                                        Description = new { type = "string" },
                                        IsInternalSubjectData = new { type = "boolean" }
                                    },
                                    required = new[] { "SubjectCode", "SemesterNumber", "SubjectName", "Description", "IsInternalSubjectData" },
                                    additionalProperties = false
                                }
                            }
                        },
                        required = new[] { "nodes" },
                        additionalProperties = false
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

            if (root.TryGetProperty("choices", out var choices) &&
                choices.ValueKind == JsonValueKind.Array && choices.GetArrayLength() > 0 &&
                choices[0].TryGetProperty("message", out var message) &&
                message.TryGetProperty("content", out var contentElement))
            {
                var content = contentElement.GetString();
                if (string.IsNullOrEmpty(content))
                    return new List<CreateNodeDto>();

                var parsed = System.Text.Json.JsonSerializer.Deserialize<NodesWrapper>(
                    content,
                    new JsonSerializerOptions { PropertyNameCaseInsensitive = true });

                return parsed?.Nodes ?? new List<CreateNodeDto>();
            }

            return new List<CreateNodeDto>();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error in GenExternalSubjectNodesInStudyRoadmap");
            return new List<CreateNodeDto>();
        }
    }

    private class NodesWrapper
    {
        public List<CreateNodeDto> Nodes { get; set; }
    }

    public async Task<List<RoadmapLinkDto>> GetTheCompleteLinkedNodes(string userPrompt)
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
                    name = "roadmap_links",
                    schema = new
                    {
                        type = "object",
                        properties = new
                        {
                            links = new
                            {
                                type = "array",
                                items = new
                                {
                                    type = "object",
                                    properties = new
                                    {
                                        FromNodeId = new { type = "integer" },
                                        ToNodeId = new { type = "integer" }
                                    },
                                    required = new[] { "FromNodeId", "ToNodeId" },
                                    additionalProperties = false
                                }
                            }
                        },
                        required = new[] { "links" },
                        additionalProperties = false
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

            if (root.TryGetProperty("choices", out var choices) &&
                choices.ValueKind == JsonValueKind.Array && choices.GetArrayLength() > 0 &&
                choices[0].TryGetProperty("message", out var message) &&
                message.TryGetProperty("content", out var contentElement))
            {
                var content = contentElement.GetString();
                if (string.IsNullOrEmpty(content))
                    return new List<RoadmapLinkDto>();

                var parsed = System.Text.Json.JsonSerializer.Deserialize<LinksWrapper>(
                    content,
                    new JsonSerializerOptions { PropertyNameCaseInsensitive = true });

                return parsed?.Links ?? new List<RoadmapLinkDto>();
            }

            return new List<RoadmapLinkDto>();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error in GetTheCompleteLinkedNodes");
            return new List<RoadmapLinkDto>();
        }
    }

    private class LinksWrapper
    {
        public List<RoadmapLinkDto> Links { get; set; }
    }

    #endregion

    #region ChatBot
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
    #endregion

    #region Validate the comment with the reason
    public async Task<(bool isValid, string? reason)> ValidateCommentAsync(string content)
    {
        try
        {

            // Use OpenAI's moderation API - purpose-built, faster, and free!
            _httpClient.DefaultRequestHeaders.Authorization =
                new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", _chatBotSettings.ApiKey);

            var moderationRequest = new { input = content };
            var moderationJson = new StringContent(
                JsonConvert.SerializeObject(moderationRequest),
                Encoding.UTF8,
                "application/json");

            var moderationResponse = await _httpClient.PostAsync(
                "https://api.openai.com/v1/moderations",
                moderationJson);

            if (moderationResponse.IsSuccessStatusCode)
            {
                var moderationResult = await moderationResponse.Content.ReadAsStringAsync();
                var moderationData = JsonConvert.DeserializeObject<dynamic>(moderationResult);

                if (moderationData?.results?[0]?.flagged == true)
                {
                    // Get detailed category information
                    var categories = moderationData.results[0].categories;
                    var flaggedCategories = new List<string>();

                    // Convert dynamic to dictionary for safe property access
                    var categoryDict = JsonConvert.DeserializeObject<Dictionary<string, bool>>(categories.ToString());

                    if (categoryDict != null)
                    {
                        if (categoryDict.ContainsKey("hate") && categoryDict["hate"]) flaggedCategories.Add("hate speech");
                        if (categoryDict.ContainsKey("harassment") && categoryDict["harassment"]) flaggedCategories.Add("harassment");
                        if (categoryDict.ContainsKey("violence") && categoryDict["violence"]) flaggedCategories.Add("violence");
                        if (categoryDict.ContainsKey("sexual") && categoryDict["sexual"]) flaggedCategories.Add("sexual content");
                        if (categoryDict.ContainsKey("self-harm") && categoryDict["self-harm"]) flaggedCategories.Add("self-harm");
                    }

                    var reason = flaggedCategories.Any()
                        ? $"Detect by OPENAI: Content contains inappropriate {string.Join(", ", flaggedCategories)}"
                        : "Detect by OPENAI:Content contains inappropriate language";

                    _logger.LogWarning("Comment flagged by OpenAI moderation: {Content}. Categories: {Categories}",
                        content, string.Join(", ", flaggedCategories));

                    return (false, reason);
                }
            }
            else
            {
                _logger.LogWarning("OpenAI moderation API failed with status: {StatusCode}", moderationResponse.StatusCode);
            }

            // Content passed OpenAI moderation, but let's also check with our educational fallback
            return ValidateWithEducationalFallback(content);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error validating comment with OpenAI Moderation API, using educational fallback");
            // In case of errors, use educational fallback validation
            return ValidateWithEducationalFallback(content);
        }
    }

    /// <summary>
    /// Educational platform specific content validation fallback
    /// </summary>
    private (bool isValid, string? reason) ValidateWithEducationalFallback(string content)
    {
        if (string.IsNullOrWhiteSpace(content))
            return (false, "Content cannot be empty");

        var lowerContent = content.ToLowerInvariant();

        // Academic integrity violations
        var academicIntegrityWords = new[]
        {
            "cheat", "cheating", "plagiarize", "plagiarism", "copy answers", "homework answers",
            "test answers", "exam answers", "assignment answers", "quiz answers"
        };

        if (academicIntegrityWords.Any(word => lowerContent.Contains(word)))
        {
            return (false, "Content appears to violate academic integrity policies");
        }

        // Inappropriate educational content
        var inappropriateEducationalWords = new[]
        {
            "stupid teacher", "dumb professor", "worst class", "hate this course",
            "teacher sucks", "professor sucks", "waste of time", "useless course",
            "terrible instructor", "awful teacher", "bad professor"
        };

        if (inappropriateEducationalWords.Any(phrase => lowerContent.Contains(phrase)))
        {
            return (false, "Content contains inappropriate language about educational staff or courses");
        }

        // Spam patterns common in educational platforms
        var spamPatterns = new[]
        {
            "buy essay", "essay writing service", "homework help service",
            "assignment writing", "paper writing service", "thesis writing",
            "click here", "visit our website", "contact us for", "www.", "http",
            "make money", "earn cash", "work from home", "free money"
        };

        if (spamPatterns.Any(pattern => lowerContent.Contains(pattern)))
        {
            return (false, "Content appears to be spam or promotional material");
        }

        // Strong profanity check (educational appropriate)
        var strongProfanity = new[]
        {
            "fuck", "fucking", "shit", "bitch", "bastard", "asshole", "ass hole",
            "damn it", "goddamn", "god damn", "son of a bitch", "piece of shit"
        };

        if (strongProfanity.Any(word => lowerContent.Contains(word)))
        {
            return (false, "Content contains inappropriate language that is not suitable for an educational platform");
        }

        // Basic profanity check (educational appropriate) - allow some minor frustration
        var basicProfanity = new[]
        {
            "damn", "hell", "crap", "suck", "stupid", "dumb", "idiot", "moron",
            "shut up", "screw", "piss", "bullshit", "bs"
        };

        var profanityCount = basicProfanity.Count(word => lowerContent.Contains(word));
        if (profanityCount >= 3) // Allow some frustration, but not excessive profanity
        {
            return (false, "Content contains excessive inappropriate language for an educational platform");
        }

        // Personal attacks or harassment patterns
        var personalAttackPatterns = new[]
        {
            "you are stupid", "you're stupid", "you are dumb", "you're dumb",
            "you suck", "shut up", "go kill yourself", "kill yourself",
            "you're worthless", "you are worthless", "loser", "failure"
        };

        if (personalAttackPatterns.Any(pattern => lowerContent.Contains(pattern)))
        {
            return (false, "Content contains personal attacks or harassment");
        }

        // Discriminatory content
        var discriminatoryPatterns = new[]
        {
            "because you're", "all women", "all men", "typical girl", "typical guy",
            "all asians", "all blacks", "all whites", "you people", "your kind"
        };

        if (discriminatoryPatterns.Any(pattern => lowerContent.Contains(pattern)))
        {
            return (false, "Content contains potentially discriminatory language");
        }

        // Content length validation (prevent spam)
        if (content.Length > 2000)
        {
            return (false, "Comment is too long (maximum 2000 characters)");
        }

        // Excessive caps (shouting)
        var upperCaseChars = content.Count(char.IsUpper);
        var totalLetters = content.Count(char.IsLetter);
        if (totalLetters > 10 && (double)upperCaseChars / totalLetters > 0.7)
        {
            return (false, "Please avoid excessive use of capital letters (it appears like shouting)");
        }

        // Excessive repetition (spam indication)
        if (System.Text.RegularExpressions.Regex.IsMatch(content, @"(.)\1{4,}") || // Same character 5+ times
            System.Text.RegularExpressions.Regex.IsMatch(content, @"\b(\w+)\s+\1\s+\1\b")) // Same word 3+ times
        {
            return (false, "Content contains excessive repetition");
        }

        return (true, null);
    }


    #endregion
}
