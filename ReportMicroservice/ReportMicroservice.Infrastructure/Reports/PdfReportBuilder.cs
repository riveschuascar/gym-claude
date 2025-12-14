using QuestPDF.Infrastructure;
using ReportMicroservice.Domain.Models;
using ReportMicroservice.Domain.Ports;
using QuestPDF.Fluent;
using QuestPDF.Helpers;
using ReportMicroservice.Domain.DTO;
using System.Net.Http.Headers;
using System.Text.Json;

namespace ReportMicroservice.Infrastructure.Reports
{
    // Implementación del Patrón Builder con QuestPDF
    public class PdfReportBuilder : IReportBuilder
    {
        private SaleReportData _data;
        private Document _document;
        private IContainer _container; 

        public PdfReportBuilder()
        {
            // Configuración de licencia Community 
            QuestPDF.Settings.License = LicenseType.Community;
        }

        public IReportBuilder Reset()
        {
            _data = null;
            _document = null;
            return this;
        }

        public IReportBuilder SetReportData(SaleReportData data)
        {
            _data = data;
            return this;
        }

        private Action<IContainer> _headerAction;
        private Action<IContainer> _customerInfoAction;
        private Action<IContainer> _tableAction;
        private Action<IContainer> _totalAction;
        private Action<IContainer> _footerAction;

        public IReportBuilder BuildHeader()
        {
            _headerAction = container =>
            {
                container.Row(row =>
                {
                    var logoPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Assets", "logo.jpeg");

                    var logoContainer = row.ConstantItem(100).Height(60); 

                    if (File.Exists(logoPath))
                    {
                        var imageBytes = File.ReadAllBytes(logoPath);
                        logoContainer.Image(imageBytes).FitArea();
                    }
                    else
                    {
                        logoContainer.AlignCenter().AlignMiddle().Text("Logo").FontSize(10);
                    }

                    row.RelativeItem().Column(col =>
                    {
                        col.Item().Text("COMPROBANTE DE VENTA")
                           .FontSize(20).Bold().FontColor(Colors.Black).AlignCenter();

                        col.Item().Text("Gym Claude Center").FontSize(12).FontColor(Colors.Grey.Darken1).AlignCenter();
                    });
                });
            };
            return this;
        }

        public IReportBuilder BuildCustomerInfo()
        {
            _customerInfoAction = container =>
            {
                container.Column(col =>
                {
                    col.Item().Text($"Fecha: {_data.Date:dd/MM/yyyy}");

                    col.Item().PaddingTop(5).Row(row =>
                    {
                        row.AutoItem().Text($"CI/NIT: {_data.ClientCiNit}").Bold();
                        row.RelativeItem().PaddingLeft(20).Text($"Razón Social: {_data.ClientName}");
                    });
                });
            };
            return this;
        }

        public IReportBuilder BuildDetailsTable()
        {
            _tableAction = container =>
            {
                container.PaddingTop(10).Table(table =>
                {
                    // Definición de columnas
                    table.ColumnsDefinition(columns =>
                    {
                        columns.ConstantColumn(60); // Cantidad
                        columns.RelativeColumn();   // Descripción
                        columns.ConstantColumn(100); // Precio U.
                        columns.ConstantColumn(100); // Importe
                    });

                    // Cabecera
                    table.Header(header =>
                    {
                        header.Cell().Element(CellStyle).Text("Cantidad");
                        header.Cell().Element(CellStyle).Text("Descripción");
                        header.Cell().Element(CellStyle).Text("Precio Unitario");
                        header.Cell().Element(CellStyle).Text("Importe");

                        static IContainer CellStyle(IContainer container)
                    {
                        return container.DefaultTextStyle(x => x.SemiBold()).PaddingVertical(5).BorderBottom(1).BorderColor(Color.FromRGB(128, 128, 128));
                        }
                    });

                    // Filas
                    foreach (var item in _data.Details)
                    {
                        table.Cell().Element(CellStyle).Text(item.Quantity.ToString());
                        table.Cell().Element(CellStyle).Text(item.Description);
                        table.Cell().Element(CellStyle).Text($"{item.UnitPrice:F2}");
                        table.Cell().Element(CellStyle).Text($"{item.Import:F2}");

                        static IContainer CellStyle(IContainer container)
                        {
                            return container.PaddingVertical(5).BorderBottom(1).BorderColor(Colors.Grey.Lighten3);
                        }
                    }
                });
            };
            return this;
        }

        public IReportBuilder BuildTotalSection()
        {
            _totalAction = container =>
            {
                container.PaddingTop(10).Row(row =>
                {
                    row.RelativeItem(); // Espacio vacío a la izquierda
                    row.AutoItem().Text($"Total: {_data.TotalAmount:F2}").FontSize(14).Bold();
                });
            };
            return this;
        }

        public IReportBuilder BuildFooter()
        {
            _footerAction = container =>
            {
                container.Row(row =>
                {
                    row.RelativeItem().Text(text =>
                    {
                        text.Span($"{_data.GeneratedAt:g} - ");
                        text.Span(_data.GeneratedByEmail).Italic();
                    });
                });
            };
            return this;
        }

        public byte[] GetPdfBytes()
        {
            var doc = Document.Create(container =>
            {
                container.Page(page =>
                {
                    page.Margin(2, Unit.Centimetre);

                    page.Header().Element(c => _headerAction?.Invoke(c));

                    page.Content().Column(col =>
                    {
                        col.Item().PaddingTop(20).Element(c => _customerInfoAction?.Invoke(c));
                        col.Item().Element(c => _tableAction?.Invoke(c));
                        col.Item().Element(c => _totalAction?.Invoke(c));
                    });

                    page.Footer().Element(c => _footerAction?.Invoke(c));
                });
            });

            return doc.GeneratePdf();
        }
    }
}
