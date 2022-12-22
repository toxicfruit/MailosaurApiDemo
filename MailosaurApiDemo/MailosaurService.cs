using Mailosaur.Models;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using System;
using System.Threading.Tasks;

namespace MailosaurApiDemo
{
    public interface IMailosaurService
    {
        Task GetMessageAsync(DateTime startTime, Guid messageId);
    }

    public class MailosaurService : IMailosaurService
    {
        private readonly IMailosaurClient client;
        private readonly MailosaurOptions options;
        private readonly ILogger<MailosaurService> logger;

        public MailosaurService(IOptions<MailosaurOptions> options, IMailosaurClient client, ILogger<MailosaurService> logger)
        {
            this.client = client ?? throw new ArgumentNullException(nameof(client));
            this.options = options.Value;
            this.logger = logger;
        }

        public async Task GetMessageAsync(DateTime startTime, Guid messageId)
        {
            var criteria = new SearchCriteria()
            {
                SentTo = $"{options.SendTo}@" + options.ServerId,
                Subject = $"{messageId}"
                // Subject = "Closing Document Package"
            };

            try
            {
                logger.LogInformation("Waiting for email.");
                var email = await client.GetMessageAsync(options.ServerId, criteria, (int)TimeSpan.FromSeconds(60).TotalMilliseconds, startTime);

                // If we have an email, print the subject
                logger.LogInformation("Email has been received successfully.");
                logger.LogInformation("Subject: " + email.Subject);
            }
            catch (MailosaurException ex)
            {
                // No message found
                logger.LogError(ex, "No message received.");
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Oops, something went wrong.");
            }
        }

    }
}
