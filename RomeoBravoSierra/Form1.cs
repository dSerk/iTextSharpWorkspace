using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using iTextSharp.text;
using iTextSharp.text.pdf;
using iTextSharp.text.pdf.parser;
using iTextSharp.text.pdf.collection;

namespace RomeoBravoSierra
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }

        #region "INTERFACE EVENTS"

        // DRAG and DROP 
        private void tbLawCase_DragDrop(object sender, DragEventArgs e)
        {
            // allow user to drop a file into the app
            if (e.Data.GetDataPresent(DataFormats.FileDrop))
            {
                // open the file if it's good
                string[] filepaths = (string[])e.Data.GetData(DataFormats.FileDrop);

                if (filepaths.Length > 0)
                {
                    foreach (string filepath in filepaths)
                    {
                        // it's good if it exists
                        if (File.Exists(filepath))
                        {
                            // open the stuff
                            Open(filepath);
                        }
                    }
                }
            }
            else
            {
                // we could allow raw text to be dropped as well....
                this.lblStatus.Text = "Drag && drop failed.";
            }
        }

        // DRAGOVER
        private void tbLawCase_DragOver(object sender, DragEventArgs e)
        {
            if (e.Data.GetDataPresent(DataFormats.FileDrop))
            {
                e.Effect = DragDropEffects.Copy;
            }
            else
            {
                e.Effect = DragDropEffects.None;
            }
        }

        // OPEN
        private void openToolStripMenuItem_Click(object sender, EventArgs e)
        {
            using (OpenFileDialog OFD = new OpenFileDialog())
            {
                OFD.RestoreDirectory = true;
                OFD.Multiselect = true;
                if (OFD.ShowDialog() == System.Windows.Forms.DialogResult.OK)
                {
                    // open everything
                    foreach (string filepath in OFD.FileNames)
                    {
                        Open(filepath);
                    }
                }
            }
        }

        // SAVE
        private void saveToolStripMenuItem_Click(object sender, EventArgs e)
        {
            // save everything to one file
        }

        // QUIT
        private void quitToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Application.Exit();
        }

        // HELP or ABOUT
        private void aboutToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Form_About MyAboutForm = new Form_About();
            MyAboutForm.Show();
        }

        #endregion



        #region "METHODS"

        /// <summary>
        /// 
        /// </summary>
        /// <param name="filepath"></param>
        private void Open(string filepath)
        {
            PdfReader MyPdfReader = new PdfReader(filepath);

            CatalogView(MyPdfReader);

        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="MyPdfReader"></param>
        private void CatalogView(PdfReader MyPdfReader)
        {
            PdfDictionary catalog = MyPdfReader.Catalog;
            //PdfDictionary md = catalog.GetAsDict(PdfName.METADATA);
            //PdfDictionary dests = md.GetAsDict(PdfName.DESTS);
            //PdfArray array = dests.GetAsArray(PdfName.NAMES);

            foreach (var x in catalog.GetAsArray(PdfName.PAGE))
            {
                Console.WriteLine(x.Type.ToString() + "\t" + x.Length.ToString("#,###"));
            }

        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="MyPdfReader"></param>
        private void ShowRawText(PdfReader MyPdfReader)
        {
            StringBuilder SB = new StringBuilder();

            for (int i = 1; i <= MyPdfReader.NumberOfPages; i++)
            {
                SB.AppendLine(" --- Page " + i.ToString("#,###") + " ------------------------------ " + Environment.NewLine);
                SB.AppendLine(PdfTextExtractor.GetTextFromPage(MyPdfReader, i));
                SB.AppendLine();
            }

            this.tbText.AppendText(SB.ToString());
        }

        #endregion


    }
}
