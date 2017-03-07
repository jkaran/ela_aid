using System.Windows.Media;

namespace Agent.Interaction.Desktop.Helpers
{
    public interface IMyMessages
    {
        int Index { get; set; }

        ImageSource MessageIconImageSource { get; set; }

        string MessageType { get; set; }

        string MessageSender { get; set; }

        string MessageSubject { get; set; }

        string MessagePriority { get; set; }

        string MessageDate { get; set; }

        string MessageAudience { get; set; }

        string MessageBody { get; set; }

        bool ISread { get; set; }
    }
}