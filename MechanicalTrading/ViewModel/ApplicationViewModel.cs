// Copyright (c) 2012, Adam Duval Kangaroo Point South 4169, Australia.
// All Rights Reserved.
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Adam.Trading.Mechanical.ViewModel
{
    public class ApplicationViewModel : ViewModelBase
    {
        private static string _userName = String.Empty;           // The user for the PC
        private static string _computerName = String.Empty;       // The PC name
        private static string _operatingSystem = String.Empty;    // The operating system name
        private static string _osVersion = String.Empty;          // The operating system version
        private static string _assemName = String.Empty;          // The assembly name
        private static string _assemVersion = String.Empty;       // The assembly version
        private static string _clrVersion = String.Empty;         // The CLR version
        private static MainWindowViewModel _mainViewModel;        // The Main Window View Model

        #region Accessor Methods
        /// <summary>
        /// The User Name
        /// </summary>
        public string UserName
        {
            get { return _userName;  }
            set
            {
                if (_userName != value)
                {
                    _userName = value;
                    FirePropertyChanged("UserName");
                }
            }
        }
        /// <summary>
        /// The Computer Name
        /// </summary>
        public string ComputerName
        {
            get { return _computerName; }
            set
            {
                if (_computerName != value)
                {
                    _computerName = value;
                    FirePropertyChanged("ComputerName");
                }
            }
        }
        /// <summary>
        /// The Common Language RunTime (CLR) Version
        /// </summary>
        public string CLRVersion
        {
            get { return _clrVersion; }
            set
            {
                if (_clrVersion != value)
                {
                    _clrVersion = value;
                    FirePropertyChanged("CLRVersion");
                }
            }
        }
        /// <summary>
        /// The Operating System Name
        /// </summary>
        public string OperatingSystem
        {
            get { return _operatingSystem; }
            set
            {
                if (_operatingSystem != value)
                {
                    _operatingSystem = value;
                    FirePropertyChanged("OperatingSystem");
                }
            }
        }
        /// <summary>
        /// The Operating System Version
        /// </summary>
        public string OperatingSystemVersion
        {
            get { return _osVersion; }
            set
            {
                if (_osVersion != value)
                {
                    _osVersion = value;
                    FirePropertyChanged("OperatingSystemVersion");
                }
            }
        }
        /// <summary>
        /// The Assembly Name
        /// </summary>
        public string AssemblyName
        {
            get { return _assemName; }
            set
            {
                if (_assemName != value)
                {
                    _assemName = value;
                    FirePropertyChanged("AssemblyName");
                }
            }
        }
        /// <summary>
        /// The Assembly Version
        /// </summary>
        public string AssemblyVersion
        {
            get { return _assemVersion; }
            set
            {
                if (_assemVersion != value)
                {
                    _assemVersion = value;
                    FirePropertyChanged("AssemblyVersion");
                }
            }
        }
        /// <summary>
        /// The Main Window View Model
        /// </summary>
        public MainWindowViewModel MainViewModel
        {
            get { return _mainViewModel; }
            set
            {
                if (_mainViewModel != value)
                {

                }
            }
        }
        #endregion

        public ApplicationViewModel()
        {
            _mainViewModel = new MainWindowViewModel();
        }
    }
}
