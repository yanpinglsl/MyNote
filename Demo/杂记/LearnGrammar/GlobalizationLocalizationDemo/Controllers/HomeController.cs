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

        // �����ṩ HomeController ����������Դ
        private readonly IStringLocalizer<HomeController> _localizer;

        // ͨ������α���ṩ������Դ
        private readonly IStringLocalizer<SharedResource> _sharedLocalizer;

        //��������Ҫʹ��һЩû�д������������޷�ʹ�õ�������Դ���޷�ֱ��ͨ��IStringLocalizer<>����ע�룬
        //��IStringLocalizerFactory�Ϳ��԰������ǻ�ȡ��Ӧ��IStringLocalizer
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
        #region  �ڷ�������ʹ�ñ��ػ�
        //https://localhost:7019/home/getstring?culture=en-US
        [HttpGet]
        public IActionResult GetString()
        {
            var content = $"��ǰ�����Ļ���{CultureInfo.CurrentCulture.Name}\n" +
                $"{_localizer["HelloWorld"]}\n" +
                $"{_sharedLocalizer["CurrentTime"]}{DateTime.Now.ToLocalTime()}\n";
            return Content(content);
        }
        #endregion

        #region  ��ģ����֤��ʹ�ñ��ػ�
        [HttpPost]
        public IActionResult Register([FromBody] RegisterDto dto)
        {
            if (!ModelState.IsValid)
            {
                return Content($"��ǰ�����Ļ���{CultureInfo.CurrentCulture.Name}\n" +
                    "ģ��״̬��Ч��" + Environment.NewLine +
                    string.Join(Environment.NewLine, ModelState.Values.SelectMany(v => v.Errors.Select(e => e.ErrorMessage))));
            }

            return Ok();
        }
        #endregion


        #region  IStringLocalizerFactory
        [HttpGet]
        public IActionResult GetIStringLocalizerString()
        {
            var content = $"��ǰ�����Ļ���{CultureInfo.CurrentCulture.Name}\n" +
                $"{_localizer1["HelloWorld"]}\n" +
                $"{_localizer2["HelloWorld"]}\n";
            return Content(content);
        }
        #endregion
    }
}
