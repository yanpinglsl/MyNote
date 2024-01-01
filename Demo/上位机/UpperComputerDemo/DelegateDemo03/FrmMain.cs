namespace DelegateDemo03
{
    //【1】声明委托
    public delegate void ShowCount(string msg);
    public partial class FrmMain : Form
    {
        //根据委托创建委托对象
        public ShowCount msgSender;
        public int Count = 0;
        public FrmMain()
        {
            InitializeComponent();

            FrmOther01 objFrm01 = new FrmOther01();
            FrmOther02 objFrm02 = new FrmOther02();

            //将委托变量和具体方法关联
            this.msgSender += objFrm01.Receiver;
            this.msgSender += objFrm02.Receiver;

            objFrm01.Show();
            objFrm02.Show();
        }

        private void btnCount_Click(object sender, EventArgs e)
           
        {
            Count++;
            msgSender(Count.ToString());

        }

        private void btnReset_Click(object sender, EventArgs e)
        {
            Count = 0;
            msgSender(Count.ToString());
        }
    }
}