using AISEA.ApiService.DAL.Entities;
using AISEA.ApiService.DAL.Repositories;
using AISEA.ApiService.SHARED.Const.Values;
using AISEA.ApiService.SHARED.DTOs.Roadmap;

namespace AISEA.ApiService.WebApi.BgJob;

public class CachingForAIFeatureBgService : BackgroundService
{
    private readonly ILogger<CachingForAIFeatureBgService> _logger;
    private readonly IServiceProvider _serviceProvider;

    public CachingForAIFeatureBgService(ILogger<CachingForAIFeatureBgService> logger, IServiceProvider serviceProvider)
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
                    var userRepo = scope.ServiceProvider.GetRequiredService<UserRepository>();
                    var joinSubRepo = scope.ServiceProvider.GetRequiredService<JoinedSubjectRepository>();
                    var subjectRepo = scope.ServiceProvider.GetRequiredService<SubjectRepository>();
                    var roadmapRepo = scope.ServiceProvider.GetRequiredService<RoadmapRepository>();


                    var studentUserIDs = await userRepo.GetAllActiveStudentUserIDsAsync();

                    //cache data relating to student
                    foreach (var studentUserID in studentUserIDs)
                    {
                        var studentData = await userRepo.GetStudentByIdAsync(studentUserID);

                        if (studentData.StudentProfile is not null)
                        {
                            var studentPersonalSubjectsInCurriculum = await subjectRepo.GetAllViaCurriculumNotIncludeComboAsync(studentData.StudentProfile.CurriculumCode);
                            var studentCurrentTranscript = await joinSubRepo.GetTranscriptAsync(studentData.StudentProfile.Id);
                            var studentPersonalSubjectsInCombo = await subjectRepo.GetAllViaComboNameAsync(studentData.StudentProfile.RegisteredComboCode);
                            var roadmapId = await roadmapRepo.GetIDByStudentProfileIDAsync(studentData.StudentProfile.Id);
                            RoadmapDto roadmapDto = null;
                            if (roadmapId > 0)
                            {
                                var roadmap = await roadmapRepo.GetRoadmapWithGraphAsync(roadmapId);
                                roadmapDto = roadmap == null ? null : MapToDto(roadmap);
                            }

                            //register to redis
                            var cachedKeystudentData = $"{CacheKeyForAIFeature.PrefixToGetStudentDataByUserID}{studentData.Id}";
                            var cachedKeystudentPersonalSubjectsInCurriculum = $"{CacheKeyForAIFeature.PrefixToGetPersonalCurByStudentProfileID}{studentData.StudentProfile.Id}";
                            var cachedKeystudentPersonalSubjectsInCombo = $"{CacheKeyForAIFeature.PrefixToGetPersonalComboByStudentProfileID}{studentData.StudentProfile.Id}";
                            var cachedKeystudentCurrentTranscript = $"{CacheKeyForAIFeature.PrefixToGetStudentTranscriptByStudentProfileID}{studentData.StudentProfile.Id}";
                            // var cachedKeyroadmapDto = $"{CacheKeyForAIFeature.PrefixToGetRoadmapDataByStudentProfileID}{studentData.StudentProfile.Id}";



                            await redisRepo.SetValueAsync(cachedKeystudentData, studentData, TimeSpan.FromDays(3));
                            await redisRepo.SetValueAsync(cachedKeystudentPersonalSubjectsInCurriculum, studentPersonalSubjectsInCurriculum, TimeSpan.FromDays(3));
                            await redisRepo.SetValueAsync(cachedKeystudentPersonalSubjectsInCombo, studentPersonalSubjectsInCombo, TimeSpan.FromDays(3));
                            await redisRepo.SetValueAsync(cachedKeystudentCurrentTranscript, studentCurrentTranscript, TimeSpan.FromDays(3));
                            // await redisRepo.SetValueAsync(cachedKeyroadmapDto, roadmapDto, TimeSpan.FromDays(3));


                        }

                    }


                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred in CachingForAIFeatureBgService.");
            }

            await Task.Delay(TimeSpan.FromDays(1), stoppingToken);
        }
    }


    private static RoadmapDto MapToDto(StudyRoadMap entity)
    {
        var links = entity.Nodes
            .SelectMany(n => n.Dependents)
            .Select(d => new RoadmapLinkDto
            {
                Id = d.Id,
                FromNodeId = d.FromNodeId,
                ToNodeId = d.ToNodeId
            }).ToList();

        var nodes = entity.Nodes.Select(n => new RoadmapNodeDto
        {
            Id = n.Id,
            SubjectCode = n.SubjectCode,
            SemesterNumber = n.SemesterNumber,
            SubjectName = n.SubjectName,
            Description = n.Description,
            PrerequisiteIds = n.Prerequisites.Select(p => p.FromNodeId).ToList(),
            DependentIds = n.Dependents.Select(d => d.ToNodeId).ToList(),
            OutgoingLinks = n.Dependents
                .Select(d => new RoadmapLinkDto
                {
                    Id = d.Id,
                    FromNodeId = d.FromNodeId,
                    ToNodeId = d.ToNodeId
                }).ToList()
        }).ToList();

        return new RoadmapDto
        {
            Id = entity.Id,
            Name = entity.Name,
            StudentProfileId = entity.StudentProfileId,
            Nodes = nodes,
            Links = links
        };
    }

}