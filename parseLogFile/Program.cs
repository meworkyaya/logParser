using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using System.Text.RegularExpressions;
using System.Globalization; 

namespace parseLogFile
{
    class Program
    {
        static int Main(string[] args)
        {
            //int result = 0;
            //testRegex();

            // string FileName = @"C:\_work\_projects\PhluantMobile\_staff\logs_2015-02-25\_test\log_short.log";       // short log - part of sec
            // string FileName = @"C:\_work\_projects\PhluantMobile\_staff\logs_2015-02-25\_test\log_midle.log";    // midle log - few secs
            // string FileName = @"C:\_work\_projects\PhluantMobile\_staff\logs_2015-02-25\_test\test.log";         // few mins

            string FileName = @"C:\_work\_projects\PhluantMobile\_staff\logs_2015-02-25\dojo.access.log";        // core src log

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


        static void testRegex() {
            string input = "71.89.33.161 - - [24/Feb/2015:03:19:46 +0000] \"GET /getimg?plId=1922acf5c38981";

            string date_regex = @"\d{2}/[a-zA-Z]{3}/\d{4}:\d{2}:\d{2}:\d{2}";
            Regex rgx = new Regex(date_regex, RegexOptions.IgnoreCase);

            MatchCollection matches = rgx.Matches(input);

            DateTime date;
            if (matches.Count > 0)
            {
                Console.WriteLine("{0} ({1} matches):", input, matches.Count);
                foreach (Match match in matches)
                {
                    Console.WriteLine("   " + match.Value);

                    date = DateTime.ParseExact(match.Value, "dd/MMM/yyyy:HH:mm:ss", CultureInfo.InvariantCulture);
                    Console.WriteLine("   " + date);
                }
            }
            return;
        }
    }
}
