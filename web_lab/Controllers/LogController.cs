using Auth.Service;
using DAL;
using DAL.Filters;
using Domain.Helpers;
using Logging;
using Microsoft.AspNetCore.Mvc;
using Web_lab.Models;

namespace Web_lab.Controllers
{
    public class LogController : Controller
    {
        private readonly ILogRepository _logRepository;
        private readonly AccountService _accountService;

        public LogController(ILogRepository logRepository, AccountService accountService)
        {
            _logRepository = logRepository;
            _accountService = accountService;
        }

        public IActionResult Index()
        {
            var model = new LogFilterModel
            {
                LevelList = EnumHelper.ToSelectList<LoggingLevel>(),
            };
            return View(model);
        }

        public IActionResult GetLogs(LogFilter filter)
        {
            var token = Request.Cookies["authToken"];
            if (token == null)
                return Unauthorized();

            var userAccount = _accountService.GetAccount(token);
            if (userAccount == null || userAccount.IsAdmin == false)
                return NotFound();

            var logs = _logRepository.Select(filter);

            if (!logs.Any())
                return Json(new { State = 1, Message = "Список пуст." });

            return Json(new { State = 0, logs });
        }

        public IActionResult GetReport()
        {
            var token = Request.Cookies["authToken"];
            if (token == null)
                return Unauthorized();

            var userAccount = _accountService.GetAccount(token);
            if (userAccount == null || userAccount.IsAdmin == false)
                return NotFound();
            var report = _logRepository.GetPopularPagesRating();
            if (!report.Any())
                return Json(new { State = 1, Message = "Логи не найдены." });

            return Json(new { State = 0, report });
        }
    }
}
