using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using System.IO;
using System.Runtime.InteropServices;
using System.Collections;
using System.Data;
using DBInterfaceCommonLib;
using DBInterfaceModels;
using EMSDBInterface;

namespace OfficeReportInterface
{
    public class CommandLine
    {
        public int commandsnum;
        public Command[] commands;
        public MemoryStream SerailizeToMemory()
        {
 
            try
            {
                MemoryStream stream = new MemoryStream();
                BinaryWriter binWriter = new BinaryWriter(stream);
                binWriter.Write(commandsnum);
                for (int i = 0; i < commands.Length; i++)
                {
                    commands[i].SerailizeToMemory(stream);
                }
                return stream;
            }
            catch (Exception exc)
            {
                return null;
            }
                
            
           
        }

        public bool DeserializeFormMemory(MemoryStream stream)
        {
            try
            {
                if (stream == null)
                    return false;
                BinaryReader binReader = new BinaryReader(stream);
                commandsnum = binReader.ReadInt32();
                for (int i = 0; i < commands.Length; i++)
                {
                    Command cmd = new Command();
                    cmd.DeserializeFormMemory(stream);
                    commands[i] = cmd;
                }
                return true;
            }
            catch (Exception exc)
            {
                return false;
            }
        }
    }

    public class Command
    {
        public uint version = (uint)VersionsManager.CurrentVersion;
        public uint reportID;
        public uint nodeType;
        public uint nodeID;
        /// <summary>
        /// 时刻标准化已在引擎端处理
        /// </summary>
        public DateTime checkTime;

        public Command()
        { }

        public Command(uint reportID,uint nodeType,uint nodeID,DateTime checkTime)
        {
            this.version = (uint)VersionsManager.CurrentVersion;
            this.reportID = reportID;
            this.nodeType = nodeType;
            this.nodeID = nodeID;
            this.checkTime = checkTime; 
        }

        public bool SerailizeToMemory(MemoryStream stream)
        {
            try
            {
                BinaryWriter binWriter = new BinaryWriter(stream);
                binWriter.Write((uint)VersionsManager.CurrentVersion);
                binWriter.Write(reportID);
                binWriter.Write(nodeType);
                binWriter.Write(nodeID);
                binWriter.Write(checkTime.ToBinary());
                return true;
            }
            catch (Exception exc)
            {
                return false;
            }
        }

        public bool DeserializeFormMemory(MemoryStream stream)
        {
            try
            {
                if (stream == null)
                    return false;
                BinaryReader binReader = new BinaryReader(stream);
                version = binReader.ReadUInt32();
                reportID = binReader.ReadUInt32();
                nodeType = binReader.ReadUInt32();
                nodeID = binReader.ReadUInt32();
                checkTime = DateTime.FromBinary(binReader.ReadInt64()); ;
                return true;
            }
            catch (Exception exc)
            {
                return false;
            }
        }
    }

    /// <summary>
    /// 交互命令执行状态
    /// </summary>
    public enum ProcCommState
    {
        /// <summary>
        /// 避免引擎端在读取时读出所有指令（state=0读取所有）
        /// </summary>
        ProcInitial = 1,  
        ProcStart = 2,
        ProcSuccess = 3,
        ProcFalse = 4,
        ProcTimeout = 5,
        ProcCommWrong = 6,
    }

    public class CommandManager
    {
        public static List<SYSTEM_COMMAND> newCommands = new List<SYSTEM_COMMAND>();
        private const int CONST_RESULTLENGTH = 1200;
        private const int CONST_COMMANDLENGTH = 6500;
        /// <summary>
        /// 命令超时时间（暂定1小时）
        /// </summary>
        private const int CONST_TIMEOUT = 60 * 60;
        /// <summary>
        /// 命令类型
        /// </summary>
        private const int CONST_TYPE = 2;

        /// <summary>
        /// 根据需求信息编译生成相应的交互命令
        /// </summary>
        /// <param name="commandList">报表生成过程中记录的需要缓存的内容</param>
        /// <returns></returns>
        public static SYSTEM_COMMAND CompileCommand(List<Command> commandList)
        {
            SYSTEM_COMMAND newCommand = new SYSTEM_COMMAND();
            //设置命令发出时间
            newCommand.commandTime = DateTime.Now;
            //命令超时间隔
            newCommand.timeOut = CONST_TIMEOUT;
            //命令包类型
            newCommand.type = CONST_TYPE;
            //命令发出程序
            newCommand.source = "OfficeReport";
            //命令接收者
            newCommand.destination = "PecReportEngine";
            //执行命令的用户
            newCommand.operatorID = 0;          
            newCommand.state = (short)ProcCommState.ProcInitial;   
            newCommand.handleTime = NullValue.DTNA;
            //命令返回数据长度
            newCommand.resultLength = CONST_RESULTLENGTH;
            //命令返回数据流
            newCommand.result = null;
            //命令内容长度
            newCommand.commandLength = CONST_COMMANDLENGTH;         
            //对command字段的处理           
            Command[] commandsarray = new Command[commandList.Count];
            commandList.CopyTo(commandsarray);
            //处理为结构体
            CommandLine commandline = new CommandLine();
            commandline.commandsnum = commandList.Count;
            commandline.commands = commandsarray;
            //转换为二进制流
            newCommand.command = commandline.SerailizeToMemory().ToArray();
            newCommand.commandLength = newCommand.command.Length;  

            return newCommand;
        }

        /// <summary>
        /// （发出命令端）解析单条交互命令
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public static List<SYSTEM_COMMAND> AnalysisComm(uint id)
        {
            DataTable resultDT = new DataTable();
            int resultCode = SysCommandProvider.Instance.ReadSysCommandByID(id, ref resultDT);
            string error = DBInterfaceCommonLib.ErrorQuerier.Instance.GetLastErrorString();
            newCommands = new List<SYSTEM_COMMAND>();
            if (resultCode == (int)ErrorCode.Success)
            {
                newCommands = GetCommandList(resultDT);
            }
            return newCommands;
        }

        ///// <summary>
        ///// （执行命令端）解析交互命令集
        ///// </summary>
        ///// <param name="cmdType"></param>
        ///// <param name="destination"></param>
        ///// <param name="state"></param>
        ///// <returns></returns>
        //public static List<SYSTEM_COMMAND> AnalysisComm(uint cmdType, string destination, short state)
        //{
        //    DataTable resultDT = new DataTable();
        //    int resultCode = SysCommandProvider.Instance.ReadSysCommands(cmdType, destination, state, ref resultDT);
        //    newCommands = new List<SYSTEM_COMMAND>();
        //    if (resultCode == (int)ErrorCode.Success)
        //    {
        //        newCommands = GetCommandList(resultDT);                        
        //    }
        //    return newCommands;
        //}

        /// <summary>
        /// 将数据库表中信息转化为交互命令格式
        /// </summary>
        /// <param name="resultDT"></param>
        /// <returns></returns>
        private static List<SYSTEM_COMMAND> GetCommandList(DataTable resultDT)
        {
            List<SYSTEM_COMMAND> newCommands = new List<SYSTEM_COMMAND>();
            for (int i = 0; i < resultDT.Rows.Count; i++)
            {
                DataRow row = resultDT.Rows[i];
                SYSTEM_COMMAND newCommand = new SYSTEM_COMMAND();
                newCommand.id = Convert.ToUInt32(row["id"]);
                //设置命令发出时间
                newCommand.commandTime = Convert.ToDateTime(row["CommandTime"]);
                //命令超时间隔
                newCommand.timeOut = Convert.ToUInt32(row["TimeOut"]);
                //命令包类型，启停实时数据
                newCommand.type = Convert.ToUInt32(row["Type"]);
                //命令发出程序
                newCommand.source = Convert.ToString(row["Source"]);
                //命令接收者
                newCommand.destination = Convert.ToString(row["Destination"]);
                //执行命令的用户
                newCommand.operatorID = Convert.ToUInt32(row["OperatorID"]) ;         
                newCommand.state = Convert.ToInt16(row["State"]);
                newCommand.handleTime = NullValue.DTNA;
                if (!Convert.IsDBNull(row["HandleTime"]))
                    newCommand.handleTime = Convert.ToDateTime(row["HandleTime"]);
                //命令返回数据长度
                newCommand.resultLength = Convert.ToInt32(row["ResultLength"]);
                //命令返回数据流
                newCommand.result = new byte[newCommand.resultLength];   
                if (!Convert.IsDBNull(row["Result"]))
                    newCommand.result = (byte[])row["Result"];
                //命令内容长度
                newCommand.commandLength = Convert.ToInt32(row["CommandLength"]);         
                newCommand.command = new byte[newCommand.commandLength];
                if (!Convert.IsDBNull(row["Command"]))
                {
                    newCommand.command = (byte[])row["Command"];
                }
                newCommands.Add(newCommand);
            }
            return newCommands;
        }
    }
}
