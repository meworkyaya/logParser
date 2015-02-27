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
            string FileName = @"C:\_work\_projects\PhluantMobile\_staff\logs_2015-02-25\_test\log_short.log";       // short log - part of sec
            // string FileName = @"C:\_work\_projects\PhluantMobile\_staff\logs_2015-02-25\_test\log_midle.log";    // midle log - few secs
            // string FileName = @"C:\_work\_projects\PhluantMobile\_staff\logs_2015-02-25\_test\test.log";         // few mins

            // string FileName = @"C:\_work\_projects\PhluantMobile\_staff\logs_2015-02-25\dojo.access.log";        // core src log

            var ReadFile = new ReadBigFile();

            //// make test file from source file
            //var result = ReadFile.MakeTestFile(FileName, FileNameTest, 500000);

            ReadFile.FileName = FileName;
            int result = ReadFile.ProcessFile();

            Console.WriteLine("\n\nResult of run: {0}", result);
            Console.WriteLine("press Enter to exit");
            Console.ReadLine();

            return result;
        }
    }
}
