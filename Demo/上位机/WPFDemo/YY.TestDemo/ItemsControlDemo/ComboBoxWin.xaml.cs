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
    /// ComboBoxWin.xaml 的交互逻辑
    /// </summary>
    public partial class ComboBoxWin : Window
    {
        public ComboBoxWin()
        {
            InitializeComponent();
            List<Student> students = new List<Student>()
            {
                new Student() { Name="张三",Age=25,Address="山西省大同市" },
                new Student() { Name="李四",Age=35,Address="陕西省西安市"},
                new Student() { Name="王五",Age=19,Address="广东省深圳市"}

            };
            cbxStu.ItemsSource = students;
        }

        private void cbxStu_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            ComboBox cbx = sender as ComboBox;
            if (cbx == null) return;

            var student = cbx.SelectedItem as Student;
            if (student == null) return;
            txtName.Text = student.Name;
            txtAge.Text = student.Age.ToString();
            txtAddr.Text = student.Address;
        }

        private void cbxTel_TextChanged(object sender, TextChangedEventArgs e)
        {
            txtTel.Text = cbxTel.Text;
        }
    }
}
