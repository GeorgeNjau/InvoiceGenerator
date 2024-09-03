using InvoiceGeneratorApp.Models;
using QuestPDF.Fluent;
using QuestPDF.Helpers;
using QuestPDF.Infrastructure;

namespace InvoiceGeneratorApp
{
    public class InvoiceDocument : IDocument
    {
        public static Image LogoImage { get; } = Image.FromFile("");
        public InvoiceModel Model { get; }

        public InvoiceDocument(InvoiceModel model)
        {
            Model = model;
        }

        public void Compose(IDocumentContainer container)
        {
            container.Page(page =>
            {
                page.Margin(50);

                page.Header().Element(ComposeHeader);
                page.Content().Element(ComposeContent);

                page.Footer().AlignCenter().Text(x =>
                {
                    x.CurrentPageNumber();
                    x.Span(" / ");
                    x.TotalPages();
                });

            });
        }

        private void ComposeContent(IContainer container)
        {
            container.PaddingVertical(40).Column(column =>
            {
                column.Spacing(20);

                column.Item().Row(row =>
                {
                    row.RelativeItem().Component(new AddressComponent("From", Model.SellerAddress));
                    row.ConstantItem(50);
                    row.RelativeItem().Component(new AddressComponent("For", Model.CustomerAddress));
                });

                column.Item().Element(ComposeTable);

                var totalPrice = Model.Items.Sum(x => x.Price * x.Quantity);

                column.Item().PaddingRight(5).AlignRight().Text($"Grand Total: {totalPrice:C}").SemiBold();

                if (!string.IsNullOrWhiteSpace(Model.Comments))
                    column.Item().PaddingTop(25).Element(ComposeComments);

            });

        }

        void ComposeTable(IContainer container)
        {
            var headerStyle = TextStyle.Default.SemiBold();

            container.Table(table =>
            {
                table.ColumnsDefinition(columns =>
                {
                    columns.ConstantColumn(25);
                    columns.RelativeColumn(3);
                    columns.RelativeColumn();
                    columns.RelativeColumn();
                    columns.RelativeColumn();
                });

                table.Header(header =>
                {
                    header.Cell().Text("#");
                    header.Cell().Text("Product").Style(headerStyle);
                    header.Cell().AlignRight().Text("Unit Price").Style(headerStyle);
                    header.Cell().AlignRight().Text("Quantity").Style(headerStyle);
                    header.Cell().AlignRight().Text("Total").Style(headerStyle);

                    header.Cell().ColumnSpan(5).PaddingTop(5).BorderBottom(1).BorderColor(Color.FromHex("#FF0000"));
                });

                static IContainer CellStyle(IContainer container) => container.BorderBottom(1).BorderColor(Colors.Grey.Lighten2).PaddingVertical(5);

                foreach (var item in Model.Items)
                {
                    var index = Model.Items.IndexOf(item) + 1;

                    table.Cell().Element(CellStyle).Text($"{index}");
                    table.Cell().Element(CellStyle).Text(item.Name);
                    table.Cell().Element(CellStyle).AlignRight().Text($"{item.Price:C}");
                    table.Cell().Element(CellStyle).AlignRight().Text($"{item.Quantity}");
                    table.Cell().Element(CellStyle).AlignRight().Text($"{item.Price * item.Quantity:C}");                    
                }

            });

        }

        void ComposeComments(IContainer container)
        {
            container.ShowEntire().Background(Colors.Grey.Lighten3).Padding(10).Column(column =>
            {
                column.Spacing(5);
                column.Item().Text("Comments").FontSize(14).SemiBold();
                column.Item().Text(Model.Comments);
            });
        }

        private void ComposeHeader(IContainer container)
        {
            var titleStyle = TextStyle.Default.FontSize(20).SemiBold().FontColor(Colors.Blue.Medium);

            container.Row(row =>
            {
                row.RelativeItem().Column(column =>
                {
                    column.Item().Text($"Invoice #({Model.InvoiceNumber})").Style(titleStyle);

                    column.Item().Text(text =>
                    {
                        text.Span("Issue date: ").SemiBold();
                        text.Span($"{Model.IssueDate:d}");
                    });

                    column.Item().Text(text =>
                    {
                        text.Span("Due date: ").SemiBold();
                        text.Span($"{Model.DueDate:d}");
                    });

                });

                row.ConstantItem(100).Height(50).Placeholder();

            });
        }

        public DocumentMetadata GetMetadata() => DocumentMetadata.Default;

        public DocumentMetadata GetSettings() => DocumentMetadata.Default;
    }

    public class AddressComponent : IComponent
    {
        public string Title { get; set; }
        public Address Address { get; set; }

        public AddressComponent(string title, Address address)
        {
            Title = title;
            Address = address;
        }

        public void Compose(IContainer container)
        {
            container.ShowEntire().Column(column =>
            {
                column.Spacing(2);

                column.Item().Text(Title).SemiBold();
                column.Item().PaddingBottom(5).LineHorizontal(1);

                column.Item().Text(Address.CompanyName);
                column.Item().Text(Address.Street);
                column.Item().Text($"{Address.CompanyName}, {Address.State}");
                column.Item().Text(Address.Email);
                column.Item().Text(Address.Phone);

            });
        }
    }

}
