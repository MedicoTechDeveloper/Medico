namespace Medico.Application.ViewModels
{
    public class IcdCodeViewModel : BaseViewModel
    {
        private string _code;
        private string _name;
        private string _notes;
        public string Code {
            get
            {
                return _code;
            }
            set
            {
                _code = value;
            }
        }

        public string Name
        {
            get
            {
                return $"{_code}-{_name}";
            }
            set
            {
                _name = value;
            }
        }

        public string Notes {
            get
            {
                return _notes;
            }
            set
            {
                _notes = value;
            }
        }
    }
}
