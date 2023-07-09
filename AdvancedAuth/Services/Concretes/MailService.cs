using AdvancedAuth.Services.Abstracts;
using System.Net.Mail;
using System.Net;
using System.Text;

namespace AdvancedAuth.Services.Concretes
{
    public class MailService : IMailService
    {
        private readonly IConfiguration _configuration;
        public MailService(IConfiguration configuration) => _configuration = configuration;

        public async Task SendMailAsync(string to, string subject, string body, bool isBodyHtml = true)
        {
            MailMessage mail = new();
            mail.IsBodyHtml = isBodyHtml;
            mail.Subject = subject;
            mail.Body = body;
            mail.To.Add(to);
            mail.From = new(_configuration["Mail:Username"], "Authentication App", Encoding.UTF8);

            //Send this mail
            SmtpClient smtpClient = new();
            smtpClient.Credentials = new NetworkCredential(_configuration["Mail:Username"], _configuration["Mail:Password"]);
            smtpClient.Port = 587;
            smtpClient.EnableSsl = true;
            smtpClient.Host = _configuration["Mail:Host"];

            await smtpClient.SendMailAsync(mail);
        }

        public async Task SendPasswordResetMailAsync(string to, string userId, string resetToken)
        {
            StringBuilder mail = new();
            mail.Append("Hello<br>If you have requested a new password, you can renew your password from the link below.<br><strong> <a target=\"_blank\" href=\"");

            mail.Append(_configuration["Url:Backend"]);
            mail.Append("/password/resetPassword/");
            mail.Append(userId);
            mail.Append("/");
            mail.Append(resetToken);
            mail.AppendLine("\">Click for new password request...</a></strong><br><br><span style=\"font-size:12px;\">NOTE : If this request has not been fulfilled by you, please do not take this mail seriously.</span><br>Regards...<br><br><br>Authentication App Email Support");

            await SendMailAsync(to, "Password Renewal Request", mail.ToString());
        }
    }
}
