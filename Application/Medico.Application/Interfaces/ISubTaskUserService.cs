using Medico.Application.ViewModels;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Medico.Application.Interfaces
{
    public interface ISubTaskUserService
    {
        Task<SubTaskUserViewModel> Create(SubTaskUserViewModel subTaskUserViewModel);
        Task<SubTaskUserViewModel> Update(SubTaskUserViewModel subTaskUserViewModel);
        Task<IEnumerable<SubTaskUserViewModel>> GetByTaskId(Guid id);
    }
}
