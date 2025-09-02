using AISEA.ApiService.DAL.Repositories;
using AISEA.ApiService.SHARED.Const.Values;

namespace AISEA.ApiService.WebApi.BgJob;

public class CachingFLMForAIFeatureBgService : BackgroundService
{
    private readonly ILogger<CachingFLMForAIFeatureBgService> _logger;
    private readonly IServiceProvider _serviceProvider;

    public CachingFLMForAIFeatureBgService(ILogger<CachingFLMForAIFeatureBgService> logger, IServiceProvider serviceProvider)
    {
        _logger = logger;
        _serviceProvider = serviceProvider;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        while (!stoppingToken.IsCancellationRequested)
        {
            try
            {
                using (var scope = _serviceProvider.CreateScope())
                {
                    var redisRepo = scope.ServiceProvider.GetRequiredService<IRedisRepository>();
                    var curRepo = scope.ServiceProvider.GetRequiredService<CurriculumRepository>();

                    var curriculums = await curRepo.GetAllAcademicDataAsync();

                    var cacheKey = CacheKeyForAIFeature.PrefixToGetAllDataOfFLMCurComSub;

                    await redisRepo.SetValueAsync(cacheKey, curriculums, TimeSpan.FromDays(7));



                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred in CachingFLMForAIFeatureBgService.");
            }

            await Task.Delay(TimeSpan.FromDays(4), stoppingToken);
        }
    }


}