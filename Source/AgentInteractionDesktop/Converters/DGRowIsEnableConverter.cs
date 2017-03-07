using System;
using System.Windows.Data;
using Agent.Interaction.Desktop.Settings;

namespace Agent.Interaction.Desktop.Converters
{
    public class DGRowIsEnableConverter : IValueConverter
    {
        #region IValueConverter Members
        private Datacontext _dataContext = Datacontext.GetInstance();

        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            if (value.ToString().Length > _dataContext.ConsultDialDigits && _dataContext.isOnCall)
            {
                return false;
            }
            else
            {
                return true;
            }
        }

        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            return null;
        }

        #endregion IValueConverter Members
    }
}