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

namespace ItemsControlDemo
{
    /// <summary>
    /// DataGridWin.xaml 的交互逻辑
    /// </summary>
    public partial class DataGridWin : Window
    {
        public DataGridWin()
        {
            InitializeComponent();
            List<Student> students = new List<Student>()
            {
                new Student() { Name="张三",Age=25,Address="山西省大同市" },
                new Student() { Name="李四",Age=35,Address="陕西省西安市"},
                new Student() { Name="王五",Age=19,Address="广东省深圳市"}

            };
            dataGrid.ItemsSource = students;
        }

        private void dataGrid_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            DataGrid grid = sender as DataGrid;
            if (grid == null) return;

            var student = grid.SelectedItem as Student;
            if (student == null) return;
            txtName.Text = student.Name;
            txtAge.Text = student.Age.ToString();
            txtAddr.Text = student.Address;
        }
    }
}
