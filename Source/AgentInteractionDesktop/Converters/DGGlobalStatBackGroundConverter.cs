using System;
using System.Windows;
using System.Windows.Data;
using System.Windows.Media;
using Agent.Interaction.Desktop.Settings;

namespace Agent.Interaction.Desktop.Converters
{
    internal class DGGlobalStatBackGroundConverter : IMultiValueConverter
    {
        #region IMultiValueConverter Members

        public object Convert(object[] values, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            Datacontext _dataContext = Datacontext.GetInstance();
            string statType = values[0] as string;
            string statName = values[1] as string;
            if (statType == "Queue" || statType == "GroupPlaces")
            {
                if (statName == (_dataContext.AnnexStatValues.ContainsKey("acd-calls-waiting") == true ?
                                                _dataContext.AnnexStatValues["acd-calls-waiting"] : string.Empty))
                    return (System.Windows.Media.Brush)new BrushConverter().ConvertFromString("#5CB3FF");
                else if (statName == (_dataContext.AnnexStatValues.ContainsKey("dn-calls-waiting") == true ?
                                                _dataContext.AnnexStatValues["dn-calls-waiting"] : string.Empty))
                    return (System.Windows.Media.Brush)new BrushConverter().ConvertFromString("#82CAFA");
                else if (statName == (_dataContext.AnnexStatValues.ContainsKey("vq-calls-in-queue") == true ?
                                                _dataContext.AnnexStatValues["vq-calls-in-queue"] : string.Empty))
                    return (System.Windows.Media.Brush)new BrushConverter().ConvertFromString("#98AFC7");
                else
                {
                    return DependencyProperty.UnsetValue;
                }
            }
            else
            {
                return DependencyProperty.UnsetValue;
            }
        }

        public object[] ConvertBack(object values, Type[] targetTypes, object parameter, System.Globalization.CultureInfo culture)
        {
            return null;
        }

        #endregion IMultiValueConverter Members
    }
}