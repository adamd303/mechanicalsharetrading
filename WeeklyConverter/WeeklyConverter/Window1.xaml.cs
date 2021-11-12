using System;
using System.IO;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using System.Globalization;

namespace WeeklyConverter
{
    /// <summary>
    /// Interaction logic for Window1.xaml
    /// </summary>
    public partial class Window1 : Window
    {
        #region Class Variables
        private string fileName;                          // The input data file name
        private string resultsFileName = "Output.csv";    // The output file name
        char delimiterChar = ',';                         // We will only handle csv files
        #endregion
        public Window1()
        {
            InitializeComponent();
            InputFileSelector.txtFileName.Text="Please select a .csv file.";
        }
        
        # region Event Handler Methods
        /// <summary>
        /// The Charting Button has been clicked.
        /// If we have a valid input file, plot the chart from the file data.
        /// </summary>
        private void ProcessFile_Click(object sender, System.Windows.RoutedEventArgs e)
        {
            if (Validate())
            {
                //
                // Only support csv files with no 
                // header at the moment
                // 
                String buffer;

                StreamReader myFile;
                try
                {
                    myFile = File.OpenText(fileName);
                }
                catch (Exception ex)
                {
                    ShowErrorMessage("Error opening input file " + fileName + ex.ToString());
                    return;
                }
                //
                // Open the output file
                //
                StreamWriter outFile;
                try
                {
                    outFile = File.CreateText(resultsFileName);
                }
                catch (Exception ex)
                {
                    ShowErrorMessage("Error opening error log file " + resultsFileName + ex.ToString());
                    return;
                }

                string date;
                string open;
                string high;
                string low;
                string close;
                string volume;
                string weekenddate;
                double currOpen, currHigh;
                double currLow, currClose, prevClose;
                double currVol, weekVol;
                double weekOpen, weekHigh, weekLow, weekClose;
                string currWeekEndDate, prevWeekEndDate;

                currOpen = 0.0;
                currHigh = 0.0;
                currLow = 0.0;
                currClose = 0.0;
                prevClose = 0.0; ;
                currVol = 0.0;
                weekVol = 0.0;
                weekOpen = 0.0;
                weekHigh = 0.0;
                weekLow = 0.0;
                weekClose = 0.0;
                currWeekEndDate = "000000";
                prevWeekEndDate = "000000";
                double max, min;

                // Read the first line
                if ((buffer = myFile.ReadLine()) != null)
                {
                    string[] words = buffer.Split(delimiterChar);
                    date = words[0];
                    open = words[1];
                    high = words[2];
                    low = words[3];
                    close = words[4];
                    volume = words[5];
                    weekenddate = words[6];
                    currOpen = System.Convert.ToDouble(open);
                    currClose = System.Convert.ToDouble(close);
                    weekOpen = currOpen;
                    prevClose = currClose;
                    max = Math.Max(currClose, currOpen);
                    min = Math.Min(currClose, currOpen);
                    if (high == "")
                    {
                        currHigh = max;
                    }
                    else
                    {
                        currHigh = System.Convert.ToDouble(high);
                    }
                    weekHigh = currHigh;
                    if (low == "")
                    {
                        currLow = min;
                    }
                    else
                    {
                        currLow = System.Convert.ToDouble(low);
                    }
                    weekLow = currLow;
                    currVol = System.Convert.ToDouble(volume);
                    weekVol = currVol;
                    currWeekEndDate = weekenddate;
                    prevWeekEndDate = currWeekEndDate;
                }

                while ((buffer = myFile.ReadLine()) != null)
                {
                    string[] words = buffer.Split(delimiterChar);
                    date = words[0];
                    open = words[1];
                    high = words[2];
                    low = words[3];
                    close = words[4];
                    volume = words[5];
                    weekenddate = words[6];
                    currOpen = System.Convert.ToDouble(open);
                    currClose = System.Convert.ToDouble(close);
                    max = Math.Max(currClose, currOpen);
                    min = Math.Min(currClose, currOpen);
                    if (high == "")
                    {
                        currHigh = max;
                    }
                    else
                    {
                        currHigh = System.Convert.ToDouble(high);
                    }
                    if (low == "")
                    {
                        currLow = min;
                    }
                    else
                    {
                        currLow = System.Convert.ToDouble(low);
                    }
                    currVol = System.Convert.ToDouble(volume);
                    currWeekEndDate = weekenddate;
                    if (currWeekEndDate == prevWeekEndDate)
                    {
                        if (currLow < weekLow) weekLow = currLow;
                        if (currHigh > weekHigh) weekHigh = currHigh;
                        weekVol += currVol;
                    }
                    else
                    {
                        weekClose = prevClose;
                        //
                        // Output the results
                        //
                        words[0] = prevWeekEndDate.ToString();
                        words[1] = weekOpen.ToString();
                        words[2] = weekHigh.ToString();
                        words[3] = weekLow.ToString();
                        words[4] = weekClose.ToString();
                        words[5] = weekVol.ToString();
                        words[6] = null;
                        outFile.WriteLine(String.Join(",", words));
                        if (currOpen - prevClose < 0.0001)
                        {
                            // For the early data on All Ordinaries
                            // the current open = previous close 
                            // and there are no lows or highs 
                            // only closes for each day
                            // For the first day of a new week
                            weekOpen = currClose;
                            weekHigh = currHigh;
                            weekLow = currLow;
                            weekClose = 0.0;
                            weekVol = currVol;
                        }
                        else
                        {
                            weekOpen = currOpen;
                            weekHigh = currHigh;
                            weekLow = currLow;
                            weekClose = 0.0;
                            weekVol = currVol;
                        }
                    }
                    prevWeekEndDate = currWeekEndDate;
                    prevClose = currClose;
                }
                myFile.Close();
                myFile.Dispose();
                outFile.Flush();
                outFile.Close();
                outFile.Dispose();
            }
        }
        #endregion
        
        #region Helper Methods
        /// <summary>
        /// Make sure that we have a valid file name.
        /// </summary>
        /// <returns>True if we have a valid file name, false otherwise.</returns>
        private bool Validate()
        {
            if (InputFileSelector.txtFileName.Text.Trim(' ') == null)
            {
                ShowErrorMessage("The filename is empty!");
                return false;
            }
            if (InputFileSelector.txtFileName.Text.Trim(' ').Length < 1)
            {
                ShowErrorMessage("The filename is blank!");
                return false;
            }
            this.fileName = InputFileSelector.txtFileName.Text.Trim(' ');
            return true;
        }

        /// <summary>
        /// Show error messages as System.Windows.MessageBox(es)
        /// </summary>
        /// <param name="Message">The error message string.</param>
        private void ShowErrorMessage(string Message)
        {
            MessageBox.Show(Message,
                            "Error",
                            MessageBoxButton.OK,
                            MessageBoxImage.Error,
                            MessageBoxResult.OK,
                            MessageBoxOptions.None);
        }

        /// <summary>
        /// Show messages as System.Windows.MessageBox(es)
        /// </summary>
        /// <param name="Message">The error message string.</param>
        private void ShowSuccessMessage(string Message)
        {
            MessageBox.Show(Message,
                            "Success",
                            MessageBoxButton.OK,
                            MessageBoxImage.Exclamation,
                            MessageBoxResult.OK,
                            MessageBoxOptions.None);
        }        
        #endregion
    }
}
 
