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
                return Color.FromArgb("#4F3AD6"); // Color activo (púrpura)
            }
            return Color.FromArgb("#1E1E2E"); // Color inactivo (gris oscuro)
        }

        public object ConvertBack(object? value, Type targetType, object? parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
