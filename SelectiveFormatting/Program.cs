using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SelectiveFormatting
{
    class Program
    {
        static void Main(string[] args)
        {


            var a = "100001";
            var b = 100001;
            var f = 100001.06f;
            var c = 'Q';
            var d = 2045622.0002d;

            Console.WriteLine(SelectiveFormat(a) + "\t\t" + a.ToString());
            Console.WriteLine(SelectiveFormat(b) + "\t\t" + b.ToString());
            Console.WriteLine(SelectiveFormat(c) + "\t\t" + c.ToString());
            Console.WriteLine(SelectiveFormat(d) + "\t" + d.ToString());
            Console.WriteLine(SelectiveFormat(f) + "\t" + f.ToString());

            

            Console.Write("Press ENTER to continue....");
            Console.ReadLine();

        }



        public static string SelectiveFormat(Object v)
        {
            string Result = "";

            Type vType = v.GetType();

            Console.Write(vType.ToString() + "\t");

            switch (vType.ToString())
            {
                case "System.String" :
                    Result = (string)v;
                    break;
                case "System.Int32":
                    Result = ((int)v).ToString("#,###");
                    break;
                case "System.Single":
                    Result = ((float)v).ToString("#,###.00000");
                    break;
                case "System.Char":
                    Result = ((char)v).ToString();
                    break;
                case "System.Double":
                    Result = ((double)v).ToString("#,###.#####");
                    break;
            }

            return Result;
        }

    }
}
