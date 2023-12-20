namespace ExplicitAndImplicitDemo
{
    internal class Program
    {
        static void Main(string[] args)
        {
            #region implicit 
            //Digit dig = new Digit(7);
            ////This call invokes the implicit "double" operator
            //double num = dig;
            ////This call invokes the implicit "Digit" operator
            //Digit dig2 = 12;
            //Console.WriteLine("num = {0} dig2 = {1}", num, dig2.val);
            #endregion

            #region explicit

            #region ①
            //Fahrenheit fahr = new Fahrenheit(100.0f);
            //Console.WriteLine($"{fahr.Degrees} Fahrenheit");
            //Celsius c = (Celsius)fahr;

            //Console.WriteLine($" = {c.Degrees} Celsius");
            //Fahrenheit fahr2 = (Fahrenheit)c;
            //Console.WriteLine($" = {fahr2.Degrees} Fahrenheit");
            #endregion

            #region ②
            try
            {
                byte b = 3;
                DigitByte d = (DigitByte)b; // 显示转换
            }
            catch (Exception e)
            {
                Console.WriteLine("{0} 捕获到异常.", e);
            }
            #endregion


            #endregion
            Console.ReadLine();
        }
    }
}
