using System;
using System.Threading.Tasks;
using Medico.Application.Services.PatientChart;

namespace Medico.Application.Report.ReportNodes
{
    public interface IReportNodeBuilder
    {
        PatientChartNodeType NodeType { get; }

        Task<string> BuildContent(PatientChartNode patientChartNode,
            Guid patientId, Guid admissionId, int clientUtcOffset);
    }
}