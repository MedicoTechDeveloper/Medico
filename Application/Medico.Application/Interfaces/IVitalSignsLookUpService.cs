using Medico.Application.ViewModels;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace Medico.Application.Interfaces
{
    public  interface IVitalSignsLookUpService
    {
        Task<VitalSignsLookUpViewModel> Create(VitalSignsLookUpViewModel vitalSignsLookUpViewModel);
        Task<VitalSignsLookUpViewModel> Update(VitalSignsLookUpViewModel vitalSignsLookUpViewModel);
        IQueryable<VitalSignsLookUpProjectionViewModel> GetAll();
        IQueryable<LookupViewModel> GetAllForLookup(DxOptionsViewModel dxOptions, int lookupItemsCount);
        Task<VitalSignsLookUpViewModel> GetById(Guid id);
        Task Delete(Guid id);
    }
}
