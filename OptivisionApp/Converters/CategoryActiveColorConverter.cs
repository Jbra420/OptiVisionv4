using System;
using System.Globalization;
using Microsoft.Maui.Controls;
using Microsoft.Maui.Graphics;

namespace OptivisionApp.Converters
{
    public class CategoryActiveColorConverter : IValueConverter
    {
        public object Convert(object? value, Type targetType, object? parameter, CultureInfo culture)
        {
            if (value != null && parameter != null && value.ToString() == parameter.ToString())
            {
                return Color.FromArgb("#2563EB"); // Royal Blue (Active)
            }
            return Color.FromArgb("#94A3B8"); // Slate Gray (Inactive)
        }

        public object ConvertBack(object? value, Type targetType, object? parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
