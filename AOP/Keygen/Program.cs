using System;
using Castle.DynamicProxy;
using Logger;

namespace Keygen
{
    public class Program
    {
        private static void Main(string[] args)
        {
            var generator = new ProxyGenerator();
            var keygen = generator.CreateInterfaceProxyWithTarget<IKeygen>(new Keygen(), new LoggerInterceptor());
            string key;
            if (args.Length != 0)
            {
                DateTime date;
              
                key = keygen.GenerateKey(DateTime.TryParse(args[0], out date) ? date : DateTime.Now);
                Console.WriteLine($"Key: {key}");
            }

            key = keygen.GenerateKey(DateTime.Now);
            Console.WriteLine($"Key: {key}");
        }
    }
}
