using System;
using System.Reflection;
using System.Threading;
using System.Security;
using System.Security.Principal;
using System.Windows;
using System.Windows.Threading;
using Adam.Trading.Mechanical.ViewModel;
using log4net;
[assembly: log4net.Config.XmlConfigurator(Watch = true)]

namespace Adam.Trading.Mechanical.View
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        protected static readonly ILog log = LogManager.GetLogger(typeof(Application));
        private static DispatcherUnhandledExceptionEventHandler appUnhandledevent;
        private static ApplicationViewModel _appViewModel;
        private static MainWindow _mainWindow;
        
        public App()
        {
            log.Info("App constructor Thread.CurrentThread.ManagedThreadId: [" + Thread.CurrentThread.ManagedThreadId.ToString() + "]");
            appUnhandledevent = new DispatcherUnhandledExceptionEventHandler(App_DispatcherUnhandledException);
        }

        /// <summary>
        /// Application Entry Point.
        /// </summary>
        [System.STAThreadAttribute()]
        public static void Main()
        {
            try
            {
                log.Info("Main Thread.CurrentThread.ManagedThreadId: [" + Thread.CurrentThread.ManagedThreadId.ToString() + "]");
                Adam.Trading.Mechanical.View.App app = new Adam.Trading.Mechanical.View.App();
                app.InitializeComponent();
                app.DispatcherUnhandledException += appUnhandledevent;

                app.Run();
            }
            catch (Exception ex)
            {
                log.Fatal("Main Fatal Error: " + ex.ToString());
            }
        }

        void App_Startup(object sender, StartupEventArgs e)
        {
            log.Info("App_Startup Thread.CurrentThread.ManagedThreadId: [" + Thread.CurrentThread.ManagedThreadId.ToString() + "]");
            ViewModelBase.ApplicationDispatcher = App.Current.Dispatcher;
            _appViewModel = new ApplicationViewModel();
            App.Current.Resources["AppViewModel"] = _appViewModel;
            try
            {
                // Get the operating system version.
                OperatingSystem os = Environment.OSVersion;
                Version ver = os.Version;
                log.Info("App_Startup Operating System Version: " + os.VersionString +  "(" + ver.ToString() + ")");
                _appViewModel.OperatingSystem = os.VersionString;
                _appViewModel.OperatingSystemVersion = ver.ToString();

                // Get the common language runtime version.
                Version clrVer = Environment.Version;
                log.Info("App_Startup Common Language Run Time Version: " + clrVer.ToString());
                _appViewModel.CLRVersion = clrVer.ToString();

                // Get the version of the executing assembly (that is, this assembly).
                Assembly assem = Assembly.GetEntryAssembly();
                AssemblyName assemName = assem.GetName();
                string assVer = (assemName.Version.ToString()).Trim(' ');
                log.Info("App_Startup: " + assemName.Name + ", Version: " + assVer + " starting..." );
                _appViewModel.AssemblyName = assemName.Name;
                _appViewModel.AssemblyVersion = assVer;
                // Get the computer and user information
                string userName = String.Empty;
                try
                {
                    userName = WindowsIdentity.GetCurrent().Name;
                }
                catch (SecurityException ex)
                {
                    log.Info("App_Startup Could not obtain user name: " + ex.ToString());
                }
                string computerName = String.Empty;
                try
                {
                    computerName = Environment.MachineName;
                }
                catch (InvalidOperationException ex)
                {
                    log.Info("AppStartup Could not obtain computer name: " + ex.ToString());
                }
                _appViewModel.UserName = userName;
                _appViewModel.ComputerName = computerName;

                // Application is running
                // Code below to show how to process command line args
                // TODO: Add processing of command line arguments and add them to the AppViewModel
                //bool startMinimized = false;
                //for (int i = 0; i != e.Args.Length; ++i)
                //{
                //if (e.Args[i] == "/StartMinimized")
                //{
                //startMinimized = true;
                //}
                //}

                if (_appViewModel != null)
                {
                    _mainWindow = new MainWindow();
                    _mainWindow.DataContext = _appViewModel.MainViewModel;
                    this.MainWindow = _mainWindow;
                    this.MainWindow.Show();
                }

                // TODO: Add processing of command line arguments
                //if (startMinimized)
                //{
                //_mainWindow.WindowState = WindowState.Minimized;
                //}
            }
            catch(Exception ex)
            {
                log.Fatal("App_Startup Fatal Error: " + ex.ToString());
            }
            log.Info("Application_Startup ends...");
        }

        private void App_Exit(object sender, ExitEventArgs e)
        {
            log.Info("Application_Shutdown requested...");
            // TO DO: Implement disposing objects etc. if required.
        }

        private void App_DispatcherUnhandledException(object sender, DispatcherUnhandledExceptionEventArgs e)
        {
            log.Info("App_DispatcherUnhandledException Exception:" + e.Exception.ToString());
            System.Windows.MessageBox.Show(Adam.Trading.Mechanical.View.Properties.Resources.AppUnhandledExceptionMessage,
                                           Adam.Trading.Mechanical.View.Properties.Resources.AppError,
                                           MessageBoxButton.OK, MessageBoxImage.Error);
            e.Handled = true;
        }
    }
 }