using System;
using System.Linq;
using System.Threading.Tasks;
using Medico.Application.ViewModels;
using Medico.Application.ViewModels.ErrorLog;

namespace Medico.Application.Interfaces
{
    public interface IApiErrorLogRecordService
    {
        Task<ApiErrorLogRecordVm> GetById(Guid id);

        Task<ApiErrorLogRecordVm> Create(ApiErrorLogRecordVm admissionVm);

        IQueryable<ApiErrorLogRecordVm> Grid(CompanyDxOptionsViewModel loadOptions);

        Task Delete(Guid id);
    }
}