using Medico.Application.ViewModels.Enums;
using Medico.Domain.Interfaces;
using Medico.Domain.Models;
using System.Linq;
using System.Threading.Tasks;

namespace Medico.Application.Interfaces
{
    public interface IAuditTrailService 
    {
        Task CreateAuditTrail(AuditActionType Action, string KeyFieldID, string KeyFieldName, object OldObject, object NewObject);
        Task<IOrderedEnumerable<AuditTable>> GetAll(string id, string dataModel);
    }
}
