// Copyright (c) 2012, Adam Duval Kangaroo Point South 4169, Australia.
// All Rights Reserved.
using System;
using System.Collections.Generic;
using System.Windows.Controls;
using System.Windows;
using System.Windows.Media;
using System.Windows.Shapes;
using Adam.Trading.Mechanical.Model;

namespace Adam.Trading.Mechanical.ViewModel
{
    public class SymbolsViewModel : ViewModelBase
    {
        private StockChartEnumerations.SymbolTypeEnum symbolType;
        private double symbolSize;
        private Brush borderColor;
        private Brush fillColor;
        private double borderThickness;
        private List<LineData> _symbols = new List<LineData>();  // The list of symbols

        public SymbolsViewModel()
        {
            symbolType = StockChartEnumerations.SymbolTypeEnum.None;
            symbolSize = 8.0;
            borderColor = Brushes.Black;
            fillColor = Brushes.Black;
            borderThickness = 1.0;
        }

        public double BorderThickness
        {
            get { return borderThickness; }
            set { borderThickness = value; }
        }

        public Brush BorderColor
        {
            get { return borderColor; }
            set { borderColor = value; }
        }

        public Brush FillColor
        {
            get { return fillColor; }
            set { fillColor = value; }
        }

        public double SymbolSize
        {
            get { return symbolSize; }
            set { symbolSize = value; }
        }

        public StockChartEnumerations.SymbolTypeEnum SymbolType
        {
            get { return symbolType; }
            set { symbolType = value; }
        }

        public List<LineData> Symbols
        {
            get { return _symbols; }
            set { _symbols = value; }
        }
    }
}

