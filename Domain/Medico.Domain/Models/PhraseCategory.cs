using System.Collections.Generic;

namespace Medico.Domain.Models
{
    public class PhraseCategory : Entity
    {
        public string CategoryName { get; set; }
        public bool IsActive { get; set; }
        public List<Phrase> Phrases { get; set; }
    }
}
