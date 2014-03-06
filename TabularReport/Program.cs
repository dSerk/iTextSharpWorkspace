using System;
using System.Collections.Generic;
using iTextSharp;
using iTextSharp.text;
using iTextSharp.text.pdf;
using System.Text;
using System.Data;
using System.Threading.Tasks;

namespace TabularReport
{
    class Program
    {
        static void Main(string[] args)
        {

            // introduction
            Console.WriteLine("Build Tabular Report as PDF Document");

            if (args.Length == 1)
            {

                // make dummy data
                DataTable DummyData;

                using (MakeDummyTable MyDummyMaker = new MakeDummyTable())
                {
                    DummyData = MyDummyMaker.RenderDataTable(6, 40, "Dummy Data Table");
                }

                // build PDF
                using (BuildTabularReport MyReportBuilder = new BuildTabularReport())
                {
                    MyReportBuilder.RenderReport(DummyData, args[0]);
                }

            }
            else
            {
                Console.WriteLine("Error: need name of report");
            }


            // pause
            Console.Write("Press ENTER to continue....");
            Console.ReadLine();

        }
    }
}
