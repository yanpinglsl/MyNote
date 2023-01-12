using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CSharp11Demo.example
{
    struct CheckFoo
    {
        public static CheckFoo operator +(CheckFoo left, CheckFoo right)
        {
            return new CheckFoo();
        }

        public static CheckFoo operator checked +(CheckFoo left, CheckFoo right)
        {
            return new CheckFoo();
        }
    }
}
