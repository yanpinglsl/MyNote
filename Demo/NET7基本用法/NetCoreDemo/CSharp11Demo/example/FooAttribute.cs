using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;

namespace CSharp11Demo.example
{
    [AttributeUsage(AttributeTargets.Method, AllowMultiple = true)]
    class FooAttribute<T> : Attribute where T : INumber<T>
    {
        public T Value { get; }
        public FooAttribute(T v)
        {
            Value = v;
        }
    }


    
}
