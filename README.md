Hardware Info
=============

Hardware Info is a Nokia example for retrieving hardware information of a
Windows Phone device. The methods demonstrated by this app can be used for
adapting your application to wider range of phone models; If a phone does not
support a certain non-vital feature of your application, you can gracefully
adapt by hiding that feature dynamically. 

The app consists of two pivot views. The Availability view lists the phone's
capabilities, such as sensors, based on the values retrieved from the
corresponding APIs. The Characteristics view was added to version 1.1 and it
provides the screen resolution of the phone and the amount of memory used and
available for the application itself.

This example application is hosted in GitHub:
https://github.com/nokia-developer/hardware-info

For more information on the subject, visit Nokia Lumia Developer's Library:
https://www.developer.nokia.com/Resources/Library/Lumia/#!how-to-adapt-to-lumia-phone-hardware-features.html


1. Prerequisites
-------------------------------------------------------------------------------

- XAML basics
- C# basics


2. Project structure and implementation
-------------------------------------------------------------------------------

2.1 Important files and classes
-------------------------------

- ViewModels/ItemModel: A model of a single item in the Availability view
- ViewModels/MainViewModel: The model which contains and allows to manage all
  the items in the Availability view

- HardwareInfoResolver: The class which resolves and holds the information of
  all the different phone specific properties shown by the application
- MainPage: Implements the application UI and controls the application logic


2.2 Important APIs used
-----------------------

The most relevant APIs used by this application are found in the following
namespaces:
- Microsoft.Devices.Radio
- Windows.Devices.Sensors
- Windows.Networking.Proximity
- Windows.Phone.Media.Capture
- Windows.Phone.Devices.Power


3. Compatibility
-------------------------------------------------------------------------------

Windows Phone 7.5 and Windows Phone 8.0 devices. Tested on Nokia Lumia 920, 620
and 520. 

3.1 Required capabilities
-------------------------

- ID_CAP_ISV_CAMERA
- ID_CAP_MEDIALIB_AUDIO
- ID_CAP_PROXIMITY
- ID_CAP_SENSORS

3.2 Known issues
----------------

None.


4. License
-------------------------------------------------------------------------------

See the license text file delivered with this project. The license file is also
available online at
https://github.com/nokia-developer/hardware-info/blob/master/Licence.txt


5. Related documentation
-------------------------------------------------------------------------------

Nokia Lumia Developer's Library:
- https://www.developer.nokia.com/Resources/Library/Lumia/#!index.html
- https://www.developer.nokia.com/Resources/Library/Lumia/#!how-to-adapt-to-lumia-phone-hardware-features.html


6. Version history
-------------------------------------------------------------------------------

* Version 1.1: Refactored the user interface. Characteristics view added.
* Version 1.0: The initial release.
