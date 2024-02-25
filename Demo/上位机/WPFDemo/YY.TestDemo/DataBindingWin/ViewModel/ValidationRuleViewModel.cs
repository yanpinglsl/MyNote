using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataBindingWin.ViewModel
{
    public class ValidationRuleViewModel : ObservableObject
    {
        private Person person { get; set; }
        public Person Person
        {
            get { return person; }
            set
            {
                person = value;
                OnPropertyChanged();
            }
        }
        public ValidationRuleViewModel()
        {
            person = new Person
            {
                Name = "张三",
                Age = 50,
                Money = 5000,
                Address = "居无定所",
            };
        }
    }
}
