using DataBindingWin.ViewModel;
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

namespace DataBindingWin
{
    /// <summary>
    /// NotifyPropertyWin.xaml 的交互逻辑
    /// </summary>
    public partial class NotifyPropertyWin : Window
    {
        public NotifyPropertyWin()
        {
            InitializeComponent();
            this.DataContext = new NotifyPropertyViewModel();
        }

        private void btnChange_Click(object sender, RoutedEventArgs e)
        {
            NotifyPropertyViewModel vm = this.DataContext as NotifyPropertyViewModel;
            if (vm == null) return;
            vm.Person.Age = new Random().Next(1, 100);
            vm.Person.Address = DateTime.Now.ToString();
        }
    }
}
