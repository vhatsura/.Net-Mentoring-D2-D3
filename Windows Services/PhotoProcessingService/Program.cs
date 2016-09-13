using Topshelf;

namespace PhotoProcessingService
{
    class Program
    {
        static void Main(string[] args)
        {
            HostFactory.New(x => x.UseNLog());

            HostFactory.Run(x =>
            {
                x.Service<PhotoProcessingService>();

                x.SetServiceName("Photo Processing Service");
                x.SetDisplayName("Photo Service");
                x.SetDescription("Photo Service");
                x.RunAsLocalSystem();
            });
        }
    }
}
