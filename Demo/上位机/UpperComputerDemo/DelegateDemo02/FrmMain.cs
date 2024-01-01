namespace DelegateDemo02
{
    //��1������ί��
    public delegate void ShowCounter(string counter);
    public partial class FrmMain : Form
    {
        public FrmMain()
        {
            InitializeComponent();
            FrmOther frmOther = new FrmOther();
            //���Ӵ����ί�б�����������Ķ�Ӧ��������
            frmOther.msgSender += Receiver;
            frmOther.Show();
        }

        /// <summary>
        /// ����ί�д��ݵ���Ϣ
        /// </summary>
        /// <param name="counter"></param>
        private void Receiver(string counter)
        {
            this.lblCount.Text = counter;
        }
    }
}