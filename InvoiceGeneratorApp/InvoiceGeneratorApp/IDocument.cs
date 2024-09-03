using QuestPDF.Infrastructure;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace InvoiceGeneratorApp
{
    public interface IDocument1
    {
        DocumentMetadata GetMetadata();
        DocumentMetadata GetSettings();

        void Compose(IDocumentContainer container);
    }
}
