namespace ErrSendApplication.Interfaces.Authorization
{
    public interface IJwtTokenService
    {
        string GenerateToken(string login, string password, IEnumerable<string> roles);
    }
} 