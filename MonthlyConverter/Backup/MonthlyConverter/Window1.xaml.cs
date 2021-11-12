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

namespace MonthlyConverter
{
    /// <summary>
    /// Interaction logic for Window1.xaml
    /// </summary>
    public partial class Window1 : Window
    {
        #region Class Variables
        private string fileName;                            // The input data file name
        private string resultsFileName = "Output.csv";      // The output file name
        private string minMaxFileName =  "ExtraData.csv";   // The file for the max and min date data
        char delimiterChar = ',';                           // We will only handle csv files
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
                    ShowErrorMessage("Error opening output file " + resultsFileName + ex.ToString());
                    return;
                }
                //
                // Open the min and max data file
                //
                StreamWriter minMaxFile;
                try
                {
                    minMaxFile = File.CreateText(minMaxFileName);
                }
                catch (Exception ex)
                {
                    ShowErrorMessage("Error opening min max data file " + minMaxFileName + ex.ToString());
                    return;
                }


                string code;
                string date;
                string open;
                string high;
                string low;
                string close;
                string volume;
                string yyyymm;
                double currOpen, currHigh;
                double currLow, currClose, prevClose;
                double currVol, monthVol;
                double monthOpen, monthHigh, monthLow, monthClose;
                string currYearMonthCode, prevYearMonthCode;
                string minDate;  // The date for the minimum data point for the month
                string maxDate;  // the date for the maximum data point for the month

                currOpen = 0.0;
                currHigh = 0.0;
                currLow = 0.0;
                currClose = 0.0;
                prevClose = 0.0; ;
                currVol = 0.0;
                monthVol = 0.0;
                monthOpen = 0.0;
                monthHigh = 0.0;
                monthLow = 0.0;
                monthClose = 0.0;
                currYearMonthCode = "000000";
                prevYearMonthCode = "000000";
                minDate = "";
                maxDate = "";

                // Read the first line
                if ((buffer = myFile.ReadLine()) != null)
                {
                    string[] words = buffer.Split(delimiterChar);
                    code = words[0];
                    date = words[1];
                    open = words[2];
                    high = words[3];
                    low = words[4];
                    close = words[5];
                    volume = words[6];
                    yyyymm = words[7];
                    currOpen = System.Convert.ToDouble(open);
                    monthOpen = currOpen;
                    currHigh = System.Convert.ToDouble(high);
                    monthHigh = currHigh;
                    maxDate = date;
                    currLow = System.Convert.ToDouble(low);
                    minDate = date;
                    monthLow = currLow;
                    currClose = System.Convert.ToDouble(close);
                    prevClose = currClose;
                    currVol = System.Convert.ToDouble(volume);
                    monthVol = currVol;
                    currYearMonthCode = yyyymm;
                    prevYearMonthCode = currYearMonthCode;
                }

                while ((buffer = myFile.ReadLine()) != null)
                {
                    string[] words = buffer.Split(delimiterChar);
                    code = words[0];
                    date = words[1];
                    open = words[2];
                    high = words[3];
                    low = words[4];
                    close = words[5];
                    volume = words[6];
                    yyyymm = words[7];
                    currOpen = System.Convert.ToDouble(open);
                    currHigh = System.Convert.ToDouble(high);
                    currLow = System.Convert.ToDouble(low);
                    currClose = System.Convert.ToDouble(close);
                    currVol = System.Convert.ToDouble(volume);
                    currYearMonthCode = yyyymm;
                    if (currYearMonthCode == prevYearMonthCode)
                    {
                        if (currLow < monthLow)
                        {
                            monthLow = currLow;
                            minDate = date;
                        }
                        if (currHigh > monthHigh)
                        {
                            monthHigh = currHigh;
                            maxDate = date;
                        }
                        monthVol += currVol;
                    }
                    else
                    {
                        monthClose = prevClose;
                        //
                        // Output the results
                        //
                        words[0] = prevYearMonthCode.ToString();
                        words[1] = monthOpen.ToString();
                        words[2] = monthHigh.ToString();
                        words[3] = monthLow.ToString();
                        words[4] = monthClose.ToString();
                        words[5] = monthVol.ToString();
                        words[6] = null;
                        words[7] = null;
                        outFile.WriteLine(String.Join(",", words));
                        minMaxFile.WriteLine(words[0] + "," + minDate + "," + monthLow + "," + maxDate + "," + monthHigh);
                        monthOpen = currOpen;
                        monthHigh = currHigh;
                        monthLow = currLow;
                        monthClose = 0.0;
                        monthVol = currVol;
                        minDate = date;
                        maxDate = date;
                    }
                    prevYearMonthCode = currYearMonthCode;
                    prevClose = currClose;
                }
                myFile.Close();
                myFile.Dispose();
                outFile.Flush();
                outFile.Close();
                outFile.Dispose();
                minMaxFile.Flush();
                minMaxFile.Close();
                minMaxFile.Dispose();
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
 
