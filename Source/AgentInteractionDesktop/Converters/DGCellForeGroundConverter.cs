namespace Agent.Interaction.Desktop.Converters
{
    using System;
    using System.Windows;
    using System.Windows.Data;
    using System.Windows.Media;

    using Pointel.Configuration.Manager;

    public class DGCellForeGroundConverter : IValueConverter
    {
        #region Methods

        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            string input = value as string;
            if (input.StartsWith("www"))
                input = "http://" + input;
            Uri uriResult;
            bool result = Uri.TryCreate(input, UriKind.Absolute, out uriResult)
                && (uriResult.Scheme == Uri.UriSchemeHttp ||
                uriResult.Scheme == Uri.UriSchemeHttps ||
                uriResult.Scheme == Uri.UriSchemeFtp ||
                uriResult.Scheme == Uri.UriSchemeFile);
            if (result &&
                    (ConfigContainer.Instance().AllKeys.Contains("voice.enable.attach-data-popup-url") &&
                        ((string)ConfigContainer.Instance().GetValue("voice.enable.attach-data-popup-url")).ToLower().Equals("true")))
            {
                return Brushes.Blue;
            }
            else
            {
                return DependencyProperty.UnsetValue;
            }
        }

        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            return null;
        }

        #endregion Methods
    }
}