// Copyright (c) 2012, Adam Duval Kangaroo Point South 4169, Australia.
// All Rights Reserved.
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Adam.Trading.Mechanical.ViewModel
{
    public class StockChartEnumerations
    {
        public enum LinePattern
        {
            Solid = 1,
            Dash = 2,
            Dot = 3,
            DashDot = 4,
            None = 5
        }

        public enum StockChartType
        {
            HiLoOpenClose = 0,
            Candle = 1,
        }

        public enum GridlinePattern
        {
            Solid = 1,
            Dash = 2,
            Dot = 3,
            DashDot = 4
        }

        public enum SymbolTypeEnum
        {
            Box = 0,
            Circle = 1,
            Cross = 2,
            Diamond = 3,
            Dot = 4,
            InvertedTriangle = 5,
            None = 6,
            OpenDiamond = 7,
            OpenInvertedTriangle = 8,
            OpenTriangle = 9,
            Square = 10,
            Star = 11,
            Triangle = 12,
            Plus = 13
        }
    }
}
