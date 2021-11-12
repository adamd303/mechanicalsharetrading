// Copyright (c) 2012, Adam Duval Kangaroo Point South 4169, Australia.
// All Rights Reserved.
using System;
using System.Windows.Input;

namespace Adam.Trading.Mechanical.View.Command
{
    public static class MainWindowCommands
    {
        public readonly static RoutedUICommand ShowSupportResistanceWindow;
        public readonly static RoutedUICommand ShowKrauseGannWindow;
        public readonly static RoutedUICommand ShowGann2DaySwingWindow;

        static MainWindowCommands()
        {
            ShowSupportResistanceWindow = new RoutedUICommand(Properties.Resources.SupportResistanceSystem_Show_Tooltip, "ShowSupportResistanceWindow", typeof(MainWindowCommands));
            ShowKrauseGannWindow = new RoutedUICommand(Properties.Resources.KrauseSystem_Show_Tooltip, "ShowKrauseGannWindow", typeof(MainWindowCommands));
            ShowGann2DaySwingWindow = new RoutedUICommand(Properties.Resources.Gann2DaySwingSystem_Show_Tooltip, "ShowGann2DaySwingWindow", typeof(MainWindowCommands));
        }
    }
}
