using Logging;
using Microsoft.AspNetCore.Mvc.Filters;

public class LoggingActionFilter : IActionFilter
{
    private readonly ILogService _logService;

    public LoggingActionFilter(ILogService logService)
    {
        _logService = logService;
    }

    public void OnActionExecuting(ActionExecutingContext context)
    {
        var controllerName = context.RouteData.Values["controller"]?.ToString();
        var actionName = context.RouteData.Values["action"]?.ToString();
        var token = context.HttpContext.Request.Cookies["authToken"];

        _logService.Log(actionName, controllerName, "Пользователь перешёл на страницу", LoggingLevel.Information.ToString(), token ?? null);
    }

    public void OnActionExecuted(ActionExecutedContext context) { }
}
