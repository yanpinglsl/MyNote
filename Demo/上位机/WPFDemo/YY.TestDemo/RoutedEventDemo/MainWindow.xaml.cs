using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace RoutedEventDemo
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
        }

        private void Window_PreviewMouseUp(object sender, MouseButtonEventArgs e)
        {
            Console.WriteLine($"Window对象的隧道事件PreviewMouseUp被触发");

        }

        private void Border_PreviewMouseUp(object sender, MouseButtonEventArgs e)
        {

            Console.WriteLine($"Border对象的隧道事件PreviewMouseUp被触发");
        }

        private void Canvas_PreviewMouseUp(object sender, MouseButtonEventArgs e)
        {

            Console.WriteLine($"Canvas对象的隧道事件PreviewMouseUp被触发");
        }

        private void Button_PreviewMouseUp_1(object sender, MouseButtonEventArgs e)
        {

            Console.WriteLine($"Button(取消)对象的隧道事件PreviewMouseUp被触发");
        }

        private void Button_PreviewMouseUp(object sender, MouseButtonEventArgs e)
        {
            Console.WriteLine($"Button(确定)对象的隧道事件PreviewMouseUp被触发");
        }

        private void Border_MouseUp(object sender, MouseButtonEventArgs e)
        {
            Console.WriteLine($"Border对象的冒泡事件MouseUp被触发");
        }

        private void Canvas_MouseUp(object sender, MouseButtonEventArgs e)
        {

            Console.WriteLine($"Canvas对象的冒泡事件MouseUp被触发");
        }

        private void Button_MouseUp(object sender, MouseButtonEventArgs e)
        {
            Console.WriteLine($"Button(确定)对象的冒泡事件MouseUp被触发");

        }

        private void Button_MouseUp_1(object sender, MouseButtonEventArgs e)
        {
            Console.WriteLine($"Button(取消)对象的冒泡事件MouseUp被触发");
        }

        private void Window_MouseUp(object sender, MouseButtonEventArgs e)
        {
            Console.WriteLine($"Window对象的冒泡事件MouseUp被触发");

        }
    }
}