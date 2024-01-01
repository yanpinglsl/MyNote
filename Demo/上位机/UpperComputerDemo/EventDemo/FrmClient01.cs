using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace EventDemo
{
    public partial class FrmClient01 : Form
    {
        public FrmClient01()
        {
            InitializeComponent();
        }
        //【4】定义事件响应方法
        public void EventResponse(string msg)
        {
            this.txtMsg.Text = msg;
        }
    }
}
