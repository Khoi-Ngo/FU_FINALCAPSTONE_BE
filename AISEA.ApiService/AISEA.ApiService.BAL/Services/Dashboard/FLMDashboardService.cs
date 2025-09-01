using AISEA.ApiService.DAL.Repositories;
using AISEA.ApiService.SHARED.DTOs.Responses.Dashboard;
using AISEA.ApiService.SHARED.Interfaces;
using Microsoft.Extensions.Logging;

namespace AISEA.ApiService.BAL.Services.Dashboard
{
    public class FLMDashboardService
    {
        private readonly FLMDashboardRepository _dashboardRepository;
        private readonly IRedisRepository _redisRepository;
        private readonly ILogger<FLMDashboardService> _logger;

        // Cache keys
        private const string OVERVIEW_CACHE_KEY = "flm_dashboard_overview";
        private const string SUBJECT_STATS_CACHE_KEY = "flm_dashboard_subject_stats";
        private const string CURRICULA_STATS_CACHE_KEY = "flm_dashboard_curricula_stats";
        private const string RECENT_ACTIVITIES_CACHE_KEY = "flm_dashboard_recent_activities";

        // Cache durations
        private readonly TimeSpan _overviewCacheDuration = TimeSpan.FromMinutes(15);
        private readonly TimeSpan _statisticsCacheDuration = TimeSpan.FromMinutes(30);
        private readonly TimeSpan _activitiesCacheDuration = TimeSpan.FromMinutes(5);

        public FLMDashboardService(
            FLMDashboardRepository dashboardRepository,
            IRedisRepository redisRepository,
            ILogger<FLMDashboardService> logger)
        {
            _dashboardRepository = dashboardRepository;
            _redisRepository = redisRepository;
            _logger = logger;
        }

        public async Task<FLMDashboardOverviewResponse> GetOverviewAsync()
        {
            try
            {
                var cachedOverview = await _redisRepository.GetValueAsync<FLMDashboardOverviewResponse>(OVERVIEW_CACHE_KEY);
                if (cachedOverview != null)
                {
                    _logger.LogInformation("Returning cached overview data from Redis");
                    return cachedOverview;
                }

                _logger.LogInformation("Generating fresh overview data");

                var summary = await _dashboardRepository.GetOverviewSummaryAsync();
                var approvalDistribution = await _dashboardRepository.GetApprovalStatusDistributionAsync();

                var response = new FLMDashboardOverviewResponse
                {
                    Summary = summary,
                    ApprovalDistribution = approvalDistribution,
                    GeneratedAt = DateTime.UtcNow
                };

                await _redisRepository.SetValueAsync(OVERVIEW_CACHE_KEY, response, _overviewCacheDuration);
                _logger.LogInformation("Overview data cached in Redis for {Duration} minutes", _overviewCacheDuration.TotalMinutes);

                return response;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error generating overview data");
                throw;
            }
        }

        public async Task<SubjectStatisticsResponse> GetSubjectStatisticsAsync()
        {
            try
            {
                var cachedStats = await _redisRepository.GetValueAsync<SubjectStatisticsResponse>(SUBJECT_STATS_CACHE_KEY);
                if (cachedStats != null)
                {
                    _logger.LogInformation("Returning cached subject statistics from Redis");
                    return cachedStats;
                }

                _logger.LogInformation("Generating fresh subject statistics");

                var subjectsByProgram = await _dashboardRepository.GetSubjectsByProgramAsync();
                var creditDistribution = await _dashboardRepository.GetCreditDistributionAsync();
                var syllabusAvailability = await _dashboardRepository.GetSyllabusAvailabilityAsync();
                var topSubjectsWithVersions = await _dashboardRepository.GetTopSubjectsWithMostVersionsAsync();

                var response = new SubjectStatisticsResponse
                {
                    SubjectsByProgram = subjectsByProgram,
                    CreditDistribution = creditDistribution,
                    SyllabusAvailability = syllabusAvailability,
                    TopSubjectsWithMostVersions = topSubjectsWithVersions,
                    GeneratedAt = DateTime.UtcNow
                };

                await _redisRepository.SetValueAsync(SUBJECT_STATS_CACHE_KEY, response, _statisticsCacheDuration);
                _logger.LogInformation("Subject statistics cached in Redis for {Duration} minutes", _statisticsCacheDuration.TotalMinutes);

                return response;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error generating subject statistics");
                throw;
            }
        }

        public async Task<CurriculaStatisticsResponse> GetCurriculaStatisticsAsync()
        {
            try
            {
                var cachedStats = await _redisRepository.GetValueAsync<CurriculaStatisticsResponse>(CURRICULA_STATS_CACHE_KEY);
                if (cachedStats != null)
                {
                    _logger.LogInformation("Returning cached curricula statistics from Redis");
                    return cachedStats;
                }

                _logger.LogInformation("Generating fresh curricula statistics");

                var curriculaByProgram = await _dashboardRepository.GetCurriculaByProgramAsync();
                var averageSubjects = await _dashboardRepository.GetAverageSubjectsPerCurriculumAsync();
                var sizeDistribution = await _dashboardRepository.GetCurriculumSizeDistributionAsync();
                var semesterCompleteness = await _dashboardRepository.GetSemesterCompletenessAsync();

                var response = new CurriculaStatisticsResponse
                {
                    CurriculaByProgram = curriculaByProgram,
                    AverageSubjects = averageSubjects,
                    SizeDistribution = sizeDistribution,
                    SemesterCompleteness = semesterCompleteness,
                    GeneratedAt = DateTime.UtcNow
                };

                await _redisRepository.SetValueAsync(CURRICULA_STATS_CACHE_KEY, response, _statisticsCacheDuration);
                _logger.LogInformation("Curricula statistics cached in Redis for {Duration} minutes", _statisticsCacheDuration.TotalMinutes);

                return response;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error generating curricula statistics");
                throw;
            }
        }

        public async Task<RecentActivitiesResponse> GetRecentActivitiesAsync()
        {
            try
            {
                var cachedActivities = await _redisRepository.GetValueAsync<RecentActivitiesResponse>(RECENT_ACTIVITIES_CACHE_KEY);
                if (cachedActivities != null)
                {
                    _logger.LogInformation("Returning cached recent activities from Redis");
                    return cachedActivities;
                }

                _logger.LogInformation("Generating fresh recent activities data");

                var newSubjects = await _dashboardRepository.GetRecentSubjectsAsync();
                var newlyApprovedSyllabi = await _dashboardRepository.GetRecentlyApprovedSyllabiAsync();
                var pendingSubjects = await _dashboardRepository.GetPendingSubjectsAsync();
                var expiringSoon = await _dashboardRepository.GetExpiringSoonSubjectVersionsAsync();

                var response = new RecentActivitiesResponse
                {
                    NewSubjects = newSubjects,
                    NewlyApprovedSyllabi = newlyApprovedSyllabi,
                    PendingSubjects = pendingSubjects,
                    ExpiringSoon = expiringSoon,
                    GeneratedAt = DateTime.UtcNow
                };

                await _redisRepository.SetValueAsync(RECENT_ACTIVITIES_CACHE_KEY, response, _activitiesCacheDuration);
                _logger.LogInformation("Recent activities cached in Redis for {Duration} minutes", _activitiesCacheDuration.TotalMinutes);

                return response;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error generating recent activities data");
                throw;
            }
        }

        public async Task ClearCacheAsync()
        {
            await _redisRepository.RemoveByKeyAsync(OVERVIEW_CACHE_KEY);
            await _redisRepository.RemoveByKeyAsync(SUBJECT_STATS_CACHE_KEY);
            await _redisRepository.RemoveByKeyAsync(CURRICULA_STATS_CACHE_KEY);
            await _redisRepository.RemoveByKeyAsync(RECENT_ACTIVITIES_CACHE_KEY);
            _logger.LogInformation("FLM Dashboard Redis cache cleared");
        }
    }
}
