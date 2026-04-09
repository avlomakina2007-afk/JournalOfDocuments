using System;

namespace JournalOfDocuments
{
    public class Document
    {
        public int Id { get; set; }
        public string IncomingNumber { get; set; } = "";
        public DateTime DateReceived { get; set; }
        public string Sender { get; set; } = "";
        public string Subject { get; set; } = "";
        public string Executor { get; set; } = ""; // исполнитель в адм. отделе
    }
}