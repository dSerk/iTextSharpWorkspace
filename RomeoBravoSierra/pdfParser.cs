using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Threading.Tasks;

namespace RomeoBravoSierra
{
    public class pdfParser : IDisposable
    {

        private StreamReader _SR;
        private int _BufferSize = 4096;
        private bool _Disposed = false;
        private FileInfo _PdfFileInfo;

        public pdfParser(string filepath)
        {
            if (File.Exists(filepath))
            {
                _SR = new StreamReader(filepath, Encoding.Default);
                _PdfFileInfo = new FileInfo(filepath);
            }
            else
            {
                // will this work?
                // if the filepath passed to constructor is bad, then do not construct a pdfParser
                this.Dispose();
            }
        }

        public pdfContents CollectAll()
        {
            pdfContents temp = new pdfContents(_PdfFileInfo.FullName);

            // find EOF
            _SR.BaseStream.Position = _SR.BaseStream.Length - 1;
            byte[] P = new byte[1];
            _SR.Read(P, (int)_SR.Position, 1);

            for (int i = 0; i < 3; i++)
            {
                while (P[0] != 0x10)
                {
                    _SR.Read(P, (int)_SR.Position, 1);
                    _SR.Position -= 1;
                }
            }

            // read cross-reference table offset

            _SR.Position++;
            string rawoffset = "";
            char C = (char)_SR.ReadByte();
            while (char.IsDigit(C))
            {
                rawoffset+=C;
                C=(char)_SR.ReadByte();
            }

            int CrOffset = Convert.ToInt32(rawoffset);

            // read cross-reference table

            _SR.Position = CrOffset;


            // collect each stream and add it to the pdfContents object
            // note id, generation, etc in the name for the stream

            // return when we've collected all streams

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

    /// <summary>
    /// The various content streams in the PDF file
    /// </summary>
    public class pdfContents : Dictionary<string, Stream>
    {
        public string FilePath { get; set; }
        public pdfContents(string filepath) : base() 
        {
            FilePath = filepath;
        }
    }

    /// <summary>
    /// The Cross-reference table of the PDF
    /// </summary>
    public class pdfCrTable
    {
        private class Record
        {
            string Name { get; set; }
            int Offset { get; set; }
            public Record(string name, int offset)
            {
                this.Name = Name;
                this.Offset = Offset;
            }
        }
        private List<Record> _Records;
        private List<string> _Names;

        public pdfCrTable()
        {
            _Records = new List<Record>();
            _Names = new List<string>();
        }

        /// <summary>
        /// Add a record to the table of cross references
        /// </summary>
        /// <param name="Name">Name of the object in the CR table</param>
        /// <param name="Offset">Byte offset from beginning of file</param>
        public void Add(string Name, int Offset)
        {
            if (!_Names.Contains(Name))
            {
                _Names.Add(Name);
            }
            Record temp = new Record(Name, Offset);
            _Records.Add(temp);
        }

    }
}
