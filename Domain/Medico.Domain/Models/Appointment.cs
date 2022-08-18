using System;
using System.Collections.Generic;
using System.Linq;

namespace Medico.Domain.Models
{
    public class Appointment : Entity
    {
        public Appointment()
        {
            PatientChartDocumentNodes = new List<AppointmentPatientChartDocument>();
        }

        public Guid PatientId { get; set; }

        public Patient Patient { get; set; }

        public Guid CompanyId { get; set; }

        public Company Company { get; set; }

        public Guid LocationId { get; set; }

        public Location Location { get; set; }

        public Guid PhysicianId { get; set; }

        public Guid NurseId { get; set; }

        public Guid RoomId { get; set; }

        public Room Room { get; set; }

        public DateTime StartDate { get; set; }

        public DateTime EndDate { get; set; }

        public Admission Admission { get; set; }

        public Guid? AdmissionId { get; set; }

        public string Allegations { get; set; }

        public string AllegationsNotes { get; set; }

        public string AppointmentStatus { get; set; }

        public List<AppointmentPatientChartDocument> PatientChartDocumentNodes { get; }

        public void AddPatientChartDocumentNodes(
            IEnumerable<AppointmentPatientChartDocument> newPatientChartDocumentNodes)
        {
            var removedDocumentNodes = PatientChartDocumentNodes
                .Where(oldNode => newPatientChartDocumentNodes
                    .FirstOrDefault(newNode =>
                        newNode.PatientChartDocumentNodeId == oldNode.PatientChartDocumentNodeId) == null)
                .ToList();

            var newDocumentNodes = newPatientChartDocumentNodes
                .Where(newNode => PatientChartDocumentNodes
                    .FirstOrDefault(oldNode =>
                        oldNode.PatientChartDocumentNodeId == newNode.PatientChartDocumentNodeId) == null)
                .ToList();

            if (removedDocumentNodes.Any())
            {
                foreach (var removedDocumentNode in removedDocumentNodes)
                {
                    PatientChartDocumentNodes.Remove(removedDocumentNode);
                }
            }

            if (newDocumentNodes.Any())
                PatientChartDocumentNodes.AddRange(newDocumentNodes);
        }
    }
}