using static System.Net.Mime.MediaTypeNames;

namespace EventDriver
{
    public partial class FrmMain : Form
    {
        public FrmMain()
        {
            InitializeComponent();
            this.btnAndy.Click += btnTest_Click;
            this.btnCarry.Click += btnTest_Click;
            this.btnCoco.Click += btnTest_Click;
        }

        private void btnTest_Click(object sender, EventArgs e)
        {
            if (sender is Button)
            {
                string text = ((Button)sender).Text;
                MessageBox.Show("大家好！我是：" + text);
            }
        }

        private void btnTestEvent_Click(object sender, EventArgs e)
        {
            MessageBox.Show("大家好！");
        }

        private void btnLink_Click(object sender, EventArgs e)
        {
            this.btnTestEvent.Click += btnTestEvent_Click;
        }

        private void btnCut_Click(object sender, EventArgs e)
        {
            this.btnTestEvent.Click -= btnTestEvent_Click;
        }

        private void btnTestMsgbox_Click(object sender, EventArgs e)
        {

            MessageBox.Show("请输入学员姓名！");
            MessageBox.Show("请输入学员姓名！", "验证提示");
            MessageBox.Show("请输入学员姓名！", "验证提示", MessageBoxButtons.OKCancel);
            MessageBox.Show("请输入学员姓名！", "验证提示", MessageBoxButtons.OKCancel,
                MessageBoxIcon.Information);


            DialogResult result = MessageBox.Show("请输入学员姓名！", "验证提示",
                MessageBoxButtons.OKCancel,
                  MessageBoxIcon.Error);
            if (result == DialogResult.OK)//用户点击了确定按钮
            {
                MessageBox.Show("用户点击了确定按钮");
            }
            else
            {
                MessageBox.Show("用户点击了取消按钮");
            }
        }
    }
}