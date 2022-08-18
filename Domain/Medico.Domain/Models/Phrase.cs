using System;
using System.Collections.Generic;

namespace Medico.Domain.Models
{
    public class Phrase : Entity
    {
        public bool IsActive { get; set; }

        public Guid CompanyId { get; set; }

        public Company Company { get; set; }

        public string Name { get; set; }

        public string Title { get; set; }

        public string Content { get; set; }

        public string ContentWithDefaultSelectableItemsValues { get; set; }

        public List<PhraseUsageLocation> PhraseUsageLocations { get; set; }
        public Guid PhraseCategoryId { get; set; }
    }
}