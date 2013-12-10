/**
 * Copyright (c) 2013 Nokia Corporation.
 */

using Microsoft.Phone.Controls;
using Microsoft.Phone.Shell;

using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Navigation;

using HardwareInfo.Resources;
using HardwareInfo.ViewModels;


namespace HardwareInfo
{
    /// <summary>
    /// The application main page.
    /// </summary>
    public partial class MainPage : PhoneApplicationPage
    {
        // Constants
        private const int ProgressBarDelay = 2500; // Milliseconds

        public DeviceProperties Resolver
        {
            get;
            private set;
        }

        /// <summary>
        /// Constructor.
        /// </summary>
        public MainPage()
        {
            InitializeComponent();
            DataContext = App.ViewModel;
            Resolver = DeviceProperties.GetInstance();
            Loaded += MainPage_Loaded;
        }

        /// <summary>
        /// Retrieves the hardware information asynchronously.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private async void MainPage_Loaded(object sender, RoutedEventArgs e)
        {
            await Task.Factory.StartNew(Resolver.Init);
            System.Threading.Thread.Sleep(ProgressBarDelay);
            App.ViewModel.LoadData();
            MyProgressBar.Visibility = Visibility.Collapsed;
        }
    }
}