using System.Net.Security;

partial class Program
{
    static void Main(string[] args)
    {
        Welcome5640();
        wellcome5522();
        Console.ReadKey();
    }
    private static void Welcome5640()
    {
        Console.WriteLine("Enter your name");
        string userName = Console.ReadLine();
        Console.WriteLine("{0}, welcome to my first console", userName);
    }
    static partial void wellcome5522();

}