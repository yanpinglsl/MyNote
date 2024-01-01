namespace DelegateDemo02
{
    //【1】声明委托
    public delegate void ShowCounter(string counter);
    public partial class FrmMain : Form
    {
        public FrmMain()
        {
            InitializeComponent();
            FrmOther frmOther = new FrmOther();
            //将从窗体的委托变量和主窗体的对应方法关联
            frmOther.msgSender += Receiver;
            frmOther.Show();
        }

        /// <summary>
        /// 接收委托传递的信息
        /// </summary>
        /// <param name="counter"></param>
        private void Receiver(string counter)
        {
            this.lblCount.Text = counter;
        }
    }
}