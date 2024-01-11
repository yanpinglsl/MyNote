using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using CET.PecsNodeManage;

namespace OfficeReportInterface.DefaultReportInterface.EnergyCost
{
    /// <summary>
    /// 日时段方案结果
    /// </summary>
    public class DayProfileStruct
    {
        public string periodName;
        public int periodIndex;
        public List<TariffPeriodTime> periodTime;

        public DayProfileStruct()
        {
            periodTime = new List<TariffPeriodTime>();
        }
        /// <summary>
        /// 反序列化函数，加载解析节点Data属性
        /// </summary>
        /// <param name="mStream"></param>
        /// <returns></returns>
        public bool LoadFromStream(MemoryStream mStream)
        {
            bool result = true;
            if (mStream != null && mStream.Length > 0)
            {
                mStream.Position = 0;
                BinaryReader binReader = new BinaryReader(mStream);
                try
                {
                    binReader.ReadInt32();//颜色位,没用到
                    int num = binReader.ReadInt32();
                    for (int i = 0; i < num; i++)
                    {
                        string stimeStr = SysNode.GetStringFromBytes(binReader.ReadBytes(10));
                        string etimeStr = SysNode.GetStringFromBytes(binReader.ReadBytes(10));
                        uint tariffID = binReader.ReadUInt32();

                        DateTime nowDate = DateTime.Now.Date;
                        DateTime stime = new DateTime();
                        DateTime etime = new DateTime();
                        if (stimeStr == "24:00:00")
                            stime = nowDate.AddDays(1);
                        else
                            stime = Convert.ToDateTime(stimeStr);
                        if (etimeStr == "24:00:00")
                            etime = nowDate.AddDays(1);
                        else
                            etime = Convert.ToDateTime(etimeStr);

                        TimeSpan sts = stime - nowDate;
                        TimeSpan ets = etime - nowDate;


                        DateTimePair timePair = new DateTimePair();
                        timePair.startTimeMinute = sts.Hours * 60 + sts.Minutes;
                        timePair.endTimeMinute = ets.Hours * 60 + ets.Minutes;
                        if (etimeStr == "24:00:00")
                            timePair.endTimeMinute = 1440;
                        bool isExist = false;
                        for (int j = 0; j < this.periodTime.Count; j++)
                        {
                            TariffPeriodTime tariffPeriodTime = periodTime[j];
                            if (tariffPeriodTime.tariffIndex == tariffID)
                            {
                                tariffPeriodTime.periodTimeList.Add(timePair);
                                isExist = true;
                                break;
                            }
                        }

                        if (!isExist)
                        {
                            TariffPeriodTime tariffPeriodTime = new TariffPeriodTime();
                            tariffPeriodTime.tariffIndex = Convert.ToInt32(tariffID);
                            tariffPeriodTime.periodTimeList.Add(timePair);
                            this.periodTime.Add(tariffPeriodTime);
                        }
                    }
                }
                catch
                {
                    result = false;
                }
                finally
                {
                    binReader.Close();
                }
            }

            return result;
        }
    }
}
