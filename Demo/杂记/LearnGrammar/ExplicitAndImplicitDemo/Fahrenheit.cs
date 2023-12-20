using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ExplicitAndImplicitDemo
{
    public class Fahrenheit
    {
        public Fahrenheit(float temp)
        {
            Degrees = temp;
        }

        public float Degrees { get; }

        public static explicit operator Celsius(Fahrenheit fahr)
        {
            return new Celsius((5.0f / 9.0f) * (fahr.Degrees - 32));
        }
    }
}
