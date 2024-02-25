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

namespace TemplateDemo
{
    /// <summary>
    /// VisualTreeWin.xaml 的交互逻辑
    /// </summary>
    public partial class VisualTreeWin : Window
    {
        public VisualTreeWin()
        {
            InitializeComponent();
        }

        private void btnShow_Click(object sender, RoutedEventArgs e)
        {
            TreeViewItem viewItem = new TreeViewItem();
            viewItem.Header = "逻辑树根";
            VisualTree(viewItem, this);
            treeView.Items.Add(viewItem);

        }
        private void VisualTree(TreeViewItem item, object element)
        {
            if (!(element is DependencyObject)) return;
            TreeViewItem viewItem = new TreeViewItem();
            viewItem.Header = element.GetType().Name;
            item.Items.Add(viewItem);
            for(int i =0;i< VisualTreeHelper.GetChildrenCount(element as DependencyObject);i++)
            {
                VisualTree(viewItem,VisualTreeHelper.GetChild(element as DependencyObject,i));
            }
        }
    }
}
