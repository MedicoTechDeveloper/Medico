using System;

namespace Medico.Domain.Models
{
    public class DependentTemplate
    {
        public Guid SourceTemplateId { get; set; }

        public Template SourceTemplate { get; set; }

        public Guid TargetTemplateId { get; set; }

        public Template TargetTemplate { get; set; }
    }
}