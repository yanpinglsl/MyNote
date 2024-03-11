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

namespace BrushDemo
{
    /// <summary>
    /// SolidColorBrushWin.xaml 的交互逻辑
    /// </summary>
    public partial class SolidColorBrushWin : Window
    {
        public SolidColorBrushWin()
        {
            InitializeComponent();
            SolidColorBrush solidColorBrush = new SolidColorBrush();
            //从Color的FromRgb成员中得到颜色
            solidColorBrush.Color = Color.FromRgb(0, 0x80, 0);
            solidColorBrush.Color = Colors.Pink;
            gridPink.Background = solidColorBrush;

        }
    }
}
