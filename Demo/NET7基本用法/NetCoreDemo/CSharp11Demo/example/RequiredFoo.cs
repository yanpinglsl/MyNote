using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CSharp11Demo.example
{
    struct RequiredFoo
    {
        public required int X;
    }

    class RequiredFooClass
    { 
        public RequiredFooClass(int x)
        { 
            X= x;
        }


        public required int X;
    }
}
