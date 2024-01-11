using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace OfficeReportInterface
{
    
    /// <summary>
    /// 自动导出方式
    /// </summary>
    public enum AutoExportType
    {
        Daily = 0,
        Weekly = 1,
        Monthly = 2,
    }

    public struct WeeklySet
    {
        /// <summary>
        /// 按周发送方式的间隔
        /// </summary>
        public int weeks;
        /// <summary>
        /// 选择的周几(0为周日，1为周一)
        /// </summary>
        public DayOfWeek weekDay;

        public WeeklySet(int weeksParm, DayOfWeek weekDayParm)
        {
            weeks = weeksParm;
            weekDay = weekDayParm;
        }
    }

    public class MonthlySet
    {
        /// <summary>
        /// 固定日期，0表示最后一天，1,2,3...表示1,2,3号...
        /// </summary>
        public int fixedDay;
        /// <summary>
        /// 选择的自动导出月份，1,2,3...依次代表一月，二月，三月...
        /// </summary>
        public List<int> selectedMonthList = new List<int>(0);

        public MonthlySet()
        { }

        public MonthlySet(List<int> selectedMonthListParm, int fixedDayParm)
        {
            fixedDay = fixedDayParm;
            selectedMonthList = selectedMonthListParm;
        }

        public object Clone()
        {
            MonthlySet monthlySet = new MonthlySet();
            monthlySet.fixedDay = this.fixedDay;
            for (int i = 0; i < selectedMonthList.Count; i++)
            {
                monthlySet.selectedMonthList.Add(selectedMonthList[i]);
            }
            return monthlySet;
        }
    }

    /// <summary>
    /// 自动导出方案
    /// </summary>
    public class AutoExportPlan : ICloneable
    {
        /// <summary>
        /// 保存初始版本号
        /// </summary>
        private uint version =(uint) VersionsManager.CurrentVersion;
        private AutoExportType autoExportType = AutoExportType.Daily;
        private int dailySet = 1;
        private WeeklySet weeklySet = new WeeklySet(1, DayOfWeek.Monday);
        private MonthlySet monthlySet = new MonthlySet();

        #region 属性

        /// <summary>
        /// 自动发送方式
        /// </summary>
        public AutoExportType AutoExportType
        {
            get { return autoExportType; }
            set { autoExportType = value; }
        }

        /// <summary>
        /// 按天导出方式的间隔（数值代表设置的间隔）
        /// </summary>
        public int DailySet
        {
            get { return this.dailySet; }
            set { this.dailySet = value; }
        }

        /// <summary>
        /// 按周导出方式（结构体类型）
        /// </summary>
        public WeeklySet WeeklySet
        {
            get { return weeklySet; }
            set { weeklySet = value; }
        }

        /// <summary>
        /// 按月导出方式（class类型）
        /// </summary>
        public MonthlySet MonthlySet
        {
            get { return monthlySet; }
            set { monthlySet = value; }
        }

        #endregion

        public object Clone()
        {
            AutoExportPlan autoExportProgramClass = new AutoExportPlan();
            autoExportProgramClass.version = this.version;
            autoExportProgramClass.autoExportType = this.autoExportType;
            autoExportProgramClass.dailySet = this.dailySet;
            autoExportProgramClass.weeklySet = this.weeklySet;
            autoExportProgramClass.monthlySet = this.monthlySet.Clone() as MonthlySet;
            return autoExportProgramClass;
        }

        public bool SerailizeToMemory(MemoryStream stream)
        {
            try
            {
                BinaryWriter binWriter = new BinaryWriter(stream);
                WriteVersion(binWriter);
                WriteAutoExportType(binWriter);
                return true;
            }
            catch (System.Exception ex)
            {
                DbgTrace.dout(ex.Message + ex.StackTrace);
                return false;
            }
        }
        /// <summary>
        /// 写入版本信息
        /// </summary>
        /// <param name="binWriter">写入流</param>
        private void WriteVersion(BinaryWriter binWriter)
        {
            binWriter.Write((uint)VersionsManager.CurrentVersion);
        }

        /// <summary>
        /// 写入自动导出方案
        /// </summary>
        /// <param name="binWriter">写入流</param>
        private void WriteAutoExportType(BinaryWriter binWriter)
        {
            binWriter.Write((byte)autoExportType);
            if (autoExportType == AutoExportType.Daily)
                WriteDailySet(binWriter);
            else if (autoExportType == AutoExportType.Weekly)
                WriteWeeklySet(binWriter);
            else
                WriteMonthly(binWriter);
        }

        /// <summary>
        /// 按天导出
        /// </summary>
        /// <param name="binWriter">写入流</param>
        private void WriteDailySet(BinaryWriter binWriter)
        {
            binWriter.Write(dailySet);
        }

        /// <summary>
        /// 按周导出
        /// </summary>
        /// <param name="binWriter">写入流</param>
        private void WriteWeeklySet(BinaryWriter binWriter)
        {
            binWriter.Write(weeklySet.weeks);
            binWriter.Write((byte)weeklySet.weekDay);
        }

        /// <summary>
        /// 按月导出
        /// </summary>
        /// <param name="binWriter">写入流</param>
        private void WriteMonthly(BinaryWriter binWriter)
        {
            binWriter.Write(monthlySet.selectedMonthList.Count);
            foreach (int month in monthlySet.selectedMonthList)
            {
                binWriter.Write(month);
            }
            binWriter.Write(monthlySet.fixedDay);
        }

        public bool DeserializeFormMemory(MemoryStream stream)
        {
            if (stream == null)
                return false;
            try
            {
                BinaryReader binReader = new BinaryReader(stream);
                ReadVersion(binReader);
                ReadAutoExportType(binReader);
                return true;
            }
            catch (System.Exception ex)
            {
                DbgTrace.dout(ex.Message + ex.StackTrace);
                return false;
            }
        }

        /// <summary>
        /// 读取版本信息
        /// </summary>
        /// <param name="binWriter">读取流</param>
        private void ReadVersion(BinaryReader binReader)
        {
            version = binReader.ReadUInt32();
        }

        /// <summary>
        /// 读取自动导出方案
        /// </summary>
        /// <param name="binReader"></param>
        /// <returns></returns>
        private void ReadAutoExportType(BinaryReader binReader)
        {
            autoExportType = (AutoExportType)binReader.ReadByte();
            if (autoExportType == AutoExportType.Daily)
                dailySet = ReadDailySet(binReader);
            else if (autoExportType == AutoExportType.Weekly)
                weeklySet = ReadWeeklySet(binReader);
            else
                monthlySet = ReadMonthlySet(binReader);
        }


        /// <summary>
        /// 读取自动导出方案——按天导出
        /// </summary>
        /// <param name="binWriter">读取流</param>
        private int ReadDailySet(BinaryReader binReader)
        {
            return binReader.ReadInt32();
        }

        /// <summary>
        /// 读取自动导出方案——按周导出
        /// </summary>
        /// <param name="binWriter">读取流</param>
        private WeeklySet ReadWeeklySet(BinaryReader binReader)
        {
            int weeks = binReader.ReadInt32();
            DayOfWeek weekDay = (DayOfWeek)binReader.ReadByte();
            return new WeeklySet(weeks, weekDay);
        }

        /// <summary>
        /// 读取自动导出方案——按月导出
        /// </summary>
        /// <param name="binWriter">读取流</param>
        private MonthlySet ReadMonthlySet(BinaryReader binReader)
        {
            List<int> selectedMonthList = new List<int>();
            int count = binReader.ReadInt32();
            for (int i = 0; i < count; i++)
            {
                int month = binReader.ReadInt32();
                selectedMonthList.Add(month);
            }
            int fixedDay = binReader.ReadInt32();
            return new MonthlySet(selectedMonthList, fixedDay);
        }
    }

    /// <summary>
    /// 发送计划
    /// </summary>
    [Serializable]
    public class EmailSendPlan : ICloneable
    {
        private uint version=(uint)VersionsManager.CurrentVersion;
        private List<RecipientsInfo> recipientsList = new List<RecipientsInfo>();
        private string emailTitle = string.Empty;
        private List<string> reportTemplatePathList = new List<string>();
        private AutoExportPlan autoExportProgram = new AutoExportPlan();
        private bool beginOrNot;
        private DateTime startSendTime;

        private DateTime judgeTime;
        private int retryTimes;
        /// <summary>
        /// 保存该发送计划的生成路径。在该路径下，每次生成的时候，都创建一个子目录（发送计划的名称加上年月日），用于存放该次发送的Excel文件到本地。
        /// </summary>
        private string _savePathParent = string.Empty;

        #region 属性
        
        /// <summary>
        /// 重发第几次
        /// </summary>
        public int RetryTimes
        {
            get { return retryTimes; }
            set { retryTimes = value; }
        }

        /// <summary>
        /// 下次发送时刻
        /// </summary>
        public DateTime JudgeTime
        {
            get { return judgeTime; }
            set { judgeTime = value; }
        }
        /// <summary>
        /// 上次发送时间
        /// </summary>
        public DateTime LastSendTime;

        /// <summary>
        /// 收件人列表
        /// </summary>
        public List<RecipientsInfo> RecipientsList
        {
            get { return recipientsList; }
            set { recipientsList = value; }
        }

        /// <summary>
        /// 邮件标题
        /// </summary>
        public string EmailTitle
        {
            get { return emailTitle; }
            set { emailTitle = value; }
        }

        /// <summary>
        /// 模板列表
        /// </summary>
        public List<string> ReportTemplatePathList
        {
            get { return reportTemplatePathList; }
            set { reportTemplatePathList = value; }
        }

        /// <summary>
        /// 自动发送方案
        /// </summary>
        public AutoExportPlan AutoExportProgram
        {
            get { return autoExportProgram; }
            set { autoExportProgram = value; }
        }

        /// <summary>
        /// 发送进程是否已启动
        /// </summary>
        public bool BeginOrNot
        {
            get { return beginOrNot; }
            set { beginOrNot = value; }
        }

        /// <summary>
        /// 开始发送时间
        /// </summary>
        public DateTime StartSendTime
        {
            get { return startSendTime; }
            set { startSendTime = value; }
        }

        public string SavePathParent
        {
            get { return _savePathParent; }
            set { _savePathParent = value; }
        }

        #endregion

        public EmailSendPlan GetCopy()
        {
            return this.Clone() as EmailSendPlan;
        }

        public object Clone()
        {
            EmailSendPlan sendPlan = new EmailSendPlan();
            sendPlan.version = this.version;
            sendPlan.emailTitle = this.emailTitle;
            sendPlan.autoExportProgram = this.autoExportProgram.Clone() as AutoExportPlan;
            sendPlan.beginOrNot = this.beginOrNot;
            sendPlan.startSendTime = this.startSendTime;
            sendPlan.LastSendTime = this.LastSendTime;
            for (int i = 0; i < recipientsList.Count; i++)
            {
                sendPlan.recipientsList.Add(recipientsList[i].Clone() as RecipientsInfo);
            }
            for (int i = 0; i < reportTemplatePathList.Count; i++)
            {
                sendPlan.reportTemplatePathList.Add(reportTemplatePathList[i]);
            }
         
            sendPlan.SavePathParent = this.SavePathParent;

            sendPlan.JudgeTime = this.JudgeTime;
            
            return sendPlan;
        }
        /// <summary>
        /// 只序列化开关状态
        /// </summary>
        /// <param name="stream"></param>
        /// <returns></returns>
        public bool SerailizeBeginOrNotToMemory(MemoryStream stream)
        {
            try
            {
                BinaryWriter binWriter = new BinaryWriter(stream);
                binWriter.Write((uint)VersionsManager.CurrentVersion);
                binWriter.Write(BeginOrNot);
                binWriter.Write(EmailTitle);
                return true;
            }
            catch (System.Exception ex)
            {
                DbgTrace.dout(ex.Message + ex.StackTrace);
                return false;
            }

        }
        /// <summary>
        /// 序列化所有内容
        /// </summary>
        /// <param name="stream"></param>
        /// <returns></returns>
        public bool SerailizeToMemory(MemoryStream stream)
        {
            try
            {
                BinaryWriter binWriter = new BinaryWriter(stream);
                binWriter.Write((uint)VersionsManager.CurrentVersion);

                binWriter.Write(recipientsList.Count);
                for (int i = 0; i < recipientsList.Count; i++)
                {
                    recipientsList[i].SerailizeToMemory(stream);
                }
                PublicMethod.WriteStringToStream(ref binWriter, emailTitle);
                binWriter.Write(reportTemplatePathList.Count);
                for (int i = 0; i < reportTemplatePathList.Count; i++)
                {
                    PublicMethod.WriteStringToStream(ref binWriter, reportTemplatePathList[i]);
                }

                autoExportProgram.SerailizeToMemory(stream);
                binWriter.Write(startSendTime.ToBinary());

             
                binWriter.Write(SavePathParent);
                

                return true;
            }
            catch (System.Exception ex)
            {
                DbgTrace.dout("{0}", ex.Message);
                return false;
            }

        }
        public bool DeserializeFromMemory(MemoryStream stream)
        {
            if (stream == null)
                return false;
            try
            {
                BinaryReader binReader = new BinaryReader(stream);
                version = binReader.ReadUInt32();

                int recipentCount = binReader.ReadInt32();
                for (int i = 0; i < recipentCount; i++)
                {
                    RecipientsInfo recipient = new RecipientsInfo();
                    recipient.DeserializeFormMemory(stream);
                    recipientsList.Add(recipient);
                }
                emailTitle = PublicMethod.ReadStringFromStream(binReader);
                int templateCount = binReader.ReadInt32();
                for (int i = 0; i < templateCount; i++)
                {
                    string excelTemplate = PublicMethod.ReadStringFromStream(binReader);
                    reportTemplatePathList.Add(excelTemplate);
                }

                autoExportProgram.DeserializeFormMemory(stream);
                startSendTime = DateTime.FromBinary(binReader.ReadInt64());

                if (version > (uint)VersionsManager.VersionWithoutLocationForEachSendPlan)
                {
                    SavePathParent= binReader.ReadString();
                }
                return true;
            }
            catch (System.Exception ex)
            {
                DbgTrace.dout(ex.Message + ex.StackTrace);
                return false;
            }
        }
        public bool DeserializeBeginOrNotFromMemory(MemoryStream stream)
        {
            if (stream == null)
                return false;
            try
            {
                BinaryReader binReader = new BinaryReader(stream);
                version = binReader.ReadUInt32();
                BeginOrNot = binReader.ReadBoolean();
                EmailTitle = binReader.ReadString();
                return true;
            }
            catch (System.Exception ex)
            {
                DbgTrace.dout(ex.Message + ex.StackTrace);
                return false;
            }
        }
    }
   
    /// <summary>
    /// 模板文件信息
    /// </summary>
    [Serializable]
    public class OfficeReportInfo : ICloneable
    {
        public OfficeReportInfo()
        {
            version =(uint)VersionsManager.CurrentVersion;
            reportUrl = string.Empty;
            valid = 0;
            modifyTime = DateTime.Now;
            _fileType =1;
        }
        //版本号
        private uint version = (uint)VersionsManager.CurrentVersion;
        private string reportUrl = string.Empty;
        private int valid;
        private DateTime modifyTime;
        /// <summary>
        /// 0表示普通模板，1表示预置模板，2表示查询条件模板
        /// </summary>
        private int _fileType = 1;

        public int FileType
        {
            get { return _fileType; }
            set { _fileType = value; }
        }
        /// <summary>
        /// 报表模板路径，相对路径
        /// </summary>
        public string ReportUrl
        {
            get { return reportUrl; }
            set { reportUrl = value; }
        }

        /// <summary>
        /// 状态信息 0未校验，1已校验
        /// </summary>
        public int Valid
        {
            get { return valid; }
            set { valid = value; }
        }

        /// <summary>
        /// 上次修改时间
        /// </summary>
        public DateTime ModifyTime
        {
            get { return modifyTime; }
            set { modifyTime = value; }
        }
        public object Clone()
        {
            return this.MemberwiseClone();
        }
        public bool WriteToStream(BinaryWriter binWriter)
        {
            try
            {
                binWriter.Write((uint)VersionsManager.CurrentVersion);
                PublicMethod.WriteStringToStream(ref binWriter, reportUrl);
                binWriter.Write(valid);
                binWriter.Write(modifyTime.ToBinary());
                
                binWriter.Write(_fileType);
                return true;
            }
            catch (System.Exception ex)
            {
                DbgTrace.dout(ex.Message + ex.StackTrace);
                return false;
            }
        }

        public bool SerailizeToMemory(MemoryStream stream)
        {
            try
            {
                BinaryWriter binWriter = new BinaryWriter(stream);
                binWriter.Write((uint)VersionsManager.CurrentVersion);
                PublicMethod.WriteStringToStream(ref binWriter, reportUrl);
                binWriter.Write(valid);
                binWriter.Write(modifyTime.ToBinary());
                //在增加FileType之前，版本号是0.0之后的都增加了FileType。
               
                binWriter.Write(_fileType);
                return true;
            }
            catch (System.Exception ex)
            {
                DbgTrace.dout(ex.Message + ex.StackTrace);
                return false;
            }
        }

        public bool ReadFromStream(BinaryReader binReader)
        {
            //if (stream == null)
            //    return false;
            try
            {
                version = binReader.ReadUInt32();
                reportUrl = PublicMethod.ReadStringFromStream(binReader);

                valid = binReader.ReadInt32();
                modifyTime = DateTime.FromBinary(binReader.ReadInt64());
                if (version > (uint)VersionsManager.versionWithoutFileTypeForOfficeReportInfo)
                    FileType=binReader.ReadInt32();
                return true;
            }
            catch (System.Exception ex)
            {
                DbgTrace.dout(ex.Message + ex.StackTrace);
                return false;
            }
        }
        public bool DeserializeFormMemory(MemoryStream stream)
        {
            if (stream == null)
                return false;
            try
            {
                BinaryReader binReader = new BinaryReader(stream);
                version = binReader.ReadUInt32();
                reportUrl = PublicMethod.ReadStringFromStream(binReader);

                valid = binReader.ReadInt32();
                modifyTime = DateTime.FromBinary(binReader.ReadInt64());
                if (version > (uint)VersionsManager.versionWithoutFileTypeForOfficeReportInfo)
                    FileType = binReader.ReadInt32();
                return true;
            }
            catch (System.Exception ex)
            {
                DbgTrace.dout(ex.Message + ex.StackTrace);
                return false;
            }
        }
    }

    /// <summary>
    /// 收件人信息
    /// </summary>
    [Serializable]
    public class RecipientsInfo : ICloneable
    {
        private uint version=(uint)VersionsManager.CurrentVersion;
        private string recipientName;
        private string recipientEmail;

        /// <summary>
        /// 收件地址
        /// </summary>
        public string RecipientName
        {
            get { return recipientName; }
            set { recipientName = value; }
        }

        /// <summary>
        /// 收件人名
        /// </summary>
        public string RecipientEmail
        {
            get { return recipientEmail; }
            set { recipientEmail = value; }
        }

        public object Clone()
        {
            return this.MemberwiseClone();
        }

        public bool SerailizeToMemory(MemoryStream stream)
        {
            try
            {
                BinaryWriter binWriter = new BinaryWriter(stream);
                binWriter.Write((uint)VersionsManager.CurrentVersion);
                PublicMethod.WriteStringToStream(ref binWriter, recipientName);
                PublicMethod.WriteStringToStream(ref binWriter, recipientEmail);
                return true;
            }
            catch (System.Exception ex)
            {
                DbgTrace.dout(ex.Message + ex.StackTrace);
                return false;
            }
        }

        public bool DeserializeFormMemory(MemoryStream stream)
        {
            if (stream == null)
                return false;
            try
            {
                BinaryReader binReader = new BinaryReader(stream);
                version = binReader.ReadUInt32();
                recipientName = PublicMethod.ReadStringFromStream(binReader);
                recipientEmail = PublicMethod.ReadStringFromStream(binReader);
                return true;
            }
            catch (System.Exception ex)
            {
                DbgTrace.dout(ex.Message + ex.StackTrace);
                return false;
            }
        }
    }

    /// <summary>
    /// 邮件发送相关信息
    /// </summary>
    [Serializable]
    public class EmailSendSet : ICloneable
    {
        private uint version = (uint) VersionsManager.CurrentVersion;
        private string sendEmailAddress = "";
        private string serverHost = "";
        private int serverPort = 25;
        private string userName = "";
        private string passWord = "";
        private int retrytimes;
        private int waittime;
        private string reportSaveUrl = "";
        private int saveStyle;

        #region 属性

        /// <summary>
        /// 发件地址
        /// </summary>
        public string SendEmailAddress
        {
            get { return sendEmailAddress; }
            set { sendEmailAddress = value; }
        }

        /// <summary>
        /// 邮件服务器IP
        /// </summary>
        public string ServerHost
        {
            get { return serverHost; }
            set { serverHost = value; }
        }

        /// <summary>
        /// 邮件服务器端口号
        /// </summary>
        public int ServerPort
        {
            get { return serverPort; }
            set { serverPort = value; }
        }
        /// <summary>
        /// 邮件服务器名称
        /// </summary>
        public string UserName
        {
            get { return userName; }
            set { userName = value; }
        }

        /// <summary>
        /// 邮件服务器密码
        /// </summary>
        public string PassWord
        {
            get { return passWord; }
            set { passWord = value; }
        }

        /// <summary>
        /// 重发次数
        /// </summary>
        public int Retrytimes
        {
            get { return retrytimes; }
            set { retrytimes = value; }
        }

        /// <summary>
        /// 等待时间
        /// </summary>
        public int Waittime
        {
            get { return waittime; }
            set { waittime = value; }
        }

        /// <summary>
        /// 自动发送保存路径
        /// </summary>
        public string ReportSaveUrl
        {
            get { return reportSaveUrl; }
            set { reportSaveUrl = value; }
        }

        /// <summary>
        /// 自动报表保存方式，0为按年，1为按月
        /// </summary>
        public int SaveStyle
        {
            get { return saveStyle; }
            set { saveStyle = value; }
        }

        # endregion

        public object Clone()
        {
            return this.MemberwiseClone();
        }

        public MemoryStream SerailizeToMemory(MemoryStream stream)
        {
            try
            {
                BinaryWriter binWriter = new BinaryWriter(stream);
                binWriter.Write((uint)VersionsManager.CurrentVersion);
                PublicMethod.WriteStringToStream(ref binWriter, sendEmailAddress);
                PublicMethod.WriteStringToStream(ref binWriter, serverHost);
                binWriter.Write(serverPort);
                PublicMethod.WriteStringToStream(ref binWriter, userName);
                PublicMethod.WriteStringToStream(ref binWriter, passWord);
                binWriter.Write(retrytimes);
                binWriter.Write(waittime);
                PublicMethod.WriteStringToStream(ref binWriter, ReportSaveUrl);
                binWriter.Write(saveStyle);

                return stream;
            }
            catch (System.Exception ex)
            {
                DbgTrace.dout(ex.Message + ex.StackTrace);
            }
            return null;
        }

        public bool DeserializeFromMemory(MemoryStream stream)
        {
            if (stream == null || stream.Length == 0)
                return false;
            try
            {
                BinaryReader binReader = new BinaryReader(stream);
                version = binReader.ReadUInt32(); 
                sendEmailAddress = PublicMethod.ReadStringFromStream(binReader);
                ServerHost = PublicMethod.ReadStringFromStream(binReader);
                serverPort = binReader.ReadInt32();
                userName = PublicMethod.ReadStringFromStream(binReader);
                passWord = PublicMethod.ReadStringFromStream(binReader);
                retrytimes = binReader.ReadInt32();
                waittime = binReader.ReadInt32();
                ReportSaveUrl = PublicMethod.ReadStringFromStream(binReader);
                if (version > (uint)VersionsManager.versionWithoutSaveStyleForEmailSendSet)
                {
                    //兼容性处理，多添加了一个字段，那么在读出的时候就根据version的高低，新的就多读一个字段，老的就不读这个新加的字段
                    saveStyle = binReader.ReadInt32();
                } 

                return true;
            }
            catch (System.Exception ex)
            {
                DbgTrace.dout(ex.Message + ex.StackTrace);
                return false;
            }
        }
    }

    /// <summary>
    /// 邮件接收人，发送列表
    /// </summary>
    [Serializable]
    public class OfficeReportData : ICloneable
    {

        public static object lockObj = new object();
        //二进制流版本号，方便以后扩展
        private uint version=(uint)VersionsManager.CurrentVersion;
        private EmailSendSet emailSendSet = new EmailSendSet();
        private List<RecipientsInfo> recipientsList = new List<RecipientsInfo>();
        private List<OfficeReportInfo> reportList = new List<OfficeReportInfo>();
        private List<EmailSendPlan> sendPlanList = new List<EmailSendPlan>();
  
        /// <summary>
        /// 配置的默认的报表查询保存的路径
        /// </summary>
        public string DefaultReportPath
        {
            get { return emailSendSet.ReportSaveUrl; }
            set { emailSendSet.ReportSaveUrl = value; }
        }

        /// <summary>
        /// 邮件发送相关信息
        /// </summary>
        public EmailSendSet EmailSendSetIntence
        {
            get { return emailSendSet; }
            set { emailSendSet = value; }
        }

        /// <summary>
        /// 收件人列表
        /// </summary>
        public List<RecipientsInfo> RecipientsList
        {
            get { return recipientsList; }
            set { recipientsList = value; }
        }

        /// <summary>
        /// 报表模板校验信息列表列表
        /// </summary>
        public List<OfficeReportInfo> ReportList
        {
            get { return reportList; }
            set { reportList = value; }
        }

        /// <summary>
        /// 发送计划列表
        /// </summary>
        public List<EmailSendPlan> SendPlanList
        {
            get { return sendPlanList; }
            set { sendPlanList = value; }
        }

        public object Clone()
        {
            lock (lockObj)
            {
                OfficeReportData obj = new OfficeReportData(this);
                return obj;
            }
        }

        public OfficeReportData GetCopy()
        {
            return this.Clone() as OfficeReportData;
        }

        private void InitValues()
        {
            version=(uint)VersionsManager.CurrentVersion;
            emailSendSet = new EmailSendSet();
            recipientsList = new List<RecipientsInfo>();
            reportList = new List<OfficeReportInfo>();
            sendPlanList = new List<EmailSendPlan>();
            DefaultReportPath=string.Empty;
        }

        public OfficeReportData()
        {
            InitValues();
        }

        /// <summary>
        /// 拷贝构造函数
        /// </summary>
        /// <param name="srcCell"></param>
        private OfficeReportData(OfficeReportData reportData)
        {
            InitValues();
            this.version = reportData.version;          
            this.emailSendSet = reportData.emailSendSet.Clone() as EmailSendSet;
            for (int i = 0; i < reportData.recipientsList.Count; i++)
            {
                this.recipientsList.Add(reportData.recipientsList[i].Clone() as RecipientsInfo);
            }

            for (int i = 0; i < reportData.reportList.Count; i++)
            {
                this.reportList.Add(reportData.reportList[i].Clone() as OfficeReportInfo);
            }

            for (int i = 0; i < reportData.sendPlanList.Count; i++)
            {
                this.sendPlanList.Add(reportData.sendPlanList[i].Clone() as EmailSendPlan);
            }
        }

        public MemoryStream SerailizeBeginOrNotToMemory()
        {
            try
            {
                MemoryStream stream = new MemoryStream();
                BinaryWriter binWriter = new BinaryWriter(stream);

                binWriter.Write((uint)VersionsManager.CurrentVersion);
                binWriter.Write(SendPlanList.Count);
                for (int i = 0; i < SendPlanList.Count; i++)
                {
                    SendPlanList[i].SerailizeBeginOrNotToMemory(stream);
                }
    
                return stream;
            }
            catch (System.Exception ex)
            {
                DbgTrace.dout(ex.Message + ex.StackTrace);
                return null;
            }
        }


        public MemoryStream SerailizeToMemory()
        {
            try
            {
                MemoryStream stream = new MemoryStream();
                BinaryWriter binWriter = new BinaryWriter(stream);

                binWriter.Write((uint)VersionsManager.CurrentVersion);
                emailSendSet.SerailizeToMemory(stream);
                binWriter.Write(recipientsList.Count);
                for (int i = 0; i < recipientsList.Count; i++)
                {
                    recipientsList[i].SerailizeToMemory(stream);
                }

                binWriter.Write(reportList.Count);
                for (int i = 0; i < reportList.Count; i++)
                {
                    reportList[i].SerailizeToMemory(stream);
                }

                binWriter.Write(SendPlanList.Count);
                for (int i = 0; i < SendPlanList.Count; i++)
                {
                    SendPlanList[i].SerailizeToMemory(stream);
                }
                PublicMethod.WriteStringToStream(ref binWriter, this.DefaultReportPath);
                return stream;
            }
            catch (System.Exception ex)
            {
                DbgTrace.dout("{0}", ex.Message);
                return null;
            }
        }

        public bool DeserializeFromMemory(MemoryStream stream)
        {
            if (stream == null || stream.Length==0)
                return false;
            try
            {
                InitValues();
                BinaryReader binReader = new BinaryReader(stream);
                version = binReader.ReadUInt32();
                emailSendSet.DeserializeFromMemory(stream);

                int recipientCoune = binReader.ReadInt32();
                for (int i = 0; i < recipientCoune; i++)
                {
                    RecipientsInfo rec = new RecipientsInfo();
                    rec.DeserializeFormMemory(stream);
                    recipientsList.Add(rec);
                }

                int reportCount = binReader.ReadInt32();
                for (int i = 0; i < reportCount; i++)
                {
                    OfficeReportInfo reportTemplate = new OfficeReportInfo();
                    reportTemplate.DeserializeFormMemory(stream);
                    reportList.Add(reportTemplate);
                }

                int planCount = binReader.ReadInt32();
                for (int i = 0; i < planCount; i++)
                {
                    EmailSendPlan sendPlan = new EmailSendPlan();
                    sendPlan.DeserializeFromMemory(stream);
                    sendPlanList.Add(sendPlan);
                }
                if (version>=(uint)VersionsManager.VersionWithoutLocationForEachSendPlan)
                    this.DefaultReportPath = PublicMethod.ReadStringFromStream(binReader);
                return true;
            }
            catch (System.Exception ex)
            {
                DbgTrace.dout("{0}", ex.Message);
                return false;
            }
        }

        public bool DeserializeBegionOrNotFromMemory(MemoryStream stream)
        {
            if (stream == null || stream.Length == 0)
                return false;
            try
            {
                BinaryReader binReader = new BinaryReader(stream);
                version = binReader.ReadUInt32();
              
                int planCount = binReader.ReadInt32();
             
                for (int i = 0; i < planCount; i++)
                {
                    EmailSendPlan sendPlan = new EmailSendPlan();
                    sendPlan.DeserializeBeginOrNotFromMemory(stream);
                    sendPlanList.Add(sendPlan);
                }
                return true;
            }
            catch (System.Exception ex)
            {
                DbgTrace.dout(ex.Message + ex.StackTrace);
                return false;
            }
        }

        public static OfficeReportData ReadFromLocalFile(string strFilePath,string defaultReportPath)
        {
            try
            {
                if (!File.Exists(strFilePath))
                {
                    FileStream fs = File.Create(strFilePath);
                    fs.Close();
                }
                FileStream fsT = new FileStream(strFilePath, FileMode.Open);
                byte[] array = new byte[fsT.Length];
                int A = fsT.Read(array, 0, (int)fsT.Length);
                MemoryStream stream = new MemoryStream(array);
                OfficeReportData totalInfor = new OfficeReportData();
                totalInfor.DeserializeFromMemory(stream);
                
                totalInfor.DefaultReportPath = defaultReportPath;
                fsT.Close();
                return totalInfor;
            }
            catch (System.Exception ex)
            {
                DbgTrace.dout(ex.Message + ex.StackTrace);
                OfficeReportData totalInfor = new OfficeReportData();
                totalInfor.DefaultReportPath = defaultReportPath;
                return totalInfor;
            }
            return null;
        }
        public static OfficeReportData ReadBegionOrNotFromLocalFile(string strFilePath, string defaultReportPath)
        {
            try
            {
                if (!File.Exists(strFilePath))
                {
                    FileStream fs = File.Create(strFilePath);
                    fs.Close();
                }
                FileStream fsT = new FileStream(strFilePath, FileMode.Open);
                byte[] array = new byte[fsT.Length];
                int A = fsT.Read(array, 0, (int)fsT.Length);
                MemoryStream stream = new MemoryStream(array);
                OfficeReportData totalInfor = new OfficeReportData();

                totalInfor.DeserializeBegionOrNotFromMemory(stream);
                
                totalInfor.DefaultReportPath = defaultReportPath;
                fsT.Close();
                return totalInfor;
            }
            catch (System.Exception ex)
            {
                DbgTrace.dout(ex.Message + ex.StackTrace);
            }
            return null;
        }
    }

    public static class PublicMethod
    {
        /// <summary>
        /// 把字符串写入流
        /// </summary>
        /// <param name="binWriter"></param>
        /// <param name="str"></param>
        /// <returns></returns>
        public static void WriteStringToStream(ref BinaryWriter binWriter, string str)
        {
            binWriter.Write(Encoding.Default.GetBytes(str).Length);
            binWriter.Write(Encoding.Default.GetBytes(str));
        }

        public static string ReadStringFromStream(BinaryReader binReader)
        {
            int length = binReader.ReadInt32();
            byte[] weekOrderByte = binReader.ReadBytes(length);
            return Encoding.Default.GetString(weekOrderByte);
        }
    }

}
