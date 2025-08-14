using MediatR;
using ErrSendApplication.Interfaces.Telegram;

namespace ErrSendApplication.Handlers.Commands.SendTelegramMessage
{
    public class SendTelegramMessageHandler : IRequestHandler<SendTelegramMessageCommand, bool>
    {
        private readonly ITelegramService telegramService;
        public SendTelegramMessageHandler(ITelegramService telegramService)
        {
            this.telegramService = telegramService;
        }
        public async Task<bool> Handle(SendTelegramMessageCommand request, CancellationToken cancellationToken)
        {
            await telegramService.SendErrorMessageAsync(request.Request.Message, request.Request.AdditionalInfo);
            return true;
        }
    }
} 