namespace Agent.Interaction.Desktop.ChannelsHandler
{
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Linq;
    using System.Windows;
    using System.Windows.Media;
    using System.Windows.Media.Imaging;
    using System.Windows.Resources;

    using Agent.Interaction.Desktop.Helpers;
    using Agent.Interaction.Desktop.Settings;

    using Genesyslab.Platform.Commons.Protocols;
    using Genesyslab.Platform.OpenMedia.Protocols;

    using Pointel.Configuration.Manager;
    using Pointel.Interactions.Contact.Core;
    using Pointel.Interactions.Core;
    using Pointel.Interactions.IPlugins;
    using Genesyslab.Platform.Commons.Collections;

    public class InteractionHandler : IInteractionServices, IContactService
    {
        #region Fields

        public static string AgentChatState = "Logout";
        public static string AgentChatStateBeforeDND = "Logout";
        public static string AgentEmailState = "Logout";
        public static string AgentEmailStateBeforeDND = "Logout";
        public static string AgentOutboundState = "Logout";
        public static string AgentOutboundStateBeforeDND = "Logout";
        public static bool _isChatNotReadyReason = false;
        public static bool _isEmailNotReadyReason = false;
        public static bool _isIXNServerDown;
        public static bool _isOutboundNotReadyReason = false;

        public List<string> _lstChatState = new List<string>();
        public List<string> _lstEmailState = new List<string>();
        public List<string> _lstOutboundState = new List<string>();
        public List<string> _lstSocialMediaState = new List<string>();

        private static int _currentChatDNDOnTime = 0;
        private static int _currentChatLogoutTime = 0;
        private static int _currentChatNReadyTime = 0;
        private static int _currentChatReadyTime = 0;
        private static int _currentEmailDNDOnTime = 0;
        private static int _currentEmailLogoutTime = 0;
        private static int _currentEmailNReadyTime = 0;
        private static int _currentEmailReadyTime = 0;
        private static int _currentOutboundDNDOnTime = 0;
        private static int _currentOutboundLogoutTime = 0;
        private static int _currentOutboundNReadyTime = 0;
        private static int _currentOutboundReadyTime = 0;
        private static int _outOfServiceTime = 0;
        private static int _serverDownTime;

        private IContactService contactListener;
        private IInteractionServices ixnListener;
        private string _chatImagePath = string.Empty;
        private ConfigContainer _configContainer = ConfigContainer.Instance();
        private Datacontext _dataContext = Datacontext.GetInstance();
        private string _emailImagePath = string.Empty;
        private ImageDatacontext _imageDataContext = ImageDatacontext.GetInstance();
        private Pointel.Logger.Core.ILog _logger = Pointel.Logger.Core.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType,
           "AID");
        private PluginCollection _plugins = PluginCollection.GetInstance();

        #endregion Fields

        #region Constructors

        /// <summary>
        /// Initializes a new instance of the <see cref="InteractionHandler"/> class.
        /// </summary>
        public InteractionHandler()
        {
            ixnListener = this;
            contactListener = this;
            if (_configContainer.AllKeys.Contains("image-path"))
            {
                _emailImagePath = _configContainer.GetValue("image-path") + "\\Email";
                _chatImagePath = _configContainer.GetValue("image-path") + "\\Chat";
            }
            else
            {
                _emailImagePath = Environment.CurrentDirectory.ToString() + @"\\Email";
                _chatImagePath = Environment.CurrentDirectory.ToString() + @"\\Chat";
            }
        }

        #endregion Constructors

        #region Delegates

        public delegate void ChannelStates(
            string channelName, ImageSource imgVoiceSource, string channelState,
            System.Windows.Visibility isStateTimer, string channelTime);

        public delegate void NeedToConnectServer(string serverName);

        #endregion Delegates

        #region Events

        public event ChannelStates channelStates;

        public event NeedToConnectServer EventNeedToConnectServer;

        #endregion Events

        #region Methods

        /// <summary>
        /// Gets the bitmap image.
        /// </summary>
        /// <param name="uri">The URI.</param>
        /// <returns></returns>
        public static BitmapImage GetBitmapImage(Uri uri)
        {
            StreamResourceInfo imageInfo = System.Windows.Application.GetResourceStream(uri);
            var bitmap = new BitmapImage();
            try
            {
                byte[] imageBytes = ReadFully(imageInfo.Stream);
                using (Stream stream = new MemoryStream(imageBytes))
                {
                    bitmap.BeginInit();
                    bitmap.StreamSource = stream;
                    bitmap.CacheOption = BitmapCacheOption.OnLoad;
                    bitmap.EndInit();
                    bitmap.UriSource = uri;
                    if (bitmap.CanFreeze)
                        bitmap.Freeze();
                }
                imageBytes = null;
                return bitmap;
            }
            catch
            {
                return null;
            }
            finally
            {
                imageInfo = null;
                bitmap = null;
            }
        }

        /// <summary>
        /// Reads the fully.
        /// </summary>
        /// <param name="input">The input.</param>
        /// <returns></returns>
        public static byte[] ReadFully(Stream input)
        {
            byte[] buffer = new byte[16 * 1024];
            using (MemoryStream ms = new MemoryStream())
            {
                int read;
                while ((read = input.Read(buffer, 0, buffer.Length)) > 0)
                {
                    ms.Write(buffer, 0, read);
                }
                return ms.ToArray();
            }
        }

        /// <summary>
        /// Agents the media state subscriber.
        /// </summary>
        /// <param name="ixnService">The ixn service.</param>
        public void AgentMediaStateSubscriber(InteractionService ixnService)
        {
            ixnService.AgentMediaStateSubscriber(ixnListener);
        }

        /// <summary>
        /// Changes the chat status.
        /// </summary>
        /// <param name="status">The status.</param>
        public void ChangeChatStatus(string status)
        {
            try
            {
                // System.Windows.Application.Current.Dispatcher.Invoke((Action)(delegate
                //{
                if (!_dataContext.htMediaCurrentState.ContainsKey(Datacontext.Channels.Chat)) return;
                switch (status)
                {
                    case "Logout":
                        if (_lstChatState.Count > 0 && _lstChatState.Contains("Logout"))
                        {
                            _lstChatState.Clear();
                            _currentChatLogoutTime = _currentChatLogoutTime + 1;
                        }
                        else
                        {
                            if (_dataContext.htMediaCurrentState[Datacontext.Channels.Chat].ToString() == status)
                            {
                                TimeSpan time = TimeSpan.FromSeconds(_currentChatLogoutTime);
                                string timer = "[" +
                                                string.Format("{0:D2}:{1:D2}:{2:D2}", time.Hours, time.Minutes,
                                                    time.Seconds) + "]";
                                channelStates.Invoke(Datacontext.Channels.Chat.ToString(), _dataContext.ChatStateImageSource, status, System.Windows.Visibility.Visible, timer);
                            }
                            else
                            {
                                _dataContext.htMediaCurrentState[Datacontext.Channels.Chat] = status;
                                channelStates.Invoke(Datacontext.Channels.Chat.ToString(), _dataContext.ChatStateImageSource, status, System.Windows.Visibility.Visible, "[00:00:00]");
                                _currentChatLogoutTime = 0;
                            }
                        }
                        break;
                    case "Logout (Do Not Disturb)":
                        {
                            InteractionHandler.AgentChatState = "Logout (Do Not Disturb)";
                            if (_lstChatState.Count > 0 && _lstChatState.Contains("Logout (Do Not Disturb)"))
                            {
                                _lstChatState.Clear();
                                _currentChatLogoutTime = _currentChatLogoutTime + 1;
                            }
                            else
                            {
                                _dataContext.htMediaCurrentState[Datacontext.Channels.Chat] = "Logout (Do Not Disturb)";
                                channelStates.Invoke(Datacontext.Channels.Chat.ToString(), _dataContext.ChatStateImageSource, _dataContext.htMediaCurrentState[Datacontext.Channels.Chat].ToString(), System.Windows.Visibility.Visible, "[00:00:00]");
                                _currentChatLogoutTime = 0;
                            }
                            break;
                        }
                    case "Ready":
                        if (_lstChatState.Count > 0 && _lstChatState.Contains("Ready"))
                        {
                            _lstChatState.Clear();
                            _currentChatReadyTime = _currentChatReadyTime + 1;
                        }
                        else
                        {
                            if (_dataContext.htMediaCurrentState[Datacontext.Channels.Chat].ToString() == status)
                            {
                                TimeSpan time = TimeSpan.FromSeconds(_currentChatReadyTime);
                                string timer = "[" +
                                                string.Format("{0:D2}:{1:D2}:{2:D2}", time.Hours, time.Minutes,
                                                    time.Seconds) + "]";
                                channelStates.Invoke(Datacontext.Channels.Chat.ToString(), _dataContext.ChatStateImageSource, status, System.Windows.Visibility.Visible, timer);
                            }
                            else
                            {
                                _dataContext.htMediaCurrentState[Datacontext.Channels.Chat] = status;
                                channelStates.Invoke(Datacontext.Channels.Chat.ToString(), _dataContext.ChatStateImageSource, status, System.Windows.Visibility.Visible, "[00:00:00]");
                                _currentChatReadyTime = 0;
                            }
                        }
                        break;
                    case "Not Ready":
                        if (_lstChatState.Count > 0 && _lstChatState.Contains("Not Ready"))
                        {
                            _lstChatState.Clear();
                            _currentChatNReadyTime = _currentChatNReadyTime + 1;
                        }
                        else
                        {
                            if (_dataContext.htMediaCurrentState[Datacontext.Channels.Chat].ToString() == status)
                            {
                                TimeSpan time = TimeSpan.FromSeconds(_currentChatNReadyTime);
                                string timer = "[" +
                                                string.Format("{0:D2}:{1:D2}:{2:D2}", time.Hours, time.Minutes,
                                                    time.Seconds) + "]";
                                channelStates.Invoke(Datacontext.Channels.Chat.ToString(), _dataContext.ChatStateImageSource, status, System.Windows.Visibility.Visible, timer);
                            }
                            else
                            {
                                _dataContext.htMediaCurrentState[Datacontext.Channels.Chat] = status;
                                channelStates.Invoke(Datacontext.Channels.Chat.ToString(), _dataContext.ChatStateImageSource, status, System.Windows.Visibility.Visible, "[00:00:00]");
                                _currentChatNReadyTime = 0;
                            }
                        }
                        break;
                    case "Do Not Disturb":
                        if (_lstChatState.Count > 0 && _lstChatState.Contains("Do Not Disturb"))
                        {
                            _lstChatState.Clear();
                            _currentChatDNDOnTime = _currentChatDNDOnTime + 1;
                        }
                        else
                        {
                            if (_dataContext.htMediaCurrentState[Datacontext.Channels.Chat].ToString() == status)
                            {

                                TimeSpan time = TimeSpan.FromSeconds(_currentChatDNDOnTime);
                                string timer = "[" +
                                                string.Format("{0:D2}:{1:D2}:{2:D2}", time.Hours, time.Minutes,
                                                    time.Seconds) + "]";
                                channelStates.Invoke(Datacontext.Channels.Chat.ToString(), _dataContext.ChatStateImageSource, status, System.Windows.Visibility.Visible, timer);
                            }
                            else
                            {
                                _dataContext.htMediaCurrentState[Datacontext.Channels.Chat] = status;
                                channelStates.Invoke(Datacontext.Channels.Chat.ToString(), _dataContext.ChatStateImageSource, status, System.Windows.Visibility.Visible, "[00:00:00]");
                                _currentChatDNDOnTime = 0;
                            }
                        }
                        break;
                    case "Log On":
                        if (_lstChatState.Count > 0 && _lstChatState.Contains("Log On"))
                        {
                            _lstChatState.Clear();
                            _currentChatNReadyTime = _currentChatNReadyTime + 1;
                        }
                        else
                        {
                            if (_dataContext.htMediaCurrentState[Datacontext.Channels.Chat].ToString() != "Not Ready")
                            {
                                _dataContext.htMediaCurrentState[Datacontext.Channels.Chat] = "Not Ready";
                                _currentChatNReadyTime = 0;
                            }
                            _dataContext.ChatStateImageSource = _imageDataContext.ImgNotReadyStatus;
                            // _dataContext.ChatStateImageSource = new BitmapImage(new Uri("/Agent.Interaction.Desktop;component/Images/Not_Ready.png", UriKind.Relative));
                            channelStates.Invoke(Datacontext.Channels.Chat.ToString(), _dataContext.ChatStateImageSource, _dataContext.htMediaCurrentState[Datacontext.Channels.Chat].ToString(), System.Windows.Visibility.Visible, "[00:00:00]");
                        }
                        break;
                    case "On Interaction":
                        _dataContext.htMediaCurrentState[Datacontext.Channels.Chat] = InteractionHandler.AgentChatState;
                        channelStates.Invoke(Datacontext.Channels.Chat.ToString(), _dataContext.ChatStateImageSource, _dataContext.htMediaCurrentState[Datacontext.Channels.Chat].ToString(),
                            Visibility.Collapsed, "[00:00:00]");
                        var tempState = InteractionHandler.AgentChatState;
                        if (tempState.Contains("Not Ready"))
                            tempState = "NotReady"; // for notready reason
                        switch (tempState)
                        {
                            case "Ready": _currentChatReadyTime = 0; break;
                            case "NotReady": _currentChatNReadyTime = 0; break;
                            case "Logout": _currentChatLogoutTime = 0; break;
                            case "Do Not Disturb": _currentChatDNDOnTime = 0; break;
                        }
                        break;
                    case "Out Of Service":
                        if (_lstChatState.Count > 0 && _lstChatState.Contains("Out Of Service"))
                        {
                            _lstChatState.Clear();
                            _outOfServiceTime = _outOfServiceTime + 1;
                            _serverDownTime = _serverDownTime + 1;
                            if (_serverDownTime >= 5 && _isIXNServerDown)
                            {
                                _serverDownTime = 0;
                                if (!(_configContainer.AllKeys.Contains("email.enable.plugin") &&
                                   ((string)_configContainer.GetValue("email.enable.plugin")).ToLower().Equals("true")) &&
                                            !_dataContext.IsEmailPluginAdded)
                                {
                                    //below code used to background process
                                    EventNeedToConnectServer.BeginInvoke("Interaction", null, null);
                                }
                            }
                        }
                        else
                        {
                            _dataContext.htMediaCurrentState[Datacontext.Channels.Chat] = status;
                            _dataContext.ChatStateImageSource = _imageDataContext.ImgOutofServiceStatus;
                            //_dataContext.ChatStateImageSource =
                            //    new BitmapImage(new Uri(
                            //        "/Agent.Interaction.Desktop;component/Images/Out_of_Service.png", UriKind.Relative));
                            channelStates.Invoke("Chat", _dataContext.ChatStateImageSource, status,
                                Visibility.Visible, "[00:00:00]");
                            _outOfServiceTime = 0;
                            _serverDownTime = 0;
                        }
                        break;
                    case "Out Of Service1":
                        if (_lstChatState.Count > 0 && _lstChatState.Contains("Out Of Service1"))
                        {
                            _lstChatState.Clear();
                            _outOfServiceTime = _outOfServiceTime + 1;
                        }
                        else
                        {
                            _dataContext.htMediaCurrentState[Datacontext.Channels.Chat] = "Out Of Service";
                            _dataContext.ChatStateImageSource = _imageDataContext.ImgOutofServiceStatus;
                            //_dataContext.ChatStateImageSource =
                            //    new BitmapImage(new Uri(
                            //        "/Agent.Interaction.Desktop;component/Images/Out_of_Service.png", UriKind.Relative));
                            channelStates.Invoke(Datacontext.Channels.Chat.ToString(), _dataContext.ChatStateImageSource, _dataContext.htMediaCurrentState[Datacontext.Channels.Chat].ToString(),
                                Visibility.Visible, "[00:00:00]");
                            _outOfServiceTime = 0;
                        }
                        break;
                    default:
                        if (status.Contains("Not Ready -"))
                        {
                            if (_lstChatState.Count > 0 && _lstChatState[0].Contains("Not Ready -"))
                            {
                                _lstChatState.Clear();
                                _currentChatNReadyTime = _currentChatNReadyTime + 1;
                            }
                            else
                            {
                                if (_dataContext.htMediaCurrentState[Datacontext.Channels.Chat].ToString() == status)
                                {
                                    TimeSpan time = TimeSpan.FromSeconds(_currentChatNReadyTime);
                                    string timer = "[" +
                                                    string.Format("{0:D2}:{1:D2}:{2:D2}", time.Hours, time.Minutes,
                                                        time.Seconds) + "]";
                                    channelStates.Invoke(Datacontext.Channels.Chat.ToString(), _dataContext.ChatStateImageSource, status, System.Windows.Visibility.Visible, timer);
                                }
                                else
                                {
                                    _dataContext.htMediaCurrentState[Datacontext.Channels.Chat] = status;
                                    channelStates.Invoke(Datacontext.Channels.Chat.ToString(), _dataContext.ChatStateImageSource, status, System.Windows.Visibility.Visible, "[00:00:00]");
                                    _currentChatNReadyTime = 0;
                                }
                            }
                        }
                        else if (_dataContext.IsOnChatIXN)
                        {
                            if (status.Contains("Not Ready -"))
                            {
                                if (_lstChatState.Count > 0 && _lstChatState[0].Contains("Not Ready -"))
                                {
                                    _lstChatState.Clear();
                                    _currentChatNReadyTime = _currentChatNReadyTime + 1;
                                }
                                else
                                {
                                    _dataContext.htMediaCurrentState[Datacontext.Channels.Chat] = status;
                                    channelStates.Invoke(Datacontext.Channels.Chat.ToString(), _dataContext.ChatStateImageSource, status, System.Windows.Visibility.Collapsed, "[00:00:00]");
                                    _currentChatNReadyTime = 0;
                                }
                            }
                            else if (status.Contains("Not Ready"))
                            {
                                if (_lstChatState.Count > 0 && _lstChatState.Contains("Not Ready"))
                                {
                                    _lstChatState.Clear();
                                    _currentChatNReadyTime = _currentChatNReadyTime + 1;
                                }
                                else
                                {
                                    _dataContext.htMediaCurrentState[Datacontext.Channels.Chat] = "Not Ready";
                                    channelStates.Invoke(Datacontext.Channels.Chat.ToString(), _dataContext.ChatStateImageSource, _dataContext.htMediaCurrentState[Datacontext.Channels.Chat].ToString(), System.Windows.Visibility.Collapsed, "[00:00:00]");
                                    _currentChatNReadyTime = 0;
                                }
                            }
                            else
                            {
                                if (_lstChatState.Count > 0 && _lstChatState.Contains("Ready"))
                                {
                                    _lstChatState.Clear();
                                    _currentChatReadyTime = _currentChatReadyTime + 1;
                                }
                                else
                                {
                                    _dataContext.htMediaCurrentState[Datacontext.Channels.Chat] = status;
                                    channelStates.Invoke(Datacontext.Channels.Chat.ToString(), _dataContext.ChatStateImageSource, _dataContext.htMediaCurrentState[Datacontext.Channels.Chat].ToString(), System.Windows.Visibility.Collapsed, "[00:00:00]");
                                    _currentChatReadyTime = 0;
                                }
                            }
                        }
                        break;
                }
                //}));
            }
            catch (Exception commonException)
            {
                _logger.Error("InteractionHandler:ChangeChatStatus():" + commonException.ToString());
            }
        }

        /// <summary>
        /// Changes the email status.
        /// </summary>
        /// <param name="status">The status.</param>
        public void ChangeEmailStatus(string status)
        {
            try
            {
                //System.Windows.Application.Current.Dispatcher.Invoke((Action)(delegate
                //{
                switch (status)
                {
                    case "Logout":
                        if (_lstEmailState.Count > 0 && _lstEmailState.Contains("Logout"))
                        {
                            _lstEmailState.Clear();
                            _currentEmailLogoutTime = _currentEmailLogoutTime + 1;
                        }
                        else
                        {
                            if (_dataContext.htMediaCurrentState[Datacontext.Channels.Email].ToString() == status)
                            {
                                TimeSpan time = TimeSpan.FromSeconds(_currentEmailLogoutTime);
                                string timer = "[" +
                                                string.Format("{0:D2}:{1:D2}:{2:D2}", time.Hours, time.Minutes,
                                                    time.Seconds) + "]";
                                channelStates.Invoke(Datacontext.Channels.Email.ToString(), _dataContext.EmailStateImageSource, status, System.Windows.Visibility.Visible, timer);
                            }
                            else
                            {
                                _dataContext.htMediaCurrentState[Datacontext.Channels.Email] = status;
                                channelStates.Invoke(Datacontext.Channels.Email.ToString(), _dataContext.EmailStateImageSource,
                                    _dataContext.htMediaCurrentState[Datacontext.Channels.Email].ToString(), System.Windows.Visibility.Visible,
                                    "[00:00:00]");
                                _currentEmailLogoutTime = 0;
                            }
                        }
                        break;
                    case "Logout (Do Not Disturb)":
                        {
                            InteractionHandler.AgentEmailState = "Logout (Do Not Disturb)";
                            if (_lstEmailState.Count > 0 && _lstEmailState.Contains("Logout (Do Not Disturb)"))
                            {
                                _lstEmailState.Clear();
                                _currentEmailLogoutTime = _currentEmailLogoutTime + 1;
                            }
                            else
                            {
                                _dataContext.htMediaCurrentState[Datacontext.Channels.Email] = "Logout (Do Not Disturb)";
                                channelStates.Invoke(Datacontext.Channels.Email.ToString(), _dataContext.EmailStateImageSource, status, System.Windows.Visibility.Visible, "[00:00:00]");
                                _currentEmailLogoutTime = 0;
                            }
                            break;
                        }
                    case "Ready":
                        if (_lstEmailState.Count > 0 && _lstEmailState.Contains("Ready"))
                        {
                            _lstEmailState.Clear();
                            _currentEmailReadyTime = _currentEmailReadyTime + 1;
                        }
                        else
                        {
                            if (_dataContext.htMediaCurrentState[Datacontext.Channels.Email].ToString() == status)
                            {
                                TimeSpan time = TimeSpan.FromSeconds(_currentEmailReadyTime);
                                string timer = "[" +
                                                string.Format("{0:D2}:{1:D2}:{2:D2}", time.Hours, time.Minutes,
                                                    time.Seconds) + "]";
                                channelStates.Invoke(Datacontext.Channels.Email.ToString(), _dataContext.EmailStateImageSource, status, System.Windows.Visibility.Visible, timer);
                            }
                            else
                            {
                                _dataContext.htMediaCurrentState[Datacontext.Channels.Email] = status;
                                channelStates.Invoke(Datacontext.Channels.Email.ToString(), _dataContext.EmailStateImageSource,
                                    status, System.Windows.Visibility.Visible,
                                    "[00:00:00]");
                                _currentEmailReadyTime = 0;
                            }
                        }
                        break;
                    case "Not Ready":
                        if (_lstEmailState.Count > 0 && _lstEmailState.Contains("Not Ready"))
                        {
                            _lstEmailState.Clear();
                            _currentEmailNReadyTime = _currentEmailNReadyTime + 1;
                        }
                        else
                        {
                            if (_dataContext.htMediaCurrentState[Datacontext.Channels.Email].ToString() == status)
                            {
                                TimeSpan time = TimeSpan.FromSeconds(_currentEmailNReadyTime);
                                string timer = "[" +
                                                string.Format("{0:D2}:{1:D2}:{2:D2}", time.Hours, time.Minutes,
                                                    time.Seconds) + "]";
                                channelStates.Invoke(Datacontext.Channels.Email.ToString(), _dataContext.EmailStateImageSource, status, System.Windows.Visibility.Visible, timer);
                            }
                            else
                            {
                                _dataContext.htMediaCurrentState[Datacontext.Channels.Email] = status;
                                channelStates.Invoke(Datacontext.Channels.Email.ToString(), _dataContext.EmailStateImageSource,
                                    _dataContext.htMediaCurrentState[Datacontext.Channels.Email].ToString(), System.Windows.Visibility.Visible,
                                    "[00:00:00]");
                                _currentEmailNReadyTime = 0;
                            }
                        }
                        break;
                    case "Log On":
                        if (_lstEmailState.Count > 0 && _lstEmailState.Contains("Log On"))
                        {
                            _lstEmailState.Clear();
                            _currentEmailNReadyTime = _currentEmailNReadyTime + 1;
                        }
                        else
                        {
                            _dataContext.EmailStateImageSource = _imageDataContext.ImgNotReadyStatus;
                            //_dataContext.EmailStateImageSource = new BitmapImage(new Uri("/Agent.Interaction.Desktop;component/Images/Not_Ready.png",
                            //                UriKind.Relative));
                            _dataContext.htMediaCurrentState[Datacontext.Channels.Email] = "Not Ready";
                            channelStates.Invoke(Datacontext.Channels.Email.ToString(), _dataContext.EmailStateImageSource,
                                _dataContext.htMediaCurrentState[Datacontext.Channels.Email].ToString(), System.Windows.Visibility.Visible,
                                "[00:00:00]");
                            _currentEmailNReadyTime = 0;
                        }
                        break;
                    case "Do Not Disturb":
                        if (_lstEmailState.Count > 0 && _lstEmailState.Contains("Do Not Disturb"))
                        {
                            _lstEmailState.Clear();
                            _currentEmailDNDOnTime = _currentEmailDNDOnTime + 1;
                        }
                        else
                        {
                            InteractionHandler.AgentEmailState = "Do Not Disturb";
                            if (_dataContext.htMediaCurrentState[Datacontext.Channels.Email].ToString() == status)
                            {

                                TimeSpan time = TimeSpan.FromSeconds(_currentEmailDNDOnTime);
                                string timer = "[" +
                                                string.Format("{0:D2}:{1:D2}:{2:D2}", time.Hours, time.Minutes,
                                                    time.Seconds) + "]";
                                channelStates.Invoke(Datacontext.Channels.Email.ToString(), _dataContext.EmailStateImageSource, status, System.Windows.Visibility.Visible, timer);
                            }
                            else
                            {
                                _dataContext.htMediaCurrentState[Datacontext.Channels.Email] = status;
                                channelStates.Invoke(Datacontext.Channels.Email.ToString(), _dataContext.EmailStateImageSource, status, System.Windows.Visibility.Visible, "[00:00:00]");
                                _currentEmailDNDOnTime = 0;
                            }
                        }
                        break;
                    case "On Interaction":
                        _dataContext.htMediaCurrentState[Datacontext.Channels.Email] = InteractionHandler.AgentEmailState;
                        channelStates.Invoke(Datacontext.Channels.Email.ToString(), _dataContext.EmailStateImageSource, _dataContext.htMediaCurrentState[Datacontext.Channels.Email].ToString(),
                            Visibility.Collapsed, "[00:00:00]");
                        var tempState = InteractionHandler.AgentEmailState;
                        if (tempState.Contains("Not Ready"))
                            tempState = "NotReady"; // for notready reason
                        switch (tempState)
                        {
                            case "Ready": _currentEmailReadyTime = 0; break;
                            case "NotReady": _currentEmailNReadyTime = 0; break;
                            case "Logout": _currentEmailLogoutTime = 0; break;
                            case "Do Not Disturb": _currentEmailDNDOnTime = 0; break;
                        }
                        break;
                    case "Out Of Service":
                        if (_lstEmailState.Count > 0 && _lstEmailState.Contains("Out Of Service"))
                        {
                            _lstEmailState.Clear();
                            _outOfServiceTime = _outOfServiceTime + 1;
                            _serverDownTime = _serverDownTime + 1;
                            if (_serverDownTime >= 5 && _isIXNServerDown)
                            {
                                //below code used to background process
                                EventNeedToConnectServer.BeginInvoke("Interaction", null, null);
                                _serverDownTime = 0;
                            }
                        }
                        else
                        {
                            _dataContext.htMediaCurrentState[Datacontext.Channels.Email] = "Out Of Service";
                            channelStates.Invoke(Datacontext.Channels.Email.ToString(), _dataContext.EmailStateImageSource, _dataContext.htMediaCurrentState[Datacontext.Channels.Email].ToString(),
                                Visibility.Visible, "[00:00:00]");
                            _outOfServiceTime = 0;
                            _serverDownTime = 0;
                        }
                        break;
                    case "Out Of Service1":
                        if (_lstEmailState.Count > 0 && _lstEmailState.Contains("Out Of Service1"))
                        {
                            _lstEmailState.Clear();
                            _outOfServiceTime = _outOfServiceTime + 1;
                        }
                        else
                        {
                            _dataContext.htMediaCurrentState[Datacontext.Channels.Email] = "Out Of Service";
                            channelStates.Invoke(Datacontext.Channels.Email.ToString(), _dataContext.EmailStateImageSource, _dataContext.htMediaCurrentState[Datacontext.Channels.Email].ToString(),
                                Visibility.Visible, "[00:00:00]");
                            _outOfServiceTime = 0;
                        }
                        break;
                    default:
                        if (status.Contains("Not Ready -"))
                        {
                            if (_lstEmailState.Count > 0 && _lstEmailState[0].Contains("Not Ready -"))
                            {
                                _lstEmailState.Clear();
                                _currentEmailNReadyTime = _currentEmailNReadyTime + 1;
                            }
                            else
                            {
                                if (_dataContext.htMediaCurrentState[Datacontext.Channels.Email].ToString() == status)
                                {
                                    TimeSpan time = TimeSpan.FromSeconds(_currentEmailNReadyTime);
                                    string timer = "[" +
                                                    string.Format("{0:D2}:{1:D2}:{2:D2}", time.Hours, time.Minutes,
                                                        time.Seconds) + "]";
                                    channelStates.Invoke(Datacontext.Channels.Email.ToString(), _dataContext.EmailStateImageSource, _dataContext.htMediaCurrentState[Datacontext.Channels.Email].ToString(), System.Windows.Visibility.Visible, timer);
                                }
                                else
                                {
                                    _dataContext.htMediaCurrentState[Datacontext.Channels.Email] = status;
                                    channelStates.Invoke(Datacontext.Channels.Email.ToString(), _dataContext.EmailStateImageSource,
                                        _dataContext.htMediaCurrentState[Datacontext.Channels.Email].ToString(), System.Windows.Visibility.Visible,
                                        "[00:00:00]");
                                    _currentEmailNReadyTime = 0;
                                }
                            }
                        }
                        break;
                }
                //}));
            }
            catch (Exception commonException)
            {
                _logger.Error("InteractionHandler:ChangeEmailStatus():" + commonException);
            }
        }

        /// <summary>
        /// Changes the outbound status.
        /// </summary>
        /// <param name="status">The status.</param>
        public void ChangeOutboundStatus(string status)
        {
            try
            {
                System.Windows.Application.Current.Dispatcher.Invoke((Action)(delegate
                {
                    if (!_dataContext.htMediaCurrentState.ContainsKey(Datacontext.Channels.OutboundPreview)) return;
                    switch (status)
                    {
                        case "Logout":
                            if (_lstOutboundState.Count > 0 && _lstOutboundState.Contains("Logout"))
                            {
                                _lstOutboundState.Clear();
                                _currentOutboundLogoutTime = _currentOutboundLogoutTime + 1;
                            }
                            else
                            {
                                _dataContext.OutboundStateImageSource = new BitmapImage(new Uri(_emailImagePath.Replace("\\Email", "") + "\\Logout-state.png", UriKind.Relative));
                                if (_dataContext.htMediaCurrentState[Datacontext.Channels.OutboundPreview].ToString() == status)
                                {
                                    TimeSpan time = TimeSpan.FromSeconds(_currentOutboundLogoutTime);
                                    string timer = "[" +
                                                    string.Format("{0:D2}:{1:D2}:{2:D2}", time.Hours, time.Minutes,
                                                        time.Seconds) + "]";
                                    channelStates.Invoke(Datacontext.Channels.OutboundPreview.ToString(), _dataContext.OutboundStateImageSource, status, System.Windows.Visibility.Visible, timer);
                                }
                                else
                                {
                                    _dataContext.htMediaCurrentState[Datacontext.Channels.OutboundPreview] = status;
                                    channelStates.Invoke(Datacontext.Channels.OutboundPreview.ToString(), _dataContext.OutboundStateImageSource, status, System.Windows.Visibility.Visible, "[00:00:00]");
                                    _currentOutboundLogoutTime = 0;
                                }
                            }
                            break;
                        case "Logout (Do Not Disturb)":
                            {
                                _dataContext.OutboundStateImageSource = new BitmapImage(new Uri(_emailImagePath.Replace("\\Email", "") + "\\Logout-state.png", UriKind.Relative));
                                if (_lstOutboundState.Count > 0 && _lstOutboundState.Contains("Logout (Do Not Disturb)"))
                                {
                                    _lstOutboundState.Clear();
                                    _currentOutboundLogoutTime = _currentOutboundLogoutTime + 1;
                                }
                                else
                                {
                                    _dataContext.htMediaCurrentState[Datacontext.Channels.OutboundPreview] = "Logout (Do Not Disturb)";
                                    channelStates.Invoke(Datacontext.Channels.OutboundPreview.ToString(), _dataContext.OutboundStateImageSource, _dataContext.htMediaCurrentState[Datacontext.Channels.OutboundPreview].ToString(), System.Windows.Visibility.Visible, "[00:00:00]");
                                    _currentOutboundLogoutTime = 0;
                                }
                                break;
                            }
                        case "Ready":
                            if (_lstOutboundState.Count > 0 && _lstOutboundState.Contains("Ready"))
                            {
                                _lstOutboundState.Clear();
                                _currentOutboundReadyTime = _currentOutboundReadyTime + 1;
                            }
                            else
                            {
                                _dataContext.OutboundStateImageSource = new BitmapImage(new Uri("/Agent.Interaction.Desktop;component/Images/Ready.png", UriKind.Relative));
                                if (_dataContext.htMediaCurrentState[Datacontext.Channels.OutboundPreview].ToString() == status)
                                {
                                    TimeSpan time = TimeSpan.FromSeconds(_currentOutboundReadyTime);
                                    string timer = "[" +
                                                    string.Format("{0:D2}:{1:D2}:{2:D2}", time.Hours, time.Minutes,
                                                        time.Seconds) + "]";
                                    channelStates.Invoke(Datacontext.Channels.OutboundPreview.ToString(), _dataContext.OutboundStateImageSource, status, System.Windows.Visibility.Visible, timer);
                                }
                                else
                                {
                                    _dataContext.htMediaCurrentState[Datacontext.Channels.OutboundPreview] = status;
                                    channelStates.Invoke(Datacontext.Channels.OutboundPreview.ToString(), _dataContext.OutboundStateImageSource, status, System.Windows.Visibility.Visible, "[00:00:00]");
                                    _currentOutboundReadyTime = 0;
                                }
                            }
                            break;
                        case "Not Ready":
                            if (_lstOutboundState.Count > 0 && _lstOutboundState.Contains("Not Ready"))
                            {
                                _lstOutboundState.Clear();
                                _currentOutboundNReadyTime = _currentOutboundNReadyTime + 1;
                            }
                            else
                            {
                                _dataContext.OutboundStateImageSource = new BitmapImage(new Uri("/Agent.Interaction.Desktop;component/Images/Not_Ready.png", UriKind.Relative));
                                if (_dataContext.htMediaCurrentState[Datacontext.Channels.OutboundPreview].ToString() == status)
                                {
                                    TimeSpan time = TimeSpan.FromSeconds(_currentOutboundNReadyTime);
                                    string timer = "[" +
                                                    string.Format("{0:D2}:{1:D2}:{2:D2}", time.Hours, time.Minutes,
                                                        time.Seconds) + "]";
                                    channelStates.Invoke(Datacontext.Channels.OutboundPreview.ToString(), _dataContext.OutboundStateImageSource, status, System.Windows.Visibility.Visible, timer);
                                }
                                else
                                {
                                    _dataContext.htMediaCurrentState[Datacontext.Channels.OutboundPreview] = status;
                                    channelStates.Invoke(Datacontext.Channels.OutboundPreview.ToString(), _dataContext.OutboundStateImageSource, status, System.Windows.Visibility.Visible, "[00:00:00]");
                                    _currentOutboundNReadyTime = 0;
                                }
                            }
                            break;
                        case "Do Not Disturb":
                            if (_lstOutboundState.Count > 0 && _lstOutboundState.Contains("Do Not Disturb"))
                            {
                                _lstOutboundState.Clear();
                                _currentOutboundDNDOnTime = _currentOutboundDNDOnTime + 1;
                            }
                            else
                            {
                                _dataContext.OutboundStateImageSource =
                                    new BitmapImage(new Uri("/Agent.Interaction.Desktop;component/Images/DontDis.png",
                                        UriKind.Relative));
                                _dataContext.htMediaCurrentState[Datacontext.Channels.OutboundPreview] = status;
                                channelStates.Invoke(Datacontext.Channels.OutboundPreview.ToString(),
                                    _dataContext.OutboundStateImageSource, status,
                                    Visibility.Visible, "[00:00:00]");
                                _currentOutboundDNDOnTime = 0;
                            }
                            break;
                        case "Log On":
                            if (_lstOutboundState.Count > 0 && _lstOutboundState.Contains("Log On"))
                            {
                                _lstOutboundState.Clear();
                                _currentOutboundNReadyTime = _currentOutboundNReadyTime + 1;
                            }
                            else
                            {
                                if (_dataContext.htMediaCurrentState[Datacontext.Channels.OutboundPreview].ToString() == "Not Ready")
                                {
                                    _dataContext.OutboundStateImageSource = new BitmapImage(new Uri("/Agent.Interaction.Desktop;component/Images/Not_Ready.png", UriKind.Relative));
                                    channelStates.Invoke(Datacontext.Channels.OutboundPreview.ToString(), _dataContext.OutboundStateImageSource, _dataContext.htMediaCurrentState[Datacontext.Channels.OutboundPreview].ToString(), System.Windows.Visibility.Visible, "[00:00:00]");
                                }
                                else
                                {
                                    _dataContext.htMediaCurrentState[Datacontext.Channels.OutboundPreview] = "Not Ready";
                                    _dataContext.OutboundStateImageSource = new BitmapImage(new Uri("/Agent.Interaction.Desktop;component/Images/Not_Ready.png", UriKind.Relative));
                                    channelStates.Invoke(Datacontext.Channels.OutboundPreview.ToString(), _dataContext.OutboundStateImageSource, _dataContext.htMediaCurrentState[Datacontext.Channels.OutboundPreview].ToString(), System.Windows.Visibility.Visible, "[00:00:00]");
                                    _currentOutboundNReadyTime = 0;
                                }
                            }
                            break;
                        case "On Interaction":
                            if (_dataContext.htMediaCurrentState[Datacontext.Channels.OutboundPreview].ToString().Contains("[Pending]"))
                                _dataContext.htMediaCurrentState[Datacontext.Channels.OutboundPreview] = _dataContext.htMediaCurrentState[Datacontext.Channels.OutboundPreview].ToString();
                            else
                                _dataContext.htMediaCurrentState[Datacontext.Channels.OutboundPreview] = _dataContext.htMediaCurrentState[Datacontext.Channels.OutboundPreview].ToString() +
                                                                        "  [Pending]";
                            channelStates.Invoke(Datacontext.Channels.OutboundPreview.ToString(),
                                _dataContext.OutboundStateImageSource, _dataContext.htMediaCurrentState[Datacontext.Channels.OutboundPreview].ToString(),
                                Visibility.Collapsed, "[00:00:00]");
                            break;
                        case "Out Of Service":
                            if (_lstOutboundState.Count > 0 && _lstOutboundState.Contains("Out Of Service"))
                            {
                                _lstOutboundState.Clear();
                                _outOfServiceTime = _outOfServiceTime + 1;
                                _serverDownTime = _serverDownTime + 1;
                                if (_serverDownTime >= 5 && _isIXNServerDown)
                                {
                                    _serverDownTime = 0;
                                    if (!((string)_configContainer.GetValue("email.enable.plugin")).ToLower().Equals("true") &&
                                        !_dataContext.IsEmailPluginAdded &&
                                        !((string)_configContainer.GetValue("chat.enable.plugin")).ToLower().Equals("true") &&
                                        !_dataContext.IsChatPluginAdded)
                                    {
                                        //below code used to background process
                                        EventNeedToConnectServer.BeginInvoke("Interaction", null, null);
                                    }

                                }
                            }
                            else
                            {
                                _dataContext.htMediaCurrentState[Datacontext.Channels.OutboundPreview] = status;
                                _dataContext.OutboundStateImageSource =
                                    new BitmapImage(new Uri(
                                        "/Agent.Interaction.Desktop;component/Images/Out_of_Service.png", UriKind.Relative));
                                channelStates.Invoke(Datacontext.Channels.OutboundPreview.ToString(), _dataContext.OutboundStateImageSource, status,
                                    Visibility.Visible, "[00:00:00]");
                                _outOfServiceTime = 0;
                                _serverDownTime = 0;
                            }
                            break;
                        case "Out Of Service1":
                            if (_lstOutboundState.Count > 0 && _lstOutboundState.Contains("Out Of Service1"))
                            {
                                _lstOutboundState.Clear();
                                _outOfServiceTime = _outOfServiceTime + 1;
                            }
                            else
                            {
                                _dataContext.htMediaCurrentState[Datacontext.Channels.OutboundPreview] = "Out Of Service";
                                _dataContext.OutboundStateImageSource =
                                    new BitmapImage(new Uri(
                                        "/Agent.Interaction.Desktop;component/Images/Out_of_Service.png", UriKind.Relative));
                                channelStates.Invoke(Datacontext.Channels.OutboundPreview.ToString(), _dataContext.OutboundStateImageSource, _dataContext.htMediaCurrentState[Datacontext.Channels.OutboundPreview].ToString(),
                                    Visibility.Visible, "[00:00:00]");
                                _outOfServiceTime = 0;
                            }
                            break;
                        default:
                            if (status.Contains("Not Ready -"))
                            {
                                if (_lstOutboundState.Count > 0 && _lstOutboundState[0].Contains("Not Ready -"))
                                {
                                    _lstOutboundState.Clear();
                                    _currentOutboundNReadyTime = _currentOutboundNReadyTime + 1;
                                }
                                else
                                {
                                    _dataContext.OutboundStateImageSource = new BitmapImage(new Uri("/Agent.Interaction.Desktop;component/Images/Not_Ready.png", UriKind.Relative));
                                    if (_dataContext.htMediaCurrentState[Datacontext.Channels.OutboundPreview].ToString() == status)
                                    {
                                        TimeSpan time = TimeSpan.FromSeconds(_currentOutboundNReadyTime);
                                        string timer = "[" +
                                                        string.Format("{0:D2}:{1:D2}:{2:D2}", time.Hours, time.Minutes,
                                                            time.Seconds) + "]";
                                        channelStates.Invoke(Datacontext.Channels.OutboundPreview.ToString(), _dataContext.OutboundStateImageSource, status, System.Windows.Visibility.Visible, timer);
                                    }
                                    else
                                    {
                                        _dataContext.htMediaCurrentState[Datacontext.Channels.OutboundPreview] = status;
                                        channelStates.Invoke(Datacontext.Channels.OutboundPreview.ToString(), _dataContext.OutboundStateImageSource, status, System.Windows.Visibility.Visible, "[00:00:00]");
                                        _currentOutboundNReadyTime = 0;
                                    }
                                }
                            }
                            else if (status.Contains(" [Pending]"))
                            {
                                if (status.Contains("Not Ready -"))
                                {
                                    if (_lstOutboundState.Count > 0 && _lstOutboundState[0].Contains("Not Ready -"))
                                    {
                                        _lstOutboundState.Clear();
                                        _currentOutboundNReadyTime = _currentOutboundNReadyTime + 1;
                                    }
                                    else
                                    {
                                        _dataContext.htMediaCurrentState[Datacontext.Channels.OutboundPreview] = status;
                                        _dataContext.OutboundStateImageSource = new BitmapImage(new Uri("/Agent.Interaction.Desktop;component/Images/Not_Ready.png", UriKind.Relative));
                                        channelStates.Invoke(Datacontext.Channels.OutboundPreview.ToString(), _dataContext.OutboundStateImageSource, status, System.Windows.Visibility.Collapsed, "[00:00:00]");
                                        _currentOutboundNReadyTime = 0;
                                    }
                                }
                                else if (status.Contains("Not Ready"))
                                {
                                    if (_lstOutboundState.Count > 0 && _lstOutboundState.Contains("Not Ready"))
                                    {
                                        _lstOutboundState.Clear();
                                        _currentOutboundNReadyTime = _currentOutboundNReadyTime + 1;
                                    }
                                    else
                                    {
                                        _dataContext.htMediaCurrentState[Datacontext.Channels.OutboundPreview] = "Not Ready";
                                        _dataContext.OutboundStateImageSource = new BitmapImage(new Uri("/Agent.Interaction.Desktop;component/Images/Not_Ready.png", UriKind.Relative));
                                        channelStates.Invoke(Datacontext.Channels.OutboundPreview.ToString(), _dataContext.OutboundStateImageSource, _dataContext.htMediaCurrentState[Datacontext.Channels.OutboundPreview].ToString(), System.Windows.Visibility.Collapsed, "[00:00:00]");
                                        _currentOutboundNReadyTime = 0;
                                    }
                                }
                                else
                                {
                                    if (_lstOutboundState.Count > 0 && _lstOutboundState.Contains("Ready"))
                                    {
                                        _lstOutboundState.Clear();
                                        _currentOutboundReadyTime = _currentOutboundReadyTime + 1;
                                    }
                                    else
                                    {
                                        _dataContext.htMediaCurrentState[Datacontext.Channels.OutboundPreview] = status;
                                        _dataContext.OutboundStateImageSource = new BitmapImage(new Uri("/Agent.Interaction.Desktop;component/Images/Ready.png", UriKind.Relative));
                                        channelStates.Invoke(Datacontext.Channels.OutboundPreview.ToString(), _dataContext.OutboundStateImageSource, _dataContext.htMediaCurrentState[Datacontext.Channels.OutboundPreview].ToString(), System.Windows.Visibility.Collapsed, "[00:00:00]");
                                        _currentOutboundReadyTime = 0;
                                    }
                                }
                            }
                            break;
                    }
                }));
            }
            catch (Exception commonException)
            {
                _logger.Error("InteractionHandler:ChangeOutboundStatus():" + commonException.ToString());
            }
        }

        public void InitialloadInteractionMediaData()
        {
            try
            {
                #region Media Email
                if (_dataContext.htMediaCurrentState.ContainsKey(Datacontext.Channels.Email))
                {
                    if (_dataContext.htMediaCurrentState[Datacontext.Channels.Email].ToString() == "Ready" ||
                        _dataContext.htMediaCurrentState[Datacontext.Channels.Email].ToString() == "Not Ready" ||
                        _dataContext.htMediaCurrentState[Datacontext.Channels.Email].ToString().Contains("Not Ready -") ||
                        _dataContext.htMediaCurrentState[Datacontext.Channels.Email].ToString() == "Do Not Disturb" ||
                        _dataContext.htMediaCurrentState[Datacontext.Channels.Email].ToString() == "Logout" ||
                        _dataContext.htMediaCurrentState[Datacontext.Channels.Email].ToString() == "Out Of Service")
                    {
                        if (!_dataContext.IsOnEmailIXN && !_dataContext.isIXNDND)
                        {
                            ImageSource channelIcon =
                                new BitmapImage(new Uri("/Agent.Interaction.Desktop;component/Images/Email.png",
                                    UriKind.Relative));
                            var time = new TimeSpan();
                            if (_dataContext.htMediaCurrentState[Datacontext.Channels.Email].ToString() == "Ready")
                                time = TimeSpan.FromSeconds(_currentEmailReadyTime + 1);
                            if (_dataContext.htMediaCurrentState[Datacontext.Channels.Email].ToString() == "Not Ready" || _dataContext.htMediaCurrentState[Datacontext.Channels.Email].ToString().Contains("Not Ready -"))
                                time = TimeSpan.FromSeconds(_currentEmailNReadyTime + 1);
                            if (_dataContext.htMediaCurrentState[Datacontext.Channels.Email].ToString() == "Logout")
                                time = TimeSpan.FromSeconds(_currentEmailLogoutTime + 1);
                            if (_dataContext.htMediaCurrentState[Datacontext.Channels.Email].ToString() == "Out Of Service")
                                time = TimeSpan.FromSeconds(_outOfServiceTime + 1);
                            if (_dataContext.htMediaCurrentState[Datacontext.Channels.Email].ToString() == "Do Not Disturb")
                                time = TimeSpan.FromSeconds(_currentEmailDNDOnTime + 1);

                            string timer = "[" + string.Format("{0:D2}:{1:D2}:{2:D2}", time.Hours, time.Minutes, time.Seconds) + "]";
                            if (_dataContext.MediaStatus.Any(p => p.ChannelName == "Email"))
                            {
                                int i =
                                    Datacontext.GetInstance()
                                        .MediaStatus.IndexOf(
                                            Datacontext.GetInstance()
                                                .MediaStatus.Where(p => p.ChannelName == "Email")
                                                .FirstOrDefault());
                                _dataContext.MediaStatus.RemoveAt(i);
                                Datacontext.GetInstance()
                                    .MediaStatus.Insert(i,
                                        (new MediaStatus(channelIcon, "Email",
                                            _dataContext.EmailStateImageSource,
                                            _dataContext.htMediaCurrentState[Datacontext.Channels.Email].ToString(), timer, string.Empty, string.Empty,
                                            Visibility.Visible)));
                                channelIcon = null;
                            }
                            else
                            {
                                Datacontext.GetInstance()
                                    .MediaStatus.Add(
                                        new MediaStatus(channelIcon, "Email",
                                            _dataContext.EmailStateImageSource,
                                            _dataContext.htMediaCurrentState[Datacontext.Channels.Email].ToString(), timer, string.Empty, string.Empty,
                                            Visibility.Visible));
                                channelIcon = null;
                            }

                        }
                        else if (_dataContext.isIXNDND)
                        {
                            ImageSource channelIcon =
                                new BitmapImage(new Uri("/Agent.Interaction.Desktop;component/Images/Email.png",
                                    UriKind.Relative));
                            var _visibility = Visibility.Visible;
                            if (_dataContext.IsOnEmailIXN)
                                _visibility = Visibility.Collapsed;
                            var time = new TimeSpan();
                            time = TimeSpan.FromSeconds(_currentEmailDNDOnTime + 1);
                            string timer = "[" + string.Format("{0:D2}:{1:D2}:{2:D2}", time.Hours, time.Minutes, time.Seconds) + "]";
                            if (_dataContext.MediaStatus.Any(p => p.ChannelName == "Email"))
                            {
                                int i =
                                   Datacontext.GetInstance()
                                       .MediaStatus.IndexOf(
                                           Datacontext.GetInstance()
                                               .MediaStatus.Where(p => p.ChannelName == "Email")
                                               .FirstOrDefault());
                                _dataContext.MediaStatus.RemoveAt(i);
                                Datacontext.GetInstance()
                                    .MediaStatus.Insert(i,
                                        (new MediaStatus(channelIcon, "Email",
                                            _dataContext.EmailStateImageSource,
                                            "Do Not Disturb", timer, string.Empty, string.Empty,
                                            _visibility)));
                                channelIcon = null;
                            }
                            else
                            {
                                Datacontext.GetInstance()
                                    .MediaStatus.Add(
                                        new MediaStatus(channelIcon, "Email",
                                            _dataContext.EmailStateImageSource,
                                            "Do Not Disturb", timer, string.Empty, string.Empty,
                                             _visibility));
                                channelIcon = null;
                            }
                        }
                    }
                }
                #endregion Media Email

                #region Media Chat
                if (_dataContext.htMediaCurrentState.ContainsKey(Datacontext.Channels.Chat))
                {
                    if (_dataContext.htMediaCurrentState[Datacontext.Channels.Chat].ToString() == "Ready" ||
                        _dataContext.htMediaCurrentState[Datacontext.Channels.Chat].ToString() == "Not Ready" ||
                        _dataContext.htMediaCurrentState[Datacontext.Channels.Chat].ToString().Contains("Not Ready -") ||
                           _dataContext.htMediaCurrentState[Datacontext.Channels.Chat].ToString() == "Do Not Disturb" ||
                        _dataContext.htMediaCurrentState[Datacontext.Channels.Chat].ToString() == "Logout")
                    {
                        if (!_dataContext.IsOnChatIXN && !_dataContext.isIXNDND)
                        {
                            ImageSource channelIcon =
                                new BitmapImage(new Uri("/Agent.Interaction.Desktop;component/Images/Chat.png",
                                    UriKind.Relative));
                            var time = new TimeSpan();
                            if (_dataContext.htMediaCurrentState[Datacontext.Channels.Chat].ToString() == "Ready")
                                time = TimeSpan.FromSeconds(_currentChatReadyTime + 1);
                            if (_dataContext.htMediaCurrentState[Datacontext.Channels.Chat].ToString() == "Not Ready" || _dataContext.htMediaCurrentState[Datacontext.Channels.Chat].ToString().Contains("Not Ready -"))
                                time = TimeSpan.FromSeconds(_currentChatNReadyTime + 1);
                            if (_dataContext.htMediaCurrentState[Datacontext.Channels.Chat].ToString() == "Logout")
                                time = TimeSpan.FromSeconds(_currentChatLogoutTime + 1);

                            string timer = "[" + string.Format("{0:D2}:{1:D2}:{2:D2}", time.Hours, time.Minutes, time.Seconds) + "]";
                            if (_dataContext.MediaStatus.Any(p => p.ChannelName == "Chat"))
                            {
                                int i =
                                    Datacontext.GetInstance()
                                        .MediaStatus.IndexOf(
                                            Datacontext.GetInstance()
                                                .MediaStatus.Where(p => p.ChannelName == "Chat")
                                                .FirstOrDefault());
                                _dataContext.MediaStatus.RemoveAt(i);
                                Datacontext.GetInstance()
                                    .MediaStatus.Insert(i,
                                        (new MediaStatus(channelIcon, "Chat",
                                            _dataContext.ChatStateImageSource,
                                            _dataContext.htMediaCurrentState[Datacontext.Channels.Chat].ToString(), timer, string.Empty, string.Empty,
                                            (_dataContext.IsOnChatIXN ? Visibility.Collapsed : Visibility.Visible))));
                                channelIcon = null;
                            }
                            else
                            {
                                Datacontext.GetInstance()
                                    .MediaStatus.Add(
                                        new MediaStatus(channelIcon, "Chat",
                                            _dataContext.ChatStateImageSource,
                                            _dataContext.htMediaCurrentState[Datacontext.Channels.Chat].ToString(), timer, string.Empty, string.Empty,
                                            (_dataContext.IsOnChatIXN ? Visibility.Collapsed : Visibility.Visible)));
                                channelIcon = null;
                            }
                        }
                        else if (_dataContext.isIXNDND)
                        {
                            ImageSource channelIcon =
                                new BitmapImage(new Uri("/Agent.Interaction.Desktop;component/Images/Chat.png",
                                    UriKind.Relative));
                            var _visibility = Visibility.Visible;
                            if (_dataContext.IsOnChatIXN)
                                _visibility = Visibility.Collapsed;
                            var time = new TimeSpan();
                            time = TimeSpan.FromSeconds(_currentChatDNDOnTime + 1);
                            string timer = "[" + string.Format("{0:D2}:{1:D2}:{2:D2}", time.Hours, time.Minutes, time.Seconds) + "]";
                            if (_dataContext.MediaStatus.Any(p => p.ChannelName == "Chat"))
                            {
                                int i =
                                   Datacontext.GetInstance()
                                       .MediaStatus.IndexOf(
                                           Datacontext.GetInstance()
                                               .MediaStatus.Where(p => p.ChannelName == "Chat")
                                               .FirstOrDefault());
                                _dataContext.MediaStatus.RemoveAt(i);
                                Datacontext.GetInstance()
                                    .MediaStatus.Insert(i,
                                        (new MediaStatus(channelIcon, "Chat",
                                            _dataContext.ChatStateImageSource,
                                            "Do Not Disturb", timer, string.Empty, string.Empty,
                                            _visibility)));
                                channelIcon = null;
                            }
                            else
                            {
                                Datacontext.GetInstance()
                                    .MediaStatus.Add(
                                        new MediaStatus(channelIcon, "Chat",
                                            _dataContext.ChatStateImageSource,
                                            "Do Not Disturb", timer, string.Empty, string.Empty,
                                             _visibility));
                                channelIcon = null;
                            }
                        }
                    }
                }
                #endregion Media Chat

                #region Media Outbound
                if (_dataContext.htMediaCurrentState.ContainsKey(Datacontext.Channels.OutboundPreview))
                {
                    if (_dataContext.htMediaCurrentState[Datacontext.Channels.OutboundPreview].ToString() == "Ready" ||
                        _dataContext.htMediaCurrentState[Datacontext.Channels.OutboundPreview].ToString() == "Not Ready" ||
                        _dataContext.htMediaCurrentState[Datacontext.Channels.OutboundPreview].ToString().Contains("Not Ready -") ||
                        _dataContext.htMediaCurrentState[Datacontext.Channels.OutboundPreview].ToString() == "Logout")
                    {
                        ImageSource channelIcon =
                            new BitmapImage(new Uri("/Agent.Interaction.Desktop;component/Images/Outbound.png",
                                UriKind.Relative));
                        var time = new TimeSpan();
                        if (_dataContext.htMediaCurrentState[Datacontext.Channels.OutboundPreview].ToString() == "Ready")
                            time = TimeSpan.FromSeconds(_currentOutboundReadyTime + 1);
                        if (_dataContext.htMediaCurrentState[Datacontext.Channels.OutboundPreview].ToString() == "Not Ready" || _dataContext.htMediaCurrentState[Datacontext.Channels.OutboundPreview].ToString().Contains("Not Ready -"))
                            time = TimeSpan.FromSeconds(_currentOutboundNReadyTime + 1);
                        if (_dataContext.htMediaCurrentState[Datacontext.Channels.OutboundPreview].ToString() == "Logout")
                            time = TimeSpan.FromSeconds(_currentOutboundLogoutTime + 1);

                        string timer = "[" + string.Format("{0:D2}:{1:D2}:{2:D2}", time.Hours, time.Minutes, time.Seconds) + "]";
                        if (_dataContext.MediaStatus.Any(p => p.ChannelName == "OutboundPreview"))
                        {
                            int i =
                                Datacontext.GetInstance()
                                    .MediaStatus.IndexOf(
                                        Datacontext.GetInstance()
                                            .MediaStatus.Where(p => p.ChannelName == "OutboundPreview")
                                            .FirstOrDefault());
                            _dataContext.MediaStatus.RemoveAt(i);
                            Datacontext.GetInstance()
                                .MediaStatus.Insert(i,
                                    (new MediaStatus(channelIcon, "OutboundPreview",
                                        _dataContext.OutboundStateImageSource,
                                        _dataContext.htMediaCurrentState[Datacontext.Channels.OutboundPreview].ToString(), timer, string.Empty, string.Empty,
                                        (_dataContext.htMediaCurrentState[Datacontext.Channels.OutboundPreview].ToString().Contains("[Pending]") ? Visibility.Collapsed : Visibility.Visible))));
                            channelIcon = null;
                        }
                        else
                        {
                            Datacontext.GetInstance()
                                .MediaStatus.Add(
                                    new MediaStatus(channelIcon, "OutboundPreview",
                                        _dataContext.OutboundStateImageSource,
                                        _dataContext.htMediaCurrentState[Datacontext.Channels.OutboundPreview].ToString(), timer, string.Empty, string.Empty,
                                        (_dataContext.htMediaCurrentState[Datacontext.Channels.OutboundPreview].ToString().Contains("[Pending]") ? Visibility.Collapsed : Visibility.Visible)));
                            channelIcon = null;
                        }
                    }
                }
                #endregion Media Outbound
            }

            catch (Exception commonException)
            {
                _logger.Error("InteractionHandler " + commonException);
            }
        }

        public void NotifyContactProtocol(Genesyslab.Platform.Contacts.Protocols.UniversalContactServerProtocol ucsProtocol)
        {
            _dataContext.ContactServerProtocol = ucsProtocol;
        }

        /// <summary>
        /// Notifies the decimal service interaction.
        /// </summary>
        /// <param name="message">The message.</param>
        /// <param name="type">The type.</param>
        public void NotifyEServiceInteraction(IMessage message, InteractionTypes type)
        {
            //_ixnAgentStatusListener.Invoke(message, type);
            Interaction_Listener(message, type);
        }

        /// <summary>
        /// This method will provide interaction protocol
        /// </summary>
        /// <param name="interactionProtocol"></param>
        public void NotifyInteractionProtocol(InteractionServerProtocol interactionProtocol)
        {
            _dataContext.InteractionProtocol = interactionProtocol;
        }

        public void NotifyInteractionServerStatus(IXNServerState state)
        {
            if (state == IXNServerState.Opened)
            {
                InteractionService.NotifyInteractionServerState(true);
                EventNeedToConnectServer.BeginInvoke("IXNServerOpened", null, null);
            }
            if (state == IXNServerState.Closed)
            {
                InteractionService.NotifyInteractionServerState(false);
                EventNeedToConnectServer.BeginInvoke("IXNServerClosed", null, null);
            }
        }

        /// <summary>
        /// Notifies the ixn agent media status.
        /// </summary>
        /// <param name="agentMediaStatus"></param>
        public void NotifyIxnAgentMediaStatus(IMessage message)
        {
            try
            {
                switch (message.Id)
                {
                    case Genesyslab.Platform.OpenMedia.Protocols.InteractionServer.Events.EventForcedAgentStateChange.MessageId:
                        #region Event Forced Agent State Change
                        Genesyslab.Platform.OpenMedia.Protocols.InteractionServer.Events.EventForcedAgentStateChange eventForcedAgentStateChange = (Genesyslab.Platform.OpenMedia.Protocols.InteractionServer.Events.EventForcedAgentStateChange)message;
                        if (eventForcedAgentStateChange != null)
                        {
                            // eventForcedAgentStateChange.AgentStateChangeOperation == Genesyslab.Platform.OpenMedia.Protocols.InteractionServer.AgentStateChangeOperation.ReadyForMedia
                            if (eventForcedAgentStateChange.MediaTypeName == "none" && eventForcedAgentStateChange.MediaList == null && !eventForcedAgentStateChange.DonotDisturb)
                                return;
                            if (eventForcedAgentStateChange.MediaTypeName.ToString().ToLower().Equals("email") &&
                                _plugins.PluginCollections.ContainsKey(Pointel.Interactions.IPlugins.Plugins.Email) &&
                                _dataContext.htMediaCurrentState[Datacontext.Channels.Email].ToString() != "Logout")
                            {
                                string state = eventForcedAgentStateChange.MediaList[eventForcedAgentStateChange.MediaTypeName].ToString();
                                if (state == "1")
                                {
                                    AgentEmailState = "Ready";
                                    _dataContext.EmailStateImageSource = _imageDataContext.ImgReadyStatus;
                                    if (_dataContext.IsOnEmailIXN)
                                        ChangeEmailStatus("On Interaction");
                                    else
                                        ChangeEmailStatus("Ready");
                                }
                                else if (state == "0")
                                {
                                    AgentEmailState = "Not Ready";
                                    _dataContext.EmailStateImageSource = _imageDataContext.ImgNotReadyStatus;
                                    if (_dataContext.IsOnEmailIXN)
                                        ChangeEmailStatus("On Interaction");
                                    else
                                        ChangeEmailStatus("Not Ready");
                                }
                            }
                            else
                            {
                                if (_plugins.PluginCollections.ContainsKey(Pointel.Interactions.IPlugins.Plugins.Email) && _dataContext.htMediaCurrentState[Datacontext.Channels.Email].ToString().Equals("Do Not Disturb"))
                                {
                                    string state = eventForcedAgentStateChange.MediaList["email"].ToString();
                                    if (AgentEmailState != "Logout")
                                        if (state == "1")
                                        {
                                            _dataContext.EmailStateImageSource = _imageDataContext.ImgReadyStatus;
                                            AgentEmailState = "Ready";
                                            if (_dataContext.IsOnChatIXN)
                                                ChangeEmailStatus("On Interaction");
                                            else
                                                ChangeEmailStatus("Ready");
                                        }
                                        else if (state == "0")
                                        {
                                            _dataContext.EmailStateImageSource = _imageDataContext.ImgNotReadyStatus;
                                            AgentEmailState = "Not Ready";
                                            if (_dataContext.IsOnEmailIXN)
                                                ChangeEmailStatus("On Interaction");
                                            else
                                                ChangeEmailStatus("Not Ready");
                                        }
                                }
                            }
                            if (eventForcedAgentStateChange.MediaTypeName.ToString().ToLower().Equals("chat") &&
                                _plugins.PluginCollections.ContainsKey(Pointel.Interactions.IPlugins.Plugins.Chat) &&
                                _dataContext.htMediaCurrentState[Datacontext.Channels.Chat].ToString() != "Logout")
                            {
                                string state = eventForcedAgentStateChange.MediaList[eventForcedAgentStateChange.MediaTypeName].ToString();
                                if (state == "1")
                                {
                                    AgentChatState = "Ready";
                                    _dataContext.ChatStateImageSource = _imageDataContext.ImgReadyStatus;
                                    if (_dataContext.IsOnChatIXN)
                                        ChangeChatStatus("On Interaction");
                                    else
                                        ChangeChatStatus("Ready");
                                }
                                else if (state == "0")
                                {
                                    AgentChatState = "Not Ready";
                                    _dataContext.ChatStateImageSource = _imageDataContext.ImgNotReadyStatus;
                                    if (_dataContext.IsOnChatIXN)
                                        ChangeChatStatus("On Interaction");
                                    else
                                        ChangeChatStatus("Not Ready");
                                }
                            }
                            else
                            {
                                if (_plugins.PluginCollections.ContainsKey(Pointel.Interactions.IPlugins.Plugins.Chat) && _dataContext.htMediaCurrentState[Datacontext.Channels.Chat].ToString().Equals("Do Not Disturb"))
                                {
                                    string state = eventForcedAgentStateChange.MediaList["chat"].ToString();
                                    if (state == "1")
                                    {
                                        AgentChatState = "Ready";
                                        _dataContext.ChatStateImageSource = _imageDataContext.ImgReadyStatus;
                                        if (_dataContext.IsOnChatIXN)
                                            ChangeChatStatus("On Interaction");
                                        else
                                            ChangeChatStatus("Ready");
                                    }
                                    else if (state == "0")
                                    {
                                        AgentChatState = "Not Ready";
                                        _dataContext.ChatStateImageSource = _imageDataContext.ImgNotReadyStatus;
                                        if (_dataContext.IsOnChatIXN)
                                            ChangeChatStatus("On Interaction");
                                        else
                                            ChangeChatStatus("Not Ready");
                                    }
                                }
                            }
                            if (eventForcedAgentStateChange.MediaTypeName.ToString().ToLower().Equals("outboundpreview") &&
                                _plugins.PluginCollections.ContainsKey(Pointel.Interactions.IPlugins.Plugins.OutboundPreview) &&
                                _dataContext.htMediaCurrentState[Datacontext.Channels.Chat].ToString() != "Logout")
                            {
                                string state = eventForcedAgentStateChange.MediaList[eventForcedAgentStateChange.MediaTypeName].ToString();
                                if (state == "1")
                                {
                                    AgentOutboundState = "Ready";
                                    ChangeOutboundStatus("Ready");
                                }
                                else if (state == "0")
                                {
                                    AgentOutboundState = "Not Ready";
                                    ChangeOutboundStatus("Not Ready");
                                }
                            }
                            else
                            {
                                if (_plugins.PluginCollections.ContainsKey(Pointel.Interactions.IPlugins.Plugins.OutboundPreview) && _dataContext.htMediaCurrentState[Datacontext.Channels.OutboundPreview].ToString().Equals("Do Not Disturb"))
                                {
                                    string state = eventForcedAgentStateChange.MediaList["outboundpreview"].ToString();
                                    if (state == "1")
                                    {
                                        AgentOutboundState = "Ready";
                                        ChangeOutboundStatus("Ready");
                                    }
                                    else if (state == "0")
                                    {
                                        AgentOutboundState = "Not Ready";
                                        ChangeOutboundStatus("Not Ready");
                                    }
                                }
                            }

                            _dataContext.isIXNDND = eventForcedAgentStateChange.DonotDisturb;

                            if (_dataContext.isIXNDND)
                            {
                                if (_plugins.PluginCollections.ContainsKey(Pointel.Interactions.IPlugins.Plugins.Email))
                                {
                                    AgentEmailStateBeforeDND = AgentEmailState;
                                    if (!_dataContext.htMediaCurrentState[Datacontext.Channels.Email].ToString().Contains("Logout"))
                                    {
                                        AgentEmailState = "Do Not Disturb";
                                        _dataContext.EmailStateImageSource = _imageDataContext.ImgDNDStatus;
                                        //_dataContext.EmailStateImageSource = GetBitmapImage(new Uri("/Agent.Interaction.Desktop;component/Images/DontDis.png",
                                        //            UriKind.Relative));
                                        ChangeEmailStatus("Do Not Disturb");
                                    }
                                    else
                                    {
                                        AgentEmailState = "Logout (Do Not Disturb)";
                                        _dataContext.EmailStateImageSource = _imageDataContext.ImgLogoutStatus;
                                        //_dataContext.EmailStateImageSource = GetBitmapImage(new Uri("/Agent.Interaction.Desktop;component/Images/Logout-state.png",
                                        //            UriKind.Relative));
                                        ChangeEmailStatus("Logout (Do Not Disturb)");
                                    }
                                }
                                if (_plugins.PluginCollections.ContainsKey(Pointel.Interactions.IPlugins.Plugins.Chat))
                                {
                                    AgentChatStateBeforeDND = AgentChatState;
                                    if (!_dataContext.htMediaCurrentState[Datacontext.Channels.Chat].ToString().Contains("Logout"))
                                    {
                                        AgentChatState = "Do Not Disturb";
                                        _dataContext.ChatStateImageSource = _imageDataContext.ImgDNDStatus;
                                        //_dataContext.ChatStateImageSource = GetBitmapImage(new Uri("/Agent.Interaction.Desktop;component/Images/DontDis.png",
                                        //            UriKind.Relative));
                                        ChangeChatStatus("Do Not Disturb");
                                    }
                                    else
                                    {
                                        AgentChatState = "Logout (Do Not Disturb)";
                                        _dataContext.ChatStateImageSource = _imageDataContext.ImgLogoutStatus;
                                        //_dataContext.ChatStateImageSource = GetBitmapImage(new Uri("/Agent.Interaction.Desktop;component/Images/Logout-state.png",
                                        //            UriKind.Relative));
                                        ChangeChatStatus("Logout (Do Not Disturb)");
                                    }
                                }
                                if (_plugins.PluginCollections.ContainsKey(Pointel.Interactions.IPlugins.Plugins.OutboundPreview))
                                {
                                    if (!_dataContext.htMediaCurrentState[Datacontext.Channels.OutboundPreview].ToString().Contains("Logout"))
                                    {
                                        AgentOutboundState = "Do Not Disturb";
                                        ChangeOutboundStatus("Do Not Disturb");
                                    }
                                    else
                                    {
                                        AgentOutboundState = "Logout (Do Not Disturb)";
                                        ChangeOutboundStatus("Logout (Do Not Disturb)");
                                    }
                                }
                            }
                        }
                        #endregion Event Forced Agent State Change
                        break;
                    case Genesyslab.Platform.OpenMedia.Protocols.InteractionServer.Events.EventCurrentAgentStatus.MessageId:
                        Genesyslab.Platform.OpenMedia.Protocols.InteractionServer.Events.EventCurrentAgentStatus eventCurrentAgentStatus = (Genesyslab.Platform.OpenMedia.Protocols.InteractionServer.Events.EventCurrentAgentStatus)message;
                        if (eventCurrentAgentStatus != null)
                        {
                            string emailState = eventCurrentAgentStatus.MediaStateList[Datacontext.Channels.Email.ToString().ToLower()].ToString();
                            if (emailState == "1")
                            {
                                //Ready
                            }
                            else if (emailState == "0")
                            {
                                //Not Ready
                            }
                            string chatState = eventCurrentAgentStatus.MediaStateList[Datacontext.Channels.Chat.ToString().ToLower()].ToString();
                            if (chatState == "1")
                            {
                                //ready
                            }
                            else if (chatState == "0")
                            {
                                //Not Ready
                            }
                        }
                        #region Event Current Agent Status
                        //var eventCurrentAgentStatus = (Genesyslab.Platform.OpenMedia.Protocols.InteractionServer.Events.EventCurrentAgentStatus)message;
                        //AgentEmailStateBeforeDND = eventCurrentAgentStatus.MediaStateList.Contains(Datacontext.Channels.Email.ToString().ToLower()) ? (eventCurrentAgentStatus.MediaStateList[Datacontext.Channels.Email.ToString().ToLower()].ToString() == "0" ? "Not Ready" : "Ready") : "Logout";
                        //AgentChatStateBeforeDND = eventCurrentAgentStatus.MediaStateList.Contains(Datacontext.Channels.Chat.ToString().ToLower()) ? (eventCurrentAgentStatus.MediaStateList[Datacontext.Channels.Chat.ToString().ToLower()].ToString() == "0" ? "Not Ready" : "Ready") : "Logout";
                        //AgentOutboundStateBeforeDND = eventCurrentAgentStatus.MediaStateList.Contains(Datacontext.Channels.OutboundPreview.ToString().ToLower()) ? (eventCurrentAgentStatus.MediaStateList[Datacontext.Channels.OutboundPreview.ToString().ToLower()].ToString() == "0" ? "Not Ready" : "Ready") : "Logout";
                        #endregion Event Current Agent Status
                        break;
                    case Genesyslab.Platform.OpenMedia.Protocols.InteractionServer.Events.EventError.MessageId:
                        var eventerror = message as Genesyslab.Platform.OpenMedia.Protocols.InteractionServer.Events.EventError;
                        var errorMessage = new KeyValueCollection();
                        errorMessage.Add("IWS_Message", eventerror.ErrorDescription.ToString());
                        errorMessage.Add("IWS_Subject", "");
                        errorMessage.Add("IWS_Sender", "System");
                        errorMessage.Add("IWS_Priority", "4");
                        errorMessage.Add("IWS_MessageType", "System");
                        errorMessage.Add("IWS_Date", DateTime.Now.ToString());
                        errorMessage.Add("IWS_Topic", _dataContext.UserName);
                        // GettingUserData(Pointel.Softphone.Voice.Core.VoiceEvents.None, errorMessage);
                        break;
                }
            }
            catch (Exception generalException)
            {
                _logger.Error("Error occurred while do NotifyIxnAgentMediaStatus() :" + generalException.ToString());
            }
        }

        /// <summary>
        /// Subscribes the specified ixn service.
        /// </summary>
        /// <param name="ixnService">The ixn service.</param>
        /// <param name="ixnType">Type of the ixn.</param>
        public void Subscribe(InteractionService ixnService, InteractionTypes ixnType)
        {
            ixnService.Subscriber(ixnType, ixnListener);
        }

        public void UCSSubscribe(ContactService contactService)
        {
            contactService.Subscriber(contactListener);
        }

        /// <summary>
        /// Softphone bar_chat agent status listener.
        /// </summary>
        /// <param name="message">The message.</param>
        private void Interaction_Listener(IMessage message, InteractionTypes type)
        {
            try
            {
                if (message != null)
                {
                    Application.Current.Dispatcher.Invoke((Action)(delegate
                    {
                        switch (message.Name)
                        {
                            case "EventAgentInvited":
                                if (type == InteractionTypes.Email && _plugins.PluginCollections.ContainsKey(Pointel.Interactions.IPlugins.Plugins.Email))
                                {
                                    ((IEmailPlugin)_plugins.PluginCollections[Pointel.Interactions.IPlugins.Plugins.Email]).NotifyEmailInteraction(message);
                                    _dataContext.IsOnEmailIXN = true;
                                    ChangeEmailStatus("On Interaction");
                                }
                                if (type == InteractionTypes.Chat && _plugins.PluginCollections.ContainsKey(Pointel.Interactions.IPlugins.Plugins.Chat))
                                {
                                    ((IChatPlugin)_plugins.PluginCollections[Pointel.Interactions.IPlugins.Plugins.Chat]).NotifyChatInteraction(message);
                                    _dataContext.IsOnChatIXN = true;
                                    ChangeChatStatus("On Interaction");
                                }
                                break;
                            case "EventInvite":
                                if (type == InteractionTypes.Email && _plugins.PluginCollections.ContainsKey(Pointel.Interactions.IPlugins.Plugins.Email))
                                {
                                    ((IEmailPlugin)_plugins.PluginCollections[Pointel.Interactions.IPlugins.Plugins.Email]).NotifyEmailInteraction(message);
                                    _dataContext.IsOnEmailIXN = true;
                                    ChangeEmailStatus("On Interaction");
                                    //SoftPhoneBar.emailInstanceCount++;
                                }
                                if (type == InteractionTypes.Chat && _plugins.PluginCollections.ContainsKey(Pointel.Interactions.IPlugins.Plugins.Chat))
                                {
                                    ((IChatPlugin)_plugins.PluginCollections[Pointel.Interactions.IPlugins.Plugins.Chat]).NotifyChatInteraction(message);
                                    _dataContext.IsOnChatIXN = true;
                                    ChangeChatStatus("On Interaction");

                                }
                                break;
                            case "EventRevoked":
                                if (type == InteractionTypes.Email && _plugins.PluginCollections.ContainsKey(Pointel.Interactions.IPlugins.Plugins.Email))
                                {
                                    ((IEmailPlugin)_plugins.PluginCollections[Pointel.Interactions.IPlugins.Plugins.Email]).NotifyEmailInteraction(message);

                                    string state = _dataContext.htMediaCurrentState[Datacontext.Channels.Email].ToString();

                                    if (state == "Ready")
                                    {
                                        var interactionService = new InteractionService();
                                        var outputValue = interactionService.AgentNotReady(_dataContext.ProxyID, Datacontext.Channels.Email.ToString().ToLower());
                                        if (outputValue.MessageCode == "200")
                                            AgentEmailState = "Not Ready";
                                        else
                                            AgentEmailState = state;
                                    }
                                    else
                                        AgentEmailState = state;
                                    if (AgentEmailState.Contains("Not Ready"))
                                    {
                                        _dataContext.EmailStateImageSource = _imageDataContext.ImgNotReadyStatus;
                                        //_dataContext.EmailStateImageSource = GetBitmapImage(new Uri("/Agent.Interaction.Desktop;component/Images/Not_Ready.png",
                                        //            UriKind.Relative));
                                    }
                                    else if (AgentEmailState.Contains("Logout"))
                                    {
                                        _dataContext.EmailStateImageSource = _imageDataContext.ImgLogoutStatus;
                                        //_dataContext.EmailStateImageSource = GetBitmapImage(new Uri("/Agent.Interaction.Desktop;component/Images/Logout-state.png",
                                        //            UriKind.Relative));
                                    }
                                    else
                                    {
                                        _dataContext.EmailStateImageSource = _imageDataContext.ImgDNDStatus;
                                        //_dataContext.EmailStateImageSource = GetBitmapImage(new Uri("/Agent.Interaction.Desktop;component/Images/DontDis.png",
                                        //            UriKind.Relative));
                                    }
                                    if (SoftPhoneBar.GetEmailWindowInstanceCount() == 0)
                                    {
                                        _dataContext.IsOnEmailIXN = false;
                                        ChangeEmailStatus(AgentEmailState);
                                    }
                                    else
                                    {
                                        channelStates.Invoke(Datacontext.Channels.Email.ToString(), _dataContext.EmailStateImageSource,
                                                AgentEmailState, System.Windows.Visibility.Collapsed, "[00:00:00]");
                                        _currentEmailNReadyTime = 0;
                                    }
                                    //implemented
                                    //if (SoftPhoneBar.emailInstanceCount > 0)
                                    //    SoftPhoneBar.emailInstanceCount--;
                                }
                                if (type == InteractionTypes.Chat && _plugins.PluginCollections.ContainsKey(Pointel.Interactions.IPlugins.Plugins.Chat))
                                {
                                    ((IChatPlugin)_plugins.PluginCollections[Pointel.Interactions.IPlugins.Plugins.Chat]).NotifyChatInteraction(message);

                                    string state = _dataContext.htMediaCurrentState[Datacontext.Channels.Chat].ToString();

                                    if (state == "Ready")
                                    {
                                        var interactionService = new InteractionService();
                                        var outputValue = interactionService.AgentNotReady(_dataContext.ProxyID, Datacontext.Channels.Chat.ToString().ToLower());
                                        if (outputValue.MessageCode == "200")
                                            AgentChatState = "Not Ready";
                                        else
                                            AgentChatState = state;
                                    }
                                    else
                                        AgentChatState = state;
                                    if (AgentChatState.Contains("Not Ready"))
                                    {
                                        _dataContext.ChatStateImageSource = _imageDataContext.ImgNotReadyStatus;
                                        //_dataContext.ChatStateImageSource = GetBitmapImage(new Uri("/Agent.Interaction.Desktop;component/Images/Not_Ready.png",
                                        //            UriKind.Relative));
                                    }
                                    else if (AgentChatState.Contains("Logout"))
                                    {
                                        _dataContext.ChatStateImageSource = _imageDataContext.ImgLogoutStatus;
                                        //_dataContext.ChatStateImageSource = GetBitmapImage(new Uri("/Agent.Interaction.Desktop;component/Images/Logout-state.png",
                                        //            UriKind.Relative));
                                    }
                                    else
                                    {
                                        _dataContext.ChatStateImageSource = _imageDataContext.ImgDNDStatus;
                                        //_dataContext.ChatStateImageSource = GetBitmapImage(new Uri("/Agent.Interaction.Desktop;component/Images/DontDis.png",
                                        //            UriKind.Relative));
                                    }

                                    if (SoftPhoneBar.GetChatWindowInstanceCount() <= 1)
                                    {
                                        _dataContext.IsOnChatIXN = false;
                                        ChangeChatStatus(AgentChatState);
                                    }
                                    else
                                    {

                                        channelStates.Invoke(Datacontext.Channels.Email.ToString(), _dataContext.EmailStateImageSource,
                                                AgentChatState, System.Windows.Visibility.Collapsed, "[00:00:00]");
                                        _currentEmailNReadyTime = 0;
                                    }
                                }
                                break;

                            #region Not Needed, we cant subcribe tot this event
                            //case "EventRejected":
                            //    if (type == InteractionTypes.Email && _dataContext.IsOnEmailIXN)
                            //    {
                            //        string status = _dataContext.htMediaCurrentState[Datacontext.Channels.Email].ToString();
                            //        if (!status.Contains("Do Not Disturb"))
                            //            AgentEmailState = status;
                            //        if (SoftPhoneBar.GetEmailWindowInstanceCount() <= 1)
                            //        {
                            //            _dataContext.IsOnEmailIXN = false;
                            //            ChangeEmailStatus(AgentEmailState);
                            //        }
                            //        else
                            //            ChangeEmailStatus("On Interaction");

                            //    }
                            //    if (type == InteractionTypes.Chat && _dataContext.IsOnChatIXN)
                            //    {
                            //        string status = _dataContext.htMediaCurrentState[Datacontext.Channels.Chat].ToString();
                            //        if (!status.Contains("Do Not Disturb"))
                            //            AgentChatState = status;
                            //        ChangeChatStatus(status);
                            //    }
                            //    break;
                            #endregion

                            #region Not needed for now
                            //case "EventPulledInteractions":
                            //    if (type == InteractionTypes.Email && _plugins.PluginCollection.ContainsKey(Pointel.Interactions.IPlugins.Plugins.Email))
                            //    {
                            //        ((IEmailPlugin)_plugins.PluginCollection[Pointel.Interactions.IPlugins.Plugins.Email]).NotifyEmailInteraction(message);
                            //        AgentEmailState = "On Interaction";
                            //        ChangeEmailStatus("On Interaction");
                            //        _dataContext.isOnIXN = true;
                            //    }
                            //    if (type == InteractionTypes.Chat && _plugins.PluginCollection.ContainsKey(Pointel.Interactions.IPlugins.Plugins.Chat))
                            //    {
                            //        ((IChatPlugin)_plugins.PluginCollection[Pointel.Interactions.IPlugins.Plugins.Chat]).NotifyChatInteraction(message);
                            //        AgentChatState = "On Interaction";
                            //        ChangeChatStatus("On Interaction");
                            //        _dataContext.isOnIXN = true;
                            //    }
                            //    break;
                            #endregion Not needed for now

                            case "EventPropertiesChanged":
                                if (type == InteractionTypes.Chat && _plugins.PluginCollections.ContainsKey(Pointel.Interactions.IPlugins.Plugins.Chat))
                                {
                                    ((IChatPlugin)_plugins.PluginCollections[Pointel.Interactions.IPlugins.Plugins.Chat]).NotifyChatInteraction(message);
                                }
                                break;
                            case "EventPartyAdded":
                                if (type == InteractionTypes.Chat && _plugins.PluginCollections.ContainsKey(Pointel.Interactions.IPlugins.Plugins.Chat))
                                {
                                    ((IChatPlugin)_plugins.PluginCollections[Pointel.Interactions.IPlugins.Plugins.Chat]).NotifyChatInteraction(message);
                                }
                                break;
                            case "EventPartyRemoved":
                                if (type == InteractionTypes.Chat && _plugins.PluginCollections.ContainsKey(Pointel.Interactions.IPlugins.Plugins.Chat))
                                {
                                    ((IChatPlugin)_plugins.PluginCollections[Pointel.Interactions.IPlugins.Plugins.Chat]).NotifyChatInteraction(message);
                                }
                                break;
                            case "EventWorkbinContentChanged":
                                {
                                    if (type == InteractionTypes.Email && _plugins.PluginCollections.ContainsKey(Pointel.Interactions.IPlugins.Plugins.Workbin))
                                    {
                                        ((IWorkbinPlugin)_plugins.PluginCollections[Pointel.Interactions.IPlugins.Plugins.Workbin]).NotifyWorkbinContentChanged(message);
                                    }
                                }
                                break;

                        }
                    }));
                }
            }
            catch (Exception commonException)
            {
                _logger.Error("InteractionHandler:" + commonException.ToString());
            }
        }

        #endregion Methods
    }
}