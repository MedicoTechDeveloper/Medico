using Medico.Domain.Models;

namespace Medico.Domain.Interfaces
{
    public interface IAppointmentPatientChartDocumentRepository
        : IDeletableByEntityRepository<AppointmentPatientChartDocument>
    {
    }
}