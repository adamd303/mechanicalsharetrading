// Free to use, modify, distribute
// No warranty
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace OneDaySwing
{
    public class Program
    {
        public static void Main(string[] args)
        {
            bool debug = false;
            if (debug)
            {
                System.Console.WriteLine("Program Starting...");
            }
            if (debug)
            {
                System.Console.WriteLine("args = " + args.ToString());
            }
            if (args != null)
            {
                if (debug)
                {
                    System.Console.WriteLine("args != null");
                }
                if (args.Length > 0)
                {
                    if (debug)
                    {
                        System.Console.WriteLine("args.Length > 0");
                    }
                    if (args[0] != null)
                    {
                        if (debug)
                        {
                            System.Console.WriteLine("args[0] != null");
                        }
                        string inputFileName = args[0].Trim(' ');
                        if (inputFileName.Length > 0)
                        {
                            if (debug)
                            {
                                System.Console.WriteLine("inputFileName.Length > 0");
                            }
                            string outputFileName = String.Empty;
                            if (args.Length > 1)
                            {
                                if (debug)
                                {
                                    System.Console.WriteLine("args.Length > 1");
                                }
                                if (args[1] != null)
                                {
                                    outputFileName = args[1].Trim(' ');
                                    if (outputFileName.Length <= 0)
                                    {
                                        outputFileName = String.Empty;
                                    }
                                }
                            }
                            OneDaySwingData oneDaySwingData = new OneDaySwingData(inputFileName, outputFileName);
                            oneDaySwingData.CalculateData();
                        }
                        else
                        {
                            System.Console.WriteLine("Empty InputFile name.");
                        }
                    }
                    else
                    {
                        System.Console.WriteLine("Empty InputFile name.");
                    }
                }
                else
                {
                    System.Console.WriteLine("Usage: OneDaySwing InputFile <OutputFile>");
                }
            }
            else
            {
                System.Console.WriteLine("Usage: OneDaySwing InputFile <OutputFile>");
            }
            if (debug)
            {
                System.Console.WriteLine("Program Completed.");
            }
        }
    }
}
