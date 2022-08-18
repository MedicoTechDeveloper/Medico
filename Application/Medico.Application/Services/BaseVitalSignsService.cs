using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using Medico.Application.Interfaces;
using Medico.Application.ViewModels;
using Medico.Domain.Interfaces;
using Medico.Domain.Models;
using Microsoft.EntityFrameworkCore;

namespace Medico.Application.Services
{
    public class BaseVitalSignsService : IBaseVitalSignsService
    {
        #region DI

        private readonly IBaseVitalSignsRepository _repository;
        IBaseVitalSignsHistoryRepository _baseVitalSignsHistoryRepository;
        private readonly IMapper _mapper;

        public BaseVitalSignsService(IBaseVitalSignsRepository repository,
            IBaseVitalSignsHistoryRepository baseVitalSignsHistoryRepository,
            IMapper mapper)
        {
            _repository = repository;
            _baseVitalSignsHistoryRepository = baseVitalSignsHistoryRepository;
            _mapper = mapper;
        }

        #endregion

        public async Task<BaseVitalSignsViewModel> GetByPatientId(Guid patientId)
        {
            var baseVitalSigns = await _repository.GetAll()
                .FirstOrDefaultAsync(bvs => bvs.PatientId == patientId);
            return baseVitalSigns == null
                ? null
                : _mapper.Map<BaseVitalSignsViewModel>(baseVitalSigns);
        }

        public async Task<BaseVitalSignsViewModel> Create(BaseVitalSignsViewModel baseVitalSignsViewModel)
        {
            var baseVitalSigns = Mapper.Map<BaseVitalSigns>(baseVitalSignsViewModel);
            await _repository.AddAsync(baseVitalSigns);
            await _repository.SaveChangesAsync();

            baseVitalSignsViewModel.Id = baseVitalSigns.Id;

            // Insert History
            await SaveHistory(baseVitalSignsViewModel);

            return baseVitalSignsViewModel;
        }
        
        public async Task<BaseVitalSignsViewModel> Update(BaseVitalSignsViewModel baseVitalSignsViewModel)
        {
            try
            {
                var baseVitalSigns = Mapper.Map<BaseVitalSigns>(baseVitalSignsViewModel);
                _repository.Update(baseVitalSigns);
                await _repository.SaveChangesAsync();

                baseVitalSignsViewModel.Id = baseVitalSigns.Id;

                // Insert History
                await SaveHistory(baseVitalSignsViewModel);

                return baseVitalSignsViewModel;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        private async Task SaveHistory(BaseVitalSignsViewModel baseVitalSignsViewModel)
        {

            try
            {
                var baseVitalSignsHistory = _mapper.Map<BaseVitalSignsHistory>(baseVitalSignsViewModel);
                baseVitalSignsHistory.Id = Guid.NewGuid();
                baseVitalSignsHistory.CreatedOn = DateTime.UtcNow;

                await _baseVitalSignsHistoryRepository.AddAsync(baseVitalSignsHistory);
                await _baseVitalSignsHistoryRepository.SaveChangesAsync();
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public async Task<IEnumerable<BaseVitalSignsViewModel>> GetHistoryByPatientId(Guid patientId)
        {
            var baseVitalSigns = await _baseVitalSignsHistoryRepository.GetAll()
                .Where(bvs => bvs.PatientId == patientId)
                .ToListAsync();

            return baseVitalSigns == null
                ? null
                : _mapper.Map<IEnumerable<BaseVitalSignsViewModel>>(baseVitalSigns);
        }
    }
}