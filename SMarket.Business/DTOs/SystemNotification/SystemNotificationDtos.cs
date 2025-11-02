namespace SMarket.Business.DTOs.SystemNotification
{
    public class CreateSystemNotificationDto
    {
        public string Content { get; set; } = string.Empty;
        public int Type { get; set; }
        public DateTime TimeToSend { get; set; }
        public bool IsImmediate { get; set; } = false;
        public int? IdRefer { get; set; }
    }

    public class SystemNotificationDto
    {
        public int Id { get; set; }
        public string? Content { get; set; }
        public int Type { get; set; }
        public DateTime TimeToSend { get; set; }
        public bool IsSent { get; set; }
        public int? IdRefer { get; set; }
        public int CreatedByUserId { get; set; }
        public DateTime CreatedAt { get; set; }
    }
}
