using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using SkyApm.Tracing;
using SkyApm.Tracing.Segments;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Zhaoxi.MicroService.OrderServiceInstance.Model;

namespace YY.MicroService.OrderServiceInstance.Controllers
{
    [Route("api/[controller]/[action]")]
    [ApiController]
    public class SkywalkingController : ControllerBase
    {
        private readonly IEntrySegmentContextAccessor segContext;

        public SkywalkingController(IEntrySegmentContextAccessor segContext)
        {
            this.segContext = segContext;
        }

        [HttpGet]
        public string SkywalkingTest()
        {
            return "SkywalkingTest";
        }

        /// <summary>
        /// 获取链接追踪ID
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        public string GetSkywalkingTraceId()
        {
            return segContext.Context.TraceId;
        }

        /// <summary>
        /// 自定义链路调用中的日志信息
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        public IActionResult SetSkywalkingLog()
        {
            //获取全局的skywalking的TracId
            var TraceId = segContext.Context.TraceId;
            Console.WriteLine($"TraceId={TraceId}");
            segContext.Context.Span.AddLog(LogEvent.Message($"SkywalkingTest1---Worker running at: {DateTime.Now}"));

            Thread.Sleep(1000);

            segContext.Context.Span.AddLog(LogEvent.Message($"SkywalkingTest1---Worker running at--end: {DateTime.Now}"));

            return Ok($"Ok,SkywalkingTest1-TraceId={TraceId} ");
        }

        #region 告警测试

        /// <summary>
        /// 告警API
        /// </summary>
        /// <param name="msgs"></param>
        /// <returns></returns>
        [HttpPost]
        public void AlarmMsg(List<AlarmMsg> msgs)
        {
            string msg = "触发告警：";
            msg += msgs.FirstOrDefault()?.alarmMessage;
            Console.WriteLine(msg);
            SendMail(msg);
        }

        /// <summary>
        /// 告警API
        /// </summary>
        /// <param name="msgs"></param>
        /// <returns></returns>
        [HttpGet]
        public void ExAction()
        {
            int a = 1;
            int b = 0;
            int c = a / b;
        }
        private void SendMail(string msg)
        {
            Console.WriteLine("触发告警===> 发送邮件");
        }
        #endregion
    }

}
