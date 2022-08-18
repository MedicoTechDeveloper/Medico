using Medico.Application.ViewModels;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Medico.Application.Interfaces
{
    public interface IVendorDataService
    {
        Task<int> Create(VendorDataViewModel vendorDataViewModel);

        Task<IEnumerable<VendorDataViewModel>> GetVendorDdl();

        Task<bool> Update(int id, VendorDataViewModel vendorDataViewModel);

        Task<bool> Delete(int id);
    }
}
