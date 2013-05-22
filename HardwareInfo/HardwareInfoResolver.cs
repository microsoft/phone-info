/**
 * Copyright (c) 2013 Nokia Corporation.
 */

using Microsoft.Phone.Storage;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

using Windows.Devices.Sensors;
using Windows.Networking.Proximity;
using Windows.Phone.Media.Capture;
using Windows.Phone.Devices.Power;

using Microsoft.Devices.Radio;

namespace HardwareInfo
{
    /// <summary>
    /// This class implements methods to resolve harware supported by the
    /// device. Note that you need to make sure that the application has
    /// enough capabilites enabled for the implementation to work properly.
    /// 
    /// For more information about sensors on Windows Phone, see
    /// http://msdn.microsoft.com/en-us/library/windowsphone/develop/hh202968%28v=vs.105%29.aspx
    /// 
    /// 对于众多的硬件的访问，需要添加Cap，如果没有添加，当该硬件不存在，
    /// 或者功能被用户关闭时，会触发 UnauthorizedAccessException 的异常
    /// </summary>
    public class HardwareInfoResolver
    {
        public bool AccelerometerExists { get; set; }
        public bool BackCameraExists { get; set; }
        public bool BackCameraFlashExists { get; set; }
        public bool BatterySensorExists { get; set; }
        public bool CompassExists { get; set; }
        public bool FrontCameraExists { get; set; }
        public bool FrontCameraFlashExists { get; set; }
        public bool GyroscopeExists { get; set; }
        public bool InclinometerExists { get; set; }
        public bool MotionApiAvailable { get; set; }
        public bool OrientationSensorExists { get; set; }
        public bool ProximityExists { get; set; }
        public bool SDCardExists { get; set; }
        public bool VibrationDeviceExists { get; set; }
        public bool FMRadioExists { get; set; }

        public string MemoryCurrentUsed { get; set; }
        public string MemoryMaxAvailable { get; set; }

        /// <summary>
        /// Returns a string describing the screen resolution.
        /// </summary>
        public string ScreenResolution
        {
            get
            {
                switch (App.Current.Host.Content.ScaleFactor)
                {
                    case 100:
                    {
                        // Wide VGA, 480x800
                        return "WVGA";
                    }
                    case 150:
                    {
                        // HD, 720x1280
                        return "HD (720x1280)";
                    }
                    case 160:
                    {
                        // Wide Extended Graphics Array (WXGA), 768x1280
                        return "WXGA";
                    }
                }

                return "Unknown";
            }
        }                        

        /// <summary>
        /// Resolves the supported harware. Note executing this method may take
        /// some time depending on the device. Thus, it is recommended to make
        /// an async call to this method.
        /// </summary>
        public void ResolveInfo()
        {
            GetAccelerometerInfo();
            GetBatterySensorInfo();
            GetCameraInfo();
            GetCameraFlashInfo();
            GetCompassInfo();
            GetGyroscopeInfo();
            GetInclinometerInfo();
            GetMotionApiInfo();
            GetOrientationSensorInfo();
            GetProximityInfo();
            GetSDCardInfo();
            GetVibrationDeviceInfo();
            GetFMRadioInfo();
        }

        /// <summary>
        /// 获取摄像头信息
        /// </summary>
        private void GetCameraInfo()
        {
#if WINDOWS_PHONE_8   // Windows Phone 8
            try
            {
                BackCameraExists = PhotoCaptureDevice.AvailableSensorLocations.Contains<CameraSensorLocation>(CameraSensorLocation.Back);
                FrontCameraExists = PhotoCaptureDevice.AvailableSensorLocations.Contains<CameraSensorLocation>(CameraSensorLocation.Front);
            }
            catch (System.Exception e)
            {
                MessageBox.Show(e.Message);
            }

#else  // Windows Phone 7  
            FrontCameraExists = Microsoft.Devices.Camera.IsCameraTypeSupported(Microsoft.Devices.CameraType.FrontFacing);
            BackCameraExists = Microsoft.Devices.Camera.IsCameraTypeSupported(Microsoft.Devices.CameraType.Primary);
#endif
        }

        /// <summary>
        /// 获取闪光灯信息
        /// </summary>
        private void GetCameraFlashInfo()
        {
            if (BackCameraExists)
            {
                var cam = new Microsoft.Devices.PhotoCamera(Microsoft.Devices.CameraType.Primary);
                BackCameraFlashExists = cam.IsFlashModeSupported(Microsoft.Devices.FlashMode.On);
            }

            if (FrontCameraExists)
            {
                var cam = new Microsoft.Devices.PhotoCamera(Microsoft.Devices.CameraType.FrontFacing);
                FrontCameraFlashExists = cam.IsFlashModeSupported(Microsoft.Devices.FlashMode.On);
            }
        }

        /// <summary>
        /// 获取罗盘信息
        /// </summary>
        private void GetCompassInfo()
        {
#if WINDOWS_PHONE_8  // Windows Phone 8
            if (Compass.GetDefault() != null)
            {
                CompassExists = true;
            }

#else  // Windows Phone 7
            CompassExists = Microsoft.Devices.Sensors.Compass.IsSupported;
#endif
        }

        /// <summary>
        /// NFC 信息
        /// </summary>
        private void GetProximityInfo()
        {
#if WINDOWS_PHONE_8  // Windows Phone 8
            // ID_CAP_NETWORKING ID_CAP_PROXIMITY
            if (ProximityDevice.GetDefault() != null)
            {
                ProximityExists = true;
            }
#else  // Windows Phone 7 
            // Not available
#endif
        }

        /// <summary>
        /// 陀螺仪信息
        /// </summary>
        private void GetGyroscopeInfo()
        {
#if WINDOWS_PHONE_8_test  // Windows Phone 8
            // This GetDefault() method should be has one bug, it should catch
            // the file operation exception.
            try
            {
                if (Gyrometer.GetDefault() != null)
                {
                    GyroscopeExists = true;
                }
            }
            catch (Exception e)
            {
                MessageBox.Show(e.Message);
            }
#else  // Windows Phone 7
            GyroscopeExists = Microsoft.Devices.Sensors.Gyroscope.IsSupported;
#endif
        }

        /// <summary>
        /// 速度传感器信息
        /// </summary>
        private void GetAccelerometerInfo()
        {
#if WINDOWS_PHONE_8  // Windows Phone 8
            if (Windows.Devices.Sensors.Accelerometer.GetDefault() != null)
            {
                AccelerometerExists = true;
            }
#else  // Windows Phone 7
            AccelerometerExists = Microsoft.Devices.Sensors.Accelerometer.IsSupported;
#endif
        }
        
        /// <summary>
        /// 磁倾仪传感器信息
        /// </summary>
        private void GetInclinometerInfo()
        {
#if WINDOWS_PHONE_8  // Windows Phone 8
            if (Windows.Devices.Sensors.Inclinometer.GetDefault() != null)
            {
                InclinometerExists = true;
            }

#else  // Windows Phone 7
            // Not available
#endif
        }

        /// <summary>
        /// Checks if the Motion API is available. Motion API requires both
        /// compass and accelerometer sensors. Gyroscope sensor is used for
        /// more accurate results but is not mandatory.
        /// </summary>
        private void GetMotionApiInfo()
        {
            MotionApiAvailable = Microsoft.Devices.Sensors.Motion.IsSupported;
        }

        /// <summary>
        /// 方向传感器信息
        /// </summary>
        private void GetOrientationSensorInfo()
        {
#if WINDOWS_PHONE_8  // Windows Phone 8
            if (Windows.Devices.Sensors.OrientationSensor.GetDefault() != null)
            {
                OrientationSensorExists = true;
            }
#else  // Windows Phone 7
            OrientationSensorExists = Microsoft.Devices.Sensors.Motion.IsSupported;
#endif
        }
        
        /// <summary>
        /// 震动设备信息
        /// </summary>
        private void GetVibrationDeviceInfo()
        {
#if WINDOWS_PHONE_8  // Windows Phone 8
            if (Windows.Phone.Devices.Notification.VibrationDevice.GetDefault() != null)
            {
                VibrationDeviceExists = true;
            }
#else  // Windows Phone 7
            if (Microsoft.Devices.VibrateController.Default != null)
            {
                VibrationDeviceExists = true;
            }
#endif
        }

        /// <summary>
        /// 电量信息传感器信息
        /// </summary>
        private void GetBatterySensorInfo()
        {
#if WINDOWS_PHONE_8  // Windows Phone 8
            if (Windows.Phone.Devices.Power.Battery.GetDefault() != null)
            {
                BatterySensorExists = true;
            }
#else  // Windows Phone 7
            // Not available
#endif
        }

        /// <summary>
        /// Resolves used and total memory.
        /// </summary>
        private void GetMemoryInfo()
        {
#if WINDOWS_PHONE_8 // Windows Phone 8
            MemoryCurrentUsed = Windows.Phone.System.Memory.MemoryManager.ProcessCommittedBytes.ToString();
            MemoryMaxAvailable = Windows.Phone.System.Memory.MemoryManager.ProcessCommittedLimit.ToString();
#else  // Windows Phone 7
            // Not available
#endif
        }

        /// <summary>
        /// Resolves the SD card information. Note that the result false if the
        /// card is not installed even if the device supports one.
        /// </summary>
        private async void GetSDCardInfo()
        {
#if WINDOWS_PHONE_8 // Windows Phone 8
            var devices = await ExternalStorage.GetExternalStorageDevicesAsync();
            SDCardExists = (devices != null && devices.Count() > 0);
#else // Windows Phone 7
            // Not available
            // Windows Phone 7 不支持SD卡
#endif
        }

        /// <summary>
        /// Attempt to access an instance of the FMRadio API
        /// If an exception is thrown then the API is not available (early WP8 builds)
        /// or the device does not have FM radio hardware
        /// </summary>
        private void GetFMRadioInfo()
        {
            try
            {
                FMRadioExists = true;
                var radio = FMRadio.Instance;
            }
            catch (RadioDisabledException)
            {
                FMRadioExists = false;
            }
        }

    }
}
