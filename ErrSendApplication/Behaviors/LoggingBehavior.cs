using ErrSendApplication.Interfaces.Service;
using MediatR;
using Serilog;

namespace ErrSendApplication.Behaviors
{
    /// <summary>
    /// Клас для поведінки логування через Serilog.
    /// </summary>
    public class LoggingBehavior<TRequest, TResponse> : IPipelineBehavior<TRequest, TResponse> where TRequest : IRequest<TResponse>
    {
        ICurrentService currentService;

        /// <summary>
        /// Конструктор
        /// </summary>
        public LoggingBehavior(ICurrentService currentUserService)
        {
            this.currentService = currentUserService;
        }

        /// <summary>
        /// Метод обробки логування у файл.
        /// </summary>
        public async Task<TResponse> Handle(TRequest request, RequestHandlerDelegate<TResponse> next, CancellationToken cancellationToken)
        {
            var requestName = typeof(TRequest).Name;
            Log.Information("Запити: {Name} --- {@Request} ", requestName, request);

            var response = await next();

            return response;
        }
    }
}
