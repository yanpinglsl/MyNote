namespace EventDemo02
{
    //��1������ί��
    public delegate void delegateSendMsg(string msg);
    public partial class FrmMain : Form
    {
        public FrmMain()
        {
            InitializeComponent();
            FrmClient01 objClient1 = new FrmClient01();
            FrmClient02 objClient2 = new FrmClient02();

            //��5�������¼�
            objClient1.SendMsgEvent += new delegateSendMsg(ShowMsg_SendMsgEvent);
            objClient2.SendMsgEvent += new delegateSendMsg(ShowMsg_SendMsgEvent);

            //SendMsgEvent = null;//�����ڲ�ʹ��ֱ�Ӹ�ֵ��

            objClient1.Show();
            objClient2.Show();
        }
        //��4���¼���Ӧ����
        public void ShowMsg_SendMsgEvent(string msg)
        {
            this.txtMsg.Text = msg;
        }
    }


}