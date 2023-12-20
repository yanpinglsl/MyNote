using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ExplicitAndImplicitDemo
{
    /// <summary>
    /// 下面的示例定义结构 Digit，它表示单个的十进制数字。 
    /// 将运算符定义为从 byte 到 Digit 的转换，但由于并非所有字节都可转换为 Digit，
    /// 因此该转换应该应用显式转换。
    /// </summary>
    public class DigitByte
    {
        byte value;
        public DigitByte(byte value)
        {
            if (value > 9)
            {
                throw new ArgumentException();
            }
            this.value = value;
        }

        // 定义从byte到Digit的显示转换 explicit operator:
        public static explicit operator DigitByte(byte b)
        {
            DigitByte d = new DigitByte(b);
            Console.WriteLine("转换已完成");
            return d;
        }
    }
}
