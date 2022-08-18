using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Medico.Application.ViewModels;
using Medico.Application.ViewModels.Patient;
using Medico.Domain.Models;

namespace Medico.Application.Interfaces
{
    public interface IAppointmentService
    {
        Task<AppointmentGridItemViewModel> GetAppointmentGridItemById(Guid id);

        IQueryable<AppointmentGridItemViewModel> GetAllAppointmentGridItems(AppointmentDxOptionsViewModel dxOptions);

        Task<AppointmentViewModel> Create(AppointmentViewModel appointmentViewModel);

        Task<AppointmentViewModel> Update(AppointmentViewModel appointmentViewModel);

        IQueryable<AppointmentGridItemViewModel> GetAll(AppointmentDxOptionsViewModel dxOptions);

        Task Delete(Guid id);

        Task<AppointmentViewModel> GetPatientLastVisit(Guid patientId, DateTime currentDate);

        Task<IEnumerable<AppointmentGridItemViewModel>> GetPatientPreviousVisits(Guid patientId, DateTime currentDate);

        Task<AppointmentViewModel> GetByAdmissionId(Guid admissionId);

        Task<AppointmentViewModel> GetByLocationId(Guid locationId);

        Task<AppointmentViewModel> GetByRoomId(Guid roomId);

        Task<AppointmentViewModel> GetByUserId(Guid userId);

        Task<AppointmentViewModel> GetById(Guid id);

        Task<IEnumerable<PatientAppointmentVm>> GetByPatientAndCompanyId(Guid patientId, Guid companyId);
        Task<AppointmentHistory> PutStatus(AppointmentStatusVM appointmentStatus);

        Task<IEnumerable<AppointmentStatusSearch>> GetAppointmentStatus(Guid appointmentId);
        Task<IEnumerable<AppointmentStatusPatient>> GetAppointmentStatusByCompany(Guid companyId);

        Task<IEnumerable<AppointmentStatusPieChart>> GetAppointmentStatusPieChart();
        Task<IEnumerable<AppointmentPatientChartDocumentModel>> GetAppointmentPatientChartDocument(string admissionId);
    }
}