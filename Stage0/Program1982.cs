namespace Stage0
{
    partial class Program
    {
        static void Main(string[] args)
        {
            Welcome1982();
            Welcome5912(); 
            Console.ReadKey();
        }

        static partial void Welcome5912();

        private static void Welcome1982()
        {
            Console.Write("Enter your name: ");
            String name = Console.ReadLine();
            Console.WriteLine($"{name}, welcome to my first console application");
        }
    }
}