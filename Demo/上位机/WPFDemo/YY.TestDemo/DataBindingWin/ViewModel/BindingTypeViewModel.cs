﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataBindingWin
{
    public class BindingTypeViewModel : ObservableObject
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
        public BindingTypeViewModel()
        {
            person = new Person
            {
                Name = "张三",
                Age = 50,
                Address = "居无定所",
            };
        }
    }
}
