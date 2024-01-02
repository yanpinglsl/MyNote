using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;

//引入命名空间
using System.IO;

namespace TextFile
{
    public partial class FrmFile : Form
    {
        public FrmFile()
        {
            InitializeComponent();
        }
        //写入文件
        private void btnWriteAll_Click(object sender, EventArgs e)
        {
            //【1】创建文件流
            FileStream fs = new FileStream(Path.Combine(AppContext.BaseDirectory, "myfile.txt"), FileMode.Create);

            //【2】创建写入器
            StreamWriter sw = new StreamWriter(fs);

            //【3】以流的方式写入数据
            sw.Write(this.txtContent.Text.Trim());

            //【4】关闭写入器
            sw.Close();

            //【5】关闭文件流
            fs.Close();
        }
        //读取文件
        private void btnReadAll_Click(object sender, EventArgs e)
        {
            //【1】创建文件流
            FileStream fs = new FileStream(Path.Combine(AppContext.BaseDirectory, "myfile.txt"), FileMode.Open);

            //【2】创建读取器
            StreamReader sr = new StreamReader(fs);

            //【3】以流的方式读取数据
            this.txtContent.Text = sr.ReadToEnd();

            //【4】关闭读取器
            sr.Close();

            //【5】关闭文件流
            fs.Close();
        }
        //模拟写入系统日志
        private void btnWriteLine_Click(object sender, EventArgs e)
        {
            //【1】创建文件流（文件模式为：追加）
            FileStream fs = new FileStream(Path.Combine(AppContext.BaseDirectory, "myfile.txt"), FileMode.Append);

            //【2】创建写入器
            StreamWriter sw = new StreamWriter(fs);

            //【3】以流的方式“逐行”写入数据
            sw.WriteLine(DateTime.Now.ToString() + " 文件操作正常！");

            //【4】关闭写入器
            sw.Close();

            //【5】关闭文件流
            fs.Close();
        }
        //删除文件
        private void btnDel_Click(object sender, EventArgs e)
        {
            File.Delete(this.txtFrom.Text.Trim());
        }
        //复制文件
        private void btnCopy_Click(object sender, EventArgs e)
        {
            string fromPath = Path.Combine(AppContext.BaseDirectory, this.txtFrom.Text.Trim());
            string toPath = Path.Combine(AppContext.BaseDirectory, this.txtTo.Text.Trim());
            if (File.Exists(toPath)) //首先判断文件是否存在（如果文件存在，直接复制会出现错误）
            {
                File.Delete(toPath);//删除文件
            }
            File.Copy(fromPath, toPath); //复制文件
        }
        //移动文件
        private void btnRemove_Click(object sender, EventArgs e)
        {
            string fromPath = Path.Combine(AppContext.BaseDirectory, this.txtFrom.Text.Trim());
            string toPath = Path.Combine(AppContext.BaseDirectory, this.txtTo.Text.Trim());
            //首先判断目标路径文件是否存在（如果文件存在，直接复制会出现错误）
            if (File.Exists(toPath))
            {
                File.Delete(toPath);//删除文件
            }
            if (File.Exists(fromPath))//如果当前文件存在则移动
            {
                //移动文件
                File.Move(fromPath, toPath);
            }
            else
            {
                MessageBox.Show("文件不存在！");
            }
        }
        //获取当前目录下的文件
        private void btnShowAllFiles_Click(object sender, EventArgs e)
        {
            string[] files = Directory.GetFiles(@"../../../");
            this.txtContent.Clear();
            foreach (string item in files)
            {
                this.txtContent.Text += item + "\r\n";
            }
        }
        //获取指定目录下的所有子目录
        private void btnShowSubDir_Click(object sender, EventArgs e)
        {
            string[] dirs = Directory.GetDirectories(@"../../../");
            this.txtContent.Clear();
            foreach (string item in dirs)
            {
                this.txtContent.Text += item + "\r\n";
            }
        }

        //创建目录
        private void btnCreate_Click(object sender, EventArgs e)
        {
            Directory.CreateDirectory(@"../Test");
        }

        //删除指定目录下的所有子目录和文件
        private void btnDelAllFiles_Click(object sender, EventArgs e)
        {
            // Directory.Delete(Path.Combine(AppContext.BaseDirectory,@"C:\Myfiles"));//要求目录必须为空

            //使用DirectoryInfo对象，可以删除不为空的目录
            DirectoryInfo dir = new DirectoryInfo(@"../Test");
            dir.Delete(true);
        }
    }
}
