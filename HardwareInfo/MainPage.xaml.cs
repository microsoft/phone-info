/**
 * Copyright (c) 2013 Nokia Corporation.
 */

using Microsoft.Phone.Controls;
using Microsoft.Phone.Shell;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Navigation;

using HardwareInfo.Resources;

namespace HardwareInfo
{
    /// <summary>
    /// The application main page.
    /// </summary>
    public partial class MainPage : PhoneApplicationPage
    {
        private const int ProgressBarDelay = 2500; // Milliseconds
        private HardwareInfoResolver hwInfo = null;

        /// <summary>
        /// Constructor.
        /// 构造函数
        /// </summary>
        public MainPage()
        {
            InitializeComponent();
            hwInfo = new HardwareInfoResolver();
            Loaded += MainPage_Loaded;
        }

        /// <summary>
        /// Retrieves the hardware information asynchronously.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private async void MainPage_Loaded(object sender, RoutedEventArgs e)
        {
            await Task.Factory.StartNew(hwInfo.ResolveInfo);
            System.Threading.Thread.Sleep(ProgressBarDelay);
            ContentLayout.DataContext = hwInfo;
            MyProgressBar.Visibility = Visibility.Collapsed;
        }
    }
}