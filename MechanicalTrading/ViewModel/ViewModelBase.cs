// Copyright (c) 2011, Adam Duval Kangaroo Point South 4169, Australia.
// All Rights Reserved.
using System;
using System.ComponentModel;
using System.Windows.Threading;

namespace Adam.Trading.Mechanical.ViewModel
{
    /// <summary>
    /// The base class for use with all view models.
    /// </summary>
    public abstract class ViewModelBase : INotifyPropertyChanged
    {
        public static Dispatcher ApplicationDispatcher
        {
            get;
            set;
        }

        protected void RunOnUiThread(Action a)
        {
            ApplicationDispatcher.BeginInvoke(DispatcherPriority.Normal, a);
        }

        /// <summary>
        /// Fired when a property changes.
        /// </summary>
        public event PropertyChangedEventHandler PropertyChanged;

        /// <summary>
        /// Helper function to fire the property changed event.
        /// </summary>
        /// <param name="propertyName">Name of the property that has changed.</param>
        protected virtual void FirePropertyChanged(string propertyName)
        {
            var handler = this.PropertyChanged;
            if (handler != null)
            {
                handler(this, new PropertyChangedEventArgs(propertyName));
            }
        }

        /// <summary>
        /// Default constructor.
        /// </summary>
        protected ViewModelBase()
        {
        }
    }
}
