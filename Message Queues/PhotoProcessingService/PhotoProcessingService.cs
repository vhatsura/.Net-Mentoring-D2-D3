using System;
using System.IO;
using System.Linq;
using System.Messaging;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;
using MigraDoc.DocumentObjectModel;
using MigraDoc.Rendering;
using Shared;

namespace PhotoProcessingService
{
    public class PhotoProcessingService
    {
        private readonly string inDirectory;
        private readonly string tempDirectory;
        private readonly FileSystemWatcher watcher;
        private readonly Task processFilesTask;
        private readonly Task sendSettingsTask;
        private readonly Task controlTask;
        private readonly CancellationTokenSource tokenSource;
        private readonly AutoResetEvent newFileEvent;
        private readonly ManualResetEvent stopWaitEvent;
        private Document document;
        private Section section;
        private PdfDocumentRenderer pdfRender;
        private int timeout;
        private string status;

        public PhotoProcessingService(string inDir, string tempDir)
        {
            inDirectory = inDir;
            tempDirectory = tempDir;

            if (!Directory.Exists(inDirectory))
                Directory.CreateDirectory(inDirectory);

            if (!Directory.Exists(tempDirectory))
                Directory.CreateDirectory(tempDirectory);

            if (!MessageQueue.Exists(QueueNames.Server))
                MessageQueue.Create(QueueNames.Server);

            if (!MessageQueue.Exists(QueueNames.Monitor))
                MessageQueue.Create(QueueNames.Monitor);

            if (!MessageQueue.Exists(QueueNames.Client))
                MessageQueue.Create(QueueNames.Client);

            watcher = new FileSystemWatcher(inDirectory);
            watcher.Created += Watcher_Created;

            timeout = 5000;
            status = "Waiting";
            stopWaitEvent = new ManualResetEvent(false);
            tokenSource = new CancellationTokenSource();
            newFileEvent = new AutoResetEvent(false);
            processFilesTask = new Task(() => ProcessFiles(tokenSource.Token));
            sendSettingsTask = new Task(() => SendSettings(tokenSource.Token));
            controlTask = new Task(() => ControlService(tokenSource.Token));
        }

        public void ProcessFiles(CancellationToken token)
        {
            var currentImageIndex = -1;
            var nextPageWaiting = false;
            CreateNewDocument();

            do
            {
                status = "Process";
                foreach (var file in Directory.EnumerateFiles(inDirectory).OrderBy(f => f))
                {
                    var fileName = Path.GetFileName(file);

                    if (IsValidFormat(fileName))
                    {
                        var imageIndex = GetIndex(fileName);
                        if (imageIndex != currentImageIndex + 1 && currentImageIndex != -1 && nextPageWaiting)
                        {
                            SendDocument();
                            CreateNewDocument();
                            nextPageWaiting = false;
                        }

                        if (TryOpen(file, 3))
                        {
                            if (fileName != null)
                            {
                                var outFile = Path.Combine(tempDirectory, fileName);
                                if (File.Exists(outFile))
                                {
                                    File.Delete(file);
                                }
                                else
                                {
                                    File.Move(file, outFile);
                                }

                                AddImageToDocument(outFile);
                            }
                            currentImageIndex = imageIndex;
                            nextPageWaiting = true;
                        }
                    }
                    else
                    {
                        if (TryOpen(file, 3))
                        {
                            File.Delete(file);
                        }
                    }
                }

                status = "Waiting";
                if (!newFileEvent.WaitOne(timeout) && nextPageWaiting)
                {
                    SendDocument();
                    CreateNewDocument();
                    nextPageWaiting = false;
                }

                if (token.IsCancellationRequested)
                {
                    if (nextPageWaiting)
                    {
                        SendDocument();
                    }

                    foreach (var file in Directory.EnumerateFiles(tempDirectory))
                    {
                        if (TryOpen(file, 3))
                        {
                            File.Delete(file);
                        }
                    }
                }
            }
            while (!token.IsCancellationRequested);
        }

        private void CreateNewDocument()
        {
            document = new Document();
            section = document.AddSection();
            pdfRender = new PdfDocumentRenderer {Document = document};
        }

        public void SendSettings(CancellationToken token)
        {
            while (!token.IsCancellationRequested)
            {
                var settings = new Settings
                {
                    Date = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"),
                    Status = status,
                    Timeout = timeout
                };

                using (var serverQueue = new MessageQueue(QueueNames.Monitor))
                {
                    var message = new Message(settings);
                    serverQueue.Send(message);
                }

                Thread.Sleep(10000);
            }
        }

        public void ControlService(CancellationToken token)
        {
            using (var clientQueue = new MessageQueue(QueueNames.Client))
            {
                clientQueue.Formatter = new XmlMessageFormatter(new[] { typeof(int) });

                while (!token.IsCancellationRequested)
                {
                    var asyncReceive = clientQueue.BeginPeek();

                    if (WaitHandle.WaitAny(new[] { stopWaitEvent, asyncReceive.AsyncWaitHandle }) == 0)
                    {
                        break;
                    }

                    var message = clientQueue.EndPeek(asyncReceive);
                    clientQueue.Receive();
                    timeout = (int)message.Body;
                }
            }
        }

        private void SendDocument()
        {
            pdfRender.RenderDocument();
            var pageCount = pdfRender.PdfDocument.PageCount - 1;
            pdfRender.PdfDocument.Pages.RemoveAt(pageCount);
            var pdfDocument = pdfRender.PdfDocument;
            var buffer = new byte[1024];

            using (var ms = new MemoryStream())
            {
                pdfDocument.Save(ms, false);
                ms.Position = 0;
                var position = 0;
                var size = (int)Math.Ceiling((double)(ms.Length) / 1024) - 1;

                int bytesRead;
                while ((bytesRead = ms.Read(buffer, 0, buffer.Length)) > 0)
                {
                    var pdfChunk = new PDFChunk()
                    {
                        Position = position,
                        Size = size,
                        Buffer = buffer.ToList(),
                        BufferSize = bytesRead
                    };

                    position++;

                    using (var serverQueue = new MessageQueue(QueueNames.Server, QueueAccessMode.Send))
                    {
                        var message = new Message(pdfChunk);
                        serverQueue.Send(message);
                    }
                }
            }
        }

        private void AddImageToDocument(string file)
        {
            var image = section.AddImage(file);

            image.Height = document.DefaultPageSetup.PageHeight;
            image.Width = document.DefaultPageSetup.PageWidth;
            image.ScaleHeight = 0.75;
            image.ScaleWidth = 0.75;

            section.AddPageBreak();
        }

        private bool IsValidFormat(string fileName)
        {
            return Regex.IsMatch(fileName, @"^img_[0-9]{3}.(jpg|png|jpeg)$");
        }

        private int GetIndex(string fileName)
        {
            var match = Regex.Match(fileName, @"[0-9]{3}");

            return match.Success ? int.Parse(match.Value) : -1;
        }

        private void Watcher_Created(object sender, FileSystemEventArgs e)
        {
            newFileEvent.Set();
        }

        public void Start()
        {
            processFilesTask.Start();
            sendSettingsTask.Start();
            controlTask.Start();
            watcher.EnableRaisingEvents = true;
        }

        public void Stop()
        {
            watcher.EnableRaisingEvents = false;
            tokenSource.Cancel();
            stopWaitEvent.Set();
            Task.WaitAll(processFilesTask, sendSettingsTask, controlTask);
        }

        private bool TryOpen(string fileName, int tryCount)
        {
            for (int i = 0; i < tryCount; i++)
            {
                try
                {
                    var file = File.Open(fileName, FileMode.Open, FileAccess.Read, FileShare.None);
                    file.Close();

                    return true;
                }
                catch (IOException)
                {
                    Thread.Sleep(5000);
                }
            }

            return false;
        }
    }
}
