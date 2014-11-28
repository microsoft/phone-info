Phone Info
==========

Phone Info is an example application for retrieving both static and dynamic
properties of a Windows Phone device. The methods implemented in
`DeviceProperties` class and utilised in this app can be used for adapting your
application to wider range of phone models; If a phone does not support a 
certain non-vital feature of your application, you can gracefully adapt by hiding
that feature dynamically. The same applies to optimising your app for different
screen resolutions and display sizes.

![Fixed features view on Nokia Lumia 930 (Windows Phone 8.1 version)](/doc/screenshots_wp8_1/pi_fixed_small.png?raw=true)&nbsp;
![Dynamic features view on Nokia Lumia 930 (Windows Phone 8.1 version)](/doc/screenshots_wp8_1/pi_dynamic_small.png?raw=true)&nbsp;
![Camera features view Nokia Lumia 930 (Windows Phone 8.1 version)](/doc/screenshots_wp8_1/pi_camera_1_small.png?raw=true)&nbsp;
![Sensor features view on Nokia Lumia 930 (Windows Phone 8.1 version)](/doc/screenshots_wp8_1/pi_sensors_2_small.png?raw=true)

*Screenshots from Windows Phone 8.1 version running on Nokia Lumia 930.*

This example application is hosted in GitHub:
https://github.com/Microsoft/phone-info

This project consists of two Phone Info application versions. See the respective
README files for documentation:

* [Phone Info for Windows Phone 8.1](https://github.com/Microsoft/phone-info/blob/master/PhoneInfoWP8_1/README.md)
* [Phone Info for Windows Phone 8](https://github.com/Microsoft/phone-info/blob/master/PhoneInfoWP8/README.md)

For more information on the subject, visit Lumia Developer's Library:

* http://developer.nokia.com/Resources/Library/Lumia/#!optimising-for-large-screen-phones.html
* http://developer.nokia.com/Resources/Library/Lumia/#!how-to-adapt-to-lumia-phone-hardware-features.html


Features
-------------------------------------------------------------------------------

| **Feature** | **Version 2.0 (WP 8.1)** | **Version 1.2 (WP 8.0)** |
| ----------- | ------------------------ | ---------------------- |
| **General information about the phone** | | |
| Device name | X | |
| Manufacturer | X | X |
| Hardware version | X | X |
| Firmware version | X | X |
| Operator | | X |
| **Battery and power** | | |
| Battery status information availability | X | X |
| Power source (is connected to charger) | | X |
| Remaining battery charge | X | |
| Power saving mode enabled | X | |
| **Camera** | | |
| Back camera availability | X | X |
| Front camera availability | X | X |
| Back camera flash availability | X | X |
| Front camera flash availability | X | X |
| Back camera auto focus availability | X | |
| Back camera photo resolutions | X | |
| Front camera photo resolutions | X | |
| Back camera video resolutions | X | |
| Front camera video resolutions | X | |
| **Memory** | | |
| App (current) memory usage | X | X |
| App memory usage limit | X | X |
| App memory peak | | X |
| Device total memory | | X |
| **Screen and display** | | |
| Screen resolution | X | X |
| Raw DPI for width and height | (Possible, but dropped from interface) | X |
| Display size | X | |
| **Sensor availability** | | |
| Accelerometer | X | X |
| Compass | X | X |
| Gyro | X | X |
| Inclinometer | X | X |
| Motion API | | X |
| Orientation | X | X |
| Proximity (NFC) | X | X |
| SensorCore: Activity monitor API | X | |
| SensorCore: Place monitor API | X | |
| SensorCore: Step counter API | X | |
| SensorCore: Track point monitor API | X | |
| **Other harware properties** | | |
| Processor core count | X | |
| SD card present | X | X |
| Vibra availability | X | X |
| FM radio availability | | X |
| **Theme** | | |
| Theme (dark/light) | X | X |
| Theme accent color | X | X |


License
-------------------------------------------------------------------------------

See the license text file delivered with this project. The license file is also
available online at
https://github.com/Microsoft/phone-info/blob/master/Licence.txt


Version history
-------------------------------------------------------------------------------

* Version 2.0: Project ported to support Universal apps for Windows (Phone) 8.1. Support added for many new properties, especially related to camera, battery and SensorCore.
* Version 1.2: Project name changed from "Hardware Info" to "Phone Info". New
  properties added such as screen and display information. Two new Pivot items
  added and the information shown in the views rearranged.
* Version 1.1: Refactored the user interface. Characteristics view added.
* Version 1.0: The initial release.
