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
using System.Windows.Shapes;
using Adam.Trading.Mechanical.ViewModel;

namespace Adam.Trading.Mechanical.View.SubViews
{
    /// <summary>
    /// Interaction logic for Gann2DayView.xaml
    /// </summary>
    public partial class Gann2DayView : UserControl
    {
        public Gann2DayView()
        {
            InitializeComponent();
        }

        private void FileSelector_Clicked(object sender, RoutedEventArgs e)
        {
            var viewModel = this.DataContext as Gann2DayViewModel;
            if (viewModel != null)
            {
                viewModel.Load();
            }
        }
    }
}