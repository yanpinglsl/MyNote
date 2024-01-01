using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace DelegateDemo03
{
    public partial class FrmOther02 : Form
    {
        public FrmOther02()
        {
            InitializeComponent();
        }
        /// <summary>
        /// 接收委托传递的信息
        /// </summary>
        /// <param name="counter"></param>
        public void Receiver(string counter)
        {
            this.lblCount.Text = counter;
        }
    }
}
