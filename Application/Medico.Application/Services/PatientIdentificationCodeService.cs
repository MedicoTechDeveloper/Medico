using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using Medico.Application.Interfaces;
using Medico.Application.PatientIdentificationCodes.ViewModels;
using Medico.Application.ViewModels;
using Medico.Domain.Enums;
using Medico.Domain.Interfaces;
using Medico.Domain.Models;
using Microsoft.EntityFrameworkCore;

namespace Medico.Application.Services
{
    public class PatientIdentificationCodeService : IPatientIdentificationCodeService
    {
        private readonly IPatientRepository _patientRepository;
        private readonly IMapper _mapper;

        public PatientIdentificationCodeService(IPatientRepository patientRepository,
            IMapper mapper)
        {
            _patientRepository = patientRepository;
            _mapper = mapper;
        }

        public async Task<PatientIdentificationCodeVm> Get(IdentificationCodeSearchFilterVm searchFilter)
        {
            var patientId = searchFilter.PatientId;
            var patientIdentificationCodes = await _patientRepository
                .GetAll()
                .Include(p => p.PatientIdentificationCodes)
                .Where(p => p.Id == patientId)
                .SelectMany(p => p.PatientIdentificationCodes)
                .ToListAsync();

            if (!patientIdentificationCodes.Any())
                return null;

            var identificationCodeByType = patientIdentificationCodes
                .FirstOrDefault(c => c.Type == (PatientIdentificationCodeType) searchFilter.IdentificationCodeType);

            if (identificationCodeByType == null)
                return null;

            var identificationCodeVm = _mapper.Map<PatientIdentificationCodeVm>(identificationCodeByType);
            identificationCodeVm.PatientId = patientId;

            return identificationCodeVm;
        }

        public async Task<CreateUpdateResponseVm<PatientIdentificationCodeVm>> Save(PatientIdentificationCodeVm code)
        {
            var patientId = code.PatientId;

            var patient = await _patientRepository
                .GetAll()
                .Include(p => p.PatientIdentificationCodes)
                .FirstOrDefaultAsync(p => p.Id == patientId);

            if (patient == null)
                throw new InvalidOperationException($"Unable to find patient with specified Id: {patientId}");

            var isNewIdentificationCode = code.Id == default;

            var existingIdentificationCode = await _patientRepository
                .GetAll()
                .Include(p => p.PatientIdentificationCodes)
                .Where(p => p.CompanyId == patient.CompanyId)
                .SelectMany(p => p.PatientIdentificationCodes)
                .FirstOrDefaultAsync(c => c.IdentificationCodeString == code.IdentificationCodeString);

            var isIdentificationCodeUnique = isNewIdentificationCode
                ? existingIdentificationCode == null
                : existingIdentificationCode == null || existingIdentificationCode.Id == code.Id;

            if (!isIdentificationCodeUnique)
                return CreateUpdateResponseVm<PatientIdentificationCodeVm>
                    .CreateFailedResponse("The identification code is non unique.");

            if (isNewIdentificationCode)
            {
                var newIdentificationCode = _mapper.Map<PatientIdentificationCode>(code);
                if (patient.PatientIdentificationCodes == null)
                    patient.PatientIdentificationCodes = new List<PatientIdentificationCode>();

                patient.PatientIdentificationCodes.Add(newIdentificationCode);
                await _patientRepository.SaveChangesAsync();

                code.Id = newIdentificationCode.Id;

                return CreateUpdateResponseVm<PatientIdentificationCodeVm>
                    .CreateSuccessResponse(code);
            }

            var codeId = code.Id;
            var identificationCodeToModify = patient.PatientIdentificationCodes
                .FirstOrDefault(c => c.Id == codeId);

            if (identificationCodeToModify == null)
                throw new InvalidOperationException(
                    $"Unable to find patient identification code with specified Id: {codeId}");

            _mapper.Map(code, identificationCodeToModify);
            await _patientRepository.SaveChangesAsync();

            return CreateUpdateResponseVm<PatientIdentificationCodeVm>
                .CreateSuccessResponse(code);
        }

        public async Task<int?> GetNextValidNumericCodeValue(IdentificationCodeSearchFilterVm searchFilter)
        {
            var patientId = searchFilter.PatientId;

            var patient = await _patientRepository
                .GetAll()
                .Include(p => p.PatientIdentificationCodes)
                .FirstOrDefaultAsync(p => p.Id == patientId);

            if (patient == null)
                throw new InvalidOperationException($"Unable to find patient with specified Id: {patientId}");

            var lastNumericCodeValue = await _patientRepository
                .GetAll()
                .Include(p => p.PatientIdentificationCodes)
                .Where(p => p.CompanyId == patient.CompanyId)
                .SelectMany(p => p.PatientIdentificationCodes)
                .Where(c => c.Type == (PatientIdentificationCodeType) searchFilter.IdentificationCodeType)
                .OrderByDescending(c => c.NumericCode)
                .Select(c => c.NumericCode)
                .FirstOrDefaultAsync();

            if (lastNumericCodeValue == 0)
                return null;

            return ++lastNumericCodeValue;
        }
    }
}