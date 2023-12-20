using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ExplicitAndImplicitDemo
{
    /// <summary>
    /// implicit 关键字用于声明隐式的用户自定义的类型转换运算符。 
    /// 如果可以确保转换过程不会造成数据丢失，则可使用该关键字在用户定义类型和其他类型之间进行隐式转换。
    /// 使用隐式转换操作符之后，在编译时会跳过异常检查，所以隐式转换运算符应当从不引发异常并且从不丢失信息，否则在运行时会出现一些意想不到的问题。
    /// </summary>
    public class Digit
    {
        public Digit(double d) { val = d; }
        public double val;
        // ...other members

        // User-defined conversion from Digit to double
        public static implicit operator double(Digit d)
        {
            return d.val;
        }
        //  User-defined conversion from double to Digit
        public static implicit operator Digit(double d)
        {
            return new Digit(d);
        }
    }
}
