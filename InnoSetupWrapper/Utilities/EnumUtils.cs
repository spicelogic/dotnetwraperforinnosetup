using System;
using System.ComponentModel;
using System.Reflection;

namespace SpiceLogic.InnoSetupWrapper.Utilities
{
    public static class EnumUtils
    {
        /// <summary>
        /// Strings the value of.
        /// </summary>
        /// <param name="value">The value.</param>
        /// <returns></returns>
        public static string StringValueOf(Enum value)
        {
            FieldInfo fi = value.GetType().GetField(value.ToString());
            DescriptionAttribute[] attributes =
                (DescriptionAttribute[])fi.GetCustomAttributes(typeof(DescriptionAttribute), false);
            return attributes.Length > 0 ? attributes[0].Description : value.ToString();
        }

        /// <summary>
        /// Enums the value of.
        /// </summary>
        /// <param name="value">The value.</param>
        /// <param name="enumType">Type of the enum.</param>
        /// <returns></returns>
        /// <exception cref="System.ArgumentException">The string is not a description or value of the specified enum.</exception>
        public static object EnumValueOf(string value, Type enumType)
        {
            string[] names = Enum.GetNames(enumType);
            for (int i = 0; i < names.Length; i++)
            {
                if (StringValueOf((Enum)Enum.Parse(enumType, names[i])).Equals(value))
                    return Enum.Parse(enumType, names[i]);
            }

            throw new ArgumentException("The string is not a description or value of the specified enum.");
        }
    }
}