// Copyright (c) 2012, Adam Duval Kangaroo Point South 4169, Australia.
// All Rights Reserved. 
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using Adam.Trading.Mechanical.ViewModel;
using Adam.Trading.Mechanical.View.SubViews;
using log4net;

namespace Adam.Trading.Mechanical.View
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        protected static readonly ILog log = LogManager.GetLogger(typeof(MainWindow));

        private MainWindowViewModel _viewModel;

        private EventHandler supportResistanceWindowClosed;
        private EventHandler<SupportResistanceViewModelEventArgs<SupportResistanceViewModel>> supportResistanceCalculatedHandler;
        private EventHandler krauseGannWindowClosed;
        private EventHandler<KrauseGannViewModelEventArgs<KrauseGannViewModel>> krauseGannCalculatedHandler;
        private EventHandler gann2DayWindowClosed;
        private EventHandler<Gann2DayViewModelEventArgs<Gann2DayViewModel>> gann2DayCalculatedHandler;

        private enum Model
        {
            SupportResistance,
            KrauseGann,
            Gann2Day,
            None
        }

        private Model _model = Model.None;
        
        private Window SupportResistanceWindow
        {
            get;
            set;
        }

        private Window KrauseGannWindow
        {
            get;
            set;
        }

        private Window Gann2DayWindow
        {
            get;
            set;
        }

        public MainWindow()
        {
            log.Info("MainWindow Initialize starting...");
            InitializeComponent();
            supportResistanceWindowClosed = new EventHandler(SupportResistanceWindow_Closed);
            supportResistanceCalculatedHandler = new EventHandler<SupportResistanceViewModelEventArgs<SupportResistanceViewModel>>(SupportResistance_Calculated);
            krauseGannWindowClosed = new EventHandler(KrauseGannWindow_Closed);
            krauseGannCalculatedHandler = new EventHandler<KrauseGannViewModelEventArgs<KrauseGannViewModel>>(KrauseGann_Calculated);
            gann2DayWindowClosed = new EventHandler(Gann2DayWindow_Closed);
            gann2DayCalculatedHandler = new EventHandler<Gann2DayViewModelEventArgs<Gann2DayViewModel>>(Gann2Day_Calculated);
            _chartStartDate.DisplayDateStart = DateTime.MinValue;
            _chartStartDate.DisplayDateEnd = DateTime.MaxValue;
            _chartStartDate.SelectedDate = DateTime.MinValue;
            _chartEndDate.DisplayDateStart = DateTime.MinValue;
            _chartEndDate.DisplayDateEnd = DateTime.MaxValue;
            _chartEndDate.SelectedDate = DateTime.MinValue;
            log.Info("MainWindow Initialize completed...");
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            _viewModel = this.DataContext as MainWindowViewModel;
            _viewModel.SupportResistanceViewModel.OnSupportResistanceCalculated += supportResistanceCalculatedHandler;
            _viewModel.KrauseGannViewModel.OnKrauseGannCalculated += krauseGannCalculatedHandler;
            _viewModel.Gann2DayViewModel.OnGann2DayCalculated += gann2DayCalculatedHandler;
        }

        private void ShowSupportResistanceWindow_CanExecute(object sender, CanExecuteRoutedEventArgs e)
        {
            if (_viewModel != null)
            {
                if (SupportResistanceWindow == null)
                {
                    e.CanExecute = true;
                }
            }
            e.Handled = true;
        }

        public void ShowSupportResistanceWindow_Executed(object sender, ExecutedRoutedEventArgs e)
        {
            if (_viewModel != null)
            {
                if (SupportResistanceWindow == null)
                {
                    var view = new SupportResistanceView();
                    view.DataContext = _viewModel.SupportResistanceViewModel;
                    SupportResistanceWindow = new Window();
                    SupportResistanceWindow.Title = Properties.Resources.SupportResistanceWindow_Title;
                    SupportResistanceWindow.SizeToContent = System.Windows.SizeToContent.WidthAndHeight;
                    SupportResistanceWindow.Content = view;
                    SupportResistanceWindow.Closed += supportResistanceWindowClosed;
                    SupportResistanceWindow.Owner = this;
                }
                this.SupportResistanceWindow.ShowDialog();
            }
        }

        private void ShowKrauseGannWindow_CanExecute(object sender, CanExecuteRoutedEventArgs e)
        {
            if (_viewModel != null)
            {
                if (KrauseGannWindow == null)
                {
                    e.CanExecute = true;
                }
            }
            e.Handled = true;
        }

        public void ShowKrauseGannWindow_Executed(object sender, ExecutedRoutedEventArgs e)
        {
            if (_viewModel != null)
            {
                if (KrauseGannWindow == null)
                {
                    var view = new KrauseGannView();
                    view.DataContext = _viewModel.KrauseGannViewModel;
                    KrauseGannWindow = new Window();
                    KrauseGannWindow.Title = Properties.Resources.KrauseWindow_Title;
                    KrauseGannWindow.SizeToContent = System.Windows.SizeToContent.WidthAndHeight;
                    KrauseGannWindow.Content = view;
                    KrauseGannWindow.Closed += krauseGannWindowClosed;
                    KrauseGannWindow.Owner = this;
                }
                this.KrauseGannWindow.ShowDialog();
            }
        }

        private void ShowGann2DaySwingWindow_CanExecute(object sender, CanExecuteRoutedEventArgs e)
        {
            if (_viewModel != null)
            {
                if (Gann2DayWindow == null)
                {
                    e.CanExecute = true;
                }
            }
            e.Handled = true;
        }

        public void ShowGann2DaySwingWindow_Executed(object sender, ExecutedRoutedEventArgs e)
        {
            if (_viewModel != null)
            {
                if (Gann2DayWindow == null)
                {
                    var view = new Gann2DayView();
                    view.DataContext = _viewModel.Gann2DayViewModel;
                    Gann2DayWindow = new Window();
                    Gann2DayWindow.Title = Properties.Resources.Gann2DaySwingWindow_Title;
                    Gann2DayWindow.SizeToContent = System.Windows.SizeToContent.WidthAndHeight;
                    Gann2DayWindow.Content = view;
                    Gann2DayWindow.Closed += gann2DayWindowClosed;
                    Gann2DayWindow.Owner = this;
                }
                this.Gann2DayWindow.ShowDialog();
            }
        }

        private void Window_SizeChanged(object sender, SizeChangedEventArgs e)
        {
            Clear_Chart();
            if (_viewModel != null)
            {
                _viewModel.StockChartViewModel.AddChart(_stockView.tbTitle,
                                                        _stockView.tbXLabel,
                                                        _stockView.tbYLabel);
            }
        }

        private void Clear_Chart()
        {
            _stockView.textCanvas.Width = _stockView.chartGrid.ActualWidth;
            _stockView.textCanvas.Height = _stockView.chartGrid.ActualHeight;
            _stockView.chartCanvas.Children.Clear();
            _stockView.textCanvas.Children.RemoveRange(1, _stockView.textCanvas.Children.Count - 1);
        }

        private void ChartStartDate_Changed(object sender, SelectionChangedEventArgs item)
        {
            ChartStartEndDateChanged();
        }

        private void ChartEndDate_Changed(object sender, SelectionChangedEventArgs item)
        {
            ChartStartEndDateChanged();
        }

        private void ChartStartEndDateChanged()
        {
            if (_chartEndDate.SelectedDate.HasValue && _chartStartDate.SelectedDate.HasValue)
            {
                DateTime chartStartDate = (DateTime)_chartStartDate.SelectedDate;
                DateTime chartEndDate = (DateTime)_chartEndDate.SelectedDate;
                bool validStartEndDates = true;
                if (chartEndDate < chartStartDate)
                {
                    // Swap the order around
                    DateTime swapper = chartStartDate;
                    chartEndDate = chartStartDate;
                    chartEndDate = swapper;
                    validStartEndDates = true;
                }
                else if (chartEndDate == chartStartDate)
                {
                    Clear_Chart();
                    validStartEndDates = false;
                }
                if (validStartEndDates)
                {
                    switch(_model)
                    {         
                        case Model.SupportResistance:   
                            {
                                // The Support/Resistance viewmodel is the most recently calculated
                                Clear_Chart();
                                _viewModel.SupportResistanceViewModel.CalculateChart(chartStartDate, chartEndDate);
                                _viewModel.StockChartViewModel = _viewModel.SupportResistanceViewModel.ChartViewModel;
                                _viewModel.StockChartViewModel.Title = "Support/Resistance";
                                _viewModel.StockChartViewModel.ChartCanvas = _stockView.chartCanvas;
                                _viewModel.StockChartViewModel.TextCanvas = _stockView.textCanvas;
                                _viewModel.StockChartViewModel.AddChart(_stockView.tbTitle, _stockView.tbXLabel, _stockView.tbYLabel);
                                break;                  
                            }
                        case Model.KrauseGann:
                            {
                                // The Krause Gann viewmodel is the most recently calculated
                                Clear_Chart();
                                _viewModel.KrauseGannViewModel.CalculateChart(chartStartDate, chartEndDate);
                                _viewModel.StockChartViewModel = _viewModel.KrauseGannViewModel.ChartViewModel;
                                _viewModel.StockChartViewModel.Title = "Krause Gann";
                                _viewModel.StockChartViewModel.ChartCanvas = _stockView.chartCanvas;
                                _viewModel.StockChartViewModel.TextCanvas = _stockView.textCanvas;
                                _viewModel.StockChartViewModel.AddChart(_stockView.tbTitle, _stockView.tbXLabel, _stockView.tbYLabel);
                                break;               
                            }
                        case Model.Gann2Day:
                            {
                                // The Gann 2 Day Swing chart viewmodel is the most recently calculated
                                Clear_Chart();
                                _viewModel.Gann2DayViewModel.CalculateChart(chartStartDate, chartEndDate);
                                _viewModel.StockChartViewModel = _viewModel.Gann2DayViewModel.ChartViewModel;
                                _viewModel.StockChartViewModel.Title = "Gann Two Day Swing";
                                _viewModel.StockChartViewModel.ChartCanvas = _stockView.chartCanvas;
                                _viewModel.StockChartViewModel.TextCanvas = _stockView.textCanvas;
                                _viewModel.StockChartViewModel.AddChart(_stockView.tbTitle, _stockView.tbXLabel, _stockView.tbYLabel);
                                break;               
                            }
                        case Model.None:
                            {
                                break;
                            }
                        default:
                            {
                                break;
                            }      
                    }
                }
            }
        }

        private void SupportResistanceWindow_Closed(object sender, EventArgs e)
        {
            // Bring the window down
            // Remove the event handler so it can be garbage collected.
            SupportResistanceWindow.Closed -= supportResistanceWindowClosed;
            SupportResistanceWindow = null;
        }

        private void KrauseGannWindow_Closed(object sender, EventArgs e)
        {
            // Bring the window down
            // Remove the event handler so it can be garbage collected.
            KrauseGannWindow.Closed -= krauseGannWindowClosed;
            KrauseGannWindow = null;
        }

        private void Gann2DayWindow_Closed(object sender, EventArgs e)
        {
            // Bring the window down
            // Remove the event handler so it can be garbage collected.
            Gann2DayWindow.Closed -= gann2DayWindowClosed;
            Gann2DayWindow = null;
        }
        
        private void SupportResistance_Calculated(object sender, SupportResistanceViewModelEventArgs<SupportResistanceViewModel> e)
        {
            log.Info("SupportResistance_Calculated Starting");
            _viewModel.SupportResistanceViewModel = e.Item as SupportResistanceViewModel;
            _model = Model.SupportResistance;
            Clear_Chart();
            _viewModel.StockChartViewModel = _viewModel.SupportResistanceViewModel.ChartViewModel;
            _viewModel.StockChartViewModel.Title = "Support/Resistance";
            _viewModel.StockChartViewModel.ChartCanvas = _stockView.chartCanvas;
            _viewModel.StockChartViewModel.TextCanvas = _stockView.textCanvas;
            _viewModel.StockChartViewModel.AddChart(_stockView.tbTitle, _stockView.tbXLabel, _stockView.tbYLabel);
            // Reset the start and end dates
            _chartStartDate.DisplayDateStart = _viewModel.SupportResistanceViewModel.DataStartDate;
            _chartStartDate.DisplayDateEnd = _viewModel.SupportResistanceViewModel.DataEndDate;
            _chartStartDate.SelectedDate = _viewModel.SupportResistanceViewModel.ChartStartDate;
            _chartEndDate.DisplayDateStart = _viewModel.SupportResistanceViewModel.DataStartDate;
            _chartEndDate.DisplayDateEnd = _viewModel.SupportResistanceViewModel.DataEndDate;
            _chartEndDate.SelectedDate = _viewModel.SupportResistanceViewModel.ChartEndDate;
        }

        private void KrauseGann_Calculated(object sender, KrauseGannViewModelEventArgs<KrauseGannViewModel> e)
        {
            log.Info("KrauseGann_Calculated Starting");
            _viewModel.KrauseGannViewModel = e.Item as KrauseGannViewModel;
            _model = Model.KrauseGann;
            Clear_Chart();
            _viewModel.StockChartViewModel = _viewModel.KrauseGannViewModel.ChartViewModel;
            _viewModel.StockChartViewModel.Title = "Krause Gann";
            _viewModel.StockChartViewModel.ChartCanvas = _stockView.chartCanvas;
            _viewModel.StockChartViewModel.TextCanvas = _stockView.textCanvas;
            _viewModel.StockChartViewModel.AddChart(_stockView.tbTitle, _stockView.tbXLabel, _stockView.tbYLabel);
            // Reset the start and end dates
            _chartStartDate.DisplayDateStart = _viewModel.KrauseGannViewModel.DataStartDate;
            _chartStartDate.DisplayDateEnd = _viewModel.KrauseGannViewModel.DataEndDate;
            _chartStartDate.SelectedDate = _viewModel.KrauseGannViewModel.ChartStartDate;
            _chartEndDate.DisplayDateStart = _viewModel.KrauseGannViewModel.DataStartDate;
            _chartEndDate.DisplayDateEnd = _viewModel.KrauseGannViewModel.DataEndDate;
            _chartEndDate.SelectedDate = _viewModel.KrauseGannViewModel.ChartEndDate;
        }

        private void Gann2Day_Calculated(object sender, Gann2DayViewModelEventArgs<Gann2DayViewModel> e)
        {
            log.Info("Gann2Day_Calculated Starting");
            _viewModel.Gann2DayViewModel = e.Item as Gann2DayViewModel;
            _model = Model.Gann2Day;
            Clear_Chart();
            _viewModel.StockChartViewModel = _viewModel.Gann2DayViewModel.ChartViewModel;
            _viewModel.StockChartViewModel.Title = "Gann Two Day Swing";
            _viewModel.StockChartViewModel.ChartCanvas = _stockView.chartCanvas;
            _viewModel.StockChartViewModel.TextCanvas = _stockView.textCanvas;
            _viewModel.StockChartViewModel.AddChart(_stockView.tbTitle, _stockView.tbXLabel, _stockView.tbYLabel);
            // Reset the start and end dates
            _chartStartDate.DisplayDateStart = _viewModel.Gann2DayViewModel.DataStartDate;
            _chartStartDate.DisplayDateEnd = _viewModel.Gann2DayViewModel.DataEndDate;
            _chartStartDate.SelectedDate = _viewModel.Gann2DayViewModel.ChartStartDate;
            _chartEndDate.DisplayDateStart = _viewModel.Gann2DayViewModel.DataStartDate;
            _chartEndDate.DisplayDateEnd = _viewModel.Gann2DayViewModel.DataEndDate;
            _chartEndDate.SelectedDate = _viewModel.Gann2DayViewModel.ChartEndDate;
        }
    }
}
