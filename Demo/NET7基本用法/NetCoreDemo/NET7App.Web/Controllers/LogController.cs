using Microsoft.AspNetCore.Mvc;

namespace NET7App.Web.Controllers
{
    public class LogController : Controller
    {

        private readonly ILogger<LogController> _logger;
        //private readonly ILoggerFactory _ILoggerFactory;

        public LogController(ILogger<LogController> logger, ILoggerFactory iLoggerFactory)
        {
            _logger = logger;
            _logger.LogInformation($"LogInformation:{this.GetType().FullName} 被构造....");
            _logger.LogError($"LogError:{this.GetType().FullName} 被构造....");
            _logger.LogDebug($"LogDebug:{this.GetType().FullName} 被构造....");
            _logger.LogWarning($"LogWarning:{this.GetType().FullName} 被构造....");

            ////通过创建ILoggerFactory来写入日志
            //_ILoggerFactory = iLoggerFactory;
            //ILogger<LogController> ilogger = _ILoggerFactory.CreateLogger<LogController>();

            //ilogger.LogInformation($"LogInformation:{this.GetType().FullName} 被构造....");
            //ilogger.LogError($"LogError:{this.GetType().FullName} 被构造....");
            //ilogger.LogDebug($"LogDebug:{this.GetType().FullName} 被构造....");
            //ilogger.LogWarning($"LogWarning:{this.GetType().FullName} 被构造....");

        }

        //public IActionResult Index()
        //{
        //    base.ViewData["User1"] = "Kulala~~~~Kulala";
        //    ViewBag.User1 = "Kulala~";
        //    ViewData["User2"] = "闻";
        //    TempData["User3"] = "高强";
        //    //base.HttpContext.Session["User4"] = "非白";
        //    //最小化设计：通过扩展支持Session 
        //    HttpContext.Session.SetString("User4", "非白");
        //    object User5 = "角印";
        //    return View(User5); //寻找对应的视图文件，使用视图文件渲染内容，输出给客户端；
        //}

        // FromServices 特性实现依赖注入,可以直接将service注入到action方法中
        //    public IActionResult Index2([FromServices] ILogger<LogController> logger, [FromServices] ILoggerFactory iLoggerFactory)
        //    {

        //        {
        //            //这里做业务逻辑计算
        //            logger.LogInformation($"LogInformation:{this.GetType().FullName} 被构造....");
        //            logger.LogError($"LogError:{this.GetType().FullName} 被构造....");
        //            logger.LogDebug($"LogDebug:{this.GetType().FullName} 被构造....");
        //            logger.LogWarning($"LogWarning:{this.GetType().FullName} 被构造....");

        //            Console.WriteLine("==============================================");
        //            var iloger = iLoggerFactory.CreateLogger<LogController>();

        //            iloger.LogInformation($"LogInformation:{this.GetType().FullName} 被构造....");
        //            iloger.LogError($"LogError:{this.GetType().FullName} 被构造....");
        //            iloger.LogDebug($"LogDebug:{this.GetType().FullName} 被构造....");
        //            iloger.LogWarning($"LogWarning:{this.GetType().FullName} 被构造....");

        //        }
        //        base.ViewData["User1"] = "Kulala~~~~Kulala";
        //        ViewBag.User1 = "Kulala~";
        //        ViewData["User2"] = "闻";
        //        TempData["User3"] = "高强";
        //        //base.HttpContext.Session["User4"] = "非白";
        //        //最小化设计：通过扩展支持Session 
        //        HttpContext.Session.SetString("User4", "非白");
        //        object User5 = "角印";
        //        return View(User5); //寻找对应的视图文件，使用视图文件渲染内容，输出给客户端；
        //    }
    }
}
