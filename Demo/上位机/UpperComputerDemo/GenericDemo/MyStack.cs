using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace GenericDemo
{
    /*
     * 泛型好处：增加类型安全，带来编码的方便
     * 常见的泛型：泛型类和泛型方法
     * 后续深入：泛型委托（自定义泛型委托、常见的泛型委托Func、Action）
     * 
     * 泛型类的规范：public class 类名<T>{类的成员...}
     * T：仅仅是一个占位符，只要符合C#的命名规范即可使用，但一般都是用T。
     * T：表示一个通用的数据类型，在使用的时候用实际类型代替。
     * T：泛型类可以在定义中可以包含多个任意类型的参数，参数之间用多个逗号分隔开。
     *       例如：class MyGenericClass<T1,T2,T3>{...}
     *       各种类型参数可以用作成员变量的类型、属性或方法等成员的返回类型已经方法的参数类型等。
     */
    /// <summary>
    /// 编写一个入栈和出栈操作的通用类
    /// </summary>
    /// <typeparam name="T">可以是任意类型</typeparam>
    public class MyStack<T>
    {
        private T[] stack;
        private int stacPoint;//当前位置指针
        private int size;//栈数据容量

        public MyStack(int size)
        {
            this.size = size;
            this.stack = new T[size];
            this.stacPoint = -1;
        }
        /// <summary>
        /// 入栈方法
        /// </summary>
        /// <param name="item"></param>
        public void Push(T item)
        {
            if (stacPoint >= size)
                Console.WriteLine("栈空间已满！");
            else
            {
                stacPoint++;
                this.stack[stacPoint] = item;
            }
        }
        /// <summary>
        /// 出栈方法
        /// </summary>
        /// <returns></returns>
        public T Pop()
        {
            T data = this.stack[stacPoint];
            stacPoint--;
            return data;
        }

    }
}
