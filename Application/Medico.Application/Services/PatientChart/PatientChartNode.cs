using System;
using System.Collections.Generic;
using Medico.Application.ViewModels.Template;

namespace Medico.Application.Services.PatientChart
{
    public class PatientChartNode
    {
        public Guid Id { get; set; }

        public string Name { get; set; }

        public string Title { get; set; }

        public PatientChartNodeType Type { get; set; }

        public dynamic Value { get; set; }

        public PatientChartNodeAttributes Attributes { get; set; }

        public List<PatientChartNode> Children { get; set; }

        public Guid ParentId { get; set; }

        public string Template { get; set; }

        public static PatientChartNode CreatePatientChartTemplateNode(Guid id,
            Guid parentId, TemplateVm template, string templateTypeName)
        {
            var templateOrder = template.TemplateOrder;
            var templateId = template.Id;

            return new PatientChartNode
            {
                Id = id,
                Name = template.ReportTitle,
                Title = template.ReportTitle,
                Type = PatientChartNodeType.TemplateNode,
                Value = new
                {
                    DefaultTemplateHtml = template.DefaultTemplateHtml,
                    DetailedTemplateHtml = template.InitialDetailedTemplateHtml,
                    IsDetailedTemplateUsed = string.IsNullOrEmpty(template.DefaultTemplateHtml)
                },
                ParentId = parentId,
                Template = PatientChartNodeTemplates
                    .GetTemplateValueForPatientChartTemplateNode(templateId, templateTypeName),
                Attributes =
                    PatientChartNodeAttributes.CreatePatientChartNodeAttributes(templateOrder,
                        true, false, false, false, new NodeSpecificAttributes {TemplateId = templateId})
            };
        }
    }
}