using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace OfficeReportInterface
{
    public enum VersionsManager
    {
        CurrentVersion = Version3501ForCommandManager,
        /// <summary>
        /// CommandManager中的当前版本号
        /// </summary>
        Version3501ForCommandManager=3501,

        /// <summary>
        /// 增加了，对每个发送计划保存其独特的保存路径的成员变量
        /// </summary>
        VersionWithLocationForEachSendPlan=2102,
        /// <summary>
        /// 还没有增加，对每个发送计划保存其独特的保存路径的成员变量
        /// </summary>
        VersionWithoutLocationForEachSendPlan = 2101,

        /// <summary>
        /// 在没有增加FileType属性之前，OfficeReportInfo类中的version的值是0.
        /// 为了让iemsweb前端区分传过去的是预制报表，或者是查询条件，还是普通报表 类型，增加了FileType属性
        /// </summary>
        versionWithoutFileTypeForOfficeReportInfo=0,

        /// <summary>
        /// 兼容性处理，多添加了一个字段，那么在读出的时候就根据version的高低，新的就多读一个字段，老的就不读这个新加的字段.
        /// 在添加这个字段之前，该版本号是0
        /// </summary>
        versionWithoutSaveStyleForEmailSendSet = 0,
    }
}
