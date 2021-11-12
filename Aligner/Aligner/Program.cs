// Free to use, modify, distribute
// No warranty
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Aligner
{
    public class Program
    {
        public static void Main(string[] args)
        {
            bool debug = true;
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
                if (args.Length >=3)
                {
                    if (debug)
                    {
                        System.Console.WriteLine("args.Length >= 3");
                    }
                    if ((args[0] != null) && (args[1] != null) && (args[2] != null))
                    {
                        if (debug)
                        {
                            System.Console.WriteLine("args[0] != null && args[1] != null && args[2] != null");
                        }
                        string inputFileName = args[0].Trim(' ');
                        decimal startPoint;
                        decimal increment;
                        bool validStartPoint = decimal.TryParse(args[1], out startPoint);
                        if (validStartPoint)
                        {
                            if (!(startPoint >= 0.0M))
                            {
                                validStartPoint = false;
                            }
                        }
                        bool validIncrement = decimal.TryParse(args[2], out increment);
                        if (validIncrement)
                        {
                            if (!((increment >= 0.0M)) && (increment <= 30.0M))
                            {
                                validIncrement = false;
                            }
                        }
                        if ((inputFileName.Length > 0) && validStartPoint && validIncrement)
                        {
                            if (debug)
                            {
                                System.Console.WriteLine("(inputFileName.Length > 0) && validStartPoint && validIncrement");
                            }
                            string outputFileName = String.Empty;
                            if (args.Length > 3)
                            {
                                if (debug)
                                {
                                    System.Console.WriteLine("args.Length > 3");
                                }
                                if (args[3] != null)
                                {
                                    outputFileName = args[3].Trim(' ');
                                    if (outputFileName.Length <= 0)
                                    {
                                        outputFileName = string.Empty;
                                    }
                                }
                            }
                            if (debug)
                            {
                                System.Console.WriteLine("About to call aligner...");
                            }
                            Aligner aligner = new Aligner(inputFileName, startPoint, increment, outputFileName);
                            aligner.CalculateData();
                        }
                        else
                        {
                            System.Console.WriteLine("Empty InputFile name or invalid start point or invalid increment.");
                        }
                    }
                    else
                    {
                        System.Console.WriteLine("Empty InputFile name or empty start point or empty increment.");
                    }
                }
                else
                {
                    System.Console.WriteLine("Please specify the correct number of input parameters");
                }
            }
            else
            {
                System.Console.WriteLine("Usage: Aligner InputFile StartPoint Increment <OutputFile>");
            }
            if (debug)
            {
                System.Console.WriteLine("Program Completed.");
            }
        }
    }
}
