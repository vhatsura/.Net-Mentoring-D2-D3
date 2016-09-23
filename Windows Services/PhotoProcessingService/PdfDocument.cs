using MigraDoc.DocumentObjectModel;
using MigraDoc.Rendering;

namespace PhotoProcessingService
{
    public sealed class PdfDocument
    {
        private readonly Document m_Document;
        private readonly Section m_Section;
        private readonly PdfDocumentRenderer m_Render;

        public PdfDocument()
        {
            m_Document = new Document();
            m_Section = m_Document.AddSection();
            m_Render = new PdfDocumentRenderer();
        }

        public void AddImage(string filePath)
        {
            var image = m_Section.AddImage(filePath);

            image.Height = m_Document.DefaultPageSetup.PageHeight;
            image.Width = m_Document.DefaultPageSetup.PageWidth;
            image.ScaleHeight = 0.75;
            image.ScaleWidth = 0.75;

            m_Section.AddPageBreak();
        }

        public void Save(string filePath)
        {
            m_Render.Document = m_Document;
            m_Render.RenderDocument();
            m_Render.Save(filePath);
        }
    }
}
