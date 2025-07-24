namespace ErrSendApplication.DTO
{
    public class JwtTokenRequest
    {
        public string Login { get; set; }
        public string Password { get; set; }
        public List<string> Roles { get; set; }
    }
} 