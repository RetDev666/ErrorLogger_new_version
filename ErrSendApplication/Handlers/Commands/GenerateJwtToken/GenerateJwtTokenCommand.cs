using MediatR;

namespace ErrSendApplication.Handlers.Commands.GenerateJwtToken;

public class GenerateJwtTokenCommand : IRequest<string>
{
    public string Login { get; set; }
    public string Password { get; set; }
    public IEnumerable<string> Roles { get; set; }

    public GenerateJwtTokenCommand(string login, string password, IEnumerable<string> roles)
    {
        Login = login;
        Password = password;
        Roles = roles;
    }
}
