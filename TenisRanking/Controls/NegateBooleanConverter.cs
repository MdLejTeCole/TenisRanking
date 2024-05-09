using Microsoft.UI.Xaml.Data;
using System;

namespace GameTools.Controls;

public class NegateBooleanConverter : IValueConverter
{
    public object Convert(object value, Type targetType, object parameter, string language)
    {
        if (value is bool booleanValue)
        {
            return !booleanValue;
        }
        return value;
    }

    public object ConvertBack(object value, Type targetType, object parameter, string language)
    {
        throw new NotImplementedException();
    }
}
