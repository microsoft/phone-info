/**
 * Copyright (c) 2013-2014 Microsoft Mobile.
 * See the license text file for the license information.
 */

namespace PhoneInfo
{
    using Microsoft.Devices.Radio;
    using Microsoft.Phone.Info;
    using Microsoft.Phone.Storage;
    using System;
    using System.Linq;
    using System.Threading.Tasks;
    using System.Windows;
    using System.Windows.Media;
    using Windows.Devices.Sensors;
    using Windows.Networking.Proximity;
    using Windows.Phone.Media.Capture;

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
        /*
         * Constants
         */
        private const string DebugTag = "DeviceProperties";
        /*
         * Data types
         */

        public enum Resolutions
        {
            WVGA, // Wide VGA, 480x800
            HD720p, // HD, 720x1280
            WXGA, // Wide Extended Graphics Array (WXGA), 768x1280
            HD1080p, // Full HD, 1080x1920
            Unknown
        };

        public enum UnitPrefixes
        {
            Kilo,
            Mega,
            Giga
        };

        /*
         * Members and properties
         */

        private static DeviceProperties _instance = null;
        private static Object instanceLock = new Object();
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
        public long ApplicationCurrentMemoryUsageInBytes { get; private set; }
        public long ApplicationMemoryUsageLimitInBytes { get; private set; }
        public long ApplicationPeakMemoryUsageInBytes { get; private set; }
        public long DeviceTotalMemoryInBytes { get; private set; }

        // Screen
        public Resolutions ScreenResolution { get; private set; }
        public Size ScreenResolutionSize { get; private set; }
        public double RawDpiX { get; private set; }
        public double RawDpiY { get; private set; }
        public double DisplaySizeInInches { get; private set; } // E.g. 4.5 for Nokia Lumia 1020

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
        public string HardwareVersion { get; private set; }
        public string Operator { get; private set; }
        public string Manufacturer { get; private set; }
        public bool HasSDCardPresent { get; private set; }
        public bool HasVibrationDevice { get; private set; }

        // Software and other dynamic, non-hardware properties
        public string FirmwareVersion { get; private set; }
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
            lock (instanceLock)
            {
                if (_instance == null)
                {
                    _instance = new DeviceProperties();
                }
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
            System.Diagnostics.Debug.WriteLine(DebugTag + ".Init() ->");
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
            System.Diagnostics.Debug.WriteLine(DebugTag + ".Init() <- Duration: " + (DateTime.Now - before));
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
            System.Diagnostics.Debug.WriteLine(DebugTag + ".Refresh() ->");
            DateTime before = DateTime.Now;
#endif
            ResolveBatteryStatusInfo();
            ResolvePowerSource();
            ResolveMemoryInfo();
            ResolveOperator();
            ResolveSDCardInfo();

            ResolveUiTheme();
#if (DEBUG)
            System.Diagnostics.Debug.WriteLine(DebugTag + ".Refresh() <- Duration: " + (DateTime.Now - before));
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
            else
            {
                System.Diagnostics.Debug.WriteLine(DebugTag
                    + ".ResolveBatteryStatusInfo(): No battery status info available.");
            }
        }

        private void ResolvePowerSource()
        {
            System.Diagnostics.Debug.WriteLine(DebugTag
                + ".ResolvePowerSource(): " + DeviceStatus.PowerSource);

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
            catch (Exception e)
            {
                System.Diagnostics.Debug.WriteLine(DebugTag
                    + ".ResolveCameraAndFlashInfo(): Failed to resolve the camera availability: "
                    + e.ToString());
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
            ApplicationCurrentMemoryUsageInBytes = DeviceStatus.ApplicationCurrentMemoryUsage;
            ApplicationMemoryUsageLimitInBytes = DeviceStatus.ApplicationMemoryUsageLimit;
            ApplicationPeakMemoryUsageInBytes = DeviceStatus.ApplicationPeakMemoryUsage;
            DeviceTotalMemoryInBytes = DeviceStatus.DeviceTotalMemory;

            // The following properties also work:
            //ApplicationCurrentMemoryUsageInBytes = (long)Windows.Phone.System.Memory.MemoryManager.ProcessCommittedBytes;
            //ApplicationMemoryUsageLimitInBytes = (long)Windows.Phone.System.Memory.MemoryManager.ProcessCommittedLimit;

            System.Diagnostics.Debug.WriteLine(
                "From DeviceStatus class:"
                + "\n - ApplicationCurrentMemoryUsage: " + TransformBytes(ApplicationCurrentMemoryUsageInBytes, UnitPrefixes.Mega, 1) + " MB"
                + "\n - ApplicationMemoryUsageLimit: " + TransformBytes(ApplicationMemoryUsageLimitInBytes, UnitPrefixes.Mega, 1) + " MB"
                + "\n - ApplicationPeakMemoryUsage: " + TransformBytes(ApplicationPeakMemoryUsageInBytes, UnitPrefixes.Mega, 1) + " MB"
                + "\n - DeviceTotalMemory: " + TransformBytes(DeviceTotalMemoryInBytes, UnitPrefixes.Mega, 1) + " MB"
                );
        }

        #endregion // Memory

        #region Screen

        /// <summary>
        /// Resolves the screen resolution.
        /// </summary>
        private void ResolveScreenResolution()
        {
            // Initialise the values
            ScreenResolution = Resolutions.Unknown;
            ScreenResolutionSize = new Size(0, 0);
            RawDpiX = 0;
            RawDpiY = 0;

            try
            {
                ScreenResolutionSize = (Size)DeviceExtendedProperties.GetValue("PhysicalScreenResolution");

                if (ScreenResolutionSize.Width == 1080)
                {
                    ScreenResolution = Resolutions.HD1080p;
                }

            }
            catch (Exception e)
            {
                System.Diagnostics.Debug.WriteLine(DebugTag
                    + ".ResolveScreenResolution(): Failed to resolve the screen resolution size via DeviceExtendedProperties: "
                    + e.ToString());
            }

            if (ScreenResolution == Resolutions.Unknown)
            {
                /* Since the screen resolution could not be resolved via
                 * DeviceExtendedProperties, let's use the scale factor
                 * instead.
                 */
                Deployment.Current.Dispatcher.BeginInvoke(delegate()
                {
                    switch (Application.Current.Host.Content.ScaleFactor)
                    {
                        case 100:
                            ScreenResolution = Resolutions.WVGA;
                            ScreenResolutionSize = new Size(480, 800);
                            break;
                        case 150:
                            ScreenResolution = Resolutions.HD720p;
                            ScreenResolutionSize = new Size(720, 1280);
                            break;
                        case 160:
                            ScreenResolution = Resolutions.WXGA;
                            ScreenResolutionSize = new Size(768, 1280);
                            break;
                        default:
                            break;
                    }
                });
            }

            try
            {
                RawDpiX = (double)DeviceExtendedProperties.GetValue("RawDpiX");
                RawDpiY = (double)DeviceExtendedProperties.GetValue("RawDpiY");
            }
            catch (Exception e)
            {
                System.Diagnostics.Debug.WriteLine(DebugTag
                    + ".ResolveScreenResolution(): Failed to resolve the screen DPI values via DeviceExtendedProperties: "
                    + e.ToString());
            }

            if (RawDpiX > 0 && RawDpiY > 0)
            {
                // Calculate screen diagonal in inches.
                DisplaySizeInInches =
                    Math.Sqrt(Math.Pow(ScreenResolutionSize.Width / RawDpiX, 2) +
                              Math.Pow(ScreenResolutionSize.Height / RawDpiY, 2));
                DisplaySizeInInches = Math.Round(DisplaySizeInInches, 1); // One decimal is enough
            }

            System.Diagnostics.Debug.WriteLine("Screen properties:"
                + "\n - Resolution: " + ScreenResolution
                + "\n - Resolution in pixels: " + ScreenResolutionSize
                + "\n - Raw DPI: " + RawDpiX + ", " + RawDpiY
                + "\n - Screen size: " + DisplaySizeInInches + " inches"
                ); 
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
            catch (RadioDisabledException e)
            {
                System.Diagnostics.Debug.WriteLine(DebugTag
                    + ".ResolveFMRadioInfo(): Failed to get the FM radio instance: "
                    + e.ToString());
            }
        }

        private void ResolveHardwareVersion()
        {
            HardwareVersion = DeviceStatus.DeviceHardwareVersion;
        }

        private void ResolveOperator()
        {
            try
            {
                Operator = (string)DeviceExtendedProperties.GetValue("OriginalMobileOperatorName");
            }
            catch (Exception e)
            {
                System.Diagnostics.Debug.WriteLine(DebugTag
                    + ".ResolveOperator(): Failed to resolve the operator name via DeviceExtendedProperties: "
                    + e.ToString());

                Operator = null;
            }
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
            Deployment.Current.Dispatcher.BeginInvoke(delegate()
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
            });
        }

        #endregion // Software, themes and non-hardware dependent

        #region Utility methods

        /// <summary>
        /// Transforms the given bytes based on the given desired unit.
        /// </summary>
        /// <param name="bytes">The number of bytes to transform.</param>
        /// <param name="toUnit">The unit into which to transform, e.g. gigabytes.</param>
        /// <param name="numberOfDecimals">The number of decimals desired.</param>
        /// <returns></returns>
        public static double TransformBytes(long bytes, UnitPrefixes toUnit, int numberOfDecimals = 0)
        {
            double retval = 0;
            double denominator = 0;

            switch (toUnit)
            {
                case UnitPrefixes.Kilo:
                    denominator = 1024;
                    break;
                case UnitPrefixes.Mega:
                    denominator = 1024 * 1024;
                    break;
                case UnitPrefixes.Giga:
                    denominator = Math.Pow(1024, 3);
                    break;
                default:
                    break;
            }

            if (denominator != 0)
            {
                retval = bytes / denominator;
            }

            if (numberOfDecimals >= 0)
            {
                retval = Math.Round(retval, numberOfDecimals);
            }

            return retval;
        }

        #endregion // Utility methods
    }
}
