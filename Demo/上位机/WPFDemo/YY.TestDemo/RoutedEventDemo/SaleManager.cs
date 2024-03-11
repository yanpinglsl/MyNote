using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace RoutedEventDemo
{
    public class SaleManager
    {
        public static readonly RoutedEvent CheckEvent = EventManager.RegisterRoutedEvent(
            name: "CheckEvent",
            routingStrategy: RoutingStrategy.Bubble,
           handlerType: typeof(RoutedEventHandler),
           ownerType: typeof(SaleManager));
        public static void AddCheckHandler(DependencyObject dependencyObject, RoutedEventHandler handler)
        {
            if (dependencyObject is UIElement uiElement)
            {
                uiElement.AddHandler(CheckEvent, handler);
            }
        }
        public static void RemoveCheckHandler(DependencyObject dependencyObject, RoutedEventHandler handler)
        {
            if (dependencyObject is UIElement uiElement)
            {
                uiElement.RemoveHandler(CheckEvent, handler);
            }
        }
    }
}
