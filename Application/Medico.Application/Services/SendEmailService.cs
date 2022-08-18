using AutoMapper;
using Medico.Application.Helpers;
using Medico.Application.Interfaces;
using Medico.Application.ViewModels;
using Medico.Domain.Interfaces;
using Microsoft.AspNetCore.Hosting;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using SendGrid;
using SendGrid.Helpers.Mail;
using System;
using System.IO;
using System.Threading.Tasks;

namespace Medico.Application.Services
{
    public class SendEmailService : ISendEmailService
    {
        #region DI
        private readonly IMapper _mapper;
        private readonly IOptions<SendEmailViewModel> _mailSettings;
        private readonly IHostingEnvironment _environment;
        private readonly IEmailRepository _emailRepository;

        public SendEmailService(
            IMapper mapper,
            IOptions<SendEmailViewModel> mailSettings,
            IEmailRepository emailRepository,
            IHostingEnvironment environment)
        {
            _environment = environment;
            _mailSettings = mailSettings;
            _emailRepository = emailRepository;
            _mapper = mapper;
        }
        #endregion

        #region Methods
        public async Task Execute(EmailViewModel email, EmailAccountViewModel emailAccount)
        {
            try
            {
                if (emailAccount == null)
                {
                    throw new Exception("Email Account is not configured.");
                }

                var mailSettingsValue = _mailSettings.Value;
                var apiKey = mailSettingsValue.ApiKey; //emailAccount.SendGridKey;
                var client = new SendGridClient(apiKey);
                var from = new EmailAddress(emailAccount.FromEmail, "");
                var subject = email.Subject;
                var to = new EmailAddress(email.To, $"{emailAccount.FromName}");
                
                string emailBody = email.Body;
                if (email.HashValues != null)
                {
                    var fileName = Path.Combine(_environment.ContentRootPath, "EmailTemplates", string.Format("{0}.html", email.TemplateName));
                    FileInfo file__1 = new FileInfo(fileName);
                    if (file__1.Exists)
                    {

                    }
                    emailBody = Utility.GetContentFromTemplate(email.HashValues, fileName);
                }
                var plainTextContent = emailBody;
                var msg = MailHelper.CreateSingleEmail(from, to, subject, "", emailBody);
                var response = await client.SendEmailAsync(msg);

                // CC
                if (email.CcList != null)
                {
                    var cc = new EmailAddress(email.CcList, $"{emailAccount.FromName}");
                    var msg2 = MailHelper.CreateSingleEmail(from, cc, subject, "", emailBody);
                    var response2 = await client.SendEmailAsync(msg2);
                }

                // BCC
                if (emailAccount.Bcc != null)
                {
                    var bcc = new EmailAddress(emailAccount.Bcc, $"{emailAccount.FromName}");
                    var msg3 = MailHelper.CreateSingleEmail(from, bcc, subject, "", emailBody);
                    var response3 = await client.SendEmailAsync(msg3);
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public async Task<EmailAccountViewModel> GetEmailAccount()
        {
            var all = _emailRepository.GetAll();
            var emailAccount = await all.FirstOrDefaultAsync();

            return _mapper.Map<EmailAccountViewModel>(emailAccount);
        }

        public async Task SendEmailAsync(string emailTo, string subject, string smtpMsg)
        {
            var mailSettingsValue = _mailSettings.Value;
            var client = new SendGridClient(mailSettingsValue.ApiKey);
            var from = new EmailAddress(mailSettingsValue.From, "Administrator");
            var to = new EmailAddress(emailTo, "Patient User");
            var msg = MailHelper.CreateSingleEmail(from, to, subject, smtpMsg, smtpMsg);
            var response = await client.SendEmailAsync(msg);
        }
        #endregion
    }
}
