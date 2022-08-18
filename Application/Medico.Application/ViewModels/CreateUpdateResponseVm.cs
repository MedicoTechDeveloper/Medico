using System.Collections.Generic;

namespace Medico.Application.ViewModels
{
    public class CreateUpdateResponseVm<T>
    {
        private CreateUpdateResponseVm()
        {
        }

        public T Value { get; set; }

        public IEnumerable<string> Errors { get; set; }

        public static CreateUpdateResponseVm<T> CreateSuccessResponse(T value)
        {
            return new CreateUpdateResponseVm<T> {Value = value};
        }

        public static CreateUpdateResponseVm<T> CreateFailedResponse(params string[] errors)
        {
            return new CreateUpdateResponseVm<T> {Errors = errors};
        }
    }
}