namespace EventDemo02
{
    public partial class FrmClient01 : Form
    {
        public FrmClient01()
        {
            InitializeComponent();
        }

        //��2�������¼�
        public event delegateSendMsg? SendMsgEvent;

        //������Ϣ
        private void btnSend_Click(object sender, EventArgs e)
        {
            //��3�������¼�
            if (SendMsgEvent != null)
            {
                SendMsgEvent("�ͻ��ˣ�2��������Ϣ��" + this.txtMsg.Text.Trim());
            }
        }
    }
}