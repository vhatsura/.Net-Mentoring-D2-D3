using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Topshelf;

namespace PhotoProcessingService
{
    class Program
    {
        static void Main(string[] args)
        {
            var currentDir = AppDomain.CurrentDomain.BaseDirectory;
            var inDir = Path.Combine(currentDir, "in");
            var tempDir = Path.Combine(currentDir, "temp");

            HostFactory.Run(
                hostConf =>
                {
                    hostConf.Service<PhotoProcessingService>(
                        s =>
                        {
                            s.ConstructUsing(() => new PhotoProcessingService(inDir, tempDir));
                            s.WhenStarted(serv => serv.Start());
                            s.WhenStopped(serv => serv.Stop());
                        });
                    hostConf.SetServiceName("PhotoProcessingService");
                    hostConf.SetDisplayName("Photo Processing Service");
                    hostConf.StartAutomaticallyDelayed();
                    hostConf.RunAsLocalService();
                });
        }
    }
}
