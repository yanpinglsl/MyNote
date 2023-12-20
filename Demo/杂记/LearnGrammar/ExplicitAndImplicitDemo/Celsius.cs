using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ExplicitAndImplicitDemo
{
    public class Celsius
    {
        public Celsius(float temp)
        {
            Degrees = temp;
        }

        public float Degrees { get; }

        public static explicit operator Fahrenheit(Celsius c)
        {
            return new Fahrenheit((9.0f / 5.0f) * c.Degrees + 32);
        }
    }
}
