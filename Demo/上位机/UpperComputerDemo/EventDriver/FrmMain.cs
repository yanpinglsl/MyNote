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
                MessageBox.Show("��Һã����ǣ�" + text);
            }
        }

        private void btnTestEvent_Click(object sender, EventArgs e)
        {
            MessageBox.Show("��Һã�");
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

            MessageBox.Show("������ѧԱ������");
            MessageBox.Show("������ѧԱ������", "��֤��ʾ");
            MessageBox.Show("������ѧԱ������", "��֤��ʾ", MessageBoxButtons.OKCancel);
            MessageBox.Show("������ѧԱ������", "��֤��ʾ", MessageBoxButtons.OKCancel,
                MessageBoxIcon.Information);


            DialogResult result = MessageBox.Show("������ѧԱ������", "��֤��ʾ",
                MessageBoxButtons.OKCancel,
                  MessageBoxIcon.Error);
            if (result == DialogResult.OK)//�û������ȷ����ť
            {
                MessageBox.Show("�û������ȷ����ť");
            }
            else
            {
                MessageBox.Show("�û������ȡ����ť");
            }
        }
    }
}