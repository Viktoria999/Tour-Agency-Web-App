using Auth.Service;
using Domain.Models;
using Logging;
using Microsoft.AspNetCore.Mvc;
using Web_lab.Models;

namespace Web_lab.Controllers
{
    public class AuthController : Controller
    {
        private AccountService _accountService;
        private ILogService _logService;

        public AuthController(AccountService accountService, ILogService logService)
        {
            _accountService = accountService;
            _logService = logService;
        }

        [HttpGet]
        public IActionResult ShowAuthorizationModal()
        {
            var model = new LoginUserRequest();
            return PartialView("~/Views/Shared/Authorization/_AuthorizationModal.cshtml", model);
        }

        [HttpGet]
        public IActionResult ShowRegistrationModal()
        {
            var model = new RegisterUserRequest();
            return PartialView("~/Views/Shared/Authorization/_RegistrationModal.cshtml", model);
        }

        [HttpGet]
        public IActionResult ShowForgotPasswordModal()
        {
            var model = new ForgotPasswordRequest();
            return PartialView("~/Views/Shared/Authorization/_ForgotPasswordModal.cshtml", model);
        }

        [HttpPost]
        public IActionResult Register([FromBody] RegisterUserRequest request)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    return PartialView("~/Views/Shared/Authorization/_RegistrationModal.cshtml", request);
                }


                var model = new UserModel
                {
                    UserName = request.UserName,
                    FirstName = request.FirstName,
                    LastName = request.LastName,
                    MiddleName = request.MiddleName,
                    Email = request.Email,
                    ProfilePictureUrl = request.ProfilePictureUrl
                };
                _accountService.Register(model, request.Password);
            }
            catch (Exception ex)
            {
                _logService.Log("Register", "Auth", $"Пользователь {request.UserName}. Ошибка при регистрации: {ex.Message}", LoggingLevel.Error.ToString(), null);
                return Json(new { Success = false, Message = ex.Message });
            }

            _logService.Log("Register", "Auth", $"Пользователь {request.UserName} успешно зарегистрирован", LoggingLevel.Success.ToString(), null);
            return Json(new { Success = true });
        }

        [HttpPost]
        public IActionResult EditProfile([FromBody] EditProfileUserRequest request)
        {
            try
            {
                var model = new UserModel
                {
                    Id = request.Id,
                    UserName = request.UserName,
                    FirstName = request.FirstName,
                    LastName = request.LastName,
                    MiddleName = request.MiddleName,
                    Email = request.Email,
                    ProfilePictureUrl = request.ProfilePictureUrl
                };
                _accountService.EditProfile(model);
            }
            catch (Exception ex)
            {
                _logService.Log("EditProfile", "Auth", $"Пользователь {request.UserName}. Ошибка при редактировании профиля: {ex.Message}", LoggingLevel.Error.ToString(), null);
                return Json(new { Success = false, Message = ex.Message });
            }

            _logService.Log("EditProfile", "Auth", $"Пользователь {request.UserName} успешно изменил профиль", LoggingLevel.Success.ToString(), null);
            return Json(new { Success = true });
        }

        [HttpPost]

        public IActionResult Login([FromBody] LoginUserRequest request)
        {
            string token;

            try
            {
                token = _accountService.Login(request.UserName, request.Password);
                Response.Cookies.Append("authToken", token, new CookieOptions
                {
                    HttpOnly = true,
                    Secure = true,
                    SameSite = SameSiteMode.Strict,
                    Expires = DateTimeOffset.UtcNow.AddMinutes(30)
                });
            }
            catch (Exception ex)
            {
                _logService.Log("Login", "Auth", $"Пользователь {request.UserName}. Ошибка при авторизации: {ex.Message}", LoggingLevel.Error.ToString(), null);
                return Json(new { Success = false, Message = ex.Message });
            }

            _logService.Log("Login", "Auth", $"Пользователь {request.UserName} успешно авторизован", LoggingLevel.Success.ToString(), null);
            return Json(new { Success = true });
        }

        [HttpPost]
        public IActionResult Logout()
        {
            Response.Cookies.Delete("authToken");

            _logService.Log("Logout", "Auth", $"Пользователь успешно вышел из аккаунта", LoggingLevel.Success.ToString(), null);
            return Json(new { Success = true });
        }

        [HttpPost]
        public IActionResult ForgotPassword([FromBody] ForgotPasswordRequest request)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    return Json(new { Success = false, Message = "Ошибка при заполнении формы" });
                }

                var token = _accountService.GetTokenForPasswordRestoration(request.UserName, request.Email);

                if (token == null)
                {
                    return Json(new { Success = false, Message = "Ошибка при заполнении формы" });
                }

                _logService.Log("ForgotPassword", "Auth", $"Пользователь {request.UserName} успешно отправил запрос на смену пароля", LoggingLevel.Success.ToString(), null);
                return Json(new { Success = true, Token = token });
            }
            catch (Exception ex)
            {
                _logService.Log("ForgotPassword", "Auth", $"Пользователь {request.UserName}. Ошибка при отправлении запроса на смену пароля: {ex.Message}", LoggingLevel.Error.ToString(), null);
                return Json(new { Success = false, Message = ex.Message });
            }
        }

        [HttpGet]
        public IActionResult RestorePassword(string token)
        {
            var principal = _accountService.ValidateToken(token);
            if (principal == null || principal.FindFirst("userName")?.Value == null)
                return View("~/Views/Shared/Error.cshtml");

            var model = new RestorePasswordModel();
            model.UserName = principal.FindFirst("userName")?.Value;

            return View("~/Views/Auth/RestorePassword.cshtml", model);
        }

        [HttpPost]
        public IActionResult RestorePasswordPost([FromBody] RestorePasswordModel model)
        {
            if (!ModelState.IsValid)
            {
                return View("~/Views/Auth/RestorePassword.cshtml", model);
            }

            try
            {
                _accountService.RestorePassword(model.NewPassword, model.UserName);
            }
            catch (Exception ex)
            {
                _logService.Log("RestorePasswordPost", "Auth", $"Пользователь {model.UserName}. Ошибка при смене пароля: {ex.Message}", LoggingLevel.Error.ToString(), null);
                return Json(new { Success = false, Message = ex.Message });
            }

            _logService.Log("RestorePasswordPost", "Auth", $"Пользователь {model.UserName} успешно сменил пароль", LoggingLevel.Success.ToString(), null);
            return Json(new { Success = true });
        }
    }
}
