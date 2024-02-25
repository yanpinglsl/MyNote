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
    /// MenuWin2.xaml 的交互逻辑
    /// </summary>
    public partial class MenuWin2 : Window
    {
        public List<MenuMode> MenuModes { get; set; }
        public MenuWin2()
        {
            InitializeComponent();
            CreateMenu();
            menu.ItemsSource = MenuModes;
        }
        public void CreateMenu()
        {
            MenuModes = new List<MenuMode>();
            for (int i = 0; i < 5; i++)
            {
                MenuMode parent = new MenuMode();
                parent.Name = $"一级菜单{i}";
                parent.Children = new List<MenuMode>();
                for (int j = 0; j < 5; j++)
                {
                    MenuMode child = new MenuMode();
                    child.Name = $"二级菜单{i}-{j}";
                   parent.Children.Add(child);
                }
                MenuModes.Add(parent);
            }
        }
    }
    public class MenuMode
    {
        public string Name { get; set; }
        public List<MenuMode> Children { get; set; }
    }
}
