using Medico.Domain.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Medico.Domain.Interfaces
{
    public interface IDataParserRepository: IDeletableByIdRepository<DocumentLog>
    {
        Task<string> ProcessDocData(Claimant claimantModel);
        // Task<IEnumerable<DataParserDocument>> GetDocumentsToProcess();
    }
}
