﻿using System;
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
    /// ListItemContainerStyleWin.xaml 的交互逻辑
    /// </summary>
    public partial class ListItemContainerStyleWin : Window
    {
        public ListItemContainerStyleWin()
        {
            InitializeComponent();
            this.DataContext = new DataTemplateViewModel();
        }
    }
}
