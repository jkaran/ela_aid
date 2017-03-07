using System;
using System.Windows;
using System.Windows.Data;
using System.Windows.Media;

namespace Agent.Interaction.Desktop.Converters
{
    public class DGRowBackgroundConverter : IValueConverter
    {
        #region IValueConverter Members

        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            string statType = value as string;
            if (statType == "AgentGroup")
            {
                return (Brush)new BrushConverter().ConvertFromString("#b8e0ff");
            }
            else
            {
                return DependencyProperty.UnsetValue;
            }
        }

        public object ConvertBack(object value, System.Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            return null;
        }

        #endregion IValueConverter Members
    }
}