using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Navigation;
using Microsoft.Phone.Controls;
using Microsoft.Phone.Shell;
using HardwareInfo.Resources;
using System.Xml.Linq;

namespace HardwareInfo
{
    public partial class AboutPage : PhoneApplicationPage
    {
        public AboutPage()
        {
            InitializeComponent();
            VersionTextBlock.Text = AppResources.Version + " " + XDocument.Load("WMAppManifest.xml").Root.Element("App").Attribute("Version").Value;
        }
    }
}