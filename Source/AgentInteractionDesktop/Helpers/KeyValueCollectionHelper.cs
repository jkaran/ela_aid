using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Genesyslab.Platform.Commons.Collections;

namespace Agent.Interaction.Desktop.Helpers
{
    public static class KeyValueCollectionHelper
    {
        public static KeyValueCollection ToKeyValueCollection(this Dictionary<string, string> dictionary)
        {
            KeyValueCollection holdingUpdateUserData = new KeyValueCollection();
            foreach (string key in dictionary.Keys)
                holdingUpdateUserData.Add(key, dictionary[key]);

            return holdingUpdateUserData;
        }
    }
}
