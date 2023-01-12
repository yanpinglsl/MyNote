using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CSharp11Demo.example
{
    struct PointRequiredMembers
    {
        public required int X;
        public required int Y;

        [SetsRequiredMembers]
        public PointRequiredMembers(int x, int y)
        {
            X = x;
            Y = y;
        }
    }
}
