/**
 * Copyright (c) 2013-2014 Microsoft Mobile.
 */

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Data;

namespace PhoneInfo
{
    /// <summary>
    /// A small utility class for converting boolean values into matching icon
    /// URIs.
    /// </summary>
    public class BooleanToIconUriConverter : IValueConverter
    {
        // Constants
        public const String NotSupportedIconUri = "Assets/Images/cancel.png";
        public const String SupportedIconUri = "Assets/Images/check.png";

        /// <summary>
        /// Returns an icon URI as a String based on the given boolean value.
        /// If the value is not boolean, a NotSupportedException will be thrown.
        /// </summary>
        /// <param name="value">A boolean value.</param>
        /// <param name="targetType"></param>
        /// <param name="parameter"></param>
        /// <param name="culture"></param>
        /// <returns>An icon URI as a String.</returns>
        public object Convert(object value,
                              Type targetType,
                              object parameter,
                              System.Globalization.CultureInfo culture)
        {
            if (value != null && value is bool)
            {
                if ((bool)value)
                {
                    return SupportedIconUri;
                }

                return NotSupportedIconUri;
            }

            throw new NotSupportedException();
        }

        /// <summary>
        /// Not implemented. Throws NotSupportedException.
        /// </summary>
        /// <param name="value"></param>
        /// <param name="targetType"></param>
        /// <param name="parameter"></param>
        /// <param name="culture"></param>
        /// <returns></returns>
        public object ConvertBack(object value,
                                  Type targetType,
                                  object parameter,
                                  System.Globalization.CultureInfo culture)
        {
            throw new NotSupportedException();
        }
    }
}
