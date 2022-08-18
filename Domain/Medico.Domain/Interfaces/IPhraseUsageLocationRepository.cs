using Medico.Domain.Models;

namespace Medico.Domain.Interfaces
{
    public interface IPhraseUsageLocationRepository
        : IDeletableByIdRepository<PhraseUsageLocation>, IDeletableByEntityRepository<PhraseUsageLocation>
    {
    }
}