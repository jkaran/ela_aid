using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Agent.Interaction.Desktop.Helpers
{
    public interface IOutboundCallResult
    {
        string CallResultKeyName { get; set; }
         
        string CallResultKeyValue { get; set; }
    }
}
