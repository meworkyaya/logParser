using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using System.IO.MemoryMappedFiles;
using System.IO;
using System.Runtime.InteropServices;

using System.Diagnostics;

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



        public ReadBigFile() : this("")
        {
        }

        public ReadBigFile(string Filename)
        {
            this.FileName = Filename;
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

                int DisplayInfoEachLine = 100;

                var watch = Stopwatch.StartNew();

                // Create an instance of StreamReader to read from a file.
                // The using statement also closes the StreamReader.
                using (StreamReader sr = new StreamReader(FileName))
                {
                    // Read and display lines from the file until the end of 
                    // the file is reached.
                    while ((line = sr.ReadLine()) != null)
                    {
                        count++;
                        // Console.WriteLine("{0}: {1}", count, line);

                        if (count % DisplayInfoEachLine == 0)
                        {
                            Console.WriteLine("Count: {0} : time: {1}", count, watch.ElapsedMilliseconds);

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





    public struct MyColor
    {
        public short Red;
        public short Green;
        public short Blue;
        public short Alpha;

        // Make the view brighter. 
        public void Brighten(short value)
        {
            Red = (short)Math.Min(short.MaxValue, (int)Red + value);
            Green = (short)Math.Min(short.MaxValue, (int)Green + value);
            Blue = (short)Math.Min(short.MaxValue, (int)Blue + value);
            Alpha = (short)Math.Min(short.MaxValue, (int)Alpha + value);
        }
    }
}
