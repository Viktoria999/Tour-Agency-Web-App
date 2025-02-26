using Auth.Service;
using Domain.Models;
using Domain.Services;
using Microsoft.AspNetCore.Mvc;
using Web_lab.Models;

namespace Web_lab.Controllers
{
    public class ProfileController : Controller
    {
        private AccountService _accountService;
        private IFileService _fileService;

        public ProfileController(AccountService accountService, IFileService fileService)
        {
            _accountService = accountService;
            _fileService = fileService;
        }

        public IActionResult Index()
        {
            var token = Request.Cookies["authToken"];
            if (token == null)
                return Unauthorized();

            var userAccount = _accountService.GetAccount(token);
            if (userAccount == null)
                return NotFound();

            var model = new UserModel
            {
                Id = userAccount.Id,
                UserName = userAccount.UserName,
                FirstName = userAccount.FirstName,
                LastName = userAccount.LastName,
                MiddleName = userAccount.MiddleName,
                Email = userAccount.Email,
                ProfilePictureUrl = userAccount.ProfilePictureUrl
            };

            return View(model);
        }

        [HttpGet]
        public IActionResult ShowEditProfileDataModal()
        {
            var token = Request.Cookies["authToken"];
            if (token == null)
                return Unauthorized();

            var userAccount = _accountService.GetAccount(token);
            if (userAccount == null)
                return NotFound();

            var model = new EditProfileUserRequest
            {
                Id = userAccount.Id,
                UserName = userAccount.UserName,
                FirstName = userAccount.FirstName,
                LastName = userAccount.LastName,
                MiddleName = userAccount.MiddleName,
                Email = userAccount.Email,
                ProfilePictureUrl = userAccount.ProfilePictureUrl
            };

            return PartialView("~/Views/Shared/Authorization/_EditProfileDataModal.cshtml", model);
        }

        [HttpGet]
        public IActionResult ChangeProfilePictureGet()
        {
            var token = Request.Cookies["authToken"];
            if (token == null)
                return Unauthorized();

            var userAccount = _accountService.GetAccount(token);
            if (userAccount == null)
                return NotFound();

            return PartialView("~/Views/Profile/_ImportProfilePicture.cshtml");
        }

        [HttpPost]
        public async Task<IActionResult> ChangeProfilePicturePostAsync()
        {
            if (Request.Form.Files.Count > 0)
            {
                var token = Request.Cookies["authToken"];
                if (token == null)
                    return Unauthorized();

                var userAccount = _accountService.GetAccount(token);
                if (userAccount == null)
                    return NotFound();

                var requestFile = Request.Form.Files[0];
                var userName = userAccount.UserName;

                try
                {
                    var fileName = await _accountService.SaveUserProfilePictureAsync(requestFile, userName);
                }
                catch (Exception ex)
                {
                    return Json(new { Success = false, Message = ex.Message });
                }

                return Json(new { Success = true });
            }
            return Json(new { Success = false, Message = "Файл не найден." });
        }
    }
}
