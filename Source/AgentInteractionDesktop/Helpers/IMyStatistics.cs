using System.Windows.Media;

namespace Agent.Interaction.Desktop.Helpers
{
    public interface IMyStatistics
    {
        string StatisticName { get; set; }

        string StatisticValue { get; set; }

        Brush ThresoldColor { get; set; }

        string StatisticType { get; set; }
    }
}