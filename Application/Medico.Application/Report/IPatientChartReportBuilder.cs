using System;
using System.Threading.Tasks;

namespace Medico.Application.Report
{
    public interface IPatientChartReportBuilder
    {
        Task<string> BuildReportBasedOnPatientChart(Guid admissionId,
            Guid companyId, Guid patientId, int utcOffset);
    }
}