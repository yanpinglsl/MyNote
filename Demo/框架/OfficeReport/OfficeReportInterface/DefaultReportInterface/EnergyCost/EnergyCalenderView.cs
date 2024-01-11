using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace OfficeReportInterface.DefaultReportInterface.EnergyCost
{
    /// <summary>
    /// 分时计量年历视图
    /// </summary>
   public class EnergyCalenderView
    {
        /// <summary>
        /// 构造函数
        /// </summary>
        public EnergyCalenderView(string nodeName)
        {
            this.nodeName = nodeName;
            this.year = 0;
            this.dailyNodeIDs = new List<uint>();
        }


        public string nodeName;

        /// <summary>
        /// 节点年份
        /// </summary>
        public int Year
        {
            set { year = value; }
            get { return year; }
        }
        /// <summary>
        /// 日方案ID列表
        /// </summary>
        public List<uint> DailyNodeIDs
        {
            set { dailyNodeIDs = value; }
            get { return dailyNodeIDs; }
        }

        private int year;
        private List<uint> dailyNodeIDs;

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
                    this.year = binReader.ReadInt32();
                    //返回366天的日费率方案
                    for (int i = 0; i < 366; i++)
                        this.dailyNodeIDs.Add(binReader.ReadUInt32());
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
