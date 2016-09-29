using System;

namespace Keygen
{
    public class Program
    {
        private static void Main(string[] args)
        {
            if (args.Length != 0)
            {
                DateTime date;
                GenerateKey(DateTime.TryParse(args[0], out date) ? date : DateTime.Now);
            }

            GenerateKey(DateTime.Now);
        }

        private static void GenerateKey(DateTime date)
        {
            var key = new Keygen().GenerateKey(date);

            Console.WriteLine($"Key: {key}");
        }
    }
}
