using ErrSendApplication.Interfaces.Telegram;
using ErrSendApplication.DTO;
using Microsoft.AspNetCore.Mvc;
using ErrSendApplication.Interfaces.Authorization;
using Microsoft.AspNetCore.Authorization;

namespace ErrSendWebApi.Controllers
{
    public class TestController : BaseController
    {
        private readonly ITelegramService telegramService;
        private readonly IJwtTokenService jwtTokenService;

        public TestController(ITelegramService telegramService, IJwtTokenService jwtTokenService)
        {
            this.telegramService = telegramService;
            this.jwtTokenService = jwtTokenService;
        }

        /// <summary>
        /// Відправити тестове повідомлення в Telegram
        /// </summary>
        /// <param name="message">Текст повідомлення</param>
        /// <param name="additionalInfo">Додаткова інформація</param>
        [Authorize(Roles = "Admin,User")]
        [HttpPost("Відправка тестового повідомлення в Telegram")]
        public async Task<ActionResult> SendTelegramMessage([FromBody] TelegramMessageRequest request)
        {
            await telegramService.SendErrorMessageAsync(request.Message, request.AdditionalInfo);
            return Ok(new { success = true, message = "Повідомлення відправлено" });
        }

        /// <summary>
        /// Кинути тестову помилку для перевірки middleware
        /// </summary>
        /// <param name="errorMessage">Текст помилки</param>
        [Authorize(Roles = "Admin,User")]
        [HttpPost("Текст помилки")]
        public ActionResult ThrowError([FromBody] ErrorRequest request)
        {
            throw new Exception(request.ErrorMessage);
        }

        /// <summary>
        /// Згенерувати JWT токен (login, password, roles)
        /// </summary>
        [HttpPost("jwt-token")]
        [ProducesResponseType(typeof(JwtTokenResponse), 200)]
        public ActionResult GetJwtToken([FromBody] JwtTokenRequest request)
        {
            var token = jwtTokenService.GenerateToken(request.Login, request.Password, request.Roles);
            return Ok(new JwtTokenResponse { Token = token });
        }
    }
} 