namespace EventDemo02
{
    //【1】声明委托
    public delegate void delegateSendMsg(string msg);
    public partial class FrmMain : Form
    {
        public FrmMain()
        {
            InitializeComponent();
            FrmClient01 objClient1 = new FrmClient01();
            FrmClient02 objClient2 = new FrmClient02();

            //【5】关联事件
            objClient1.SendMsgEvent += new delegateSendMsg(ShowMsg_SendMsgEvent);
            objClient2.SendMsgEvent += new delegateSendMsg(ShowMsg_SendMsgEvent);

            //SendMsgEvent = null;//允许内部使用直接赋值。

            objClient1.Show();
            objClient2.Show();
        }
        //【4】事件响应方法
        public void ShowMsg_SendMsgEvent(string msg)
        {
            this.txtMsg.Text = msg;
        }
    }


}