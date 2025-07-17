using AISEA.ApiService.DAL.Repositories;
using AISEA.ApiService.SHARED.DTOs.Requests.Pagin;
using AISEA.ApiService.SHARED.DTOs.Responses.Noti;
using AISEA.ApiService.SHARED.DTOs.Responses.Pagin;
using AISEA.ApiService.SHARED.Exceptions;
using AISEA.ApiService.SHARED.Interfaces;
using AISEA.ApiService.SHARED.PropConfigs;
using AutoMapper;

namespace AISEA.ApiService.BAL.Services.Notification;

public class NotificationService
{
    private readonly NotificationRepository _notificationRepository;
    private readonly IJWTService _jwtService;
    private readonly JwtSettings _jwtSettings;
    private readonly IMapper _mapper;
    public NotificationService(NotificationRepository notificationRepository, IJWTService jwtService, JwtSettings jwtSettings, IMapper mapper)
    {
        _notificationRepository = notificationRepository;
        _jwtService = jwtService;
        _jwtSettings = jwtSettings;
        _mapper = mapper;
    }
    public async Task<(NotificationItemResponse notification, long userId)> CreateAsync(string accessToken, string title, string content, string link)
    {
        var userId = GetUserIdFromToken(accessToken);
        var notification = new DAL.Entities.Notification
        {
            UserId = userId,
            Title = title,
            Content = content,
            Link = link,
        };
        await _notificationRepository.CreateAsync(notification);
        var notificationResponse = _mapper.Map<NotificationItemResponse>(notification);
        return (notificationResponse, userId);
    }

    public async Task<NotificationItemResponse> CreateAsync(long userToNotify, string title, string content, string link)
    {
        var notification = new DAL.Entities.Notification
        {
            UserId = userToNotify,
            Title = title,
            Content = content,
            Link = link,
        };
        await _notificationRepository.CreateAsync(notification);
        var notificationResponse = _mapper.Map<NotificationItemResponse>(notification);
        return notificationResponse;
    }

    public async Task<(PagedResult<NotificationItemResponse> notifications, long userId)> GetNotificationsAsync(string accessToken, PaginationRequest request)
    {
        var userId = GetUserIdFromToken(accessToken);
        var notifications = await _notificationRepository.GetNotificationsAsync(userId, request.PageNumber, request.PageSize);
        return (notifications, userId);
    }

    public async Task<(long broadcastedNotiId, long userId)> MarkAsReadAsync(long notificationId)
    {
        var notification = await _notificationRepository.GetByIdAsync(notificationId);
        if (notification == null) throw new NotFoundException("Notification not found");
        notification.IsRead = true;
        await _notificationRepository.UpdateAsync(notification);
        return (notification.Id, notification.UserId);
    }

    public async Task<long> ConnectUserNotificationGroupAsync(string accessToken)
    {
        var userId = GetUserIdFromToken(accessToken);
        if (userId <= 0) throw new InvalidAccessSession("Invalid user ID from token");
        return userId;
    }

    private long GetUserIdFromToken(string accessToken) =>
     long.Parse(_jwtService.GetAllClaimsFromToken(accessToken).GetValueOrDefault(_jwtSettings.UserId));
}