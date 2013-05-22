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

        // Members
        private HardwareInfoResolver _resolver = null;

        /// <summary>
        /// Constructor.
        /// 构造函数
        /// </summary>
        public MainPage()
        {
            InitializeComponent();
            DataContext = App.ViewModel;
            _resolver = new HardwareInfoResolver();
            Loaded += MainPage_Loaded;
        }

        /// <summary>
        /// Retrieves the hardware information asynchronously.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private async void MainPage_Loaded(object sender, RoutedEventArgs e)
        {
            await Task.Factory.StartNew(_resolver.ResolveInfo);
            System.Threading.Thread.Sleep(ProgressBarDelay);
            App.ViewModel.LoadData(ref _resolver);
            MyProgressBar.Visibility = Visibility.Collapsed;
        }
    }
}