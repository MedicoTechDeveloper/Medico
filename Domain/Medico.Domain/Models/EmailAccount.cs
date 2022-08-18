using System;

namespace Medico.Domain.Models
{
    public class EmailAccount: Entity
    {
        public string Username { get; set; }
        public string Password { get; set; }
        public string HostName { get; set; }
        public short? PortNo { get; set; }
        public string FromName { get; set; }
        public string FromEmail { get; set; }
        public string SendGridKey { get; set; }
        public string Bcc { get; set; }
        public string AccountName { get; set; }
        public bool IsDefault { get; set; }
        public Guid CompanyId { get; set; }
        public string AppName { get; set; }
        public string AppLink { get; set; }
    }
}
