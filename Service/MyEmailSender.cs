using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Mail;
using System.Net;
using System.Web;
using pbl3_QLCF.Service;

namespace pbl3_QLCF.Service
{
    public class MyEmailSender : IMyEmailSender
    {
        private readonly string _email;
        private readonly string _password;

        public MyEmailSender(string email, string password)
        {
            _email = email;
            _password = password;
        }

        public async Task SendEmailAsync(string email, string subject, string message)
        {
            using (var client = new SmtpClient("smtp.gmail.com", 587)
            {
                EnableSsl = true,
                Credentials = new NetworkCredential(_email, _password),
                DeliveryMethod = SmtpDeliveryMethod.Network
            })
            {
                using (var mailMessage = new MailMessage
                {
                    From = new MailAddress(_email),
                    Subject = subject,
                    Body = message,
                    IsBodyHtml = true,
                })
                {
                    mailMessage.To.Add(email);
                    await client.SendMailAsync(mailMessage);
                }
            }
        }
    }
}