namespace MailosaurApiDemo
{
    public class SendgridOptions
    {
        public string ApiKey { get; set; }
        public string From { get; set; }
        public string To { get; set; }
        public string TemplateId { get; set; }
    }
}