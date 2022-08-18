using AutoMapper;
using AutoMapper.QueryableExtensions;
using Medico.Application.Interfaces;
using Medico.Application.ViewModels;
using Medico.Domain.Interfaces;
using Medico.Domain.Models;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Medico.Application.Services
{
    public class SubTaskUserService :  BaseDeletableByIdService<SubTaskUser, SubTaskUserViewModel>,
        ISubTaskUserService
    {
        public SubTaskUserService(ISubTaskUserRepository repository, IMapper mapper)
        : base(repository, mapper)
        {
        }

        public async Task<IEnumerable<SubTaskUserViewModel>> GetByTaskId(Guid id)
        {
            var subTaskViews = await Repository.GetAll()
                .Where(th => th.SubTaskId == id)
                .ProjectTo<SubTaskUserViewModel>()
                .ToListAsync();

            return subTaskViews;
        }
    }
}
