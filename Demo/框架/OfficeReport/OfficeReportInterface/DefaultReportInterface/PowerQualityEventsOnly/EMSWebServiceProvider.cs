using System;
using System.Collections.Generic;
using System.Data;
using System.Threading;
using DBInterfaceCommonLib;

namespace OfficeReportInterface
{
    class EMSWebServiceProvider
    {
        private static EMSWebServiceProvider _Instance = null;

        public static EMSWebServiceProvider Instance
        {
            get
            {
                if(_Instance!=null)
                return _Instance;
                EMSWebServiceProvider temp = new EMSWebServiceProvider();
                Interlocked.CompareExchange(ref _Instance, temp, null);
                return _Instance;
            }

        }

        /// <summary>
        /// 查询事件确认信息
        /// </summary>
        /// <param name="toArray"></param>
        /// <param name="startTime"></param>
        /// <param name="endTime"></param>
        /// <param name="ints"></param>
        /// <param name="toArray1"></param>
        /// <param name="i"></param>
        /// <param name="queryResult"></param>
        /// <returns></returns>
        public int ReadEventAckInfo(NodeDataParam[] toArray, DateTime startTime, DateTime endTime, int[] ints, int[] toArray1, int i, ref DataTable queryResult)
        {
            queryResult = new DataTable();
            return (int)ErrorCode.Success;
        }

        /// <summary>
        /// 查询事件PD_TB_06表的起始事件（eventType=18）
        /// </summary>
        /// <param name="dBOperationFlag"></param>
        /// <param name="deviceIds"></param>
        /// <param name="startTime"></param>
        /// <param name="endTime"></param>
        /// <param name="queryType">17,18,0</param>
        /// <param name="eventCodes"></param>
        /// <param name="code2">传入值1</param>
        /// <param name="eventID"></param>
        /// <param name="maxRowCount"></param>
        /// <param name="queryResult"></param>
        /// <returns></returns>
        internal  int QueryRMSEventLogs(DBOperationFlag dBOperationFlag, uint[] deviceIds, DateTime startTime, DateTime endTime, int queryType, int[] eventCodes, int code2, uint eventID, int maxRowCount, ref DataTable queryResult)
        {
         
            //查询起始事件
            return (int)ErrorCode.Success;
        }




        //写evestr1，这个要实现。首先检查看通用接口有没有该功能。通用数据库接口只有读取的功能
        public int WriteRMSEventStr(uint eventId, int year, string eventStr)
        {
     
            return (int)ErrorCode.Success;
        }

    }
}
