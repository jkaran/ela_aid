using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Agent.Interaction.Desktop.Helpers
{
    public class OutboundCallResult: IOutboundCallResult
    {

        /// <summary>
        /// Initializes a new instance of the <see cref="OutboundCallResult"/> class.
        /// </summary>
        /// <param name="key">The key.</param>
        /// <param name="value">The value.</param>
        public OutboundCallResult(string key,string value)
        {
            CallResultKeyName = key;
            CallResultKeyValue = value;
        }

        #region Properties
        public string CallResultKeyName { get; set; }

        public string CallResultKeyValue  { get; set; }
        #endregion
    }
}
