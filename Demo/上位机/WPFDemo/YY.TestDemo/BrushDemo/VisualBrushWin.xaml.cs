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
    /// VisualBrushWin.xaml 的交互逻辑
    /// </summary>
    public partial class VisualBrushWin : Window
    {
        public VisualBrushWin()
        {
            InitializeComponent();
        }

        private void image_MouseMove(object sender, MouseEventArgs e)
        {
            var point = e.GetPosition(image);//鼠标当前坐标
            var length = ellipse.ActualWidth * 0.5;//放大镜半径
            var radius = length / 2;
            var viewboxRect = new Rect(point.X - radius, point.Y - radius, length, length);
            visualBrush.Viewbox = viewboxRect;
            ellipse.SetValue(Canvas.LeftProperty, point.X - ellipse.ActualWidth / 2);
            ellipse.SetValue(Canvas.TopProperty, point.Y - ellipse.ActualHeight / 2);
        }

        private void image_MouseLeave(object sender, MouseEventArgs e)
        {
            ellipse.Visibility = Visibility.Collapsed;

        }

        private void image_MouseEnter(object sender, MouseEventArgs e)
        {
            ellipse.Visibility = Visibility.Visible;
        }
    }
}
