namespace Chat.SignalR.Models
{
    public class ChatMessage
    {
        public string UserName { get; set; }
        public string Message { get; set; }
        public DateTime Timestamp { get; set; }
    }
}
