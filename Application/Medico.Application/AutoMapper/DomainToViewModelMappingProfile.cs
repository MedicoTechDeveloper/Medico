using System.Collections.Generic;
using System.Linq;
using AutoMapper;
using Medico.Application.Extensions;
using Medico.Application.PatientIdentificationCodes.ViewModels;
using Medico.Application.ViewModels;
using Medico.Application.ViewModels.Admission;
using Medico.Application.ViewModels.Company;
using Medico.Application.ViewModels.Document;
using Medico.Application.ViewModels.Expression;
using Medico.Application.ViewModels.Patient;
using Medico.Application.ViewModels.Phrase;
using Medico.Application.ViewModels.ReferenceTable;
using Medico.Application.ViewModels.SelectableList;
using Medico.Application.ViewModels.SelectableListCategory;
using Medico.Application.ViewModels.Template;
using Medico.Application.ViewModels.TemplateType;
using Medico.Domain.Models;
using Newtonsoft.Json;

namespace Medico.Application.AutoMapper
{
    public class DomainToViewModelMappingProfile : Profile
    {
        public DomainToViewModelMappingProfile()
        {
            CreateMap<PatientChartDocumentNode, LookupViewModel>();

            CreateMap<Company, CompanyVm>();
            CreateMap<Company, LookupViewModel>()
                .ForMember(d => d.Id, opt => opt.MapFrom(s => s.Id))
                .ForMember(d => d.Name, opt => opt.MapFrom(s => s.Name));

            CreateMap<Location, LocationViewModel>();

            CreateMap<Room, RoomViewModel>();
            CreateMap<Room, RoomWithLocationViewModel>()
                .ForMember(d => d.Location,
                    opt => opt.MapFrom(s => s.Location.Name));

            CreateMap<LocationRoom, LocationRoomViewModel>();
            CreateMap<MedicoApplicationUserView, MedicoApplicationUserViewModel>();
            CreateMap<SelectableListCategory, SelectableListCategoryVm>();

            CreateMap<SelectableList, SelectableListVm>()
                .ForMember(d => d.SelectableListValues, opt
                    => opt.MapFrom(s =>
                        JsonConvert.DeserializeObject<IEnumerable<SelectableListValueViewModel>>(s.JsonValues)));

            CreateMap<SelectableList, SelectableListGridItemVm>();

            CreateMap<SelectableList, LookupViewModel>()
                .ForMember(d => d.Name, opt => opt.MapFrom(s => s.Title));

            CreateMap<CategorySelectableList, CategorySelectableListVm>();
            CreateMap<CategorySelectableList, CompanyCategorySelectableListVm>();

            CreateMap<TemplateType, TemplateTypeVm>();
            CreateMap<TemplateType, LookupViewModel>()
                .ForMember(d => d.Name, opt => opt.MapFrom(s => s.Title));

            CreateMap<Template, TemplateVm>();

            CreateMap<DependentTemplate, LookupViewModel>()
                .ForMember(d => d.Id,
                    opt => opt.MapFrom(s => s.TargetTemplateId))
                .ForMember(d => d.Name,
                    opt => opt.MapFrom(s => s.TargetTemplate.ReportTitle));

            CreateMap<Template, TemplateGridItemVm>()
                .ForMember(d => d.TemplateTypeName,
                    opt =>
                        opt.MapFrom(s => s.TemplateType.Name))
                .ForMember(d => d.LibraryTemplateVersion,
                    opt =>
                        opt.MapFrom(s => s.LibraryTemplate != null ? s.LibraryTemplate.Version : null));

            CreateMap<Template, TemplateWithTypeNameViewModel>()
                .ForMember(d => d.TemplateTypeName, opt => opt.MapFrom(s => s.TemplateType.Name));

            CreateMap<Template, LookupViewModel>()
                .ForMember(d => d.Name, opt => opt.MapFrom(s => s.ReportTitle));

            CreateMap<ChiefComplaint, ChiefComplaintViewModel>();
            CreateMap<ChiefComplaint, LookupViewModel>()
                .ForMember(d => d.Name,
                    opt => opt.MapFrom(s => s.Title));

            CreateMap<ChiefComplaintKeyword, ChiefComplaintKeywordViewModel>();
            CreateMap<IcdCode, IcdCodeViewModel>();
            CreateMap<CptCode, CptCodeViewModel>();
            CreateMap<AppointmentGridItem, AppointmentGridItemViewModel>();

            CreateMap<AppointmentGridItem, PatientAppointmentVm>()
                .ForMember(d => d.Nurse,
                    opt => opt.MapFrom(s => $"{s.NurseFirstName} {s.NurseLastName}"))
                .ForMember(d => d.Physician,
                    opt => opt.MapFrom(s => $"{s.PhysicianFirstName} {s.PhysicianLastName}"))
                .ForMember(d => d.IsPatientChartSignedIn,
                    opt => opt.MapFrom(s => s.SigningDate != null))
                .ForMember(d => d.AppointmentId,
                    opt => opt.MapFrom(s => s.Id));

            CreateMap<Patient, PatientVm>()
                .ForMember(d => d.Password,
                    opt => opt.MapFrom(s => s.SecurityHash.Decrypt()));

            CreateMap<Patient, PatientLookupVm>()
                .ForMember(d => d.Name,
                    opt => opt.MapFrom(s => $"{s.FirstName} {s.LastName}"));

            CreateMap<Appointment, AppointmentViewModel>()
                .ForMember(d => d.PatientChartDocumentNodes,
                    opt => opt.Ignore());

            CreateMap<AppointmentPatientChartDocument, LookupViewModel>()
                .ForMember(d => d.Id,
                    opt =>
                        opt.MapFrom(s => s.PatientChartDocumentNodeId))
                .ForMember(d => d.Name,
                    opt =>
                        opt.MapFrom(s => s.PatientChartDocumentNode.Title));

            CreateMap<MedicoApplicationUser, LookupViewModel>()
                .ForMember(d => d.Id, opt => opt.MapFrom(s => s.Id))
                .ForMember(d => d.Name, opt => opt.MapFrom(s => $"{s.FirstName} {s.LastName}"));

            CreateMap<Patient, PatientProjectionViewModel>()
                .ForMember(d => d.Name, opt => opt.MapFrom(s => $"{s.FirstName} {s.LastName}"))
                .ForMember(d => d.Phone, opt => opt.MapFrom(s => s.PrimaryPhone));

            CreateMap<Medication, LookupViewModel>()
                .ForMember(d => d.Name, opt => opt.MapFrom(s =>
                    $"{s.NdcCode} {(string.IsNullOrEmpty(s.PharmaceuticalClasses) ? "-//-" : s.PharmaceuticalClasses)} {s.SubstanceName} {(string.IsNullOrEmpty(s.RouteName) ? "-//-" : s.RouteName)} {(string.IsNullOrEmpty(s.StrengthNumber) ? "-//-" : s.StrengthNumber)} {(string.IsNullOrEmpty(s.StrengthUnit) ? "-//-" : s.StrengthUnit)} {(string.IsNullOrEmpty(s.DeaSchedule) ? "-//-" : s.DeaSchedule)}"));

            CreateMap<PatientInsurance, PatientInsuranceViewModel>();
            CreateMap<TemplateSelectableList, SelectableListTrackItemViewModel>();
            CreateMap<Admission, AdmissionVm>();
            CreateMap<Admission, FullAdmissionInfoVm>();
            CreateMap<SignatureInfo, SignatureInfoViewModel>();
            CreateMap<TobaccoHistory, TobaccoHistoryViewModel>();
            CreateMap<DrugHistory, DrugHistoryViewModel>();
            CreateMap<AlcoholHistory, AlcoholHistoryViewModel>();
            CreateMap<MedicalHistory, MedicalHistoryViewModel>();
            CreateMap<SurgicalHistory, SurgicalHistoryViewModel>();
            CreateMap<FamilyHistory, FamilyHistoryViewModel>();
            CreateMap<EducationHistory, EducationHistoryViewModel>();
            CreateMap<OccupationalHistory, OccupationalHistoryViewModel>();
            CreateMap<Allergy, AllergyViewModel>();
            CreateMap<MedicationHistory, MedicationHistoryViewModel>();
            CreateMap<MedicalRecord, MedicalRecordViewModel>();
            
            CreateMap<VitalSigns, VitalSignsViewModel>();
            CreateMap<VitalSigns, VitalSignsViewVM>();
            CreateMap<BaseVitalSigns, BaseVitalSignsViewModel>();
            CreateMap<Document, DocumentViewModel>();

            CreateMap<ChiefComplaint, ChiefComplaintWithKeywordsViewModel>()
                .ForMember(d => d.Keywords,
                    opt => opt.MapFrom(s =>
                        string.Join(", ", s.ChiefComplaintsKeywords.Select(ck => ck.Keyword.Value))));

            CreateMap<Room, LookupViewModel>();
            CreateMap<MedicationName, LookupViewModel>();

            CreateMap<MedicoApplicationUser, MedicoApplicationUserViewModel>();
            CreateMap<SelectableListCategory, LookupViewModel>()
                .ForMember(d => d.Name,
                    opt => opt.MapFrom(s => s.Title));

            CreateMap<MedicationItemInfoView, MedicationItemInfoViewModel>()
                .ForMember(d => d.DosageFormList,
                    opt => opt.MapFrom(s => s.DosageForms.Split(new[] {';'}).Distinct()))
                .ForMember(d => d.UnitList,
                    opt => opt.MapFrom(s => s.Units.Split(new[] {';'}).Distinct()))
                .ForMember(d => d.StrengthList,
                    opt => opt.MapFrom(s => s.Strength.Split(new[] {';'}).Distinct()))
                .ForMember(d => d.RouteList,
                    opt => opt.MapFrom(s => s.Routes.Split(new[] {';'}).Distinct()));

            CreateMap<MedicationName, LookupViewModel>();
            CreateMap<MedicationPrescription, MedicationPrescriptionViewModel>();

            CreateMap<MedicationClass, LookupViewModel>()
                .ForMember(d => d.Name,
                    opt => opt.MapFrom(s => s.ClassName));

            CreateMap<Allergy, AllergyOnMedicationViewModel>()
                .ForMember(d => d.MedicationName,
                    opt => opt.MapFrom(s => s.MedicationName != null ? s.MedicationName.Name : null))
                .ForMember(d => d.MedicationClass,
                    opt => opt.MapFrom(s => s.MedicationClass != null ? s.MedicationClass.ClassName : null));

            CreateMap<MedicationsUpdateItem, MedicationsUpdateItemViewModel>()
                .ForMember(d => d.MedicationsFilePath,
                    opt => opt.MapFrom(s =>
                        $@"api/medications-scheduled-item/download/medications/{s.MedicationsFileName}"));

            CreateMap<VisionVitalSigns, VisionVitalSignsViewModel>();
            CreateMap<AllegationsNotesStatus, AllegationsNotesStatusViewModel>();
            
            CreateMap<Phrase, LookupViewModel>()
                .ForMember(d => d.Name,
                    opt => opt.MapFrom(s => s.Title));
            CreateMap<VitalSignsNotes, VitalSignsNotesViewModel>();

            CreateMap<PatientChartDocumentNode, LookupViewModel>()
                .ForMember(d => d.Name,
                    opt => opt.MapFrom(s => s.Title));

            CreateMap<PatientChartDocumentNode, PatientChartDocumentNodeViewModel>();

            CreateMap<ReferenceTable, ReferenceTableGridItemVm>();
            CreateMap<ReferenceTable, ReferenceTableVm>()
                .ForMember(d => d.Data,
                    opt => opt.MapFrom(s =>
                        JsonConvert.DeserializeObject<ReferenceTableData>(s.Data)));
            CreateMap<ReferenceTable, LookupViewModel>()
                .ForMember(d => d.Name,
                    opt => opt.MapFrom(s => s.Title));

            CreateMap<Expression, ExpressionGridItemVm>();
            CreateMap<Expression, ExpressionVm>();
            CreateMap<Expression, LookupViewModel>()
                .ForMember(d => d.Name,
                    opt => opt.MapFrom(s => s.Title));
            
            CreateMap<PatientIdentificationCode, PatientIdentificationCodeVm>();
            CreateMap<InsuranceCompany, InsuranceCompanyProjectionViewModel>();
            CreateMap<InsuranceCompany, InsuranceCompanyViewModel>();
            CreateMap<InsuranceCompany, LookupViewModel>();

            CreateMap<DocumentLog, DocumentProjectionViewModel>();
            CreateMap<LabTest, LabTestViewModel>();
            CreateMap<PatientOrder, PatientOrderViewModel>();

            CreateMap<EmailAccount, EmailAccountViewModel>();

            CreateMap<PhraseCategory, PhraseCategoryViewModel>();

            CreateMap<VitalSignsLookUp, VitalSignsLookUpViewModel>();
            CreateMap<VitalSignsLookUp, VitalSignsLookUpProjectionViewModel>();

            CreateMap<PreAuthData, PreAuthDataViewModel>();
            CreateMap<PreAuthData, LookupViewModel>();
            CreateMap<PreAuthData, PreAuthDataProjectionViewModel>();

            CreateMap<ProblemList, ProblemListViewModel>();

            CreateMap<BaseVitalSignsHistory, BaseVitalSignsViewModel>();

            CreateMap<SubTask, SubTaskViewModel>();
            CreateMap<SubTaskUser, SubTaskUserViewModel>();
        }
    }
}