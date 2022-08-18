using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using Medico.Application.Interfaces;
using Medico.Domain.Interfaces;
using Medico.Domain.Models;
using Microsoft.EntityFrameworkCore;

namespace Medico.Application.Services
{
    public class TemplateSelectableListService : ITemplateSelectableListService
    {
        private readonly ITemplateSelectableListRepository _templateSelectableListRepository;

        public TemplateSelectableListService(ITemplateSelectableListRepository templateSelectableListRepository)
        {
            _templateSelectableListRepository = templateSelectableListRepository;
        }

        public async Task AddToCompanyFromLibrary(IDictionary<Guid, Guid> templatesMap, IDictionary<Guid, Guid> selectableListsMap)
        {
            var libraryTemplateIds = templatesMap.Keys;
            var libraryTemplateSelectableLists = await _templateSelectableListRepository
                .GetAll()
                .Where(tsl => libraryTemplateIds.Contains(tsl.TemplateId))
                .ToListAsync();

            foreach (var libraryTemplateSelectableList in libraryTemplateSelectableLists)
            {
                var companyTemplateSelectableList = new TemplateSelectableList
                {
                    TemplateId = templatesMap[libraryTemplateSelectableList.TemplateId],
                    SelectableListId = selectableListsMap[libraryTemplateSelectableList.SelectableListId]
                };

                await _templateSelectableListRepository.AddAsync(companyTemplateSelectableList);
            }
        }
    }
}
