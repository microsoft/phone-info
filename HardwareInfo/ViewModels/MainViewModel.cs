/**
 * Copyright (c) 2013 Nokia Corporation.
 */

using System;
using System.Collections.ObjectModel;
using System.ComponentModel;

using HardwareInfo.Resources;

namespace HardwareInfo.ViewModels
{
    /// <summary>
    /// Generates and holds the items for the pivot page.
    /// </summary>
    public class MainViewModel : INotifyPropertyChanged
    {
        /// <summary>
        /// A collection for ItemViewModel objects.
        /// </summary>
        public ObservableCollection<ItemModel> Items { get; private set; }

        public bool IsDataLoaded
        {
            get;
            private set;
        }

        private string _screenResolution;
        public string ScreenResolution
        {
            get
            {
                return _screenResolution;
            }
            private set
            {
                _screenResolution = value;
                NotifyPropertyChanged("ScreenResolution");
            }
        }

        private int _memoryUsedInPercentages;
        public int MemoryUsedInPercentages
        {
            get
            {
                return _memoryUsedInPercentages;
            }
            set
            {
                _memoryUsedInPercentages = value;
                NotifyPropertyChanged("MemoryUsedInPercentages");
            }
        }

        private string _memoryStatus;
        public string MemoryStatus
        {
            get
            {
                return _memoryStatus;
            }
            set
            {
                _memoryStatus = value;
                NotifyPropertyChanged("MemoryStatus");
            }
        }

        /// <summary>
        /// Constructor.
        /// </summary>
        public MainViewModel()
        {
            Items = new ObservableCollection<ItemModel>();
            CreateItems();
        }

        /// <summary>
        /// Sets the values of the items.
        /// </summary>
        public void LoadData()
        {
            DeviceProperties properties = DeviceProperties.GetInstance();

            if (properties.Initialized == false)
            {
                return;
            }

            foreach (ItemModel item in Items)
            {
                if (item.HardwareFeatureText.Equals(AppResources.Accelerometer))
                {
                    item.BooleanValue = properties.HasAccelerometerSensor;
                }
                else if (item.HardwareFeatureText.Equals(AppResources.PrimaryCamera))
                {
                    item.BooleanValue = properties.HasBackCamera;
                }
                else if (item.HardwareFeatureText.Equals(AppResources.PrimaryCameraFlash))
                {
                    item.BooleanValue = properties.HasBackCameraFlash;
                }
                else if (item.HardwareFeatureText.Equals(AppResources.Compass))
                {
                    item.BooleanValue = properties.HasCompass;
                }
                else if (item.HardwareFeatureText.Equals(AppResources.FMRadio))
                {
                    item.BooleanValue = properties.HasFMRadio;
                }
                else if (item.HardwareFeatureText.Equals(AppResources.FrontCamera))
                {
                    item.BooleanValue = properties.HasFrontCamera;
                }
                else if (item.HardwareFeatureText.Equals(AppResources.FrontCameraFlash))
                {
                    item.BooleanValue = properties.HasFrontCameraFlash;
                }
                else if (item.HardwareFeatureText.Equals(AppResources.Gyroscope))
                {
                    item.BooleanValue = properties.HasGyroscopeSensor;
                }
                else if (item.HardwareFeatureText.Equals(AppResources.Inclinometer))
                {
                    item.BooleanValue = properties.HasInclinometerSensor;
                }
                else if (item.HardwareFeatureText.Equals(AppResources.MotionApi))
                {
                    item.BooleanValue = properties.MotionApiAvailable;
                }
                else if (item.HardwareFeatureText.Equals(AppResources.OrientationSensor))
                {
                    item.BooleanValue = properties.HasOrientationSensor;
                }
                else if (item.HardwareFeatureText.Equals(AppResources.NFC))
                {
                    item.BooleanValue = properties.HasProximitySensor;
                }
                else if (item.HardwareFeatureText.Equals(AppResources.SDCard))
                {
                    item.BooleanValue = properties.HasSDCardPresent;
                }
                else if (item.HardwareFeatureText.Equals(AppResources.VibrationDevice))
                {
                    item.BooleanValue = properties.HasVibrationDevice;
                }
            }

            ScreenResolution = properties.ScreenResolution;

            try
            {
                MemoryUsedInPercentages =
                    (int)Math.Round(100 * Double.Parse(properties.MemoryCurrentUsed)
                    / Double.Parse(properties.MemoryMaxAvailable));
                MemoryStatus =
                    (int.Parse(properties.MemoryCurrentUsed) / (1024 * 1024)) + " MB used of "
                    + (int.Parse(properties.MemoryMaxAvailable) / (1024 * 1024)) + " MB";
            }
            catch (Exception)
            {
                MemoryStatus = AppResources.NotAvailable;
            }

            IsDataLoaded = true;
        }

        /// <summary>
        /// Creates the items.
        /// </summary>
        private void CreateItems()
        {
            Items.Add(new ItemModel() { BooleanValue = false, HardwareFeatureText = AppResources.Accelerometer });
            Items.Add(new ItemModel() { BooleanValue = false, HardwareFeatureText = AppResources.PrimaryCamera });
            Items.Add(new ItemModel() { BooleanValue = false, HardwareFeatureText = AppResources.PrimaryCameraFlash });
            Items.Add(new ItemModel() { BooleanValue = false, HardwareFeatureText = AppResources.Compass });
            Items.Add(new ItemModel() { BooleanValue = false, HardwareFeatureText = AppResources.FMRadio });
            Items.Add(new ItemModel() { BooleanValue = false, HardwareFeatureText = AppResources.FrontCamera });
            Items.Add(new ItemModel() { BooleanValue = false, HardwareFeatureText = AppResources.FrontCameraFlash });
            Items.Add(new ItemModel() { BooleanValue = false, HardwareFeatureText = AppResources.Gyroscope });
            Items.Add(new ItemModel() { BooleanValue = false, HardwareFeatureText = AppResources.Inclinometer });
            Items.Add(new ItemModel() { BooleanValue = false, HardwareFeatureText = AppResources.MotionApi });
            Items.Add(new ItemModel() { BooleanValue = false, HardwareFeatureText = AppResources.OrientationSensor });
            Items.Add(new ItemModel() { BooleanValue = false, HardwareFeatureText = AppResources.NFC });
            Items.Add(new ItemModel() { BooleanValue = false, HardwareFeatureText = AppResources.SDCard });
            Items.Add(new ItemModel() { BooleanValue = false, HardwareFeatureText = AppResources.VibrationDevice });
            
            ScreenResolution = AppResources.Waiting;
            MemoryStatus = AppResources.Waiting;
        }

        public event PropertyChangedEventHandler PropertyChanged;
        private void NotifyPropertyChanged(String propertyName)
        {
            PropertyChangedEventHandler handler = PropertyChanged;
            if (null != handler)
            {
                handler(this, new PropertyChangedEventArgs(propertyName));
            }
        }
    }
}