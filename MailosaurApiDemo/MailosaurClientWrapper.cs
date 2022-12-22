using Mailosaur;
using Mailosaur.Models;
using System;
using System.Threading.Tasks;

namespace MailosaurApiDemo
{
    /// <summary>
    /// This class implements IMailosaurClient which is required for testing.
    /// </summary>
    public class MailosaurClientWrapper : IMailosaurClient
    {
        private readonly MailosaurClient client;

        public MailosaurClientWrapper(string apiKey)
        {
            client = new MailosaurClient(apiKey);
        }

        public Task<Message> GetMessageAsync(string server, SearchCriteria criteria, int timeout = 10_000, DateTime? receivedAfter = null)
        {
            return client.Messages.GetAsync(server, criteria, timeout, receivedAfter);
        }
    }
}
