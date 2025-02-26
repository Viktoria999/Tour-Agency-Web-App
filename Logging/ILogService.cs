namespace Logging
{
    public interface ILogService
    {
        void Log(string actionName, string controllerName, string message, string level, string? token);
    }
}
