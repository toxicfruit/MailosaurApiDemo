using Mailosaur.Models;
using System;
using System.Threading.Tasks;

namespace MailosaurApiDemo
{
    /// <summary>
    /// This interface introduced for testing purposes as Mailosaur.MailosaurClient does not implement any interfaces.
    /// </summary>
    public interface IMailosaurClient
    {
        Task<Message> GetMessageAsync(string server, SearchCriteria criteria, int timeout, DateTime? receivedAfter);
    }
}
