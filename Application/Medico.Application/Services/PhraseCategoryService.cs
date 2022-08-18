using AutoMapper;
using AutoMapper.QueryableExtensions;
using Medico.Application.Interfaces;
using Medico.Application.ViewModels.Phrase;
using Medico.Domain.Interfaces;
using Medico.Domain.Models;
using Microsoft.EntityFrameworkCore;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace Medico.Application.Services
{
    public class PhraseCategoryService : BaseDeletableByIdService<PhraseCategory, PhraseCategoryViewModel>, IPhraseCategoryService
    {
        public PhraseCategoryService(IPhraseCategoryRepository repository,
            IMapper mapper)
            : base(repository, mapper)
        {

        }

        public IQueryable<PhraseCategoryViewModel> GetAll()
        {
            return Repository.GetAll()
                .ProjectTo<PhraseCategoryViewModel>();
        }

        public  async Task<PhraseCategoryViewModel> GetCatById(Guid id)
        {
            var phrase = await Repository.GetAll()
                .FirstOrDefaultAsync(t => t.Id == id);

            var phraseVm = Mapper.Map<PhraseCategoryViewModel>(phrase);

            return phraseVm;
        }
    }
}
