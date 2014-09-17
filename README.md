Phone Info
==========

Phone Info is an example application for retrieving both static and dynamic
properties of a Windows Phone device. The methods implemented in
`DeviceProperties` class and utilised in this app can be used for adapting your
application to wider range of phone models; If a phone does not support a 
certain non-vital feature of your application, you can gracefully adapt by hiding
that feature dynamically. The same applies to optimising your app for different
screen resolutions and display sizes.

![Fixed features view on Nokia Lumia 930 (Windows Phone 8.1 version)](https://raw.github.com/nokia-developer/phone-info/master/doc/screenshots_wp8_1/pi_fixed_small.png)&nbsp;
![Dynamic features view on Nokia Lumia 930 (Windows Phone 8.1 version)](https://raw.github.com/nokia-developer/phone-info/master/doc/screenshots_wp8_1/pi_dynamic_small.png)&nbsp;
![Camera features view Nokia Lumia 930 (Windows Phone 8.1 version)](https://raw.github.com/nokia-developer/phone-info/master/doc/screenshots_wp8_1/pi_camera_1_small.png)&nbsp;
![Sensor features view on Nokia Lumia 930 (Windows Phone 8.1 version)](https://raw.github.com/nokia-developer/phone-info/master/doc/screenshots_wp8_1/pi_sensors_2_small.png)

*Screenshots from Windows Phone 8.1 version running on Nokia Lumia 930.*

This example application is hosted in GitHub:
https://github.com/nokia-developer/phone-info

This project consists of two Phone Info application versions. See the respective
README files for documentation:

* [Phone Info for Windows Phone 8.1](https://github.com/nokia-developer/phone-info/blob/master/PhoneInfoWP8_1/README.md)
* [Phone Info for Windows Phone 8](https://github.com/nokia-developer/phone-info/blob/master/PhoneInfoWP8/README.md)

For more information on the subject, visit Nokia Lumia Developer's Library:

* http://developer.nokia.com/Resources/Library/Lumia/#!optimising-for-large-screen-phones.html
* http://developer.nokia.com/Resources/Library/Lumia/#!how-to-adapt-to-lumia-phone-hardware-features.html


License
-------------------------------------------------------------------------------

See the license text file delivered with this project. The license file is also
available online at
https://github.com/nokia-developer/phone-info/blob/master/Licence.txt


Version history
-------------------------------------------------------------------------------

* Version 2.0: Project ported to support Windows (Phone) 8.1.
* Version 1.2: Project name changed from "Hardware Info" to "Phone Info". New
  properties added such as screen and display information. Two new Pivot items
  added and the information shown in the views rearranged.
* Version 1.1: Refactored the user interface. Characteristics view added.
* Version 1.0: The initial release.
