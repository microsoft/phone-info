Phone Info
==========

Phone Info is a Nokia example for retrieving hardware information of a Windows
Phone device. The methods demonstrated by this app can be used for adapting your
application to wider range of phone models; If a phone does not support a
certain non-vital feature of your application, you can gracefully adapt by
hiding that feature dynamically. 

![General view on Nokia Lumia 1520](https://raw.github.com/nokia-developer/phone-info/master/doc/screenshots/general_l1520_small.png)&nbsp;
![Cams and sensors view on Nokia Lumia 1520](https://raw.github.com/nokia-developer/phone-info/master/doc/screenshots/cams_and_sensors_1_l1520_small.png)&nbsp;
![Others view on Nokia Lumia 520](https://raw.github.com/nokia-developer/phone-info/master/doc/screenshots/others_1_l520_small.png)

*Two first screenshots are from Nokia Lumia 1520, the third one is from Nokia Lumia 520.*


The app consists of three pivot views. The General view provides screen and
display properties, memory and battery status. The Cameras and sensors view
lists the camera, flash and sensor availability of the phone. The last, Others
view displays information about the phone itself (firmware and hardware
versions). In addition, there is information about operator, phone theme and
availability of FM radio, SD card etc.

This example application is hosted in GitHub:
https://github.com/nokia-developer/phone-info

For more information on the subject, visit Nokia Lumia Developer's Library:

* http://developer.nokia.com/Resources/Library/Lumia/#!optimising-for-large-screen-phones.html
* https://developer.nokia.com/Resources/Library/Lumia/#!how-to-adapt-to-lumia-phone-hardware-features.html

The latest version is compatible with Windows Phone 8 devices. Tested on Nokia
Lumia 520, Nokia Lumia 1020 and Nokia Lumia 1520.


1. About implementation
-------------------------------------------------------------------------------

**Important files and classes:**

* `ViewModels/ItemModel`: A model of a single item in the Availability view
* `ViewModels/MainViewModel`: The model which contains and allows to manage all
  the items in the Availability view
* `DeviceProperties`: The class which resolves and holds the information of
  all the different phone specific properties shown by the application
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
https://github.com/nokia-developer/phone-info/blob/master/Licence.txt


3. Version history
-------------------------------------------------------------------------------

* Version 1.2: Project name changed from "Hardware Info" to "Phone Info". New
  properties added such as screen and display information. One new Pivot item
  added and the information shown in the views rearranged.
* Version 1.1: Refactored the user interface. Characteristics view added.
* Version 1.0: The initial release.
