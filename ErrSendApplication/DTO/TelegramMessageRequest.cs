namespace ErrSendApplication.DTO
{
    public class TelegramMessageRequest
    {
        /// <summary>
        /// Message to send
        /// </summary>
        public required string Message { get; set; }

        /// <summary>
        /// Additional info
        /// </summary>
        public string? AdditionalInfo { get; set; }
    }
} 