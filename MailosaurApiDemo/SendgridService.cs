using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using SendGrid;
using SendGrid.Helpers.Mail;
using System;
using System.Threading.Tasks;

namespace MailosaurApiDemo
{
    public interface ISendgridService
    {
        Task SendMessageAsync(DateTime dateTime, Guid testId);
    }

    public class SendgridService : ISendgridService
    {
        private readonly SendgridOptions options;
        private readonly ILogger<SendgridService> logger;
        private readonly ISendGridClient client;

        public SendgridService(IOptions<SendgridOptions> options, ISendGridClient client, ILogger<SendgridService> logger)
        {
            this.options = options.Value;
            this.logger = logger;
            this.client = client ?? new SendGridClient(this.options.ApiKey);
        }

        public async Task SendMessageAsync(DateTime dateTime, Guid testId)
        {

            var from = new EmailAddress(options.From, "Mailosaur API Demo");
            var to = new EmailAddress(options.To, "Mailosaur API Demo");
            var dynamicData = new
            {
                TestId = testId.ToString(),
                TestDateTime = dateTime.ToString()
            };

            var msg = MailHelper.CreateSingleTemplateEmail(from, to, options.TemplateId, dynamicData);

            var response = await client.SendEmailAsync(msg);

            if (response.IsSuccessStatusCode)
            {
                logger.LogInformation("Email has been sent successfully.");
            }
            else
            {
                logger.LogError("Failed to send email.");
            }

        }
    }
}
