namespace DelegateDemo02
{
    public partial class FrmOther : Form
    {
        //根据委托创建委托对象
        public ShowCounter? msgSender;
        //计数
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