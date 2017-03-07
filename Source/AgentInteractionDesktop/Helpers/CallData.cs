using System.Windows.Media;

namespace Agent.Interaction.Desktop.Helpers
{
    public class CallData : ICallData
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="CallData"/> class.
        /// </summary>
        /// <param name="key">The key.</param>
        /// <param name="value">The value.</param>
        /// <param name="fontFamily">The font family.</param>
        /// <param name="fontWeight">The font weight.</param>

        #region CallData
        public CallData(string key, string value, FontFamily fontFamily, System.Windows.FontWeight fontWeight)
        {
            Key = key;
            Value = value;
            Fontfamily = fontFamily;
            Fontweight = fontWeight;
        } 
        #endregion

        #region Properties
        public string Key { get; set; }

        public string Value { get; set; }

        public FontFamily Fontfamily { get; set; }

        public System.Windows.FontWeight Fontweight { get; set; } 
        #endregion
    }
}