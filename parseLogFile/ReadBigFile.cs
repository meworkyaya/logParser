using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using System.IO.MemoryMappedFiles;
using System.IO;
using System.Runtime.InteropServices;

using System.Diagnostics;

using System.Text.RegularExpressions;
using System.Globalization; 


namespace parseLogFile
{
    public class ReadBigFile
    {
        // file name which read
        public string FileName { get; set; }
        
        // name of logfile where will be stored info about last parsed data; needed for restoring parsing
        public string LogFileName { get; set; }

        // current line where we are at file
        public int CurrentLine { get; set; }

        // current ofsset at file at bytes
        public int CurrentByteOffset {  get; set; }

        // regex template used for parsing
        public string RegexTemplate = @"\d{2}/[a-zA-Z]{3}/\d{4}:\d{2}:\d{2}:\d{2}";

        // regex used for parsgin date
        public Regex DateRegex = null;



        public ReadBigFile() : this("")
        {
        }

        public ReadBigFile(string Filename)
        {
            FileName = Filename;

            DateRegex = new Regex( RegexTemplate, RegexOptions.IgnoreCase);
        }

        // get line from file
        public string GetLine(int LineNumber, int OffsetLineNumber = 0)
        {
            string Line = "";
            return Line;
        }
        public string GetNextLine()
        {
            string Line = "";
            return Line;
        }


        public void Error(string Message)
        {
            Console.WriteLine(Message);

        }

        // process file
        public int ProcessFile()
        {
            // check that file exists
            if (!File.Exists(FileName))
            {
                Error("File " + FileName + " dont exists" );
                return -1;
            }


            try
            {
                long count = 0;
                long breakWork = 1000000;

                String line;

                int DisplayInfoEachLine = 1000;

                var watch = Stopwatch.StartNew();

                DateTime date = new DateTime();
                bool DateIsParsed = false;

                // Create an instance of StreamReader to read from a file.
                // The using statement also closes the StreamReader.
                using (StreamReader sr = new StreamReader(FileName))
                {
                    // Read and display lines from the file until the end of 
                    // the file is reached.
                    while ((line = sr.ReadLine()) != null)
                    {
                        count++;

                        // DateIsParsed = ParseDate(line, out date);    // regexp version - at 10 time slonger than string + date.parse version
                        DateIsParsed = ParseDateByString(line, out date);

                        if (count % DisplayInfoEachLine == 0)
                        {
                            Console.WriteLine("Count: {0} : time: {1}", count, watch.ElapsedMilliseconds);
                            // Console.WriteLine("{0}: {1}", count, line);
                        }

                        if (count > breakWork)
                        {
                            break;
                        }
                    }
                }

                watch.Stop();
                var elapsedMs = watch.ElapsedMilliseconds;
                //var elapsedMs = watch.ElapsedMilliseconds;
                Console.WriteLine("elapsed time: {0}", elapsedMs);
                Console.WriteLine("{0}: {1}", count, line);

                

            }
            catch (Exception e)
            {
                // Let the user know what went wrong.
                Console.WriteLine("The file could not be read:");
                Console.WriteLine(e.Message);
            }

            return 0;
        }


        /// <summary>
        /// parse line date by regexp; timing: 50000 records - 4s 500 ms
        /// </summary>
        /// <param name="Line"></param>
        /// <param name="date"></param>
        /// <returns></returns>
        public bool ParseDate(string Line, out DateTime date)
        {
            bool result = false;
            MatchCollection matches = this.DateRegex.Matches(Line);

            if ( matches.Count  > 0  )
            {
                date = DateTime.ParseExact(matches[0].Value, "dd/MMM/yyyy:HH:mm:ss", CultureInfo.InvariantCulture);

                // date = new DateTime();
                result = true;
            }
            else
            {
                date = new DateTime();
            }
            return result;
        }



        /// <summary>
        /// timing: 50 000 lines - 408 ms. At 10 times faster than regexp )
        /// </summary>
        /// <param name="Line"></param>
        /// <param name="date"></param>
        /// <returns></returns>
        public bool ParseDateByString(string Line, out DateTime date)
        {
            int pos_1 = Line.IndexOf('[');
            if (!(pos_1 > 0))
            {
                date = new DateTime();
                return false;
            }

            int pos_2 = Line.IndexOf(' ', pos_1 + 1);
            if (!(pos_2 > 0))
            {
                date = new DateTime();
                return false;
            }

            
            try
            {
                string subString = Line.Substring(pos_1 + 1, pos_2 - pos_1 - 1);
                date = DateTime.ParseExact(subString, "dd/MMM/yyyy:HH:mm:ss", CultureInfo.InvariantCulture);

                return true;
            }
            catch (Exception ex)
            {
                date = new DateTime();
                return false;
            }
        }





        #region MakeTestFile definition

        // get source file and copy LinesCount from it to Out file
        public int MakeTestFile( string FileNameInput, string FileNameOut, int LinesCount)
        {
            // check that file exists
            if (!File.Exists(FileNameInput))
            {
                Error("File " + FileNameInput + " dont exists");
                return -1;
            }


            try
            {
                long count = 0;

                String line;
                string[] lines = new string[LinesCount];

                var watch = Stopwatch.StartNew();

                // Create an instance of StreamReader to read from a file.
                // The using statement also closes the StreamReader.
                using (StreamReader sr = new StreamReader(FileNameInput))
                {
                    // Read and display lines from the file until the end of 
                    // the file is reached.
                    while ((line = sr.ReadLine()) != null)
                    {
                        lines[count] = line;
                        count++;

                        // Console.WriteLine("{0}: {1}", count, line);

                        if (count >= LinesCount)
                        {
                            break;
                        }
                    }
                }

                watch.Stop();
                var elapsedMs = watch.ElapsedMilliseconds;
                Console.WriteLine("elapsed time: {0}", elapsedMs);
                Console.WriteLine("{0}: {1}", count, line);

                System.IO.File.WriteAllLines( FileNameOut, lines);
                Console.WriteLine("File {0} writed", FileNameOut );
            }
            catch (Exception e)
            {
                // Let the user know what went wrong.
                Console.WriteLine("The file could not be read:");
                Console.WriteLine(e.Message);
            }

            return 0;
        }
        #endregion MakeTestFile definition


    }




    public struct LogRecord
    {
        public int LineNumber;
        public string FullDate;

        public string Minute;
        public string Second;
        public int RequestPerSecond;
        public int RequestPerMinute;

    }


}
