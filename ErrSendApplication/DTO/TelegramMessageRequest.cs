namespace ErrSendApplication.DTO
{
    public class TelegramMessageRequest
    {
        /// <summary>
        /// Повідомлення для відправки
        /// </summary>
        public required string Message { get; set; } 

        /// <summary>
        /// Додаткова інформація
        /// </summary>
        public string? AdditionalInfo { get; set; }
    }
} 