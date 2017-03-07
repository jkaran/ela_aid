using System;
using System.Windows.Data;

namespace Agent.Interaction.Desktop.Converters
{
    public class UnsavedData : IMultiValueConverter
    {
        //IMultiValueConverter
        public object Convert(object[] values, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            try
            {
                if (values[0] != null && values[1] != null)
                {
                    if (values[0].ToString() != values[1].ToString())
                    {
                        return false;
                    }
                }
                return true;
            }
            catch { return true; }
        }

        public object[] ConvertBack(object value, Type[] targetTypes, object parameter, System.Globalization.CultureInfo culture)
        {
            return null;
        }
    }
}
