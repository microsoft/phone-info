Phone Info
==========

Phone Info is an example application for retrieving both static and dynamic
properties of a Windows Phone device. The methods implemented in
`DeviceProperties` class and utilised in this app can be used for adapting your
application to wider range of phone models; If a phone does not support a 
ertain non-vital feature of your application, you can gracefully adapt by hiding
that feature dynamically. The same applies to optimising your app for different
screen resolutions and display sizes.

![Fixed features view on Nokia Lumia 930 (Windows Phone 8.1 version)](https://raw.github.com/Microsoft/phone-info/master/doc/screenshots_wp8_1/pi_fixed_small.png)&nbsp;
![Dynamic features view on Nokia Lumia 930 (Windows Phone 8.1 version)](https://raw.github.com/Microsoft/phone-info/master/doc/screenshots_wp8_1/pi_dynamic_small.png)&nbsp;
![Camera features view Nokia Lumia 930 (Windows Phone 8.1 version)](https://raw.github.com/Microsoft/phone-info/master/doc/screenshots_wp8_1/pi_camera_1_small.png)&nbsp;
![Sensor features view on Nokia Lumia 930 (Windows Phone 8.1 version)](https://raw.github.com/Microsoft/phone-info/master/doc/screenshots_wp8_1/pi_sensors_2_small.png)

*Screenshots from Windows Phone 8.1 version running on Nokia Lumia 930.*

The user interface of the app consists of four pivot views:

* The **fixed** view provides screen and display properties.
* The **dynamic** view displays app memory consumption, theme details and
  battery status.
* The **camera** view lists the available cameras and camera flashes of the
  phone including the respective capture resolutions for both photo and video.
* The **sensors** view lists the sensor availability of the phone including the
  new SensorCore APIs.

This example application is hosted in GitHub:
https://github.com/Microsoft/phone-info

For more information on the subject, visit Nokia Lumia Developer's Library:

* http://developer.nokia.com/Resources/Library/Lumia/#!optimising-for-large-screen-phones.html
* http://developer.nokia.com/Resources/Library/Lumia/#!how-to-adapt-to-lumia-phone-hardware-features.html

The latest version is compatible with Windows Phone 8.1 devices. Tested on Nokia
Lumia 630, Nokia Lumia 930 and Nokia Lumia 1020. Note that the class responsible
for resolving the device properties,
[DeviceProperties](https://github.com/Microsoft/phone-info/blob/master/PhoneInfoWP8_1/PhoneInfo/DeviceProperties.cs),
is compatible for Windows 8.1 and will work e.g. on Windows 8.1 tablets. The
user interface implementation, however, is Windows Phone 8.1 specific.


1. About implementation
-------------------------------------------------------------------------------

**Important files and classes:**

* `DeviceProperties`: The main helper class which resolves and holds the
  information of all the different device specific properties shown by the
  application. **This is the class to extract from this project and place it
  into your own application.**
* `PivotPage`: Implements the application UI and controls the application logic.

**Required capabilities:**

* Proximity
* Removable Storage
* Webcam
* *Internet (Client & Server)* (Required only by the link on the about page)


2. License
-------------------------------------------------------------------------------

See the license text file delivered with this project. The license file is also
available online at
https://github.com/Microsoft/phone-info/blob/master/Licence.txt


3. Version history
-------------------------------------------------------------------------------

* Version 2.0: Initial release for Windows (Phone) 8.1
