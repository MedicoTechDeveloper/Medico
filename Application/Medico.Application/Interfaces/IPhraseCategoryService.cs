using Medico.Application.ViewModels.Phrase;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace Medico.Application.Interfaces
{
    public interface IPhraseCategoryService
    {
        Task<PhraseCategoryViewModel> Create(PhraseCategoryViewModel phraseViewModel);

        Task<PhraseCategoryViewModel> Update(PhraseCategoryViewModel phraseViewModel);

        Task<PhraseCategoryViewModel> GetCatById(Guid id);

        IQueryable<PhraseCategoryViewModel> GetAll();
    }
}
