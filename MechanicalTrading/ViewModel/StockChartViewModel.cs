// Copyright (c) 2012, Adam Duval Kangaroo Point South 4169, Australia.
// All Rights Reserved.
using System;
using System.Collections.Generic;
using System.Windows.Controls;
using System.Windows;
using System.Windows.Media;
using System.Windows.Shapes;

namespace Adam.Trading.Mechanical.ViewModel
{
    public class StockChartViewModel : ViewModelBase
    {
        private string title = "Title";
        private string xLabel = "Date/Time";
        private string yLabel = "Price";
        private bool isXGrid = true;
        private bool isYGrid = true;
        private Brush gridlineColor = Brushes.LightGray;
        private StockChartEnumerations.GridlinePattern gridlinePattern;
        private double leftOffset = 20;
        private double bottomOffset = 15;
        private double rightOffset = 10;
        private double topOffset = 0;
        private Line gridline = new Line();
        
        private double xmin = 0;
        private double xmax = 1;
        private double ymin = 0;
        private double ymax = 1;

        private Canvas _chartCanvas = new Canvas();
        private Canvas _textCanvas = new Canvas();

        private StockDataSeriesViewModel _stockDataSeries = new StockDataSeriesViewModel();
        private List<LineDataSeriesViewModel> _lineDataSeries = new List<LineDataSeriesViewModel>();
        private List<SymbolsViewModel> _symbols = new List<SymbolsViewModel>();

        public Canvas ChartCanvas
        {
            get { return _chartCanvas; }
            set { _chartCanvas = value; }
        }

        public StockDataSeriesViewModel StockDataSeries
        {
            get { return _stockDataSeries; }
            set { _stockDataSeries = value; }
        }

        public List<LineDataSeriesViewModel> LineDataSeries
        {
            get { return _lineDataSeries; }
            set { _lineDataSeries = value; }
        }

        public List<SymbolsViewModel> Symbols
        {
            get { return _symbols; }
            set { _symbols = value; }
        }

        public double Xmin
        {
            get { return xmin; }
            set { xmin = value; }
        }

        public double Xmax
        {
            get { return xmax; }
            set { xmax = value; }
        }

        public double Ymin
        {
            get { return ymin; }
            set { ymin = value; }
        }

        public double Ymax
        {
            get { return ymax; }
            set { ymax = value; }
        }

        public Point NormalizePoint(Point pt)
        {
            if (ChartCanvas.Width.ToString() == "NaN")
                ChartCanvas.Width = 270;
            if (ChartCanvas.Height.ToString() == "NaN")
                ChartCanvas.Height = 250;
            Point result = new Point();
            result.X = (pt.X - Xmin) * ChartCanvas.Width / (Xmax - Xmin);
            result.Y = ChartCanvas.Height - (pt.Y - Ymin) * ChartCanvas.Height / (Ymax - Ymin);
            return result;
        }

        public double TimeSpanToDouble(TimeSpan ts)
        {
            DateTime dt = DateTime.Parse("1 Jan");
            double d1 = BitConverter.ToDouble(BitConverter.GetBytes(dt.Ticks), 0);
            dt += ts;
            double d2 = BitConverter.ToDouble(BitConverter.GetBytes(dt.Ticks), 0);
            return d2 - d1;
        }

        public double DateToDouble(string date)
        {
            DateTime dt = DateTime.Parse(date);
            return BitConverter.ToDouble(BitConverter.GetBytes(dt.Ticks), 0);
        }

        public double DaysDiff(DateTime date1, DateTime date2)
        {
            TimeSpan dateDiff = date2 - date1;
            return dateDiff.TotalDays;
        }

        public string Title
        {
            get { return title; }
            set { title = value; }
        }

        public string XLabel
        {
            get { return xLabel; }
            set { xLabel = value; }
        }

        public string YLabel
        {
            get { return yLabel; }
            set { yLabel = value; }
        }

        public StockChartEnumerations.GridlinePattern GridlinePattern
        {
            get { return gridlinePattern; }
            set { gridlinePattern = value; }
        }

        public Brush GridlineColor
        {
            get { return gridlineColor; }
            set { gridlineColor = value; }
        }

        public Canvas TextCanvas
        {
            get { return _textCanvas; }
            set { _textCanvas = value; }
        }

        public bool IsXGrid
        {
            get { return isXGrid; }
            set { isXGrid = value; }
        }

        public bool IsYGrid
        {
            get { return isYGrid; }
            set { isYGrid = value; }
        }

        public double OptimalSpacing(double original)
        {
            double[] da = {1.0, 2.0, 5.0, 10.0, 30.0, 60.0, 90.0, 120.0, 150.0, 
                           180.0, 210.0, 240.0, 270.0, 300.0, 330.0, 360.0};
            double multiplier = Math.Pow(10, Math.Floor(Math.Log(original) / Math.Log(10)));
            double dmin = 100.0 * multiplier;
            double spacing = 0.0;
            double mn = 100.0;

            foreach (double d in da)
            {
                double delta = Math.Abs(original - d * multiplier);
                if (delta < dmin)
                {
                    dmin = delta;
                    spacing = d * multiplier;
                }
                if (d < mn)
                {
                    mn = d;
                }
            }

            if (Math.Abs(original - 10.0 * mn * multiplier) < Math.Abs(original - spacing))
            {
                spacing = 10.0 * mn * multiplier;
            }
            return spacing;
        }

        // TODO: Put all the data on the BarChart data series so it has a common date
        public void AddChart(TextBlock tbTitle, TextBlock tbXLabel, TextBlock tbYLabel)
        {
            Point pt = new Point();
            Line tick = new Line();
            double offset = 0;
            double dx, dy;
            TextBlock tb = new TextBlock();
            double optimalXSpacing = 100.0;
            double optimalYSpacing = 80.0;
            
            //  Determine right offset:
            tb.Text = Ymax.ToString("0.00");
            tb.Measure(new Size(Double.PositiveInfinity, Double.PositiveInfinity));
            Size size = tb.DesiredSize;
            rightOffset = size.Width / 2 + 2;

            // Determine left offset:
            double xScale = 0.0, yScale = 0.0;
            double xSpacing = 0.0, ySpacing = 0.0;
            double xTick = 0.0, yTick = 0.0;
            int xStart = 0, xEnd = 1;
            int yStart = 0, yEnd = 1;
            double offset0 = 30.0;

            while (Math.Abs(offset - offset0) > 1)
            {
                if (Xmin != Xmax)
                {
                    xScale = (TextCanvas.Width - offset0 - rightOffset - 5) /
                             (Xmax - Xmin);
                }
                if (Ymin != Ymax)
                {
                    yScale = TextCanvas.Height / (Ymax - Ymin);
                }
                xSpacing = optimalXSpacing / xScale;
                xTick = OptimalSpacing(xSpacing);
                ySpacing = optimalYSpacing / yScale;
                yTick = OptimalSpacing(ySpacing);
                xStart = (int)Math.Ceiling(Xmin / xTick);
                xEnd = (int)Math.Floor(Xmax / xTick);
                yStart = (int)Math.Ceiling(Ymin / yTick);
                yEnd = (int)Math.Floor(Ymax / yTick);

                for (int i = yStart; i <= yEnd; i++)
                {
                    dy = i * yTick;
                    pt = NormalizePoint(new Point(Xmin, dy));
                    tb = new TextBlock();
                    tb.Text = dy.ToString("0.00");
                    tb.TextAlignment = TextAlignment.Right;
                    tb.Measure(new Size(Double.PositiveInfinity, Double.PositiveInfinity));
                    size = tb.DesiredSize;
                    if (offset < size.Width)
                    {
                        offset = size.Width;
                    }
                }
                if (offset0 > offset)
                {
                    offset0 -= 0.5;
                }
                else if (offset0 < offset)
                {
                    offset0 += 0.5;
                }
            }
            leftOffset = offset + 5;

            topOffset = size.Height / 1.5;

            // Determine the bottom offset
            offset = bottomOffset;
            for (dx = Xmin; dx < Xmax; dx += xTick)
            {
                if (dx > Xmin && dx < Xmax)
                {
                    tb = new TextBlock();
                    if (_stockDataSeries.OHLC.Count > 0)
                    {
                        // The list should be sorted in ascending order
                        tb.Text = (_stockDataSeries.OHLC[0].Date.AddDays(dx)).ToShortDateString();
                    }
                    tb.LayoutTransform = new RotateTransform(-90.0);
                    tb.Measure(new Size(Double.PositiveInfinity, Double.PositiveInfinity));
                    size = tb.DesiredSize;
                    if (offset < size.Height)
                    {
                        offset = size.Height;
                    }
                }
            }
            bottomOffset = offset + 5;

            Canvas.SetLeft(ChartCanvas, leftOffset);
            Canvas.SetBottom(ChartCanvas, bottomOffset);
            ChartCanvas.Width = TextCanvas.Width - leftOffset - rightOffset;
            ChartCanvas.Height = TextCanvas.Height - bottomOffset - topOffset;
            Rectangle chartRect = new Rectangle();
            chartRect.Stroke = Brushes.Black;
            chartRect.Width = ChartCanvas.Width;
            chartRect.Height = ChartCanvas.Height;
            ChartCanvas.Children.Add(chartRect);

            if (Xmin != Xmax)
            {
                xScale = ChartCanvas.Width / (Xmax - Xmin);
            }
            if (Ymin != Ymax)
            {
                yScale = ChartCanvas.Height / (Ymax - Ymin);
            }
            xSpacing = optimalXSpacing / xScale;
            xTick = OptimalSpacing(xSpacing);
            ySpacing = optimalYSpacing / yScale;
            yTick = OptimalSpacing(ySpacing);
            xStart = (int)Math.Ceiling(Xmin / xTick);
            xEnd = (int)Math.Floor(Xmax / xTick);
            yStart = (int)Math.Ceiling(Ymin / yTick);
            yEnd = (int)Math.Floor(Ymax / yTick);

            // Create vertical gridlines and x tick marks:
            if (IsYGrid == true)
            {
                for (int i = xStart; i <= xEnd; i++)
                {
                    gridline = new Line();
                    AddLinePattern();
                    dx = i * xTick;
                    gridline.X1 = NormalizePoint(new Point(dx, Ymin)).X;
                    gridline.Y1 = NormalizePoint(new Point(dx, Ymin)).Y;
                    gridline.X2 = NormalizePoint(new Point(dx, Ymax)).X;
                    gridline.Y2 = NormalizePoint(new Point(dx, Ymax)).Y;
                    ChartCanvas.Children.Add(gridline);

                    pt = NormalizePoint(new Point(dx, Ymin));
                    tick = new Line();
                    tick.Stroke = Brushes.Black;
                    tick.X1 = pt.X;
                    tick.Y1 = pt.Y;
                    tick.X2 = pt.X;
                    tick.Y2 = pt.Y - 5;
                    ChartCanvas.Children.Add(tick);

                    tb = new TextBlock();
                    if (_stockDataSeries.OHLC.Count > 0)
                    {
                        // The list should be sorted in ascending order
                        tb.Text = (_stockDataSeries.OHLC[0].Date.AddDays(dx)).ToShortDateString();
                    }
                    tb.LayoutTransform = new RotateTransform(-90.0);
                    tb.Measure(new Size(Double.PositiveInfinity, Double.PositiveInfinity));
                    size = tb.DesiredSize;
                    TextCanvas.Children.Add(tb);
                    Canvas.SetLeft(tb, leftOffset + pt.X - size.Width / 2);
                    Canvas.SetTop(tb, pt.Y + size.Width / 1.5 + 5);
                }
            }

            // Create horizontal gridlines and y tick marks:
            if (IsXGrid == true)
            {
                for (int i = yStart; i <= yEnd; i++)
                {
                    gridline = new Line();
                    AddLinePattern();
                    dy = i * yTick;
                    gridline.X1 = NormalizePoint(new Point(Xmin, dy)).X;
                    gridline.Y1 = NormalizePoint(new Point(Xmin, dy)).Y;
                    gridline.X2 = NormalizePoint(new Point(Xmax, dy)).X;
                    gridline.Y2 = NormalizePoint(new Point(Xmax, dy)).Y;
                    ChartCanvas.Children.Add(gridline);

                    pt = NormalizePoint(new Point(Xmin, dy));
                    tick = new Line();
                    tick.Stroke = Brushes.Black;
                    tick.X1 = pt.X;
                    tick.Y1 = pt.Y;
                    tick.X2 = pt.X + 5;
                    tick.Y2 = pt.Y;
                    ChartCanvas.Children.Add(tick);

                    tb = new TextBlock();
                    tb.Text = dy.ToString("0.00");
                    tb.Measure(new Size(Double.PositiveInfinity, Double.PositiveInfinity));
                    size = tb.DesiredSize;
                    TextCanvas.Children.Add(tb);
                    Canvas.SetLeft(tb, 0.0);
                    // Canvas.SetRight(tb , chartCanvas.Width + 10);
                    Canvas.SetTop(tb, pt.Y);
                }
            }

            // Add title and labels:
            tbTitle.Text = Title;
            tbXLabel.Text = XLabel;
            tbYLabel.Text = YLabel;
            tbXLabel.Margin = new Thickness(leftOffset + 2, 2, 2, 2);
            tbTitle.Margin = new Thickness(leftOffset + 2, 2, 2, 2);
            AddDataSeriesToChart();
        }

        public void AddLinePattern()
        {
            gridline.Stroke = GridlineColor;
            gridline.StrokeThickness = 1;

            switch (GridlinePattern)
            {
                case StockChartEnumerations.GridlinePattern.Dash:
                    gridline.StrokeDashArray = new DoubleCollection(new double[2] { 4, 3 });
                    break;
                case StockChartEnumerations.GridlinePattern.Dot:
                    gridline.StrokeDashArray = new DoubleCollection(new double[2] { 1, 2 });
                    break;
                case StockChartEnumerations.GridlinePattern.DashDot:
                    gridline.StrokeDashArray = new DoubleCollection(new double[4] { 4, 2, 1, 2 });
                    break;
            }
        }

        private void AddDataSeriesToChart()
        {
            if ((_stockDataSeries != null) && (_stockDataSeries.OHLC != null) && (_stockDataSeries.OHLC.Count > 0))
            {
                // Firstly Add the Stock Series as OHLC or Candles
                // This is the bar width.
                // It should really be based on the display start and end dates
                double barWidth = _chartCanvas.Width / (5 * _stockDataSeries.OHLC.Count);

                for (int i = 0; i < _stockDataSeries.OHLC.Count; i++)
                {
                    // Find the x coordinate
                    double x = DaysDiff(_stockDataSeries.OHLC[0].Date, _stockDataSeries.OHLC[i].Date);
                    // Convert to screen coordinates
                    Point ptHigh = NormalizePoint(new Point(x, double.Parse(_stockDataSeries.OHLC[i].High.ToString())));
                    Point ptLow = NormalizePoint(new Point(x, double.Parse(_stockDataSeries.OHLC[i].Low.ToString())));
                    Point ptOpen = NormalizePoint(new Point(x, double.Parse(_stockDataSeries.OHLC[i].Open.ToString())));
                    Point ptClose = NormalizePoint(new Point(x, double.Parse(_stockDataSeries.OHLC[i].Close.ToString())));
                    // Draw the candle body
                    Point ptOpen1 = new Point(ptOpen.X - barWidth, ptOpen.Y);
                    Point ptClose1 = new Point(ptClose.X + barWidth, ptClose.Y);
                    Point ptOpen2 = new Point(ptOpen.X + barWidth, ptOpen.Y);
                    Point ptClose2 = new Point(ptClose.X - barWidth, ptClose.Y);

                    switch (_stockDataSeries.StockChartType)
                    {
                        case StockChartEnumerations.StockChartType.HiLoOpenClose:  // Draw Hi-Lo-Open-Close stock chart:
                            DrawLine(_chartCanvas, ptLow, ptHigh, _stockDataSeries.LineColor, _stockDataSeries.LineThickness);
                            DrawLine(_chartCanvas, ptOpen, ptOpen1, _stockDataSeries.LineColor, _stockDataSeries.LineThickness);
                            DrawLine(_chartCanvas, ptClose, ptClose1, _stockDataSeries.LineColor, _stockDataSeries.LineThickness);
                            break;
                        case StockChartEnumerations.StockChartType.Candle: // Draw candle stock chart:
                            Brush lineColor = _stockDataSeries.LineColor;
                            Brush fillColor = _stockDataSeries.FillColor;
                            if (_stockDataSeries.OHLC[i].MajorTrend == Model.TradingEnumerations.Trend.Up)
                            {
                                fillColor = _stockDataSeries.MajorTrendUpColor;
                                lineColor = _stockDataSeries.MajorTrendUpColor;
                            }
                            else if ((_stockDataSeries.OHLC[i].MajorTrend == Model.TradingEnumerations.Trend.Down))
                            {
                                fillColor = _stockDataSeries.MajorTrendDownColor;
                                lineColor = _stockDataSeries.MajorTrendDownColor;
                            }
                            if (_stockDataSeries.OHLC[i].Open < _stockDataSeries.OHLC[i].Close)
                            {
                                fillColor = Brushes.White;
                            }
                            DrawLine(_chartCanvas, ptLow, ptHigh, lineColor, _stockDataSeries.LineThickness);
                            Polygon plg = new Polygon();
                            plg.Stroke = lineColor;
                            plg.StrokeThickness = _stockDataSeries.LineThickness;
                            plg.Fill = fillColor;
                            plg.Points.Add(ptOpen1);
                            plg.Points.Add(ptOpen2);
                            plg.Points.Add(ptClose1);
                            plg.Points.Add(ptClose2);
                            plg.ToolTip = _stockDataSeries.OHLC[i].Date.ToShortDateString() + "[" +
                                          _stockDataSeries.OHLC[i].Open.ToString() + " " +
                                          _stockDataSeries.OHLC[i].High.ToString() + " " + 
                                          _stockDataSeries.OHLC[i].Low.ToString() + " " +
                                          _stockDataSeries.OHLC[i].Close.ToString() + "]";
                            _chartCanvas.Children.Add(plg);
                            break;
                    }
                }
            }
            // Now add the line series
            if (_lineDataSeries != null)
            {
                foreach (LineDataSeriesViewModel ls in _lineDataSeries)
                {
                    if (ls.Line != null)
                    {
                        for (int i = 0; i < ls.Line.Count; i++)
                        {
                            // Convert to screen coordinates
                            if (i > 0)
                            {
                                double defaultLineThickness = ls.LineThickness;
                                double tripleLineThickness;
                                if (ls.Line[i].LastLineSegmentActive)
                                {
                                    tripleLineThickness = 3.0 * defaultLineThickness;
                                    defaultLineThickness = tripleLineThickness;
                                }
                                // Find the x coordinates
                                double x1 = DaysDiff(_stockDataSeries.OHLC[0].Date, ls.Line[i - 1].Date);
                                double x2 = DaysDiff(_stockDataSeries.OHLC[0].Date, ls.Line[i].Date);
                                Point pt1 = NormalizePoint(new Point(x1, double.Parse(ls.Line[i - 1].Price.ToString())));
                                Point pt2 = NormalizePoint(new Point(x2, double.Parse(ls.Line[i].Price.ToString())));
                                DrawLine(_chartCanvas, pt1, pt2, ls.LineColor, defaultLineThickness);
                            }
                        }
                    }
                }
            }
            // Now add the symbols
            if (_symbols != null)
            {
                foreach (SymbolsViewModel sy in _symbols)
                {
                    if (sy.Symbols != null)
                    {
                        for (int i = 0; i < sy.Symbols.Count; i++)
                        {
                            // Find the x coordinates
                            double x = DaysDiff(_stockDataSeries.OHLC[0].Date, sy.Symbols[i].Date);
                            Point pt = NormalizePoint(new Point(x, double.Parse(sy.Symbols[i].Price.ToString())));
                            DrawSymbol(_chartCanvas,
                                       pt,
                                       sy.SymbolType,
                                       sy.SymbolSize,
                                       sy.BorderColor,
                                       sy.FillColor,
                                       sy.BorderThickness);
                        }
                    }
                }
            }
        }

        private void DrawLine(Canvas canvas, Point pt1, Point pt2, Brush lineColor, double lineThickness)
        {
            Line line = new Line();
            line.Stroke = lineColor;
            line.StrokeThickness = lineThickness;
            line.X1 = pt1.X;
            line.Y1 = pt1.Y;
            line.X2 = pt2.X;
            line.Y2 = pt2.Y;
            canvas.Children.Add(line);
        }

        public void DrawSymbol(Canvas canvas, 
                               Point pt, 
                               StockChartEnumerations.SymbolTypeEnum symbolType, 
                               double symbolSize,
                               Brush borderColor,
                               Brush fillColor,
                               double borderThickness)
        {
            Polygon plg = new Polygon();
            plg.Stroke = borderColor;
            plg.StrokeThickness = borderThickness;
            Ellipse ellipse = new Ellipse();
            ellipse.Stroke = borderColor;
            ellipse.StrokeThickness = borderThickness;
            Line line = new Line();
            double halfSize = 0.5 * symbolSize;

            Canvas.SetZIndex(plg, 5);
            Canvas.SetZIndex(ellipse, 5);

            switch (symbolType)
            {
                case StockChartEnumerations.SymbolTypeEnum.Square:
                    plg.Fill = fillColor;
                    plg.Points.Add(new Point(pt.X - halfSize, pt.Y - halfSize));
                    plg.Points.Add(new Point(pt.X + halfSize, pt.Y - halfSize));
                    plg.Points.Add(new Point(pt.X + halfSize, pt.Y + halfSize));
                    plg.Points.Add(new Point(pt.X - halfSize, pt.Y + halfSize));
                    canvas.Children.Add(plg);
                    break;
                case StockChartEnumerations.SymbolTypeEnum.OpenDiamond:
                    plg.Fill = Brushes.White;
                    plg.Points.Add(new Point(pt.X - halfSize, pt.Y));
                    plg.Points.Add(new Point(pt.X, pt.Y - halfSize));
                    plg.Points.Add(new Point(pt.X + halfSize, pt.Y));
                    plg.Points.Add(new Point(pt.X, pt.Y + halfSize));
                    canvas.Children.Add(plg);
                    break;
                case StockChartEnumerations.SymbolTypeEnum.Circle:
                    ellipse.Fill = fillColor;
                    ellipse.Width = symbolSize;
                    ellipse.Height = symbolSize;
                    Canvas.SetLeft(ellipse, pt.X - halfSize);
                    Canvas.SetTop(ellipse, pt.Y - halfSize);
                    canvas.Children.Add(ellipse);
                    break;
                case StockChartEnumerations.SymbolTypeEnum.OpenTriangle:
                    plg.Fill = Brushes.White;
                    plg.Points.Add(new Point(pt.X - halfSize, pt.Y + halfSize));
                    plg.Points.Add(new Point(pt.X, pt.Y - halfSize));
                    plg.Points.Add(new Point(pt.X + halfSize, pt.Y + halfSize));
                    canvas.Children.Add(plg);
                    break;
                case StockChartEnumerations.SymbolTypeEnum.None:
                    break;
                case StockChartEnumerations.SymbolTypeEnum.Cross:
                    line = new Line();
                    Canvas.SetZIndex(line, 5);
                    line.Stroke = borderColor;
                    line.StrokeThickness = borderThickness;
                    line.X1 = pt.X - halfSize;
                    line.Y1 = pt.Y + halfSize;
                    line.X2 = pt.X + halfSize;
                    line.Y2 = pt.Y - halfSize;
                    canvas.Children.Add(line);
                    line = new Line();
                    Canvas.SetZIndex(line, 5);
                    line.Stroke = borderColor;
                    line.StrokeThickness = borderThickness;
                    line.X1 = pt.X - halfSize;
                    line.Y1 = pt.Y - halfSize;
                    line.X2 = pt.X + halfSize;
                    line.Y2 = pt.Y + halfSize;
                    canvas.Children.Add(line);
                    Canvas.SetZIndex(line, 5);
                    break;
                case StockChartEnumerations.SymbolTypeEnum.Star:
                    line = new Line();
                    Canvas.SetZIndex(line, 5);
                    line.Stroke = borderColor;
                    line.StrokeThickness = borderThickness;
                    line.X1 = pt.X - halfSize;
                    line.Y1 = pt.Y + halfSize;
                    line.X2 = pt.X + halfSize;
                    line.Y2 = pt.Y - halfSize;
                    canvas.Children.Add(line);
                    line = new Line();
                    Canvas.SetZIndex(line, 5);
                    line.Stroke = borderColor;
                    line.StrokeThickness = borderThickness;
                    line.X1 = pt.X - halfSize;
                    line.Y1 = pt.Y - halfSize;
                    line.X2 = pt.X + halfSize;
                    line.Y2 = pt.Y + halfSize;
                    canvas.Children.Add(line);
                    line = new Line();
                    Canvas.SetZIndex(line, 5);
                    line.Stroke = borderColor;
                    line.StrokeThickness = borderThickness;
                    line.X1 = pt.X - halfSize;
                    line.Y1 = pt.Y;
                    line.X2 = pt.X + halfSize;
                    line.Y2 = pt.Y;
                    canvas.Children.Add(line);
                    line = new Line();
                    Canvas.SetZIndex(line, 5);
                    line.Stroke = borderColor;
                    line.StrokeThickness = borderThickness;
                    line.X1 = pt.X;
                    line.Y1 = pt.Y - halfSize;
                    line.X2 = pt.X;
                    line.Y2 = pt.Y + halfSize;
                    canvas.Children.Add(line);
                    break;
                case StockChartEnumerations.SymbolTypeEnum.OpenInvertedTriangle:
                    plg.Fill = Brushes.White;
                    plg.Points.Add(new Point(pt.X, pt.Y + halfSize));
                    plg.Points.Add(new Point(pt.X - halfSize, pt.Y - halfSize));
                    plg.Points.Add(new Point(pt.X + halfSize, pt.Y - halfSize));
                    canvas.Children.Add(plg);
                    break;
                case StockChartEnumerations.SymbolTypeEnum.Plus:
                    line = new Line();
                    Canvas.SetZIndex(line, 5);
                    line.Stroke = borderColor;
                    line.StrokeThickness = borderThickness;
                    line.X1 = pt.X - halfSize;
                    line.Y1 = pt.Y;
                    line.X2 = pt.X + halfSize;
                    line.Y2 = pt.Y;
                    canvas.Children.Add(line);
                    line = new Line();
                    Canvas.SetZIndex(line, 5);
                    line.Stroke = borderColor;
                    line.StrokeThickness = borderThickness;
                    line.X1 = pt.X;
                    line.Y1 = pt.Y - halfSize;
                    line.X2 = pt.X;
                    line.Y2 = pt.Y + halfSize;
                    canvas.Children.Add(line);
                    break;
                case StockChartEnumerations.SymbolTypeEnum.Dot:
                    ellipse.Fill = fillColor;
                    ellipse.Width = symbolSize;
                    ellipse.Height = symbolSize;
                    Canvas.SetLeft(ellipse, pt.X - halfSize);
                    Canvas.SetTop(ellipse, pt.Y - halfSize);
                    canvas.Children.Add(ellipse);
                    break;
                case StockChartEnumerations.SymbolTypeEnum.Box:
                    plg.Fill = fillColor;
                    plg.Points.Add(new Point(pt.X - halfSize, pt.Y - halfSize));
                    plg.Points.Add(new Point(pt.X + halfSize, pt.Y - halfSize));
                    plg.Points.Add(new Point(pt.X + halfSize, pt.Y + halfSize));
                    plg.Points.Add(new Point(pt.X - halfSize, pt.Y + halfSize));
                    canvas.Children.Add(plg);
                    break;
                case StockChartEnumerations.SymbolTypeEnum.Diamond:
                    plg.Fill = fillColor;
                    plg.Points.Add(new Point(pt.X - halfSize, pt.Y));
                    plg.Points.Add(new Point(pt.X, pt.Y - halfSize));
                    plg.Points.Add(new Point(pt.X + halfSize, pt.Y));
                    plg.Points.Add(new Point(pt.X, pt.Y + halfSize));
                    canvas.Children.Add(plg);
                    break;
                case StockChartEnumerations.SymbolTypeEnum.InvertedTriangle:
                    plg.Fill = fillColor;
                    plg.Points.Add(new Point(pt.X, pt.Y + halfSize));
                    plg.Points.Add(new Point(pt.X - halfSize, pt.Y - halfSize));
                    plg.Points.Add(new Point(pt.X + halfSize, pt.Y - halfSize));
                    canvas.Children.Add(plg);
                    break;
                case StockChartEnumerations.SymbolTypeEnum.Triangle:
                    plg.Fill = fillColor;
                    plg.Points.Add(new Point(pt.X - halfSize, pt.Y + halfSize));
                    plg.Points.Add(new Point(pt.X, pt.Y - halfSize));
                    plg.Points.Add(new Point(pt.X + halfSize, pt.Y + halfSize));
                    canvas.Children.Add(plg);
                    break;
            }
        }
    }
}
