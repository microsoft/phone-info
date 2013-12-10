/**
 * Copyright (c) 2013 Nokia Corporation.
 */

namespace HardwareInfo
{
    using Microsoft.Devices.Radio;
    using Microsoft.Phone.Storage;

    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;
    using System.Windows;
    using System.Windows.Threading;

    using Windows.Devices.Sensors;
    using Windows.Networking.Proximity;
    using Windows.Phone.Media.Capture;
    using Windows.Phone.Devices.Power;

    /// <summary>
    /// This class implements methods to resolve harware supported by the
    /// device. Note that you need to make sure that the application has
    /// enough capabilites enabled for the implementation to work properly.
    /// 
    /// For more information about sensors on Windows Phone, see
    /// http://msdn.microsoft.com/en-us/library/windowsphone/develop/hh202968%28v=vs.105%29.aspx
    /// </summary>
    public class DeviceProperties
    {
        private static DeviceProperties _instance = null;

        public bool Initialized { get; private set; }

        // Cameras and flashes
        public bool HasBackCamera { get; private set; }
        public bool HasBackCameraFlash { get; private set; }
        public bool HasFrontCamera { get; private set; }
        public bool HasFrontCameraFlash { get; private set; }

        // Memory
        public string MemoryCurrentUsed { get; private set; }
        public string MemoryMaxAvailable { get; private set; }                      

        // Screen
        public string ScreenResolution { get; private set; }

        // Sensors
        public bool HasAccelerometerSensor { get; private set; }
        public bool HasCompass { get; private set; }
        public bool HasGyroscopeSensor { get; private set; }
        public bool HasInclinometerSensor { get; private set; }
        public bool MotionApiAvailable { get; private set; }
        public bool HasOrientationSensor { get; private set; }
        public bool HasProximitySensor { get; private set; } // NFC

        // Other hardware properties
        public bool HasBatteryStatusInfo { get; private set; }
        public bool HasFMRadio { get; private set; }
        public bool HasSDCardPresent { get; private set; }
        public bool HasVibrationDevice { get; private set; }

        // TODO: Add info from: http://msdn.microsoft.com/EN-US/library/windowsphone/develop/microsoft.phone.info.deviceextendedproperties%28v=vs.105%29.aspx
        // TODO: Add dynamic traits like theme etc.

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
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
        /// 
        /// </summary>
        public void Init()
        {
            if (Initialized)
            {
                System.Diagnostics.Debug.WriteLine("DeviceProperties instance already initialised!");
                return;
            }

#if (DEBUG)
            DateTime before = DateTime.Now;
#endif
            ResolveCameraAndFlashInfo();
            ResolveMemoryInfo();
            ResolveScreenResolution();
            ResolveSensorInfo();
            ResolveBatteryStatusInfo();
            ResolveFMRadioInfo();
            ResolveSDCardInfo();
            ResolveVibrationDeviceInfo();
#if (DEBUG)
            System.Diagnostics.Debug.WriteLine(DateTime.Now - before);
#endif
            Initialized = true;
        }

        /// <summary>
        /// 
        /// </summary>
        public async void InitAsync()
        {
            await Task.Run(() => Init());
        }


        #region Cameras and flashes

        private void ResolveCameraAndFlashInfo()
        {
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
        }

        #endregion // Memory

        #region Screen

        /// <summary>
        /// Resolves the screen resolution.
        /// </summary>
        private void ResolveScreenResolution()
        {
            Deployment.Current.Dispatcher.BeginInvoke(() =>
            {
                switch (App.Current.Host.Content.ScaleFactor)
                {
                    case 100:
                        {
                            // Wide VGA, 480x800
                            ScreenResolution = "WVGA, 480x800";
                            break;
                        }
                    case 150:
                        {
                            // HD, 720x1280
                            ScreenResolution = "HD, 720x1280";
                            break;
                        }
                    case 160:
                        {
                            // Wide Extended Graphics Array (WXGA), 768x1280
                            ScreenResolution = "WXGA, 768x1280";
                            break;
                        }
                    default:
                        {
                            ScreenResolution = "Unknown";
                            break;
                        }
                }
            });
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

        private void ResolveBatteryStatusInfo()
        {
            if (Windows.Phone.Devices.Power.Battery.GetDefault() != null)
            {
                HasBatteryStatusInfo = true;
            }
        }

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
    }
}
