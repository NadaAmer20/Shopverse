using Microsoft.EntityFrameworkCore;
using Shopverse.Application.Helpers;
using Shopverse.Domain.Abstractions;
using Shopverse.Domain.Entities;
using Shopverse.Domain.Interfaces.Communication;
using System.Net;
using System.Net.Mail;

namespace Shopverse.Infrastructure.Services.Communication
{
    public class SmtpEmailService : IEmailService
    {
        private readonly IRepository<Setting> _settingsRepo;

        public SmtpEmailService(IRepository<Setting> settingsRepo)
        {
            _settingsRepo = settingsRepo;
        }
        public async Task SendEmailAsync(string to, string subject, string body, CancellationToken cancellationToken = default)
        {
            var smtpSettings = await GetSmtpSettingsAsync(cancellationToken);
            if (smtpSettings == null)
                throw new Exception("SMTP settings are not configured.");

            using var client = new SmtpClient(smtpSettings.Host, smtpSettings.Port)
            {
                Credentials = new NetworkCredential(smtpSettings.Username, smtpSettings.Password),
                EnableSsl = smtpSettings.EnableSsl
            };

            var htmlBody = EmailTemplateBuilder.BuildNewsletterHtml(subject, body);

            var msg = new MailMessage(smtpSettings.From, to, subject, htmlBody)
            {
                IsBodyHtml = true
            };

            await client.SendMailAsync(msg, cancellationToken);
        }


        private async Task<SmtpSettings?> GetSmtpSettingsAsync(CancellationToken ct)
        {
            var host = await _settingsRepo.GetQueryable().Where(s => s.Key == "Smtp_Host").Select(s => s.Value).FirstOrDefaultAsync(ct);
            var portStr = await _settingsRepo.GetQueryable().Where(s => s.Key == "Smtp_Port").Select(s => s.Value).FirstOrDefaultAsync(ct);
            var username = await _settingsRepo.GetQueryable().Where(s => s.Key == "Smtp_Username").Select(s => s.Value).FirstOrDefaultAsync(ct);
            var password = await _settingsRepo.GetQueryable().Where(s => s.Key == "Smtp_Password").Select(s => s.Value).FirstOrDefaultAsync(ct);
            var from = await _settingsRepo.GetQueryable().Where(s => s.Key == "Smtp_From").Select(s => s.Value).FirstOrDefaultAsync(ct);
            var enableSslStr = await _settingsRepo.GetQueryable().Where(s => s.Key == "Smtp_EnableSsl").Select(s => s.Value).FirstOrDefaultAsync(ct);

            if (string.IsNullOrWhiteSpace(host) || string.IsNullOrWhiteSpace(portStr) || string.IsNullOrWhiteSpace(username) ||
                string.IsNullOrWhiteSpace(password) || string.IsNullOrWhiteSpace(from) || string.IsNullOrWhiteSpace(enableSslStr))
            {
                return null;
            }

            return new SmtpSettings
            {
                Host = host,
                Port = int.TryParse(portStr, out var port) ? port : 587,
                Username = username,
                Password = password,
                From = from,
                EnableSsl = bool.TryParse(enableSslStr, out var ssl) && ssl
            };
        }
    }
}
