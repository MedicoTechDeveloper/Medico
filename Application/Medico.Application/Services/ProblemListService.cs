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
    public class ProblemListService : BaseDeletableByIdService<ProblemList, ProblemListViewModel>,
        IProblemListService
    {
        public ProblemListService(
            IProblemListRepository repository, IMapper mapper)
              : base(repository, mapper)
        {
        }

        public Task Delete(Guid id)
        {
            throw new NotImplementedException();
        }

        public async Task<IEnumerable<ProblemListViewModel>> GetByAppointmentId(Guid appointmentId)
        {
            var problemList = await Repository.GetAll()
               .Where(th => th.AppointmentId == appointmentId)
               .ProjectTo<ProblemListViewModel>()
               .ToListAsync();

            return problemList;
        }
    }
}
