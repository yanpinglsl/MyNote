using System;
using System.Collections.Generic;
using System.Text;

namespace TextFile
{
    [Serializable]
    class Student
    {
        public string Name { get; set; }
        public string Gender { get; set; }
        public int Age { get; set; }
        public DateTime Birthday { get; set; }
    }
}
