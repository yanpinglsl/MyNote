using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DependencyPropertyDemo
{
    public class Person : ObservableObject
    {
        private string username;
        public string UserName
        {
            get { return username; }
            set { username = value; OnPropertyChanged(); }
        }

        private string password;
        public string Password
        {
            get { return password; }
            set { password = value; OnPropertyChanged(); }
        }
    }
}
