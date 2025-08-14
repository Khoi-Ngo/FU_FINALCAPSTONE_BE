namespace AISEA.ApiService.SHARED.Interfaces
{
    public interface IMailService
    {
        Task SendEmailAsync(string to, string subject, string body);
        Task SendEmailWithAttachmentAsync(string to, string subject, string body, string attachmentPath);
        Task SendHtmlEmailAsync(string to, string subject, string htmlBody);
    }
}