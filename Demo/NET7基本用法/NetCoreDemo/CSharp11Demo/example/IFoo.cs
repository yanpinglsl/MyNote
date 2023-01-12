using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CSharp11Demo.example
{
    interface IFoo
    {
        // 抽象静态方法
        abstract static int Foo1();

        // 虚静态方法
        virtual static int Foo2()
        {
            return 42;
        }
    }

    struct Bar : IFoo
    {
        /// <summary>
        /// 隐式实现接口方法
        /// </summary>
        /// <returns></returns>
        public static int Foo1()
        {
            return 7;
        }

        /// <summary>
        /// 显示实现接口中的静态方法
        /// </summary>
        /// <returns></returns>
        static int IFoo.Foo1()
        {
            return 7;
        }

    }


    /// <summary>
    /// 由于运算符也属于静态方法，因此从 C# 11 开始，也可以用接口来对运算符进行抽象了。
    /// </summary>
    /// <typeparam name="T"></typeparam>
    interface ICanAdd<T> where T : ICanAdd<T>
    {
        abstract static T operator +(T left, T right);
    }

    /// <summary>
    /// 这样我们就可以给自己的类型实现该接口了，例如实现一个二维的点 Point：
    /// </summary>
    /// <param name="X"></param>
    /// <param name="Y"></param>
    record struct Point(int X, int Y) : ICanAdd<Point>
    {
        // 隐式实现接口方法
        public static Point operator +(Point left, Point right)
        {
            return new Point(left.X + right.X, left.Y + right.Y);
        }


        // 显式实现接口方法
        static Point ICanAdd<Point>.operator +(Point left, Point right)
        {
            return new Point(left.X + right.X, left.Y + right.Y);
        }

        void CallFoo1<T>() where T : IFoo
        {
            T.Foo1();
        } 
    }





    interface IA
    {
        /// <summary>
        /// 静态虚方法
        /// </summary>
        /// <returns></returns>
        virtual static int Foo()
        {
            return 1;
        }
    }

    interface IB
    {
        /// <summary>
        ///  静态虚方法
        /// </summary>
        /// <returns></returns>
        virtual static int Foo()
        {
            return 2;
        }
    }

    interface IC : IA, IB
    { 
        static int IA.Foo()
        {
            return 3;
        }

        static int IB.Foo()
        {
            return 4;
        }
    }

    struct Bar1 : IC
    { 
        public static int Foo()
        {
            return 5;
        }
    }

    struct Bar2 : IC
    {
        static int IA.Foo()
        {
            return 6;
        }
    }

    struct Bar3 : IC
    {
        static int IB.Foo()
        {
            return 7;
        }
    }

    struct Bar4 : IA, IB
    {

    }







}
