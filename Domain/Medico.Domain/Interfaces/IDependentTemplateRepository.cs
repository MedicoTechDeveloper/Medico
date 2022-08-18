using Medico.Domain.Models;

namespace Medico.Domain.Interfaces
{
    public interface IDependentTemplateRepository
        : IDeletableByIdRepository<DependentTemplate>
    {
    }
}