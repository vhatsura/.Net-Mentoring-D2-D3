using System;
using System.IO;
using Topshelf;

namespace Server
{
    class Program
    {
        static void Main(string[] args)
        {
            var currentDirectory = AppDomain.CurrentDomain.BaseDirectory;
            var outputDirectory = Path.Combine(currentDirectory, "output");

            HostFactory.Run(
                hostConf =>
                {
                    hostConf.Service<PhotoServer>(
                        s =>
                        {
                            s.ConstructUsing(() => new PhotoServer(outputDirectory));
                            s.WhenStarted(serv => serv.Start());
                            s.WhenStopped(serv => serv.Stop());
                        });
                    hostConf.SetServiceName("PhotoServer");
                    hostConf.SetDisplayName("Photo Server");
                    hostConf.StartAutomaticallyDelayed();
                    hostConf.RunAsLocalService();
                });
        }
    }
}
