using System.IO;
using System.Threading;

namespace PhotoProcessingService
{
    public sealed class FileSystem
    {
        private readonly string m_InputDirectory;
        private readonly string m_OutputDirectory;
        private readonly string m_TempDirectory;

        public DirectoryInfo InputDirectory { get; }
        public DirectoryInfo TempDirectory { get; }

        public FileSystem(string inDirectory, string outDirectory, string tempDirectory)
        {
            m_InputDirectory = inDirectory;
            m_OutputDirectory = outDirectory;
            m_TempDirectory = tempDirectory;

            InputDirectory = !Directory.Exists(inDirectory) ? Directory.CreateDirectory(inDirectory) : new DirectoryInfo(inDirectory);
            TempDirectory = !Directory.Exists(tempDirectory) ? Directory.CreateDirectory(tempDirectory) : new DirectoryInfo(tempDirectory);

            if (!Directory.Exists(outDirectory))
            {
                Directory.CreateDirectory(outDirectory);
            }
        }

        public PdfDocument CreateNewDocument() => new PdfDocument();

        public void DeleteFile(FileInfo file)
        {
            if (TryOpenFile(file, 3))
            {
                file.Delete();
            }
        }

        public bool TryOpenFile(FileInfo fileInfo, int tryCount)
        {
            for (int i = 0; i < tryCount; i++)
            {
                try
                {
                    var fileStream = fileInfo.Open(FileMode.Open, FileAccess.Read, FileShare.None);
                    fileStream.Close();

                    return true;
                }
                catch (IOException)
                {
                    Thread.Sleep(5000);
                }
            }

            return false;
        }

        public string GetNextFilename()
        {
            var documentIndex = Directory.GetFiles(m_OutputDirectory).Length + 1;
            return Path.Combine(m_OutputDirectory, $"result_{documentIndex}.pdf");
        }
    }
}
