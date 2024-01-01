namespace EventDemo02
{
    public partial class FrmClient01 : Form
    {
        public FrmClient01()
        {
            InitializeComponent();
        }

        //【2】定义事件
        public event delegateSendMsg? SendMsgEvent;

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