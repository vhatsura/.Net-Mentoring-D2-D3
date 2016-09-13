using Topshelf;
using Topshelf.Logging;

namespace PhotoProcessingService
{
    public sealed class PhotoProcessingService : ServiceControl
    {
        private readonly LogWriter logWriter;

        public PhotoProcessingService()
        {
            logWriter = HostLogger.Get<PhotoProcessingService>();
        }

        public bool Start(HostControl hostControl)
        {
            return true;
        }

        public bool Stop(HostControl hostControl)
        {
            return true;
        }
    }
}
