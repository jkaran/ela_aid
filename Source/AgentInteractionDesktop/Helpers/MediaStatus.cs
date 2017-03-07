using System.Windows;
using System.Windows.Media;

namespace Agent.Interaction.Desktop.Helpers
{
    public class MediaStatus : IMediaStatus
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="MediaStatus"/> class.
        /// </summary>
        /// <param name="channelIconImageSource">The channel icon image source.</param>
        /// <param name="channelName">Name of the channel.</param>
        /// <param name="channelStateImageSource">The channel state image source.</param>
        /// <param name="channelState">State of the channel.</param>
        /// <param name="channelInitialTime">The channel initial time.</param>
        /// <param name="forward">The forward.</param>
        /// <param name="information">The information.</param>
        /// <param name="stateTimer">The state timer.</param>
       
        #region MediaStatus
        public MediaStatus(ImageSource channelIconImageSource, string channelName, ImageSource channelStateImageSource,
           string channelState, string channelInitialTime, string forward, string information, Visibility stateTimer)
        {
            ChannelIconImageSource = channelIconImageSource;
            ChannelName = channelName;
            ChannelStateImageSource = channelStateImageSource;
            ChannelState = channelState;
            ChannelInitialTime = channelInitialTime;
            Forward = forward;
            Information = information;
            StateTimer = stateTimer;
        } 
        #endregion

        #region Properties
        public ImageSource ChannelIconImageSource { get; set; }

        public string ChannelName { get; set; }

        public ImageSource ChannelStateImageSource { get; set; }

        public string ChannelState { get; set; }

        public string ChannelInitialTime { get; set; }

        public string Forward { get; set; }

        public string Information { get; set; }

        public Visibility StateTimer { get; set; } 
        #endregion
    }
}