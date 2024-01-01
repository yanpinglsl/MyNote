namespace EventDemo
{
    //【1】声明委托
    public delegate void delegateSendMsg(string msg);
    public partial class FrmMain : Form
    {
        //【2】定义一个事件
        public event delegateSendMsg SendMsgEvent;
        public FrmMain()
        {
            InitializeComponent();
            FrmClient01 objClient1 = new FrmClient01();
            FrmClient02 objClient2 = new FrmClient02();

            //【5】关联事件
            SendMsgEvent += new delegateSendMsg(objClient1.EventResponse);
            SendMsgEvent += new delegateSendMsg(objClient2.EventResponse);

            //SendMsgEvent = null;//允许内部使用直接赋值。

            objClient1.Show();
            objClient2.Show();
        }

        private void btnSend_Click(object sender, EventArgs e)
        {
            //【3】激发事件
            SendMsgEvent(this.txtMsg.Text.Trim());
        }
    }

}