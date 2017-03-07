using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.Windows;

namespace Agent.Interaction.Desktop.Helpers
{
    public class ImportLanguageModule
    {
        [ImportMany(typeof(ResourceDictionary))]
        public IEnumerable<Lazy<ResourceDictionary, IDictionary<string, object>>> ResourceDictionaryList { get; set; }
    }
}