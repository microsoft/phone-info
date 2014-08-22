/*
 * Copyright (c) 2013-2014 Microsoft Mobile. All rights reserved.
 * See the license text file delivered with this project for more information.
 */

using System;
using System.ComponentModel;
using System.Diagnostics;
using System.Net;
using System.Windows;
using System.Windows.Input;

namespace PhoneInfo.DataModel
{
    /// <summary>
    /// Container class for the property values bound to the pivot view.
    /// </summary>
    public class BoolItemModel : INotifyPropertyChanged
    {
        private bool _booleanValue;
        private string _textValue;

        public bool BooleanValue
        {
            get
            {
                return _booleanValue;
            }
            set
            {
                if (value != _booleanValue)
                {
                    _booleanValue = value;
                    NotifyPropertyChanged("BooleanValue");
                }
            }
        }

        public string HardwareFeatureText
        {
            get
            {
                return _textValue;
            }
            set
            {
                if (value != _textValue)
                {
                    _textValue = value;
                    NotifyPropertyChanged("HardwareFeatureText");
                }
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;
        private void NotifyPropertyChanged(String propertyName)
        {
            PropertyChangedEventHandler handler = PropertyChanged;
            if (null != handler)
            {
                handler(this, new PropertyChangedEventArgs(propertyName));
            }
        }
    }
}