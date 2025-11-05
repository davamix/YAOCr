using Microsoft.UI.Xaml.Data;
using System;

namespace YAOCr.Converters;
public class NullToBooleanConverter : IValueConverter {
    public object Convert(object value, Type targetType, object parameter, string language) {
        return value != null;
    }

    public object ConvertBack(object value, Type targetType, object parameter, string language) {
        throw new NotImplementedException();
    }
}
