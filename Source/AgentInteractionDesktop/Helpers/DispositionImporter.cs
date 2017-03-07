using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.Windows.Controls;

namespace Agent.Interaction.Desktop.Helpers
{
    public class DispositionImporter
    {
        [ImportMany(typeof(UserControl), AllowRecomposition = true, RequiredCreationPolicy = CreationPolicy.NonShared)]
        public IEnumerable<UserControl> window { get; set; }
    }
}
