namespace Test
{
    public record class Student(string FirstName,string LastName); 
    internal class Program
    {
        static void Main(string[] args)
        {
            Student s1 = new Student("si", "li");
            Student s2 = new Student("san", "zhang");
            Student s11 = new Student("si", "li");
            Student s22 = s1 with { FirstName = "Wang" };
            Console.WriteLine(s1 == s11);
            Console.WriteLine(s1 == s2);
            Console.WriteLine(s2 == s22);
            Console.ReadLine();
        }
    }
}