using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using WebCoreExtend.FilterExtend.AuthorizationExtend;
using WebCoreExtend.FilterExtend.DIShow;
using WebCoreExtend.FilterExtend.RegisterWayShow;
using WebCoreExtend.FilterExtend.SimpleShow;

namespace DemoProject.Controllers
{
    //[CustomControllerRegisterActionFilterAttribute]
    public class FilterController : Controller//, IActionFilter, IAsyncActionFilter, IResultFilter, IAsyncResultFilter
    {
        #region Identity
        private readonly IConfiguration _iConfiguration = null;
        private readonly ILogger<FilterController> _logger;

        public FilterController(IConfiguration configuration
            , ILogger<FilterController> logger)
        {
            this._iConfiguration = configuration;
            this._logger = logger;
        }
        #endregion

        #region Index+ControllerFilter
        /// <summary>
        /// http://localhost:5726/Filter/Index
        /// IActionFilter和IAsyncActionFilter默认已实现，可以override
        /// IResultFilter和IAsyncResultFilter没有默认实现，需要实现
        /// </summary>
        /// <returns></returns>

        //[CustomSimpleShowActionFilterAttribute]//同步版本
        //[CustomSimpleShowAsyncActionFilterAttribute]//异步版本
        //[CustomSimpleShowDoubleActionFilterAttribute]//只执行异步--源码里面有
        public IActionResult Index()
        {
            this._logger.LogWarning($"This is {nameof(FilterController)}-Index LogWarning");
            base.ViewBag.Now = DateTime.Now;
            Thread.Sleep(2000);

            return View();
        }

        #region 控制器继承Filter
        ///// <summary>
        ///// IActionFilter和IAsyncActionFilter默认已实现，可以override
        ///// </summary>
        ///// <param name="context"></param>
        //public override void OnActionExecuted(ActionExecutedContext context)
        //{
        //    Console.WriteLine($"This {nameof(FilterController)} OnActionExecuted");
        //}
        //public override void OnActionExecuting(ActionExecutingContext context)
        //{
        //    Console.WriteLine($"This {nameof(FilterController)} OnActionExecuting");
        //}
        //public async override Task OnActionExecutionAsync(ActionExecutingContext context, ActionExecutionDelegate next)
        //{
        //    Console.WriteLine($"This {nameof(FilterController)} OnActionExecutionAsync  --Begin");
        //    await next.Invoke();
        //    Console.WriteLine($"This {nameof(FilterController)} OnActionExecutionAsync  --End");
        //}

        ///// <summary>
        ///// IResultFilter和IAsyncResultFilter没有默认实现，需要实现
        ///// </summary>
        ///// <param name="context"></param>
        //public void OnResultExecuting(ResultExecutingContext context)
        //{
        //    Console.WriteLine($"This {nameof(FilterController)} OnResourceExecuting ");
        //}
        //public void OnResultExecuted(ResultExecutedContext context)
        //{
        //    Console.WriteLine($"This {nameof(FilterController)} OnResourceExecuted ");
        //}
        //public async Task OnResultExecutionAsync(ResultExecutingContext context, ResultExecutionDelegate next)
        //{
        //    Console.WriteLine($"This {nameof(FilterController)} OnResultExecutionAsync  --Begin");
        //    await next.Invoke();
        //    Console.WriteLine($"This {nameof(FilterController)} OnResultExecutionAsync --End");
        //}
        #endregion

        #endregion

        #region Filter注册方式
        /// <summary>
        /// http://localhost:5726/Filter/RegisterWay
        /// </summary>
        /// <returns></returns>
        [CustomActionRegisterActionFilterAttribute]//Action生效
        public IActionResult RegisterWay()
        {
            this._logger.LogWarning($"This is {nameof(FilterController)}-RegisterWay LogWarning");

            base.ViewBag.Now = DateTime.Now;
            Thread.Sleep(2000);
            return View();
        }
        #endregion

        #region Filter排序
        /// <summary>
        /// http://localhost:5726/Filter/Order
        /// 1 全局---控制器---Action  
        /// 2 Order默认0，从小到大执行  可以负数
        /// </summary>
        /// <returns></returns>
        //[CustomActionRegisterActionFilterAttribute(Remark = "默认Order Before")]//D
        //[CustomActionRegisterActionFilterAttribute(Remark = "默认Order After")]//E
        [CustomActionRegisterActionFilterAttribute(Order = 10, Remark = "Order10")]//A
        [CustomActionRegisterActionFilterAttribute(Order = -1, Remark = "Order-1")]//B
        [CustomActionRegisterActionFilterAttribute(Order = 1, Remark = "Order1")]//C
        //[IResourceFilter]
        public IActionResult Order()
        {
            this._logger.LogWarning($"This is {nameof(FilterController)}-Order LogWarning");

            base.ViewBag.Now = DateTime.Now;
            Thread.Sleep(2000);
            return View();
        }
        #endregion

        #region Filter的IOC注入问题
        /// <summary>
        /// http://localhost:5726/Filter/DI
        /// 
        /// </summary>
        /// <returns></returns>
        [ServiceFilter(typeof(CustomDIActionFilterAttribute))]//需要IOC注册，不支持参数

        //[TypeFilter(typeof(CustomDIActionFilterAttribute))]//直接用，不需要做注册，支持额外指定参数
        //[CustomAttribute(new LoggerFactory().CreateLogger<CustomAttribute>())]
        //[CustomAttribute(CustomAttribute.Name)]
        public IActionResult DI()
        {
            this._logger.LogWarning($"This is {nameof(FilterController)}-DI LogWarning");

            base.ViewBag.Now = DateTime.Now;
            Thread.Sleep(2000);
            return View();
        }
        #endregion

        #region FilterFactory
        /// <summary>
        /// http://localhost:5726/Filter/Factory
        /// </summary>
        /// <returns></returns>
        [CustomIOCFilterFactory(typeof(CustomDisposeActionFilterAttribute))]
        //[CustomIOCFilterFactory(typeof(CustomDIActionFilterAttribute))]
        public IActionResult Factory()
        {
            this._logger.LogWarning($"This is {nameof(FilterController)}-Factory LogWarning");

            base.ViewBag.Now = DateTime.Now;
            Thread.Sleep(2000);

            //return new OkResult();
            return View();
        }
        #endregion

        #region Authorization
        /// <summary>
        /// http://localhost:5726/Filter/Authorization
        /// http://localhost:5726/Filter/Authorization?UserName=Eleven
        /// </summary>
        /// <returns></returns>
        [CustomDoubleAuthorizationFilterAttribute]//2个都有，啥也不干
        [CustomAuthorizationFilter]
        [CustomAsyncAuthorizationFilter]
        //[AllowAnonymous]
        public async Task<IActionResult> Authorization()
        {
            await Task.CompletedTask;
            this._logger.LogWarning($"This is {nameof(FilterController)}-Authorization LogWarning");

            base.ViewBag.Now = DateTime.Now;
            Thread.Sleep(2000);

            return View();
        }

        [AllowAnonymous]
        public async Task<IActionResult> Login()
        {
            await Task.CompletedTask;
            this._logger.LogWarning($"This is {nameof(FilterController)}-Authorization LogWarning");

            base.ViewBag.Now = DateTime.Now;
            Thread.Sleep(2000);

            return View();
        }
        #endregion
    }
}
