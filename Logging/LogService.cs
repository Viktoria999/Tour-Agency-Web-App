using Auth.Service;
using DAL;
using Domain.Models;

namespace Logging
{
    public class LogService : ILogService
    {
        private readonly ILogRepository _logRepository;
        private AccountService _accountService;

        public LogService(ILogRepository logRepository, AccountService accountService)
        {
            _logRepository = logRepository;
            _accountService = accountService;
        }

        public void Log(string actionName, string controllerName, string message, string level, string? token)
        {
            var log = new Log
            {
                Controller = controllerName,
                Action = actionName,
                Message = message,
                Level = level
            };

            if (token != null)
            {
                var principal = _accountService.ValidateToken(token);
                if (principal != null)
                {
                    log.UserName = principal.FindFirst("userName")?.Value;
                }
            }


            _logRepository.Insert(log);
        }
    }
}
