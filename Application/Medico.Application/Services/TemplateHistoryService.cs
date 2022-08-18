using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Medico.Application.Extensions;
using Medico.Application.Interfaces;
using Medico.Application.Services.PatientChart;
using Medico.Application.ViewModels.TemplateHistory;
using Medico.Domain.Interfaces;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;

namespace Medico.Application.Services
{
    public class TemplateHistoryService : ITemplateHistoryService
    {
        private readonly IAdmissionRepository _admissionRepository;
        private readonly ITemplateService _templateService;

        public TemplateHistoryService(IAdmissionRepository admissionRepository,
            ITemplateService templateService)
        {
            _admissionRepository = admissionRepository;
            _templateService = templateService;
        }

        public async Task<List<TemplateHistoryVm>> GetPreviousTemplateContent(Guid admissionId, Guid templateId,
            Guid patientId, Guid documentId)
        {
            var template = await _templateService.GetById(templateId);
            if (template != null && !template.IsHistorical)
                return new List<TemplateHistoryVm>();

            var patientAdmissions = await _admissionRepository
                .GetAll()
                .Include(a => a.Appointment)
                .Where(a => a.PatientId == patientId)
                .ToListAsync();

            var specifiedAdmission =
                patientAdmissions.First(a => a.Id == admissionId);

            var previousAdmissionsAccordingSpecified = patientAdmissions
                .Where(a => a.Appointment.StartDate < specifiedAdmission.Appointment.StartDate)
                .OrderByDescending(a => a.Appointment.StartDate)
                .ToList();

            if (!previousAdmissionsAccordingSpecified.Any())
                return new List<TemplateHistoryVm>();

            List<TemplateHistoryVm> TemplateHistoryVmList = new List<TemplateHistoryVm>();
            foreach (var previousAdmission in previousAdmissionsAccordingSpecified)
            {
                var patientChartNode =
                    JsonConvert.DeserializeObject<PatientChartNode>(previousAdmission.AdmissionData);

                var documentNode =
                    patientChartNode.Children.FirstOrDefault(n => n.Id == documentId);

                var previousTemplateNode = documentNode?.FirstOrDefault(n =>
                {
                    var nodeType = n.Type;
                    if (nodeType != PatientChartNodeType.TemplateNode)
                        return false;

                    return n.Attributes.NodeSpecificAttributes.TemplateId == templateId;
                });

                if (previousTemplateNode == null)
                    continue;

                bool isDetailedTemplateUsed =
                    previousTemplateNode.Value.isDetailedTemplateUsed;

                string templateContent = isDetailedTemplateUsed
                    ? previousTemplateNode.Value.detailedTemplateHtml
                    : previousTemplateNode.Value.defaultTemplateHtml;

                TemplateHistoryVmList.Add( TemplateHistoryVm.Create(previousAdmission.Appointment.StartDate, templateContent));
            }

            return TemplateHistoryVmList;
        }
    }
}