using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Medico.Application.ViewModels;
using Medico.Application.ViewModels.PhraseUsage;

namespace Medico.Application.Interfaces
{
    public interface IPhraseUsageService
    {
        Task<PhraseUsageVm> Update(PhraseUsageVm phraseUsageVm);

        Task<IEnumerable<PhraseUsageReadVm>> Grid(CompanyDxOptionsViewModel loadOptions);
    }
}