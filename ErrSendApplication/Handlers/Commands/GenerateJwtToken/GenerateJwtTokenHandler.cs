using MediatR;
using ErrSendApplication.Interfaces.Authorization;

namespace ErrSendApplication.Handlers.Commands.GenerateJwtToken
{
    public class GenerateJwtTokenHandler : IRequestHandler<GenerateJwtTokenCommand, string>
    {
        private readonly IJwtTokenService jwtTokenService;
        public GenerateJwtTokenHandler(IJwtTokenService jwtTokenService)
        {
            this.jwtTokenService = jwtTokenService;
        }
        public Task<string> Handle(GenerateJwtTokenCommand request, CancellationToken cancellationToken)
        {
            var token = jwtTokenService.GenerateToken(request.Login, request.Password, request.Roles);
            return Task.FromResult(token);
        }
    }

    
} 