using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace CommandDemo
{
    public class CommandViewModel: ObservableObject
    {
        //public RelayCommand OpenCommand { get; set; } = new RelayCommand(() =>
        //{
        //    MessageBox.Show("Hello,World!");
        //});
        public RelayCommand<object> OpenCommandParam { get; set; } = new RelayCommand<object>((param) =>
        {
            MessageBox.Show($"Hello,World!{param.ToString()}");
        });
    }
}
