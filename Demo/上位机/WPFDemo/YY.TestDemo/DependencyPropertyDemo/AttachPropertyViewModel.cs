using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DependencyPropertyDemo
{
    public class AttachPropertyViewModel:ObservableObject
    {
        private Person person = new Person();
        public Person Person
        {
            get { return person; }
            set { person = value; OnPropertyChanged(); }
        }
    }
}
