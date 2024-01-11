using System.Collections.Generic;

namespace OfficeReportInterface.DefaultReportInterface
{
 
    public class PointsNode
    {
        public string text;
        public List<PointsNode> children;
        /// <summary>
        /// 由于设备节点与测量节点（日报测点/定时记录）的类型不一致，用object类型来存储
        /// </summary>
        public object tag;
        public bool leaf;

        public int ImageIndex;
        public int SelectedImageIndex;

        public List<PointsNode> Nodes
        {
            get { return children; }
        }

        public object Tag
        {
            get { return tag; }
            set { tag = value; }
        }

        public PointsNode()
        {
            this.children = new List<PointsNode>();
            this.leaf = false;
        }
        public PointsNode(string text, List<PointsNode> children, object tag, bool leaf)
        {
            this.text = text;
            this.children = children;
            this.tag = tag;
            this.leaf = leaf;
        }

        public PointsNode(string text)
        {
            this.text = text;
            this.children = new  List<PointsNode>();
            this.tag = new object();
            this.leaf = false;
        }

        public void Remove()
        {
            
        }

    }
}
