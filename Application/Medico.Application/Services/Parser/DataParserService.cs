using AutoMapper;
using AutoMapper.QueryableExtensions;
using Medico.Application.Interfaces;
using Medico.Application.ViewModels;
using Medico.Application.ViewModels.Document;
using Medico.Domain.Interfaces;
using Medico.Domain.Models;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace Medico.Application.Services.Parser
{
    public class DataParserService : BaseDeletableByIdService<DocumentLog, VitalSignsViewModel>, IDataParserService
    {
        private readonly IDataParserRepository _dataParserRepository;
        private readonly IPhysicianDocLogRepository _physicianDocLogRepository;
        private readonly IUnitOfWork _unitOfWork;

        public DataParserService(IDataParserRepository dataParserRepository,
            IPhysicianDocLogRepository physicianDocLogRepository,
             IUnitOfWork unitOfWork,
            IMapper mapper) : base(dataParserRepository, mapper)
        {
            _dataParserRepository = dataParserRepository;
            _physicianDocLogRepository = physicianDocLogRepository;
            _unitOfWork = unitOfWork;
        }

        public async Task<IEnumerable<DocumentProjectionViewModel>> DocumentGrid(DocumentDxOptionsViewModel loadOptions)
        {
            var companyId = loadOptions.CompanyId;

            var documents = await Repository.GetAll()
               .Where(vs => vs.IsProcessed == false)
               .Where(vs => vs.IsDeleted == false)
               .OrderByDescending(vs => vs.DocParserProcessDate)
               .ProjectTo<DocumentProjectionViewModel>()
               .ToListAsync();

            if (loadOptions.CompanyId == Guid.Empty)
            {
                documents = documents.Where(c => c.ParserId == "lapbzmcdrnuj").ToList();
            }
            else
            {
                documents = documents.Where(c => c.ParserId != "lapbzmcdrnuj").ToList();
            }

            return documents;
        }

        public int AddDocuments(IEnumerable<DocumentLog> documents)
        {
            try
            {
                int savedDocs = 0;
                var allDocs = Repository.GetAll();

                foreach (var item in documents)
                {
                    var existing = allDocs.FirstOrDefault(c => c.Id == item.Id);

                    if (existing == null)
                    {
                        Repository.AddAsync(item);
                        savedDocs++;
                    }
                }

                Repository.SaveChangesAsync();
                _unitOfWork.Commit();

                return savedDocs;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public async Task<int> AddDocuments(IEnumerable<PhysicianDocLog> physicianDocuments)
        {
            try
            {
                foreach (var doc in physicianDocuments)
                {
                    await _physicianDocLogRepository.AddAsync(doc);
                }

                await _dataParserRepository.SaveChangesAsync();
                return 1;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public async Task<string> ProcessDocData(ClaimantModel item, string companyId)
        {
            Claimant claimant = new Claimant
            {
                Id = string.Empty,
                ApptDate = item.examdate,
                Birthday = item.birthday,
                ClaimantSSN = item.claimant_ssn,
                DocumentId = item.document_id,
                FileName = item.file_name,
                //HPI = item.variable_hpi,
                MediaLink = item.media_link,
                MediaLinkData = item.media_link_data,
                MediaLinkOriginal = item.media_link_original,
                //MedicoInfo = item.medico_info,
                PageCount = item.page_count,
                ProblemList = item.allegationsbytext,
                ProcessedAt = item.processed_at,
                RemoteId = item.remote_id,
                RQID = item.rqidbytext,
                //Service = item.service,
                SpecialInstructions = item.special_instructions,
                UploadedAt = item.uploaded_at,
                //VariableHPI = item.variable_hpi,
                PhoneNumber = item.phone_number.number,
                AppointmentTime = item.appointment_time,
                ClaimantName = item.claimant_name,
                PhysicianName = item.physicianbylocation,
                ExamLocation = item.exam_location,
                CaseNumber = item.case_number,
            };

            return await _dataParserRepository.ProcessDocData(claimant);
        }

        public async Task UpdateDocument(DocumentLog documentLog)
        {
            var existing = Repository.GetAll().SingleOrDefault(c => c.Id == documentLog.Id);

            if (existing != null)
            {
                existing.IsProcessed = true;
                existing.ProcessedDate = DateTime.UtcNow;

                Repository.Update(existing);
                await Repository.SaveChangesAsync();
                await _unitOfWork.Commit();
            }
        }

     
        public IEnumerable<PhysicianDocLog> GetPhysicianDocs()
        {
            var docs = _physicianDocLogRepository.GetAll().AsEnumerable().Where(c=>c.IsDeleted==false);
            return docs;
        }

        public int DeleteDocument(DocumentLog documentLog)
        {
            try
            {
                //DocumentLog document = Repository.GetAll().Where(vs => vs.IsProcessed == false).FirstOrDefault();
                //document.IsDeleted = true;
                //Repository.SaveChangesAsync();
                //_unitOfWork.Commit();

                var existing = Repository.GetAll().SingleOrDefault(c => c.Id == documentLog.Id);

                if (existing != null)
                {
                    existing.IsDeleted = true;

                    Repository.Update(existing);
                    Repository.SaveChangesAsync();
                    _unitOfWork.Commit();
                }
                return 1;
            }
            catch (Exception ee)
            {
                return 0;
            }

        }

        public int DeletePhysicianDocument(Guid id)
        {
            try
            {
                var existing = _physicianDocLogRepository.GetAll().SingleOrDefault(c => c.Id == id);

                if (existing != null)
                {
                    existing.IsDeleted = true;

                    _physicianDocLogRepository.Update(existing);
                    _physicianDocLogRepository.SaveChangesAsync();
                    _unitOfWork.Commit();
                }
                return 1;
            }
            catch (Exception ee)
            {
                return 0;
            }

        }
    }
}
