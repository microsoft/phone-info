/*
 * Copyright (c) 2013-2014 Microsoft Mobile. All rights reserved.
 * See the license text file delivered with this project for more information.
 */

using PhoneInfo.Common;
using PhoneInfo.DataModel;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Threading;
using System.Threading.Tasks;
using Windows.ApplicationModel.Resources;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.Graphics.Display;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;

// The Pivot Application template is documented at http://go.microsoft.com/fwlink/?LinkID=391641

namespace PhoneInfo
{
    public sealed partial class PivotPage : Page
    {
        // Constants
        private const string DebugTag = "PivotPage: ";
        private const int ProgressBarDelay = 2000; // Milliseconds

        // Properties and members

        private NavigationHelper navigationHelper;
        private ResourceLoader _resourceLoader;
        private Timer _progressBarTimer;

        /// <summary>
        /// Gets the <see cref="NavigationHelper"/> associated with this <see cref="Page"/>.
        /// </summary>
        public NavigationHelper NavigationHelper
        {
            get { return this.navigationHelper; }
        }

        public DeviceProperties Resolver
        {
            get;
            private set;
        }

        public PivotPage()
        {
            this.InitializeComponent();

            //this.NavigationCacheMode = NavigationCacheMode.Required;

            this.navigationHelper = new NavigationHelper(this);
            this.navigationHelper.LoadState += this.NavigationHelper_LoadState;
            this.navigationHelper.SaveState += this.NavigationHelper_SaveState;

            _resourceLoader = new ResourceLoader();
            DataContext = App.ViewModel;
            Resolver = DeviceProperties.GetInstance();
            Resolver.IsReadyChanged += OnPropertiesResolvedChanged;
            Loaded += OnPageLoaded;
            MyPivot.SelectionChanged += OnPivotSelectionChanged;
        }

        private void OnPropertiesResolvedChanged(object sender, bool e)
        {
            if (e)
            {
                App.ViewModel.LoadData();
                RemainingBatteryChargeTextBlock.Text = string.Format(CultureInfo.CurrentCulture, _resourceLoader.GetString("PercentageOfBatteryChargeRemaining/Text"), App.ViewModel.RemainingBatteryCharge);
                ProcessorCoreCountTextBlock.Text = string.Format(CultureInfo.CurrentCulture, _resourceLoader.GetString("NumberOfCores/Text"), App.ViewModel.ProcessorCoreCount);

                HideProgressBar(null);
                ThemeAccentColorRectangle.Visibility = Visibility.Visible;
                RefreshButton.IsEnabled = true;
            }
        }

        #region Navigation helper methods

        /// <summary>
        /// Populates the page with content passed during navigation. Any saved state is also
        /// provided when recreating a page from a prior session.
        /// </summary>
        /// <param name="sender">
        /// The source of the event; typically <see cref="NavigationHelper"/>.
        /// </param>
        /// <param name="e">Event data that provides both the navigation parameter passed to
        /// <see cref="Frame.Navigate(Type, Object)"/> when this page was initially requested and
        /// a dictionary of state preserved by this page during an earlier
        /// session. The state will be null the first time a page is visited.</param>
        private void NavigationHelper_LoadState(object sender, LoadStateEventArgs e)
        {
        }

        /// <summary>
        /// Preserves state associated with this page in case the application is suspended or the
        /// page is discarded from the navigation cache. Values must conform to the serialization
        /// requirements of <see cref="SuspensionManager.SessionState"/>.
        /// </summary>
        /// <param name="sender">The source of the event; typically <see cref="NavigationHelper"/>.</param>
        /// <param name="e">Event data that provides an empty dictionary to be populated with
        /// serializable state.</param>
        private void NavigationHelper_SaveState(object sender, SaveStateEventArgs e)
        {
            // TODO: Save the unique state of the page here.
        }

        #endregion Navigation helper

        /// <summary>
        /// Retrieves the hardware information asynchronously.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private async void OnPageLoaded(object sender, RoutedEventArgs e)
        {
            await Task.Factory.StartNew(Resolver.Resolve);
            await Task.Delay(TimeSpan.FromMilliseconds(ProgressBarDelay));
        }

        private void OnPivotSelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (MyPivot.SelectedIndex == 1) // Dynamic
            {
                RefreshButton.Visibility = Visibility.Visible;
            }
            else
            {
                RefreshButton.Visibility = Visibility.Collapsed;
            }

            /* The header of the selected pivot item should be white where as
             * others should be slightly darker
             */
            foreach (PivotItem pivotItem in MyPivot.Items)
            {
                if (pivotItem == MyPivot.Items[MyPivot.SelectedIndex])
                {
                    ((TextBlock)pivotItem.Header).Foreground = new SolidColorBrush(Windows.UI.Color.FromArgb(255, 255, 255, 255));
                }
                else
                {
                    ((TextBlock)pivotItem.Header).Foreground = new SolidColorBrush(Windows.UI.Color.FromArgb(255, 220, 220, 220));
                }
            }
        }

        private async void HideProgressBar(object state)
        {
            System.Diagnostics.Debug.WriteLine(DebugTag + "HideProgressBar()");
            await Windows.ApplicationModel.Core.CoreApplication.MainView.CoreWindow.Dispatcher.RunAsync(
                Windows.UI.Core.CoreDispatcherPriority.Normal, () =>
                {
                    MyProgressBar.Visibility = Visibility.Collapsed;
                });

            if (_progressBarTimer != null)
            {
                _progressBarTimer.Dispose();
                _progressBarTimer = null;
            }
        }

        private async void RefreshButton_Click(object sender, RoutedEventArgs e)
        {
            RefreshButton.IsEnabled = false;
            MyProgressBar.Visibility = Visibility.Visible;

            if (_progressBarTimer != null)
            {
                _progressBarTimer.Dispose();
                _progressBarTimer = null;
            }

            _progressBarTimer = new Timer(HideProgressBar, null,
                TimeSpan.FromMilliseconds(ProgressBarDelay),
                TimeSpan.FromMilliseconds(ProgressBarDelay));

            await Task.Factory.StartNew(Resolver.Resolve);
        }

        private void AboutButton_Click(object sender, RoutedEventArgs e)
        {
            Frame.Navigate(typeof(AboutPage));
        }

        #region NavigationHelper registration

        /// <summary>
        /// The methods provided in this section are simply used to allow
        /// NavigationHelper to respond to the page's navigation methods.
        /// <para>
        /// Page specific logic should be placed in event handlers for the  
        /// <see cref="NavigationHelper.LoadState"/>
        /// and <see cref="NavigationHelper.SaveState"/>.
        /// The navigation parameter is available in the LoadState method 
        /// in addition to page state preserved during an earlier session.
        /// </para>
        /// </summary>
        /// <param name="e">Provides data for navigation methods and event
        /// handlers that cannot cancel the navigation request.</param>
        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            this.navigationHelper.OnNavigatedTo(e);
        }

        protected override void OnNavigatedFrom(NavigationEventArgs e)
        {
            this.navigationHelper.OnNavigatedFrom(e);
        }


        #endregion
    }
}
