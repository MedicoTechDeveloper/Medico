using Medico.Application.ViewModels;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace Medico.Application.Interfaces
{
    public interface IInsuranceService
    {
        Task<InsuranceCompanyViewModel> Create(InsuranceCompanyViewModel alcoholHistoryViewModel);

        Task<InsuranceCompanyViewModel> Update(InsuranceCompanyViewModel alcoholHistoryViewModel);

        IQueryable<InsuranceCompanyProjectionViewModel> GetAllCompanies();

        IQueryable<LookupViewModel> GetAllCompaniesForLookup(DxOptionsViewModel dxOptions, int lookupItemsCount);
        Task<InsuranceCompanyViewModel> GetById(Guid id);
        Task DeleteCompany(Guid id);
    }
}
