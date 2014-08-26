/*
 * Copyright (c) 2013-2014 Microsoft Mobile. All rights reserved.
 * See the license text file delivered with this project for more information.
 */

using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using PhoneInfo;
using Windows.ApplicationModel.Resources;

namespace PhoneInfo.DataModel
{
    /// <summary>
    /// Generates and holds the items for the pivot page.
    /// </summary>
    public class MainViewModel : INotifyPropertyChanged
    {
        /// <summary>
        /// A collection for ItemViewModel objects.
        /// </summary>
        public ObservableCollection<BoolItemModel> SensorItems { get; private set; }
        public ObservableCollection<BoolItemModel> CameraItems { get; private set; }
        public ObservableCollection<BoolItemModel> SensorCoreItems { get; private set; }

        public bool IsDataLoaded
        {
            get;
            private set;
        }

        private string _backCameraResolutions;
        public string BackCameraResolutions
        {
            get
            {
                return _backCameraResolutions;
            }
            private set
            {
                _backCameraResolutions = value;
                NotifyPropertyChanged("BackCameraResolutions");
            }
        }

        private string _deviceName;
        public string DeviceName
        {
            get
            {
                return _deviceName;
            }
            private set
            {
                _deviceName = value;
                NotifyPropertyChanged("DeviceName");
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

        private string _firmwareVersion;
        public string FirmwareVersion
        {
            get
            {
                return _firmwareVersion;
            }
            private set
            {
                _firmwareVersion = value;
                NotifyPropertyChanged("FirmwareVersion");
            }
        }

        private string _frontCameraResolutions;
        public string FrontCameraResolutions
        {
            get
            {
                return _frontCameraResolutions;
            }
            private set
            {
                _frontCameraResolutions = value;
                NotifyPropertyChanged("FrontCameraResolutions");
            }
        }

        private string _hardwareVersion;
        public string HardwareVersion
        {
            get
            {
                return _hardwareVersion;
            }
            private set
            {
                _hardwareVersion = value;
                NotifyPropertyChanged("HardwareVersion");
            }
        }

        private bool _hasBatteryStatusInfo;
        public bool HasBatteryStatusInfo
        {
            get
            {
                return _hasBatteryStatusInfo;
            }
            set
            {
                _hasBatteryStatusInfo = value;
                NotifyPropertyChanged("HasBatteryStatusInfo");
            }
        }

        private string _manufacturer;
        public string Manufacturer
        {
            get
            {
                return _manufacturer;
            }
            private set
            {
                _manufacturer = value;
                NotifyPropertyChanged("Manufacturer");
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

        private bool _powerSavingModeEnabled;
        public bool PowerSavingModeEnabled
        {
            get
            {
                return _powerSavingModeEnabled;
            }
            set
            {
                _powerSavingModeEnabled = value;
                NotifyPropertyChanged("PowerSavingModeEnabled");
            }
        }

        private int _processorCoreCount;
        public int ProcessorCoreCount
        {
            get
            {
                return _processorCoreCount;
            }
            private set
            {
                _processorCoreCount = value;
                NotifyPropertyChanged("ProcessorCoreCount");
            }
        }

        private int _remainingBatteryCharge;
        public int RemainingBatteryCharge
        {
            get
            {
                return _remainingBatteryCharge;
            }
            private set
            {
                _remainingBatteryCharge = value;
                NotifyPropertyChanged("RemainingBatteryCharge");
            }
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

        private string _sdCardPresent;
        public string SDCardPresent
        {
            get
            {
                return _sdCardPresent;
            }
            private set
            {
                _sdCardPresent = value;
                NotifyPropertyChanged("SDCardPresent");
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

        private string _vibrationDeviceAvailable;
        public string VibrationDeviceAvailable
        {
            get
            {
                return _vibrationDeviceAvailable;
            }
            private set
            {
                _vibrationDeviceAvailable = value;
                NotifyPropertyChanged("VibrationDeviceAvailable");
            }
        }

        private ResourceLoader _resourceLoader;

        /// <summary>
        /// Constructor.
        /// </summary>
        public MainViewModel()
        {
            SensorItems = new ObservableCollection<BoolItemModel>();
            CameraItems = new ObservableCollection<BoolItemModel>();
            SensorCoreItems = new ObservableCollection<BoolItemModel>();
            _resourceLoader = new ResourceLoader();
            CreateItems();
        }

        /// <summary>
        /// Sets the values of the items.
        /// </summary>
        public void LoadData()
        {
            DeviceProperties properties = DeviceProperties.GetInstance();

            if (properties.IsReady == false)
            {
                return;
            }

            foreach (BoolItemModel item in SensorItems)
            {
                if (item.HardwareFeatureText.Equals(_resourceLoader.GetString("Accelerometer/Text")))
                {
                    item.BooleanValue = properties.HasAccelerometerSensor;
                }
                else if (item.HardwareFeatureText.Equals(_resourceLoader.GetString("Compass/Text")))
                {
                    item.BooleanValue = properties.HasCompass;
                }
                else if (item.HardwareFeatureText.Equals(_resourceLoader.GetString("Gyroscope/Text")))
                {
                    item.BooleanValue = properties.HasGyroscopeSensor;
                }
                else if (item.HardwareFeatureText.Equals(_resourceLoader.GetString("Inclinometer/Text")))
                {
                    item.BooleanValue = properties.HasInclinometerSensor;
                }
                else if (item.HardwareFeatureText.Equals(_resourceLoader.GetString("OrientationSensor/Text")))
                {
                    item.BooleanValue = properties.HasOrientationSensor;
                }
                else if (item.HardwareFeatureText.Equals(_resourceLoader.GetString("NFC/Text")))
                {
                    item.BooleanValue = properties.HasProximitySensor;
                }
            }

            foreach (BoolItemModel item in CameraItems)
            {
                if (item.HardwareFeatureText.Equals(_resourceLoader.GetString("BackCamera/Text")))
                {
                    item.BooleanValue = properties.HasBackCamera;
                }
                else if (item.HardwareFeatureText.Equals(_resourceLoader.GetString("BackCameraFlash/Text")))
                {
                    item.BooleanValue = properties.HasBackCameraFlash;
                }
                else if (item.HardwareFeatureText.Equals(_resourceLoader.GetString("BackCameraAutoFocus/Text")))
                {
                    item.BooleanValue = properties.HasBackCameraAutoFocus;
                }
                else if (item.HardwareFeatureText.Equals(_resourceLoader.GetString("FrontCamera/Text")))
                {
                    item.BooleanValue = properties.HasFrontCamera;
                }
                else if (item.HardwareFeatureText.Equals(_resourceLoader.GetString("FrontCameraFlash/Text")))
                {
                    item.BooleanValue = properties.HasFrontCameraFlash;
                }
                else if (item.HardwareFeatureText.Equals(_resourceLoader.GetString("FrontCameraAutoFocus/Text")))
                {
                    item.BooleanValue = properties.HasFrontCameraAutoFocus;
                }
            }

            foreach (BoolItemModel item in SensorCoreItems)
            {
                if (item.HardwareFeatureText.Equals(_resourceLoader.GetString("ActivityMonitor/Text")))
                {
                    item.BooleanValue = properties.SensorCoreActivityMonitorApiSupported;
                }
                else if (item.HardwareFeatureText.Equals(_resourceLoader.GetString("PlaceMonitor/Text")))
                {
                    item.BooleanValue = properties.SensorCorePlaceMonitorApiSupported;
                }
                else if (item.HardwareFeatureText.Equals(_resourceLoader.GetString("StepCounter/Text")))
                {
                    item.BooleanValue = properties.SensorCoreStepCounterApiSupported;
                }
                else if (item.HardwareFeatureText.Equals(_resourceLoader.GetString("TrackPointMonitor/Text")))
                {
                    item.BooleanValue = properties.SensorCoreTrackPointMonitorApiSupported;
                }
            }

            if (properties.HasBackCamera
                && (properties.BackCameraPhotoResolutions.Count > 0
                    || properties.BackCameraVideoResolutions.Count > 0))
            {
                string backCameraResolutions = properties.BackCameraPhotoResolutions.Aggregate("Photo capture:\n", (current, size) => current + ("  " + size.Width + "x" + size.Height + "\n"));

                backCameraResolutions += "\nVideo capture:\n";

                backCameraResolutions = properties.BackCameraVideoResolutions.Aggregate(backCameraResolutions, (current, size) => current + ("  " + size.Width + "x" + size.Height + "\n"));

                BackCameraResolutions = backCameraResolutions;
            }
            else
            {
                BackCameraResolutions = _resourceLoader.GetString("NotAvailable/Text");
            }

            if (properties.HasFrontCamera
                && (properties.FrontCameraPhotoResolutions.Count > 0
                    || properties.FrontCameraVideoResolutions.Count > 0))
            {
                string frontCameraResolutions = properties.FrontCameraPhotoResolutions.Aggregate("Photo capture:\n", (current, size) => current + ("  " + size.Width + "x" + size.Height + "\n"));

                frontCameraResolutions += "\nVideo capture:\n";

                frontCameraResolutions = properties.FrontCameraVideoResolutions.Aggregate(frontCameraResolutions, (current, size) => current + ("  " + size.Width + "x" + size.Height + "\n"));

                FrontCameraResolutions = frontCameraResolutions;
            }
            else
            {
                FrontCameraResolutions = _resourceLoader.GetString("NotAvailable/Text");
            }
            
            HasBatteryStatusInfo = properties.HasBatteryStatusInfo;
            RemainingBatteryCharge = properties.RemainingBatteryCharge;
            PowerSavingModeEnabled = properties.PowerSavingModeEnabled;

            ScreenResolution = _resourceLoader.GetString("ScreenResolution/Text") + ": "
                + properties.ScreenResolution.ToString() + ", "
                + properties.ScreenResolutionSize.Width + "x" + properties.ScreenResolutionSize.Height
                + " " + _resourceLoader.GetString("Pixels/Text");

            if (properties.DisplaySizeInInches > 0)
            {
                DisplaySize = _resourceLoader.GetString("DisplaySize/Text") + ": "
                    + properties.DisplaySizeInInches + " "
                    + _resourceLoader.GetString("Inches/Text");
            }
            else
            {
                DisplaySize = _resourceLoader.GetString("DisplaySize/Text") + ": "
                    + _resourceLoader.GetString("NotAvailable/Text");
            }

            try
            {
                MemoryUsedInPercentages =
                    (int)Math.Round((double)(100 * properties.ApplicationCurrentMemoryUsageInBytes
                    / properties.ApplicationMemoryUsageLimitInBytes));
                MemoryStatus =
                    DeviceProperties.TransformBytes(properties.ApplicationCurrentMemoryUsageInBytes, DeviceProperties.UnitPrefixes.Mega, 1)
                    + " MB " + _resourceLoader.GetString("CurrentlyInUseOf/Text") + " "
                    + DeviceProperties.TransformBytes(properties.ApplicationMemoryUsageLimitInBytes, DeviceProperties.UnitPrefixes.Mega, 1)
                    + " MB";
            }
            catch (Exception)
            {
                MemoryStatus = _resourceLoader.GetString("NotAvailable/Text");
            }

            ProcessorCoreCount = properties.ProcessorCoreCount;
            DeviceName = _resourceLoader.GetString("DeviceName/Text") + ": " + properties.DeviceName;
            Manufacturer = _resourceLoader.GetString("Manufacturer/Text") + ": " + properties.Manufacturer;
            HardwareVersion = _resourceLoader.GetString("HardwareVersion/Text") + ": " + properties.HardwareVersion;
            FirmwareVersion = _resourceLoader.GetString("FirmwareVersion/Text") + ": " + properties.FirmwareVersion;
            SDCardPresent = properties.HasSDCardPresent ? _resourceLoader.GetString("Yes/Text") : _resourceLoader.GetString("NoCardPresent/Text");
            Theme = _resourceLoader.GetString("Theme/Text") + ": " + ((properties.AppTheme == Windows.UI.Xaml.ApplicationTheme.Dark) ? _resourceLoader.GetString("Dark/Text") : _resourceLoader.GetString("Light/Text"));
            ThemeAccentColor = _resourceLoader.GetString("ThemeAccentColor/Text") + ": " + properties.ThemeAccentColor.ToString();
            VibrationDeviceAvailable = properties.HasVibrationDevice ? _resourceLoader.GetString("Available/Text") : _resourceLoader.GetString("NotAvailable/Text");

            IsDataLoaded = true;
        }

        /// <summary>
        /// Creates the items.
        /// </summary>
        private void CreateItems()
        {
            SensorItems.Add(new BoolItemModel() { BooleanValue = false, HardwareFeatureText = _resourceLoader.GetString("Accelerometer/Text") });
            SensorItems.Add(new BoolItemModel() { BooleanValue = false, HardwareFeatureText = _resourceLoader.GetString("Compass/Text") });
            SensorItems.Add(new BoolItemModel() { BooleanValue = false, HardwareFeatureText = _resourceLoader.GetString("Gyroscope/Text") });
            SensorItems.Add(new BoolItemModel() { BooleanValue = false, HardwareFeatureText = _resourceLoader.GetString("Inclinometer/Text") });
            SensorItems.Add(new BoolItemModel() { BooleanValue = false, HardwareFeatureText = _resourceLoader.GetString("OrientationSensor/Text") });
            SensorItems.Add(new BoolItemModel() { BooleanValue = false, HardwareFeatureText = _resourceLoader.GetString("NFC/Text") });

            CameraItems.Add(new BoolItemModel() { BooleanValue = false, HardwareFeatureText = _resourceLoader.GetString("BackCamera/Text") });
            CameraItems.Add(new BoolItemModel() { BooleanValue = false, HardwareFeatureText = _resourceLoader.GetString("BackCameraFlash/Text") });
            CameraItems.Add(new BoolItemModel() { BooleanValue = false, HardwareFeatureText = _resourceLoader.GetString("BackCameraAutoFocus/Text") });
            CameraItems.Add(new BoolItemModel() { BooleanValue = false, HardwareFeatureText = _resourceLoader.GetString("FrontCamera/Text") });
            CameraItems.Add(new BoolItemModel() { BooleanValue = false, HardwareFeatureText = _resourceLoader.GetString("FrontCameraFlash/Text") });
            
            // Front camera focus information is not reliable and thus not added
            //CameraItems.Add(new BoolItemModel() { BooleanValue = false, HardwareFeatureText = _resourceLoader.GetString("FrontCameraAutoFocus/Text") });

            SensorCoreItems.Add(new BoolItemModel() { BooleanValue = false, HardwareFeatureText = _resourceLoader.GetString("ActivityMonitor/Text") });
            SensorCoreItems.Add(new BoolItemModel() { BooleanValue = false, HardwareFeatureText = _resourceLoader.GetString("PlaceMonitor/Text") });
            SensorCoreItems.Add(new BoolItemModel() { BooleanValue = false, HardwareFeatureText = _resourceLoader.GetString("StepCounter/Text") });
            SensorCoreItems.Add(new BoolItemModel() { BooleanValue = false, HardwareFeatureText = _resourceLoader.GetString("TrackPointMonitor/Text") });

            DeviceTotalMemory = _resourceLoader.GetString("Waiting/Text");
            MemoryStatus = _resourceLoader.GetString("Waiting/Text");
            ScreenResolution = _resourceLoader.GetString("Waiting/Text");
            DeviceName = _resourceLoader.GetString("Waiting/Text");
            SDCardPresent = _resourceLoader.GetString("Waiting/Text");
            Theme = _resourceLoader.GetString("Waiting/Text");
            VibrationDeviceAvailable = _resourceLoader.GetString("Waiting/Text");
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