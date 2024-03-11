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

namespace DependencyPropertyDemo
{
    /// <summary>
    /// PropertyChangedCallbackWin.xaml 的交互逻辑
    /// </summary>
    public partial class PropertyChangedCallbackWin : Window
    {
        public PropertyChangedCallbackWin()
        {
            InitializeComponent();
        }

        private void btnSubmit_Click(object sender, RoutedEventArgs e)
        {
            foreach (var item in trayCtrl.SelectedItems)
            {
                MessageBox.Show($"{item.Name.ToString()} 移动坐标 = ({item.Tag.ToString()})");
            }
        }
    }
}
