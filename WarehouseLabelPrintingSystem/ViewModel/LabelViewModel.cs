﻿using iTextSharp.text.pdf;
using iTextSharp.text;
using System.Drawing;
using System.IO;
using ZXing.Common;
using ZXing;
using ZXing.Windows.Compatibility;
using Rectangle = iTextSharp.text.Rectangle;
using Image = iTextSharp.text.Image;
using System.Drawing.Printing;


namespace WarehouseLabelPrintingSystem.ViewModel
{
    /// <summary>
    /// ViewModel for creating labels with product information and barcodes.
    /// </summary>
    public class LabelViewModel
    {
        public string? ProductNumber { get; set; }
        public string? ProductName { get; set; }
        public string? Unit { get; set; }
        public string? Location { get; set; }
        public string? BarcodeText { get; set; }

        // Positions for various label elements
        public PointF BarcodePosition { get; set; }
        public PointF ProductNumberPosition { get; set; }
        public PointF ProductNamePosition { get; set; }
        public PointF UnitPosition { get; set; }
        public PointF LocationPosition { get; set; }
        public PointF BarcodeTextPosition { get; set; }
        public PointF NotePosition { get; set; }

        public LabelViewModel()
        {
            
        }

        public LabelViewModel(
            PointF barcodePosition,
            PointF productNumberPosition,
            PointF productNamePosition,
            PointF unitPosition,
            PointF locationPosition,
            PointF barcodeTextPosition,
            PointF notePosition)
        {
            BarcodePosition = barcodePosition;
            ProductNumberPosition = productNumberPosition;
            ProductNamePosition = productNamePosition;
            UnitPosition = unitPosition;
            LocationPosition = locationPosition;
            BarcodeTextPosition = barcodeTextPosition;
            NotePosition = notePosition;
        }

        public void GenerateLabelSize208x148(string filePath, string barcodeStr)
        {
            try
            {
                // Set up the PDF document with specific dimensions and margins
                Document document = new(new Rectangle(208f, 148f), 0, 0, 0, 0);
                PdfWriter writer = PdfWriter.GetInstance(document, new FileStream(filePath, FileMode.Create));

                document.Open();

                PdfContentByte canvas = writer.DirectContent;

                // Set the font for ProductName
                var productNameFont = FontFactory.GetFont(FontFactory.HELVETICA, 12, BaseColor.BLACK);

                // Define an area for ProductName with wrapping text
                var productNameWidth = 188f;  // Width of the area
                var productNameHeight = 40f;  // Height for text wrapping

                Rectangle productNameRect = new(
                    ProductNamePosition.X,
                    ProductNamePosition.Y,
                    ProductNamePosition.X + productNameWidth,
                    ProductNamePosition.Y + productNameHeight
                );

                // Add ProductName with text wrapping
                ColumnText ct = new(canvas);
                ct.SetSimpleColumn(
                    new Phrase(ProductName, productNameFont),
                    productNameRect.Left,
                    productNameRect.Bottom,
                    productNameRect.Right,
                    productNameRect.Top,
                    15f, // Leading between lines
                    Element.ALIGN_LEFT
                );

                ct.Go();

                // Place other elements using ColumnText with predefined positions
                ColumnText.ShowTextAligned(
                    canvas,
                    Element.ALIGN_LEFT,
                    new Phrase(ProductNumber, FontFactory.GetFont(FontFactory.HELVETICA_BOLD, 25, BaseColor.BLACK)),
                    ProductNumberPosition.X,
                    ProductNumberPosition.Y,
                    0
                );

                ColumnText.ShowTextAligned(
                    canvas,
                    Element.ALIGN_LEFT,
                    new Phrase(Unit, FontFactory.GetFont(FontFactory.HELVETICA, 10, BaseColor.BLACK)),
                    UnitPosition.X,
                    UnitPosition.Y,
                    0
                );

                ColumnText.ShowTextAligned(
                    canvas,
                    Element.ALIGN_LEFT,
                    new Phrase(Location, FontFactory.GetFont(FontFactory.HELVETICA, 25, BaseColor.BLACK)),
                    LocationPosition.X,
                    LocationPosition.Y,
                    0
                );

                if (!string.IsNullOrEmpty(barcodeStr))
                {

                    // Generate the barcode using ZXing and add it to the PDF
                    var barcodeWriter = new BarcodeWriter
                    {
                        Format = BarcodeFormat.EAN_13,
                        Options = new EncodingOptions
                        {
                            Width = 200,
                            Height = 30,
                            NoPadding = true,
                            PureBarcode = true,
                        }
                    };

                    var barcodeImage = barcodeWriter.Write(barcodeStr);
                    var barcode = Image.GetInstance(barcodeImage, System.Drawing.Imaging.ImageFormat.Bmp);
                    barcode.SetAbsolutePosition(BarcodePosition.X, BarcodePosition.Y);

                    canvas.AddImage(barcode);

                    // Add the barcode text with specific position
                    ColumnText.ShowTextAligned(
                        canvas,
                        Element.ALIGN_LEFT,
                        new Phrase(BarcodeText, FontFactory.GetFont(FontFactory.HELVETICA, 12, BaseColor.BLACK)),
                        BarcodeTextPosition.X,
                        BarcodeTextPosition.Y,
                        0
                    );
                }

                document.Close(); // Close the document to save changes
            }
            catch (Exception ex)
            {
                // Handle exceptions and provide meaningful feedback
                Console.WriteLine($"Error while generating label PDF: {ex.Message}");
            }
        }

        public void GenerateLabelSize39x27(string filePath, string barcodeStr)
        {
            try
            {
                using (var fileStream = new FileStream(filePath, FileMode.Create, FileAccess.Write, FileShare.None))
                {
                    var document = new Document(new Rectangle(39f, 27f), 0, 0, 0, 0);
                    using (var writer = PdfWriter.GetInstance(document, fileStream))
                    {
                        document.Open();

                        var canvas = writer.DirectContent;

                        var productNameFont = FontFactory.GetFont(FontFactory.HELVETICA, 3, BaseColor.BLACK);
                        var productNameWidth = 35f;
                        var productNameHeight = 20f;
                        var productNameRect = new Rectangle(
                            ProductNamePosition.X,
                            ProductNamePosition.Y,
                            ProductNamePosition.X + productNameWidth,
                            ProductNamePosition.Y + productNameHeight
                        );

                        var ct = new ColumnText(canvas);
                        ct.SetSimpleColumn(
                            new Phrase(ProductName, productNameFont),
                            productNameRect.Left,
                            productNameRect.Bottom,
                            productNameRect.Right,
                            productNameRect.Top,
                            4f,
                            Element.ALIGN_LEFT
                        );

                        ct.Go();

                        ColumnText.ShowTextAligned(
                            canvas,
                            Element.ALIGN_LEFT,
                            new Phrase(ProductNumber, FontFactory.GetFont(FontFactory.HELVETICA_BOLD, 5, BaseColor.BLACK)),
                            ProductNumberPosition.X,
                            ProductNumberPosition.Y,
                            0
                        );

                        ColumnText.ShowTextAligned(
                            canvas,
                            Element.ALIGN_LEFT,
                            new Phrase(Unit, FontFactory.GetFont(FontFactory.HELVETICA, 3, BaseColor.BLACK)),
                            UnitPosition.X,
                            UnitPosition.Y,
                            0
                        );

                        if (!string.IsNullOrEmpty(barcodeStr))
                        {
                            var barcodeWriter = new BarcodeWriter
                            {
                                Format = BarcodeFormat.EAN_13,
                                Options = new EncodingOptions
                                {
                                    Width = 200,
                                    Height = 30,
                                    NoPadding = true,
                                    PureBarcode = true
                                }
                            };

                            var barcodeImage = barcodeWriter.Write(barcodeStr);
                            var barcode = Image.GetInstance(barcodeImage, System.Drawing.Imaging.ImageFormat.Bmp);

                            barcode.ScaleAbsolute(30f, 5f);
                            barcode.SetAbsolutePosition(BarcodePosition.X, BarcodePosition.Y);
                            canvas.AddImage(barcode);

                            ColumnText.ShowTextAligned(
                                canvas,
                                Element.ALIGN_LEFT,
                                new Phrase(BarcodeText, FontFactory.GetFont(FontFactory.HELVETICA, 3, BaseColor.BLACK)),
                                BarcodeTextPosition.X,
                                BarcodeTextPosition.Y,
                                0
                            );
                        }

                        document.Close();
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error generating PDF: {ex.Message}");
            }
        }

        public static void PrintPdf(string filePath, string printerName, int width, int height)
        {
            try
            {
                // Create the printer settings for our printer
                var printerSettings = new PrinterSettings
                {
                    PrinterName = printerName,
                };

                // Create our page settings for the paper size selected
                var pageSettings = new PageSettings(printerSettings)
                {
                    Margins = new Margins(0, 0, 0, 0),
                    PaperSize = new PaperSize(printerName, width, height),
                };

                foreach (PaperSize paperSize in printerSettings.PaperSizes)
                {
                    if (paperSize.PaperName == printerName)
                    {
                        pageSettings.PaperSize = paperSize;
                        break;
                    }
                }

                // Now print the PDF document
                using (var document = PdfiumViewer.PdfDocument.Load(filePath))
                {
                    using (var printDocument = document.CreatePrintDocument())
                    {
                        printDocument.PrinterSettings = printerSettings;
                        printDocument.DefaultPageSettings = pageSettings;
                        printDocument.PrintController = new StandardPrintController();
                        printDocument.Print();
                    }
                }
            }
            catch
            {
                Console.WriteLine("An error occurred while printing the document.");
            }
        }
    }
}
