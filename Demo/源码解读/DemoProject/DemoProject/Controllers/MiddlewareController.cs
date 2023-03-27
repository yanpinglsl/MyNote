using Microsoft.AspNetCore.Mvc;

namespace DemoProject.Controllers
{
    public class MiddlewareController : Controller
    {
        //[FromQuery]//不是IOC注入的，而是靠参数绑定
        //public string Name { get; set; }

        private readonly ILogger<MiddlewareController> _logger;
        private readonly IConfiguration _iConfiguration = null;

        public MiddlewareController(ILogger<MiddlewareController> logger, IConfiguration configuration)
        {
            this._logger = logger;
            this._iConfiguration = configuration;
        }
        /// <summary>
        /// dotnet run --urls="http://*:5726" ip="127.0.0.1" /port=5726 ConnectionStrings:Write=CommandLineArgument
        /// </summary>
        /// <returns></returns>
        public IActionResult Index()
        {
            return View();
        }

        /// <summary>
        /// http://localhost:5726/Middleware/Exceptions/1
        /// http://localhost:5726/Middleware/Exceptions/2
        /// </summary>
        /// <returns></returns>
        public IActionResult Exceptions(int? id)
        {
            if (id == 1)
                throw new Exception($"MiddlewareController--Exceptions---{nameof(id)}={id}");
            return View();
        }

        /// <summary>
        /// http://localhost:5726/Middleware/Session
        /// 
        /// dotnet run --urls="http://*:5726" ip="127.0.0.1" /port=5726 ConnectionStrings:Write=CommandLineArgument
        /// 
        /// dotnet run --urls="http://*:5727" ip="127.0.0.1" /port=5727 ConnectionStrings:Write=CommandLineArgument
        /// Redis存储
        /// </summary>
        /// <returns></returns>
        public IActionResult Session()
        {
            string sessionUser = base.HttpContext.Session.GetString("CurrentUser")!;

            if (sessionUser == null)
            {
                base.HttpContext.Session.SetString("CurrentUser", $"{this._iConfiguration["ip"]}:{this._iConfiguration["port"]} Eleven");
                base.ViewBag.Info = $"{this._iConfiguration["ip"]}:{this._iConfiguration["port"]} 头一次进入，初始化Session";
            }
            else
            {
                base.ViewBag.Info = $"{this._iConfiguration["ip"]}:{this._iConfiguration["port"]} 后续进入，从Session读取数据CurrentUser={sessionUser}";
            }
            return View();
        }

    }
}
