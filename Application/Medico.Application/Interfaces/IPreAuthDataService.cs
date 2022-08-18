using Medico.Application.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
namespace Medico.Application.Interfaces
{
    public interface IPreAuthDataService
    {
        Task<PreAuthDataViewModel> Create(PreAuthDataViewModel preAuthDataViewModel);
        Task<PreAuthDataViewModel> Update(PreAuthDataViewModel preAuthDataViewModel);
        IQueryable<PreAuthDataProjectionViewModel> GetAll();
        /*IQueryable<LookupViewModel> GetAllForLookup(DxOptionsViewModel dxOptions, int lookupItemsCount);*/
        Task<PreAuthDataViewModel> GetById(Guid id);
        Task Delete(Guid id);
        Task<PreAuthDataViewModel> GetByAppointmentId(Guid appointmentId);
        Task<PreAuthDataViewModel> GetByCompanyId(Guid companyId);
    }
}
