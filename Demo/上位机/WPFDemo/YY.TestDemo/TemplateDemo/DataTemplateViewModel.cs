using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TemplateDemo
{
    class DataTemplateViewModel : ObservableObject
    {
        private List<Person> persons { get; set; }
        public List<Person> Persons
        {
            get { return persons; }
            set
            {
                persons = value;
                OnPropertyChanged();
            }
        }
        public DataTemplateViewModel()
        {
            var person = new Person()
            {
                Name = "Michael Jackson",
                Occupation = "Musicians",
                Age = 25,
                Money = 9999999,
                Address = "深圳市光明区智慧招商城B4栋5楼"
            };
            var bill = new Person()
            {
                Name = "比尔·盖茨（Bill Gates）",
                Occupation = "微软公司创始人",
                Age = 61,
                Money = 9999999,
                Address = "美国华盛顿州西雅图"
            };

            var musk = new Person()
            {
                Name = "Elon Reeve Musk",
                Occupation = "首席执行官",
                Age = 50,
                Money = 365214580,
                Address = "出生于南非的行政首都比勒陀利亚"
            };

            var jeff = new Person()
            {
                Name = "杰夫·贝索斯（Jeff Bezos）",
                Occupation = "董事会执行主席",
                Age = 25,
                Money = 85745845,
                Address = "杰夫·贝索斯出生于美国新墨西哥州阿尔布奎克。"
            };
            persons = new List<Person>();
            persons.Add(person);
            persons.Add(bill);
            persons.Add(musk);
            persons.Add(jeff);
        }
    }
}
