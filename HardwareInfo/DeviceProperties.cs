/**
 * Copyright (c) 2013 Nokia Corporation.
 * See the license text file for the license information.
 */

namespace HardwareInfo
{
    using Microsoft.Devices.Radio;
    using Microsoft.Phone.Info;
    using Microsoft.Phone.Storage;
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;
    using System.Windows;
    using System.Windows.Media;
    using System.Windows.Threading;
    using Windows.Devices.Sensors;
    using Windows.Networking.Proximity;
    using Windows.Phone.Media.Capture;
    using Windows.Phone.Devices.Power;

    /// <summary>
    /// This class implements methods to resolve harware supported by the
    /// phone and details about the phone software. In addition, the dynamic
    /// traits of the phone are resolved. The resolved values are stored in
    /// the class properties enabling fast queries.
    /// 
    /// Note that you need to make sure that the application has enough
    /// capabilites enabled for the implementation to work properly.
    /// 
    /// For more information, see
    ///  - Sensors: http://msdn.microsoft.com/en-us/library/windowsphone/develop/hh202968%28v=vs.105%29.aspx
    ///  - Device status properties: http://msdn.microsoft.com/en-us/library/windowsphone/develop/microsoft.phone.info.devicestatus_properties%28v=vs.105%29.aspx
    ///  - Device extended properties: http://msdn.microsoft.com/EN-US/library/windowsphone/develop/microsoft.phone.info.deviceextendedproperties%28v=vs.105%29.aspx
    ///
    /// </summary>
    public class DeviceProperties
    {
        private static DeviceProperties _instance = null;
        private SolidColorBrush _themeBackgroundBrush = null;

        public bool Initialized { get; private set; }

        // Battery and power
        public bool HasBatteryStatusInfo { get; private set; }
        public bool IsConnectedToExternalPowerSupply { get; private set; }

        // Cameras and flashes
        public bool HasBackCamera { get; private set; }
        public bool HasBackCameraFlash { get; private set; }
        public bool HasFrontCamera { get; private set; }
        public bool HasFrontCameraFlash { get; private set; }

        // Memory
        public String MemoryCurrentUsed { get; private set; }
        public String MemoryMaxAvailable { get; private set; }
        public long DeviceTotalMemoryInBytes { get; private set; }

        // Screen
        public Size ScreenResolution { get; private set; }
        public Double RawDpiX { get; private set; }
        public Double RawDpiY { get; private set; }
        public Double ScreenSizeInInches { get; private set; } // E.g. 4.5 for Nokia Lumia 1020

        // Sensors
        public bool HasAccelerometerSensor { get; private set; }
        public bool HasCompass { get; private set; }
        public bool HasGyroscopeSensor { get; private set; }
        public bool HasInclinometerSensor { get; private set; }
        public bool MotionApiAvailable { get; private set; }
        public bool HasOrientationSensor { get; private set; }
        public bool HasProximitySensor { get; private set; } // NFC

        // Other hardware properties
        public bool HasFMRadio { get; private set; }
        public String HardwareVersion { get; private set; }
        public String Operator { get; private set; }
        public String Manufacturer { get; private set; }
        public bool HasSDCardPresent { get; private set; }
        public bool HasVibrationDevice { get; private set; }

        // Software and other dynamic, non-hardware properties
        public String FirmwareVersion { get; private set; }
        public bool HasDarkUiThemeInUse { get; private set; }
        public Color ThemeAccentColor { get; private set; }

        // Utility

        public SolidColorBrush ThemeBackgroundBrush
        {
            get
            {
                if (_themeBackgroundBrush == null)
                {
                    Color color = HasDarkUiThemeInUse ? Colors.Black : Colors.White;
                    _themeBackgroundBrush = new SolidColorBrush(color);
                }

                return _themeBackgroundBrush;
            }
        }

        #region Construction, initialisation and refreshing

        /// <summary>
        /// </summary>
        /// <returns>The singleton instance of this class.</returns>
        public static DeviceProperties GetInstance()
        {
            if (_instance == null)
            {
                _instance = new DeviceProperties();
            }

            return _instance;
        }

        /// <summary>
        /// Constructor.
        /// </summary>
        private DeviceProperties()
        {
        }

        /// <summary>
        /// Resolves all the properties. Note that this method is synchronous
        /// and may take from 0.5 up to several seconds to execute depending on
        /// the device.
        /// </summary>
        public void Init()
        {
#if (DEBUG)
            System.Diagnostics.Debug.WriteLine("DeviceProperties.Init() ->");
            DateTime before = DateTime.Now;
#endif
            if (Initialized)
            {
                System.Diagnostics.Debug.WriteLine("DeviceProperties instance already initialised!");
                return;
            }

            // Hardware
            ResolveBatteryStatusInfo();
            ResolvePowerSource();
            ResolveCameraAndFlashInfo();
            ResolveMemoryInfo();
            ResolveScreenResolution();
            ResolveSensorInfo();
            ResolveFMRadioInfo();
            ResolveHardwareVersion();
            ResolveManufacturer();
            ResolveOperator();
            ResolveSDCardInfo();
            ResolveVibrationDeviceInfo();

            // Software
            ResolveFirmwareVersion();
            ResolveUiTheme();
#if (DEBUG)
            System.Diagnostics.Debug.WriteLine("DeviceProperties.Init() <- Duration: " + (DateTime.Now - before));
#endif
            Initialized = true;
        }

        /// <summary>
        /// For convenience. Runs Init() asynchronously.
        /// </summary>
        public async void InitAsync()
        {
            await Task.Run(() => Init());
        }

        /// <summary>
        /// Refreshes the properties which can change.
        /// </summary>
        public void Refresh()
        {
            Initialized = false;
#if (DEBUG)
            System.Diagnostics.Debug.WriteLine("DeviceProperties.Refresh() ->");
            DateTime before = DateTime.Now;
#endif
            ResolveBatteryStatusInfo();
            ResolvePowerSource();
            ResolveMemoryInfo();
            ResolveOperator();
            ResolveSDCardInfo();

            ResolveUiTheme();
#if (DEBUG)
            System.Diagnostics.Debug.WriteLine("DeviceProperties.Refresh() <- Duration: " + (DateTime.Now - before));
#endif
            Initialized = true;
        }

        /// <summary>
        /// For convenience. Runs Refresh() asynchronously.
        /// </summary>
        public async void RefreshAsync()
        {
            await Task.Run(() => Refresh());
        }

        #endregion // Construction, initialisation and refreshing

        #region Battery and power supply

        private void ResolveBatteryStatusInfo()
        {
            HasBatteryStatusInfo = false;

            if (Windows.Phone.Devices.Power.Battery.GetDefault() != null)
            {
                HasBatteryStatusInfo = true;
            }
        }

        private void ResolvePowerSource()
        {
            /* PowerSource.Battery -> false
             * PowerSource.External -> true (connected to external power supply)
             */
            IsConnectedToExternalPowerSupply = (DeviceStatus.PowerSource == PowerSource.External);
        }

        #endregion // Battery and power supply

        #region Cameras and flashes

        private void ResolveCameraAndFlashInfo()
        {
            HasBackCamera = false;
            HasFrontCamera = false;
            HasBackCameraFlash = false;
            HasFrontCameraFlash = false;

            try
            {
                HasBackCamera = PhotoCaptureDevice.AvailableSensorLocations.Contains<CameraSensorLocation>(CameraSensorLocation.Back);
                HasFrontCamera = PhotoCaptureDevice.AvailableSensorLocations.Contains<CameraSensorLocation>(CameraSensorLocation.Front);
            }
            catch (System.Exception e)
            {
                MessageBox.Show(e.Message);
            }

            if (HasBackCamera)
            {
                var cam = new Microsoft.Devices.PhotoCamera(Microsoft.Devices.CameraType.Primary);
                HasBackCameraFlash = cam.IsFlashModeSupported(Microsoft.Devices.FlashMode.On);
            }

            if (HasFrontCamera)
            {
                var cam = new Microsoft.Devices.PhotoCamera(Microsoft.Devices.CameraType.FrontFacing);
                HasFrontCameraFlash = cam.IsFlashModeSupported(Microsoft.Devices.FlashMode.On);
            }
        }

        #endregion // Cameras and flashes

        #region Memory

        private void ResolveMemoryInfo()
        {
            MemoryCurrentUsed = Windows.Phone.System.Memory.MemoryManager.ProcessCommittedBytes.ToString();
            MemoryMaxAvailable = Windows.Phone.System.Memory.MemoryManager.ProcessCommittedLimit.ToString();

            /* DeviceStatus class also provides information about memory, see
             * http://msdn.microsoft.com/en-us/library/windowsphone/develop/microsoft.phone.info.devicestatus_properties(v=vs.105).aspx
             * 
             * The properties are:
             *   - ApplicationCurrentMemoryUsage
             *   - ApplicationMemoryUsageLimit
             *   - ApplicationPeakMemoryUsage
             *   - DeviceTotalMemory
             */

            DeviceTotalMemoryInBytes = DeviceStatus.DeviceTotalMemory;

            System.Diagnostics.Debug.WriteLine(
                "From DeviceStatus class:"
                + "\n - ApplicationCurrentMemoryUsage: " + DeviceStatus.ApplicationCurrentMemoryUsage
                + "\n - ApplicationMemoryUsageLimit: " + DeviceStatus.ApplicationMemoryUsageLimit
                + "\n - ApplicationPeakMemoryUsage: " + DeviceStatus.ApplicationPeakMemoryUsage
                + "\n - DeviceTotalMemory: " + DeviceTotalMemoryInBytes
                );
        }

        #endregion // Memory

        #region Screen

        /// <summary>
        /// Resolves the screen resolution.
        /// </summary>
        private void ResolveScreenResolution()
        {
            // ScaleFactor can be used to find out the the screen resolution...
            /*
            Deployment.Current.Dispatcher.BeginInvoke(() =>
            {
                switch (App.Current.Host.Content.ScaleFactor)
                {
                    case 100:
                        // Wide VGA, 480x800
                        ScreenResolution = "WVGA, 480x800";
                        break;
                    case 150:
                        // HD, 720x1280
                        ScreenResolution = "HD, 720x1280";
                        break;
                    case 160:
                        // Wide Extended Graphics Array (WXGA), 768x1280
                        ScreenResolution = "WXGA, 768x1280";
                        break;
                    default:
                        ScreenResolution = "Unknown";
                        break;
                }
            });
            */

            // ...as well as DeviceExtendedProperties:
            ScreenResolution = (Size)DeviceExtendedProperties.GetValue("PhysicalScreenResolution");

            RawDpiX = (Double)DeviceExtendedProperties.GetValue("RawDpiX");
            RawDpiY = (Double)DeviceExtendedProperties.GetValue("RawDpiY");

            // TODO: Calc screen size in inches

        }

        #endregion // Screen

        #region Sensors

        private void ResolveSensorInfo()
        {
            if (Windows.Devices.Sensors.Accelerometer.GetDefault() != null)
            {
                HasAccelerometerSensor = true;
            }

            if (Compass.GetDefault() != null)
            {
                HasCompass = true;
            }

            try
            {
                if (Gyrometer.GetDefault() != null)
                {
                    HasGyroscopeSensor = true;
                }
            }
            catch (Exception e)
            {
                /* Older phone software had a bug causing the
                 * Gyrometer.GetDefault() to throw a file operation
                 * exception.
                 */
                System.Diagnostics.Debug.WriteLine(e.ToString());
                HasGyroscopeSensor = Microsoft.Devices.Sensors.Gyroscope.IsSupported;
            }

            if (Windows.Devices.Sensors.Inclinometer.GetDefault() != null)
            {
                HasInclinometerSensor = true;
            }

            /* Motion API requires both magnetometer (compass) and
             * accelerometer sensors. Gyroscope sensor is used for more
             * accurate results but is not mandatory.
             */
            MotionApiAvailable = Microsoft.Devices.Sensors.Motion.IsSupported;

            if (Windows.Devices.Sensors.OrientationSensor.GetDefault() != null)
            {
                HasOrientationSensor = true;
            }

            // ProximityDevice is NFC
            if (ProximityDevice.GetDefault() != null)
            {
                HasProximitySensor = true;
            }
        }

        #endregion

        #region Others

        private void ResolveFMRadioInfo()
        {
            try
            {
                var radio = FMRadio.Instance;
                HasFMRadio = true;
            }
            catch (RadioDisabledException)
            {
                // No radio
            }
        }

        private void ResolveHardwareVersion()
        {
            HardwareVersion = DeviceStatus.DeviceHardwareVersion;
        }

        private void ResolveOperator()
        {
            Operator = (String)DeviceExtendedProperties.GetValue("OriginalMobileOperatorName");
        }

        private void ResolveManufacturer()
        {
            Manufacturer = DeviceStatus.DeviceManufacturer;
        }

        /// <summary>
        /// Resolves the SD card information. Note that the result false if the
        /// card is not installed even if the device supports one.
        /// </summary>
        private async void ResolveSDCardInfo()
        {
            var devices = await ExternalStorage.GetExternalStorageDevicesAsync();
            HasSDCardPresent = (devices != null && devices.Count() > 0);
        }

        private void ResolveVibrationDeviceInfo()
        {
            if (Windows.Phone.Devices.Notification.VibrationDevice.GetDefault() != null)
            {
                HasVibrationDevice = true;
            }
        }

        #endregion // Others

        #region Software, themes and non-hardware dependent

        private void ResolveFirmwareVersion()
        {
            FirmwareVersion = DeviceStatus.DeviceFirmwareVersion;
        }

        private void ResolveUiTheme()
        {
            Visibility darkBackgroundVisibility =
                (Visibility)Application.Current.Resources["PhoneDarkThemeVisibility"];
            HasDarkUiThemeInUse = (darkBackgroundVisibility == Visibility.Visible);

            if (_themeBackgroundBrush != null)
            {
                // In case the theme color has been changed, the brush needs to
                // be recreated
                _themeBackgroundBrush = null;
            }

            ThemeAccentColor = (Color)Application.Current.Resources["PhoneAccentColor"];
        }

        #endregion // Software, themes and non-hardware dependent
    }
}
