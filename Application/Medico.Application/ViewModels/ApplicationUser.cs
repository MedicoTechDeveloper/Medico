using System;
using System.Collections.Generic;

namespace Medico.Application.ViewModels
{
    public class ApplicationUserViewModel
    {
        public ApplicationUserViewModel()
        {
            Roles = new List<string>();
            Errors = new List<string>();
        }

        public bool IsAuthenticated { get; set; }

        public List<string> Roles { get; }

        public List<string> Errors { get; }

        public Guid? CompanyId { get; set; }
        public string Email { get; set; }
        public string Username { get; set; }
        public string Token { get; set; }
        public string FullName { get; set; }
    }

    public class UserModel
    {
        public string Id { get; set; }
        public string Username { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
    }
}
