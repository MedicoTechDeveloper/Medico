using Medico.Application.ViewModels;
using Medico.Application.ViewModels.Document;
using Medico.Domain.Models;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Medico.Application.Interfaces
{
    public interface IDataParserService
    {
        Task<string> ProcessDocData(ClaimantModel claimantModel, string companyId);
        Task<IEnumerable<DocumentProjectionViewModel>> DocumentGrid(DocumentDxOptionsViewModel loadOptions);
        int AddDocuments(IEnumerable<DocumentLog> documents);
        Task<int> AddDocuments(IEnumerable<PhysicianDocLog> physicianDocuments);
        Task UpdateDocument(DocumentLog documentLog);
        IEnumerable<PhysicianDocLog> GetPhysicianDocs();
        int DeleteDocument(DocumentLog document);
        int DeletePhysicianDocument(Guid id);
    }
}
