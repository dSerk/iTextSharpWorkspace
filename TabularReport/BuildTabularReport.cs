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

                    Font MyFont = new Font(Font.FontFamily.TIMES_ROMAN, 8.0f, 0, BaseColor.BLACK);

                    // table name
                    Phrase docHeader = new Phrase("Table: " + DT.TableName + Environment.NewLine);
                    doc.Add(docHeader);

                    // filename
                    docHeader = new Phrase("Filename: " + ReportFileName + Environment.NewLine + Environment.NewLine);
                    doc.Add(docHeader);

                    doc.Add(RenderTable(DT));

                    // at this point we need to render an XML file and
                    // embed it in the PDF

                    //PdfEFStream EmbeddedStream = new PdfEFStream(RenderXmlFile(DT), MyPdfWriter);

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
            table.HeaderRows = 1;

            // header
            for (int C = 0; C < DT.Columns.Count; C++)
            {
                string value = DT.Columns[C].ColumnName;          
                table.AddCell(new PdfPCell(new Paragraph(value)));
            }

            // data
            for (int R = 0; R < DT.Rows.Count; R++)
            {
                for (int C = 0; C < DT.Columns.Count; C++)
                {
                    string value = (string)DT.Rows[R][C];
                    table.AddCell(new PdfPCell(new Paragraph(value)));
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
