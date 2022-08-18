namespace Medico.Application.ViewModels
{
    public class SelectableListValueViewModel : BaseViewModel
    {
        public string Value { get; set; }

        public string Description { get; set; }

        public bool IsDefault { get; set; }
    }

    public class Docs
    {
        public string JsonValues { get; set; }
    }
}
