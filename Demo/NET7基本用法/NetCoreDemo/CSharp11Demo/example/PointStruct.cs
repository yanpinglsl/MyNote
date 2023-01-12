using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CSharp11Demo.example
{
    struct PointStruct
    {
        public int X;
        public int Y;

        public PointStruct(int x)
        {
            X = x;
            // Y 自动初始化为 0
        }
    }
}
