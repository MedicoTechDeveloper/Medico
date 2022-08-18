using System;
using System.Collections.Generic;
using Medico.Application.Services.PatientChart;
using Medico.Application.ViewModels;

namespace Medico.Application.Interfaces
{
    public interface IPatientChartNodeManagementService
    {
        IEnumerable<LookupViewModel> GetNodes(); 
            
        string GetFullPathNameToChildNode(Guid patientChartNodeId);

        IPatientChartNodeManagementService SetPatientChartRootNode(PatientChartNode rootNode);

        IEnumerable<PatientChartNode> Find(Func<PatientChartNode, bool> filter);
    }
}