using System.Windows.Data;
using System.Windows.Media;
using Agent.Interaction.Desktop.Settings;

namespace Agent.Interaction.Desktop.Converters
{
    public class ValueConverter : IValueConverter
    {
        public object Convert(object value, System.Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            Datacontext _dataContext = Datacontext.GetInstance();
            if (value == null)
                return Brushes.Black;

            var color = new SolidColorBrush((System.Windows.Media.Color)ColorConverter.ConvertFromString(_dataContext.BroadCastAttributes[value.ToString()]));
            return color;

            //var dValue = System.Convert.ToDecimal(value);
            //if (dValue < 0)
            //    return new SolidColorBrush(_dataContext.BroadCastForegroundBrush);
            //else
            // return new SolidColorBrush(_dataContext.BroadCastForegroundBrush);
        }

        public object ConvertBack(object value, System.Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            return null;
        }
    }
}