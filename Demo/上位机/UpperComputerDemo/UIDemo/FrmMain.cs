using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace UIDemo
{
    public partial class FrmMain : Form
    {
        public FrmMain()
        {
            InitializeComponent();
        }

        private void btnClose_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void btnMoodsInfo_Click(object sender, EventArgs e)
        {
            ////【1】关闭嵌入的其他窗体
            //foreach (Control item in this.splitContainer.Panel2.Controls)
            //{
            //    if (item is Form)
            //        ((Form)item).Close();
            //}
            ////【2】打开新的窗体
            //FrmAddProduct newFrm = new FrmAddProduct();
            //newFrm.TopLevel = false;//将子窗体设置成非顶级控件
            //                        //newFrm.FormBorderStyle = FormBorderStyle.None;//去掉子窗体的边框
            //newFrm.Parent = this.splitContainer.Panel2;//指定子窗体要嵌入的容器
            //newFrm.Dock = DockStyle.Fill;//这句话保证子窗体会随着容器大小而变化
            //newFrm.Show();

            OpenNewForm(new FrmAddProduct());

        }

        private void OpenNewForm(Form newFrm)
        {
            //【1】关闭嵌入的其他窗体
            foreach (Control item in this.splitContainer.Panel2.Controls)
            {
                if (item is Form)
                    ((Form)item).Close();
            }
            //【2】
            newFrm.TopLevel = false;//将子窗体设置成非顶级控件
                                    //newFrm.FormBorderStyle = FormBorderStyle.None;//去掉子窗体的边框
            newFrm.Parent = this.splitContainer.Panel2;//指定子窗体要嵌入的容器
            newFrm.Dock = DockStyle.Fill;//这句话保证子窗体会随着容器大小而变化
            newFrm.Show();
        }

        #region  窗体拖动

        private Point mouseOff;//鼠标移动位置变量
        private bool leftFlag;//标签是否为左键
        private void Frm_MouseDown(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Left)
            {
                mouseOff = new Point(-e.X, -e.Y); //得到变量的值
                leftFlag = true;                  //点击左键按下时标注为true;
            }
        }
        private void Frm_MouseMove(object sender, MouseEventArgs e)
        {
            if (leftFlag)
            {
                Point mouseSet = Control.MousePosition;
                mouseSet.Offset(mouseOff.X, mouseOff.Y);  //设置移动后的位置
                Location = mouseSet;
            }
        }
        private void Frm_MouseUp(object sender, MouseEventArgs e)
        {
            if (leftFlag)
            {
                leftFlag = false;//释放鼠标后标注为false;
            }
        }

        #endregion
    }
}
