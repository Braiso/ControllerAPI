using System;

namespace ControllerAPI
{
    internal static class Program
    {
        static int Main(string[] args)
        {
            try
            {
                new App().Run();
                return 0;
            }
            catch (Exception ex)
            {
                Console.WriteLine("ERROR: " + ex);
                return 1;
            }
        }
    }

}
