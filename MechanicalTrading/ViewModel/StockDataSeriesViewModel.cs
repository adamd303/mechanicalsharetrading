// Copyright (c) 2012, Adam Duval Kangaroo Point South 4169, Australia.
// All Rights Reserved.
using System;
using System.Collections.Generic;
using System.Windows.Media;
using System.Windows.Shapes;
using Adam.Trading.Mechanical.Model;

namespace Adam.Trading.Mechanical.ViewModel
{
    public class StockDataSeriesViewModel : ViewModelBase
    {
        private Polyline _lineSeries = new Polyline();
        private Brush _lineColor = Brushes.Black;
        private double _lineThickness = 1;
        private StockChartEnumerations.LinePattern _linePattern;
        private string _seriesName = "Default Name";
        private Brush _fillColor = Brushes.Black;
        private Brush _majorTrendUpColor = Brushes.Black;
        private Brush _majorTrendDownColor = Brushes.Black;
        private List<BarData> _ohlc = new List<BarData>();  // The list of chart data
        private StockChartEnumerations.StockChartType _stockChartType = StockChartEnumerations.StockChartType.HiLoOpenClose;

        public StockChartEnumerations.StockChartType StockChartType
        {
            get { return _stockChartType; }
            set { _stockChartType = value; }
        }

        // Note: the OHLC data series should be sorted in ascending order by date
        public List<BarData> OHLC
        {
            get { return _ohlc; }
            set { _ohlc = value; }
        }

        public Brush FillColor
        {
            get { return _fillColor; }
            set { _fillColor = value; }
        }

        public Brush LineColor
        {
            get { return _lineColor; }
            set { _lineColor = value; }
        }

        public Brush MajorTrendUpColor
        {
            get { return _majorTrendUpColor; }
            set { _majorTrendUpColor = value; }
        }

        public Brush MajorTrendDownColor
        {
            get { return _majorTrendDownColor; }
            set { _majorTrendDownColor = value; }
        }

        public Polyline LineSeries
        {
            get { return _lineSeries; }
            set { _lineSeries = value; }
        }

        public double LineThickness
        {
            get { return _lineThickness; }
            set { _lineThickness = value; }
        }

        public StockChartEnumerations.LinePattern LinePattern
        {
            get { return _linePattern; }
            set { _linePattern = value; }
        }

        public string SeriesName
        {
            get { return _seriesName; }
            set { _seriesName = value; }
        }

        public void AddLinePattern()
        {
            LineSeries.Stroke = LineColor;
            LineSeries.StrokeThickness = LineThickness;
            switch (LinePattern)
            {
                case StockChartEnumerations.LinePattern.Dash:
                    LineSeries.StrokeDashArray = new DoubleCollection(new double[2] { 4, 3 });
                    break;
                case StockChartEnumerations.LinePattern.Dot:
                    LineSeries.StrokeDashArray = new DoubleCollection(new double[2] { 1, 2 });
                    break;
                case StockChartEnumerations.LinePattern.DashDot:
                    LineSeries.StrokeDashArray = new DoubleCollection(new double[4] { 4, 2, 1, 2 });
                    break;
                case StockChartEnumerations.LinePattern.None:
                    LineSeries.Stroke = Brushes.Transparent;
                    break;
            }
        }
    }
}
