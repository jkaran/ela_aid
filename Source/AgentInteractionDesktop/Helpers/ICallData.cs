using System.Windows.Media;

namespace Agent.Interaction.Desktop.Helpers
{
    public interface ICallData
    {
        string Key { get; set; }

        string Value { get; set; }

        FontFamily Fontfamily { get; set; }

        System.Windows.FontWeight Fontweight { get; set; }
    }
}