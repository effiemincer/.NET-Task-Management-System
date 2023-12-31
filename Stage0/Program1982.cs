
namespace Stage0
{
    public partial class Program
    {
        static void Main(string[] args)
        {
            Welcome1982();
            Welcome5912(); 
            Console.ReadKey();
        }

        private static void Welcome1982()
        {
            Console.Write("Enter your name: ");
            String name = Console.ReadLine();
            Console.WriteLine($"{name}, welcome to my first console application");
        }
    }
}

