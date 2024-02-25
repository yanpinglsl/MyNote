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
    /// BindingStyleWin.xaml 的交互逻辑
    /// </summary>
    public partial class BindingTypeWin : Window
    {
        public Person Worker {  get; set; }
        public BindingTypeWin()
        {
            InitializeComponent();
            Worker = new Person() { Name = "张三", Age = 15, Address = "陕西省西安市" };
            this.DataContext= new BindingTypeViewModel();   
        }
    }
}
