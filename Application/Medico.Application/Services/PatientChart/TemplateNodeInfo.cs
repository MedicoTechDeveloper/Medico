using System;

namespace Medico.Application.Services.PatientChart
{
    public class TemplateNodeInfo
    {
        public Guid Id { get; set; }

        public int? Order { get; set; }

        public string Title { get; set; }

        public Guid SectionId { get; set; }

        public static TemplateNodeInfo CreateNew(Guid id, int? order, string title, Guid sectionId)
        {
            return new TemplateNodeInfo
            {
                Id = id,
                Title = title,
                Order = order,
                SectionId = sectionId
            };
        }
    }
}