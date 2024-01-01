namespace EventDemo
{
    //��1������ί��
    public delegate void delegateSendMsg(string msg);
    public partial class FrmMain : Form
    {
        //��2������һ���¼�
        public event delegateSendMsg SendMsgEvent;
        public FrmMain()
        {
            InitializeComponent();
            FrmClient01 objClient1 = new FrmClient01();
            FrmClient02 objClient2 = new FrmClient02();

            //��5�������¼�
            SendMsgEvent += new delegateSendMsg(objClient1.EventResponse);
            SendMsgEvent += new delegateSendMsg(objClient2.EventResponse);

            //SendMsgEvent = null;//�����ڲ�ʹ��ֱ�Ӹ�ֵ��

            objClient1.Show();
            objClient2.Show();
        }

        private void btnSend_Click(object sender, EventArgs e)
        {
            //��3�������¼�
            SendMsgEvent(this.txtMsg.Text.Trim());
        }
    }

}