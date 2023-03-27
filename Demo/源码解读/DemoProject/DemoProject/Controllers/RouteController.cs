using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Routing;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using WebCoreExtend.RouteExtend;
//using Zhaoxi.AgileFramework.WebCore.RouteExtend;

namespace DemoProject.Controllers
{
    public class RouteController : Controller
    {
        private readonly IConfiguration _iConfiguration = null;
        private readonly ILogger<RouteController> _logger;

        public RouteController(IConfiguration configuration
            , ILogger<RouteController> logger)
        {
            this._iConfiguration = configuration;
            this._logger = logger;
        }

        /// <summary>
        /// dotnet run --urls="http://*:5726" ip="127.0.0.1" /port=5726 ConnectionStrings:Write=CommandLineArgument
        /// http://localhost:5726/route/index
        /// </summary>
        /// <returns></returns>

        public IActionResult Index()
        {
            this._logger.LogWarning("This is RouteController-Index LogWarning");
            string des = $"controller={this.HttpContext.Request.RouteValues["controller"]}&action={this.HttpContext.Request.RouteValues["action"]}";
            Console.WriteLine(des);
            base.ViewBag.Des = des;
            return View();
        }

        /// <summary>
        /// http://localhost:5726/about
        /// http://localhost:5726/route/about
        /// </summary>
        /// <returns></returns>
        public IActionResult About()
        {
            this._logger.LogWarning("This is RouteController-About LogWarning");
            string des = $"controller={this.HttpContext.Request.RouteValues["controller"]}&action={this.HttpContext.Request.RouteValues["action"]}";
            Console.WriteLine(des);
            base.ViewBag.Des = des;
            return View();
        }

        /// <summary>
        /// 伪静态
        /// http://localhost:5726/static/133.html
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public IActionResult StaticPage(int id)
        {
            this._logger.LogWarning("This is RouteController-PageInfo LogWarning");
            string des = $"controller={this.HttpContext.Request.RouteValues["controller"]}&action={this.HttpContext.Request.RouteValues["action"]}&Id={this.HttpContext.Request.RouteValues["id"]}";
            base.ViewBag.Des = des;
            return View();
        }



        /// <summary>
        /// http://localhost:5726/Item2/133.html       可以
        /// http://localhost:5726/Item1/133.html   可以
        /// http://localhost:5726/Item2/13s.html    不行
        /// http://localhost:5726/Route/PageInfo/133   不行
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [Route("/Item1/{id:int}.html")]
        [Route("/Item2/{id:int}.html")]
        public IActionResult PageInfo(int id)
        {
            this._logger.LogWarning("This is RouteController-PageInfo LogWarning");
            string des = $"controller={this.HttpContext.Request.RouteValues["controller"]}&action={this.HttpContext.Request.RouteValues["action"]}&Id={this.HttpContext.Request.RouteValues["id"]}";
            base.ViewBag.Des = des;
            return View();
        }

        /// <summary>
        /// http://localhost:5726/Gender/1.html
        /// http://localhost:5726/Gender/2.html   不行
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [Route("/Gender/{id:GenderConstraint}.html")]
        public IActionResult GenderInfo(int id)
        {
            this._logger.LogWarning("This is RouteController-GenderInfo LogWarning");
            string des = $"controller={this.HttpContext.Request.RouteValues["controller"]}&action={this.HttpContext.Request.RouteValues["action"]}&Id={this.HttpContext.Request.RouteValues["id"]}";
            base.ViewBag.Des = des;
            return View();
        }

        /// <summary>
        /// http://localhost:5726/Route/HostInfo/123
        /// http://localhost:5727/Route/HostInfo/123  不行
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [Host("*:5726")]
        public IActionResult HostInfo(int id)
        {
            this._logger.LogWarning("This is RouteController-HostInfo LogWarning");
            string des = $"controller={this.HttpContext.Request.RouteValues["controller"]}&action={this.HttpContext.Request.RouteValues["action"]}&Id={this.HttpContext.Request.RouteValues["id"]}";
            base.ViewBag.Des = des;
            return View();
        }

        /// <summary>
        /// http://localhost:5726/Route/Data/2019-11  三个都满足，但是找最精准的
        /// http://localhost:5726/Route/Data/2019-13  满足2个，找更精准的
        /// http://localhost:5726/Route/Data/2018-09  满足2个，找更精准的
        /// http://localhost:5726/Route/Data/2018-9   只满足默认路由
        /// http://localhost:5726/Route/Data?year=2019&month=11  默认路由
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public IActionResult Data(int year, int month)
        {
            this._logger.LogWarning("This is RouteController-Data LogWarning");

            var endpoint = base.HttpContext.GetEndpoint();
            if (endpoint != null && endpoint is RouteEndpoint routeEndpoint)
            {
                base.ViewBag.RoutePattenDes = routeEndpoint.RoutePattern.Format();
            }
            
            string des = $"controller={this.HttpContext.Request.RouteValues["controller"]}&action={this.HttpContext.Request.RouteValues["action"]}&路由Id={this.HttpContext.Request.RouteValues["id"]}&路由year={this.HttpContext.Request.RouteValues["year"]}&路由month={this.HttpContext.Request.RouteValues["month"]}&参数year={year}&参数month={month}";
            base.ViewBag.Des = des;

            return View();
        }

        /// <summary>
        /// dotnet run --urls="http://*:5726" ip="127.0.0.1" /port=5726 ConnectionStrings:Write=CommandLineArgument
        /// http://localhost:5726/route/Info
        /// 
        /// http://localhost:5726/en/route/Info
        /// http://localhost:5726/ch/route1/Info1
        /// http://localhost:5726/hk/route2/Info2
        /// </summary>
        /// <returns></returns>
        public IActionResult Info()
        {
            this._logger.LogWarning("This is RouteController-Info LogWarning");
            string des = $"language={this.HttpContext.Request.RouteValues["language"]}&controller={this.HttpContext.Request.RouteValues["controller"]}&action={this.HttpContext.Request.RouteValues["action"]}";
            Console.WriteLine(des);
            base.ViewBag.Des = des;

            return View();
        }
    }
}
