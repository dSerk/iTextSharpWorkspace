using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using iTextSharp.text;
using iTextSharp.text.pdf;

namespace PdfHasComments
{
    public class Processor : IDisposable
    {

        private bool _Disposed = false;
        private string SearchPath;
        private PdfReader MyReader;

        public Processor(string path)
        {
            SearchPath = path;
        }

        public void FindPopup()
        {
            GetFiles(new DirectoryInfo(SearchPath));
        }

        private void GetFiles(DirectoryInfo Dir)
        {
            try
            {
                foreach (FileInfo Fi in Dir.GetFiles("*.PDF"))
                {
                    ScanPdf(Fi.FullName);
                }
            }
            catch (Exception Ex)
            {
                Console.WriteLine(Ex.Message + "\t" + Dir.FullName);
            }

            foreach (DirectoryInfo SubDir in Dir.GetDirectories())
            {
                try
                {
                    GetFiles(SubDir); // RECURSE!
                }
                catch (Exception Ex)
                {
                    Console.WriteLine(Ex.Message + "\t" + Dir.FullName);
                }
            }
        }

        public void ScanPdf(string Fullpath)
        {
            try
            {
                MyReader = new PdfReader(Fullpath);
            }
            catch (Exception Ex)
            {
                Console.WriteLine(Ex.Message + "\t" + Fullpath);
                return;
            }

            int CountAnnots = ScanLinearizedPdf();

            if (CountAnnots > 0)
            {
                Console.WriteLine("{1,-6} {0}",
                    Fullpath,
                    CountAnnots.ToString("#,##0")
                    );
            }
        }

        /// <summary>
        /// This method will look for markup annotations on EVERY page
        /// </summary>
        /// <returns>Count of apparent markup annotations</returns>
        private int ScanLinearizedPdf()
        {
            int CountAnnots = 0;

            for (int P = 1; P < MyReader.NumberOfPages; P++)
            {
                PdfDictionary Next = MyReader.GetPageN(P);
                PdfArray annotArray = Next.GetAsArray(PdfName.ANNOTS);

                if (annotArray != null)
                {
                    for (int i = 0; i < annotArray.Size - 1; i++)
                    {
                        PdfDictionary NextAnnot = annotArray.GetAsDict(i);
                        if (NextAnnot.Contains(PdfName.POPUP)
                            || (NextAnnot.Contains(PdfName.T) && NextAnnot.Contains(PdfName.POPUP))
                            || NextAnnot.Contains(PdfName.INTENT)
                            )
                        {
                            CountAnnots++;
                        }
                    }
                }
            }

            return CountAnnots;
        }

        public void Explore(string Fullpath)
        {
            Console.WriteLine("Processing {0}", Path.GetFileNameWithoutExtension(Fullpath));
            MyReader = new PdfReader(Fullpath);        
            for (int P = 1; P < MyReader.NumberOfPages; P++)
            {
                PdfDictionary Next = MyReader.GetPageN(P);
                PdfArray annotArray = Next.GetAsArray(PdfName.ANNOTS);
                for (int i = 0; i < annotArray.Size-1; i++)
                {
                    PdfDictionary NextAnnot = annotArray.GetAsDict(i);
                    foreach (KeyValuePair<PdfName, PdfObject> kvp in NextAnnot )
                    {
                        Console.WriteLine("{2,-5} {0,-25} {1}", 
                            kvp.Key.ToString(), 
                            kvp.Value.ToString(), 
                            (kvp.Key.ToString() == PdfName.POPUP.ToString() ? "---->" : "")
                            );
                    }
                    Console.WriteLine();
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
