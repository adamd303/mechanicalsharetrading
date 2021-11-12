// The well known MVVM Relay Command Class
using System;
using System.Diagnostics;
using System.Text;
using System.Windows.Input;

namespace Adam.Trading.Mechanical.ViewModel.Command
{

	/// <summary>
	///		The RelayCommand class is an ICommand that makes it so that
	///		it's not necessary to define a different ICommand subclass
	///		for each command.
	/// </summary>
	/// <remarks>
	///		This class comes from an MSDN article, and demo app, from Josh Smith,
	///		titled "WPF Apps With The Model-View-ViewModel Design Pattern",
	///		at http://msdn.microsoft.com/en-us/magazine/dd419663.aspx.
	///		To use this, add a private member in the ViewModel, like
	///		"private RelayCommand _saveCommand;", in the view's XAML,
	///		bind the command to a control like
	///		'Command="{Binding Path=SaveCommand}"', and in the ViewModel,
	///		add a property like:
	///		
	///			public ICommand SaveCommand
	///			{
	///				get
	///				{
	///					if (_saveCommand == null)
	///					{
	///						_saveCommand = new RelayCommand(
	///							param => this.Save(),
	///							param => this.CanSave );
	///					}
	///					return _saveCommand;
	///				}
	///			}
	///			
	/// </remarks>
	/// 
	public class RelayCommand : ICommand
	{
		#region Fields

		readonly Action<object>
			_execute;
		readonly Predicate<object>
			_canExecute;

		#endregion // Fields


		#region Constructors

		public RelayCommand( Action<object> execute )
			: this( execute, null )
		{
		}

		public RelayCommand(
			Action<object>
				execute,
			Predicate<object>
				canExecute )
		{
			if ( execute == null )
				throw new ArgumentNullException( "execute" );

			_execute = execute;
			_canExecute = canExecute;
		}
		#endregion // Constructors


		#region ICommand Members

		[DebuggerStepThrough]
		public bool CanExecute(
			object
				parameter )
		{
			return _canExecute == null ? true : _canExecute( parameter );
		}

		public event EventHandler CanExecuteChanged
		{
			add { CommandManager.RequerySuggested += value; }
			remove { CommandManager.RequerySuggested -= value; }
		}

		public void Execute(
			object
				parameter )
		{
			_execute( parameter );
		}

		#endregion // ICommand Members
	}
}

