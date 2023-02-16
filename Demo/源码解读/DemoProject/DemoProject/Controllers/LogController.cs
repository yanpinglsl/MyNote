using Microsoft.AspNetCore.Mvc;

namespace DemoProject.Controllers
{
    public class LogController : Controller
    {
        private readonly IConfiguration _iConfiguration = null;
        private readonly ILogger<LogController> _logger;
        private readonly ILoggerFactory _loggerFactory;

        public LogController(IConfiguration configuration
            , ILoggerFactory loggerFactory
            , ILogger<LogController> logger)
        {
            this._iConfiguration = configuration;
            this._logger = logger;
            this._loggerFactory = loggerFactory;
        }

        /// <summary>
        /// http://localhost:5726/log
        /// </summary>
        /// <returns></returns>
        public IActionResult Index()
        {
            this._logger.LogCritical("This is LogController-Index LogCritical");
            this._logger.LogDebug("This is LogController-Index LogDebug");
            this._logger.LogError("This is LogController-Index LogError");
            this._logger.LogInformation("This is LogController-Index LogInformation");
            this._logger.LogTrace("This is LogController-Index LogTrace");
            this._logger.LogWarning("This is LogController-Index LogWarning");

            this._loggerFactory.CreateLogger<LogController>().LogWarning("This is LogController-Index 1");
            return View();
        }

    }
}
