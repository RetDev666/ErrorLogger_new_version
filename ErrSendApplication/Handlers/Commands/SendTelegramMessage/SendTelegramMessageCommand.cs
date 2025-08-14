using MediatR;
using ErrSendApplication.DTO;

namespace ErrSendApplication.Handlers.Commands.SendTelegramMessage
{
    /// <summary>
    /// Команда для надсилання повідомлення в Telegram
    /// </summary>
    public class SendTelegramMessageCommand : IRequest<bool>
    {
        public TelegramMessageRequest Request { get; set; }
        public SendTelegramMessageCommand(TelegramMessageRequest request)
        {
            Request = request;
        }
    }
} 