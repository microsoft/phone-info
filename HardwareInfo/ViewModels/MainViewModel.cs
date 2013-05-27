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
        /// <param name="resolver">An instance of hardware resolver class.</param>
        public void LoadData(ref HardwareInfoResolver resolver)
        {
            foreach (ItemModel item in Items)
            {
                if (item.HardwareFeatureText.Equals(AppResources.Accelerometer))
                {
                    item.BooleanValue = resolver.AccelerometerExists;
                }
                else if (item.HardwareFeatureText.Equals(AppResources.PrimaryCamera))
                {
                    item.BooleanValue = resolver.BackCameraExists;
                }
                else if (item.HardwareFeatureText.Equals(AppResources.PrimaryCameraFlash))
                {
                    item.BooleanValue = resolver.BackCameraFlashExists;
                }
                else if (item.HardwareFeatureText.Equals(AppResources.Compass))
                {
                    item.BooleanValue = resolver.CompassExists;
                }
                else if (item.HardwareFeatureText.Equals(AppResources.FMRadio))
                {
                    item.BooleanValue = resolver.FMRadioExists;
                }
                else if (item.HardwareFeatureText.Equals(AppResources.FrontCamera))
                {
                    item.BooleanValue = resolver.FrontCameraExists;
                }
                else if (item.HardwareFeatureText.Equals(AppResources.FrontCameraFlash))
                {
                    item.BooleanValue = resolver.FrontCameraFlashExists;
                }
                else if (item.HardwareFeatureText.Equals(AppResources.Gyroscope))
                {
                    item.BooleanValue = resolver.GyroscopeExists;
                }
                else if (item.HardwareFeatureText.Equals(AppResources.Inclinometer))
                {
                    item.BooleanValue = resolver.InclinometerExists;
                }
                else if (item.HardwareFeatureText.Equals(AppResources.MotionApi))
                {
                    item.BooleanValue = resolver.MotionApiAvailable;
                }
                else if (item.HardwareFeatureText.Equals(AppResources.OrientationSensor))
                {
                    item.BooleanValue = resolver.OrientationSensorExists;
                }
                else if (item.HardwareFeatureText.Equals(AppResources.NFC))
                {
                    item.BooleanValue = resolver.ProximityExists;
                }
                else if (item.HardwareFeatureText.Equals(AppResources.SDCard))
                {
                    item.BooleanValue = resolver.SDCardExists;
                }
                else if (item.HardwareFeatureText.Equals(AppResources.VibrationDevice))
                {
                    item.BooleanValue = resolver.VibrationDeviceExists;
                }
            }

            ScreenResolution = resolver.ScreenResolution;

            try
            {
                MemoryUsedInPercentages =
                    (int)Math.Round(100 * Double.Parse(resolver.MemoryCurrentUsed)
                    / Double.Parse(resolver.MemoryMaxAvailable));
                MemoryStatus =
                    (int.Parse(resolver.MemoryCurrentUsed) / (1024 * 1024)) + " MB used of "
                    + (int.Parse(resolver.MemoryMaxAvailable) / (1024 * 1024)) + " MB";
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