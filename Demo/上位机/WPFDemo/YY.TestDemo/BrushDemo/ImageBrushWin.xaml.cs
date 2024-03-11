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
    /// ImageBrushWin.xaml 的交互逻辑
    /// </summary>
    public partial class ImageBrushWin : Window
    {
        public ImageBrushWin()
        {
            InitializeComponent();
        }

        private void Grid_MouseWheel(object sender, MouseWheelEventArgs e)
        {
            double offset = e.Delta / 3600.0;
            ImageBrush imageBrush = grid.Background as ImageBrush;
            Rect rect = imageBrush.Viewport;
            if (rect.Width + offset <= 0 && rect.Height + offset <= 0)
                return;

            rect.Width += offset;
            rect.Height += offset;
            imageBrush.Viewport = rect;
        }
    }
}
