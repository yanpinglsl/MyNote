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
    /// RadialGradientBrushWin.xaml 的交互逻辑
    /// </summary>
    public partial class RadialGradientBrushWin : Window
    {
        public RadialGradientBrushWin()
        {
            InitializeComponent();
        }

        private void Ellipse_MouseMove(object sender, MouseEventArgs e)
        {
            Ellipse ellipse = e.Source as Ellipse;
            Point point = e.GetPosition(ellipse);
            double width = ellipse.Width;
            double height = ellipse.Height;
            RadialGradientBrush brush = ellipse.Fill as RadialGradientBrush;
            double x = point.X / width;
            double y = point.Y / height;
            brush.GradientOrigin = new Point(x, y);

        }
    }
}
