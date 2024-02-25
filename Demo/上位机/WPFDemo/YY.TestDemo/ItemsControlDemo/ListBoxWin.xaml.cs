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
    /// ListBoxWin.xaml 的交互逻辑
    /// </summary>
    public partial class ListBoxWin : Window
    {
        public ListBoxWin()
        {
            InitializeComponent();
            //listPerson.Items.Add(new Person() { Name = "张三", Age = 20, Address = "山西省大同市" });
            //listPerson.Items.Add(new Person() { Name = "李四", Age = 20, Address = "陕西省西安市" });
            //listPerson.Items.Add(new Person() { Name = "王五", Age = 20, Address = "广东省深圳市" });
        }

        private void btnShow_Click(object sender, RoutedEventArgs e)
        {
            //单选
            //Person person = listPerson.SelectedItem as Person;
            //this.txtContent.Text += string.Format("{0},{1};", person.Name, this.listPerson.SelectedValue);
            //多选
            //var list = this.listPerson.SelectedItems;
            //if (list.Count == 0)
            //{
            //    MessageBox.Show("请至少选择一项内容");
            //    return;
            //}
            //this.txtContent.Text = string.Empty;
            //foreach (var item in list)
            //{
            //    Person person = item as Person;
            //    this.txtContent.Text += string.Format("{0},{1},{2};", person.Name, person.Age, person.Address);

            //}
        }

        private void btnShowAll_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                var selectedItem = lisBox.SelectedItem;
                var content = ((ContentControl)selectedItem).Content;
                txtContent.Text = $"selectedItem={selectedItem}\r\ncontent={content}";
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }
    }
    public class Person
    {
        public string Name { get; set; }
        public int Age { get; set; }
        public string Address { get; set; }
    }
}
