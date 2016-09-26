using System;
using System.Collections.Generic;
using System.IO;
using System.Messaging;
using System.Threading;
using System.Threading.Tasks;
using System.Xml.Linq;
using Shared;

namespace Server
{
    public class PhotoServer
    {
        private readonly string outDirectory;
        private readonly FileSystemWatcher watcher;
        private readonly Task processFilesTask;
        private readonly Task monitoringTask;
        private readonly CancellationTokenSource tokenSource;
        private readonly ManualResetEvent stopWaitEvent;
        private int lastTimeout;

        public PhotoServer(string outDir)
        {
            outDirectory = outDir;
            if (!Directory.Exists(outDirectory))
                Directory.CreateDirectory(outDirectory);

            if (!MessageQueue.Exists(QueueNames.Server))
                MessageQueue.Create(QueueNames.Server);

            if (!MessageQueue.Exists(QueueNames.Monitor))
                MessageQueue.Create(QueueNames.Monitor);

            if (!MessageQueue.Exists(QueueNames.Client))
                MessageQueue.Create(QueueNames.Client);

            var pathToXml = GetFullPath();
            watcher = new FileSystemWatcher(pathToXml) {Filter = "*.xml"};
            watcher.Changed += Watcher_Changed;

            stopWaitEvent = new ManualResetEvent(false);
            tokenSource = new CancellationTokenSource();
            processFilesTask = new Task(() => ProcessFiles(tokenSource.Token));
            monitoringTask = new Task(() => MonitorService(tokenSource.Token));
        }

        public void ProcessFiles(CancellationToken token)
        {
            using (var serverQueue = new MessageQueue(QueueNames.Server))
            {
                serverQueue.Formatter = new XmlMessageFormatter(new[] { typeof(PDFChunk) });
                var chunks = new List<PDFChunk>();

                do
                {
                    var enumerator = serverQueue.GetMessageEnumerator2();
                    var count = 0;

                    while (enumerator.MoveNext())
                    {
                        var body = enumerator.Current?.Body;
                        var section = body as PDFChunk;
                        if (section != null)
                        {
                            var chunk = section;
                            chunks.Add(chunk);

                            if (chunk.Position == chunk.Size)
                            {
                                SaveFile(chunks);
                                chunks.Clear();
                            }
                        }

                        count++;
                    }

                    for (var i = 0; i < count; i++)
                    {
                        serverQueue.Receive();
                    }

                    Thread.Sleep(1000);
                }
                while (!token.IsCancellationRequested);
            }
        }

        public void SaveFile(List<PDFChunk> chunks)
        {
            var documentIndex = Directory.GetFiles(outDirectory).Length + 1;
            var resultFile = Path.Combine(outDirectory, $"result_{documentIndex}.pdf");
            using (Stream destination = File.Create(Path.Combine(outDirectory, resultFile)))
            {
                foreach (var chunk in chunks)
                {
                    destination.Write(chunk.Buffer.ToArray(), 0, chunk.BufferSize);
                }
            }
        }

        public void MonitorService(CancellationToken token)
        {
            using (var serverQueue = new MessageQueue(QueueNames.Monitor))
            {
                serverQueue.Formatter = new XmlMessageFormatter(new[] { typeof(Settings) });

                while (!token.IsCancellationRequested)
                {
                    var asyncReceive = serverQueue.BeginPeek();

                    if (WaitHandle.WaitAny(new[] { stopWaitEvent, asyncReceive.AsyncWaitHandle }) == 0)
                    {
                        break;
                    }

                    var message = serverQueue.EndPeek(asyncReceive);
                    serverQueue.Receive();
                    var settings = (Settings)message.Body;
                    lastTimeout = settings.Timeout;
                    WriteSettings(settings);
                }
            }
        }

        public void WriteSettings(Settings settings)
        {
            var path = GetFullPath();
            var fullPath = Path.Combine(path, "setting.csv");

            using (var sw = File.AppendText(fullPath))
            {
                var line = $"{settings.Date},{settings.Status},{settings.Timeout}s";
                sw.WriteLine(line);
            }
        }

        private void Watcher_Changed(object sender, FileSystemEventArgs e)
        {
            var path = GetFullPath();
            var fullPath = Path.Combine(path, "timeout.xml");
            if (TryOpen(fullPath, 3))
            {
                var doc = XDocument.Load(fullPath);
                if (doc.Root != null)
                {
                    var timeout = int.Parse(doc.Root.Value);
                    if (lastTimeout != timeout)
                    {
                        using (var clientQueue = new MessageQueue(QueueNames.Client))
                        {
                            clientQueue.Send(timeout);
                        }
                    }
                }
            }
        }

        private static string GetFullPath()
        {
            var currentDir = AppDomain.CurrentDomain.BaseDirectory;
            return Path.GetFullPath(Path.Combine(currentDir, @"..\..\"));
        }

        public void Start()
        {
            processFilesTask.Start();
            monitoringTask.Start();
            watcher.EnableRaisingEvents = true;
        }

        public void Stop()
        {
            watcher.EnableRaisingEvents = false;
            tokenSource.Cancel();
            stopWaitEvent.Set();
            Task.WaitAll(processFilesTask, monitoringTask);
        }

        private static bool TryOpen(string fileName, int tryCount)
        {
            for (var i = 0; i < tryCount; i++)
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
