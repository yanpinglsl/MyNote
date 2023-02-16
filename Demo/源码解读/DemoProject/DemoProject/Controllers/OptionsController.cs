using DemoProject.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;

namespace DemoProject.Controllers
{
    public class OptionsController : Controller
    {
        #region Identity
        private readonly IConfiguration _iConfiguration = null;
        private readonly ILogger<OptionsController> _logger;

        private IOptions<EmailOptions> _optionsDefault;//直接单例，读出来就缓存，不支持数据变化，性能高--只能度默认名字
        private IOptionsMonitor<EmailOptions> _optionsMonitor;//只读一次，写入缓存-----但是支持数据修改，靠的是监听文件更新(onchange)数据，实时变更
        private IOptionsSnapshot<EmailOptions> _optionsSnapshot;//作用域注册，一次请求内数据是缓存不变的，但是不同请求是每次都会重新初始化的数据
        public OptionsController(IOptions<EmailOptions> options
            , IOptionsMonitor<EmailOptions> optionsMonitor
            , IOptionsSnapshot<EmailOptions> optionsSnapshot
            , IConfiguration configuration
            , ILogger<OptionsController> logger)
        {
            this._optionsDefault = options;
            this._optionsMonitor = optionsMonitor;
            this._optionsSnapshot = optionsSnapshot;

            this._iConfiguration = configuration;
            this._logger = logger;
        }
        #endregion

        /// <summary>
        /// http://localhost:5726/Options
        /// </summary>
        /// <returns></returns>
        public IActionResult Index()
        {
            this._logger.LogWarning($"This is {nameof(OptionsController)} Index");

            //只能获取默认名字，也就是空名字的值
            base.ViewBag.defaultEmailOptions = this._optionsDefault.Value;

            base.ViewBag.defaultEmailOptions1 = this._optionsMonitor.CurrentValue;//默认名字
            base.ViewBag.fromMemoryEmailOptions1 = this._optionsMonitor.Get("FromMemory");//带名字
            base.ViewBag.fromConfigurationEmailOptions1 = this._optionsMonitor.Get("FromConfiguration");

            base.ViewBag.defaultEmailOptions2 = this._optionsSnapshot.Value;//默认名字
            base.ViewBag.fromMemoryEmailOptions2 = this._optionsSnapshot.Get("FromMemory");//带名字
            base.ViewBag.fromMemoryEmailOptions2 = this._optionsSnapshot.Get("FromMemory");//带名字
            base.ViewBag.fromMemoryEmailOptions2 = this._optionsSnapshot.Get("FromMemory");//3遍都是一样的
            base.ViewBag.fromConfigurationEmailOptions2 = this._optionsSnapshot.Get("FromConfiguration");

            return View();
        }
    }
}
