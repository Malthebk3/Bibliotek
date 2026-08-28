using System;
using System.Globalization;
using Avalonia.Data.Converters;

namespace Bibliotek.Avalonia.Converters;

public class BoolToUserTypeConverter : IValueConverter
{
    public object? Convert(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        return value is true ? "Premium" : "Standard";
    }

    // One-way display only; converting back makes no sense here
    public object? ConvertBack(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        throw new NotSupportedException();
    }
}