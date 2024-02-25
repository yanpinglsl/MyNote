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
using System.Windows.Media.Media3D;
using System.Windows.Shapes;

namespace TemplateDemo
{
    /// <summary>
    /// LogicTreeWin.xaml 的交互逻辑
    /// </summary>
    public partial class LogicTreeWin : Window
    {
        public LogicTreeWin()
        {
            InitializeComponent();
        }

        private void btnShow_Click(object sender, RoutedEventArgs e)
        {
            TreeViewItem viewItem = new TreeViewItem();
            viewItem.Header = "逻辑树根";
            LogicTree(viewItem, this);
            treeView.Items.Add(viewItem);

        }
        private void LogicTree(TreeViewItem item, object element)
        {
            if (!(element is DependencyObject)) return;
            TreeViewItem viewItem = new TreeViewItem();
            viewItem.Header = element.GetType().Name;
            item.Items.Add(viewItem);
            var elements =  LogicalTreeHelper.GetChildren(element as DependencyObject);
            foreach ( var child in elements)
            {
                LogicTree(viewItem, child);
            }
        }
    }
}
