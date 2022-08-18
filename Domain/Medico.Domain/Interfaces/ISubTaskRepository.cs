﻿using Medico.Domain.Models;

namespace Medico.Domain.Interfaces
{
    public interface ISubTaskRepository
        : IDeletableByIdRepository<SubTask>
    {
    }
}
