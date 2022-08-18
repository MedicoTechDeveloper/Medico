using Medico.Application.ViewModels;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Medico.Application.Interfaces
{
    public interface ISubTaskService
    {
        Task<SubTaskViewModel> Create(SubTaskViewModel subTaskViewModel);
        Task<SubTaskViewModel> Update(SubTaskViewModel subTaskViewModel);
        Task<SubTaskViewModel> GetById(Guid id);
        Task<IEnumerable<SubTaskViewModel>> GetByNotification(int notificationId);
    }
}
