/**
 * Copyright (c) 2013-2014 Microsoft Mobile.
 */

namespace PhoneInfo
{
    using System;
    using System.Net;
    using System.Threading;
    using System.Threading.Tasks;
    using System.Windows;
    using Microsoft.Phone.Controls;
    using Microsoft.Phone.Shell;

    using PhoneInfo.Resources;
    
    /// <summary>
    /// The application main page.
    /// </summary>
    public partial class MainPage : PhoneApplicationPage
    {
        // Constants
        private const string RefreshIconUri = "/Assets/refresh.png";
        private const int ProgressBarDelay = 2000; // Milliseconds
        
        // Properties and members
        
        public DeviceProperties Resolver
        {
            get;
            private set;
        }

        private ApplicationBarIconButton _refreshButton = null;
        private Timer _progressBarTimer = null;

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
            HideProgressBar(null);
            ThemeAccentColorRectangle.Visibility = Visibility.Visible;
            BuildLocalizedApplicationBar();
        }

        private void BuildLocalizedApplicationBar()
        {
            ApplicationBar = new ApplicationBar();

            // Create a new button and set the text value to the localized
            // string from AppResources.
            _refreshButton = new ApplicationBarIconButton(new Uri(RefreshIconUri, UriKind.Relative));
            _refreshButton.Text = AppResources.Refresh;
            _refreshButton.Click += refreshButton_Click;
            ApplicationBar.Buttons.Add(_refreshButton);

            // Create menu items with the localized string from AppResources
            ApplicationBarMenuItem aboutMenuItem = new ApplicationBarMenuItem(AppResources.About);
            aboutMenuItem.Click += aboutMenuItem_Click;
            ApplicationBar.MenuItems.Add(aboutMenuItem);
        }

        private void HideProgressBar(object state)
        {
            System.Diagnostics.Debug.WriteLine("MainPage.HideProgressBar()");
            Dispatcher.BeginInvoke(() => MyProgressBar.Visibility = Visibility.Collapsed);

            if (_progressBarTimer != null)
            {
                _progressBarTimer.Dispose();
                _progressBarTimer = null;
            }
        }

        private async void refreshButton_Click(object sender, EventArgs e)
        {
            _refreshButton.IsEnabled = false;
            MyProgressBar.Visibility = Visibility.Visible;

            if (_progressBarTimer != null)
            {
                _progressBarTimer.Dispose();
                _progressBarTimer = null;
            }

            _progressBarTimer = new Timer(HideProgressBar, null,
                TimeSpan.FromMilliseconds(ProgressBarDelay),
                TimeSpan.FromMilliseconds(ProgressBarDelay));

            await Task.Factory.StartNew(Resolver.Refresh);
            App.ViewModel.LoadData();
            _refreshButton.IsEnabled = true;            
        }

        void aboutMenuItem_Click(object sender, EventArgs e)
        {
            NavigationService.Navigate(new Uri("/AboutPage.xaml", UriKind.Relative));
        }
    }
}