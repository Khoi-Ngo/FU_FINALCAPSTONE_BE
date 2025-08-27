using SendGrid;
using SendGrid.Helpers.Mail;
using AISEA.ApiService.SHARED.Interfaces;

namespace AISEA.ApiService.DAL.Infrastructure
{
    public class MailService : IMailService, IDisposable
    {
        private readonly SendGridClient _sendGridClient;
        private readonly SHARED.PropConfigs.MailSettings _mailSettings;
        private bool _disposed;

        public MailService(SHARED.PropConfigs.MailSettings mailSettings)
        {
            _mailSettings = mailSettings;
            _sendGridClient = new SendGridClient(_mailSettings.Password);
        }

        public async Task SendEmailAsync(string to, string subject, string body)
        {
            try
            {
                if (string.IsNullOrEmpty(to)) throw new ArgumentNullException(nameof(to));

                var from = new EmailAddress(_mailSettings.From, _mailSettings.DisplayName);
                var toAddress = new EmailAddress(to);
                var msg = MailHelper.CreateSingleEmail(
                    from,
                    toAddress,
                    subject,
                    body, // Plain text content
                    body  // HTML content (same as body for simplicity; can be customized)
                );

                var response = await _sendGridClient.SendEmailAsync(msg);
                if (response.StatusCode != System.Net.HttpStatusCode.Accepted &&
                    response.StatusCode != System.Net.HttpStatusCode.OK)
                {
                    throw new Exception($"Failed to send email: {response.StatusCode}");
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
            }
        }

        public async Task SendHtmlEmailAsync(string to, string subject, string htmlBody)
        {
            try
            {
                if (string.IsNullOrEmpty(to)) throw new ArgumentNullException(nameof(to));

                var from = new EmailAddress(_mailSettings.From, _mailSettings.DisplayName);
                var toAddress = new EmailAddress(to);

                // Plain text fallback in case the client does not support HTML
                var plainTextFallback = "Please view this email in an HTML-compatible email client.";

                var msg = MailHelper.CreateSingleEmail(
                    from,
                    toAddress,
                    subject,
                    plainTextFallback,  // Plain text content
                    htmlBody            // HTML content
                );

                var response = await _sendGridClient.SendEmailAsync(msg);
                if (response.StatusCode != System.Net.HttpStatusCode.Accepted &&
                    response.StatusCode != System.Net.HttpStatusCode.OK)
                {
                    Console.WriteLine($"Failed to send HTML email: {response.StatusCode}");
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e);

            }
        }


        public async Task SendEmailWithAttachmentAsync(string to, string subject, string body, string attachmentPath)
        {
            try
            {
                if (string.IsNullOrEmpty(to)) throw new ArgumentNullException(nameof(to));
                if (string.IsNullOrEmpty(attachmentPath) || !File.Exists(attachmentPath))
                    throw new ArgumentException("Invalid attachment path", nameof(attachmentPath));

                var from = new EmailAddress(_mailSettings.From, _mailSettings.DisplayName);
                var toAddress = new EmailAddress(to);
                var msg = MailHelper.CreateSingleEmail(
                    from,
                    toAddress,
                    subject,
                    body,
                    body
                );

                // Add attachment
                var fileBytes = File.ReadAllBytes(attachmentPath);
                var fileBase64 = Convert.ToBase64String(fileBytes);
                var fileName = Path.GetFileName(attachmentPath);
                var mimeType = GetMimeType(attachmentPath);

                msg.AddAttachment(new Attachment
                {
                    Content = fileBase64,
                    Filename = fileName,
                    Type = mimeType,
                    Disposition = "attachment"
                });

                var response = await _sendGridClient.SendEmailAsync(msg);
                if (response.StatusCode != System.Net.HttpStatusCode.Accepted &&
                    response.StatusCode != System.Net.HttpStatusCode.OK)
                {
                    throw new Exception($"Failed to send email with attachment: {response.StatusCode}");
                }
            }
            catch (Exception e)
            {

            }
        }

        private static string GetMimeType(string filePath)
        {
            // Same MIME type lookup as original
            return Path.GetExtension(filePath).ToLowerInvariant() switch
            {
                ".pdf" => "application/pdf",
                ".doc" => "application/msword",
                ".docx" => "application/vnd.openxmlformats-officedocument.wordprocessingml.document",
                ".jpg" => "image/jpeg",
                ".png" => "image/png",
                ".txt" => "text/plain",
                _ => "application/octet-stream"
            };
        }

        public void Dispose()
        {
            if (_disposed) return;
            _disposed = true;
        }

        public async ValueTask DisposeAsync()
        {
            if (_disposed) return;
            _disposed = true;
            await Task.CompletedTask;
        }
    }
}