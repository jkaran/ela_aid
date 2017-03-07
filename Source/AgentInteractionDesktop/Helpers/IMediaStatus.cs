using System.Windows;
using System.Windows.Media;

namespace Agent.Interaction.Desktop.Helpers
{
    public interface IMediaStatus
    {
        ImageSource ChannelIconImageSource { get; set; }

        string ChannelName { get; set; }

        ImageSource ChannelStateImageSource { get; set; }

        string ChannelState { get; set; }

        string ChannelInitialTime { get; set; }

        string Forward { get; set; }

        string Information { get; set; }

        Visibility StateTimer { get; set; }
    }
}