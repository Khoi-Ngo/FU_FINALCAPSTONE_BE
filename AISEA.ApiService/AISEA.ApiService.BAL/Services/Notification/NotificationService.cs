using AISEA.ApiService.DAL.Repositories;
using AISEA.ApiService.SHARED.DTOs.Requests.Noti;
using AISEA.ApiService.SHARED.DTOs.Requests.Pagin;
using AISEA.ApiService.SHARED.DTOs.Responses.Noti;
using AISEA.ApiService.SHARED.DTOs.Responses.Pagin;
using AISEA.ApiService.SHARED.Exceptions;
using AISEA.ApiService.SHARED.Interfaces;
using AutoMapper;

namespace AISEA.ApiService.BAL.Services.Notification;

public class NotificationService
{
    private readonly NotificationRepository _notificationRepository;
    private readonly IJWTService _jwtService;
    private readonly IMapper _mapper;
    public NotificationService(NotificationRepository notificationRepository, IJWTService jwtService, IMapper mapper)
    {
        _notificationRepository = notificationRepository;
        _jwtService = jwtService;
        _mapper = mapper;
    }
    public async Task<(NotificationItemResponse notification, long userId)> CreateAsync(string accessToken, NotificationDTO notificationDTO)
    {
        var userId = _jwtService.GetUserIdFromToken(accessToken);
        var notification = MapToNotification(notificationDTO, userId);
        await _notificationRepository.CreateAsync(notification);
        var notificationResponse = _mapper.Map<NotificationItemResponse>(notification);
        return (notificationResponse, userId);
    }

    public async Task<NotificationItemResponse> CreateAsync(long userToNotify, NotificationDTO notificationDTO)
    {
        var notification = MapToNotification(notificationDTO, userToNotify);
        await _notificationRepository.CreateAsync(notification);
        var notificationResponse = _mapper.Map<NotificationItemResponse>(notification);
        return notificationResponse;
    }

    public async Task<(PagedResult<NotificationItemResponse> notifications, long userId)> GetNotificationsAsync(string accessToken, PaginationRequest request)
    {
        var userId = _jwtService.GetUserIdFromToken(accessToken);
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
        var userId = _jwtService.GetUserIdFromToken(accessToken);
        if (userId <= 0) throw new InvalidAccessSession("Invalid user ID from token");
        return userId;
    }


    public async Task<List<long>> RemoveAllExistedOverDaysAsync(int expiredDays)
    {
        return await _notificationRepository.RemoveAllExistedOverDaysAsync(expiredDays);
    }

    private DAL.Entities.Notification MapToNotification(NotificationDTO notificationDTO, long userId)
    {
        var notification = _mapper.Map<DAL.Entities.Notification>(notificationDTO);
        notification.UserId = userId;
        return notification;
    }

}