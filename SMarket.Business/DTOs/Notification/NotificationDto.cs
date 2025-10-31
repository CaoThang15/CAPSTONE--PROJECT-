namespace SMarket.Business.DTOs.Notification
{
    public class NotificationDto
    {
        public int Id { get; set; }
        public int SystemNotificationId { get; set; }
        public int Type { get; set; }
        public string Content { get; set; } = string.Empty;
        public bool IsRead { get; set; }
        public int? IdRefer { get; set; }
        public DateTime? SendAt { get; set; }
    }

    public class CreateNotificationDto
    {
        public int SystemNotificationId { get; set; }
        public int Type { get; set; }
        public string Content { get; set; } = string.Empty;
        public int ToUserId { get; set; }
        public int? IdRefer { get; set; }
        public DateTime? SendAt { get; set; }
    }
}
