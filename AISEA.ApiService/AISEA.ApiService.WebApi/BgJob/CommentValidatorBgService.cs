using AISEA.ApiService.DAL.Entities;
using AISEA.ApiService.DAL.Repositories;
using AISEA.ApiService.SHARED.DTOs.Requests.Noti;
using AISEA.ApiService.SHARED.DTOs.Responses.SubjectComment;
using AISEA.ApiService.SHARED.Interfaces;
using AISEA.ApiService.WebApi.HubUtil;

namespace AISEA.ApiService.WebApi.BgJob;

public class CommentValidatorBgService : BackgroundService
{
    private readonly ILogger<CommentValidatorBgService> _logger;
    private readonly IServiceProvider _serviceProvider;

    public CommentValidatorBgService(ILogger<CommentValidatorBgService> logger, IServiceProvider serviceProvider)
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
                using var scope = _serviceProvider.CreateScope();
                var subjectCommentRepository = scope.ServiceProvider.GetRequiredService<SubjectCommentRepository>();
                var chatOpenAIService = scope.ServiceProvider.GetRequiredService<IChatOpenAIService>();
                var notifier = scope.ServiceProvider.GetRequiredService<NotificationHubNotifier>();
                var studentProfileRepository = scope.ServiceProvider.GetRequiredService<StudentProfileRepository>();


                var commentsToValidate = await subjectCommentRepository.GetAllToValidateAsync();

                if (commentsToValidate.Any())
                {
                    var commentsToRemove = new List<SubjectComment>();
                    var commentsToUpdate = new List<SubjectComment>();
                    var notifications = new List<(long, NotificationDTO)>();

                    foreach (var comment in commentsToValidate)
                    {
                        CommentVerificationResult res = await chatOpenAIService.VerifyCommentAsync(comment.Content);

                        if (res.IsBad)
                        {
                            commentsToRemove.Add(comment);
                            var studentProfile = await studentProfileRepository.GetByIdAsync(comment.StudentProfileId);

                            notifications.Add((
                             studentProfile.UserId,
                             new NotificationDTO
                             {
                                 Title = $"Comment Removed on subject '{comment.SubjectId}'",
                                 Content = $"Reason: {res.Reason}"
                             }
                         ));
                        }
                        else
                        {
                            comment.IsScannedToValidate = true;
                            commentsToUpdate.Add(comment);
                        }
                    }

                    if (commentsToRemove.Any())
                        await subjectCommentRepository.RemoveRangeAsync(commentsToRemove);

                    if (commentsToUpdate.Any())
                        await subjectCommentRepository.UpdateRangeAsync(commentsToUpdate);
                        
                    if (notifications.Any())
                        await notifier.NotifyUsersAsync(notifications);

                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error while validating the subject comments.");
            }

            await Task.Delay(TimeSpan.FromMinutes(10), stoppingToken);
        }
    }
}
