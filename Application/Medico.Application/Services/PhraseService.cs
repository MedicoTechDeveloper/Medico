using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using AutoMapper.QueryableExtensions;
using Dapper;
using Medico.Application.Interfaces;
using Medico.Application.SelectableItemsManagement;
using Medico.Application.ViewModels;
using Medico.Application.ViewModels.Phrase;
using Medico.Domain.Interfaces;
using Medico.Domain.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;

namespace Medico.Application.Services
{
    public class PhraseService : BaseDeletableByIdService<Phrase, PhraseVm>, IPhraseService
    {
        private readonly ISelectableItemsService _selectableItemsService;
        private readonly IConfiguration _configuration;
        public IDbConnection Connection => new SqlConnection(_configuration.GetConnectionString("DefaultConnection"));

        public PhraseService(IPhraseRepository repository,
            IConfiguration configuration,
            IMapper mapper,
            ISelectableItemsService selectableItemsService)
            : base(repository, mapper)
        {
            _selectableItemsService = selectableItemsService;
            _configuration = configuration;
        }

        public override async Task<PhraseVm> GetById(Guid id)
        {
            /*var phrase = await Repository.GetAll()
                .FirstOrDefaultAsync(t => t.Id == id);*/

            using (IDbConnection con = Connection)
            {
                con.Open();

                string q = @"select * from [dbo].[Phrase] where Id=@id";

                var phrase = await con.QuerySingleAsync(q,
                        new
                        {
                            id
                        });

                var phraseVm = Mapper.Map<PhraseVm>(phrase);

                var phraseContent = phraseVm.Content;

                if (!string.IsNullOrEmpty(phraseContent))
                {
                    phraseVm.ContentWithDefaultSelectableItemsValues = _selectableItemsService
                        .SetInitialValues(phraseContent);
                }

                return phraseVm;
            }
        }

        public IQueryable<PhraseVm> GetAll()
        {
            return Repository.GetAll()
                .ProjectTo<PhraseVm>();
        }

        public async Task<PhraseVm> GetByName(string name, Guid companyId)
        {
            var phrase = await Repository.GetAll()
                .FirstOrDefaultAsync(p => p.Name == name && p.CompanyId == companyId);

            return phrase == null
                ? null
                : Mapper.Map<PhraseVm>(phrase);
        }

        public Task Delete(Guid id)
        {
            return DeleteById(id);
        }

        public async Task<IEnumerable<LookupViewModel>> Lookup(PhraseDxOptionsViewModel loadOptions)
        {
            var companyId = loadOptions.CompanyId;
            if (companyId == Guid.Empty)
                return Enumerable.Empty<LookupViewModel>();

            var query = Repository.GetAll()
                .Include(p => p.PhraseUsageLocations)
                .Where(p => p.CompanyId == companyId && p.IsActive);

            var patientChartNodeId = loadOptions.PatientChartNodeId;
            if (patientChartNodeId != null)
                query = query
                    .Where(p => p.PhraseUsageLocations
                        .OfType<PatientChartNodeItem>()
                        .FirstOrDefault(u => u.NodeId == patientChartNodeId.Value) != null);

            var templateId = loadOptions.TemplateId;
            if (templateId != null)
            {
                if (patientChartNodeId != null)
                    throw new InvalidOperationException(
                        "Either template id or patient chart node is should be set in phase load options");
                query = query
                    .Where(p => p.PhraseUsageLocations
                        .OfType<TemplateItem>()
                        .FirstOrDefault(u => u.TemplateId == templateId.Value) != null);
            }

            var queryResult = await query
                .ProjectTo<LookupViewModel>()
                .ToListAsync();

            return queryResult.Any()
                ? queryResult
                : await Repository.GetAll()
                    .Where(p => p.CompanyId == companyId && p.IsActive).ProjectTo<LookupViewModel>()
                    .ToListAsync();
        }

        public IQueryable<PhraseVm> Grid(CompanyDxOptionsViewModel loadOptions)
        {
            var companyId = loadOptions.CompanyId;
            if (companyId == Guid.Empty)
                return Enumerable.Empty<PhraseVm>().AsQueryable();

            return Repository.GetAll()
                .Where(p => p.CompanyId == companyId)
                .ProjectTo<PhraseVm>();
        }
    }
}