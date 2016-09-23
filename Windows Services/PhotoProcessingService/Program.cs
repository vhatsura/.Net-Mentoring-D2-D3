using System;
using System.IO;
using Topshelf;

namespace PhotoProcessingService
{
    class Program
    {
        static void Main(string[] args)
        {
            var currentDirectory = AppDomain.CurrentDomain.BaseDirectory;
            var inDirectory = Path.Combine(currentDirectory, "input");
            var outDirectory = Path.Combine(currentDirectory, "output");
            var tempDirectory = Path.Combine(currentDirectory, "temp");

            HostFactory.Run(x =>
            {
                x.UseNLog();

                x.Service(() => new PhotoProcessingService(inDirectory, outDirectory, tempDirectory));

                x.SetServiceName("Photo Processing Service");
                x.SetDisplayName("Photo Service");
                x.SetDescription("Photo Service");
                x.StartAutomaticallyDelayed();
                x.RunAsLocalSystem();
            });
        }
    }
}
