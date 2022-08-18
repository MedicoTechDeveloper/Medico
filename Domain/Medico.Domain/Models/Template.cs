using System;
using System.Collections.Generic;
using System.Linq;

namespace Medico.Domain.Models
{
    public class Template : Entity
    {
        public Template()
        {
            TargetTemplates = new List<DependentTemplate>();
            SourceTemplates = new List<DependentTemplate>();
        }

        public Guid? LibraryTemplateId { get; set; }

        public Template LibraryTemplate { get; set; }

        public int? Version { get; set; }

        public bool IsActive { get; set; }

        public Guid? CompanyId { get; set; }

        public Company Company { get; set; }

        public int? TemplateOrder { get; set; }

        public string Title { get; set; }

        public string ReportTitle { get; set; }

        public string DetailedTemplateHtml { get; set; }

        public string InitialDetailedTemplateHtml { get; set; }

        public string DefaultTemplateHtml { get; set; }

        public bool IsRequired { get; set; }

        public bool IsHistorical { get; set; }

        public Guid TemplateTypeId { get; set; }

        public TemplateType TemplateType { get; set; }

        public List<ChiefComplaintTemplate> ChiefComplaintTemplates { get; set; }

        public List<Template> LibraryRelatedTemplates { get; set; }

        public List<TemplateSelectableList> TemplateSelectableLists { get; set; }

        public List<TemplateExpression> TemplateExpressions { get; set; }

        public List<DependentTemplate> TargetTemplates { get; }
        
        public List<DependentTemplate> SourceTemplates { get; }

        public void AddDependentTemplates(IEnumerable<DependentTemplate> dependentTemplates)
        {
            var removedTemplates = TargetTemplates
                .Where(t => dependentTemplates
                    .FirstOrDefault(dt => dt.TargetTemplateId == t.TargetTemplateId) == null)
                .ToList();

            var newTemplates = dependentTemplates
                .Where(t => TargetTemplates
                    .FirstOrDefault(dt => dt.TargetTemplateId == t.TargetTemplateId) == null)
                .ToList();

            if (removedTemplates.Any())
            {
                foreach (var removedTemplate in removedTemplates)
                {
                    TargetTemplates.Remove(removedTemplate);
                }
            }

            if (newTemplates.Any())
                TargetTemplates.AddRange(newTemplates);
        }
    }
}