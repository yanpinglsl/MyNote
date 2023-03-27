using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Zhaoxi.MicroService.OrderServiceInstance.Model
{
    /// <summary>
    /// Skywalking告警数据模型
    /// </summary>
    public class AlarmMsg
    {
        public int scopeId { get; set; }
        public string? scope { get; set; }
        public string? name { get; set; }
        public string? id0 { get; set; }
        public string? id1 { get; set; }
        public string? ruleName { get; set; }
        public string? alarmMessage { get; set; }
    }
}

