using System.Text;
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
    private readonly IMailService _mailService;
    private readonly IMapper _mapper;
    public NotificationService(NotificationRepository notificationRepository
    , IJWTService jwtService
    , IMapper mapper)
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

    public async Task SendBulkNotificationDataAsMail(string accessToken, List<NotificationDTO> notificationDTOs)
    {
        var receiverMail = _jwtService.GetEmailFromToken(accessToken);
        await _mailService.SendHtmlEmailAsync(receiverMail, "Import subject failed", BuildNotificationTable(notificationDTOs));
    }

    private string BuildNotificationTable(IEnumerable<NotificationDTO> notifications)
    {
        var sb = new StringBuilder(notifications.Count() * 150 + 200);
        sb.Append("<table border='1' cellpadding='5' cellspacing='0' style='border-collapse:collapse;font-family:Arial,sans-serif;font-size:14px;'>");
        sb.Append("<thead style='background-color:#f2f2f2;'>");
        sb.Append("<tr><th>Title</th><th>Content</th><th>Link</th></tr>");
        sb.Append("</thead><tbody>");

        foreach (var n in notifications)
        {
            sb.Append("<tr>");
            sb.Append("<td>").Append(System.Net.WebUtility.HtmlEncode(n.Title ?? "")).Append("</td>");
            sb.Append("<td>").Append(System.Net.WebUtility.HtmlEncode(n.Content ?? "")).Append("</td>");
            sb.Append("<td>");
            if (!string.IsNullOrEmpty(n.Link))
                sb.Append("<a href='").Append(System.Net.WebUtility.HtmlEncode(n.Link)).Append("'>Open</a>");
            sb.Append("</td>");
            sb.Append("</tr>");
        }

        sb.Append("</tbody></table>");
        return sb.ToString();
    }


}