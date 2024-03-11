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

namespace RoutedEventDemo
{
    /// <summary>
    /// Widget.xaml 的交互逻辑
    /// </summary>
    public partial class Widget : UserControl
    {
        public Widget()
        {
            InitializeComponent();
        }
        public string Icon
        {
            get { return (string)GetValue(IconProperty); }
            set { SetValue(IconProperty, value); }
        }

        public static readonly DependencyProperty IconProperty =
            DependencyProperty.Register("Icon", typeof(string), typeof(Widget), new PropertyMetadata("❤"));

        public double Value
        {
            get { return (double)GetValue(ValueProperty); }
            set { SetValue(ValueProperty, value); }
        }

        public static readonly DependencyProperty ValueProperty =
            DependencyProperty.Register("Value", typeof(double), typeof(Widget), 
                new PropertyMetadata(
                    0.0,
                    new PropertyChangedCallback(OnValueChangedCallback)));

        public string Title
        {
            get { return (string)GetValue(TitleProperty); }
            set { SetValue(TitleProperty, value); }
        }

        public static readonly DependencyProperty TitleProperty =
            DependencyProperty.Register("Title", typeof(string), typeof(Widget), new PropertyMetadata("请输入标题"));

        public double Target
        {
            get { return (double)GetValue(TargetProperty); }
            set { SetValue(TargetProperty, value); }
        }

        public static readonly DependencyProperty TargetProperty =
            DependencyProperty.Register("Target", typeof(double), typeof(Widget), new PropertyMetadata(0.0));


        /// <summary>
        /// 1、注册路由事件
        /// 2、通过event包装成普通事件的外观
        /// 3、触发路由事件
        /// </summary>
        public static readonly RoutedEvent CompletedEvent = EventManager.RegisterRoutedEvent(
            name: "CompleteEvent",
            routingStrategy: RoutingStrategy.Bubble,
            handlerType: typeof(RoutedEventHandler),
            ownerType: typeof(Widget));
        /// <summary>
        /// 通过event包装成普通事件的外观
        /// </summary>
        public event RoutedEventHandler Completed
        {
            add { AddHandler(CompletedEvent, value); }
            remove { RemoveHandler(CompletedEvent, value); }
        }
        /// <summary>
        /// 触发路由事件
        /// </summary>
        void RaiseRoutedEvent()
        {
            RoutedEventArgs routedEventArgs = new RoutedEventArgs(CompletedEvent, this);
            RaiseEvent(routedEventArgs);
        }

        public static void OnValueChangedCallback(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if(d is Widget control && e.NewValue is double value)
            {
                if (value >= control.Target && control.Target != 0)//当业绩大于100万美元时
                {
                    control.Icon = "☻";
                    control.RaiseRoutedEvent();//触发路由事件，完成销售目标
                }
                else
                {
                    control.Icon = "❤";
                }
            }
        }

    }
}
