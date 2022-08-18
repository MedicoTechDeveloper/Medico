using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Medico.Application.ViewModels;
using Medico.Application.ViewModels.Phrase;

namespace Medico.Application.Interfaces
{
    public interface IPhraseService
    {
        IQueryable<PhraseVm> GetAll();

        Task<PhraseVm> GetById(Guid id);

        Task<PhraseVm> GetByName(string name, Guid companyId);

        Task<PhraseVm> Create(PhraseVm phraseViewModel);

        Task<PhraseVm> Update(PhraseVm phraseViewModel);

        Task Delete(Guid id);

        Task<IEnumerable<LookupViewModel>> Lookup(PhraseDxOptionsViewModel loadOptions);

        IQueryable<PhraseVm> Grid(CompanyDxOptionsViewModel loadOptions);
    }
}