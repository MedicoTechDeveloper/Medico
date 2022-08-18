using AutoMapper;
using AutoMapper.QueryableExtensions;
using Medico.Application.Interfaces;
using Medico.Application.ViewModels;
using Medico.Domain.Interfaces;
using Medico.Domain.Models;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;


namespace Medico.Application.Services
{
    public class PreAuthDataService : BaseDeletableByIdService<PreAuthData, PreAuthDataViewModel>,
        IPreAuthDataService
    {
        #region DI
        public PreAuthDataService(IPreAuthDataRepository repository, IMapper mapper)
            : base(repository, mapper)
        {
        }
        #endregion

        #region Methods
        public IQueryable<PreAuthDataProjectionViewModel> GetAll()
        {
            return Repository.GetAll()
                .ProjectTo<PreAuthDataProjectionViewModel>();
        }

        /*public IQueryable<LookupViewModel> GetAllForLookup(DxOptionsViewModel dxOptions, int lookupItemsCount)
        {
            dxOptions.PrimaryKey = new[] { "Id" };
            dxOptions.PaginateViaPrimaryKey = true;

            var takeItemsCount = dxOptions.Take;
            dxOptions.Take = takeItemsCount != 0 ? takeItemsCount : lookupItemsCount;

            var query = Repository.GetAll();

            var filters = dxOptions.Filter;

            if (filters == null)
                return query.ProjectTo<LookupViewModel>();

            var filter = filters[0] as JArray;

            if (filter == null)
            {
                var id = filters[0] as string;
                if (string.IsNullOrEmpty(id) || id.ToUpperInvariant() != "ID" || filters.Count != 2)
                    return query.ProjectTo<LookupViewModel>();

                var idValue = filters[1] as string;
                if (string.IsNullOrEmpty(idValue))
                    return query.ProjectTo<LookupViewModel>();

                var idGuid = Guid.Parse(idValue);
                return query.Where(mn => mn.Id == idGuid).ProjectTo<LookupViewModel>();
            }

            var searchString = filter[2] == null
                ? string.Empty
                : ((JValue)filter[2]).Value.ToString();

            var isSearchMedicationStringExist = dxOptions.Filter != null && !string.IsNullOrEmpty(searchString);
            if (!isSearchMedicationStringExist)
                return query.ProjectTo<LookupViewModel>();

            dxOptions.Filter = null;

            return query.Where(c => EF.Functions.Contains(c.Name, $"\"{searchString}\""))
                .ProjectTo<LookupViewModel>();
        }*/

        public Task Delete(Guid id)
        {
            return DeleteById(id);
        }

        public async Task<PreAuthDataViewModel> GetByAppointmentId(Guid appointmentId)
        {
            var preAuthData = await Repository.GetAll()
                .Where(th => th.AppointmentId == appointmentId)
                .ProjectTo<PreAuthDataViewModel>()
                .ToListAsync();
            return preAuthData.Count() > 0 ? preAuthData.FirstOrDefault()  : new PreAuthDataViewModel();
        }

        public async Task<PreAuthDataViewModel> GetByCompanyId(Guid companyId)
        {
            var preAuthData = await Repository.GetAll()
                .Where(th => th.CompanyId == companyId)
                .ProjectTo<PreAuthDataViewModel>()
                .ToListAsync();
            return preAuthData.Count() > 0 ? preAuthData.FirstOrDefault() : new PreAuthDataViewModel();
        }

        #endregion
    }
}
