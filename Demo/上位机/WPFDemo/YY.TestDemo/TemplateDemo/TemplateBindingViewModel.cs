using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TemplateDemo
{
    public class TemplateBindingViewModel : ObservableObject
    {
       private Person person { get; set; }
        public Person Person { 
            get { return person; } 
            set
            { 
                person = value;
                OnPropertyChanged();
            }
        }
        public TemplateBindingViewModel()
        {
            person = new Person()
            {
                Name = "Michael Jackson",
                Occupation = "Musicians",
                Age = 25,
                Money = 9999999,
                Address = "深圳市光明区智慧招商城B4栋5楼"
            };
        }
    }
}
