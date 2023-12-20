using GlobalizationLocalizationDemo.Dto;
using GlobalizationLocalizationDemo.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Localization;
using System.Diagnostics;
using System.Globalization;
using System.Reflection;

namespace GlobalizationLocalizationDemo.Controllers
{
    [Route("[controller]/[action]")]
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;

        // 用于提供 HomeController 的区域性资源
        private readonly IStringLocalizer<HomeController> _localizer;

        // 通过代理伪类提供共享资源
        private readonly IStringLocalizer<SharedResource> _sharedLocalizer;

        //当我们想要使用一些没有代理类或代理类无法使用的区域资源，无法直接通过IStringLocalizer<>进行注入，
        //那IStringLocalizerFactory就可以帮助我们获取对应的IStringLocalizer
        private readonly IStringLocalizer _localizer1;

        private readonly IStringLocalizer _localizer2;

        public HomeController(ILogger<HomeController> logger,
       IStringLocalizer<HomeController> localizer,
       IStringLocalizer<SharedResource> sharedLocalizer,
       IStringLocalizerFactory localizerFactory)
        {
            _logger = logger;
            _localizer = localizer;
            _sharedLocalizer = sharedLocalizer;
            _localizer1 = localizerFactory.Create(typeof(HomeController));
            _localizer2 = localizerFactory.Create("Controllers.HomeController", Assembly.GetExecutingAssembly().FullName);
        }

        [HttpGet]
        public IActionResult Index()
        {
            return View();
        }
        [HttpGet]
        public IActionResult Privacy()
        {
            return View();
        }
        [HttpGet]
        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
        #region  在服务类中使用本地化
        //https://localhost:7019/home/getstring?culture=en-US
        [HttpGet]
        public IActionResult GetString()
        {
            var content = $"当前区域文化：{CultureInfo.CurrentCulture.Name}\n" +
                $"{_localizer["HelloWorld"]}\n" +
                $"{_sharedLocalizer["CurrentTime"]}{DateTime.Now.ToLocalTime()}\n";
            return Content(content);
        }
        #endregion

        #region  在模型验证中使用本地化
        [HttpPost]
        public IActionResult Register([FromBody] RegisterDto dto)
        {
            if (!ModelState.IsValid)
            {
                return Content($"当前区域文化：{CultureInfo.CurrentCulture.Name}\n" +
                    "模型状态无效：" + Environment.NewLine +
                    string.Join(Environment.NewLine, ModelState.Values.SelectMany(v => v.Errors.Select(e => e.ErrorMessage))));
            }

            return Ok();
        }
        #endregion


        #region  IStringLocalizerFactory
        [HttpGet]
        public IActionResult GetIStringLocalizerString()
        {
            var content = $"当前区域文化：{CultureInfo.CurrentCulture.Name}\n" +
                $"{_localizer1["HelloWorld"]}\n" +
                $"{_localizer2["HelloWorld"]}\n";
            return Content(content);
        }
        #endregion
    }
}
