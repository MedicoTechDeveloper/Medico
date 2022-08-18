using AutoMapper;
using AutoMapper.QueryableExtensions;
using Dapper;
using Medico.Application.Interfaces;
using Medico.Application.ViewModels;
using Medico.Domain.Interfaces;
using Medico.Domain.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Threading.Tasks;

namespace Medico.Application.Services
{
    public class LabTestService : BaseDeletableByIdService<LabTest, LabTestViewModel>, ILabTestService
    {
        #region DI

        private readonly IPatientOrderRepository _patientOrderRepository;
        private readonly IPatientRepository _patientRepository;
        private readonly IUserService _userService;
        private readonly IInsuranceService _insuranceService;
        private readonly IVendorDataService _vendorDataService;
        public IDbConnection Connection => new SqlConnection(_configuration.GetConnectionString("DefaultConnection"));
        private readonly IPatientOrderItemRepository _patientOrderItemRepository;
        private readonly IConfiguration _configuration;
        public LabTestService(
           IPatientOrderRepository patientOrderRepository,
           IPatientRepository patientRepository,
           IPatientOrderItemRepository patientOrderItemRepository,
           ILabTestRepository labOrderRepository,
           IVendorDataService vendorDataService,
           IUserService userService,
           IInsuranceService insuranceService,
           IMapper mapper, IConfiguration configuration) : base(labOrderRepository,
           mapper)
        {
            _patientOrderRepository = patientOrderRepository;
            _patientOrderItemRepository = patientOrderItemRepository;
            _patientRepository = patientRepository;
            _vendorDataService = vendorDataService;
            _userService = userService;
            _insuranceService = insuranceService;
            _configuration = configuration;
        }

        #endregion

        #region Lab Tests

        public async Task<LabTestViewModel> GetExisting(string testName)
        {
            var labTests = await Repository.GetAll()
               .FirstOrDefaultAsync(a => a.TestName == testName);

            var labTestModel = labTests == null
               ? null
               : Mapper.Map<LabTestViewModel>(labTests);

            return labTestModel;
        }

        public async Task<IEnumerable<LabTestViewModel>> Search(int categoryId, string filter)
        {
            var category = "";
            if (categoryId == 1)
            {
                category = "In house";
            }
            if (categoryId == 2)
            {
                category = "Lab Tests";
            }
            if (categoryId == 3)
            {
                category = "Imaging";
            }
            if (string.IsNullOrEmpty(filter))
            {
                return await Repository.GetAll()
                 .Where(a => a.Category == category)
                 .OrderBy(a => a.TestName).ProjectTo<LabTestViewModel>()
                 .ToListAsync();
            }
            var labTests = await Repository.GetAll()
                .Where(a => a.TestName == filter && a.Category == category)
                .Where(a => a.TestName == filter)
                .OrderBy(a => a.TestName).ProjectTo<LabTestViewModel>()
                .ToListAsync();

            return labTests;
        }

        public async Task<IEnumerable<LabTestGrouped>> GetMostOrdered(int categoryId, int maxCount)
        {
            var category = "";
            if (categoryId == 1)
            {
                category = "In house";
            }
            if (categoryId == 2)
            {
                category = "Lab Tests";
            }
            if (categoryId == 3)
            {
                category = "Imaging";
            }
            var orderItems = await _patientOrderItemRepository.GetAll().ToListAsync();
            var labTests = await Repository.GetAll()
                .Where(c => c.Category == category)
                .ToListAsync();

            var orders = from oit in orderItems
                         join lt in labTests on oit.LabTestId equals lt.Id
                         select new { lt.TestName };

            List<LabTestGrouped> testGroupeds = new List<LabTestGrouped>();
            foreach (var line in orders.GroupBy(info => info.TestName)
             .Select(group => new
             {
                 TestName = group.Key,
                 Count = group.Count()
             })
             .OrderBy(x => x.TestName))
                testGroupeds.Add(new LabTestGrouped
                {
                    TestName = line.TestName,
                    Count = line.Count
                });

            return testGroupeds
                .OrderByDescending(c => c.Count)
                .Take(maxCount).Skip(0);
        }

        public async Task<IEnumerable<LabTestViewModel>> GetAllLabTest()
        {
            var labTests = await Repository.GetAll().ToListAsync();

            var labTest = from lb in labTests
                         select new LabTestViewModel
                         {
                             Id = lb.Id,
                             TestName = lb.TestName,
                             TestCode = lb.TestCode,
                             CodeType = lb.CodeType,
                             TestFee = lb.TestFee,
                             Description = lb.Description,
                             IsAvailableInHouse = lb.IsAvailableInHouse,
                             Category = lb.Category,
                             Procedure = lb.Procedure,
                         };
            return labTest;
        }

        public async Task<IEnumerable<LabTestViewModel>> GetTestById(Guid id)
        {
            using (IDbConnection con = Connection)
            {
                var labTests = await Repository.GetAll().Where(c => c.Id == id).ToListAsync();
                var labTest = from lb in labTests
                              select new LabTestViewModel
                              {
                                  Id = lb.Id,
                                  TestName = lb.TestName,
                                  TestCode = lb.TestCode,
                                  CodeType = lb.CodeType,
                                  TestFee = lb.TestFee,
                                  Description = lb.Description,
                                  IsAvailableInHouse = lb.IsAvailableInHouse,
                                  Category = lb.Category,
                                  Procedure = lb.Procedure,
                                  MedicoCode = lb.MedicoCode,
                              };
                return labTest;
            }
        }
        public async Task<bool> EditLabTest(LabTestViewModel labTestViewModel)
        {
            var labTest = Mapper.Map<LabTest>(labTestViewModel);
            Repository.Update(labTest);
            await Repository.SaveChangesAsync();
            return true;
        }

        public async Task<bool> EditLabTestFee(string id, LabTestFee fee)
        {
            using (IDbConnection con = Connection)
            {
                con.Open();

                string q = @"Update [dbo].[LabTest]
                             set TestFee=@fee WHERE Id=@id";

                var data = await con.ExecuteAsync(q,
                        new
                        {
                            id,
                            fee = fee.TestFee
                        });
                return true;
            }
        }

        public async Task<bool> DeleteLabTest(string id)
        {
            using (IDbConnection con = Connection)
            {
                con.Open();

                string q = @"Delete From [dbo].[LabTest]
                                      WHERE Id=@id";

                var data = await con.ExecuteAsync(q,
                        new
                        {
                            id
                        });
                return true;
            }
        }

        #endregion

        #region Patient Order
        public async Task<PatientOrderViewModel> CreatePatientOrder(PatientOrderViewModel patientOrderViewModel)
        {
            patientOrderViewModel.OrderNumber = await GenerateOrderNo();
            var patientOrder = Mapper.Map<PatientOrder>(patientOrderViewModel);
            await _patientOrderRepository.AddAsync(patientOrder);
            await _patientOrderRepository.SaveChangesAsync();

            patientOrderViewModel.Id = patientOrder.Id;
            return patientOrderViewModel;
        }

        public async Task<IEnumerable<PatientOrderSearch>> GetPatientOrders(Guid pid, Guid cid,string categoryId)
        {
            IEnumerable<PatientOrder> allPatientOrders;
            List<PatientOrderSearch> orders = new List<PatientOrderSearch>();
            if (pid != null)
            {
                allPatientOrders = await _patientOrderRepository.GetAll()
               .Where(a => a.PatientId == pid)
               .OrderByDescending(a => a.CreatedOn)
               .ToListAsync();

                var vendors = await _vendorDataService.GetVendorDdl();
                var physicians = _userService.Lookup(new UserDxOptionsViewModel
                {
                    CompanyId = cid,
                    EmployeeType = 1
                });

                var insurance = _insuranceService.GetAllCompanies();

                var patientOrders = from po in allPatientOrders
                                    join v in vendors on po.VendorId equals v.Id
                                    join ph in physicians on po.PhysicianId equals ph.Id
                                    join ins in insurance on po.InsuranceId equals ins.Id into gj
                                    from subpet in gj.DefaultIfEmpty()
                                    orderby po.CreatedOn descending
                                    select new PatientOrderSearch
                                    {
                                        Id = po.Id,
                                        DateOrdered = po.DateOrdered,
                                        AttachmentId = po.AttachmentId,
                                        CreatedBy = po.CreatedBy,
                                        CreatedOn = po.CreatedOn,
                                        Notes = po.Notes,
                                        OrderNumber = po.OrderNumber,
                                        PatientId = po.PatientId,
                                        OrderStatus = po.OrderStatus,
                                        Vendor = v.VendorName,
                                        Physician = ph.Name,
                                        Insurance = subpet?.Name ?? "No Insurance Taken"
                                    };

                foreach (var item in patientOrders)
                {
                    var items = await GetOrderItems(item.Id, categoryId);
                    orders.Add(new PatientOrderSearch
                    {
                        Id = item.Id,
                        DateOrdered = item.DateOrdered,
                        AttachmentId = item.AttachmentId,
                        CreatedBy = item.CreatedBy,
                        CreatedOn = item.CreatedOn,
                        Notes = item.Notes,
                        OrderNumber = item.OrderNumber,
                        PatientId = item.PatientId,
                        OrderStatus = item.OrderStatus,
                        Vendor = item.Vendor,
                        Physician = item.Physician,
                        Insurance = item.Insurance,
                        ItemString = string.Join(",", items)
                    });
                }
            }
            return orders;
        }

        public async Task<IEnumerable<PatientOrderGrouped>> GetAllOrders(string patientId, string companyId,string categoryId,int statusId, string physicianId)
        {
            var patientOrders = await _patientOrderRepository.GetAll().ToListAsync();
            var patients = await _patientRepository.GetAll().ToListAsync();
            var insurance = _insuranceService.GetAllCompanies();

            var compId = new Guid(companyId);

            var orders = from po in patientOrders
                         join pat in patients on po.PatientId equals pat.Id
                         join ins in insurance on po.InsuranceId equals ins.Id into pGroup
                         from ins in pGroup.DefaultIfEmpty()
                         orderby po.CreatedOn descending
                         where pat.CompanyId == compId
                         select new PatientOrderGrouped
                         {
                             PatientId = pat.Id,
                             PatientName = $"{ pat.FirstName} { pat.LastName}",
                             OrderNumber = po.OrderNumber,
                             OrderId = po.Id,
                             OrderStatus = po.OrderStatus,
                             CreatedOn = po.CreatedOn,
                             InsuranceId = po.InsuranceId,
                             VendorId = po.VendorId,
                             PhysicianId = po.PhysicianId,
                             Notes = po.Notes,
                             ReferenceNo = po.ReferenceNo,
                             Insurance = ins == null ? "No Insurance Taken" : ins.Name
                         };

            if (!string.IsNullOrEmpty(patientId))
            {
                var patId = new Guid(patientId);
                orders = orders.Where(c => c.PatientId == patId);
            }
            if (!string.IsNullOrEmpty(physicianId))
            {
                var phyId = new Guid(physicianId);
                orders = orders.Where(c => c.PhysicianId == phyId);
            }

            if (statusId > 0)
            {
                orders = orders.Where(c => c.OrderStatus == statusId);
            }

            List<PatientOrderGrouped> orderList = new List<PatientOrderGrouped>();
            foreach (var item in orders)
            {
                var patientOrderItems = await _patientOrderItemRepository.GetAll().Where(c => c.PatientOrderId == item.OrderId).ToListAsync();
                var labTests = await Repository.GetAll().ToListAsync();

                var orderItems = from ordItem in patientOrderItems
                                 join tst in labTests on ordItem.LabTestId equals tst.Id
                                 select new PatientOrderItemModel1
                                 {
                                     Id = ordItem.Id,
                                     LabTestName = tst.TestName,
                                     LabTestCode = tst.TestCode,
                                     LabTestFee = tst.TestFee,
                                     Quantity = ordItem.Quantity,
                                     IsCompleted = ordItem.IsCompleted,
                                     CompletedOn = ordItem.CompletedOn,
                                     PatientOrderId = ordItem.PatientOrderId,
                                     LabTestId = ordItem.LabTestId,
                                     Status = ordItem.Status,
                                     Category = tst.Category,
                                 };

                if (!string.IsNullOrEmpty(categoryId))
                {
                    orderItems = orderItems.Where(c => c.Category == categoryId);
                }

                var items = await GetOrderItems(item.OrderId);

                orderList.Add(new PatientOrderGrouped
                {
                    CreatedOn = item.CreatedOn,
                    PatientName = item.PatientName,
                    OrderNumber = item.OrderNumber,
                    OrderId = item.OrderId,
                    OrderStatus = item.OrderStatus,
                    ItemString = string.Join(",", items),
                    OrderItems = orderItems,
                    InsuranceId = item.InsuranceId,
                    VendorId = item.VendorId,
                    PhysicianId = item.PhysicianId,
                    Notes = item.Notes,
                    ReferenceNo = item.ReferenceNo,
                    Insurance = item.Insurance
                });
            }

            return orderList;
        }

        public async Task<bool> PutOrder(PatientOrderUpdateModel patientOrderUpdateModel)
        {

            // update patient order
            var entity = await _patientOrderRepository.GetAll().FirstOrDefaultAsync(c => c.Id == patientOrderUpdateModel.Id);

            entity.DateOrdered = patientOrderUpdateModel.DateOrdered;
            entity.PhysicianId = patientOrderUpdateModel.PhysicianId;
            entity.InsuranceId = patientOrderUpdateModel.InsuranceId.Value;
            entity.ReferenceNo = patientOrderUpdateModel.ReferenceNo;
            entity.VendorId = patientOrderUpdateModel.VendorId ?? 0;
            entity.Notes = patientOrderUpdateModel.Notes;

            _patientOrderRepository.Update(entity);
            await _patientOrderRepository.SaveChangesAsync();

            // update item status
            int completeCount = 0;
            foreach (var item in patientOrderUpdateModel.PatientOrderItems)
            {
                if (item.Status == 3)
                {
                    completeCount++;
                }
                var patientOrderItem = Mapper.Map<PatientOrderItem>(item);
                _patientOrderItemRepository.Update(patientOrderItem);
            }

            // If all order item are completed, update status
            if (completeCount == patientOrderUpdateModel.PatientOrderItems.Count())
            {
                var orderId = patientOrderUpdateModel.PatientOrderItems.FirstOrDefault().PatientOrderId;
                var patientOrder = await _patientOrderRepository.GetAll().FirstOrDefaultAsync(c => c.Id == orderId);
                patientOrder.OrderStatus = 3;

                _patientOrderRepository.Update(patientOrder);
                await _patientOrderRepository.SaveChangesAsync();
            }

            await _patientOrderItemRepository.SaveChangesAsync();
            return true;

        }

        public async Task<bool> PutOrderStatus(IEnumerable<PatientOrderItemModel> patientOrderItemModel)
        {
            int completeCount = 0;
            foreach (var item in patientOrderItemModel)
            {
                if (item.Status == 3)
                {
                    completeCount++;
                }
                var patientOrderItem = Mapper.Map<PatientOrderItem>(item);
                _patientOrderItemRepository.Update(patientOrderItem);
            }

            // If all order item are completed, update status
            if (completeCount == patientOrderItemModel.Count())
            {
                var orderId = patientOrderItemModel.FirstOrDefault().PatientOrderId;
                var patientOrder = await _patientOrderRepository.GetAll().FirstOrDefaultAsync(c => c.Id == orderId);
                patientOrder.OrderStatus = 3;

                _patientOrderRepository.Update(patientOrder);
                await _patientOrderRepository.SaveChangesAsync();
            }

            await _patientOrderItemRepository.SaveChangesAsync();
            return true;
        }
        #endregion

        public async Task<List<string>> GetOrderItems(Guid id)
        {
            var patientOrderItems = await _patientOrderItemRepository.GetAll()
                .Where(c => c.PatientOrderId == id).ToListAsync();

            List<string> items = new List<string>();
            foreach (var item in patientOrderItems)
            {
                var labTest = Repository.GetAll().FirstOrDefault(c => c.Id == item.LabTestId);
                items.Add(labTest.TestName);
            }

            return items;
        }

        public async Task<List<string>> GetOrderItems(Guid id,string catId)
        {
            var patientOrderItems = await _patientOrderItemRepository.GetAll()
                .Where(c => c.PatientOrderId == id).ToListAsync();
            var labTests = await Repository.GetAll().ToListAsync();

            var orderItems = from ordItem in patientOrderItems
                             join tst in labTests on ordItem.LabTestId equals tst.Id
                             select new PatientOrderItemModel1
                             {
                                 Id = ordItem.Id,
                                 LabTestName = tst.TestName,
                                 LabTestCode = tst.TestCode,
                                 LabTestFee = tst.TestFee,
                                 Quantity = ordItem.Quantity,
                                 IsCompleted = ordItem.IsCompleted,
                                 CompletedOn = ordItem.CompletedOn,
                                 PatientOrderId = ordItem.PatientOrderId,
                                 LabTestId = ordItem.LabTestId,
                                 Status = ordItem.Status,
                                 Category = tst.Category,
                             };

            if (!string.IsNullOrEmpty(catId))
            {
                orderItems = orderItems.Where(c => c.Category == catId);
            }

            List<string> items = new List<string>();
            foreach (var item in orderItems)
            {
                var labTest = Repository.GetAll().FirstOrDefault(c => c.Id == item.LabTestId);
               
                items.Add(labTest.TestName);
            }

            return items;
        }

        public async Task<IEnumerable<PatientOrderGrouped>> GetOrdersById(Guid id, Guid cid)
        {
            var patientOrders = await _patientOrderRepository.GetAll().Where(a => a.Id == id).ToListAsync();
            var patients = await _patientRepository.GetAll().ToListAsync();
            var insurance = _insuranceService.GetAllCompanies();

            var compId = cid;

            var orders = from po in patientOrders
                         join pat in patients on po.PatientId equals pat.Id
                         join ins in insurance on po.InsuranceId equals ins.Id into pGroup
                         from ins in pGroup.DefaultIfEmpty()
                         orderby po.CreatedOn descending
                         where pat.CompanyId == compId
                         select new PatientOrderGrouped
                         {
                             PatientId = pat.Id,
                             PatientName = $"{ pat.FirstName} { pat.LastName}",
                             OrderNumber = po.OrderNumber,
                             OrderId = po.Id,
                             PhysicianId = po.PhysicianId,
                             VendorId = po.VendorId,
                             OrderStatus = po.OrderStatus,
                             CreatedOn = po.CreatedOn,
                             DateOrdered = po.DateOrdered,
                             Notes = po.Notes,
                             ReferenceNo = po.ReferenceNo,
                             InsuranceId = po.InsuranceId,
                             Insurance = ins == null ? "No Insurance Taken" : ins.Name
                         };

            List<PatientOrderGrouped> orderList = new List<PatientOrderGrouped>();
            foreach (var item in orders)
            {
                var patientOrderItems = await _patientOrderItemRepository.GetAll().Where(c => c.PatientOrderId == item.OrderId).ToListAsync();
                var labTests = await Repository.GetAll().ToListAsync();

                var orderItems = from ordItem in patientOrderItems
                                 join tst in labTests on ordItem.LabTestId equals tst.Id
                                 select new PatientOrderItemModel1
                                 {
                                     Id = ordItem.Id,
                                     LabTestName = tst.TestName,
                                     LabTestCode = tst.TestCode,
                                     LabTestFee = tst.TestFee,
                                     Quantity = ordItem.Quantity,
                                     IsCompleted = ordItem.IsCompleted,
                                     CompletedOn = ordItem.CompletedOn,
                                     PatientOrderId = ordItem.PatientOrderId,
                                     LabTestId = ordItem.LabTestId,
                                     Status = ordItem.Status,
                                 };

                var items = await GetOrderItems(item.OrderId);

                orderList.Add(new PatientOrderGrouped
                {
                    CreatedOn = item.CreatedOn,
                    PatientName = item.PatientName,
                    OrderNumber = item.OrderNumber,
                    OrderId = item.OrderId,
                    OrderStatus = item.OrderStatus,
                    ItemString = string.Join(",", items),
                    OrderItems = orderItems,
                    Insurance = item.Insurance,
                    PhysicianId = item.PhysicianId,
                    DateOrdered = item.DateOrdered,
                    Notes = item.Notes,
                    ReferenceNo = item.ReferenceNo,
                    VendorId = item.VendorId,
                    InsuranceId = item.InsuranceId
                });
            }

            return orderList;
        }

        public async Task<bool> DeleteOrder(string id)
        {
            using (IDbConnection con = Connection)
            {
                con.Open();

                string q = @"Delete From [dbo].[PatientOrder]
                                      WHERE Id=@id
                                   
                             Delete From [dbo].[PatientOrderItem] where PatientOrderId=@id";

                var data = await con.ExecuteAsync(q,
                        new
                        {
                            id
                        });
                return true;
            }
        }

        public async Task<string> GenerateOrderNo()
        {
            try
            {
                string cInitial = "PO";
                using (IDbConnection con = Connection)
                {
                    con.Open();

                    var maxId =
                      await con.QuerySingleAsync<int>(@"SELECT ISNULL(MAX(ID)+1,0)  FROM Notifications",
                     null, commandType: CommandType.Text);

                    if (Convert.ToString(maxId).Length == 1)
                    {
                        return string.Format("{0}{1}0000{2}", cInitial, DateTime.Now.Year.ToString().Substring(2), maxId);
                    }
                    if (Convert.ToString(maxId).Length == 2)
                    {
                        return string.Format("{0}{1}000{2}", cInitial, DateTime.Now.Year.ToString().Substring(2), maxId);
                    }
                    if (Convert.ToString(maxId).Length == 3)
                    {
                        return string.Format("{0}{1}00{2}", cInitial, DateTime.Now.Year.ToString().Substring(2), maxId);
                    }
                    if (Convert.ToString(maxId).Length == 4)
                    {
                        return string.Format("{0}{1}0{2}", cInitial, DateTime.Now.Year.ToString().Substring(2), maxId);
                    }

                    return string.Empty;
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        Task<List<string>> ILabTestService.GetOrderItems(Guid id, string catId)
        {
            throw new NotImplementedException();
        }
    }
}
