using System;
using System.Windows.Controls;

namespace Agent.Interaction.Desktop.Converters
{
    /// <summary>
    /// ValueLengthValidator
    /// </summary>
    public class ValueLengthValidator : ValidationRule
    {
        #region Private Members

        private Int32 _iMaxLen;
        private Int32 _iMinLen;

        #endregion Private Members

        #region Properties

        public Int32 MinLen
        {
            get { return _iMinLen; }
            set { _iMinLen = value; }
        }

        public Int32 MaxLen
        {
            get { return _iMaxLen; }
            set { _iMaxLen = value; }
        }

        #endregion Properties

        /// <summary>
        /// When overridden in a derived class, performs validation checks on a value.
        /// </summary>
        /// <param name="value">The value from the binding target to check.</param>
        /// <param name="cultureInfo">The culture to use in this rule.</param>
        /// <returns>
        /// A <see cref="T:System.Windows.Controls.ValidationResult" /> object.
        /// </returns>
        public override ValidationResult Validate(object value, System.Globalization.CultureInfo cultureInfo)
        {
            try
            {
                String lsString = value.ToString();

                if (lsString.Length < this.MinLen)
                    return new ValidationResult(false, "too short");

                if (lsString.Length > this.MaxLen)
                    return new ValidationResult(false, "too long");

                return new ValidationResult(true, null);
            }
            catch (Exception ex)
            {
                return new ValidationResult(false, ex.ToString());
            }
        }
    }
}