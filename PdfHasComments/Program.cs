using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Threading.Tasks;

namespace PdfHasComments
{
    class Program
    {
        static void Main(string[] args)
        {

            Console.WriteLine("PdfHasComments -- exploring Pdf Markup Annotations");
            Console.BufferWidth = 300;
            Console.BufferHeight = 9000;

            if (args.Length == 1)
            {
                if (Directory.Exists(args[0]))
                {
                    // engage the itext API and look for annotations, comments
                    using (Processor MyProc = new Processor(args[0]))
                    {
                        MyProc.FindPopup();
                    }
                }
                else
                {
                    Console.WriteLine("{0} does not exist.", args[0]);
                }
                
            }
            else
            {
                Console.WriteLine("PdfHasComments needs path as argument.");
            }

            Console.Write("Press ENTER to continue....");
            Console.ReadLine();

        }
    }
}
