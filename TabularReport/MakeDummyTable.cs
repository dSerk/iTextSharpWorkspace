using System;
using System.Collections.Generic;
using System.Data;
using System.Text;
using System.Threading.Tasks;

namespace TabularReport
{
    public class MakeDummyTable : IDisposable
    {

        private bool _Disposed = false;

        public MakeDummyTable()
        {
        }

        /// <summary>
        /// Returns DataTable of C columns width and R rows length 
        /// populated with random numbers
        /// </summary>
        /// <param name="C">Number of Columns, at least 1</param>
        /// <param name="R">Number of Rows, at least 1</param>
        /// <param name="Name">Arbitrary table name</param>
        /// <returns>DataTable of R*C cells</returns>
        public DataTable RenderDataTable(int C, int R, string Name)
        {
            DataTable temp = new DataTable(Name);

            // at least 1 row
            if (R < 1) R = 1;

            // at least 1 column
            if (C < 1) C = 1;

            // name dummy columns
            for (int i = 1; i < C+1; i++ )
            {
                temp.Columns.Add("COL" + i.ToString("000"), typeof(string));
            }

            // make dummy data
            for (int Row = 1; Row < R; Row++)
            {
                List<object> Fields = new List<object>();
                
                for (int i = 0; i < C; i++ )
                {
                    if (i == 0)
                    {
                        Fields.Add(Row.ToString("#,###"));
                    }
                    else
                    {
                        Fields.Add( ((Row)*(i+1)).ToString("0".PadLeft(i,'0')));
                    }
                }

                temp.Rows.Add(Fields.ToArray());
            }

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
