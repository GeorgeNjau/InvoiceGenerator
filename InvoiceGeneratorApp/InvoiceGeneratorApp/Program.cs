// See https://aka.ms/new-console-template for more information
using InvoiceGeneratorApp;
using QuestPDF.Fluent;
using QuestPDF.Infrastructure;

internal class Program
{
    private static void Main(string[] args)
    {
        QuestPDF.Settings.License = LicenseType.Community;

        //SampleGenerator.Generate();
        var model = InvoiceDocumentDataSource.GetInvoiceDetails();
        var document = new InvoiceDocument(model);

        document.GeneratePdf(@"D:\MySampleFile.pdf");
        
    }
}