using iTextSharp;
using iTextSharp.text;
using iTextSharp.text.pdf;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Xml;
using System.Data;

namespace TabularReport
{
    public class BuildTabularReport : IDisposable
    {
        private bool _Disposed = false;

        public BuildTabularReport()
        {

        }

        public void RenderReport(DataTable DT, string ReportFileName)
        {

            // unclear why we can't make this a using block
            // if we close the pdfwriter before the doc block closes then we get runtime error
            PdfWriter MyPdfWriter;

            using (FileStream OutputStream = new FileStream(ReportFileName, FileMode.Create))
            {
                using (Document doc = new Document(PageSize.LETTER))
                {
                    MyPdfWriter = PdfWriter.GetInstance(doc, OutputStream);
                    doc.Open();

                    // table name
                    Phrase docHeader = new Phrase("Table: " + DT.TableName + Environment.NewLine);
                    doc.Add(docHeader);

                    // filename
                    docHeader = new Phrase("Filename: " + ReportFileName + Environment.NewLine + Environment.NewLine);
                    doc.Add(docHeader);

                    string DocTitle = "Search Terms Report (STR) -- " + Path.GetFileNameWithoutExtension(ReportFileName);
                    doc.AddTitle(DocTitle);

                    doc.Add(RenderTable(DT));

                    // at this point we need to render an XML file and
                    // embed it in the PDF

                    //System.Diagnostics.Debug.Print("");

                }
            }

            // clean up
            MyPdfWriter.Close();
            MyPdfWriter.Dispose();
        }

        private PdfPTable RenderTable(DataTable DT)
        {
            PdfPTable table = new PdfPTable(DT.Columns.Count);
            table.HeaderRows = 2;
            //table.DefaultCell.Border = 1;

            // font for everything
            float FontSize = 12.0f;
            BaseFont f = BaseFont.CreateFont(BaseFont.HELVETICA, BaseFont.CP1252, true);
            Font titlefont = new Font(f, FontSize+6, Font.BOLD, BaseColor.RED);
            Font headerfont = new Font(f, FontSize, Font.BOLD, BaseColor.BLACK);
            Font datafont = new Font(f, FontSize, Font.NORMAL, BaseColor.BLACK);

            // cell style
            PdfPCell celltemplate = new PdfPCell();
            celltemplate.Padding = 5.0f;
            celltemplate.NoWrap = false;
            //celltemplate.CellEvent = new CellMod();
            celltemplate.Padding = 3.0f;

            IPdfPTableEvent stylingEvent = new TableStyling(DT, headerfont, celltemplate);
            table.TableEvent = stylingEvent;

            // Section Title
            PdfPCell TitleCell = new PdfPCell(celltemplate);
            TitleCell.Colspan = DT.Columns.Count; 
            Paragraph Title = new Paragraph(DT.TableName, titlefont);
            Title.Alignment = Element.ALIGN_CENTER;
            TitleCell.PaddingBottom = 5;
            TitleCell.AddElement(Title);
            table.AddCell(TitleCell);

            // header column names
            for (int C = 0; C < DT.Columns.Count; C++)
            {
                string value = DT.Columns[C].ColumnName;
                Paragraph P = new Paragraph(value, headerfont);
                PdfPCell cell = new PdfPCell(celltemplate);
                cell.AddElement(P);
                table.AddCell(cell);
            }


            // data
            for (int R = 0; R < DT.Rows.Count; R++)
            {
                for (int C = 0; C < DT.Columns.Count; C++)
                {
                    string value = (string)DT.Rows[R][C];
                    Paragraph P = new Paragraph(value, datafont);
                    PdfPCell cell = new PdfPCell(celltemplate);
                    cell.AddElement(P);
                    table.AddCell(cell);
                }
            }

            return table;
        }

        /// <summary>
        /// Render a table to XML
        /// need to rewrite these classes to work on DataSource instead
        /// </summary>
        /// <returns>Stream representing an XML file</returns>
        private Stream RenderXmlFile(DataTable DT)
        {
            MemoryStream temp = new MemoryStream();
            DT.WriteXml(temp);
            return temp;
        }


        private class CellMod : IPdfPCellEvent
        {
            public void CellLayout(PdfPCell cell, Rectangle position, PdfContentByte[] canvases)
            {

                //Rectangle rect = new Rectangle(position.Left, position.Right, position.Bottom, position.Top);
                float colortint = 0.5f;

                if (cell.Phrase != null)
                {
                    if (cell.Phrase.Content.ToString() != "")
                    {
                        int number = 0;
                        if (Int32.TryParse(cell.Phrase.ToString(), out number))
                        {
                            if (number % 2 != 0)
                            {
                                colortint = 0.5f;
                            }
                            else
                            {
                                colortint = 0.0f;
                            }
                        }
                    }
                }
                canvases[PdfPTable.BACKGROUNDCANVAS].SetColorFill(new PdfSpotColor("odd", BaseColor.RED), colortint);
            }
        }


        private class TableStyling : IPdfPTableEvent
        {

            private DataTable DT;
            private Font headerfont;
            private PdfPCell celltemplate;

            public TableStyling(DataTable dt, Font HeaderFont, PdfPCell CellTemplate )
            {
                this.DT = dt;
                this.headerfont = HeaderFont;
                this.celltemplate = CellTemplate;
            }

            public void TableLayout(PdfPTable table, float[][] widths, float[] heights, int HeaderRows, int RowStart, PdfContentByte[] Canvases)
            {
                // header row style
                float color = 1.0f;

                int columns = widths[0].Length - 1;

                Rectangle rect = new Rectangle(widths[0][0], heights[0], widths[0][columns], heights[1]);
                rect.GrayFill = color;
                rect.BorderColorBottom = BaseColor.WHITE;
                rect.BorderWidthBottom = 3;
                rect.DisableBorderSide(Rectangle.TOP_BORDER);
                rect.DisableBorderSide(Rectangle.LEFT_BORDER);
                rect.DisableBorderSide(Rectangle.RIGHT_BORDER);
                Canvases[PdfPTable.LINECANVAS].Rectangle(rect);

                columns = widths[HeaderRows].Length - 1;
                color = 0.5f;

                rect = new Rectangle(widths[1][0], heights[1], widths[1][columns], heights[2]);
                rect.GrayFill = color;
                rect.BorderColorBottom = BaseColor.WHITE;
                rect.BorderWidthBottom = 3;
                rect.DisableBorderSide(Rectangle.TOP_BORDER);
                rect.DisableBorderSide(Rectangle.LEFT_BORDER);
                rect.DisableBorderSide(Rectangle.RIGHT_BORDER);
                Canvases[PdfPTable.LINECANVAS].Rectangle(rect);

                color = 0.85f;

                // data rows style
                for (int row = HeaderRows; row < table.Rows.Count; row++ )
                {
                    columns = widths[row].Length-1;
                    rect = new Rectangle(widths[row][0], heights[row], widths[row][columns], heights[row + 1]);
                    rect.GrayFill = color;
                    rect.BorderColorBottom = BaseColor.WHITE;
                    rect.BorderWidthBottom = 1;
                    rect.DisableBorderSide(Rectangle.TOP_BORDER);
                    rect.DisableBorderSide(Rectangle.LEFT_BORDER);
                    rect.DisableBorderSide(Rectangle.RIGHT_BORDER);
                    Canvases[PdfPTable.LINECANVAS].Rectangle(rect);

                }
                
            }

        }


        #region "IDISPOSABLE"

        protected virtual void Dispose(bool disposing)
        {
            if (!_Disposed)
            {
                if (disposing)
                {
                    // Dispose managed resources
                    //
                }
                _Disposed = true;
            }
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        #endregion

    }
}
