using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Medico.Application.ViewModels.TemplateHistory;

namespace Medico.Application.Interfaces
{
    public interface ITemplateHistoryService
    {
        Task<List<TemplateHistoryVm>> GetPreviousTemplateContent(Guid admissionId,
            Guid templateId, Guid patientId, Guid documentId);
    }
}