using QuestPDF.Infrastructure;

namespace InvoiceGeneratorApp
{
    public interface IDocument1
    {
        DocumentMetadata GetMetadata();
        DocumentMetadata GetSettings();

        void Compose(IDocumentContainer container);
    }
}
