using System;
namespace pbl3_QLCF.Interface
{
    public interface IMyEmailSender
    {
        Task SendEmailAsync(string email, string subject, string message);
    }
}
