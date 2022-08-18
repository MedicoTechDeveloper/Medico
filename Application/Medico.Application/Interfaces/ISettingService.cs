using Medico.Application.ViewModels;
using System.Threading.Tasks;

namespace Medico.Application.Interfaces
{
    public interface ISettingService
    {
        Task<EditorConfigVM> UpdateEditorConfig(string id, EditorConfigVM editorConfig);
        Task<EditorConfigVM> GetEditorConfig();
    }
}
