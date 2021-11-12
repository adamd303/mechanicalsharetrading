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

namespace Adam.Trading.Mechanical.View.SubViews
{
    /// <summary>
    /// Interaction logic for SupportResistanceView.xaml
    /// </summary>
    public partial class SupportResistanceView : UserControl
    {
        public SupportResistanceView()
        {
            InitializeComponent();
        }

        private void FileSelector_Clicked(object sender, RoutedEventArgs e)
        {
            var viewModel = this.DataContext as SupportResistanceViewModel;
            if (viewModel != null)
            {
                viewModel.Load();
            }
        }
    }
}
