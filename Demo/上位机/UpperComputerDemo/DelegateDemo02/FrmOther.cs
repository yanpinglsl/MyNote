namespace DelegateDemo02
{
    public partial class FrmOther : Form
    {
        //����ί�д���ί�ж���
        public ShowCounter? msgSender;
        //����
        public int counter = 0;
        public FrmOther()
        {
            InitializeComponent();
        }
        private void btnClick_Click(object sender, EventArgs e)
        {
            counter++;
            if(msgSender != null)
            {
                msgSender(counter.ToString());
            }
        }
    }
}