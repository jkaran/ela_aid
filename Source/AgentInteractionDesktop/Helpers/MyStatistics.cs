using System;
using System.ComponentModel;
using System.Linq.Expressions;
using System.Windows.Media;

namespace Agent.Interaction.Desktop.Helpers
{
    public class MyStatistics : IMyStatistics, INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;
        /// <summary>
        /// Initializes a new instance of the <see cref="MyStatistics"/> class.
        /// </summary>
        /// <param name="statisticName">Name of the statistic.</param>
        /// <param name="statisticValue">The statistic value.</param>
        /// <param name="thresholdcolor">The thresholdcolor.</param>
        /// <param name="statisticType">Type of the statistic.</param>
        #region MyStatistics
        public MyStatistics(string statisticName, string statisticValue, Brush thresholdcolor, string statisticType)
        {
            StatisticName = statisticName;
            StatisticValue = statisticValue;
            ThresoldColor = thresholdcolor;
            if (ThresoldColor.CanFreeze)
                ThresoldColor.Freeze();
            StatisticType = statisticType;
        } 
        #endregion

        #region Properties
        private string _statisticName;
        public string StatisticName
        {
            get { return _statisticName; }
            set
            {
                _statisticName = value;
                RaisePropertyChanged(() => StatisticName);
            }
        }
        private string _statisticValue;
        public string StatisticValue
        {
            get { return _statisticValue; }
            set
            {
                _statisticValue = value;
                RaisePropertyChanged(() => StatisticValue);
            }
        }
        private Brush _thresoldColor;
        public Brush ThresoldColor
        {
            get { return _thresoldColor; }
            set
            {
                _thresoldColor = value;
                RaisePropertyChanged(() => ThresoldColor);
            }
        }

        private string _statisticType;
        public string StatisticType
        {
            get { return _statisticType; }
            set
            {
                _statisticType = value;
                RaisePropertyChanged(() => StatisticType);
            }
        } 
        #endregion

        #region INotifyPropertyChabge

        /// <summary>
        /// Raises the property changed.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="action">The action.</param>
        protected void RaisePropertyChanged<T>(Expression<Func<T>> action)
        {
            var propertyName = GetPropertyName(action);
            RaisePropertyChanged(propertyName);
        }

        /// <summary>
        /// Gets the name of the property.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="action">The action.</param>
        /// <returns></returns>
        private static string GetPropertyName<T>(Expression<Func<T>> action)
        {
            var expression = (MemberExpression)action.Body;
            var propertyName = expression.Member.Name;
            return propertyName;
        }

        /// <summary>
        /// Raises the property changed.
        /// </summary>
        /// <param name="propertyName">Name of the property.</param>
        private void RaisePropertyChanged(string propertyName)
        {
            if (PropertyChanged != null)
                PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
        }

        #endregion INotifyPropertyChabge
    }
}