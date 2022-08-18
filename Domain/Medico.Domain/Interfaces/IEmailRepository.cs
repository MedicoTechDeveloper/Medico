using Medico.Domain.Models;

namespace Medico.Domain.Interfaces
{
    public interface IEmailRepository : IDeletableByIdRepository<EmailAccount>
    {
    }
}
