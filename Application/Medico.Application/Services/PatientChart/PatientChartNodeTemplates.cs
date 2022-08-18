using System;

namespace Medico.Application.Services.PatientChart
{
    public static class PatientChartNodeTemplates
    {
        public static string GetTemplateValueForPatientChartTemplateNode(Guid templateId, string templateNodeTypeName)
        {
            return
                $"<patient-chart-template [admissionId]='admissionId' [companyId]='companyId' [isSignedOff]='isSignedOff' [patientChartNode]='patientChartNode' [templateType]='{templateNodeTypeName}' [templateId]='\"{templateId}\"' [patientChartDocumentNode]='patientChartDocumentNode'></patient-chart-template>";
        }

        public static string GetTemplateValueForPatientChartTemplateListNode(Guid templateTypeName)
        {
            return
                $"<template-list [companyId]='companyId' [isSignedOff]='isSignedOff' [patientChartNode]='patientChartNode' templateType='${templateTypeName}'></template-list>";
        }

        public static string GetTemplateValueForPatientChartScanDocumentNode(int pageNumber)
        {
            return
                $"<scan-document [companyId]='companyId' [isSignedOff]='isSignedOff' [patientId]='patientId' [appointmentId]='appointmentId' [pageNum]='${pageNumber}'></scan-document>";
        }
    }
}