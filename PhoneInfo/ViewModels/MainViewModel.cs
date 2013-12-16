/**
 * Copyright (c) 2013 Nokia Corporation.
 */

using System;
using System.Collections.ObjectModel;
using System.ComponentModel;

using PhoneInfo.Resources;

namespace PhoneInfo.ViewModels
{
    /// <summary>
    /// Generates and holds the items for the pivot page.
    /// </summary>
    public class MainViewModel : INotifyPropertyChanged
    {
        /// <summary>
        /// A collection for ItemViewModel objects.
        /// </summary>
        public ObservableCollection<BoolItemModel> CameraAndSensorItems { get; private set; }
        public ObservableCollection<BoolItemModel> BatteryAndPowerItems { get; private set; }
        public ObservableCollection<BoolItemModel> OtherItems { get; private set; }

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

        private string _displaySize;
        public string DisplaySize
        {
            get
            {
                return _displaySize;
            }
            private set
            {
                _displaySize = value;
                NotifyPropertyChanged("DisplaySize");
            }
        }

        private string _deviceTotalMemory;
        public string DeviceTotalMemory
        {
            get
            {
                return _deviceTotalMemory;
            }
            private set
            {
                _deviceTotalMemory = value;
                NotifyPropertyChanged("DeviceTotalMemory");
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

        private string _appMemoryPeak;
        public string AppMemoryPeak
        {
            get
            {
                return _appMemoryPeak;
            }
            set
            {
                _appMemoryPeak = value;
                NotifyPropertyChanged("AppMemoryPeak");
            }
        }

        private string _manufacturer;
        public string Manufacturer
        {
            get
            {
                return _manufacturer;
            }
            set
            {
                _manufacturer = value;
                NotifyPropertyChanged("Manufacturer");
            }
        }

        private string _hadrwareVersion;
        public string HardwareVersion
        {
            get
            {
                return _hadrwareVersion;
            }
            set
            {
                _hadrwareVersion = value;
                NotifyPropertyChanged("HardwareVersion");
            }
        }

        private string _firmwareVersion;
        public string FirmwareVersion
        {
            get
            {
                return _firmwareVersion;
            }
            set
            {
                _firmwareVersion = value;
                NotifyPropertyChanged("FirmwareVersion");
            }
        }

        private string _operator;
        public string Operator
        {
            get
            {
                return _operator;
            }
            set
            {
                _operator = value;
                NotifyPropertyChanged("Operator");
            }
        }

        private string _theme;
        public string Theme
        {
            get
            {
                return _theme;
            }
            set
            {
                _theme = value;
                NotifyPropertyChanged("Theme");
            }
        }

        private string _themeAccentColor;
        public string ThemeAccentColor
        {
            get
            {
                return _themeAccentColor;
            }
            set
            {
                _themeAccentColor = value;
                NotifyPropertyChanged("ThemeAccentColor");
            }
        }

        /// <summary>
        /// Constructor.
        /// </summary>
        public MainViewModel()
        {
            CameraAndSensorItems = new ObservableCollection<BoolItemModel>();
            BatteryAndPowerItems = new ObservableCollection<BoolItemModel>();
            OtherItems = new ObservableCollection<BoolItemModel>();
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

            foreach (BoolItemModel item in CameraAndSensorItems)
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

            ScreenResolution = AppResources.ScreenResolution + ": "
                + properties.ScreenResolution.ToString() + ", "
                + properties.ScreenResolutionSize.Width + "x" + properties.ScreenResolutionSize.Height
                + " " + AppResources.Pixels;

            if (properties.DisplaySizeInInches > 0)
            {
                DisplaySize = AppResources.DisplaySize + ": "
                    + properties.DisplaySizeInInches + " "
                    + AppResources.Inches;
            }
            else
            {
                DisplaySize = AppResources.DisplaySize + ": " + AppResources.NotAvailable;
            }

            try
            {
                MemoryUsedInPercentages =
                    (int)Math.Round((double)(100 * properties.ApplicationCurrentMemoryUsageInBytes
                    / properties.ApplicationMemoryUsageLimitInBytes));
                MemoryStatus =
                    DeviceProperties.TransformBytes(properties.ApplicationCurrentMemoryUsageInBytes, DeviceProperties.UnitPrefixes.Mega, 1)
                    + " MB " + AppResources.CurrentlyInUseOf + " "
                    + DeviceProperties.TransformBytes(properties.ApplicationMemoryUsageLimitInBytes, DeviceProperties.UnitPrefixes.Mega, 1)
                    + " MB";
            }
            catch (Exception)
            {
                MemoryStatus = AppResources.NotAvailable;
            }

            try
            {
                AppMemoryPeak = AppResources.AppMemoryPeakedAt + " "
                    + DeviceProperties.TransformBytes(properties.ApplicationPeakMemoryUsageInBytes, DeviceProperties.UnitPrefixes.Mega, 1)
                    + " MB";
            }
            catch (Exception)
            {
            }

            DeviceTotalMemory = DeviceProperties.TransformBytes(properties.DeviceTotalMemoryInBytes, DeviceProperties.UnitPrefixes.Mega, 0) + " MB";

            foreach (BoolItemModel item in BatteryAndPowerItems)
            {
                if (item.HardwareFeatureText.Equals(AppResources.BatteryStatusInfo))
                {
                    item.BooleanValue = properties.HasBatteryStatusInfo;
                }
                else if (item.HardwareFeatureText.Equals(AppResources.ConnectedToExternalPowerSupply))
                {
                    item.BooleanValue = properties.IsConnectedToExternalPowerSupply;
                }
            }

            Manufacturer = AppResources.Manufacturer + ": " + properties.Manufacturer;
            HardwareVersion = AppResources.HardwareVersion + ": " + properties.HardwareVersion;
            FirmwareVersion = AppResources.FirmwareVersion + ": " + properties.FirmwareVersion;
            Operator = (properties.Operator != null) ? properties.Operator : AppResources.NotAvailable;
            Theme = AppResources.Theme + ": " + (properties.HasDarkUiThemeInUse ? AppResources.Dark : AppResources.Light);
            ThemeAccentColor = AppResources.ThemeAccentColor + ": " + properties.ThemeAccentColor.ToString();

            foreach (BoolItemModel item in OtherItems)
            {
                if (item.HardwareFeatureText.Equals(AppResources.FMRadio))
                {
                    item.BooleanValue = properties.HasFMRadio;
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

            IsDataLoaded = true;
        }

        /// <summary>
        /// Creates the items.
        /// </summary>
        private void CreateItems()
        {
            CameraAndSensorItems.Add(new BoolItemModel() { BooleanValue = false, HardwareFeatureText = AppResources.Accelerometer });
            CameraAndSensorItems.Add(new BoolItemModel() { BooleanValue = false, HardwareFeatureText = AppResources.PrimaryCamera });
            CameraAndSensorItems.Add(new BoolItemModel() { BooleanValue = false, HardwareFeatureText = AppResources.PrimaryCameraFlash });
            CameraAndSensorItems.Add(new BoolItemModel() { BooleanValue = false, HardwareFeatureText = AppResources.Compass });
            CameraAndSensorItems.Add(new BoolItemModel() { BooleanValue = false, HardwareFeatureText = AppResources.FrontCamera });
            CameraAndSensorItems.Add(new BoolItemModel() { BooleanValue = false, HardwareFeatureText = AppResources.FrontCameraFlash });
            CameraAndSensorItems.Add(new BoolItemModel() { BooleanValue = false, HardwareFeatureText = AppResources.Gyroscope });
            CameraAndSensorItems.Add(new BoolItemModel() { BooleanValue = false, HardwareFeatureText = AppResources.Inclinometer });
            CameraAndSensorItems.Add(new BoolItemModel() { BooleanValue = false, HardwareFeatureText = AppResources.MotionApi });
            CameraAndSensorItems.Add(new BoolItemModel() { BooleanValue = false, HardwareFeatureText = AppResources.OrientationSensor });
            CameraAndSensorItems.Add(new BoolItemModel() { BooleanValue = false, HardwareFeatureText = AppResources.NFC });

            BatteryAndPowerItems.Add(new BoolItemModel() { BooleanValue = false, HardwareFeatureText = AppResources.BatteryStatusInfo });
            BatteryAndPowerItems.Add(new BoolItemModel() { BooleanValue = false, HardwareFeatureText = AppResources.ConnectedToExternalPowerSupply });

            OtherItems.Add(new BoolItemModel() { BooleanValue = false, HardwareFeatureText = AppResources.FMRadio });
            OtherItems.Add(new BoolItemModel() { BooleanValue = false, HardwareFeatureText = AppResources.SDCard });
            OtherItems.Add(new BoolItemModel() { BooleanValue = false, HardwareFeatureText = AppResources.VibrationDevice });

            ScreenResolution = AppResources.Waiting;
            DeviceTotalMemory = AppResources.Waiting;
            MemoryStatus = AppResources.Waiting;
            Manufacturer = AppResources.Waiting;
            Operator = AppResources.Waiting;
            Theme = AppResources.Waiting;
        }

        public event PropertyChangedEventHandler PropertyChanged;
        private void NotifyPropertyChanged(string propertyName)
        {
            PropertyChangedEventHandler handler = PropertyChanged;
            if (null != handler)
            {
                handler(this, new PropertyChangedEventArgs(propertyName));
            }
        }
    }
}