// Copyright (c) 2012, Adam Duval Kangaroo Point South 4169, Australia.
// All Rights Reserved.
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Input;

namespace Adam.Trading.Mechanical.ViewModel
{
    public class MainWindowViewModel : ViewModelBase
    {
        private StockChartViewModel _stockChartViewModel;
        private SupportResistanceViewModel _supportResistanceViewModel;
        private KrauseGannViewModel _krauseGannViewModel;
        private Gann2DayViewModel _gann2DayViewModel;
        
        public StockChartViewModel StockChartViewModel
        {
            get 
            { 
                return _stockChartViewModel;
            }
            set
            {
                if (_stockChartViewModel != value)
                {
                    _stockChartViewModel = value;
                    FirePropertyChanged("StockChartViewModel");
                }
            }
        }

        public SupportResistanceViewModel SupportResistanceViewModel
        {
            get
            {
                return _supportResistanceViewModel;
            }
            set
            {
                if (_supportResistanceViewModel != value)
                {
                    _supportResistanceViewModel = value;
                    FirePropertyChanged("SupportResistanceViewModel");
                }
            }
        }

        public KrauseGannViewModel KrauseGannViewModel
        {
            get
            {
                return _krauseGannViewModel;
            }
            set
            {
                if (_krauseGannViewModel != value)
                {
                    _krauseGannViewModel = value;
                    FirePropertyChanged("KrauseGannViewModel");
                }
            }
        }

        public Gann2DayViewModel Gann2DayViewModel
        {
            get
            {
                return _gann2DayViewModel;
            }
            set
            {
                if (_gann2DayViewModel != value)
                {
                    _gann2DayViewModel = value;
                    FirePropertyChanged("Gann2DayViewModel");
                }
            }
        }

        public MainWindowViewModel()
        {
            _stockChartViewModel = new StockChartViewModel();
            _supportResistanceViewModel = new SupportResistanceViewModel(new FilePathProvider());
            _krauseGannViewModel = new KrauseGannViewModel(new FilePathProvider());
            _gann2DayViewModel = new Gann2DayViewModel(new FilePathProvider()); 
        }
    }
}
