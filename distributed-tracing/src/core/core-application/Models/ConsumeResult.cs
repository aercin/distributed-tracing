namespace core_application.Models
{
    public class ConsumeResult
    {
        public string Message { get; set; }
        public List<MessageHeader> Headers { get; set; } = new List<MessageHeader>();
    }
}
