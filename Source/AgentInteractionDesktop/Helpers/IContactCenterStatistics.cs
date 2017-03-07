using System.Windows.Media;

namespace Agent.Interaction.Desktop.Helpers
{
    public interface IContactCenterStatistics
    {
        string ContactStatisticName { get; set; }

        string ContactStatisticDesc { get; set; }

        string ContactStatisticValue { get; set; }

        Brush ContactThresoldColor { get; set; }

        string ContactStatisticType { get; set; }
    }
}