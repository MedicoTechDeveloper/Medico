using System.Collections.Generic;
using System.Linq;

namespace Medico.Application.SelectableItemsManagement
{
    public class VariablesUniquenessCheckResultVm
    {
        public VariablesUniquenessCheckResultVm()
        {
            NonUniqueVariableNames = new List<string>();
        }

        public bool DoNonUniqueVariablesExist =>
            NonUniqueVariableNames.Any();
        
        public List<string> NonUniqueVariableNames { get; private set; }
    }
}