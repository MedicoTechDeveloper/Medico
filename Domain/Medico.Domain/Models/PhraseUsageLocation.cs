using System;

namespace Medico.Domain.Models
{
    public abstract class PhraseUsageLocation : Entity
    {
        public PhraseUsageLocationType Type { get; set; }
        
        public Phrase Phrase { get; set; }
    }

    public class PatientChartNodeItem : PhraseUsageLocation
    {
        public Guid? DocumentId { get; set; }

        public Guid? NodeId { get; set; }
    }

    public class TemplateItem : PhraseUsageLocation
    {
        public Guid? TemplateId { get; set; }
    }

    public class TemplateItem1
    {
        public string Id { get; set; }
        public string TemplateId { get; set; }
        public string Name { get; set; }
    }
}