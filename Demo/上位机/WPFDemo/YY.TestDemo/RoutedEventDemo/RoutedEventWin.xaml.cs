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

namespace RoutedEventDemo
{
    /// <summary>
    /// RoutedEventWin.xaml 的交互逻辑
    /// </summary>
    public partial class RoutedEventWin : Window
    {
        public RoutedEventWin()
        {
            InitializeComponent();
        }

        private void Widget_Completed(object sender, RoutedEventArgs e)
        {
            Widget widget = sender as Widget;
            listBox.Items.Insert(0, $"完成目标销售额：{widget.Value}");
            widget.RaiseEvent(new RoutedEventArgs(SaleManager.CheckEvent));
        }

        private void Widget_Check(object sender, RoutedEventArgs e)
        {
            Widget widget = sender as Widget;
            if (((int)widget.Value) % 50000 < 5000)
            {
                listBox.Items.Insert(0, $"当前业绩：{widget.Value}，每累计50万发奖金啦：{widget.Value * 0.5}");
            }
        }
    }
}
