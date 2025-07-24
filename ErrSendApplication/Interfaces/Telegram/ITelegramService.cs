namespace ErrSendApplication.Interfaces.Telegram
{
    public interface ITelegramService
    {
        Task SendErrorMessageAsync(string errorMessage, string? additionalInfo = null);
    }
} 