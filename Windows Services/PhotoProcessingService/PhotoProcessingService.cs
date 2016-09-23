using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;
using Topshelf;
using Topshelf.Logging;

namespace PhotoProcessingService
{
    public sealed class PhotoProcessingService : ServiceControl
    {
        private readonly LogWriter m_LogWriter;

        private FileSystem FileSystem { get; }

        private readonly FileSystemWatcher m_Watcher;
        private readonly Task workTask;
        private readonly CancellationTokenSource tokenSource;
        private readonly AutoResetEvent newFileEvent;

        public PhotoProcessingService(string inDirectory, string outDirectory, string tempDirectory)
        {
            m_LogWriter = HostLogger.Get<PhotoProcessingService>();

            FileSystem = new FileSystem(inDirectory, outDirectory, tempDirectory);

            m_Watcher = new FileSystemWatcher();
            m_Watcher.Created += (sender, args) => newFileEvent.Set();
            tokenSource = new CancellationTokenSource();
            workTask = new Task(() => WorkProcedure(tokenSource.Token));
            newFileEvent = new AutoResetEvent(false);
        }

        public bool Start(HostControl hostControl)
        {
            workTask.Start();
            m_Watcher.EnableRaisingEvents = true;
            return true;
        }

        public bool Stop(HostControl hostControl)
        {
            m_Watcher.EnableRaisingEvents = false;
            tokenSource.Cancel();
            workTask.Wait();
            return true;
        }

        private static bool IsValidFormat(string fileName) => Regex.IsMatch(fileName, @"^img_[0-9]{3}.(jpg|png|jpeg)$");

        private static int GetIndex(string fileName)
        {
            var match = Regex.Match(fileName, @"[0-9]{3}");

            return match.Success ? int.Parse(match.Value) : -1;
        }

        public void WorkProcedure(CancellationToken token)
        {
            var currentImageIndex = -1;
            var imageCount = 0;
            var nextPageWaiting = false;

            var pdf = CreateNewDocument();

            do
            {
                foreach (var file in FileSystem.InputDirectory.GetFiles().Skip(imageCount))
                {
                    var fileName = file.Name;
                    if (IsValidFormat(fileName))
                    {
                        var imageIndex = GetIndex(fileName);
                        if (imageIndex != currentImageIndex + 1 && currentImageIndex != -1 && nextPageWaiting)
                        {
                            SavePdf(ref pdf, out nextPageWaiting);
                        }

                        if (FileSystem.TryOpenFile(file, 3))
                        {
                            pdf.AddImage(file.FullName);
                            imageCount++;
                            currentImageIndex = imageIndex;
                            nextPageWaiting = true;
                        }
                    }
                    else
                    {
                        var outFile = Path.Combine(FileSystem.TempDirectory.FullName, fileName);
                        if (File.Exists(outFile))
                        {
                            FileSystem.DeleteFile(file);
                        }
                        else
                        {
                            file.MoveTo(outFile);
                        }
                    }
                }

                if (!newFileEvent.WaitOne(5000) && nextPageWaiting)
                {
                    SavePdf(ref pdf, out nextPageWaiting);
                }

                if (token.IsCancellationRequested)
                {
                    if (nextPageWaiting)
                    {
                        pdf.Save(FileSystem.GetNextFilename());
                    }

                    foreach (var file in FileSystem.InputDirectory.GetFiles())
                    {
                        FileSystem.DeleteFile(file);
                    }
                }
            } while (!token.IsCancellationRequested);
        }

        private void SavePdf(ref PdfDocument pdf, out bool nextPageWaiting)
        {
            pdf.Save(FileSystem.GetNextFilename());
            pdf = CreateNewDocument();
            nextPageWaiting = false;
        }

        private PdfDocument CreateNewDocument() => FileSystem.CreateNewDocument();
    }
}
