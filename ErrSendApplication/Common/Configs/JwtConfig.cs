namespace ErrSendApplication.Common.Configs
{
    public class JwtConfig
    {
        public string Secret { get; set; }
        public int ExpiryMinutes { get; set; }
    }
} 