using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Medico.Application.Interfaces;
using Medico.Application.Services.PatientChart;
using Medico.Application.ViewModels;
using Medico.Application.ViewModels.PhraseUsage;
using Medico.Domain.Interfaces;
using Medico.Domain.Models;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;

namespace Medico.Application.Services
{
    public class PhraseUsageService : IPhraseUsageService
    {
        private readonly IPhraseRepository _phraseRepository;
        private readonly ITemplateRepository _templateRepository;
        private readonly IPatientChartDocumentNodeRepository _patientChartDocumentNodeRepository;
        private readonly IPatientChartNodeManagementService _patientChartNodeManagementService;

        public PhraseUsageService(IPhraseRepository phraseRepository,
            ITemplateRepository templateRepository,
            IPatientChartDocumentNodeRepository patientChartDocumentNodeRepository,
            IPatientChartNodeManagementService patientChartNodeManagementService)
        {
            _phraseRepository = phraseRepository;
            _templateRepository = templateRepository;
            _patientChartDocumentNodeRepository = patientChartDocumentNodeRepository;
            _patientChartNodeManagementService = patientChartNodeManagementService;
        }

        public async Task<PhraseUsageVm> Update(PhraseUsageVm phraseUsageVm)
        {
            var phrase = await _phraseRepository.GetAll()
                .Include(p => p.PhraseUsageLocations)
                .FirstOrDefaultAsync(p => p.Id == phraseUsageVm.PhraseId);

            var oldPhraseUsageLocations = phrase.PhraseUsageLocations;

            IEnumerable<PhraseUsageLocation> patientChartNodeUsePhrases =
                phraseUsageVm.PatientChartNodeUsePhrases == null
                    ? Enumerable.Empty<PatientChartNodeItem>()
                    : phraseUsageVm.PatientChartNodeUsePhrases
                        .Select(p => new PatientChartNodeItem
                        {
                            Id = p.Id,
                            DocumentId = p.DocumentId,
                            NodeId = p.PatientChartNodeId
                        });

            IEnumerable<PhraseUsageLocation> templateUsePhrases = phraseUsageVm.TemplateUsePhrases == null
                ? Enumerable.Empty<TemplateItem>()
                : phraseUsageVm.TemplateUsePhrases
                    .Select(t => new TemplateItem
                    {
                        Id = t.Id,
                        TemplateId = t.TemplateId
                    });

            var phraseUsageLocations =
                patientChartNodeUsePhrases.Concat(templateUsePhrases);

            var removedPhraseUsageLocations = oldPhraseUsageLocations
                .Where(oldPhraseUsageLocation => phraseUsageLocations
                                                     .FirstOrDefault(newPhraseUsageLocation =>
                                                         newPhraseUsageLocation.Id == oldPhraseUsageLocation.Id) ==
                                                 null).ToList();

            var newPhraseUsageLocations = phraseUsageLocations
                .Where(newPhraseUsageLocation => oldPhraseUsageLocations
                                                     .FirstOrDefault(oldPhraseUsageLocation =>
                                                         oldPhraseUsageLocation.Id == newPhraseUsageLocation.Id) ==
                                                 null).ToList();

            if (removedPhraseUsageLocations.Any())
            {
                foreach (var removedUsageLocation in removedPhraseUsageLocations)
                {
                    oldPhraseUsageLocations.Remove(removedUsageLocation);
                }
            }

            if (newPhraseUsageLocations.Any())
                oldPhraseUsageLocations.AddRange(newPhraseUsageLocations);

            await _phraseRepository.SaveChangesAsync();

            return phraseUsageVm;
        }

        public async Task<IEnumerable<PhraseUsageReadVm>> Grid(CompanyDxOptionsViewModel loadOptions)
        {
            var companyId = loadOptions.CompanyId;
            if (companyId == Guid.Empty)
                return Enumerable.Empty<PhraseUsageReadVm>().AsQueryable();

            var phrases = await _phraseRepository.GetAll()
                .Include(p => p.PhraseUsageLocations)
                .Where(phrase => phrase.CompanyId == companyId)
                .ToListAsync();

            if (!phrases.Any())
                return Enumerable.Empty<PhraseUsageReadVm>().AsQueryable();

            return phrases.Select(phrase =>
            {
                var phraseUsage = new PhraseUsageReadVm
                {
                    PhraseId = phrase.Id,
                    PhraseName = phrase.Title,
                };

                var phraseUsageLocations = phrase.PhraseUsageLocations;

                if (phraseUsageLocations == null || !phraseUsageLocations.Any())
                {
                    phraseUsage.TemplateUsePhrases =
                        Enumerable.Empty<TemplateUsePhraseReadVm>().ToList();

                    phraseUsage.PatientChartNodeUsePhrases =
                        Enumerable.Empty<PatientChartNodeUsePhraseReadVm>().ToList();

                    return phraseUsage;
                }

                var templatesUsePhrases = phraseUsageLocations
                    .Where(phraseUsageLocation => phraseUsageLocation is TemplateItem)
                    .Join(_templateRepository.GetAll(),
                        phraseLocation => ((TemplateItem) phraseLocation).TemplateId,
                        template => template.Id, (phraseLocation, template) => new TemplateUsePhraseReadVm
                        {
                            Id = phraseLocation.Id,
                            TemplateName = template.ReportTitle,
                            TemplateId = template.Id
                        }).ToList();

                phraseUsage.TemplateUsePhrases = templatesUsePhrases;

                var patientChartNodeUsePhrases = phraseUsageLocations
                    .Where(phraseUsageLocation => phraseUsageLocation is PatientChartNodeItem)
                    .Join(_patientChartDocumentNodeRepository.GetAll(),
                        phraseLocation => ((PatientChartNodeItem) phraseLocation).DocumentId,
                        documentNode => documentNode.Id, (phraseLocation, documentNode) =>
                        {
                            var nodeId = ((PatientChartNodeItem) phraseLocation).NodeId;
                            if (nodeId == null)
                                throw new NullReferenceException(nameof(nodeId));

                            return new PatientChartNodeUsePhraseReadVm
                            {
                                Id = phraseLocation.Id,
                                DocumentName = documentNode.Title,
                                DocumentId = documentNode.Id,
                                PatientChartNodeId = nodeId.Value,
                                PatientChartNodePath =
                                    _patientChartNodeManagementService
                                        .SetPatientChartRootNode(
                                            JsonConvert.DeserializeObject<PatientChartNode>(documentNode
                                                .PatientChartDocumentNodeJsonString))
                                        .GetFullPathNameToChildNode(nodeId.Value)
                            };
                        }).Where(patientChartNode => !string.IsNullOrEmpty(patientChartNode.PatientChartNodePath))
                    .ToList();

                phraseUsage.PatientChartNodeUsePhrases = patientChartNodeUsePhrases;

                return phraseUsage;
            });
        }
    }
}