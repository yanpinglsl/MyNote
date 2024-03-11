using System.Configuration;

namespace DBHelperTest
{
    internal class Program
    {
        static void Main(string[] args)
        {
        string name=    ConfigurationManager.ConnectionStrings["ConnString"].ConnectionString;
            Console.WriteLine("Hello, World!");
        }
    }
}
