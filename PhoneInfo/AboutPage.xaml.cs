using Microsoft.Phone.Controls;
using PhoneInfo.Resources;
using System.Xml.Linq;

namespace PhoneInfo
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