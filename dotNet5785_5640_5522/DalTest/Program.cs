namespace DalTest;

internal class Program
{
    private static IStudent? s_dalStudent = new StudentImplementation();
    static void Main(string[] args)
    {
        Console.WriteLine("Hello, World!");

    }
}