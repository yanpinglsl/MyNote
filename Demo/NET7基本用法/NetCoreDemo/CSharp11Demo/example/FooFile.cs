using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CSharp11Demo.example
{
    file class FooFile
    {
    }

    file struct BarFile
    {
        // ...
    }

    public class InfoShow
    {
        public void Show()
        {
            //在同一个文件中访问，正常访问
            var foo = new FooFile(); // ok
            var bar = new BarFile(); // ok
        }
    }
}
