namespace ErrSendApplication.DTO
{
    public class JwtTokenRequest
    {
        /// <summary>
        /// User login
        /// </summary>
        public string Login { get; set; }
        /// <summary>
        /// User password
        /// </summary>
        public string Password { get; set; }
        /// <summary>
        /// User roles
        /// </summary>
        public List<string> Roles { get; set; }
    }
} 