/*
 * Copyright (c) 2013-2014 Microsoft Mobile. All rights reserved.
 * See the license text file delivered with this project for more information.
 */

namespace PhoneInfo
{
    using System;
    using System.Collections.Generic;
    using System.Diagnostics;
    using System.Linq;
    using System.Threading.Tasks;
    using System.Windows;
    using Windows.Devices.Enumeration;
    using Windows.Devices.Sensors;
    using Windows.Foundation;
    using Windows.Graphics.Display;
    using Windows.Media.Capture;
    using Windows.Media.MediaProperties;
    using Windows.Networking.Proximity;
    using Windows.UI;
    using Windows.UI.Xaml;
    using Windows.UI.Xaml.Media;

    /// <summary>
    /// This class implements methods to resolve harware supported by the
    /// phone and details about the phone software. In addition, the dynamic
    /// traits of the phone are resolved. The resolved values are stored in
    /// the class properties enabling fast queries.
    /// 
    /// Note that you need to make sure that the application has enough
    /// capabilites enabled for the implementation to work properly.
    /// </summary>
    public class DeviceProperties
    {
        // Constants ->

        private const string DebugTag = "DeviceProperties: ";

        /* Focus properties available via MediaCapture.VideoDeviceController
         * may return invalid values for older phones, which original were
         * released with Windows Phone OS version 8.0. Thus, we need to check
         * those phone models explicitly.
         */
        private readonly string[] WP80PhoneModelsWithAutoFocus =
        {
            "RM-820", // Nokia Lumia 920
            "RM-821", // Nokia Lumia 920
            "RM-822", // Nokia Lumia 920
            "RM-824", // Nokia Lumia 820
            "RM-825", // Nokia Lumia 820
            "RM-826", // Nokia Lumia 820
            "RM-846", // Nokia Lumia 620
            "RM-867", // Nokia Lumia 920
            "RM-875", // Nokia Lumia 1020
            "RM-876", // Nokia Lumia 1020
            "RM-877", // Nokia Lumia 1020
            "RM-885", // Nokia Lumia 720
            "RM-887", // Nokia Lumia 720
            "RM-892", // Nokia Lumia 925
            "RM-893", // Nokia Lumia 925
            "RM-910", // Nokia Lumia 925
            "RM-955"  // Nokia Lumia 925
        };

        // Data types ->

        public enum Resolutions
        {
            WVGA, // Wide VGA, 480x800
            qHD, // qHD, 540x960
            HD720, // HD, 720x1280
            WXGA, // Wide Extended Graphics Array (WXGA), 768x1280
            HD1080, // Full HD, 1080x1920
            Unknown
        };

        public enum UnitPrefixes
        {
            Kilo,
            Mega,
            Giga
        };

        // Members and properties ->

        private static DeviceProperties _instance = null;
        private static Object _syncLock = new Object();
        private MediaCapture _mediaCapture = null;
        private int _numberOfAsyncOperationsToComplete;
        private int _numberOfAsyncOperationsCompleted;
#if (DEBUG)
        private DateTime _startOfResolveTime;
#endif

        public EventHandler<bool> IsReadyChanged { get; set; }

        public bool IsReady { get; private set; }

        // Battery and power
        public int RemainingBatteryCharge { get; private set; }
        public bool HasBatteryStatusInfo { get; private set; }
        public bool IsConnectedToExternalPowerSupply { get; private set; }
        public bool PowerSavingModeEnabled { get; private set; }

        // Cameras and flashes
        public bool HasBackCamera { get; private set; }
        public bool HasFrontCamera { get; private set; }
        public bool HasBackCameraFlash { get; private set; }
        public bool HasFrontCameraFlash { get; private set; }
        public bool HasBackCameraAutoFocus { get; private set; }
        public bool HasFrontCameraAutoFocus { get; private set; }
        public List<Size> BackCameraPhotoResolutions { get; private set; }
        public List<Size> FrontCameraPhotoResolutions { get; private set; }
        public List<Size> BackCameraVideoResolutions { get; private set; }
        public List<Size> FrontCameraVideoResolutions { get; private set; }

        // Memory
        public long ApplicationCurrentMemoryUsageInBytes { get; private set; }
        public long ApplicationMemoryUsageLimitInBytes { get; private set; }

        // Screen
        public Resolutions ScreenResolution { get; private set; }
        public Size ScreenResolutionSize { get; private set; }
        public double DisplaySizeInInches { get; private set; } // E.g. 4.5 for Nokia Lumia 1020

        // Sensors
        public bool HasAccelerometerSensor { get; private set; }
        public bool HasCompass { get; private set; }
        public bool HasGyroscopeSensor { get; private set; }
        public bool HasInclinometerSensor { get; private set; }
        public bool HasOrientationSensor { get; private set; }
        public bool HasProximitySensor { get; private set; } // NFC

        // SensorCore
        public bool SensorCoreActivityMonitorApiSupported { get; private set; }
        public bool SensorCorePlaceMonitorApiSupported { get; private set; }
        public bool SensorCoreStepCounterApiSupported { get; private set; }
        public bool SensorCoreTrackPointMonitorApiSupported { get; private set; }

        // Other hardware properties
        public string DeviceName { get; private set; }
        public string Manufacturer { get; private set; }
        public string HardwareVersion { get; private set; }
        public string FirmwareVersion { get; private set; }
        public bool HasSDCardPresent { get; private set; }
        public bool HasVibrationDevice { get; private set; }
        public int ProcessorCoreCount { get; private set; }

        // Software and other dynamic, non-hardware properties
        public ApplicationTheme AppTheme { get; private set; }
        public Color ThemeAccentColor { get; private set; }


        #region Construction, initialisation and refreshing

        /// <summary>
        /// </summary>
        /// <returns>The singleton instance of this class.</returns>
        public static DeviceProperties GetInstance()
        {
            lock (_syncLock)
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
        /// Resolves all the properties.
        /// 
        /// Note that this method is synchronous, but some method calls within
        /// are asynchronous. Thus, this method will be executed while some of
        /// the asynchronous methods may still be running. If you remove or add
        /// asynchronous method calls, make sure to update the value of
        /// _numberOfAsyncOperationsToComplete so that the IsReadyChanged event
        /// is properly fired.
        /// </summary>
        public void Resolve()
        {
#if (DEBUG)
            Debug.WriteLine(DebugTag + "Resolve() ->");
            _startOfResolveTime = DateTime.Now;
#endif
            if (!IsReady)
            {
                _numberOfAsyncOperationsToComplete = 5; // This must match the number of async method calls!
                _numberOfAsyncOperationsCompleted = 0;

                ResolveDeviceInformation(); // ResolveCameraInfoAsync() depends on this to be run first!
                ResolveCameraInfoAsync();
                ResolveMemoryInfo();
                ResolvePowerInfo();
                ResolveProcessorCoreCount();
                ResolveScreenResolutionAsync();
                ResolveSDCardInfoAsync();
                ResolveSensorCoreAvailabilityAsync();
                ResolveSensorInfo();
                ResolveUiThemeAsync();
                ResolveVibrationDeviceInfo();
            }
            else
            {
                // Refreshing dynamic properties
                Debug.WriteLine(DebugTag + "Resolve(): Already resolved once, refreshing dynamic properties...");
                IsReady = false;

                if (IsReadyChanged != null)
                {
                    IsReadyChanged(this, IsReady);
                }

                _numberOfAsyncOperationsToComplete = 2; // This must match the number of async method calls!
                _numberOfAsyncOperationsCompleted = 0;

                ResolveMemoryInfo();
                ResolvePowerInfo();
                ResolveSDCardInfoAsync();
                ResolveUiThemeAsync();
            }

            if (_numberOfAsyncOperationsToComplete == 0)
            {
                /* There was no async method calls, so all the properties
                 * have been resolved. AsyncOperationComplete() will change
                 * IsReady property and notify listeners.
                 */
                AsyncOperationComplete();
            }
        }

        /// <summary>
        /// For convenience. Runs Resolve() asynchronously.
        /// </summary>
        public async void ResolveAsync()
        {
            await Task.Run(() => Resolve());
        }

        private void AsyncOperationComplete()
        {
            lock (_syncLock)
            {
                _numberOfAsyncOperationsCompleted++;

                if (_numberOfAsyncOperationsCompleted >= _numberOfAsyncOperationsToComplete)
                {
                    Debug.WriteLine(DebugTag + "AsyncOperationComplete(): All operations complete!");
                    IsReady = true;
                    NotifyIsReadyChangedAsync();
#if (DEBUG)
                    Debug.WriteLine(DebugTag + "AsyncOperationComplete(): Time elapsed: " + (DateTime.Now - _startOfResolveTime));
#endif
                }
            }
        }

        private async void NotifyIsReadyChangedAsync()
        {
            await Windows.ApplicationModel.Core.CoreApplication.MainView.CoreWindow.Dispatcher.RunAsync(
                Windows.UI.Core.CoreDispatcherPriority.Normal, () =>
                {
                    if (IsReadyChanged != null)
                    {
                        IsReadyChanged(this, IsReady);
                    }
                });
        }

        #endregion // Construction, initialisation and refreshing

        #region Battery and power supply

        private void ResolvePowerInfo()
        {
            HasBatteryStatusInfo = false;

            if (Windows.Phone.Devices.Power.Battery.GetDefault() != null)
            {
                HasBatteryStatusInfo = true;
            }
            else
            {
                Debug.WriteLine(DebugTag
                    + "ResolvePowerInfo(): No battery status info available.");
            }

            if (HasBatteryStatusInfo)
            {
                Windows.Phone.Devices.Power.Battery battery = Windows.Phone.Devices.Power.Battery.GetDefault();
                RemainingBatteryCharge = battery.RemainingChargePercent;
                PowerSavingModeEnabled = Windows.Phone.System.Power.PowerManager.PowerSavingModeEnabled;
                Debug.WriteLine(DebugTag + "ResolvePowerInfo(): " + RemainingBatteryCharge + ", " + PowerSavingModeEnabled);
            }
        }

        #endregion // Battery and power supply

        #region Cameras and flashes

        /// <summary>
        /// Resolves the following properties for both back and front camera:
        /// Flash and (auto) focus support and both photo and video capture
        /// resolutions.
        /// </summary>
        private async void ResolveCameraInfoAsync()
        {
            HasBackCamera = false;
            HasFrontCamera = false;
            HasBackCameraFlash = false;
            HasFrontCameraFlash = false;
            HasBackCameraAutoFocus = false;
            HasFrontCameraAutoFocus = false;
            BackCameraPhotoResolutions = new List<Size>();
            BackCameraVideoResolutions = new List<Size>();
            FrontCameraPhotoResolutions = new List<Size>();
            FrontCameraVideoResolutions = new List<Size>();

            var devices = await Windows.Devices.Enumeration.DeviceInformation.FindAllAsync(
                Windows.Devices.Enumeration.DeviceClass.VideoCapture);
            DeviceInformation backCameraDeviceInformation = devices.FirstOrDefault(x => x.EnclosureLocation != null
                && x.EnclosureLocation.Panel == Windows.Devices.Enumeration.Panel.Back);
            DeviceInformation frontCameraDeviceInformation = devices.FirstOrDefault(x => x.EnclosureLocation != null
                && x.EnclosureLocation.Panel == Windows.Devices.Enumeration.Panel.Front);

            string backCameraId = null;
            string frontCameraId = null;
            var cameraIds = new List<string>();

            if (backCameraDeviceInformation != null)
            {
                backCameraId = backCameraDeviceInformation.Id;
                HasBackCamera = true;
                cameraIds.Add(backCameraId);
            }

            if (frontCameraDeviceInformation != null)
            {
                frontCameraId = frontCameraDeviceInformation.Id;
                HasFrontCamera = true;
                cameraIds.Add(frontCameraId);
            }

            foreach (string cameraId in cameraIds)
            {
                if (_mediaCapture == null)
                {
                    _mediaCapture = new MediaCapture();

                    try
                    {
                        await _mediaCapture.InitializeAsync(
                            new MediaCaptureInitializationSettings
                            {
                                VideoDeviceId = cameraId
                            });
                    }
                    catch (Exception e)
                    {
                        Debug.WriteLine(DebugTag + "ResolveCameraInfoAsync(): Failed to initialize camera: " + e.ToString());
                        _mediaCapture = null;
                        continue;
                    }
                }

                if (_mediaCapture.VideoDeviceController == null)
                {
                    Debug.WriteLine(DebugTag + "ResolveCameraInfoAsync(): No video device controller!");
                    continue;
                }

                bool hasFlash = _mediaCapture.VideoDeviceController.FlashControl.Supported;

                Windows.Media.Devices.MediaDeviceControlCapabilities focusCaps = _mediaCapture.VideoDeviceController.Focus.Capabilities;
                bool focusChangedSupported = _mediaCapture.VideoDeviceController.FocusControl.FocusChangedSupported;
                bool autoAdjustmentEnabled = false;
                _mediaCapture.VideoDeviceController.Focus.TryGetAuto(out autoAdjustmentEnabled);

                Debug.WriteLine(DebugTag + "ResolveCameraInfoAsync(): Focus details of the "
                    + (cameraId.Equals(backCameraId) ? "back camera:" : "front camera:")
                    + "\n\t- Focus.Capabilities.AutoModeSupported: " + focusCaps.AutoModeSupported
                    + "\n\t- Focus.Capabilities.Max: " + focusCaps.Max
                    + "\n\t- Focus.Capabilities.Min: " + focusCaps.Min
                    + "\n\t- Focus.Capabilities.Step: " + focusCaps.Step
                    + "\n\t- Focus.Capabilities.Supported: " + focusCaps.Supported
                    + "\n\t- Focus.TryGetAuto() (automatic adjustment enabled): " + autoAdjustmentEnabled
                    + "\n\t- FocusControl.FocusChangedSupported: " + focusChangedSupported);

                if (cameraId.Equals(backCameraId))
                {
                    HasBackCameraFlash = hasFlash;
                    HasBackCameraAutoFocus = focusChangedSupported;
                    BackCameraPhotoResolutions = ResolveCameraResolutions(_mediaCapture, MediaStreamType.Photo);
                    BackCameraVideoResolutions = ResolveCameraResolutions(_mediaCapture, MediaStreamType.VideoRecord);
                }
                else if (cameraId.Equals(frontCameraId))
                {
                    HasFrontCameraFlash = hasFlash;
                    HasFrontCameraAutoFocus = focusChangedSupported;
                    FrontCameraPhotoResolutions = ResolveCameraResolutions(_mediaCapture, MediaStreamType.Photo);
                    FrontCameraVideoResolutions = ResolveCameraResolutions(_mediaCapture, MediaStreamType.VideoRecord);
                }

                _mediaCapture.Dispose();
                _mediaCapture = null;
            }

            // Auto focus fix for older phones
            foreach (string model in WP80PhoneModelsWithAutoFocus)
            {
                if (DeviceName.Contains(model))
                {
                    Debug.WriteLine(DebugTag + "ResolveCameraInfoAsync(): Auto focus fix applied");
                    HasBackCameraAutoFocus = true;
                    break;
                }
            }

            // Sort resolutions from highest to lowest
            SortSizesFromHighestToLowest(BackCameraPhotoResolutions);
            SortSizesFromHighestToLowest(FrontCameraPhotoResolutions);
            SortSizesFromHighestToLowest(BackCameraVideoResolutions);
            SortSizesFromHighestToLowest(FrontCameraVideoResolutions);

            Debug.WriteLine(DebugTag + "ResolveCameraInfoAsync(): "
                + "\n\t- Back camera ID: " + backCameraId
                + "\n\t- Front camera ID: " + frontCameraId
                + "\n\t- Back camera flash supported: " + HasBackCameraFlash
                + "\n\t- Front camera flash supported: " + HasFrontCameraFlash);

            AsyncOperationComplete();
        }

        /// <summary>
        /// Resolves the available resolutions for the device defined by the given
        /// media capture instance.
        /// </summary>
        /// <param name="mediaCapture">An initialised media capture instance.</param>
        /// <param name="mediaStreamType">The type of the media stream (e.g. video or photo).</param>
        /// <returns>The list of available resolutions or an empty list, if not available.</returns>
        private List<Size> ResolveCameraResolutions(MediaCapture mediaCapture, MediaStreamType mediaStreamType)
        {
            List<Size> resolutions = new List<Size>();
            IReadOnlyList<IMediaEncodingProperties> mediaStreamPropertiesList = null;
            
            try
            {
                mediaStreamPropertiesList = mediaCapture.VideoDeviceController.GetAvailableMediaStreamProperties(mediaStreamType);
            }
            catch (Exception e)
            {
                Debug.WriteLine(DebugTag + "ResolveCameraResolutions(): " + e.ToString());
                return resolutions;
            }

            foreach (var mediaStreamProperties in mediaStreamPropertiesList)
            {
                Size size;
                bool sizeSet = false;

                var streamProperties = mediaStreamProperties as VideoEncodingProperties;
                if (streamProperties != null)
                {
                    VideoEncodingProperties properties = streamProperties;
                    size = new Size(properties.Width, properties.Height);
                    sizeSet = true;
                }
                else
                {
                    var encodingProperties = mediaStreamProperties as ImageEncodingProperties;
                    if (encodingProperties != null)
                    {
                        ImageEncodingProperties properties = encodingProperties;
                        size = new Size(properties.Width, properties.Height);
                        sizeSet = true;
                    }
                }

                if (sizeSet)
                {
                    if (!resolutions.Contains(size))
                    {
                        resolutions.Add(size);
                    }
                }
            }

            return resolutions;
        }

        #endregion // Cameras and flashes

        #region Memory

        private void ResolveMemoryInfo()
        {
            ApplicationCurrentMemoryUsageInBytes = (long)Windows.System.MemoryManager.AppMemoryUsage;
            ApplicationMemoryUsageLimitInBytes = (long)Windows.System.MemoryManager.AppMemoryUsageLimit;
            
            Debug.WriteLine("ResolveMemoryInfo()"
                + "\n - ApplicationCurrentMemoryUsage: " + TransformBytes(ApplicationCurrentMemoryUsageInBytes, UnitPrefixes.Mega, 1) + " MB"
                + "\n - ApplicationMemoryUsageLimit: " + TransformBytes(ApplicationMemoryUsageLimitInBytes, UnitPrefixes.Mega, 1) + " MB"
                + "\n . AppMemoryUsageLevel: " + Windows.System.MemoryManager.AppMemoryUsageLevel);
        }

        #endregion // Memory

        #region Screen

        /// <summary>
        /// Resolves the screen resolution and display size.
        /// </summary>
        private async void ResolveScreenResolutionAsync()
        {
            // Initialise the values
            ScreenResolution = Resolutions.Unknown;
            ScreenResolutionSize = new Size(0, 0);

            double rawPixelsPerViewPixel = 0;
            double rawDpiX = 0;
            double rawDpiY = 0;
            double logicalDpi = 0;
            double screenResolutionX = 0;
            double screenResolutionY = 0;

            await Windows.ApplicationModel.Core.CoreApplication.MainView.CoreWindow.Dispatcher.RunAsync(
                Windows.UI.Core.CoreDispatcherPriority.Normal, () =>
                {
                    DisplayInformation displayInformation =
                        Windows.Graphics.Display.DisplayInformation.GetForCurrentView();
                    rawPixelsPerViewPixel = displayInformation.RawPixelsPerViewPixel;
                    rawDpiX = displayInformation.RawDpiX;
                    rawDpiY = displayInformation.RawDpiY;
                    logicalDpi = displayInformation.LogicalDpi;
                    screenResolutionX = Windows.ApplicationModel.Core.CoreApplication.MainView.CoreWindow.Bounds.Width * rawPixelsPerViewPixel;
                    screenResolutionY = Windows.ApplicationModel.Core.CoreApplication.MainView.CoreWindow.Bounds.Height * rawPixelsPerViewPixel;
                });

            ScreenResolutionSize = new Size(Math.Round(screenResolutionX), Math.Round(screenResolutionY));

            if (screenResolutionY < 960)
            {
                ScreenResolution = Resolutions.WVGA;
            }
            else if (screenResolutionY < 1280)
            {
                ScreenResolution = Resolutions.qHD;
            }
            else if (screenResolutionY < 1920)
            {
                if (screenResolutionX < 768)
                {
                    ScreenResolution = Resolutions.HD720;
                }
                else
                {
                    ScreenResolution = Resolutions.WXGA;
                }
            }
            else if (screenResolutionY > 1280)
            {
                ScreenResolution = Resolutions.HD1080;
            }

            if (rawDpiX > 0 && rawDpiY > 0)
            {
                // Calculate screen diagonal in inches.
                DisplaySizeInInches =
                    Math.Sqrt(Math.Pow(ScreenResolutionSize.Width / rawDpiX, 2) +
                              Math.Pow(ScreenResolutionSize.Height / rawDpiY, 2));
                DisplaySizeInInches = Math.Round(DisplaySizeInInches, 1); // One decimal is enough
            }

            Debug.WriteLine(DebugTag + "ResolveScreenResolutionAsync(): Screen properties:"
                + "\n - Raw pixels per view pixel: " + rawPixelsPerViewPixel
                + "\n - Raw DPI: " + rawDpiX + ", " + rawDpiY
                + "\n . Logical DPI: " + logicalDpi
                + "\n - Resolution: " + ScreenResolution
                + "\n - Resolution in pixels: " + ScreenResolutionSize
                + "\n - Screen size in inches: " + DisplaySizeInInches);

            AsyncOperationComplete();
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
                Debug.WriteLine(e.ToString());
            }

            if (Windows.Devices.Sensors.Inclinometer.GetDefault() != null)
            {
                HasInclinometerSensor = true;
            }

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

        private async void ResolveSensorCoreAvailabilityAsync()
        {
            SensorCoreActivityMonitorApiSupported = await Lumia.Sense.ActivityMonitor.IsSupportedAsync();
            SensorCorePlaceMonitorApiSupported = await Lumia.Sense.PlaceMonitor.IsSupportedAsync();
            SensorCoreStepCounterApiSupported = await Lumia.Sense.StepCounter.IsSupportedAsync();
            SensorCoreTrackPointMonitorApiSupported = await Lumia.Sense.TrackPointMonitor.IsSupportedAsync();

            AsyncOperationComplete();
        }

        #endregion

        #region Other hardware properties

        private void ResolveDeviceInformation()
        {
            Windows.Security.ExchangeActiveSyncProvisioning.EasClientDeviceInformation deviceInformation =
                new Windows.Security.ExchangeActiveSyncProvisioning.EasClientDeviceInformation();
            DeviceName = deviceInformation.SystemProductName;
            Manufacturer = deviceInformation.SystemManufacturer;
            HardwareVersion = deviceInformation.SystemHardwareVersion;
            FirmwareVersion = deviceInformation.SystemFirmwareVersion;
        }

        /// <summary>
        /// Resolves the SD card information. Note that the result false if the
        /// card is not installed even if the device supports one.
        /// 
        /// "You can't simply check the presence of SD card without first
        /// registering in Package.appxmanifest Declarations page a File type
        /// Association. After you have done that, then you can check the
        /// presence of SD card with this code."
        /// 
        // Example snippet from Package.appxmanifest:
        //
        //     <Extension Category="windows.fileTypeAssociation">
        //         <FileTypeAssociation Name="dummy_file_type_association_for_sd_card_detection">
        //             <SupportedFileTypes>
        //                 <FileType>.notarealfiletype</FileType>
        //             </SupportedFileTypes>
        //         </FileTypeAssociation>
        //     </Extension>
        // 
        /// </summary>
        private async void ResolveSDCardInfoAsync()
        {
            try
            {
                var removableDevices = Windows.Storage.KnownFolders.RemovableDevices;
                var sdCards = await removableDevices.GetFoldersAsync();
                HasSDCardPresent = (sdCards.Count > 0);
            }
            catch (UnauthorizedAccessException e)
            {
                Debug.WriteLine(DebugTag + "ResolveSDCardInfoAsync(): " + e.ToString());
            }

            AsyncOperationComplete();
        }

        private void ResolveVibrationDeviceInfo()
        {
            if (Windows.Phone.Devices.Notification.VibrationDevice.GetDefault() != null)
            {
                HasVibrationDevice = true;
            }
        }

        private void ResolveProcessorCoreCount()
        {
            ProcessorCoreCount = System.Environment.ProcessorCount;
            Debug.WriteLine(DebugTag + "ResolveProcessorCoreCount(): " + ProcessorCoreCount);
        }

        #endregion // Other hardware properties

        #region Software, themes and non-hardware dependent

        private async void ResolveUiThemeAsync()
        {
            await Windows.ApplicationModel.Core.CoreApplication.MainView.CoreWindow.Dispatcher.RunAsync(
                Windows.UI.Core.CoreDispatcherPriority.Normal, () =>
                {
                    AppTheme = Application.Current.RequestedTheme;

                    try
                    {
                        SolidColorBrush brush = (SolidColorBrush)Application.Current.Resources["PhoneAccentBrush"];
                        ThemeAccentColor = brush.Color;
                    }
                    catch (System.Runtime.InteropServices.COMException e)
                    {
                        // Thrown if no resources with the given key is found
                        Debug.WriteLine(DebugTag + "ResolveUiThemeAsync(): " + e.ToString());
                    }
                });

            AsyncOperationComplete();
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

        /// <summary>
        /// Dumps the details of every device to the output.
        /// </summary>
        public async static void DumpDeviceInformation()
        {
            DeviceInformationCollection devices = await Windows.Devices.Enumeration.DeviceInformation.FindAllAsync();

            foreach (DeviceInformation device in devices)
            {
                Debug.WriteLine("Found device: " /*+ device.Id + ": "*/ + device.Name + (device.IsEnabled ? " (enabled) " : " (disabled)"));
                IReadOnlyDictionary<string, object> properties = device.Properties;

                foreach (string key in properties.Keys)
                {
                    object value = null;
                    properties.TryGetValue(key, out value);

                    if (value != null)
                    {
                        Debug.WriteLine("\t" + key + ": " + value.ToString());
                    }
                }
            }
        }

        private void SortSizesFromHighestToLowest(List<Size> sizes)
        {
            if (sizes != null && sizes.Count > 1)
            {
                sizes.Sort(delegate(Size x, Size y)
                {
                    if (x.Width * x.Height < y.Width * y.Height)
                    {
                        return 1;
                    }

                    return -1;
                });
            }
        }

        #endregion // Utility methods
    }
}
