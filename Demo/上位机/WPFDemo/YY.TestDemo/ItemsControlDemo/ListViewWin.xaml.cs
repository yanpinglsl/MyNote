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
    /// ListViewWin.xaml 的交互逻辑
    /// </summary>
    public partial class ListViewWin : Window
    {
        public ListViewWin()
        {
            InitializeComponent();
            List<Student> students = new List<Student>()
            {
                new Student() { Name="张三",Age=25,Address="山西省大同市" },
                new Student() { Name="李四",Age=35,Address="陕西省西安市"},
                new Student() { Name="王五",Age=19,Address="广东省深圳市"}

            };
            listView.ItemsSource = students;
        }

        private void listView_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            ListView listView = sender as ListView;
            if (listView == null) return;

            var student = listView.SelectedItem as Student;
            if (student == null) return;
            txtName.Text = student.Name;
            txtAge.Text = student.Age.ToString();
            txtAddr.Text = student.Address;
        }
    }
    public class Student
    {
        public string Name { get; set; }
        public int Age { get; set; }
        public string Address { get; set; }
    }
}
