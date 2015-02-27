using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace parseLogFile
{
    class Program
    {
        static int Main(string[] args)
        {
            string FileNameTest = @"C:\_work\_projects\PhluantMobile\_staff\logs_2015-02-25\_test\test.log";
            string FileName = @"C:\_work\_projects\PhluantMobile\_staff\logs_2015-02-25\dojo.access.log";

            var ReadFile = new ReadBigFile();

            // make test file from source file
            var result = ReadFile.MakeTestFile(FileName, FileNameTest, 500000);

            // ReadFile.FileName = FileName;
            // int result = ReadFile.ProcessFile();

            Console.WriteLine("\n\nResult of run: {0}", result);
            Console.WriteLine("press Enter to exit");
            Console.ReadLine();

            return result;
        }
    }
}
