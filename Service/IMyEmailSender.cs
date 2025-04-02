using System;
namespace pbl3_QLCF.Service
{
    public interface IMyEmailSender
    {
        Task SendEmailAsync(string email, string subject, string message);
    }
}
