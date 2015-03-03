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
    /// <summary>
    /// select format of output result
    /// </summary>
    public enum OutputResultFormatEnum {  text = 1, highcharts = 2 }

    public class ReadBigFile
    {
        // source file name with log data which read
        public string FileName { get; set; }

        // output data file name
        public string OutputFileName { get; set; }

        // output data file name for slow responses
        public string OutputSlowFileName { get; set; }


        // limit counts of rows which will be parsed; if 0 - dont limit at all; if > 0 - limit 
        public long LimitRowsCount { get; set; }

        // output result format selector
        public OutputResultFormatEnum OutputResultFormat { get; set; }

        public string ChartTemplate {get; set;}


        // contains dictionary with results
        Dictionary<string, LogRecord> records = new Dictionary<string, LogRecord>();



        // DONT USED YET
        
        // name of logfile where will be stored info about last parsed data; needed for restoring parsing; dont used
        public string LogFileName { get; set; }

        // current line where we are at file; dont used
        public int CurrentLine { get; set; }

        // current ofsset at file at bytes; dont used
        public int CurrentByteOffset {  get; set; }

        // regex template used for parsing of date/time; dont used; at 10 times longer than indexof parsing
        public string RegexTemplate = @"\d{2}/[a-zA-Z]{3}/\d{4}:\d{2}:\d{2}:\d{2}";

        // regex used for parsgin date
        public Regex DateRegex = null;

        





        public ReadBigFile() : this("")
        {
            LimitRowsCount = 0;
            OutputResultFormat = OutputResultFormatEnum.text;
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
                long breakWork = this.LimitRowsCount;

                String line;

                int DisplayInfoEachLine = 1000;

                var watch = Stopwatch.StartNew();

                DateTime date = new DateTime();
                bool DateIsParsed = false;

                // Dictionary<string, LogRecord> records = new Dictionary<string, LogRecord>();
                // LogRecord[] records = new LogRecord[90000]; // records for seconds

                string DateString = "";
                LogRecord cr = new LogRecord();

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
                        DateIsParsed = ParseDateByString(line, out date, out DateString);

                        if (DateIsParsed)
                        {
                            // if dont have such time yet - add it
                            if ( records.ContainsKey(DateString))
                            {
                            }
                            else 
                            {
                                cr = new LogRecord();
                                cr.FullDate = DateString;
                                records.Add(DateString, cr );
                            }
                            cr = records[DateString];

                            cr.RequestPerSecond++;
                            cr.RequestPerMinute++;

                            records[DateString] = cr;

                        }


                        if (count % DisplayInfoEachLine == 0)
                        {
                            Console.WriteLine("Count: {0} : time: {1}", count, watch.ElapsedMilliseconds);
                            // Console.WriteLine("{0}: {1}", count, line);
                        }

                        if ( (breakWork > 0) && (count > breakWork))
                        {
                            Console.WriteLine("Limit of rows count happens; stopped");
                            break;
                        }
                    }
                }

                watch.Stop();
                var elapsedMs = watch.ElapsedMilliseconds;
                //var elapsedMs = watch.ElapsedMilliseconds;
                Console.WriteLine("elapsed time: {0}", elapsedMs);
                Console.WriteLine("{0}: {1}", count, line);


                OutputResult();

            }
            catch (Exception e)
            {
                // Let the user know what went wrong.
                Console.WriteLine("Some error happens");
                Console.WriteLine(e.Message);
            }

            return 0;
        }








        // process file
        public int ProcessFileRequestTime()
        {
            // check that file exists
            if (!File.Exists(FileName))
            {
                Error("File " + FileName + " dont exists");
                return -1;
            }


            try
            {
                long count = 0;
                long breakWork = this.LimitRowsCount;

                String line;

                int DisplayInfoEachLine = 1000;

                var watch = Stopwatch.StartNew();

                DateTime date = new DateTime();
                bool DateIsParsed = false;
                bool ReqTimeIsParsed = false;

                // Dictionary<string, LogRecord> records = new Dictionary<string, LogRecord>();
                // LogRecord[] records = new LogRecord[90000]; // records for seconds

                string DateString = "";
                LogRecord cr = new LogRecord();
                LogRecord crParsed = new LogRecord();

                string key;


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
                        crParsed = new LogRecord();
                        DateIsParsed = ParseDateByString(line, out date, out DateString);
                        ReqTimeIsParsed = ParseRequestByString(line, ref crParsed);


                        if (DateIsParsed && ReqTimeIsParsed)
                        {
                            key = DateString + "_" + count.ToString();
                            crParsed.FullDate = DateString;

                            records.Add(key, crParsed);
                        }


                        if (count % DisplayInfoEachLine == 0)
                        {
                            Console.WriteLine("Count: {0} : time: {1}", count, watch.ElapsedMilliseconds);
                            // Console.WriteLine("{0}: {1}", count, line);
                        }

                        if ((breakWork > 0) && (count > breakWork))
                        {
                            Console.WriteLine("Limit of rows count happens; stopped");
                            break;
                        }
                    }
                }

                watch.Stop();
                var elapsedMs = watch.ElapsedMilliseconds;
                //var elapsedMs = watch.ElapsedMilliseconds;
                Console.WriteLine("elapsed time: {0}", elapsedMs);
                Console.WriteLine("{0}: {1}", count, line);


                OutputResultReqTime(OutputFileName);

            }
            catch (Exception e)
            {
                // Let the user know what went wrong.
                Console.WriteLine("Some error happens");
                Console.WriteLine(e.Message);
            }

            return 0;
        }











        public void OutputResult()
        {
            switch (OutputResultFormat)
            {
                default:
                case OutputResultFormatEnum.text:
                    OutputResultText(OutputFileName);
                    break;
                case OutputResultFormatEnum.highcharts:
                    OutputResultHighCharts(OutputFileName);
                    break;
            }
        }



        public bool OutputResultText(string FileName)
        {
            using (System.IO.StreamWriter file = new System.IO.StreamWriter(FileName))
            {
                foreach (KeyValuePair<string, LogRecord> entry in records)
                {
                    // do something with entry.Value or entry.Key
                    file.WriteLine("{0} :: {1} :: {2}", entry.Value.FullDate, entry.Value.RequestPerSecond, getColumn(entry.Value.RequestPerSecond));
                }
            }
            return true;
        }




        public bool OutputResultReqTime(string FileName)
        {
            using (System.IO.StreamWriter file = new System.IO.StreamWriter(FileName))
            {
                foreach (KeyValuePair<string, LogRecord> entry in records)
                {
                    // do something with entry.Value or entry.Key
                    file.WriteLine("{0} :: {1} :: {2} :: {3}", 
                            entry.Value.FullDate, 
                            entry.Value.Request.PadLeft(10), 
                            entry.Value.RequestTime, 
                            getColumn( (int)(5 * 1000 * double.Parse(entry.Value.RequestTime)) )
                            // 1000 * double.Parse(entry.Value.RequestTime)
                    );
                }
            }


            // write slow items
            using (System.IO.StreamWriter file = new System.IO.StreamWriter(OutputSlowFileName))
            {
                double SlowTime = 0.050;
                double ReqTime = 0;
                foreach (KeyValuePair<string, LogRecord> entry in records)
                {
                    ReqTime = double.Parse(entry.Value.RequestTime);
                    if (ReqTime <= SlowTime) { 
                        continue; 
                    }

                    // do something with entry.Value or entry.Key
                    file.WriteLine("{0} :: {1} :: {2} :: {3}",
                            entry.Value.FullDate,
                            entry.Value.Request.PadLeft(10),
                            entry.Value.RequestTime,
                            getColumn((int)(5 * 1000 * double.Parse(entry.Value.RequestTime)))
                        // 1000 * double.Parse(entry.Value.RequestTime)
                    );
                }
            }


            return true;
        }




        public bool OutputResultHighCharts(string FileName)
        {
            bool HaveTemplate = File.Exists(ChartTemplate);
            if (!HaveTemplate)
            {
                Error("Cant find chart Template file; data will be written to text file ");
            }

            string Data = MakeDataForChartRps();    // prepare data for charts
            string Output;

            if (HaveTemplate)
            {
                Output = MakeChartFromTemplate(ChartTemplate, Data);
            }
            else
            {
                Output = Data;
            }


            using (System.IO.StreamWriter file = new System.IO.StreamWriter(FileName))
            {
                file.WriteLine(Output);
            }

            return true;
        }



        public string MakeChartFromTemplate(string template, string data)
        {
            string result = File.ReadAllText( template);
            result = result.Replace( "###data###", data );
            return result;
        }





        public string MakeDataForChartRps()
        {
            string[] sbData = new string[records.Count];
            string Spacer = ", ";

            int i = 0;
            foreach (KeyValuePair<string, LogRecord> entry in records)
            {
                sbData[i] = entry.Value.RequestPerSecond.ToString();
                i++;

                // do something with entry.Value or entry.Key
                // file.WriteLine("{0} :: {1} :: {2}", entry.Value.FullDate, entry.Value.RequestPerSecond, getColumn(entry.Value.RequestPerSecond));
                // sbData.Append(entry.Value.RequestPerSecond).Append(Spacer);
            }
            string Data = String.Join(Spacer, sbData);

            return Data;

        }





        public string getColumn(int count)
        {
            char ch = '=';
            StringBuilder sb = new StringBuilder();
            sb.Append(ch, (int)(0.5 + count / 5));

            return sb.ToString();
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
        public bool ParseDateByString(string Line, out DateTime date, out string  DateString )
        {
            int pos_1 = Line.IndexOf('[');
            if (!(pos_1 > 0))
            {
                DateString = "";
                date = new DateTime();
                return false;
            }

            int pos_2 = Line.IndexOf(' ', pos_1 + 1);
            if (!(pos_2 > 0))
            {
                DateString = "";
                date = new DateTime();
                return false;
            }

            
            try
            {
                DateString = Line.Substring(pos_1 + 1, pos_2 - pos_1 - 1);
                date = DateTime.ParseExact(DateString, "dd/MMM/yyyy:HH:mm:ss", CultureInfo.InvariantCulture);

                return true;
            }
            catch (Exception ex)
            {
                DateString = "";
                date = new DateTime();
                return false;
            }
        }




        /// <summary>
        /// timing: 50 000 lines - 408 ms. At 10 times faster than regexp )
        /// </summary>
        /// <param name="Line"></param>
        /// <param name="date"></param>
        /// <returns></returns>
        public bool ParseRequestByString(string Line, ref LogRecord cr)
        {
            int subResultLength = 3;
            int RequestGetLength = 30;

            int pos_1 = Line.IndexOf('"');  // first "
            if (!(pos_1 > 0))
            {
                goto Error;
            }

            int pos_2 = Line.IndexOf('"', pos_1 + 1);   // second "
            if (!(pos_2 > 0))
            {
                goto Error;
            }

            int pos_3 = Line.IndexOf('"', pos_2 + 1);   // third "
            if (!(pos_3 > 0))
            {
                goto Error;
            }

            string subResult = Line.Substring(pos_2 + 2, pos_3 - pos_2 - 2 - 1);    // we eat at start: '" '; eat at end: ' "'
            string[] items = subResult.Split(' ');

            if (items.Length < subResultLength)
            {
                goto Error;
            }

            cr.Request = Line.Substring(pos_1 + 1, RequestGetLength); 
            cr.RequestStatus    = items[0];
            cr.RequestTime      = items[2];


            return true;

            // if error parsing - return empty data - dont break parsing
            Error:
                return false;
                
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

        public string Request;
        public string RequestStatus;
        public string RequestTime;

    }


}
