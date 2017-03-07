using System;
using System.Windows;
using System.Windows.Data;
using System.Windows.Media;
using Agent.Interaction.Desktop.Settings;

namespace Agent.Interaction.Desktop.Converters
{
    public class DGRowForegroundConverter : IMultiValueConverter
    {
        #region IMultiValueConverter Members

        public object Convert(object[] values, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            Datacontext _dataContext = Datacontext.GetInstance();
            string input = values[0] as string;
            string number = values[1] as string;
            if (number.ToString().Length > _dataContext.ConsultDialDigits && _dataContext.isOnCall)
            {
                return (Brush)new BrushConverter().ConvertFromString("#888");
            }
            else
            {
                if (input == "11")
                {
                    return Brushes.Red;
                }
                else if (input == "12")
                {
                    return Brushes.Blue;
                }
                else if (input == "13")
                {
                    return Brushes.Green;
                }
                else
                {
                    return DependencyProperty.UnsetValue;
                }
            }
        }

        public object[] ConvertBack(object value, Type[] targetTypes, object parameter, System.Globalization.CultureInfo culture)
        {
            return null;
        }

        #endregion IMultiValueConverter Members
    }
}