using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Threading;
using DBInterfaceModels;
using EMSDBInterface;

namespace OfficeReportInterface
{
    public class CommandInteraction
    {
        private Thread commandInteractionThread;
        private ProcCommState commandState;
        private bool tryExportAgain;
        private string lastErrorString;
        private uint commandID;
        private bool isCancel = false;

        /// <summary>
        /// 是否是外部取消等待缓存
        /// </summary>
        public bool IsCancel
        {
            get { return isCancel; }
            set { isCancel = value; }
        }

        /// <summary>
        /// 交互命令ID
        /// </summary>
        public uint CommandID
        {
            get { return commandID; }
            set { commandID = value; }
        }

        /// <summary>
        /// 错误字符串
        /// </summary>
        public string LastErrorString
        {
            get { return lastErrorString; }
        }

        /// <summary>
        /// 是否再次导出报表
        /// </summary>
        public bool TryExportAgain
        {
            get { return tryExportAgain; }
        }

        /// <summary>
        /// 交互命令状态
        /// </summary>
        public ProcCommState CommandState
        {
            get { return commandState; }
            set { commandState = value; }
        }

        public CommandInteraction(uint commandID)
        {
            this.isCancel = false;
            this.commandState = ProcCommState.ProcInitial;
            this.commandID = commandID;
            this.tryExportAgain = false;
            StartCommandInteraction();
        }

        /// <summary>
        /// 停止命令交互过程
        /// </summary>
        public void StopCommandInteraction()
        {
            isCancel = true;
        }

        /// <summary>
        /// 通过新建线程方式启动命令交互过程
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void StartCommandInteraction()
        {
            this.commandInteractionThread = new Thread(new ThreadStart(WaitForCommandInteraction));
            this.commandInteractionThread.IsBackground = true;
            this.commandInteractionThread.Start();
        }

        public void WaitForCommandInteraction()
        {
            while (true)
            {
                if (this.isCancel)
                {
                    SysCommandProvider.Instance.DeleteSysCommand(this.commandID);
                    break;
                }
                RespondCommand();
                if (ComplicateCommandPro())
                    break;
                Thread.Sleep(1000);
            }
        }

        /// <summary>
        /// 相应交互命令状态的变化情况
        /// </summary>
        private void RespondCommand()
        {
            List<SYSTEM_COMMAND> system_command = CommandManager.AnalysisComm(this.commandID);
            if (system_command.Count == 0)
            {
                this.commandState = ProcCommState.ProcFalse;
                return;
            }
            //判断发出的命令是否超时，若超时则自己进行超时处理
            if (system_command[0].commandTime.AddSeconds(system_command[0].timeOut) < DateTime.Now)
            {
                this.commandState = ProcCommState.ProcTimeout;
                return;
            }
            this.commandState = (ProcCommState)system_command[0].state;
        }

        /// <summary>
        /// 交互命令是否处理完成
        /// </summary>
        /// <returns></returns>
        private bool ComplicateCommandPro()
        {
            switch (this.commandState)
            {
                case ProcCommState.ProcSuccess:
                    this.tryExportAgain = true;
                    SysCommandProvider.Instance.DeleteSysCommand(this.commandID);
                    return true;
                case ProcCommState.ProcFalse:
                    this.lastErrorString =  LocalResourceManager.GetInstance().GetString("0318","获取数据失败，是否重试？");
                    SysCommandProvider.Instance.DeleteSysCommand(this.commandID);
                    return true;
                case ProcCommState.ProcTimeout:
                    this.lastErrorString =  LocalResourceManager.GetInstance().GetString("0328","获取数据超时.是否重试？");
                    SysCommandProvider.Instance.DeleteSysCommand(this.commandID);
                    return true;
                case ProcCommState.ProcCommWrong:
                    this.lastErrorString = LocalResourceManager.GetInstance().GetString("0318", "获取数据失败，是否重试？");
                    SysCommandProvider.Instance.DeleteSysCommand(this.commandID);
                    return true;
                default:
                    return false;
            }
        }

        /// <summary>
        /// 由于是创建线程方式监听命令状态，不能自己阻塞，因此采用该方法来进行阻塞
        /// </summary>
        public void BlockCommonInteraction()
        {
            while (true)
            {
                if (this.isCancel)
                {
                    SysCommandProvider.Instance.DeleteSysCommand(this.commandID);
                    break;
                }
                if (this.commandState != ProcCommState.ProcInitial && this.commandState != ProcCommState.ProcStart)
                    break;
                Thread.Sleep(50);
            }
        }
    }
}
