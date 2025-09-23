
using System.Net;
using System.Net.Mail;

namespace StockTradePro.NotificationService.API.Services
{
    public class EmailService : IEmailService
    {
        private readonly IConfiguration _configuration;
        private readonly IUserService _userService;
        private readonly ILogger<EmailService> _logger;

        public EmailService(IConfiguration configuration, IUserService userService, ILogger<EmailService> logger)
        {
            _configuration = configuration;
            _userService = userService;
            _logger = logger;
        }

        public async Task SendNotificationEmailAsync(string userId, string subject, string message)
        {
            try
            {
                var userEmail = await _userService.GetUserEmailAsync(userId);
                if (string.IsNullOrEmpty(userEmail))
                {
                    _logger.LogWarning("No email found for user {UserId}", userId);
                    return;
                }

                await SendEmailAsync(userEmail, subject, message);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to send notification email to user {UserId}", userId);
            }
        }

        public async Task SendBulkNotificationEmailAsync(IEnumerable<string> userIds, string subject, string message)
        {
            var tasks = userIds.Select(userId => SendNotificationEmailAsync(userId, subject, message));
            await Task.WhenAll(tasks);
        }

        public async Task SendEmailAsync(string toEmail, string subject, string message)
        {
            var smtpSettings = _configuration.GetSection("SMTP");

            using var client = new SmtpClient(smtpSettings["Host"], int.Parse(smtpSettings["Port"]))
            {
                Credentials = new NetworkCredential(smtpSettings["Username"], smtpSettings["Password"]),
                EnableSsl = bool.Parse(smtpSettings["EnableSsl"])
            };

            var mailMessage = new MailMessage
            {
                From = new MailAddress(smtpSettings["FromEmail"], smtpSettings["FromName"]),
                Subject = subject,
                Body = GenerateEmailHtml(subject, message),
                IsBodyHtml = true
            };

            mailMessage.To.Add(toEmail);

            await client.SendMailAsync(mailMessage);
            _logger.LogInformation("Email sent successfully to {Email}", toEmail);
        }

        private string GenerateEmailHtml(string subject, string message)
        {
            return $@"
                <!DOCTYPE html>
                <html>
                <head>
                    <style>
                        body {{ font-family: Arial, sans-serif; margin: 0; padding: 20px; background-color: #f4f4f4; }}
                        .container {{ max-width: 600px; margin: 0 auto; background-color: white; padding: 20px; border-radius: 5px; }}
                        .header {{ background-color: #007bff; color: white; padding: 15px; border-radius: 5px 5px 0 0; }}
                        .content {{ padding: 20px; }}
                        .footer {{ text-align: center; color: #666; font-size: 12px; margin-top: 20px; }}
                    </style>
                </head>
                <body>
                    <div class='container'>
                        <div class='header'>
                            <h2>StockTradePro</h2>
                        </div>
                        <div class='content'>
                            <h3>{subject}</h3>
                            <p>{message}</p>
                        </div>
                        <div class='footer'>
                            <p>This is an automated message from StockTradePro. Please do not reply to this email.</p>
                        </div>
                    </div>
                </body>
                </html>";
        }
    }
}