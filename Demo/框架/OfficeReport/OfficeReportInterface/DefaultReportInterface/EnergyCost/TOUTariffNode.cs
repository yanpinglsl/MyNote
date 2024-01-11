using System.Collections.Generic;
using System.IO;
using CET.PecsNodeManage;

namespace OfficeReportInterface.DefaultReportInterface.EnergyCost
{
    /// <summary>
    /// TOU费率节点
    /// </summary>
    public class TOUTariffNode
    {
        /// <summary>
        /// 构造函数
        /// </summary>
        public TOUTariffNode(uint nodeID, string nodeName)
        {
            this.nodeID = nodeID;
            this.nodeName = nodeName;
            this.tariffValues = new List<double>();
            this.currencyUnit = string.Empty;
        }

        /// <summary>
        /// 货币单位
        /// </summary>
        public string CurrencyUnit
        {
            set { currencyUnit = value; }
            get { return currencyUnit; }
        }
        /// <summary>
        /// 费率定义，固定返回6个费率值，分别表示kwh、kvarh、kvah、kw、kvar、kva
        /// </summary>
        public List<double> TariffValues
        {
            set { tariffValues = value; }
            get { return tariffValues; }
        }

        private string nodeName;

        public string NodeName
        {
            get { return nodeName; }
            set { nodeName = value; }
        }
        private uint nodeID;

        public uint NodeID
        {
            get { return nodeID; }
            set { nodeID = value; }
        }
        private string currencyUnit;
        private List<double> tariffValues;

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
                    byte[] buffbytes = binReader.ReadBytes(10);
                    this.currencyUnit = SysNode.GetStringFromBytes(buffbytes);
                    //固定读取6个费率值
                    for (int i = 0; i < 6; i++)
                        this.tariffValues.Add(binReader.ReadDouble());
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
