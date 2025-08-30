using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using AISEA.ApiService.SHARED.Interfaces;
using AISEA.ApiService.SHARED.PropConfigs;

namespace AISEA.ApiService.DAL.Infrastructure;

public class GitRepoService : IGitRepoService
{
    private readonly CourseTrackSettings _courseTrackSettings;

    public GitRepoService(CourseTrackSettings courseTrackSettings)
    {
        _courseTrackSettings = courseTrackSettings;
    }

    public async Task<object?> GetRepoDataAsync(string owner, string repoName)
    {
        try
        {
            var token = _courseTrackSettings.PersonalDevGitHubToken;

            using var client = new HttpClient();
            client.DefaultRequestHeaders.Add("User-Agent", "AISEA-CourseTracker-App");
            if (!string.IsNullOrEmpty(token))
            {
                client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
            }

            var options = new JsonSerializerOptions { PropertyNameCaseInsensitive = true };

            // GraphQL query
            var graphQlQuery = @"query ($owner: String!, $repo: String!) {
                repository(owner: $owner, name: $repo) {
                    name
                    nameWithOwner
                    description
                    url
                    stargazerCount
                    watchers { totalCount }
                    forkCount
                    issues(states: OPEN) { totalCount }
                    pullRequests(states: OPEN) { totalCount }
                    defaultBranchRef { name target { ... on Commit { history(first: 0) { totalCount } } } }
                    refs(refPrefix: ""refs/heads/"") { totalCount }
                    releases(last: 1) { totalCount }
                    languages(first: 100, orderBy: { field: SIZE, direction: DESC }) { edges { size node { name } } }
                    repositoryTopics(first: 100) { nodes { topic { name } } }
                    licenseInfo { name }
                    isPrivate
                    isFork
                    isArchived
                    isDisabled
                    createdAt
                    pushedAt
                    updatedAt
                    diskUsage
                    primaryLanguage { name }
                    owner { login avatarUrl }
                    mentionableUsers { totalCount }
                }
                rateLimit { cost remaining }
            }";
            var requestBody = new
            {
                query = graphQlQuery,
                variables = new { owner, repo = repoName }
            };

            GitHubRepoData? repoData = null;
            try
            {
                var content = new StringContent(JsonSerializer.Serialize(requestBody), Encoding.UTF8, "application/json");
                var graphQlResponse = await client.PostAsync("https://api.github.com/graphql", content);

                if (!graphQlResponse.IsSuccessStatusCode)
                {
                    return null; // Return null instead of throwing
                }

                var jsonResponse = await graphQlResponse.Content.ReadAsStringAsync();
                Console.WriteLine($"Status: {graphQlResponse.StatusCode}, Response: {jsonResponse}");
                var graphResponse = JsonSerializer.Deserialize<GraphQLRepoResponse>(jsonResponse, options);
                if (graphResponse?.Data?.Repository == null)
                {
                    return null; // Return null if no repo data
                }

                var graphRepo = graphResponse.Data.Repository;
                repoData = new GitHubRepoData
                {
                    Name = graphRepo.Name,
                    NameWithOwner = graphRepo.NameWithOwner,
                    Description = graphRepo.Description,
                    Url = graphRepo.Url,
                    StargazerCount = graphRepo.StargazerCount,
                    WatcherCount = graphRepo.Watchers?.TotalCount ?? 0,
                    ForkCount = graphRepo.ForkCount,
                    OpenIssuesCount = graphRepo.Issues?.TotalCount ?? 0,
                    OpenPullRequestsCount = graphRepo.PullRequests?.TotalCount ?? 0,
                    DefaultBranchName = graphRepo.DefaultBranchRef?.Name,
                    TotalCommitCount = graphRepo.DefaultBranchRef?.Target?.History?.TotalCount ?? 0,
                    BranchCount = graphRepo.Refs?.TotalCount ?? 0,
                    ReleaseCount = graphRepo.Releases?.TotalCount ?? 0,
                    Languages = graphRepo.Languages?.Edges?.Select(e => new RepositoryLanguage { Name = e.Node?.Name, Size = e.Size }).ToList() ?? new List<RepositoryLanguage>(),
                    Topics = graphRepo.RepositoryTopics?.Nodes?.Select(n => n.Topic?.Name).ToList() ?? new List<string>(),
                    LicenseName = graphRepo.LicenseInfo?.Name,
                    IsPrivate = graphRepo.IsPrivate,
                    IsFork = graphRepo.IsFork,
                    IsArchived = graphRepo.IsArchived,
                    IsDisabled = graphRepo.IsDisabled,
                    CreatedAt = graphRepo.CreatedAt,
                    PushedAt = graphRepo.PushedAt,
                    UpdatedAt = graphRepo.UpdatedAt,
                    DiskUsage = graphRepo.DiskUsage > 0 ? graphRepo.DiskUsage : null,
                    PrimaryLanguage = graphRepo.PrimaryLanguage?.Name,
                    Owner = graphRepo.Owner,
                    MentionableUsersCount = graphRepo.MentionableUsers?.TotalCount ?? 0,
                    CommitActivity = new List<CommitActivityWeek>(),
                    Contributors = new List<ContributorStat>(),
                    CodeFrequency = new List<CodeFrequencyEntry>(),
                    Participation = new Participation { All = new List<int>(), Owner = new List<int>() },
                    PunchCard = new List<PunchCardEntry>(),
                    RecentEvents = new List<RepositoryEvent>(),
                    CommitsPerDayLastYear = new Dictionary<string, int>()
                };
            }
            catch
            {
                return null; // Return null on any GraphQL error
            }

            // Contributor count
            try
            {
                var contributorsUrl = $"https://api.github.com/repos/{owner}/{repoName}/contributors?per_page=1&anon=true";
                var contribResponse = await client.GetAsync(contributorsUrl);

                if (contribResponse.IsSuccessStatusCode)
                {
                    if (contribResponse.Headers.TryGetValues("Link", out var linkHeaders))
                    {
                        var linkStr = linkHeaders?.FirstOrDefault();
                        if (!string.IsNullOrEmpty(linkStr))
                        {
                            var match = Regex.Match(linkStr, @"<[^>]+[?&]page=(\d+)[^>]*>;\s*rel=""last""");
                            if (match.Success)
                            {
                                repoData.ContributorCount = int.Parse(match.Groups[1].Value);
                            }
                        }
                    }

                    if (repoData.ContributorCount == 0)
                    {
                        var contribJson = await contribResponse.Content.ReadAsStringAsync();
                        var contribArray = JsonSerializer.Deserialize<List<object>>(contribJson, options);
                        repoData.ContributorCount = contribArray?.Count ?? 0;
                    }
                }
                else
                {
                    repoData.ContributorCount = 0;
                }
            }
            catch
            {
                repoData.ContributorCount = 0; // Fallback on error
            }

            // Stats endpoints
            var statsEndpoints = new[]
            {
                "stats/commit_activity",
                "stats/contributors",
                "stats/code_frequency",
                "stats/participation",
                "stats/punch_card"
            };

            foreach (var endpoint in statsEndpoints)
            {
                try
                {
                    var statsUrl = $"https://api.github.com/repos/{owner}/{repoName}/{endpoint}";
                    var statsResponse = await client.GetAsync(statsUrl);

                    if (statsResponse.StatusCode == System.Net.HttpStatusCode.Accepted)
                    {
                        await Task.Delay(2000);
                        statsResponse = await client.GetAsync(statsUrl);
                    }

                    var statsKey = endpoint.Split('/')[1];
                    if (statsResponse.IsSuccessStatusCode)
                    {
                        var statsJson = await statsResponse.Content.ReadAsStringAsync();
                        switch (statsKey)
                        {
                            case "commit_activity":
                                repoData.CommitActivity = JsonSerializer.Deserialize<List<CommitActivityWeek>>(statsJson, options) ?? new List<CommitActivityWeek>();
                                break;
                            case "contributors":
                                repoData.Contributors = JsonSerializer.Deserialize<List<ContributorStat>>(statsJson, options) ?? new List<ContributorStat>();
                                break;
                            case "code_frequency":
                                var rawFreq = JsonSerializer.Deserialize<List<long[]>>(statsJson, options) ?? new List<long[]>();
                                repoData.CodeFrequency = rawFreq.Select(r => new CodeFrequencyEntry
                                {
                                    Timestamp = r.Length > 0 ? r[0] : 0,
                                    Additions = r.Length > 1 ? (int)r[1] : 0,
                                    Deletions = r.Length > 2 ? (int)r[2] : 0
                                }).ToList();
                                break;
                            case "participation":
                                repoData.Participation = JsonSerializer.Deserialize<Participation>(statsJson, options) ?? new Participation { All = new List<int>(), Owner = new List<int>() };
                                break;
                            case "punch_card":
                                var rawPunch = JsonSerializer.Deserialize<List<int[]>>(statsJson, options) ?? new List<int[]>();
                                repoData.PunchCard = rawPunch.Select(r => new PunchCardEntry
                                {
                                    Day = r.Length > 0 ? r[0] : 0,
                                    Hour = r.Length > 1 ? r[1] : 0,
                                    Commits = r.Length > 2 ? r[2] : 0
                                }).ToList();
                                break;
                        }
                    }
                    else
                    {
                        switch (statsKey)
                        {
                            case "commit_activity":
                                repoData.CommitActivity = new List<CommitActivityWeek>();
                                break;
                            case "contributors":
                                repoData.Contributors = new List<ContributorStat>();
                                break;
                            case "code_frequency":
                                repoData.CodeFrequency = new List<CodeFrequencyEntry>();
                                break;
                            case "participation":
                                repoData.Participation = new Participation { All = new List<int>(), Owner = new List<int>() };
                                break;
                            case "punch_card":
                                repoData.PunchCard = new List<PunchCardEntry>();
                                break;
                        }
                    }
                }
                catch
                {
                    var statsKey = endpoint.Split('/')[1];
                    switch (statsKey)
                    {
                        case "commit_activity":
                            repoData.CommitActivity = new List<CommitActivityWeek>();
                            break;
                        case "contributors":
                            repoData.Contributors = new List<ContributorStat>();
                            break;
                        case "code_frequency":
                            repoData.CodeFrequency = new List<CodeFrequencyEntry>();
                            break;
                        case "participation":
                            repoData.Participation = new Participation { All = new List<int>(), Owner = new List<int>() };
                            break;
                        case "punch_card":
                            repoData.PunchCard = new List<PunchCardEntry>();
                            break;
                    }
                }
            }

            // Recent events
            try
            {
                var eventsUrl = $"https://api.github.com/repos/{owner}/{repoName}/events?per_page=30";
                var eventsResponse = await client.GetAsync(eventsUrl);
                if (eventsResponse.IsSuccessStatusCode)
                {
                    var eventsJson = await eventsResponse.Content.ReadAsStringAsync();
                    repoData.RecentEvents = JsonSerializer.Deserialize<List<RepositoryEvent>>(eventsJson, options) ?? new List<RepositoryEvent>();
                }
                else
                {
                    repoData.RecentEvents = new List<RepositoryEvent>();
                }
            }
            catch
            {
                repoData.RecentEvents = new List<RepositoryEvent>(); // Empty list on error
            }

            // Commits per day
            var commitsPerDay = new Dictionary<string, int>();
            foreach (var week in repoData.CommitActivity)
            {
                var weekStart = DateTimeOffset.FromUnixTimeSeconds(week.Week).Date;
                for (int i = 0; i < week.Days.Count; i++)
                {
                    var dayDate = weekStart.AddDays(i).ToString("yyyy-MM-dd");
                    commitsPerDay[dayDate] = week.Days[i];
                }
            }
            repoData.CommitsPerDayLastYear = commitsPerDay;

            return repoData;
        }
        catch
        {
            return null; // Return null for any top-level error
        }
    }
}

public class RepositoryLanguage
{
    public string Name { get; set; }
    public int Size { get; set; }
}

public class Owner
{
    public string Login { get; set; }
    [JsonPropertyName("avatar_url")]
    public string AvatarUrl { get; set; }
}

public class CommitActivityWeek
{
    public long Week { get; set; }
    public int Total { get; set; }
    public List<int> Days { get; set; }
}

public class WeekStat
{
    public long W { get; set; }
    public int A { get; set; } // Additions
    public int D { get; set; } // Deletions
    public int C { get; set; } // Commits
}

public class User
{
    public long Id { get; set; }
    public string Login { get; set; }
    [JsonPropertyName("avatar_url")]
    public string AvatarUrl { get; set; }
    // Additional fields from GitHub User object can be added if needed (e.g., Url, HtmlUrl, etc.)
}

public class ContributorStat
{
    public User Author { get; set; } // Null for anonymous contributors
    public string Name { get; set; } // Present for anonymous
    public string Email { get; set; } // Present for anonymous
    public int Total { get; set; }
    public List<WeekStat> Weeks { get; set; }
}

public class CodeFrequencyEntry
{
    public long Timestamp { get; set; }
    public int Additions { get; set; }
    public int Deletions { get; set; }
}

public class Participation
{
    public List<int> All { get; set; }
    public List<int> Owner { get; set; }
}

public class PunchCardEntry
{
    public int Day { get; set; } // 0 = Sunday, 6 = Saturday
    public int Hour { get; set; } // 0-23
    public int Commits { get; set; }
}

public class RepoInfo
{
    public long Id { get; set; }
    public string Name { get; set; }
    public string Url { get; set; }
}

public class RepositoryEvent
{
    public string Id { get; set; }
    public string Type { get; set; }
    public User Actor { get; set; }
    public RepoInfo Repo { get; set; }
    public JsonElement Payload { get; set; } // Flexible for varying event payloads
    public bool Public { get; set; }
    [JsonPropertyName("created_at")]
    public DateTimeOffset? CreatedAt { get; set; }
}

// Main DTO to contain all repository data
public class GitHubRepoData
{
    // From GraphQL Repository fields (verified against GitHub GraphQL API docs)
    public string Name { get; set; }
    public string NameWithOwner { get; set; }
    public string Description { get; set; }
    public string Url { get; set; }
    public int StargazerCount { get; set; }
    public int WatcherCount { get; set; }
    public int ForkCount { get; set; }
    public int OpenIssuesCount { get; set; } // Open issues only
    public int OpenPullRequestsCount { get; set; } // Open PRs only
    public string DefaultBranchName { get; set; }
    public int TotalCommitCount { get; set; } // Total commits on default branch
    public int BranchCount { get; set; }
    public int ReleaseCount { get; set; }
    public List<RepositoryLanguage> Languages { get; set; } // Ordered by size descending
    public List<string> Topics { get; set; }
    public string LicenseName { get; set; }
    public bool IsPrivate { get; set; }
    public bool IsFork { get; set; }
    public bool IsArchived { get; set; }
    public bool IsDisabled { get; set; }
    public DateTimeOffset CreatedAt { get; set; }
    public DateTimeOffset PushedAt { get; set; }
    public DateTimeOffset UpdatedAt { get; set; }
    public int? DiskUsage { get; set; } // In KB
    public string PrimaryLanguage { get; set; }
    public Owner Owner { get; set; }
    public int MentionableUsersCount { get; set; }

    // From REST APIs (verified against GitHub REST API docs v2022-11-28)
    public int ContributorCount { get; set; } // Includes anonymous if applicable
    public List<CommitActivityWeek> CommitActivity { get; set; } // Weekly aggregates for last year
    public List<ContributorStat> Contributors { get; set; } // Contributor commit activity
    public List<CodeFrequencyEntry> CodeFrequency { get; set; } // Weekly additions/deletions
    public Participation Participation { get; set; } // Weekly commits over last year
    public List<PunchCardEntry> PunchCard { get; set; } // Hourly commit counts by day of week
    public List<RepositoryEvent> RecentEvents { get; set; } // Up to 30 recent events

    // Derived fields
    public Dictionary<string, int> CommitsPerDayLastYear { get; set; }
}

// Additional classes for GraphQL deserialization (to map to GitHubRepoData)
public class GraphQLRepoResponse
{
    public GraphQLData Data { get; set; }
}

public class GraphQLData
{
    public GraphQLRepository Repository { get; set; }
    public RateLimit RateLimit { get; set; }
}

public class RateLimit
{
    public int Cost { get; set; }
    public int Remaining { get; set; }
}

public class GraphQLRepository
{
    public string Name { get; set; }
    public string NameWithOwner { get; set; }
    public string Description { get; set; }
    public string Url { get; set; }
    public int StargazerCount { get; set; }
    public TotalCountObject Watchers { get; set; }
    public int ForkCount { get; set; }
    public TotalCountObject Issues { get; set; }
    public TotalCountObject PullRequests { get; set; }
    public DefaultBranchRef DefaultBranchRef { get; set; }
    public TotalCountObject Refs { get; set; }
    public TotalCountObject Releases { get; set; }
    public LanguageConnection Languages { get; set; }
    public RepositoryTopicConnection RepositoryTopics { get; set; }
    public LicenseInfo LicenseInfo { get; set; }
    public bool IsPrivate { get; set; }
    public bool IsFork { get; set; }
    public bool IsArchived { get; set; }
    public bool IsDisabled { get; set; }
    public DateTimeOffset CreatedAt { get; set; }
    public DateTimeOffset PushedAt { get; set; }
    public DateTimeOffset UpdatedAt { get; set; }
    public int DiskUsage { get; set; }
    public PrimaryLanguage PrimaryLanguage { get; set; }
    public Owner Owner { get; set; }
    public TotalCountObject MentionableUsers { get; set; }
}

public class TotalCountObject
{
    public int TotalCount { get; set; }
}

public class DefaultBranchRef
{
    public string Name { get; set; }
    public CommitTarget Target { get; set; }
}

public class CommitTarget
{
    public CommitHistory History { get; set; }
}

public class CommitHistory
{
    public int TotalCount { get; set; }
}

public class LanguageConnection
{
    public List<LanguageEdge> Edges { get; set; }
}

public class LanguageEdge
{
    public int Size { get; set; }
    public LanguageNode Node { get; set; }
}

public class LanguageNode
{
    public string Name { get; set; }
}

public class RepositoryTopicConnection
{
    public List<RepositoryTopicNode> Nodes { get; set; }
}

public class RepositoryTopicNode
{
    public Topic Topic { get; set; }
}

public class Topic
{
    public string Name { get; set; }
}

public class LicenseInfo
{
    public string Name { get; set; }
}

public class PrimaryLanguage
{
    public string Name { get; set; }
}