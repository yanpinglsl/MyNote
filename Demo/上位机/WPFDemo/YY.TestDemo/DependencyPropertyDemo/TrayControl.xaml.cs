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
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace DependencyPropertyDemo
{
    /// <summary>
    /// TrayControl.xaml 的交互逻辑
    /// </summary>
    public partial class TrayControl : UserControl
    {
        public TrayControl()
        {
            InitializeComponent();
        }


        public int Size
        {
            get { return (int)GetValue(SizeProperty); }
            set { SetValue(SizeProperty, value); }
        }

        public static readonly DependencyProperty SizeProperty =
            DependencyProperty.Register("Size", typeof(int), typeof(TrayControl),
                new PropertyMetadata(
                    60
                    , new PropertyChangedCallback(OnSizePropertyChangedCallback))
                );

        public static void OnSizePropertyChangedCallback(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            TrayControl control = d as TrayControl;
            control.Initlize();
        }
        private void Initlize()
        {
            SelectedCount = 0;
            SelectedItems.Clear();
            container.Children.Clear();
            if(Count > 0)
            {
                for(int i = 0; i < Count; i++)
                {
                    CheckBox checkbox = new CheckBox();
                    checkbox.Style = Application.Current.Resources["CheckBoxDishStyle"] as Style;
                    checkbox.Width = Size;
                    checkbox.Height = Size;
                    checkbox.Tag = new Point(i * 10, Size + i * 2);
                    checkbox.Name = "Dish_" + i.ToString();
                    checkbox.Checked += (sender, e) =>
                    {
                        SelectedCount++;
                        SelectedItems.Add(checkbox);
                    };
                    checkbox.Unchecked -= (sender, e) =>
                    {
                        SelectedCount--;
                        SelectedItems.Remove(checkbox);
                    };
                    container.Children.Add(checkbox);
                }
            }
        }
        public int Count
        {
            get { return (int)GetValue(CountProperty); }
            set { SetValue(CountProperty, value); }
        }

        public static readonly DependencyProperty CountProperty =
            DependencyProperty.Register("Count", typeof(int), typeof(TrayControl),
                new PropertyMetadata(
                    0
                    , new PropertyChangedCallback(OnCountPropertyChangedCallback)
                    , new CoerceValueCallback(OnCountCoerceValueCallback))
                );

        //这里演示当依赖属性值等于10，强制与10相乘，输出100
        public static void OnCountPropertyChangedCallback(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            TrayControl control = d as TrayControl;
            control.Initlize();
        }
        public static object OnCountCoerceValueCallback(DependencyObject d, object baseValue)
        {
            int count = (int)baseValue;
            if (count == 10)
            {
                return count * 10;

            }
            return baseValue;

        }

        public int SelectedCount
        {
            get { return (int)GetValue(SelectedCountProperty); }
            set { SetValue(SelectedCountProperty, value); }
        }

        public static readonly DependencyProperty SelectedCountProperty =
            DependencyProperty.Register("SelectCount", typeof(int), typeof(TrayControl), new PropertyMetadata(0));

        public List<CheckBox> SelectedItems
        {
            get { return (List<CheckBox>)GetValue(SelectedItemsProperty); }
            set { SetValue(SelectedItemsProperty, value); }
        }

        public static readonly DependencyProperty SelectedItemsProperty =
            DependencyProperty.Register("SelectedItems", typeof(List<CheckBox>), typeof(TrayControl), new PropertyMetadata(new List<CheckBox>()));
    }
}
