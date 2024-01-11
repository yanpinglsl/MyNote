using BasicDataInterface.Models.Response;
using DBInterfaceModels;
using System;
using System.Collections.Generic;
using System.Data;
using System.Threading;

namespace OfficeReportInterface
{
    class EndTimeManager
    {
        #region 单例
        private static EndTimeManager _EndTimeManager = null;

        private EndTimeManager()
        {

        }
        public static EndTimeManager GetInstance()
        {
            if (_EndTimeManager != null)
                return _EndTimeManager;

            EndTimeManager endTimeManagerTemp = new EndTimeManager();
            Interlocked.CompareExchange(ref _EndTimeManager, endTimeManagerTemp, null);
            return _EndTimeManager;
        }
        #endregion

        #region 使用数据库接口的时候常用

        private  DateTime _lastStartEventTime = DateTime.MinValue;
        public string LastErrorString { get; set; }
        private static void Dispose(DataTable eventDT)
        {
            if (eventDT != null)
                eventDT.Dispose();
        }

        private void WriteExceptionLog(Exception ex)
        {
            LastErrorString = ex.Message;
            DbgTrace.dout(ex.Message + ex.StackTrace);
        }
        private void WriteLog(int errorCode)
        {
            LastErrorString = DBInterfaceCommonLib.ErrorQuerier.Instance.GetLastErrorString();
            DbgTrace.dout("EventlogProvider.Instance.ReadEventLogsByMultiStaChnDev invoked failed.{0}.ErrorCode={1}.",    LastErrorString, errorCode);
        }

        #endregion

        #region 真正实现在别的类的函数

        //private  int ReadEventLogsByMultiStaChnDev(List<NODE_TYPE_ID> nodeTypeIDList, DateTime startTime, DateTime endTime, int[] eventType, ref DataTable eventDT)
        //{
        //    return PD_TB_06_Manager.ReadEventLogsByMultiStaChnDev(nodeTypeIDList, startTime, endTime, eventType,
        //        ref eventDT);
        //}

        private  List<NODE_TYPE_ID> GetNodeTypeIDList(List<uint> deviceIdList)
        {
            return DirectionManager.GetNodeTypeIDList(deviceIdList);
        }
        /// <summary>
        /// 从DataRow读取PD_TB_06表的一行数据的部分数据到eventInformation对象
        /// </summary>
        /// <param name="oneRow"></param>
        /// <param name="eventInformation"></param>
        /// <returns></returns>
        private bool GetStartEventKey(EventLogByDevResponse oneRow, out StartEventKey eventInformation)
        {
            return StartEventKey.GetStartEventKey(oneRow, out eventInformation);

        }
        #endregion

        #region 获取所有设备（无论是否是所传入的设备）的最后一个起始事件对应的时间

        public void SetLastEventTime(DateTime time)
        {
            _lastStartEventTime = time;
        }

        #region 获取所有的设备的EventType=18的事件中，最后一个起始事件对应的时间  函数
        /// <summary>
        /// 获取所有的设备的EventType=18的事件中，最后一个起始事件对应的时间
        /// </summary>
        /// <param name="startTime"></param>
        /// <param name="endTime"></param>
        /// <param name="lastStartEventTime"></param>
        /// <returns></returns>
        public bool GetLastEventTime(DateTime startTime, DateTime endTime, out DateTime lastStartEventTime)
        {
            //计算过一次就不再计算
            if (_lastStartEventTime != DateTime.MinValue)
            {
                lastStartEventTime = _lastStartEventTime;
                return true;
            }

            //下面从数据库查询获取
            lastStartEventTime = endTime;

            //获取所有设备的设备ID
            PC_TB_05Manager pC_TB_05Manager = new PC_TB_05Manager();
            List<uint> deviceList = new List<uint>();
            if (!pC_TB_05Manager.LoadDeviceList(out deviceList))
                return false;
            //查询所有的设备的起始事件
            List<NODE_TYPE_ID> nodeTypeIDList = GetNodeTypeIDList(deviceList);

            int[] eventType = { SysConstDefinition.SAG_SWELL_TYPE };
            try
            {
                var returnReslut = PD_TB_06_Manager.ReadEventLogsByMultiStaChnDev(nodeTypeIDList, startTime, endTime, eventType, new int[] { });
                if (!returnReslut.Success)
                {
                    DbgTrace.dout("GetAllEvents:{0}", returnReslut.ErrorMessage);
                    return false;
                }
                //选择出最后一个起始事件的行
                List< EventLogByDevResponse> eventDT = returnReslut.ResultList;
                //起始事件要满足的条件：1.EventType=18；2.Code2=1；3.Code1在SwellCodes，SagCodes，InteruptCodes,OtherCodes中
                for (int i = eventDT.Count - 1; i >= 0; --i)
                {
                    StartEventKey eventInformation;
                    if (!GetStartEventKey(eventDT[i], out eventInformation))
                        continue;
                    if (!eventInformation.IsStartEvent(startTime, endTime))
                        continue;
                    lastStartEventTime = eventInformation.FullTime;
                    //将这个值保存起来，下次就不用再查数据库获取了
                    _lastStartEventTime = lastStartEventTime;
                    return true;
                }
            }
            catch (System.Exception ex)
            {
                WriteExceptionLog(ex);
                return false;
            }
            finally
            {
                //Dispose(eventDT);
            }
            return false;
        }
        #endregion


        #endregion
    }
}
