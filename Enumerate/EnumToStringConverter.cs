﻿using Enumerate.Properties;
using System;
using System.ComponentModel;
using System.Globalization;
using System.Reflection;
using System.Windows.Data;

namespace Enumerate
{
    [ValueConversion(typeof(Enum), typeof(string))]
    public class EnumToStringConverter : IValueConverter
    {
        #region IValueConverter Members

        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value == null)
            { return null; }

            CheckSourceType(typeof(Enum), value);
            CheckTargetType(typeof(string), targetType, true);

            Type valueType = value.GetType();
            FieldInfo fieldInfo = valueType.GetField(value.ToString(), BindingFlags.Static | BindingFlags.Public);

            if (fieldInfo == null)
            { throw new ArgumentException(Resources.BitFieldsNotSupported, "value"); }

            DescriptionAttribute[] attributes = (DescriptionAttribute[])fieldInfo.GetCustomAttributes(typeof(DescriptionAttribute), false);

            if (attributes.Length > 0)
            { return attributes[0].Description; }
            else
            { return fieldInfo.Name; }
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value == null)
            { return null; }

            CheckSourceType(typeof(string), value);
            CheckTargetType(typeof(Enum), targetType, false);

            string str = (string)value;

            foreach (FieldInfo fieldInfo in targetType.GetFields(BindingFlags.Static | BindingFlags.Public))
            {
                if (fieldInfo.Name == str)
                { return fieldInfo.GetValue(null); }

                DescriptionAttribute[] attributes = (DescriptionAttribute[])fieldInfo.GetCustomAttributes(typeof(DescriptionAttribute), false);

                foreach (DescriptionAttribute attribute in attributes)
                {
                    if (attribute.Description == str)
                    { return fieldInfo.GetValue(null); }
                }
            }

            throw new ArgumentException(string.Format(Resources.EnumValueNotFound, str), "value");
        }

        #endregion

        #region Private Methods

        private static void CheckSourceType(Type supportedSourceType, object value)
        {
            if (!supportedSourceType.IsInstanceOfType(value))
            { throw new ArgumentException(string.Format(CultureInfo.CurrentCulture, Resources.ValueNotOfType, supportedSourceType.Name), "value"); }
        }

        private static void CheckTargetType(Type supportedTargeType, Type requestedTargetType, bool covariance)
        {
            if (covariance)
            {
                if (!requestedTargetType.IsAssignableFrom(supportedTargeType))
                { throw new ArgumentException(string.Format(CultureInfo.CurrentCulture, Resources.TargetNotExtendingType, requestedTargetType.Name, supportedTargeType.Name), "targetType"); }
            }
            else
            {
                if (!supportedTargeType.IsAssignableFrom(requestedTargetType))
                { throw new ArgumentException(string.Format(CultureInfo.CurrentCulture, Resources.TargetNotExtendingType, requestedTargetType.Name, supportedTargeType.Name), "targetType"); }
            }
        }

        #endregion
    }
}