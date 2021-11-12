using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace OctaveSearcher
{
    public class Program
    {
        public static void Main(string[] args)
        {
            if (args != null)
            {
                if (args.Length >= 2)
                {
                    if (args[0] != null)
                    {
                        uint length;
                        if (uint.TryParse(args[0], out length))
                        {
                            if (args[1] != null)
                            {
                                string inputFileName = args[1].Trim(' ');
                                if (inputFileName.Length > 0)
                                {
                                    string outputFileName = String.Empty;
                                    if ((args.Length >= 3) && (args[2] != null))
                                    {
                                        outputFileName = args[2].Trim(' ');
                                        if (outputFileName.Length <= 0)
                                        {
                                            outputFileName = String.Empty;
                                        }
                                    }
                                    else
                                    {
                                        outputFileName = String.Empty;
                                    }
                                    Console.WriteLine(length + ", " + inputFileName);
                                    OctaveSearcher octaveSearcher = new OctaveSearcher(length, inputFileName, outputFileName);
                                    octaveSearcher.CalculateData();
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
                            System.Console.WriteLine("Invalid Octave Length");
                        }
                    }
                    else
                    {
                        System.Console.WriteLine("Empty Octave Length.");
                    }
                }
                else
                {
                    System.Console.WriteLine("Usage: OctaveSearcher Length InputFile <OutputFile>");
                }
            }
            else
            {
                System.Console.WriteLine("Usage: OctaveSearcher Length InputFile <OutputFile>");
            }
        }
    }
}
