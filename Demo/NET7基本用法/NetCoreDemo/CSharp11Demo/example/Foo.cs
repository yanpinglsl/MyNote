using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace CSharp11Demo.example
{
    ref struct Foo
    {
        public ref int X;

        public bool IsNull => Unsafe.IsNullRef(ref X);

        public Foo(ref int x)
        {
            X = ref x;
        }
    }

    ref struct FooNew
    { 
        public FooNew(scoped ref int x)
        {
            
        }
    }

    ref struct FooNew2
    {
        public ref int X;
        public FooNew2(scoped ref int x)
        {
            x = 30;
            //X = ref x; // error
        }
    }





}
