using Microsoft.Extensions.Logging;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace MailosaurApiDemo
{
    public interface IRunnerService
    {
        Task RunAsync();
    }

    public class DemoService : IRunnerService
    {
        private readonly ISendgridService sendgrid;
        private readonly IMailosaurService mailosaur;
        private readonly ILogger<DemoService> logger;

        public DemoService(ISendgridService sendgrid, IMailosaurService mailosaur, ILogger<DemoService> logger)
        {
            this.sendgrid = sendgrid;
            this.mailosaur = mailosaur;
            this.logger = logger;
        }

        public async Task RunAsync()
        {
            logger.LogDebug("Starting demo.");

            var now = DateTime.Now;
            var testId = Guid.NewGuid();

            // Create two tasks, one will wait for 10 seconds then send an email,
            // another one will wait for email message for 60 seconds. The second task
            // will complete as soon as the message received.
            await Task.WhenAll(new Task[]
            {
                SendMessageTask(now, testId),
                GetMessageTask(now, testId)
            });

            logger.LogInformation("All done.");
        }

        private async Task SendMessageTask(DateTime demoStartTime, Guid testId)
        {
            logger.LogInformation("Waiting 10 seconds before sending email.");
            await Task.Delay(TimeSpan.FromSeconds(10));
            logger.LogInformation("Sending email.");
            await sendgrid.SendMessageAsync(demoStartTime, testId);
        }

        private Task GetMessageTask(DateTime demoStartTime, Guid testId)
        {
            return mailosaur.GetMessageAsync(demoStartTime, testId);
        }
    }
}