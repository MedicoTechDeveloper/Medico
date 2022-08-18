using Medico.Application.ViewModels;
using System.Threading.Tasks;

namespace Medico.Application.Interfaces
{
    public interface ISendEmailService
    {
        Task SendEmailAsync (string toEmail, string subject, string message);
        Task<EmailAccountViewModel> GetEmailAccount();
        Task Execute(EmailViewModel email, EmailAccountViewModel emailAccount);
    }
}
