// Copyright (c) 2012, Adam Duval Kangaroo Point South 4169, Australia.
// All Rights Reserved.
using System;
using System.Collections.Generic;
using System.Windows.Media;
using System.Windows.Shapes;
using Adam.Trading.Mechanical.Model;

namespace Adam.Trading.Mechanical.ViewModel
{
    public class LineDataSeriesViewModel : ViewModelBase
    {
        private Polyline _lineSeries = new Polyline();
        private Brush _lineColor = Brushes.Black;
        private double _lineThickness = 1;
        private StockChartEnumerations.LinePattern _linePattern;
        private string _seriesName = "Default Name";
        private Brush _fillColor = Brushes.Black;
        private List<LineData> _line = new List<LineData>();  // The list of line data

        // Note: the Line data series should be sorted in ascending order by date
        public List<LineData> Line
        {
            get { return _line; }
            set { _line = value; }
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
    }
}
