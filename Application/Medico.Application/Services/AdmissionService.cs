using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using AutoMapper.QueryableExtensions;
using Dapper;
using Medico.Application.Interfaces;
using Medico.Application.Services.PatientChart;
using Medico.Application.ViewModels;
using Medico.Application.ViewModels.Admission;
using Medico.Application.ViewModels.PatientChartDocument;
using Medico.Data.Context;
using Medico.Domain.Constants;
using Medico.Domain.Interfaces;
using Medico.Domain.Models;
using Microsoft.AspNetCore.Hosting;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;

namespace Medico.Application.Services
{
    public class AdmissionService : BaseDeletableByIdService<Admission, AdmissionVm>, IAdmissionService
    {
        #region DI
        private IDbConnection Connection => new SqlConnection(_configuration.GetConnectionString("DefaultConnection"));
        private readonly IAppointmentRepository _appointmentRepository;
        private readonly IPatientChartDocumentNodeService _patientChartDocumentNodeService;
        private readonly IPatientChartNodeManagementService _patientChartNodeManagementService;
        private readonly ITemplateService _templateService;
        private readonly IPatientChartService _patientChartService;
        private readonly IConfiguration _configuration;
        private readonly IHostingEnvironment _env;
        private readonly IUserService _userService;
        private readonly JsonSerializerSettings _jsonSerializerSettings = new JsonSerializerSettings
        {
            ContractResolver = new CamelCasePropertyNamesContractResolver()
        };

        private readonly FullAdmissionInfoVm _expressionTestAdmission =
            new FullAdmissionInfoVm
            {
                PatientId = Guid.Parse(ExpressionTestConstants.Ids.PatientId),
                VitalSigns = new List<VitalSignsViewModel>
                {
                    new VitalSignsViewModel
                    {
                        CreatedDate = DateTime.UtcNow,
                        Pulse = 70,
                        SystolicBloodPressure = 120,
                        DiastolicBloodPressure = 80,
                        BloodPressurePosition = "Sitting",
                        BloodPressureLocation = "Right Bicep",
                        OxygenSaturationAtRest = "12",
                        RespirationRate = 10
                    },
                    new VitalSignsViewModel
                    {
                        CreatedDate = DateTime.UtcNow.AddHours(1),
                        Pulse = 60,
                        SystolicBloodPressure = 130,
                        DiastolicBloodPressure = 70,
                        BloodPressurePosition = "Sitting",
                        BloodPressureLocation = "Right Bicep",
                        OxygenSaturationAtRest = "12",
                        RespirationRate = 11
                    }
                },
                MedicationPrescriptions = new List<MedicationPrescriptionViewModel>
                {
                }
            };

        public AdmissionService(IAdmissionRepository admissionRepository,
            IMapper mapper,
            IConfiguration configuration,
            IAppointmentRepository appointmentRepository,
            IPatientChartDocumentNodeService patientChartDocumentNodeService,
            IPatientChartNodeManagementService patientChartNodeManagementService,
            ITemplateService templateService,
            IPatientChartService patientChartService,
            IUserService userService,
            IHostingEnvironment env) : base(admissionRepository, mapper)
        {
            _appointmentRepository = appointmentRepository;
            _patientChartDocumentNodeService = patientChartDocumentNodeService;
            _patientChartNodeManagementService = patientChartNodeManagementService;
            _templateService = templateService;
            _patientChartService = patientChartService;
            _configuration = configuration;
            _env = env;
            _userService = userService;
        }
        #endregion

        public override async Task<AdmissionVm> Create(AdmissionVm viewModel)
        {
            var appointment = await _appointmentRepository.GetAll()
                .Include(a => a.PatientChartDocumentNodes)
                .FirstOrDefaultAsync(a => a.Id == viewModel.AppointmentId);

            var patientChartDocumentNodeIds = appointment.PatientChartDocumentNodes
                .Select(d => d.PatientChartDocumentNodeId);

            var companyId = appointment.CompanyId;

            var patientChartDocumentFilter = new PatientChartDocumentFilterVm
            {
                PatientChartDocumentNodes =
                    string.Join(',', patientChartDocumentNodeIds),
                CompanyId = companyId,
                RestrictByPatientChartDocumentNodes = true
            };

            var patientChart =
                await _patientChartService.GetByFilter(patientChartDocumentFilter);

            var doDocumentNodesExist =
                patientChart != null && patientChart.Children.Any();

            if (doDocumentNodesExist)
            {
                foreach (var documentNode in patientChart.Children)
                {
                    await SetDefaultContentToTemplatesNodes(documentNode);

                    await AddRequiredTemplatesToTemplateListNodes(documentNode, companyId);
                }
            }

            viewModel.AdmissionData =
                JsonConvert.SerializeObject(patientChart, _jsonSerializerSettings);

            var newAdmission = Mapper.Map<Admission>(viewModel);
            await Repository.AddAsync(newAdmission);
            await Repository.SaveChangesAsync();

            var newAdmissionId = newAdmission.Id;

            appointment.AdmissionId = newAdmissionId;
            _appointmentRepository.Update(appointment);
            await Repository.SaveChangesAsync();

            viewModel.Id = newAdmissionId;

            return viewModel;
        }

        public async Task<FullAdmissionInfoVm> GetFullAdmissionInfoById(Guid id)
        {
            var isExpressionTestAdmission =
                Guid.Parse(ExpressionTestConstants.Ids.AdmissionId) == id;

            if (isExpressionTestAdmission)
                return _expressionTestAdmission;

            var admission = await Repository.GetAll()
                .Include(a => a.VitalSigns)
                .Include(a => a.MedicationPrescriptions)
                .FirstOrDefaultAsync(a => a.Id == id);

            return admission == null
                ? null
                : Mapper.Map<FullAdmissionInfoVm>(admission);
        }

        public async Task<AdmissionVm> GetByAppointmentId(Guid appointmentId)
        {
            var admission = await Repository.GetAll()
                .FirstOrDefaultAsync(a => a.AppointmentId == appointmentId);

            return admission == null
                ? null
                : Mapper.Map<AdmissionVm>(admission);
        }

        public async Task<IEnumerable<AdmissionVm>> GetPreviousPatientAdmissions(Guid patientId, DateTime fromDate)
        {
            var previousAdmissions = await Repository.GetAll()
                .Where(a => a.PatientId == patientId && a.CreatedDate < fromDate)
                .OrderByDescending(a => a.CreatedDate)
                .ProjectTo<AdmissionVm>()
                .ToListAsync();

            return previousAdmissions;
        }

        public Task Delete(Guid id)
        {
            return DeleteById(id);
        }

        public async Task DeleteWithAllRelatedData(Guid id)
        {
            var admission = await Repository.GetAll()
                .Include(a => a.MedicationPrescriptions)
                .Include(a => a.SignatureInfo)
                .Include(a => a.VitalSigns)
                .Include(a => a.VitalSignsNotes)
                .Include(a => a.AllegationsNotesStatus)
                .FirstOrDefaultAsync(a => a.Id == id);

            if (admission == null)
                throw new InvalidOperationException(
                    $"Unable to delete admission with id {id}. Admission was not found.");

            if (admission.SignatureInfo != null)
                throw new InvalidOperationException(
                    $"Unable to delete admission with id {id}. Admission is already signed in");

            if (admission.MedicationPrescriptions != null && admission.MedicationPrescriptions.Any())
                admission.MedicationPrescriptions.Clear();

            if (admission.VitalSigns != null && admission.VitalSigns.Any())
                admission.VitalSigns.Clear();

            if (admission.VitalSignsNotes != null)
                admission.VitalSignsNotes = null;

            if (admission.AllegationsNotesStatus != null)
                admission.AllegationsNotesStatus = null;

            await DeleteById(id);
        }

        public async Task<AdmissionVm> UpdatePatientChartDocumentNodes(
            UpdatePatientChartDocumentNodesVm updatePatientChartDocumentNodesVm)
        {
            var admissionId =
                updatePatientChartDocumentNodesVm.AdmissionId;

            var admission = await Repository.GetAll()
                .FirstOrDefaultAsync(a => a.Id == admissionId);

            var appointment = await _appointmentRepository.GetAll()
                .Include(t => t.PatientChartDocumentNodes)
                .FirstOrDefaultAsync(t => t.Id == admission.AppointmentId);

            var patientChart =
                JsonConvert.DeserializeObject<PatientChartNode>(admission.AdmissionData);

            var newPatientChart =
                JsonConvert.DeserializeObject<PatientChartNode>(admission.AdmissionData);

            newPatientChart.Children.Clear();

            var documentNodes =
                updatePatientChartDocumentNodesVm.DocumentNodes == null
                    ? Enumerable.Empty<LookupStateViewModel>().ToList()
                    : updatePatientChartDocumentNodesVm.DocumentNodes.ToList();

            if (!documentNodes.Any())
            {
                await SaveDocumentNodesChanges(newPatientChart, admission, appointment,
                    Enumerable.Empty<AppointmentPatientChartDocument>());
            }
            else
            {
                foreach (var documentNode in documentNodes)
                {
                    var isNewAddedDocumentNode =
                        documentNode.EntityStateType == EntityStateType.New;

                    if (!isNewAddedDocumentNode)
                    {
                        var savedDocumentNode =
                            patientChart.Children
                                .FirstOrDefault(n => n.Id == documentNode.Id);

                        newPatientChart.Children.Add(savedDocumentNode);
                    }
                    else
                    {
                        var newlyCreatedDocumentNode =
                            await CreateNewDocumentNode(documentNode.Id, newPatientChart.Id);

                        newPatientChart.Children.Add(newlyCreatedDocumentNode);
                    }
                }

                var patientChartDocumentNodes = documentNodes
                    .Select(d => new AppointmentPatientChartDocument
                    { AppointmentId = appointment.Id, PatientChartDocumentNodeId = d.Id });

                await SaveDocumentNodesChanges(newPatientChart, admission, appointment, patientChartDocumentNodes);
            }

            return Mapper.Map<AdmissionVm>(admission);
        }

        private async Task<PatientChartNode> CreateNewDocumentNode(Guid documentNodeId, Guid parentId)
        {
            var documentNode = await _patientChartDocumentNodeService
                .GetById(documentNodeId);

            var documentNodeData =
                JsonConvert.DeserializeObject<PatientChartNode>(documentNode.PatientChartDocumentNodeJsonString);

            documentNodeData.ParentId = parentId;

            await SetDefaultContentToTemplatesNodes(documentNodeData);

            await AddRequiredTemplatesToTemplateListNodes(documentNodeData, documentNode.CompanyId);

            return documentNodeData;
        }

        private async Task SetDefaultContentToTemplatesNodes(PatientChartNode documentNodeData)
        {
            var templateNodes = _patientChartNodeManagementService
                .SetPatientChartRootNode(documentNodeData)
                .Find((node => node.Type == PatientChartNodeType.TemplateNode))
                .ToList();

            if (!templateNodes.Any())
                return;

            var templateIds = templateNodes
                .Select(n =>
                {
                    var templateId =
                        n.Attributes.NodeSpecificAttributes.TemplateId;

                    if (templateId == null)
                        throw new NullReferenceException(nameof(templateId));

                    return templateId.Value;
                });

            var templates =
                await _templateService.GetSpecific(templateIds);

            foreach (var templateNode in templateNodes)
            {
                var templateId =
                    templateNode.Attributes.NodeSpecificAttributes.TemplateId;

                if (templateId == null)
                    throw new NullReferenceException(nameof(templateId));

                var template = templates.First(t => t.Id == templateId);

                templateNode.Value = new
                {
                    DefaultTemplateHtml = template.DefaultTemplateHtml,
                    DetailedTemplateHtml = template.InitialDetailedTemplateHtml,
                    IsDetailedTemplateUsed = string.IsNullOrEmpty(template.DefaultTemplateHtml)
                };
            }
        }

        private async Task AddRequiredTemplatesToTemplateListNodes(PatientChartNode documentNodeData, Guid companyId)
        {
            var documentTemplateListNodes = _patientChartNodeManagementService
                .SetPatientChartRootNode(documentNodeData)
                .Find((node => node.Type == PatientChartNodeType.TemplateListNode))
                .ToList();

            if (!documentTemplateListNodes.Any())
                return;

            var documentTemplateListNodeNames = documentTemplateListNodes
                .Select(d => d.Name);

            var requiredTemplatesPerTypeDictionary =
                await _templateService
                    .GetRequired(companyId, documentTemplateListNodeNames);

            if (!requiredTemplatesPerTypeDictionary.Any())
                return;

            foreach (var (templateTypeName, templates) in requiredTemplatesPerTypeDictionary)
            {
                var templateListNode =
                    documentTemplateListNodes.First(n => n.Name == templateTypeName);

                templateListNode.Value = new List<TemplateNodeInfo>();

                if (templateListNode.Children == null)
                    templateListNode.Children = new List<PatientChartNode>();

                foreach (var templateVm in templates)
                {
                    var newlyCreatedNodeId = Guid.NewGuid();

                    var templateNode = PatientChartNode.CreatePatientChartTemplateNode(newlyCreatedNodeId,
                        templateListNode.Id, templateVm, templateTypeName);

                    templateListNode.Children.Add(templateNode);

                    var templateOrder = templateVm.TemplateOrder;

                    var templateNodeInfo = TemplateNodeInfo.CreateNew(templateVm.Id, templateOrder,
                        templateVm.ReportTitle, newlyCreatedNodeId);

                    templateListNode.Value.Add(templateNodeInfo);
                }

                templateListNode.Value =
                    ((List<TemplateNodeInfo>)templateListNode.Value).OrderBy(t => t.Order)
                    .ToList();

                templateListNode.Children =
                    templateListNode.Children.OrderBy(n => n.Attributes.Order)
                        .ToList();
            }
        }

        private async Task SaveDocumentNodesChanges(PatientChartNode newPatientChart, Admission admission,
            Appointment appointment, IEnumerable<AppointmentPatientChartDocument> newPatientChartDocumentNodes)
        {
            admission.AdmissionData =
                JsonConvert.SerializeObject(newPatientChart, _jsonSerializerSettings);

            appointment.AddPatientChartDocumentNodes(newPatientChartDocumentNodes);

            _appointmentRepository.Update(appointment);
            Repository.Update(admission);

            await Repository.SaveChangesAsync();
        }
       
        public async Task<Admission> GetSingle(Guid id)
        {
            try
            {
                string connectString = _configuration.GetConnectionString("DefaultConnection").ToString();

                MedicoContext context = new MedicoContext(_env);
                var admission = await context.Set<Admission>().FirstOrDefaultAsync(c => c.Id == id);

               

                return admission;
            }
            catch(Exception ex)
            {
                throw ex;
            }
        }
    }
}