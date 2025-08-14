using ErrSendApplication.DTO;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using ErrSendApplication.Handlers.Commands.GenerateJwtToken;
using ErrSendApplication.Handlers.Commands.SendTelegramMessage;

namespace ErrSendWebApi.Controllers
{
    public class TestController : BaseController
    {
        // Всі залежності через MediatR
        public TestController() { }

        /// <summary>
        /// Відправити тестове повідомлення в Telegram
        /// </summary>
        /// <param name="message">Текст повідомлення</param>
        /// <param name="additionalInfo">Додаткова інформація</param>
        [Authorize(Roles = "Admin,User")]
        [HttpPost("Відправка тестового повідомлення в Telegram")]
        public async Task<ActionResult> SendTelegramMessage([FromBody] TelegramMessageRequest request)
        {
            await Mediator.Send(new SendTelegramMessageCommand(request));
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
        [HttpPost("JWT Токен")]
        [ProducesResponseType(typeof(JwtTokenResponse), 200)]
        public async Task<ActionResult> GetJwtToken([FromBody] JwtTokenRequest request)
        {
            var token = await Mediator.Send(new GenerateJwtTokenCommand(request.Login, request.Password, request.Roles));
            return Ok(new JwtTokenResponse { Token = token });
        }
    }
} 