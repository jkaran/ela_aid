using System.Windows.Media;

namespace Agent.Interaction.Desktop.Helpers
{
    public class MyMessages : IMyMessages
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="MyMessages"/> class.
        /// </summary>
        /// <param name="index">The index.</param>
        /// <param name="messageIconImageSource">The message icon image source.</param>
        /// <param name="messageType">Type of the message.</param>
        /// <param name="messageSender">The message sender.</param>
        /// <param name="messageSubject">The message subject.</param>
        /// <param name="messagePriority">The message priority.</param>
        /// <param name="messageDate">The message date.</param>
        /// <param name="messageAudience">The message audience.</param>
        /// <param name="messageBody">The message body.</param>
        /// <param name="isread">if set to <c>true</c> [isread].</param>
        #region MyMessages
        public MyMessages(int index, ImageSource messageIconImageSource, string messageType, string messageSender,
           string messageSubject, string messagePriority, string messageDate, string messageAudience, string messageBody, bool isread)
        {
            Index = index;
            MessageIconImageSource = messageIconImageSource;
            MessageType = messageType;
            MessageSender = messageSender;
            MessageSubject = messageSubject;
            MessagePriority = messagePriority;
            MessageDate = messageDate;
            MessageAudience = messageAudience;
            MessageBody = messageBody;
            ISread = isread;
        } 
        #endregion

        #region Properties

        public int Index { get; set; }

        public ImageSource MessageIconImageSource { get; set; }

        public string MessageType { get; set; }

        public string MessageSender { get; set; }

        public string MessageSubject { get; set; }

        public string MessagePriority { get; set; }

        public string MessageDate { get; set; }

        public string MessageAudience { get; set; }

        public string MessageBody { get; set; }

        public bool ISread { get; set; } 
        #endregion
    }
}