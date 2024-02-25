using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StyleResourceDemo
{
    public class DataTriggerViewModel : ObservableObject
    {
        public ObservableCollection<Person> Persons { get; set; } = new ObservableCollection<Person>();
        public DataTriggerViewModel()
        {
            Persons = new ObservableCollection<Person>()
            {
                new Person(){Name = "张三1",Age = 19,Money = 5000,Address = "居无定所" },
                new Person(){Name = "张三2",Age = 25,Money = 5000,Address = "居无定所" },
                new Person(){Name = "张三3",Age = 20,Money = 5000,Address = "居无定所" },
                new Person(){Name = "张三4",Age = 19,Money = 5000,Address = "居无定所" },
                new Person(){Name = "张三5",Age = 80,Money = 5000,Address = "居无定所" },
                new Person(){Name = "张三6",Age = 21,Money = 5000,Address = "居无定所" }
            };
        }
    }

    public class Person : ObservableObject
    {
        private string name;
        public string Name
        {
            get { return name; }
            set { name = value; OnPropertyChanged(); }
        }

        private int age;
        public int Age
        {
            get { return age; }
            set { age = value; OnPropertyChanged(); }
        }

        private double money;
        public double Money
        {
            get { return money; }
            set { money = value; OnPropertyChanged(); }
        }

        private string address;
        public string Address
        {
            get { return address; }
            set { address = value; OnPropertyChanged(); }
        }
    }
}
