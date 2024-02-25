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
    /// ObservableCollectionWin.xaml 的交互逻辑
    /// </summary>
    public partial class ObservableCollectionWin : Window
    {
        public ObservableCollectionWin()
        {
            InitializeComponent();
            this.DataContext = new ObservableCollectionViewModel();
        }

        private void btnChange_Click(object sender, RoutedEventArgs e)
        {
            ObservableCollectionViewModel vm = this.DataContext as ObservableCollectionViewModel;
            if (vm == null) return;
            Person person = new Person();
            person.Name ="张三" + new Random().Next(1, 1000);
            person.Age = new Random().Next(1, 100);
            person.Money = new Random().Next(5000, 10000000);
            person.Address = DateTime.Now.ToString();
            vm.Persons.Add(person);
        }
    }
}
