Phone Info
==========

Phone Info is a Nokia example for retrieving both static and dynamic properties
of a Windows Phone device. The methods implemented in `DeviceProperties` class
and utilised in this app can be used for adapting your application to wider
range of phone models; If a phone does not support a certain non-vital feature
of your application, you can gracefully adapt by hiding that feature
dynamically. The same applies to optimising your app for different screen
resolutions and display sizes.

![General view on Nokia Lumia 1520](https://raw.github.com/Microsoft/phone-info/master/doc/screenshots_wp8/general_l1520_small.png)&nbsp;
![Sensors on Nokia Lumia 1520](https://raw.github.com/Microsoft/phone-info/master/doc/screenshots_wp8/sensors_l1520_small.png)&nbsp;
![Cameras and flashes on Nokia Lumia 1520](https://raw.github.com/Microsoft/phone-info/master/doc/screenshots_wp8/camera_l1520_small.png)&nbsp;
![Others view on Nokia Lumia 520](https://raw.github.com/Microsoft/phone-info/master/doc/screenshots_wp8/others_1_l520_small.png)

*Three first screenshots are from Nokia Lumia 1520, the last one (Others view) is from Nokia Lumia 520.*

The user interface of the app consists of four pivot views. The General view
provides screen and display properties, memory and battery status. The Sensors
view lists the sensor availability of the phone. Similarly, the Cameras view
lists the available cameras and camera flashes of the phone. The last, Others
view displays information about the phone itself (firmware and hardware
versions). In addition, there is information about operator, phone theme and
availability of FM radio, SD card etc.

This example application is hosted in GitHub:
https://github.com/Microsoft/phone-info

For more information on the subject, visit Nokia Lumia Developer's Library:

* http://developer.nokia.com/Resources/Library/Lumia/#!optimising-for-large-screen-phones.html
* http://developer.nokia.com/Resources/Library/Lumia/#!how-to-adapt-to-lumia-phone-hardware-features.html

The latest version is compatible with Windows Phone 8 devices. Tested on Nokia
Lumia 520, Nokia Lumia 1020 and Nokia Lumia 1520.


1. About implementation
-------------------------------------------------------------------------------

**Important files and classes:**

* `ViewModels/ItemModel`: A model of a single item in the Availability view
* `ViewModels/MainViewModel`: The model which contains and allows to manage all
  the items in the Availability view
* `DeviceProperties`: The main helper class which resolves and holds the
  information of all the different phone specific properties shown by the
  application. **This is the class to extract from this project and place it
  into your own application.**
* `MainPage`: Implements the application UI and controls the application logic

**Required capabilities:**

* `ID_CAP_ISV_CAMERA`
* `ID_CAP_MEDIALIB_AUDIO`
* `ID_CAP_PROXIMITY`
* `ID_CAP_SENSORS`


2. License
-------------------------------------------------------------------------------

See the license text file delivered with this project. The license file is also
available online at
https://github.com/Microsoft/phone-info/blob/master/Licence.txt


3. Version history
-------------------------------------------------------------------------------

* Version 1.2: Project name changed from "Hardware Info" to "Phone Info". New
  properties added such as screen and display information. Two new Pivot items
  added and the information shown in the views rearranged.
* Version 1.1: Refactored the user interface. Characteristics view added.
* Version 1.0: The initial release.
