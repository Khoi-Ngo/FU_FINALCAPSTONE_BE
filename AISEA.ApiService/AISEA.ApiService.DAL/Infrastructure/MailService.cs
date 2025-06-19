using System.Net.Mail;
using Microsoft.Extensions.Options;
using AISEA.ApiService.SHARED.Interfaces;
using AISEA.ApiService.SHARED.PropConfigs;

namespace AISEA.ApiService.DAL.Infrastructure
{
    public class MailService : IMailService, IDisposable
    {
        private readonly SmtpClient _smtpClient;
        private readonly MailSettings _mailSettings;
        private bool _disposed;

        public MailService(MailSettings mailSettings)
        {
            _mailSettings = mailSettings;
            _smtpClient = CreateSmtpClient();
        }

        public async Task SendEmailAsync(string to, string subject, string body)
        {
            if (string.IsNullOrEmpty(to)) throw new ArgumentNullException(nameof(to));

            using var mailMessage = CreateMailMessage(to, subject, body);
            await _smtpClient.SendMailAsync(mailMessage);
        }

        public async Task SendEmailWithAttachmentAsync(string to, string subject, string body, string attachmentPath)
        {
            if (string.IsNullOrEmpty(to)) throw new ArgumentNullException(nameof(to));
            if (string.IsNullOrEmpty(attachmentPath) || !File.Exists(attachmentPath))
                throw new ArgumentException("Invalid attachment path", nameof(attachmentPath));

            using var mailMessage = CreateMailMessage(to, subject, body);

            string mimeType = GetMimeType(attachmentPath);
            using var attachment = new Attachment(attachmentPath, mimeType)
            {
                ContentDisposition =
                {
                    FileName = Path.GetFileName(attachmentPath),
                    Size = new FileInfo(attachmentPath).Length
                }
            };

            mailMessage.Attachments.Add(attachment);
            await _smtpClient.SendMailAsync(mailMessage);
        }

        private MailMessage CreateMailMessage(string to, string subject, string body)
        {
            return new MailMessage
            {
                From = new MailAddress(_mailSettings.From, _mailSettings.DisplayName),
                Subject = subject,
                Body = body,
                IsBodyHtml = true,
                To = { to }
            };
        }

        private SmtpClient CreateSmtpClient()
        {
            return new SmtpClient
            {
                Host = _mailSettings.SmtpHost,
                Port = _mailSettings.SmtpPort,
                Credentials = new System.Net.NetworkCredential(_mailSettings.UserName, _mailSettings.Password),
                EnableSsl = true,
                DeliveryMethod = SmtpDeliveryMethod.Network,
                Timeout = 20000,
            };
        }


        private static string GetMimeType(string filePath)
        {
            // Simplified MIME type lookup for common file types
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

            _smtpClient?.Dispose();
            _disposed = true;
        }

        public async ValueTask DisposeAsync()
        {
            if (_disposed) return;

            if (_smtpClient is IAsyncDisposable asyncDisposable)
            {
                await asyncDisposable.DisposeAsync();
            }
            else
            {
                _smtpClient?.Dispose();
            }

            _disposed = true;
        }
    }
}