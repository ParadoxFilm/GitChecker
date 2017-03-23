using System;

namespace GitChecker
{
    class Program
    {
        static void Main(string[] args)
        {
            try
            {
                Console.WriteLine(@"Press Enter to start...");
                Console.ReadKey();
                var checker = new Checker();
                checker.AddFromDump();
                Console.WriteLine($@"Sites count: {checker.Sites.Count}");
                checker.CheckStart();
                checker.SaveResult();
                Console.WriteLine(@"Done.");
            }
            catch (Exception ex)
            {
                Console.WriteLine($@"Error: {ex.Message}");
            }

            Console.WriteLine(@"Press any key for exit...");
            Console.ReadKey();
        }
    }
}
