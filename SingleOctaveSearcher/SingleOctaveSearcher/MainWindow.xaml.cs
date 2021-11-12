using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace SingleOctaveSearcher
{
	/// <summary>
	/// Interaction logic for MainWindow.xaml
	/// </summary>
	public partial class MainWindow : Window
	{
		#region Class Variables
		private double _length;         // The octave length to search for
        private uint _repeats;        // The length increment for octave
		private double _orb;            // The orb
		private string _fileName;     // The input data file name
        #endregion

		public MainWindow()
		{
			InitializeComponent();
			_lengthValue.Text = "720";
            _repeatsValue.Text = "0";
			_orbValue.Text = "5.0";
			_inputFileSelector.txtFileName.Text="Please select a .csv file.";
		}
		
		# region Event Handler Methods
		/// <summary>
		/// The calculate button has been clicked.
		/// If we have a valid input number, perform the conversion.
		/// </summary>
		private void btnCalculate_Click(object sender, System.Windows.RoutedEventArgs e)
        {
			if (Validate()) 
			{
                OctaveSearcher octaveSearcher = new OctaveSearcher(_length, _repeats, _orb, _fileName);
                octaveSearcher.CalculateData();
			}
			// Error message boxes are generated in the Validate method
        }
		#endregion
		
        #region Helper Methods
        private bool Validate()
		{
        	if ( _lengthValue.Text.Trim(' ') == null )
        	{
        		ShowErrorMessage("No length entered.");
        		return false;
        	}
        	if ( _lengthValue.Text.Trim(' ').Length < 1 )
        	{
        		ShowErrorMessage("No length entered.");
        		return false;
        	}
            if (_repeatsValue.Text.Trim(' ') == null)
            {
                ShowErrorMessage("No repeats entered.");
                return false;
            }
            if (_repeatsValue.Text.Trim(' ').Length < 1)
            {
                ShowErrorMessage("No repeats entered.");
                return false;
            }
        	if ( _orbValue.Text.Trim(' ') == null )
        	{
        		ShowErrorMessage("No orb entered.");
        		return false;
        	}
        	if ( _orbValue.Text.Trim(' ').Length < 1 )
        	{
        		ShowErrorMessage("No orb entered.");
        		return false;
        	}
        	double intLengthValue = 0;
        	double intOrbValue = 0;
            uint intRepeatsValue = 0;
        	try
        	{
        	    intLengthValue = System.Convert.ToDouble(_lengthValue.Text);
        	    intOrbValue = System.Convert.ToDouble(_orbValue.Text);
                intRepeatsValue = System.Convert.ToUInt32(_repeatsValue.Text);
        	}
        	catch
        	{
        		ShowErrorMessage("Invalid integer entered.");
        		return false;
        	}
        	if ( intLengthValue < 1 )
        	{
        		ShowErrorMessage("Length is too small.");
        		return false;
        	}
        	if ( intOrbValue < 1 )
        	{
        		ShowErrorMessage("Orb is too small.");
        		return false;
        	}
        	if ( _inputFileSelector.txtFileName.Text.Trim(' ') == null )
        	{
        		ShowErrorMessage("No input file selected.");
        		return false;
        	}
        	if ( _inputFileSelector.txtFileName.Text.Trim(' ').Length < 1 )
        	{
                ShowErrorMessage("No input file selected.");
        		return false;
        	}    
            _length = intLengthValue;
		    _orb = intOrbValue;
            _repeats = intRepeatsValue;
		    _fileName = _inputFileSelector.txtFileName.Text.Trim(' ');
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
        #endregion
	}
}
