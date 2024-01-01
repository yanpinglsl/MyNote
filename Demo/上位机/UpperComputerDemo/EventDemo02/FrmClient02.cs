using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace EventDemo02
{
    public partial class FrmClient02 : Form
    {
        public FrmClient02()
        {
            InitializeComponent();
        }

        //【2】定义事件
        public event delegateSendMsg? SendMsgEvent ;

        //发送消息
        private void btnSend_Click(object sender, EventArgs e)
        {
            //【3】激发事件
            if (SendMsgEvent != null)
            {
                SendMsgEvent("客户端（2）发送消息：" + this.txtMsg.Text.Trim());
            }
        }
    }
}
