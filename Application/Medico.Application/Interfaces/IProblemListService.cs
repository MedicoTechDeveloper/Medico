using Medico.Application.ViewModels;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Medico.Application.Interfaces
{
    public interface IProblemListService
    {
        Task<ProblemListViewModel> GetById(Guid id);

        Task<ProblemListViewModel> Create(ProblemListViewModel vitalSignsViewModel);

        Task<ProblemListViewModel> Update(ProblemListViewModel vitalSignsViewModel);

        Task Delete(Guid id);

        Task<IEnumerable<ProblemListViewModel>> GetByAppointmentId(Guid appointmentId);
    }
}
