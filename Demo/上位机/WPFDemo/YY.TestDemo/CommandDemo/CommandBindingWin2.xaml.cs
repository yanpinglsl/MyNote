using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace CommandDemo
{
    /// <summary>
    /// CommandBindingWin2.xaml 的交互逻辑
    /// </summary>
    public partial class CommandBindingWin2 : Window
    {
        public CommandBindingWin2()
        {
            InitializeComponent();
        }
        public RelayCommand<TextBox> OpenCommandParam { get; set; } = new RelayCommand<TextBox>((textbox) =>
        {
            textbox.Text += DateTime.Now + " 您单击了TextBox" + "\r";
        });
    }
}
