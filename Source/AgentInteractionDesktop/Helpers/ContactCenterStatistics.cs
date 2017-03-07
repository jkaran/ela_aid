using System;
using System.ComponentModel;
using System.Linq.Expressions;
using System.Windows.Media;

namespace Agent.Interaction.Desktop.Helpers
{
    public class ContactCenterStatistics : IContactCenterStatistics, INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;
        /// <summary>
        /// Initializes a new instance of the <see cref="ContactCenterStatistics"/> class.
        /// </summary>
        /// <param name="statisticName">Name of the statistic.</param>
        /// <param name="statisticDesc">The statistic description.</param>
        /// <param name="statisticValue">The statistic value.</param>
        /// <param name="thresholdcolor">The thresholdcolor.</param>
        /// <param name="contactStatisticType">Type of the contact statistic.</param>

        #region ContactCenterStatistics
        public ContactCenterStatistics(string statisticName, string statisticDesc, string statisticValue, Brush thresholdcolor, string contactStatisticType)
        {
            ContactStatisticName = statisticName;
            ContactStatisticDesc = statisticDesc;
            ContactStatisticValue = statisticValue;
            ContactThresoldColor = thresholdcolor;
            if(ContactThresoldColor.CanFreeze)
                ContactThresoldColor.Freeze();
            ContactStatisticType = contactStatisticType;
        } 
        #endregion

        #region Properties
        private string _contactStatisticName;
        public string ContactStatisticName
        {
            get { return _contactStatisticName; }
            set
            {
                _contactStatisticName = value;
                RaisePropertyChanged(() => ContactStatisticName);
            }
        }

        private string _contactStatisticDesc;
        public string ContactStatisticDesc
        {
            get { return _contactStatisticDesc; }
            set
            {
                _contactStatisticDesc = value;
                RaisePropertyChanged(() => ContactStatisticDesc);
            }
        }

        private string _contactStatisticValue;
        public string ContactStatisticValue
        {
            get { return _contactStatisticValue; }
            set
            {
                _contactStatisticValue = value;
                RaisePropertyChanged(() => ContactStatisticValue);
            }
        }
        private Brush _contactThresoldColor;
        public Brush ContactThresoldColor
        {
            get { return _contactThresoldColor; }
            set
            {
                _contactThresoldColor = value;
                RaisePropertyChanged(() => ContactThresoldColor);
            }
        }

        private string _contactStatisticType;
        public string ContactStatisticType
        {
            get { return _contactStatisticType; }
            set
            {
                _contactStatisticType = value;
                RaisePropertyChanged(() => ContactStatisticType);
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