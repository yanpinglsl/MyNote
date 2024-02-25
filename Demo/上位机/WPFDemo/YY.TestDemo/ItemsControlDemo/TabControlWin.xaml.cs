using System.Windows;
using System.Windows.Controls;

namespace ItemsControlDemo
{
    /// <summary>
    /// TabControlWin.xaml 的交互逻辑
    /// </summary>
    public partial class TabControlWin : Window
    {
        public TabControlWin()
        {
            InitializeComponent();
        }

        private void tabControl_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            var tab=sender as TabControl;
            if (tab != null) 
            { 
               var item =  tab.SelectedItem as TabItem;
                var content = tab?.SelectedContent;
                txtShow.Text = "标题:" + item.Header.ToString() + " 内容:" + content;
            }
        }
    }
}
