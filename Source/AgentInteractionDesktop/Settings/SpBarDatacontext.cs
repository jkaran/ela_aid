namespace Agent.Interaction.Desktop.Settings
{
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using System.Collections.ObjectModel;
    using System.ComponentModel;
    using System.Linq.Expressions;
    using System.Reflection;
    using System.Windows;
    using System.Windows.Controls;
    using System.Windows.Data;
    using System.Windows.Media;
    using System.Windows.Media.Effects;

    using Agent.Interaction.Desktop.Helpers;

    using Genesyslab.Platform.ApplicationBlocks.Commons.Protocols;
    using Genesyslab.Platform.ApplicationBlocks.ConfigurationObjectModel;
    using Genesyslab.Platform.ApplicationBlocks.ConfigurationObjectModel.CfgObjects;
    using Genesyslab.Platform.Commons.Collections;
    using Genesyslab.Platform.Configuration.Protocols;
    using Genesyslab.Platform.Contacts.Protocols;
    using Genesyslab.Platform.OpenMedia.Protocols;

    /// <summary>
    /// Data Context for all windows in AID with NotifyPropertyChange
    /// </summary>
    public class Datacontext : INotifyPropertyChanged
    {
        #region Fields

        public static Dictionary<string, string> AvailableServerDic = new Dictionary<string, string>();
        public static Dictionary<string, string> DNsCollection = new Dictionary<string, string>();
        public static Hashtable hshLoadGroupContact = new Hashtable();
        public static bool isCalledRefinePlace = false;
        public static bool isDialling = false;
        public static bool isLoggerInitialized = false;
        public static bool isRinging = false;

        //public static Dictionary<string, string> loadChatNotReadyReasonCodes = new Dictionary<string, string>();
        //public static Dictionary<string, string> loadNotReadyReasonCodes = new Dictionary<string, string>();
        public static Dictionary<string, string> loadThresholdColor = new Dictionary<string, string>();
        public static Dictionary<string, int> loadThresholdValues = new Dictionary<string, int>();
        public static Dictionary<int, string> TServersSwitchDic = new Dictionary<int, string>();
        public static int UsedTServerSwitchDBId;
        public static Datacontext _instance = null;

        public string AgentGroupName = string.Empty;

        //End
        //26-04-2014 Added for the purpose of get employee id - Smoorthy
        public string AgentID = string.Empty;
        public string AgentLoginId = string.Empty;
        public string AgentPassword = string.Empty;
        public int agentWrapUpTime = 0;
        public string[] BroadCastSubscribTopics;
        public string[] BroadCastToastOrder = new string[4];
        public ContextMenu CallDataAdd = new ContextMenu();
        public string CallType = string.Empty;
        public List<string> channelCollection = new List<string>();
        public ContextMenu cmshow = new System.Windows.Controls.ContextMenu();
        public Dictionary<string, string> configuredAttachData = new Dictionary<string, string>();

        //Code Added - Smoorthy
        //03-04-2014
        public int ConfiguredWindowHeight = 95;
        public int ConfiguredWindowWidth = 0;
        public UniversalContactServerProtocol ContactServerProtocol = null;
        public int CurrentMemberId = 0;
        public string DialedNumber = string.Empty;
        public KeyValuePair<string, object> DispositionObjCollection = new KeyValuePair<string, object>();
        public bool Dualclick = false;
        public ContextMenu Forward = new ContextMenu();
        public Hashtable HshApplicationLevel = new Hashtable(StringComparer.InvariantCultureIgnoreCase);
        public Dictionary<Datacontext.Channels, string> htMediaCurrentState = new Dictionary<Datacontext.Channels, string>();
        public InteractionServerProtocol InteractionProtocol = null;
        public bool IsAboutOpened = false;
        public bool isAgentExtendACWTime = false;
        public bool IsChatPluginAdded = false;
        public bool IsCMELoginEnabled = false;
        public bool IsConfDialPadOpen = false;
        public bool IsDebug;
        public bool isDND = false;
        public bool IsEmailPluginAdded = false;

        //CallNotify
        public bool isEnableCallAccept = false;
        public bool isEnableCallReject = false;
        public bool IsEnableOutboundScreenPop = false;
        public bool isIXNDND = false;
        public bool IsLogonInfoOpened = false;
        public bool IsStatPluginAdded = false;
        public bool IsStatTickerLoaded = false;
        public bool IsTeamCommunicatorPluginAdded = false;
        public bool IsThirdPartyPluginAdded = false;
        public bool IsTopmost = false;
        public bool IsTransDialPadOpen = false;
        public bool IsTserverConnected = false;
        public bool IsWorkbinPluginAdded = false;
        public FontWeight KeyFontWeight;

        //end
        //ChannelSelection
        public Dictionary<string, bool> lstHoldingChannelSelection = new Dictionary<string, bool>();
        public Dictionary<int, string> MemberId = new Dictionary<int, string>();
        public string[] Msgsummary;
        public string OpenedMessage = "";
        public string OpenedNotifyMessage = "";

        //public string TrialXMLFile = Environment.GetFolderPath(Environment.SpecialFolder.CommonApplicationData).ToString() + @"\user.trial.config";
        //public string TrialXMLFile = System.Windows.Forms.Application.CommonAppDataPath.ToString() + @"\user.trial.config";
        //end
        public CfgPerson Person;
        public int ProxyID;
        public int referenceId = 5000;

        //private string xmlFile = Environment.CurrentDirectory.ToString() + @"\localuser.settings.config";
        public string SettingsXMLFile = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData).ToString() + @"\Pointel\AgentInteractionDesktop" + @"\localuser.settings.config";
        public bool Singleclick = false;

        //public string SettingXMLFile = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData) + @"\localuser.settings.config";
        //private string speedDialXMLFile = Environment.CurrentDirectory.ToString() + @"\SpeedDial.xml";
        public string SpeedDialXMLFile = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData).ToString() + @"\Pointel\AgentInteractionDesktop" + @"\SpeedDial.xml";
        public int StatProcessId;
        public string StatusChangeOnVoiceInteraction = string.Empty;
        public string SwitchName = string.Empty;
        public CfgSwitch SwitchType = null;
        public string TempFile = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData).ToString() + @"\Pointel\temp";
        public string ThisDN;
        public string ThridpartyDN;

        //public string SpeedDialXMLFile = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData).ToString() + @"\SpeedDial.xml";
        //private string trialXMLFile = Environment.CurrentDirectory.ToString() + @"\user.trial.config";
        public string TrialXMLFile = Environment.GetFolderPath(Environment.SpecialFolder.CommonApplicationData).ToString() + @"\Pointel\AgentInteractionDesktopV" + (System.Diagnostics.FileVersionInfo.GetVersionInfo(Assembly.GetExecutingAssembly().Location)).FileVersion + @"\user.trial.config";
        public int UnreadMsgCount = 0;
        public Dictionary<string, string> userAttachData = new Dictionary<string, string>();
        public ConsultType UserSetConfType = ConsultType.None;
        public ConsultType UserSetTransType = ConsultType.None;
        public string _wrapUpTime = "";

        private static bool isEnableStatusChangeOncall = false;
        private static string _accessGroup = string.Empty;
        private static ImageSource _loadAgentIcon;

        private bool isAgentClickedNotReady = false;
        private bool isAgentClickedReady = false;
        private bool isAgentClickedRetrieve = false;
        private bool isAlternateCallEnable = false;
        private string statusMessage = string.Empty;
        private GridLength statusMessageHeight;
        private string _agentFullName;
        private string _agentGroupStatisticsName = string.Empty;
        private GridLength _agentLoginIDRowHeight = new GridLength(0);
        private List<string> _agentLoginIds = new List<string>();
        private GridLength _agentPasswordRowHeight = new GridLength(0);
        private ImageSource _agentStateImageSource;
        private string _agentStatisticsName = string.Empty;

        //Code Added - V.Palaniappan
        //02.12.2013
        private ImageSource _alternateCallImageSource;
        private string _alternateCallText;
        private Dictionary<string, string> _annexContacts = new Dictionary<string, string>();

        //end
        private Dictionary<string, string> _annexStatValues = new Dictionary<string, string>();
        private string _applicationName = string.Empty;
        private List<string> _appNameList = new List<string>();
        private ImageSource _attachDataImageSource;
        private string _attachDatatext;
        private List<string> _autoComplete = new List<string>();
        private Visibility _a_DataTabVisibility;
        private GridLength _bottomRowHeight = new GridLength(0);
        private OpendBy _bottomwindow;
        private Dictionary<string, string> _broadCastAttributes = new Dictionary<string, string>();
        private Brush _broadCastBackgroundBrush;
        private Color _broadCastforegroundBrush;
        private ObservableCollection<IOutboundCallResult> _callResultItemSource = new ObservableCollection<IOutboundCallResult>();
        private string _callTypeStatus = string.Empty;
        private bool _canAddSkill;
        private bool _canDeleteSkill;
        private bool _canEditSkill;
        private ChannelSelection _channelSelectionWindow;
        private ContextMenu _chatContextMenu = new ContextMenu();
        private ImageSource _chatStateImageSource;
        private GridLength _confDetails_SP_Height;
        private GridLength _confDetails_SP_Height_keepplace;
        private string _configXMLName = string.Empty;
        private ImageSource _confImageSource;
        private string _conftext;
        private ContextMenu _consultCallSelection = new ContextMenu();
        private int _consultDialDigits = 0;
        private ObservableCollection<IContactCenterStatistics> _contactCenterStatistics = new ObservableCollection<IContactCenterStatistics>();
        private List<string> _contactExtnContextMenu = new List<string>();
        private ObservableCollection<IContacts> _contacts = new ObservableCollection<IContacts>();
        private ObservableCollection<IContacts> _contactsFilter = new ObservableCollection<IContacts>();
        private Visibility _contactTabVisibility;
        private string _csErrorMessage = string.Empty;
        private GridLength _csErrorRowHeightpublic = new GridLength(0);
        private string _currentEditingSkill = "";
        private string _dialedDigits = string.Empty;
        private string _dialedNumbers = string.Empty;
        private ImageSource _dialImageSource;
        private string _diallingNumber = string.Empty;
        private int _dialpadDigits = 0;
        private string _dialtext;
        private Visibility _dispositionOnlyVisibility = Visibility.Hidden;
        private Visibility _dispositionVisibility = Visibility.Visible;
        private Dictionary<string, Int64> _editingSkill = new Dictionary<string, Int64>();
        private ContextMenu _emailContextMenu = new ContextMenu();
        private ImageSource _emailStateImageSource;
        private bool _enableDNDMenuitems;
        private bool _enableGlobalDNDMenuitem;
        private bool _enableLogonMenuitem;
        private bool _enableMenuitems;
        private bool _enableRefinePlace;
        private string _errorMessage;
        private GridLength _errorRowHeight = new GridLength();
        private string _expanderHeader;
        private string _firstName;
        private string _forwardDN = string.Empty;
        private List<string> _forwardDns = new List<string>();
        private string _forwardStatus;

        //code ended
        //code added by Manikandan on 21-03-2014 to retain Stat Gadget State and enable/disable plugin
        private string _gadgetState = "Closed";
        private Visibility _gadgetVisibility = Visibility.Visible;
        private ContextMenu _globalStatesContextMenu = new ContextMenu();
        private ImageSource _holdImageSource;
        private string _holdtext;
        private ObservableCollection<string> _hostNameItemSource = new ObservableCollection<string>();
        private string _hostNameSelectedValue;
        private string _hostNameText = string.Empty;
        private ImportLanguageModule _importCatalog;
        private Importer _importClass;
        private string _interactionDataObjectName = string.Empty;
        private bool _isAddSkillsEnabled;
        private bool _isAlternateCallEnabled;
        private bool _isAttachDataEnabled;
        private bool _isCallDataFilter = false;

        //Code added by Manikandan on 08-04-2014 to implement enable.view from third party plugin
        //true for bcbs
        private bool _isCallWindowEnabled = false;
        private bool _isChatChecked = false;
        private bool _isConfEnabled;
        private bool _isContactsPluginAdded = false;
        private bool _isDeleteSkillsEnabled;
        private bool _isDialEnabled;
        private bool _isDispositioncodeEnabled;
        private bool _isDispositionNoneChecked = true;
        private bool _isEditSkill;
        private bool _isEmailChecked = false;
        private bool _isenableAutoAnswer = false;
        private bool _isEnableCaseDataFromTransaction = false;
        private bool _isEnableOutboundUpate;
        private bool _isEpicChecked = false;
        private bool _isExpanded;
        private bool _isFacebookChecked = false;
        private bool _isFacetsChecked = false;
        private bool _isHoldClicked = false;
        private bool _isHoldEnabled;
        private bool _isInitiateConfClicked = false;
        private bool _isInitiateTransferClicked = false;

        //bool Variables
        private bool _isKeepRecentPlace = false;
        private bool _isLawsonChecked = false;
        private bool _isMediPacChecked = false;
        private bool _isMergeCallEnabled;
        private bool _isMyContactsEnabled = false;
        private bool _isMyHistoryEnabled = false;
        private bool _isNotReadyResonCode;
        private bool _isOnCall = false;
        private bool _isOnChatIXN = false;
        private bool _isOnEmailIXN = false;
        private bool _isOutboundChecked = false;
        private bool _isOutboundPluginAdded = false;
        private Visibility _isPhoneBookEnabled;
        private bool _iSPopOpen;
        private bool _isReadyEnabled;
        private bool _isReConEnabled;
        private bool _isreconnectClicked = false;
        private bool _isRssChecked = false;
        private bool _isSalesforceAlive;
        private Visibility _isSalesForceEnabled;
        private Visibility _isShowMIDPushButton = Visibility.Collapsed;
        private bool _isSkillsTabEnabled;
        private bool _isSmsChecked = false;
        private bool _isStatisticsLogEnabled = false;
        private bool _isTalkEnabled;
        private bool _isTransEnabled;
        private bool _isTwitterChecked = false;
        private bool _isUpdateSkillsEnabled;
        private bool _isVoiceChecked = true;
        private Visibility _isVoiceEnabledAddCallData = Visibility.Hidden;
        private Visibility _isVoicePopCaseData;
        private Visibility _isVoicestateTimer;
        private bool _isWebCallBackChecked = false;
        private bool _isWorkItemChecked = false;
        private FontFamily _keyFontFamily;
        private List<Languages> _languageList;
        private string _lastName;
        private GridLength _ldCodeRowHeight = new GridLength(0);
        private ContextMenu _leftsettingContextMenu = new ContextMenu();
        private List<string> _loadAllSkills = new List<string>();
        private List<string> _loadAttachDataFilterKeys = new List<string>();
        private List<string> _loadAttachDataKeys = new List<string>();
        private List<string> _loadAttachDataSortKeys = new List<string>();
        private List<string> _loadCollection = new List<string>();
        private ImageSource _logImageSource;
        private GridLength _loginQueueRowHeight = new GridLength(0);
        private Login _loginWindow;
        private string _logtext;
        private string _longDistanceCode;
        private string _mainBorder;
        private Brush _mainBorderBrush;
        private int _maxDialDigits = 0;
        private ObservableCollection<IMediaStatus> _mediaStatus = new ObservableCollection<IMediaStatus>();

        //End
        //Code Added - V.Palaniappan
        //04.12.2013
        private ImageSource _mergeCallImageSource;

        //End
        private string _mergeCallText;
        private string _message1 = string.Empty;
        private string _message11 = string.Empty;
        private string _message111 = string.Empty;
        private string _message2 = string.Empty;
        private string _message22 = string.Empty;
        private string _message222 = string.Empty;
        private string _message3 = string.Empty;
        private string _message33 = string.Empty;
        private string _message333 = string.Empty;
        private string _message4 = string.Empty;
        private string _message44 = string.Empty;
        private string _message444 = string.Empty;
        private string _message55 = string.Empty;
        private string _message555 = string.Empty;
        private string _messageAudience = string.Empty;
        private string _messageBodyMsg = string.Empty;
        private string _messageCountRange = "-1";
        private string _messageDate = string.Empty;
        private ImageSource _messageIconImageSource;
        private string _messagePriority = string.Empty;
        private string _messageSender = string.Empty;
        private List<string> _messageSpaceExtnContextMenu = new List<string>();
        private string _messageSubject = string.Empty;
        private Visibility _messageTabVisibility;
        private string _messageType = string.Empty;
        private double _modifiedTextSize = 0;
        private GridLength _msgRowHeight = new GridLength(0);
        private ObservableCollection<IMyMessages> _myMessages = new ObservableCollection<IMyMessages>();
        private ObservableCollection<IMySkills> _mySkills = new ObservableCollection<IMySkills>();
        private ObservableCollection<IMyStatistics> _myStatistics = new ObservableCollection<IMyStatistics>();
        private ObservableCollection<ICallData> _notifyCallData = new ObservableCollection<ICallData>();
        private ICollectionView _notifyCallDataView;
        private string _notifyGadgetDisplayName;

        //Code added to Implement interaction data in separate window
        private int _openedCaseWindow = 0;
        private ContextMenu _optionsMenu = new ContextMenu();
        private ContextMenu _outboundContextMenu = new ContextMenu();
        private ImageSource _outboundStateImageSource;
        private string _password = string.Empty;
        private string _place;
        private List<string> _placeIDList = new List<string>();
        private ObservableCollection<string> _portItemSource = new ObservableCollection<string>();
        private string _portSelectedValue;
        private string _portText = string.Empty;
        private string _queue;
        private string _queueSelectedValue = "None";
        private ImageSource _readyImageSource;
        private string _readytext;
        private ImageSource _reConImageSource;
        private string _reContext;
        private string _requeueComments = string.Empty;
        private ListCollectionView _requeueData;
        private System.Windows.Threading.DispatcherTimer _requeueTimer = new System.Windows.Threading.DispatcherTimer();
        private string _salesForceText;
        private string _salesForceTextToolTip;
        private Languages _selectedLanguage;
        private DropShadowBitmapEffect _shadowEffect;
        private List<Int32> _skillLevelSource = new List<Int32>();
        private ContextMenu _skillsContextMenu = new ContextMenu();

        //Code Added by Manikandan 10-Jan-2013
        //For implementing the My Skills Tab
        private Visibility _skillsTabVisibility;
        private ContextMenu _statesContextMenu = new ContextMenu();
        private string _subVersion;
        private string _switchName;
        private int _tabSelectedIndex;
        private string _talkImageSource;
        private string _talktext;
        private Visibility _timerEnabled;
        private Brush _titleBgColor;
        private string _titleStatusText;
        private string _titleText;
        private ImageSource _transImageSource;
        private string _transtext;
        private string _trialMessage;
        private Visibility _trialVisibility = Visibility.Collapsed;
        private string _unreadMessageCount = "0";
        private string _userEmployeeID = string.Empty;
        private string _userLoginID;
        private string _userName;
        private string _userPlace;
        private string _userState;
        private string _voiceNotReadyReasonCode = string.Empty;
        private ImageSource _voiceStateImageSource;
        private Visibility _worksapceTabVisibility;
        private List<string> _workSpaceExtnContextMenu = new List<string>();

        //Code Added by Elango 11-Jan-2016
        //For Implementing the WrapTime updation
        private string _wrapTime;
        private GridLength _wrapTimeRowHeight = new GridLength(0);

        #endregion Fields

        #region Enumerations

        public enum Channels
        {
            None, Voice, Email, Chat, WebCallBack, OutboundPreview, WorkItem, SMS, FaceBook, Twitter
        }

        public enum ConsultType
        {
            None, OneStep, DualStep
        }

        public enum DialPadType
        {
            Normal, Transfer, Conference
        }

        public enum OpendBy
        {
            Workspace,
            MyMessage,
            MyContact
        }

        #endregion Enumerations

        #region Events

        public event PropertyChangedEventHandler PropertyChanged;

        #endregion Events

        #region Properties

        public string AgentEmployeeID
        {
            get
            {
                return _userEmployeeID;
            }
            set
            {
                if (_userEmployeeID != value)
                {
                    _userEmployeeID = value;
                    RaisePropertyChanged(() => AgentEmployeeID);
                }
            }
        }

        public string AgentFullName
        {
            get
            {
                return _agentFullName;
            }
            set
            {
                if (_agentFullName != value)
                {
                    _agentFullName = value;
                    RaisePropertyChanged(() => AgentFullName);
                }
            }
        }

        public GridLength AgentLoginIDRowHeight
        {
            get
            {
                return _agentLoginIDRowHeight;
            }
            set
            {
                if (_agentLoginIDRowHeight != value)
                {
                    _agentLoginIDRowHeight = value;
                    RaisePropertyChanged(() => AgentLoginIDRowHeight);
                }
            }
        }

        public List<string> AgentLoginIds
        {
            get
            {
                return _agentLoginIds;
            }
            set
            {
                if (_agentLoginIds != value)
                {
                    _agentLoginIds = value;
                    RaisePropertyChanged(() => AgentLoginIds);
                }
            }
        }

        public GridLength AgentPasswordRowHeight
        {
            get
            {
                return _agentPasswordRowHeight;
            }
            set
            {
                if (_agentPasswordRowHeight != value)
                {
                    _agentPasswordRowHeight = value;
                    RaisePropertyChanged(() => AgentPasswordRowHeight);
                }
            }
        }

        public ImageSource AgentStateImageSource
        {
            get
            {
                return _agentStateImageSource;
            }
            set
            {
                if (_agentStateImageSource != value)
                {
                    _agentStateImageSource = null;
                    _agentStateImageSource = value;
                    _agentStateImageSource.Freeze();
                    RaisePropertyChanged(() => AgentStateImageSource);
                }
            }
        }

        //Code Added - V.Palaniappan
        //04.12.2013
        public ImageSource AlternateCallImageSource
        {
            get
            {
                return _alternateCallImageSource;
            }
            set
            {
                if (_alternateCallImageSource != value)
                {
                    _alternateCallImageSource = null;
                    _alternateCallImageSource = value;
                    _alternateCallImageSource.Freeze();
                    RaisePropertyChanged(() => AlternateCallImageSource);
                }
            }
        }

        public string AlternateCallText
        {
            get
            {
                return _alternateCallText;
            }
            set
            {
                if (_alternateCallText != value)
                {
                    _alternateCallText = value;
                    RaisePropertyChanged(() => AlternateCallText);
                }
            }
        }

        public Dictionary<string, string> AnnexContacts
        {
            get
            {
                return _annexContacts;
            }
            set
            {
                if (_annexContacts != value)
                {
                    _annexContacts = value;
                    RaisePropertyChanged(() => AnnexContacts);
                }
            }
        }

        public Dictionary<string, string> AnnexStatValues
        {
            get
            {
                return _annexStatValues;
            }
            set
            {
                if (_annexStatValues != value)
                {
                    _annexStatValues = value;
                    RaisePropertyChanged(() => AnnexStatValues);
                }
            }
        }

        public string ApplicationName
        {
            get
            {
                return _applicationName;
            }
            set
            {
                if (_applicationName != value)
                {
                    _applicationName = value;
                    RaisePropertyChanged(() => ApplicationName);
                }
            }
        }

        public ImageSource AttachDataImageSource
        {
            get
            {
                return _attachDataImageSource;
            }
            set
            {
                if (_attachDataImageSource != value)
                {
                    _attachDataImageSource = null;
                    _attachDataImageSource = value;
                    _attachDataImageSource.Freeze();
                    RaisePropertyChanged(() => AttachDataImageSource);
                }
            }
        }

        public string AttachDataText
        {
            get
            {
                return _attachDatatext;
            }
            set
            {
                if (_attachDatatext != value)
                {
                    _attachDatatext = value;
                    RaisePropertyChanged(() => AttachDataText);
                }
            }
        }

        public List<string> AutoComplete
        {
            get
            {
                return _autoComplete;
            }
            set
            {
                if (_autoComplete != value)
                {
                    _autoComplete = value;
                    RaisePropertyChanged(() => AutoComplete);
                }
            }
        }

        public Visibility A_DataTabVisibility
        {
            get
            {
                return _a_DataTabVisibility;
            }
            set
            {
                if (_a_DataTabVisibility != value)
                {
                    _a_DataTabVisibility = value;
                    RaisePropertyChanged(() => A_DataTabVisibility);
                }
            }
        }

        public GridLength BottomRowHeight
        {
            get
            {
                return _bottomRowHeight;
            }
            set
            {
                if (_bottomRowHeight != value)
                {
                    _bottomRowHeight = value;
                    RaisePropertyChanged(() => BottomRowHeight);
                }
            }
        }

        /// <summary>
        /// The current DisplayState
        /// </summary>
        public OpendBy Bottomwindow
        {
            get
            {
                return this._bottomwindow;
            }
            set
            {
                if (value != this._bottomwindow)
                {
                    this._bottomwindow = value;
                }
            }
        }

        public Dictionary<string, string> BroadCastAttributes
        {
            get
            {
                return _broadCastAttributes;
            }
            set
            {
                if (_broadCastAttributes != value)
                {
                    _broadCastAttributes = value;
                }
            }
        }

        public Brush BroadCastBackgroundBrush
        {
            get
            {
                return _broadCastBackgroundBrush;
            }
            set
            {
                if (_broadCastBackgroundBrush != value)
                {
                    _broadCastBackgroundBrush = value;
                    RaisePropertyChanged(() => BroadCastBackgroundBrush);
                }
            }
        }

        public Color BroadCastForegroundBrush
        {
            get
            {
                return _broadCastforegroundBrush;
            }
            set
            {
                if (_broadCastforegroundBrush != value)
                {
                    _broadCastforegroundBrush = value;
                    RaisePropertyChanged(() => _broadCastforegroundBrush);
                }
            }
        }

        public ObservableCollection<IOutboundCallResult> CallResultItemSource
        {
            get
            {
                return _callResultItemSource;
            }
            set
            {
                if (_callResultItemSource != value)
                {
                    _callResultItemSource = value;
                    RaisePropertyChanged(() => CallResultItemSource);
                }
            }
        }

        public string CallTypeStatus
        {
            get
            {
                return _callTypeStatus;
            }
            set
            {
                if (_callTypeStatus != value)
                {
                    _callTypeStatus = value;
                    RaisePropertyChanged(() => CallTypeStatus);
                }
            }
        }

        public bool CanAddSkill
        {
            get
            {
                return _canAddSkill;
            }
            set
            {
                if (_canAddSkill != value)
                {
                    _canAddSkill = value;
                    RaisePropertyChanged(() => CanAddSkill);
                }
            }
        }

        public bool CanDeleteSkill
        {
            get
            {
                return _canDeleteSkill;
            }
            set
            {
                if (_canDeleteSkill != value)
                {
                    _canDeleteSkill = value;
                    RaisePropertyChanged(() => CanDeleteSkill);
                }
            }
        }

        public bool CanEditSkill
        {
            get
            {
                return _canEditSkill;
            }
            set
            {
                if (_canEditSkill != value)
                {
                    _canEditSkill = value;
                    RaisePropertyChanged(() => CanEditSkill);
                }
            }
        }

        public ChannelSelection ChannelSelectionWindow
        {
            get { return _channelSelectionWindow; }
            set { _channelSelectionWindow = value; }
        }

        public ContextMenu ChatContextMenu
        {
            get
            {
                return _chatContextMenu;
            }
            set
            {
                if (_chatContextMenu != value)
                {
                    _chatContextMenu = value;
                }
            }
        }

        public ImageSource ChatStateImageSource
        {
            get
            {
                return _chatStateImageSource;
            }
            set
            {
                if (_chatStateImageSource != value)
                {
                    _chatStateImageSource = null;
                    _chatStateImageSource = value;
                    _chatStateImageSource.Freeze();
                    RaisePropertyChanged(() => ChatStateImageSource);
                }
            }
        }

        public GridLength ConfDetails_SP_Height
        {
            get
            {
                return _confDetails_SP_Height;
            }
            set
            {
                if (_confDetails_SP_Height != value)
                {
                    _confDetails_SP_Height = value;
                    RaisePropertyChanged(() => ConfDetails_SP_Height);
                }
            }
        }

        public GridLength ConfDetails_SP_Height_keepplace
        {
            get
            {
                return _confDetails_SP_Height_keepplace;
            }
            set
            {
                if (_confDetails_SP_Height_keepplace != value)
                {
                    _confDetails_SP_Height_keepplace = value;
                    RaisePropertyChanged(() => ConfDetails_SP_Height_keepplace);
                }
            }
        }

        public ImageSource ConfImageSource
        {
            get
            {
                return _confImageSource;
            }
            set
            {
                if (_confImageSource != value)
                {
                    _confImageSource = null;
                    _confImageSource = value;
                    _confImageSource.Freeze();
                    RaisePropertyChanged(() => ConfImageSource);
                }
            }
        }

        public string ConfText
        {
            get
            {
                return _conftext;
            }
            set
            {
                if (_conftext != value)
                {
                    _conftext = value;
                    RaisePropertyChanged(() => ConfText);
                }
            }
        }

        public ContextMenu ConsultCallSelection
        {
            get
            {
                return _consultCallSelection;
            }
            set
            {
                if (_consultCallSelection != value)
                {
                    _consultCallSelection = value;
                }
            }
        }

        public int ConsultDialDigits
        {
            get
            {
                return _consultDialDigits;
            }
            set
            {
                if (_consultDialDigits != value)
                {
                    _consultDialDigits = value;
                }
            }
        }

        public ObservableCollection<IContactCenterStatistics> ContactCenterStatistics
        {
            get
            {
                return _contactCenterStatistics;
            }
            set
            {
                if (_contactCenterStatistics != value)
                {
                    _contactCenterStatistics = value;
                    RaisePropertyChanged(() => ContactCenterStatistics);
                }
            }
        }

        public ObservableCollection<IContacts> Contacts
        {
            get
            {
                return _contacts;
            }
            set
            {
                if (_contacts != value)
                {
                    _contacts = value;
                    RaisePropertyChanged(() => Contacts);
                }
            }
        }

        public ObservableCollection<IContacts> ContactsFilter
        {
            get
            {
                return _contactsFilter;
            }
            set
            {
                if (_contactsFilter != value)
                {
                    _contactsFilter = value;
                    RaisePropertyChanged(() => ContactsFilter);
                }
            }
        }

        public Visibility ContactTabVisibility
        {
            get
            {
                return _contactTabVisibility;
            }
            set
            {
                if (_contactTabVisibility != value)
                {
                    _contactTabVisibility = value;
                    RaisePropertyChanged(() => ContactTabVisibility);
                }
            }
        }

        public string CSErrorMessage
        {
            get
            {
                return _csErrorMessage;
            }
            set
            {
                if (_csErrorMessage != value)
                {
                    _csErrorMessage = value;
                    RaisePropertyChanged(() => CSErrorMessage);
                }
            }
        }

        public GridLength CSErrorRowHeight
        {
            get
            {
                return _csErrorRowHeightpublic;
            }
            set
            {
                if (_csErrorRowHeightpublic != value)
                {
                    _csErrorRowHeightpublic = value;
                    RaisePropertyChanged(() => CSErrorRowHeight);
                }
            }
        }

        public string CurrentEditingSkill
        {
            get
            {
                return _currentEditingSkill;
            }
            set
            {
                if (_currentEditingSkill != value)
                {
                    _currentEditingSkill = value;
                    RaisePropertyChanged(() => CurrentEditingSkill);
                }
            }
        }

        public string CurrentSwitchName
        {
            get
            {
                return _switchName;
            }
            set
            {
                if (_switchName != value)
                {
                    _switchName = value;
                    RaisePropertyChanged(() => CurrentSwitchName);
                }
            }
        }

        public string DialedDigits
        {
            get
            {
                return _dialedDigits;
            }
            set
            {
                if (_dialedDigits != value)
                {
                    _dialedDigits = value;
                    RaisePropertyChanged(() => DialedDigits);
                }
            }
        }

        public string DialedNumbers
        {
            get
            {
                return _dialedNumbers;
            }
            set
            {
                if (_dialedNumbers != value)
                {
                    _dialedNumbers = value;
                    RaisePropertyChanged(() => DialedNumbers);
                }
            }
        }

        public ImageSource DialImageSource
        {
            get
            {
                return _dialImageSource;
            }
            set
            {
                if (_dialImageSource != value)
                {
                    _dialImageSource = null;
                    _dialImageSource = value;
                    _dialImageSource.Freeze();
                    RaisePropertyChanged(() => DialImageSource);
                }
            }
        }

        public string DiallingNumber
        {
            get
            {
                return _diallingNumber;
            }
            set
            {
                if (_diallingNumber != value)
                {
                    _diallingNumber = value;
                    RaisePropertyChanged(() => DiallingNumber);
                }
            }
        }

        //End
        public int DialpadDigits
        {
            get
            {
                return _dialpadDigits;
            }
            set
            {
                if (_dialpadDigits != value)
                {
                    _dialpadDigits = value;
                }
            }
        }

        public string DialText
        {
            get
            {
                return _dialtext;
            }
            set
            {
                if (_dialtext != value)
                {
                    _dialtext = value;
                    RaisePropertyChanged(() => DialText);
                }
            }
        }

        public Visibility DispositionOnlyVisibility
        {
            get
            {
                return _dispositionOnlyVisibility;
            }
            set
            {
                if (_dispositionOnlyVisibility != value)
                {
                    _dispositionOnlyVisibility = value;
                    RaisePropertyChanged(() => DispositionOnlyVisibility);
                }
            }
        }

        public Visibility DispositionVisibility
        {
            get
            {
                return _dispositionVisibility;
            }
            set
            {
                if (_dispositionVisibility != value)
                {
                    _dispositionVisibility = value;
                    RaisePropertyChanged(() => DispositionVisibility);
                }
            }
        }

        public Dictionary<string, Int64> EditingSkill
        {
            get
            {
                return _editingSkill;
            }
            set
            {
                if (_editingSkill != value)
                {
                    _editingSkill = value;
                    RaisePropertyChanged(() => EditingSkill);
                }
            }
        }

        public ContextMenu EmailContextMenu
        {
            get
            {
                return _emailContextMenu;
            }
            set
            {
                if (_emailContextMenu != value)
                {
                    _emailContextMenu = value;
                }
            }
        }

        public ImageSource EmailStateImageSource
        {
            get
            {
                return _emailStateImageSource;
            }
            set
            {
                if (_emailStateImageSource != value)
                {
                    _emailStateImageSource = null;
                    _emailStateImageSource = value;
                    _emailStateImageSource.Freeze();
                    RaisePropertyChanged(() => EmailStateImageSource);
                }
            }
        }

        public bool EnableDNDMenuitems
        {
            get
            {
                return _enableDNDMenuitems;
            }
            set
            {
                if (_enableDNDMenuitems != value)
                {
                    _enableDNDMenuitems = value;
                    RaisePropertyChanged(() => EnableDNDMenuitems);
                }
            }
        }

        public bool EnableGlobalDNDMenuitem
        {
            get
            {
                return _enableGlobalDNDMenuitem;
            }
            set
            {
                if (_enableGlobalDNDMenuitem != value)
                {
                    _enableGlobalDNDMenuitem = value;
                    RaisePropertyChanged(() => EnableGlobalDNDMenuitem);
                }
            }
        }

        public bool EnableLogonMenuitem
        {
            get
            {
                return _enableLogonMenuitem;
            }
            set
            {
                if (_enableLogonMenuitem != value)
                {
                    _enableLogonMenuitem = value;
                    RaisePropertyChanged(() => EnableLogonMenuitem);
                }
            }
        }

        public bool EnableMenuitems
        {
            get
            {
                return _enableMenuitems;
            }
            set
            {
                if (_enableMenuitems != value)
                {
                    _enableMenuitems = value;
                    RaisePropertyChanged(() => EnableMenuitems);
                }
            }
        }

        public bool EnableRefinePlace
        {
            get
            {
                return _enableRefinePlace;
            }
            set
            {
                if (_enableRefinePlace != value)
                {
                    _enableRefinePlace = value;
                    RaisePropertyChanged(() => EnableRefinePlace);
                }
            }
        }

        public string ErrorMessage
        {
            get
            {
                return _errorMessage;
            }
            set
            {
                if (_errorMessage != value)
                {
                    _errorMessage = value;
                    RaisePropertyChanged(() => ErrorMessage);
                }
            }
        }

        public GridLength ErrorRowHeight
        {
            get
            {
                return _errorRowHeight;
            }
            set
            {
                if (_errorRowHeight != value)
                {
                    _errorRowHeight = value;
                    RaisePropertyChanged(() => ErrorRowHeight);
                }
            }
        }

        public string ForwardDN
        {
            get
            {
                return _forwardDN;
            }
            set
            {
                if (_forwardDN != value)
                {
                    _forwardDN = value;
                    RaisePropertyChanged(() => ForwardDN);
                }
            }
        }

        public List<string> ForwardDns
        {
            get
            {
                return _forwardDns;
            }
            set
            {
                if (_forwardDns != value)
                {
                    _forwardDns = value;
                    RaisePropertyChanged(() => ForwardDns);
                }
            }
        }

        public string ForwardStatus
        {
            get
            {
                return _forwardStatus;
            }
            set
            {
                if (_forwardStatus != value)
                {
                    _forwardStatus = value;
                }
            }
        }

        //Code ended
        //  //code added by Manikandan on 21-03-2014 to retain Stat Gadget State and enable/disable plugin
        public string GadgetState
        {
            get
            {
                return _gadgetState;
            }
            set
            {
                if (_gadgetState != value)
                {
                    _gadgetState = value;
                }
            }
        }

        public Visibility GadgetVisibility
        {
            get
            {
                return _gadgetVisibility;
            }
            set
            {
                if (_gadgetVisibility != value)
                {
                    _gadgetVisibility = value;
                    RaisePropertyChanged(() => GadgetVisibility);
                }
            }
        }

        public ContextMenu GlobalStatesContextMenu
        {
            get
            {
                return _globalStatesContextMenu;
            }
            set
            {
                if (_globalStatesContextMenu != value)
                {
                    _globalStatesContextMenu = value;
                }
            }
        }

        public ImageSource HoldImageSource
        {
            get
            {
                return _holdImageSource;
            }
            set
            {
                if (_holdImageSource != value)
                {
                    _holdImageSource = null;
                    _holdImageSource = value;
                    _holdImageSource.Freeze();
                    RaisePropertyChanged(() => HoldImageSource);
                }
            }
        }

        public string HoldText
        {
            get
            {
                return _holdtext;
            }
            set
            {
                if (_holdtext != value)
                {
                    _holdtext = value;
                    RaisePropertyChanged(() => HoldText);
                }
            }
        }

        public ObservableCollection<string> HostNameItemSource
        {
            get
            {
                return _hostNameItemSource;
            }
            set
            {
                if (_hostNameItemSource != value)
                {
                    _hostNameItemSource = value;
                    RaisePropertyChanged(() => HostNameItemSource);
                }
            }
        }

        public string HostNameSelectedValue
        {
            get
            {
                return _hostNameSelectedValue;
            }
            set
            {
                if (_hostNameSelectedValue != value)
                {
                    _hostNameSelectedValue = value;
                    RaisePropertyChanged(() => HostNameSelectedValue);
                }
            }
        }

        public string HostNameText
        {
            get
            {
                return _hostNameText;
            }
            set
            {
                if (_hostNameText != value)
                {
                    _hostNameText = value;
                    RaisePropertyChanged(() => HostNameText);
                }
            }
        }

        /// <summary>
        /// Gets the import catalog.
        /// </summary>
        /// <value>
        /// The import catalog.
        /// </value>
        public ImportLanguageModule ImportCatalog
        {
            get
            {
                _importCatalog = _importCatalog ?? new ImportLanguageModule();
                return _importCatalog;
            }
        }

        public Importer ImportClass
        {
            get
            {
                _importClass = _importClass ?? new Importer();
                return _importClass;
            }
        }

        public bool IsAddSkillsEnabled
        {
            get
            {
                return _isAddSkillsEnabled;
            }
            set
            {
                if (_isAddSkillsEnabled != value)
                {
                    _isAddSkillsEnabled = value;
                    RaisePropertyChanged(() => IsAddSkillsEnabled);
                }
            }
        }

        public bool IsAgentClickedNotReady
        {
            get
            {
                return isAgentClickedNotReady;
            }
            set
            {
                if (isAgentClickedNotReady != value)
                {
                    isAgentClickedNotReady = value;
                    RaisePropertyChanged(() => IsAgentClickedNotReady);
                }
            }
        }

        public bool IsAgentClickedReady
        {
            get
            {
                return isAgentClickedReady;
            }
            set
            {
                if (isAgentClickedReady != value)
                {
                    isAgentClickedReady = value;
                    RaisePropertyChanged(() => IsAgentClickedReady);
                }
            }
        }

        public bool IsAlternateCallEnabled
        {
            get
            {
                return _isAlternateCallEnabled;
            }
            set
            {
                if (_isAlternateCallEnabled != value)
                {
                    _isAlternateCallEnabled = value;
                    RaisePropertyChanged(() => IsAlternateCallEnabled);
                }
            }
        }

        public bool IsAttachDataEnabled
        {
            get
            {
                return _isAttachDataEnabled;
            }
            set
            {
                if (_isAttachDataEnabled != value)
                {
                    _isAttachDataEnabled = value;
                    RaisePropertyChanged(() => IsAttachDataEnabled);
                }
            }
        }

        public bool IsCallWindowEnabled
        {
            get
            {
                return _isCallWindowEnabled;
            }
            set
            {
                if (_isCallWindowEnabled != value)
                {
                    _isCallWindowEnabled = value;
                }
            }
        }

        public bool IsChatChecked
        {
            get
            {
                return _isChatChecked;
            }
            set
            {
                if (_isChatChecked != value)
                {
                    _isChatChecked = value;
                    RaisePropertyChanged(() => IsChatChecked);
                }
            }
        }

        public bool IsConfEnabled
        {
            get
            {
                return _isConfEnabled;
            }
            set
            {
                if (_isConfEnabled != value)
                {
                    _isConfEnabled = value;
                    RaisePropertyChanged(() => IsConfEnabled);
                }
            }
        }

        public bool IsContactsPluginAdded
        {
            get
            {
                return _isContactsPluginAdded;
            }
            set
            {
                if (_isContactsPluginAdded != value)
                {
                    _isContactsPluginAdded = value;
                }
            }
        }

        public bool IsDeleteSkillsEnabled
        {
            get
            {
                return _isDeleteSkillsEnabled;
            }
            set
            {
                if (_isDeleteSkillsEnabled != value)
                {
                    _isDeleteSkillsEnabled = value;
                    RaisePropertyChanged(() => IsDeleteSkillsEnabled);
                }
            }
        }

        public bool IsDialEnabled
        {
            get
            {
                return _isDialEnabled;
            }
            set
            {
                if (_isDialEnabled != value)
                {
                    _isDialEnabled = value;
                    RaisePropertyChanged(() => IsDialEnabled);
                }
            }
        }

        public bool IsDispositioncodeEnabled
        {
            get { return _isDispositioncodeEnabled; }
            set
            {
                _isDispositioncodeEnabled = value;
                RaisePropertyChanged(() => IsDispositioncodeEnabled);
            }
        }

        public bool IsDispositionNoneChecked
        {
            get
            {
                return _isDispositionNoneChecked;
            }
            set
            {
                if (_isDispositionNoneChecked != value)
                {
                    _isDispositionNoneChecked = value;
                    RaisePropertyChanged(() => IsDispositionNoneChecked);
                }
            }
        }

        public bool IsEditSkill
        {
            get
            {
                return _isEditSkill;
            }
            set
            {
                if (_isEditSkill != value)
                {
                    _isEditSkill = value;
                    RaisePropertyChanged(() => IsEditSkill);
                }
            }
        }

        public bool IsEmailChecked
        {
            get
            {
                return _isEmailChecked;
            }
            set
            {
                if (_isEmailChecked != value)
                {
                    _isEmailChecked = value;
                    RaisePropertyChanged(() => IsEmailChecked);
                }
            }
        }

        public bool IsEnableOutboundUpdate
        {
            get
            {
                return _isEnableOutboundUpate;
            }
            set
            {
                if (_isEnableOutboundUpate != value)
                {
                    _isEnableOutboundUpate = value;
                    RaisePropertyChanged(() => IsEnableOutboundUpdate);
                }
            }
        }

        public bool IsEpicChecked
        {
            get
            {
                return _isEpicChecked;
            }
            set
            {
                if (_isEpicChecked != value)
                {
                    _isEpicChecked = value;
                    RaisePropertyChanged(() => IsEpicChecked);
                }
            }
        }

        public bool IsFacebookChecked
        {
            get
            {
                return _isFacebookChecked;
            }
            set
            {
                if (_isFacebookChecked != value)
                {
                    _isFacebookChecked = value;
                    RaisePropertyChanged(() => IsFacebookChecked);
                }
            }
        }

        public bool IsFacetsChecked
        {
            get
            {
                return _isFacetsChecked;
            }
            set
            {
                if (_isFacetsChecked != value)
                {
                    _isFacetsChecked = value;
                    RaisePropertyChanged(() => IsFacetsChecked);
                }
            }
        }

        public bool IsHoldClicked
        {
            get
            {
                return _isHoldClicked;
            }
            set
            {
                if (_isHoldClicked != value)
                {
                    _isHoldClicked = value;
                    RaisePropertyChanged(() => IsHoldClicked);
                }
            }
        }

        public bool IsHoldEnabled
        {
            get
            {
                return _isHoldEnabled;
            }
            set
            {
                if (_isHoldEnabled != value)
                {
                    _isHoldEnabled = value;
                    RaisePropertyChanged(() => IsHoldEnabled);
                }
            }
        }

        public bool IsInitiateConfClicked
        {
            get { return _isInitiateConfClicked; }
            set { _isInitiateConfClicked = value; }
        }

        public bool IsInitiateTransClicked
        {
            get { return _isInitiateTransferClicked; }
            set { _isInitiateTransferClicked = value; }
        }

        public bool IsLawsonChecked
        {
            get
            {
                return _isLawsonChecked;
            }
            set
            {
                if (_isLawsonChecked != value)
                {
                    _isLawsonChecked = value;
                    RaisePropertyChanged(() => IsLawsonChecked);
                }
            }
        }

        public bool IsMediPacChecked
        {
            get
            {
                return _isMediPacChecked;
            }
            set
            {
                if (_isMediPacChecked != value)
                {
                    _isMediPacChecked = value;
                    RaisePropertyChanged(() => IsMediPacChecked);
                }
            }
        }

        public bool IsMergeCallEnabled
        {
            get
            {
                return _isMergeCallEnabled;
            }
            set
            {
                if (_isMergeCallEnabled != value)
                {
                    _isMergeCallEnabled = value;
                    RaisePropertyChanged(() => IsMergeCallEnabled);
                }
            }
        }

        public bool isOnCall
        {
            get
            {
                return _isOnCall;
            }
            set
            {
                if (value != _isOnCall)
                {
                    _isOnCall = value;
                }
                if (_isOnCall)
                { Datacontext.GetInstance().MaxDialDigits = Datacontext.GetInstance().ConsultDialDigits; }
                else { Datacontext.GetInstance().MaxDialDigits = Datacontext.GetInstance().DialpadDigits; }
            }
        }

        public bool IsOnChatIXN
        {
            get
            {
                return _isOnChatIXN;
            }
            set
            {
                _isOnChatIXN = value;
            }
        }

        public bool IsOnEmailIXN
        {
            get
            {
                return _isOnEmailIXN;
            }
            set
            {
                _isOnEmailIXN = value;
            }
        }

        public bool IsOutboundChecked
        {
            get
            {
                return _isOutboundChecked;
            }
            set
            {
                if (_isOutboundChecked != value)
                {
                    _isOutboundChecked = value;
                    RaisePropertyChanged(() => IsOutboundChecked);
                }
            }
        }

        public bool IsOutboundPluginAdded
        {
            get
            {
                return _isOutboundPluginAdded;
            }
            set
            {
                if (_isOutboundPluginAdded != value)
                    _isOutboundPluginAdded = value;
            }
        }

        public Visibility IsPhoneBookEnabled
        {
            get
            {
                return _isPhoneBookEnabled;
            }
            set
            {
                if (_isPhoneBookEnabled != value)
                {
                    _isPhoneBookEnabled = value;
                    RaisePropertyChanged(() => IsPhoneBookEnabled);
                }
            }
        }

        public bool IsReadyEnabled
        {
            get
            {
                return _isReadyEnabled;
            }
            set
            {
                if (_isReadyEnabled != value)
                {
                    _isReadyEnabled = value;
                    RaisePropertyChanged(() => IsReadyEnabled);
                }
            }
        }

        public bool IsReConEnabled
        {
            get
            {
                return _isReConEnabled;
            }
            set
            {
                if (_isReConEnabled != value)
                {
                    _isReConEnabled = value;
                    RaisePropertyChanged(() => IsReConEnabled);
                }
            }
        }

        public bool IsReconnectClicked
        {
            get { return _isreconnectClicked; }
            set { _isreconnectClicked = value; }
        }

        public bool IsRssChecked
        {
            get
            {
                return _isRssChecked;
            }
            set
            {
                if (_isRssChecked != value)
                {
                    _isRssChecked = value;
                    RaisePropertyChanged(() => IsRssChecked);
                }
            }
        }

        public bool IsSalesforceAlive
        {
            get
            {
                return _isSalesforceAlive;
            }
            set
            {
                if (_isSalesforceAlive != value)
                {
                    _isSalesforceAlive = value;
                    RaisePropertyChanged(() => IsSalesforceAlive);
                }
            }
        }

        public Visibility IsSalesForceEnabled
        {
            get
            {
                return _isSalesForceEnabled;
            }
            set
            {
                if (_isSalesForceEnabled != value)
                {
                    _isSalesForceEnabled = value;
                    RaisePropertyChanged(() => IsSalesForceEnabled);
                }
            }
        }

        public bool IsSalesforcePluginAdded
        {
            get;
            set;
        }

        public bool IsSalesforcePluginEnabled
        {
            get;
            set;
        }

        public Visibility IsShowMIDPushButton
        {
            get
            {
                return _isShowMIDPushButton;
            }
            set
            {
                if (_isShowMIDPushButton != value)
                {
                    _isShowMIDPushButton = value;
                    RaisePropertyChanged(() => IsShowMIDPushButton);
                }
            }
        }

        public bool IsSkillsTabEnabled
        {
            get
            {
                return _isSkillsTabEnabled;
            }
            set
            {
                if (_isSkillsTabEnabled != value)
                {
                    _isSkillsTabEnabled = value;
                    RaisePropertyChanged(() => IsSkillsTabEnabled);
                }
            }
        }

        public bool IsSmsChecked
        {
            get
            {
                return _isSmsChecked;
            }
            set
            {
                if (_isSmsChecked != value)
                {
                    _isSmsChecked = value;
                    RaisePropertyChanged(() => IsSmsChecked);
                }
            }
        }

        public bool IsTalkEnabled
        {
            get
            {
                return _isTalkEnabled;
            }
            set
            {
                if (_isTalkEnabled != value)
                {
                    _isTalkEnabled = value;
                    RaisePropertyChanged(() => IsTalkEnabled);
                }
            }
        }

        public bool IsTransEnabled
        {
            get
            {
                return _isTransEnabled;
            }
            set
            {
                if (_isTransEnabled != value)
                {
                    _isTransEnabled = value;
                    RaisePropertyChanged(() => IsTransEnabled);
                }
            }
        }

        public bool IsTwitterChecked
        {
            get
            {
                return _isTwitterChecked;
            }
            set
            {
                if (_isTwitterChecked != value)
                {
                    _isTwitterChecked = value;
                    RaisePropertyChanged(() => IsTwitterChecked);
                }
            }
        }

        public bool IsUpdateSkillsEnabled
        {
            get
            {
                return _isUpdateSkillsEnabled;
            }
            set
            {
                if (_isUpdateSkillsEnabled != value)
                {
                    _isUpdateSkillsEnabled = value;
                    RaisePropertyChanged(() => IsUpdateSkillsEnabled);
                }
            }
        }

        public bool IsVoiceChecked
        {
            get
            {
                return _isVoiceChecked;
            }
            set
            {
                if (_isVoiceChecked != value)
                {
                    _isVoiceChecked = value;
                    RaisePropertyChanged(() => IsVoiceChecked);
                }
            }
        }

        public Visibility IsVoiceEnabledAddCallData
        {
            get { return _isVoiceEnabledAddCallData; }
            set
            {
                _isVoiceEnabledAddCallData = value;
                RaisePropertyChanged(() => IsVoiceEnabledAddCallData);
            }
        }

        public Visibility IsVoicePopCaseData
        {
            get { return _isVoicePopCaseData; }
            set
            {
                _isVoicePopCaseData = value;
                RaisePropertyChanged(() => IsVoicePopCaseData);
            }
        }

        public Visibility IsVoiceStateTimer
        {
            get
            {
                return _isVoicestateTimer;
            }
            set
            {
                if (_isVoicestateTimer != value)
                {
                    _isVoicestateTimer = value;
                }
            }
        }

        public bool IsWebCallBackChecked
        {
            get
            {
                return _isWebCallBackChecked;
            }
            set
            {
                if (_isWebCallBackChecked != value)
                {
                    _isWebCallBackChecked = value;
                    RaisePropertyChanged(() => IsWebCallBackChecked);
                }
            }
        }

        public bool IsWorkItemChecked
        {
            get
            {
                return _isWorkItemChecked;
            }
            set
            {
                if (_isWorkItemChecked != value)
                {
                    _isWorkItemChecked = value;
                    RaisePropertyChanged(() => IsWorkItemChecked);
                }
            }
        }

        public bool KeepRecentPlace
        {
            get
            {
                return _isKeepRecentPlace;
            }
            set
            {
                if (_isKeepRecentPlace != value)
                {
                    _isKeepRecentPlace = value;
                    RaisePropertyChanged(() => KeepRecentPlace);
                }
            }
        }

        public FontFamily KeyFontFamily
        {
            get
            {
                return _keyFontFamily;
            }
            set
            {
                if (_keyFontFamily != value)
                {
                    _keyFontFamily = value;
                }
            }
        }

        public List<Languages> LanguageList
        {
            get { return _languageList; }
            set
            {
                if (_languageList != value)
                {
                    _languageList = value;
                    RaisePropertyChanged(() => LanguageList);
                }
            }
        }

        public GridLength LDCodeRowHeight
        {
            get
            {
                return _ldCodeRowHeight;
            }
            set
            {
                if (_ldCodeRowHeight != value)
                {
                    _ldCodeRowHeight = value;
                    RaisePropertyChanged(() => LDCodeRowHeight);
                }
            }
        }

        public List<string> LoadAllSkills
        {
            get
            {
                return _loadAllSkills;
            }
            set
            {
                if (_loadAllSkills != value)
                {
                    _loadAllSkills = value;
                    RaisePropertyChanged(() => LoadAllSkills);
                }
            }
        }

        public List<string> LoadAttachDataKeys
        {
            get
            {
                return _loadAttachDataKeys;
            }
            set
            {
                if (_loadAttachDataKeys != value)
                {
                    _loadAttachDataKeys = value;
                    RaisePropertyChanged(() => LoadAttachDataKeys);
                }
            }
        }

        public List<string> LoadCollection
        {
            get
            {
                return _loadCollection;
            }
            set
            {
                if (_loadCollection != value)
                {
                    _loadCollection = value;
                    RaisePropertyChanged(() => LoadCollection);
                }
            }
        }

        public ImageSource LogImageSource
        {
            get
            {
                return _logImageSource;
            }
            set
            {
                if (_logImageSource != value)
                {
                    _logImageSource = null;
                    _logImageSource = value;
                    _logImageSource.Freeze();
                    RaisePropertyChanged(() => LogImageSource);
                }
            }
        }

        public GridLength LoginQueueRowHeight
        {
            get
            {
                return _loginQueueRowHeight;
            }
            set
            {
                if (_loginQueueRowHeight != value)
                {
                    _loginQueueRowHeight = value;
                    RaisePropertyChanged(() => LoginQueueRowHeight);
                }
            }
        }

        public Login LoginWindow
        {
            get { return _loginWindow; }
            set { _loginWindow = value; }
        }

        public string LogText
        {
            get
            {
                return _logtext;
            }
            set
            {
                if (_logtext != value)
                {
                    _logtext = value;
                    RaisePropertyChanged(() => LogText);
                }
            }
        }

        public string LongDistanceCode
        {
            get
            {
                return _longDistanceCode;
            }
            set
            {
                if (_longDistanceCode != value)
                {
                    _longDistanceCode = value;
                    RaisePropertyChanged(() => LongDistanceCode);
                }
            }
        }

        public string MainBorder
        {
            get
            {
                return _mainBorder;
            }
            set
            {
                if (_mainBorder != value)
                {
                    _mainBorder = value;
                    RaisePropertyChanged(() => MainBorder);
                }
            }
        }

        public Brush MainBorderBrush
        {
            get
            {
                return _mainBorderBrush;
            }
            set
            {
                if (_mainBorderBrush != value)
                {
                    _mainBorderBrush = value;
                    RaisePropertyChanged(() => MainBorderBrush);
                }
            }
        }

        public int MaxDialDigits
        {
            get
            {
                return _maxDialDigits;
            }
            set
            {
                if (_maxDialDigits != value)
                {
                    _maxDialDigits = value;
                    RaisePropertyChanged(() => MaxDialDigits);
                }
            }
        }

        public ObservableCollection<IMediaStatus> MediaStatus
        {
            get
            {
                return _mediaStatus;
            }
            set
            {
                if (_mediaStatus != value)
                {
                    _mediaStatus = value;
                    RaisePropertyChanged(() => MediaStatus);
                }
            }
        }

        public ImageSource MergeCallImageSource
        {
            get
            {
                return _mergeCallImageSource;
            }
            set
            {
                if (_mergeCallImageSource != value)
                {
                    _mergeCallImageSource = null;
                    _mergeCallImageSource = value;
                    _mergeCallImageSource.Freeze();
                    RaisePropertyChanged(() => MergeCallImageSource);
                }
            }
        }

        public string MergeCallText
        {
            get
            {
                return _mergeCallText;
            }
            set
            {
                if (_mergeCallText != value)
                {
                    _mergeCallText = value;
                    RaisePropertyChanged(() => MergeCallText);
                }
            }
        }

        public string Message1
        {
            get
            {
                return _message1;
            }
            set
            {
                if (_message1 != value)
                {
                    _message1 = value;
                    RaisePropertyChanged(() => Message1);
                }
            }
        }

        public string Message11
        {
            get
            {
                return _message11;
            }
            set
            {
                if (_message11 != value)
                {
                    _message11 = value;
                    RaisePropertyChanged(() => Message11);
                }
            }
        }

        public string Message111
        {
            get
            {
                return _message111;
            }
            set
            {
                if (_message111 != value)
                {
                    _message111 = value;
                    RaisePropertyChanged(() => Message111);
                }
            }
        }

        public string Message2
        {
            get
            {
                return _message2;
            }
            set
            {
                if (_message2 != value)
                {
                    _message2 = value;
                    RaisePropertyChanged(() => Message2);
                }
            }
        }

        public string Message22
        {
            get
            {
                return _message22;
            }
            set
            {
                if (_message22 != value)
                {
                    _message22 = value;
                    RaisePropertyChanged(() => Message22);
                }
            }
        }

        public string Message222
        {
            get
            {
                return _message222;
            }
            set
            {
                if (_message222 != value)
                {
                    _message222 = value;
                    RaisePropertyChanged(() => Message222);
                }
            }
        }

        public string Message3
        {
            get
            {
                return _message3;
            }
            set
            {
                if (_message3 != value)
                {
                    _message3 = value;
                    RaisePropertyChanged(() => Message3);
                }
            }
        }

        public string Message33
        {
            get
            {
                return _message33;
            }
            set
            {
                if (_message33 != value)
                {
                    _message33 = value;
                    RaisePropertyChanged(() => Message33);
                }
            }
        }

        public string Message333
        {
            get
            {
                return _message333;
            }
            set
            {
                if (_message333 != value)
                {
                    _message333 = value;
                    RaisePropertyChanged(() => Message333);
                }
            }
        }

        public string Message4
        {
            get
            {
                return _message4;
            }
            set
            {
                if (_message4 != value)
                {
                    _message4 = value;
                    RaisePropertyChanged(() => Message4);
                }
            }
        }

        public string Message44
        {
            get
            {
                return _message44;
            }
            set
            {
                if (_message44 != value)
                {
                    _message44 = value;
                    RaisePropertyChanged(() => Message44);
                }
            }
        }

        public string Message444
        {
            get
            {
                return _message444;
            }
            set
            {
                if (_message444 != value)
                {
                    _message444 = value;
                    RaisePropertyChanged(() => Message444);
                }
            }
        }

        public string Message55
        {
            get
            {
                return _message55;
            }
            set
            {
                if (_message55 != value)
                {
                    _message55 = value;
                    RaisePropertyChanged(() => Message55);
                }
            }
        }

        public string Message555
        {
            get
            {
                return _message555;
            }
            set
            {
                if (_message555 != value)
                {
                    _message555 = value;
                    RaisePropertyChanged(() => Message555);
                }
            }
        }

        public string MessageAudience
        {
            get
            {
                return _messageAudience;
            }
            set
            {
                if (_messageAudience != value)
                {
                    _messageAudience = value;
                    RaisePropertyChanged(() => MessageAudience);
                }
            }
        }

        public string MessageBodyMsg
        {
            get
            {
                return _messageBodyMsg;
            }
            set
            {
                if (_messageBodyMsg != value)
                {
                    _messageBodyMsg = value;
                    RaisePropertyChanged(() => MessageBodyMsg);
                }
            }
        }

        public string MessageCountRange
        {
            get { return _messageCountRange; }
            set
            {
                _messageCountRange = value;
                RaisePropertyChanged(() => MessageCountRange);
            }
        }

        public string MessageDate
        {
            get
            {
                return _messageDate;
            }
            set
            {
                if (_messageDate != value)
                {
                    _messageDate = value;
                    RaisePropertyChanged(() => MessageDate);
                }
            }
        }

        public ImageSource MessageIconImageSource
        {
            get
            {
                return _messageIconImageSource;
            }
            set
            {
                if (_messageIconImageSource != value)
                {
                    _messageIconImageSource = null;
                    _messageIconImageSource = value;
                    _messageIconImageSource.Freeze();
                    RaisePropertyChanged(() => MessageIconImageSource);
                }
            }
        }

        public string MessagePriority
        {
            get
            {
                return _messagePriority;
            }
            set
            {
                if (_messagePriority != value)
                {
                    _messagePriority = value;
                    RaisePropertyChanged(() => MessagePriority);
                }
            }
        }

        public string MessageSender
        {
            get
            {
                return _messageSender;
            }
            set
            {
                if (_messageSender != value)
                {
                    _messageSender = value;
                    RaisePropertyChanged(() => MessageSender);
                }
            }
        }

        public string MessageSubject
        {
            get
            {
                return _messageSubject;
            }
            set
            {
                if (_messageSubject != value)
                {
                    _messageSubject = value;
                    RaisePropertyChanged(() => MessageSubject);
                }
            }
        }

        public Visibility MessageTabVisibility
        {
            get
            {
                return _messageTabVisibility;
            }
            set
            {
                if (_messageTabVisibility != value)
                {
                    _messageTabVisibility = value;
                    RaisePropertyChanged(() => MessageTabVisibility);
                }
            }
        }

        public string MessageType
        {
            get
            {
                return _messageType;
            }
            set
            {
                if (_messageType != value)
                {
                    _messageType = value;
                    RaisePropertyChanged(() => MessageType);
                }
            }
        }

        public double ModifiedTextSize
        {
            get
            {
                return _modifiedTextSize;
            }
            set
            {
                if (_modifiedTextSize != value)
                {
                    _modifiedTextSize = value;
                    RaisePropertyChanged(() => ModifiedTextSize);
                }
            }
        }

        public GridLength MsgRowHeight
        {
            get
            {
                return _msgRowHeight;
            }
            set
            {
                if (_msgRowHeight != value)
                {
                    _msgRowHeight = value;
                    RaisePropertyChanged(() => MsgRowHeight);
                }
            }
        }

        public ObservableCollection<IMyMessages> MyMessages
        {
            get
            {
                return _myMessages;
            }
            set
            {
                if (_myMessages != value)
                {
                    _myMessages = value;
                    RaisePropertyChanged(() => MyMessages);
                }
            }
        }

        public ObservableCollection<IMySkills> MySkills
        {
            get
            {
                return _mySkills;
            }
            set
            {
                if (_mySkills != value)
                {
                    _mySkills = value;
                    RaisePropertyChanged(() => MySkills);
                }
            }
        }

        public ObservableCollection<IMyStatistics> MyStatistics
        {
            get
            {
                return _myStatistics;
            }
            set
            {
                if (_myStatistics != value)
                {
                    _myStatistics = value;
                    RaisePropertyChanged(() => MyStatistics);
                }
            }
        }

        public ObservableCollection<ICallData> NotifyCallData
        {
            get
            {
                return _notifyCallData;
            }
            set
            {
                if (_notifyCallData != value)
                {
                    _notifyCallData = value;
                    NotifyCallDataView = CollectionViewSource.GetDefaultView(_notifyCallData);
                    //RaisePropertyChanged(() => NotifyCallData);
                }
            }
        }

        public ICollectionView NotifyCallDataView
        {
            get
            {
                return _notifyCallDataView;
            }
            set
            {
                if (_notifyCallDataView != value)
                {
                    _notifyCallDataView = value;
                    RaisePropertyChanged(() => NotifyCallDataView);
                }
            }
        }

        public string NotifyGadgetDisplayName
        {
            get { return _notifyGadgetDisplayName; }
            set
            {
                _notifyGadgetDisplayName = value;
                RaisePropertyChanged(() => NotifyGadgetDisplayName);
            }
        }

        public int OpenedCaseWindow
        {
            get
            {
                return _openedCaseWindow;
            }
            set
            {
                if (_openedCaseWindow != value)
                {
                    _openedCaseWindow = value;
                    RaisePropertyChanged(() => OpenedCaseWindow);
                }
            }
        }

        public ContextMenu OptionsMenu
        {
            get
            {
                return _optionsMenu;
            }
            set
            {
                if (_optionsMenu != value)
                {
                    _optionsMenu = value;
                }
            }
        }

        public ContextMenu OutboundContextMenu
        {
            get
            {
                return _outboundContextMenu;
            }
            set
            {
                if (_outboundContextMenu != value)
                {
                    _outboundContextMenu = value;
                }
            }
        }

        public ImageSource OutboundStateImageSource
        {
            get
            {
                return _outboundStateImageSource;
            }
            set
            {
                if (_outboundStateImageSource != value)
                {
                    _outboundStateImageSource = null;
                    _outboundStateImageSource = value;
                    _outboundStateImageSource.Freeze();
                    RaisePropertyChanged(() => OutboundStateImageSource);
                }
            }
        }

        public string Password
        {
            get
            {
                return _password;
            }
            set
            {
                _password = value;
                RaisePropertyChanged(() => Password);
            }
        }

        public string Place
        {
            get
            {
                return _place;
            }
            set
            {
                if (_place != value)
                {
                    _place = value;
                    RaisePropertyChanged(() => Place);
                }
            }
        }

        public ObservableCollection<string> PortItemSource
        {
            get
            {
                return _portItemSource;
            }
            set
            {
                if (_portItemSource != value)
                {
                    _portItemSource = value;
                    RaisePropertyChanged(() => PortItemSource);
                }
            }
        }

        public string PortSelectedValue
        {
            get
            {
                return _portSelectedValue;
            }
            set
            {
                if (_portSelectedValue != value)
                {
                    _portSelectedValue = value;
                    RaisePropertyChanged(() => PortSelectedValue);
                }
            }
        }

        public string PortText
        {
            get
            {
                return _portText;
            }
            set
            {
                if (_portText != value)
                {
                    _portText = value;
                    RaisePropertyChanged(() => PortText);
                }
            }
        }

        public string Queue
        {
            get
            {
                return _queue;
            }
            set
            {
                if (_queue != value)
                {
                    _queue = value;
                    RaisePropertyChanged(() => Queue);
                }
            }
        }

        public string QueueSelectedValue
        {
            get
            {
                return _queueSelectedValue;
            }
            set
            {
                if (_queueSelectedValue != value)
                {
                    _queueSelectedValue = value;
                    RaisePropertyChanged(() => QueueSelectedValue);
                }
            }
        }

        public ImageSource ReadyImageSource
        {
            get
            {
                return _readyImageSource;
            }
            set
            {
                if (_readyImageSource != value)
                {
                    _readyImageSource = null;
                    _readyImageSource = value;
                    _readyImageSource.Freeze();
                    RaisePropertyChanged(() => ReadyImageSource);
                }
            }
        }

        public string ReadyText
        {
            get
            {
                return _readytext;
            }
            set
            {
                if (_readytext != value)
                {
                    _readytext = value;
                    RaisePropertyChanged(() => ReadyText);
                }
            }
        }

        public ImageSource ReConImageSource
        {
            get
            {
                return _reConImageSource;
            }
            set
            {
                if (_reConImageSource != value)
                {
                    _reConImageSource = null;
                    _reConImageSource = value;
                    _reConImageSource.Freeze();
                    RaisePropertyChanged(() => ReConImageSource);
                }
            }
        }

        public string ReConText
        {
            get
            {
                return _reContext;
            }
            set
            {
                if (_reContext != value)
                {
                    _reContext = value;
                    RaisePropertyChanged(() => ReConText);
                }
            }
        }

        public string RequeueComments
        {
            get
            {
                return _requeueComments;
            }
            set
            {
                if (_requeueComments != value)
                {
                    _requeueComments = value;
                    RaisePropertyChanged(() => RequeueComments);
                }
            }
        }

        public ListCollectionView RequeueData
        {
            get
            {
                return _requeueData;
            }
            set
            {
                if (_requeueData != value)
                {
                    _requeueData = value;
                    RaisePropertyChanged(() => RequeueData);
                }
            }
        }

        public System.Windows.Threading.DispatcherTimer RequeueTimer
        {
            get
            {
                return _requeueTimer;
            }
            set
            {
                _requeueTimer = value;
            }
        }

        public string SalesForceText
        {
            get
            {
                return _salesForceText;
            }
            set
            {
                if (_salesForceText != value)
                {
                    _salesForceText = value;
                    RaisePropertyChanged(() => SalesForceText);
                }
            }
        }

        public string SalesForceTextToolTip
        {
            get
            {
                return _salesForceTextToolTip;
            }
            set
            {
                if (_salesForceTextToolTip != value)
                {
                    _salesForceTextToolTip = value;
                    RaisePropertyChanged(() => SalesForceTextToolTip);
                }
            }
        }

        public Languages SelectedLanguage
        {
            get { return _selectedLanguage; }
            set
            {
                _selectedLanguage = value;
                RaisePropertyChanged(() => SelectedLanguage);
            }
        }

        public DropShadowBitmapEffect ShadowEffect
        {
            get
            {
                return _shadowEffect;
            }
            set
            {
                if (_shadowEffect != value)
                {
                    _shadowEffect = value;
                    RaisePropertyChanged(() => ShadowEffect);
                }
            }
        }

        public List<Int32> SkillLevelSource
        {
            get
            {
                return _skillLevelSource;
            }
            set
            {
                if (_skillLevelSource != value)
                {
                    _skillLevelSource = value;
                    RaisePropertyChanged(() => SkillLevelSource);
                }
            }
        }

        public ContextMenu SkillsContextMenu
        {
            get
            {
                return _skillsContextMenu;
            }
            set
            {
                if (_skillsContextMenu != value)
                {
                    _skillsContextMenu = value;
                }
            }
        }

        //Code added by Manikandan on 10-Jan-2013
        //To implement Skills Tab in softphonebar
        public Visibility SkillsTabVisibility
        {
            get
            {
                return _skillsTabVisibility;
            }
            set
            {
                if (_skillsTabVisibility != value)
                {
                    _skillsTabVisibility = value;
                    RaisePropertyChanged(() => SkillsTabVisibility);
                }
            }
        }

        public ContextMenu StatesContextMenu
        {
            get
            {
                return _statesContextMenu;
            }
            set
            {
                if (_statesContextMenu != value)
                {
                    _statesContextMenu = value;
                }
            }
        }

        public string StatusMessage
        {
            get
            {
                return statusMessage;
            }
            set
            {
                if (statusMessage != value)
                {
                    statusMessage = value;
                    RaisePropertyChanged(() => StatusMessage);
                }
            }
        }

        public GridLength StatusMessageHeight
        {
            get
            {
                return statusMessageHeight;
            }
            set
            {
                if (statusMessageHeight != value)
                {
                    statusMessageHeight = value;
                    RaisePropertyChanged(() => StatusMessageHeight);
                }
            }
        }

        public string Subversion
        {
            get { return _subVersion; }
            set
            {
                _subVersion = value;
                RaisePropertyChanged(() => Subversion);
            }
        }

        public int TabSelectedIndex
        {
            get
            {
                return _tabSelectedIndex;
            }
            set
            {
                if (_tabSelectedIndex != value)
                {
                    _tabSelectedIndex = value;
                    RaisePropertyChanged(() => TabSelectedIndex);
                }
            }
        }

        public string TalkImageSource
        {
            get
            {
                return _talkImageSource;
            }
            set
            {
                if (_talkImageSource != value)
                {
                    _talkImageSource = null;
                    _talkImageSource = value;
                    RaisePropertyChanged(() => TalkImageSource);
                }
            }
        }

        public string TalkText
        {
            get
            {
                return _talktext;
            }
            set
            {
                if (_talktext != value)
                {
                    _talktext = value;
                    RaisePropertyChanged(() => TalkText);
                }
            }
        }

        public Visibility TimerEnabled
        {
            get
            {
                return _timerEnabled;
            }
            set
            {
                if (_timerEnabled != value)
                {
                    _timerEnabled = value;
                    RaisePropertyChanged(() => TimerEnabled);
                }
            }
        }

        public Brush TitleBgColor
        {
            get
            {
                return _titleBgColor;
            }
            set
            {
                if (_titleBgColor != value)
                {
                    _titleBgColor = value;
                    RaisePropertyChanged(() => TitleBgColor);
                }
            }
        }

        public string TitleStatusText
        {
            get
            {
                return _titleStatusText;
            }
            set
            {
                if (_titleStatusText != value)
                {
                    _titleStatusText = value;
                    RaisePropertyChanged(() => TitleStatusText);
                }
            }
        }

        public string TitleText
        {
            get
            {
                return _titleText;
            }
            set
            {
                if (_titleText != value)
                {
                    _titleText = value;
                    RaisePropertyChanged(() => TitleText);
                }
            }
        }

        public string TrailMessage
        {
            get
            {
                return _trialMessage;
            }
            set
            {
                if (_trialMessage != value)
                {
                    _trialMessage = value;
                    RaisePropertyChanged(() => TrailMessage);
                }
            }
        }

        public ImageSource TransImageSource
        {
            get
            {
                return _transImageSource;
            }
            set
            {
                if (_transImageSource != value)
                {
                    _transImageSource = null;
                    _transImageSource = value;
                    _transImageSource.Freeze();
                    RaisePropertyChanged(() => TransImageSource);
                }
            }
        }

        public string TransText
        {
            get
            {
                return _transtext;
            }
            set
            {
                if (_transtext != value)
                {
                    _transtext = value;
                    RaisePropertyChanged(() => TransText);
                }
            }
        }

        public Visibility TrialVisibility
        {
            get
            {
                return _trialVisibility;
            }
            set
            {
                if (_trialVisibility != value)
                {
                    _trialVisibility = value;
                    RaisePropertyChanged(() => TrialVisibility);
                }
            }
        }

        public string UnreadMessageCount
        {
            get { return _unreadMessageCount; }
            set
            {
                _unreadMessageCount = value;
                RaisePropertyChanged(() => UnreadMessageCount);
            }
        }

        public string UserLoginID
        {
            get
            {
                return _userLoginID;
            }
            set
            {
                if (_userLoginID != value)
                {
                    _userLoginID = value;
                    RaisePropertyChanged(() => UserLoginID);
                }
            }
        }

        public string UserName
        {
            get
            {
                return _userName;
            }
            set
            {
                if (_userName != value)
                {
                    _userName = value;
                    RaisePropertyChanged(() => UserName);
                }
            }
        }

        public string UserPlace
        {
            get
            {
                return _userPlace;
            }
            set
            {
                if (_userPlace != value)
                {
                    _userPlace = value;
                    RaisePropertyChanged(() => UserPlace);
                }
            }
        }

        public string UserState
        {
            get { return _userState; }
            set
            {
                _userState = value;
                RaisePropertyChanged(() => UserState);
            }
        }

        public string VoiceNotReadyReasonCode
        {
            get
            {
                return _voiceNotReadyReasonCode;
            }
            set
            {
                if (_voiceNotReadyReasonCode != value)
                {
                    _voiceNotReadyReasonCode = value;
                    RaisePropertyChanged(() => VoiceNotReadyReasonCode);
                }
            }
        }

        public ImageSource VoiceStateImageSource
        {
            get
            {
                return _voiceStateImageSource;
            }
            set
            {
                if (_voiceStateImageSource != value)
                {
                    _voiceStateImageSource = null;
                    _voiceStateImageSource = value;
                    _voiceStateImageSource.Freeze();
                    RaisePropertyChanged(() => VoiceStateImageSource);
                }
            }
        }

        public Visibility WorksapceTabVisibility
        {
            get
            {
                return _worksapceTabVisibility;
            }
            set
            {
                if (_worksapceTabVisibility != value)
                {
                    _worksapceTabVisibility = value;
                    RaisePropertyChanged(() => WorksapceTabVisibility);
                }
            }
        }

        public string WrapTime
        {
            get
            {
                return _wrapTime;
            }
            set
            {
                if (_wrapTime != value)
                {
                    _wrapTime = value;
                    RaisePropertyChanged(() => WrapTime);
                }
            }
        }

        public GridLength WrapTimeRowHeight
        {
            get
            {
                return _wrapTimeRowHeight;
            }
            set
            {
                if (_wrapTimeRowHeight != value)
                {
                    _wrapTimeRowHeight = value;
                    RaisePropertyChanged(() => WrapTimeRowHeight);
                }
            }
        }

        #endregion Properties

        #region Methods

        public static Datacontext GetInstance()
        {
            if (_instance == null)
            {
                _instance = new Datacontext();
                return _instance;
            }
            return _instance;
        }

        /// <summary>
        /// Raises the property changed.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="action">The action.</param>
        protected void RaisePropertyChanged<T>(Expression<Func<T>> action)
        {
            var propertyName = GetPropertyName(action);
            RaisePropertyChanged(propertyName);
        }

        /// <summary>
        /// Gets the name of the property.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="action">The action.</param>
        /// <returns></returns>
        private static string GetPropertyName<T>(Expression<Func<T>> action)
        {
            var expression = (MemberExpression)action.Body;
            var propertyName = expression.Member.Name;
            return propertyName;
        }

        /// <summary>
        /// Raises the property changed.
        /// </summary>
        /// <param name="propertyName">Name of the property.</param>
        private void RaisePropertyChanged(string propertyName)
        {
            if (PropertyChanged != null)
                PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
        }

        #endregion Methods

        #region Other

        //public static Dictionary<string, string> voicecodes = new Dictionary<string, string>();
        //public static Dictionary<string, string> chatcodes = new Dictionary<string, string>();
        //public static Dictionary<string, string> emailcodes = new Dictionary<string, string>();
        //public static Dictionary<string, Dictionary<string, string>> voiceSubDict = new Dictionary<string, Dictionary<string, string>>();
        //public static Dictionary<string, Dictionary<string, string>> chatSubDict = new Dictionary<string, Dictionary<string, string>>();
        //public static Dictionary<string, Dictionary<string, string>> emailSubDict = new Dictionary<string, Dictionary<string, string>>();
        //End
        ////Code Added
        ////For Team Communicator
        ////Added Manikandan 25-Nov-2013
        //public string UniqueIdentity
        //{
        //    get
        //    {
        //        return _uniqueIdentity;
        //    }
        //    set
        //    {
        //        if (_uniqueIdentity != value)
        //        {
        //            _uniqueIdentity = value;
        //            RaisePropertyChanged(() => UniqueIdentity);
        //        }
        //    }
        //}
        //public int MaxSuggestionSize
        //{
        //    get
        //    {
        //        return _maxSuggestionSize;
        //    }
        //    set
        //    {
        //        if (_maxSuggestionSize != value)
        //        {
        //            _maxSuggestionSize = value;
        //            RaisePropertyChanged(() => MaxSuggestionSize);
        //        }
        //    }
        //}
        //public int RecentMaxRecords
        //{
        //    get
        //    {
        //        return _recentMaxRecords;
        //    }
        //    set
        //    {
        //        if (_recentMaxRecords != value)
        //        {
        //            _recentMaxRecords = value;
        //            RaisePropertyChanged(() => RecentMaxRecords);
        //        }
        //    }
        //}
        //public List<string> CustomFavoriteList
        //{
        //    get
        //    {
        //        return _customFavoriteList;
        //    }
        //    set
        //    {
        //        if (_customFavoriteList != value)
        //        {
        //            _customFavoriteList = value;
        //            RaisePropertyChanged(() => CustomFavoriteList);
        //        }
        //    }
        //}
        //public List<string> InternalFavoriteList
        //{
        //    get
        //    {
        //        return _internalFavoriteList;
        //    }
        //    set
        //    {
        //        if (_internalFavoriteList != value)
        //        {
        //            _internalFavoriteList = value;
        //            RaisePropertyChanged(() => InternalFavoriteList);
        //        }
        //    }
        //}
        //public List<string> FilterList
        //{
        //    get
        //    {
        //        return _filterList;
        //    }
        //    set
        //    {
        //        if (_filterList != value)
        //        {
        //            _filterList = value;
        //            RaisePropertyChanged(() => FilterList);
        //        }
        //    }
        //}
        //public string RoutingAddress
        //{
        //    get
        //    {
        //        return _routingAddress;
        //    }
        //    set
        //    {
        //        if (_routingAddress != value)
        //        {
        //            _routingAddress = value;
        //            RaisePropertyChanged(() => RoutingAddress);
        //        }
        //    }
        //}
        //public string FavoriteItemSelectedType
        //{
        //    get
        //    {
        //        return _selectedItemType;
        //    }
        //    set
        //    {
        //        if (_selectedItemType != value)
        //        {
        //            _selectedItemType = value;
        //            RaisePropertyChanged(() => FavoriteItemSelectedType);
        //        }
        //    }
        //}
        //public bool IsStatAlive
        //{
        //    get
        //    {
        //        return _isStatAlive;
        //    }
        //    set
        //    {
        //        if (_isStatAlive != value)
        //        {
        //            _isStatAlive = value;
        //            RaisePropertyChanged(() => IsStatAlive);
        //        }
        //    }
        //}
        //public bool IsRequestSent
        //{
        //    get
        //    {
        //        return _isRequestSent;
        //    }
        //    set
        //    {
        //        if (_isRequestSent != value)
        //        {
        //            _isRequestSent = value;
        //            RaisePropertyChanged(() => IsRequestSent);
        //        }
        //    }
        //}
        //public bool IsStatRequestSent
        //{
        //    get
        //    {
        //        return _isStatRequestSent;
        //    }
        //    set
        //    {
        //        if (_isStatRequestSent != value)
        //        {
        //            _isStatRequestSent = value;
        //            RaisePropertyChanged(() => IsStatRequestSent);
        //        }
        //    }
        //}
        //public bool IsEditFavorite
        //{
        //    get
        //    {
        //        return _isEditFavorite;
        //    }
        //    set
        //    {
        //        if (_isEditFavorite != value)
        //        {
        //            _isEditFavorite = value;
        //            RaisePropertyChanged(() => IsEditFavorite);
        //        }
        //    }
        //}
        //public bool IsFavoriteItem
        //{
        //    get
        //    {
        //        return _isNewFavoriteItem;
        //    }
        //    set
        //    {
        //        if (_isNewFavoriteItem != value)
        //        {
        //            _isNewFavoriteItem = value;
        //            RaisePropertyChanged(() => IsFavoriteItem);
        //        }
        //    }
        //}
        //public string FavoriteDisplayName
        //{
        //    get
        //    {
        //        return _favoriteDisplayName;
        //    }
        //    set
        //    {
        //        if (_favoriteDisplayName != value)
        //        {
        //            _favoriteDisplayName = value;
        //            RaisePropertyChanged(() => FavoriteDisplayName);
        //        }
        //    }
        //}
        //public List<string> CategoryNamesList
        //{
        //    get
        //    {
        //        return _categoryNames;
        //    }
        //    set
        //    {
        //        if (_categoryNames != value)
        //        {
        //            _categoryNames = value;
        //            RaisePropertyChanged(() => CategoryNamesList);
        //        }
        //    }
        //}
        //public ObservableCollection<ITeamCommunicator> TeamCommunicator
        //{
        //    get
        //    {
        //        return _myMessage;
        //    }
        //    set
        //    {
        //        if (_myMessage != value)
        //        {
        //            _myMessage = value;
        //            RaisePropertyChanged(() => TeamCommunicator);
        //        }
        //    }
        //}
        //public string InternalTargets
        //{
        //    get
        //    {
        //        return _internalTargets;
        //    }
        //    set
        //    {
        //        if (_internalTargets != value)
        //        {
        //            _internalTargets = value;
        //            RaisePropertyChanged(() => InternalTargets);
        //        }
        //    }
        //}
        //public string SelectedType
        //{
        //    get
        //    {
        //        return _selectedType;
        //    }
        //    set
        //    {
        //        if (_selectedType != value)
        //        {
        //            _selectedType = value;
        //            RaisePropertyChanged(() => SelectedType);
        //        }
        //    }
        //}
        //public bool IsTeamCommunicatorEnabled
        //{
        //    get
        //    {
        //        return _isTeamCommunicatorEnabled;
        //    }
        //    set
        //    {
        //        if (_isTeamCommunicatorEnabled != value)
        //        {
        //            _isTeamCommunicatorEnabled = value;
        //            RaisePropertyChanged(() => IsTeamCommunicatorEnabled);
        //        }
        //    }
        //}
        //public bool IsStatServerClosed
        //{
        //    get
        //    {
        //        return _isStatServerClosed;
        //    }
        //    set
        //    {
        //        if (_isStatServerClosed != value)
        //        {
        //            _isStatServerClosed = value;
        //            RaisePropertyChanged(() => IsStatServerClosed);
        //        }
        //    }
        //}
        //Code Ended

        #endregion Other
    }
}