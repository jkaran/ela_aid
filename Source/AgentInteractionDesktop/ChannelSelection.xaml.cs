﻿namespace Agent.Interaction.Desktop
{
    using System;
    using System.Collections.Generic;
    using System.Configuration;
    using System.Deployment.Application;
    using System.Diagnostics;
    using System.IO;
    using System.Linq;
    using System.Reflection;
    using System.Runtime.InteropServices;
    using System.Windows;
    using System.Windows.Controls;
    using System.Windows.Input;
    using System.Windows.Interop;
    using System.Windows.Media;
    using System.Windows.Media.Effects;
    using System.Windows.Threading;

    using Agent.Interaction.Desktop.ApplicationReader;
    using Agent.Interaction.Desktop.Helpers;
    using Agent.Interaction.Desktop.Settings;

    using Genesyslab.Platform.ApplicationBlocks.ConfigurationObjectModel;
    using Genesyslab.Platform.ApplicationBlocks.ConfigurationObjectModel.CfgObjects;
    using Genesyslab.Platform.ApplicationBlocks.ConfigurationObjectModel.Queries;
    using Genesyslab.Platform.Configuration.Protocols.Types;

    using Pointel.Configuration.Manager;
    using Pointel.Configuration.Manager.Core;
    using Pointel.Integration.PlugIn;
    using Pointel.Integration.PlugIn.Common;

    /// <summary>
    /// Interaction logic for ChannelSelection.xaml
    /// </summary>
    public partial class ChannelSelection : Window
    {
        #region Fields

        public const Int32 MF_BYPOSITION = 0x400;

        private const int CU_Minimize = 1000;
        private const int CU_TopMost = 1001;
        private const int MF_BYCOMMAND = 0x00000000;
        private const int SC_Close = 0x0000f060;
        private const int SC_Maximize = 0x0000f030;
        private const int SC_Minimize = 0x0000f020;
        private const int SC_Move = 0x0000f010;
        private const int SC_Restore = 0x0000f120;
        private const int SC_Size = 0x0000f000;
        private const Int32 WM_SYSCOMMAND = 0x112;

        private Action GCDelegate = delegate()
        {
            GC.Collect();
            GC.WaitForPendingFinalizers();
        };
        private bool IsChangeLoginCalled = false;
        private bool IsChatPluginEnabled = false;
        private bool IsEmailPluginEnabled = false;
        private bool isEnableChatUnactivateChannel = false;
        private bool IsEnableCRMThirdParty = false;
        private bool isEnableEmailUnactivateChannel = false;
        private bool isEnableEpic = false;
        private bool isEnableFacets = false;
        private bool isEnableLawson = false;
        private bool isEnableLDCode = false;
        private bool isEnableLoginQueue = false;
        private bool isEnableMediPac = false;
        private bool isEnableOutboundUnactiveChannel = false;
        private bool isEnableVoiceUnactivateChannel = false;
        private bool IsOutboundPluginEnabled = false;
        private bool isPromptVoiceAgentLoginId = false;
        private bool isPromptVoiceAgentLoginPassword = false;
        private bool IsStatPluginEnabled = false;
        private bool istxtPlaceFocued = false;
        private IntPtr SystemMenu;
        private AgentInfo _agentInfo;
        private string _agentLoginID = string.Empty;
        private List<KeyValuePair<string, string>> _agentLoginIDs = new List<KeyValuePair<string, string>>();
        private int _agentWrapUpTimeMaxValue;
        private CfgSwitch _cfgSwitch;
        private ConfigContainer _configContainer = ConfigContainer.Instance();
        private Datacontext _dataContext = Datacontext.GetInstance();
        private bool _isEnableWrapUpTime = false;
        private bool _isMaxWrapTimeSet;
        private bool _isWrapTimeBeyondMaxValue;
        private Pointel.Logger.Core.ILog _logger = Pointel.Logger.Core.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType,
            "AID");
        private DropShadowBitmapEffect _shadowEffect = new DropShadowBitmapEffect();
        private Dictionary<string, int> _tempAgentLoginIDs;
        private DispatcherTimer _timerforcloseError = null;
        private SoftPhoneBar _view;

        #endregion Fields

        #region Constructors

        /// <summary>
        /// Initializes a new instance of the <see cref="ChannelSelection"/> class.
        /// </summary>
        public ChannelSelection(bool isPlaceEnabled)
        {
            _logger.Info("ChannelSelection Entry");
            InitializeComponent();
            txtPlace.IsEnabled = isPlaceEnabled;
            if (!isPlaceEnabled)
                txtPlace.Text = _dataContext.Place;
            //if (!isPlaceEnabled)
            //    RemoveSwitch_place_agentLogin();
            this.DataContext = Datacontext.GetInstance();
            _dataContext.ChannelSelectionWindow = this;
            _shadowEffect.ShadowDepth = 0;
            _shadowEffect.Opacity = 0.5;
            _shadowEffect.Softness = 0.5;
            _shadowEffect.Color = (Color)ColorConverter.ConvertFromString("#003660");
            _dataContext.CSErrorMessage = string.Empty;
            _dataContext.CSErrorRowHeight = new GridLength(0);
            //   btnCloseError.Visibility = Visibility.Collapsed;
            if (_dataContext.IsCMELoginEnabled)
                btnChangeLogin.Visibility = System.Windows.Visibility.Hidden;
            LoadConfigValues();
            _isMaxWrapTimeSet = int.TryParse(_configContainer.GetAsString("voice.agent.wrapup.time-max-value"), out _agentWrapUpTimeMaxValue);
            _logger.Info("ChannelSelection Exit");
        }

        #endregion Constructors

        #region Enumerations

        enum AgentDetailAsPwd
        {
            manual, username, agentpwd, agentloginid, empid
        }

        #endregion Enumerations

        #region Methods

        public static void ClearClipboard()
        {
            if (!OpenClipboard(System.IntPtr.Zero)) return;
            EmptyClipboard();
            CloseClipboard();
        }

        /// <summary>
        /// Determines whether [is window open] [the specified name].
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="name">The name.</param>
        /// <returns></returns>
        public static T IsWindowOpen<T>(string name = null)
            where T : Window
        {
            var windows = Application.Current.Windows.OfType<T>();
            return string.IsNullOrEmpty(name) ? windows.FirstOrDefault() : windows.FirstOrDefault(w => w.Name.Equals(name));
        }

        public bool CheckPlaceValid(string placeName)
        {
            try
            {
                var placeQuery = new CfgPlaceQuery();
                placeQuery.Name = placeName;
                var place = _configContainer.ConfServiceObject.RetrieveObject<CfgPlace>(placeQuery);
                if (place == null)
                    return false;
                else
                    return true;
                //var dnCollection = (IList<CfgDN>)place.DNs;
                //if (dnCollection != null && dnCollection.Count > 0)
                //{
                //    var directoryNumber = dnCollection.Where(dn => (dn.Name == _dataContext.UserName || dn.Name == null)).FirstOrDefault();
                //    if (directoryNumber == null)
                //    {
                //        txtPlace.IsEnabled = true;
                //        _dataContext.CSErrorMessage = "Unable to login. Place is already taken by " + dnCollection[0].Name + ".";
                //        StartTimerForError();
                //        // _dataContext.CSErrorRowHeight = GridLength.Auto;
                //        return false;
                //    }
                //}
            }
            catch (Exception ex)
            {
                return false;
            }
        }

        public string NumericKeyboardvalue(Key key)
        {
            string value = string.Empty;
            switch (key)
            {
                case Key.D0:
                case Key.NumPad0:
                    value = "0";
                    break;

                case Key.D1:
                case Key.NumPad1:
                    value = "1";
                    break;

                case Key.D2:
                case Key.NumPad2:
                    value = "2";
                    break;

                case Key.D3:
                case Key.NumPad3:
                    value = "3";
                    break;

                case Key.D4:
                case Key.NumPad4:
                    value = "4";
                    break;

                case Key.D5:
                case Key.NumPad5:
                    value = "5";
                    break;

                case Key.D6:
                case Key.NumPad6:
                    value = "6";
                    break;

                case Key.D7:
                case Key.NumPad7:
                    value = "7";
                    break;

                case Key.D8:
                case Key.NumPad8:
                    value = "8";
                    break;

                case Key.D9:
                case Key.NumPad9:
                    value = "9";
                    break;

                default:
                    value = null;
                    break;
            }
            return value;
        }

        public Dictionary<string, string> ReadDNs(string placeName)
        {
            Dictionary<string, string> SelectedPlaceDNs = new Dictionary<string, string>();
            try
            {
                CfgPlaceQuery placeQuery = new CfgPlaceQuery();
                placeQuery.TenantDbid = _configContainer.TenantDbId;
                placeQuery.Name = placeName;
                ICollection<CfgPlace> agentLogin = _configContainer.ConfServiceObject.RetrieveMultipleObjects<CfgPlace>(placeQuery);
                _logger.Debug("Retrieving DN Name");
                foreach (CfgPlace loginCodeDetail in agentLogin)
                {
                    IList<CfgDN> DNCollection = (IList<CfgDN>)loginCodeDetail.DNs;
                    if (DNCollection != null && DNCollection.Count > 0)
                    {
                        foreach (CfgDN DN in DNCollection)
                        {
                            if (DN.Type == CfgDNType.CFGACDPosition)
                            {
                                SelectedPlaceDNs.Add("ACD", DN.Number);
                            }
                            else if (DN.Type == CfgDNType.CFGExtension)
                            {
                                SelectedPlaceDNs.Add("Extension", DN.Number);
                            }
                        }
                    }
                }
            }
            catch (Exception commonException)
            {
                _logger.Error("Error occurred while reading Selected Place DNs " + commonException.ToString());
            }
            return SelectedPlaceDNs;
        }

        internal void LoadConfigValues()
        {
            try
            {
                if (_configContainer.AllKeys.Contains("login.voice.can-unactivate-channel") && _configContainer.GetValue("login.voice.can-unactivate-channel") != null)
                    isEnableVoiceUnactivateChannel = ((string)(_configContainer.GetValue("login.voice.can-unactivate-channel"))).ToLower().Equals("true");

                if (_configContainer.AllKeys.Contains("login.chat.can-unactivate-channel") && _configContainer.GetValue("login.chat.can-unactivate-channel") != null)
                    isEnableChatUnactivateChannel = ((string)(_configContainer.GetValue("login.chat.can-unactivate-channel"))).ToLower().Equals("true");

                if (_configContainer.AllKeys.Contains("login.email.can-unactivate-channel") && _configContainer.GetValue("login.email.can-unactivate-channel") != null)
                    isEnableEmailUnactivateChannel = ((string)(_configContainer.GetValue("login.email.can-unactivate-channel"))).ToLower().Equals("true");

                if (_configContainer.AllKeys.Contains("login.outbound.can-unactivate-channel") && _configContainer.GetValue("login.outbound.can-unactivate-channel") != null)
                    isEnableOutboundUnactiveChannel = ((string)(_configContainer.GetValue("login.outbound.can-unactivate-channel"))).ToLower().Equals("true");

                if (_configContainer.AllKeys.Contains("login.voice.enable.prompt-agent-login-id") &&
                                                    _configContainer.GetValue("login.voice.enable.prompt-agent-login-id") != null)
                    isPromptVoiceAgentLoginId = ((string)(_configContainer.GetValue("login.voice.enable.prompt-agent-login-id"))).ToLower().Equals("true");

                if (_configContainer.AllKeys.Contains("login.voice.enable.prompt-queue") &&
                                                    _configContainer.GetValue("login.voice.enable.prompt-queue") != null)
                    isEnableLoginQueue = ((string)(_configContainer.GetValue("login.voice.enable.prompt-queue"))).ToLower().Equals("true");

                if (_configContainer.AllKeys.Contains("login.voice.enable.prompt-dn-password") &&
                                                    _configContainer.GetValue("login.voice.enable.prompt-dn-password") != null)
                    isPromptVoiceAgentLoginPassword = ((string)(_configContainer.GetValue("login.voice.enable.prompt-dn-password"))).ToLower().Equals("true");

                if (_configContainer.AllKeys.Contains("login.voice.enable.ld-code") && _configContainer.GetValue("login.voice.enable.ld-code") != null)
                    isEnableLDCode = ((string)(_configContainer.GetValue("login.voice.enable.ld-code"))).ToLower().Equals("true");
                if (_configContainer.AllKeys.Contains("voice.enable.modify.agent-wrapup-time") && _configContainer.GetValue("voice.enable.modify.agent-wrapup-time") != null)
                    _isEnableWrapUpTime = ((string)(_configContainer.GetValue("voice.enable.modify.agent-wrapup-time"))).ToLower().Equals("true");

                if (_configContainer.AllKeys.Contains("login.voice.enable-medipac-integration") && _configContainer.GetValue("login.voice.enable-medipac-integration") != null)
                    isEnableMediPac = ((string)(_configContainer.GetValue("login.voice.enable-medipac-integration"))).ToLower().Equals("true");

                if (_configContainer.AllKeys.Contains("login.voice.enable-facets-integration") && _configContainer.GetValue("login.voice.enable-facets-integration") != null)
                    isEnableFacets = ((string)(_configContainer.GetValue("login.voice.enable-facets-integration"))).ToLower().Equals("true");

                if (_configContainer.AllKeys.Contains("login.voice.enable-epic-integration") && _configContainer.GetValue("login.voice.enable-epic-integration") != null)
                    isEnableEpic = ((string)(_configContainer.GetValue("login.voice.enable-epic-integration"))).ToLower().Equals("true");

                if (_configContainer.AllKeys.Contains("login.voice.enable-lawson-integration") && _configContainer.GetValue("login.voice.enable-lawson-integration") != null)
                    isEnableLawson = ((string)(_configContainer.GetValue("login.voice.enable-lawson-integration"))).ToLower().Equals("true");

                if (_configContainer.AllKeys.Contains("chat.enable.plugin") &&
                                                    _configContainer.GetValue("chat.enable.plugin") != null)
                    IsChatPluginEnabled = ((string)(_configContainer.GetValue("chat.enable.plugin"))).ToLower().Equals("true");

                if (_configContainer.AllKeys.Contains("email.enable.plugin") &&
                                                    _configContainer.GetValue("email.enable.plugin") != null)
                    IsEmailPluginEnabled = ((string)(_configContainer.GetValue("email.enable.plugin"))).ToLower().Equals("true");

                if (_configContainer.AllKeys.Contains("outbound.enable.plugin") &&
                                           _configContainer.GetValue("outbound.enable.plugin") != null)
                    IsOutboundPluginEnabled = ((string)(_configContainer.GetValue("outbound.enable.plugin"))).ToLower().Equals("true");

                if (_configContainer.AllKeys.Contains("statistics.enable.plugin") &&
                                                    _configContainer.GetValue("statistics.enable.plugin") != null)
                    IsStatPluginEnabled = ((string)(_configContainer.GetValue("statistics.enable.plugin"))).ToLower().Equals("true");

                if (_configContainer.AllKeys.Contains("crm.third-party-tool") &&
                                                    _configContainer.GetValue("crm.third-party-tool") != null)
                    IsEnableCRMThirdParty = ((string)(_configContainer.GetValue("crm.third-party-tool"))).ToLower().Equals("true");

                if (_configContainer.AllKeys.Contains("enable.outbound-screen-pop") && _configContainer.GetValue("enable.outbound-screen-pop") != null)
                    _dataContext.IsEnableOutboundScreenPop = ((string)(_configContainer.GetValue("enable.outbound-screen-pop"))).ToLower().Equals("true");

                if (_configContainer.AllKeys.Contains("voice.enable.modify.agent-wrapup-time") && _configContainer.GetValue("voice.enable.modify.agent-wrapup-time") != null)
                    _isEnableWrapUpTime = ((string)(_configContainer.GetValue("voice.enable.modify.agent-wrapup-time"))).ToLower().Equals("true");

                if (_configContainer.AllKeys.Contains("CfgPerson") &&
                                                    _configContainer.GetValue("CfgPerson") != null)
                {
                    _dataContext.Person = (CfgPerson)_configContainer.GetValue("CfgPerson");
                    _dataContext.AgentEmployeeID = _dataContext.Person.EmployeeID;

                    string name = "";
                    if (_dataContext.Person.FirstName != null)
                        name += _dataContext.Person.FirstName;

                    if (_dataContext.Person.LastName != null)
                        name += " " + _dataContext.Person.LastName;
                    _dataContext.AgentFullName = name;

                    if (_dataContext.Person != null)
                    {
                        var agentInfo = (ICollection<CfgAgentLoginInfo>)_dataContext.Person.AgentInfo.AgentLogins;
                        _dataContext.AgentID = string.Empty;
                        _dataContext.AgentLoginId = string.Empty;
                        _dataContext.UserLoginID = string.Empty;
                        _dataContext.AgentLoginIds.Clear();
                        _dataContext.AgentID = _dataContext.Person.EmployeeID.ToString();
                        _agentLoginIDs.Clear();
                        foreach (var loginDetails in agentInfo)
                        {
                            //AgentLoginIds.Add(loginDetails.AgentLogin.LoginCode.ToString());
                            if (loginDetails.AgentLogin == null)
                                continue;
                            _dataContext.AgentLoginId = loginDetails.AgentLogin.LoginCode.ToString();
                            _agentLoginIDs.Add(new KeyValuePair<string, string>(loginDetails.AgentLogin.LoginCode.ToString(), loginDetails.AgentLogin.Switch.Name));
                            if (!_dataContext.AgentLoginIds.Contains(loginDetails.AgentLogin.LoginCode.ToString()))
                                _dataContext.AgentLoginIds.Add(loginDetails.AgentLogin.LoginCode.ToString());
                            if (loginDetails.WrapupTime != 0)
                                _dataContext.agentWrapUpTime = loginDetails.WrapupTime;
                        }

                        if (_agentLoginIDs.Count <= 0)
                        {
                            _dataContext.CSErrorMessage = "No agent login available for the voice channel.";
                            StartTimerForError();
                        }
                        _dataContext.CSErrorMessage = string.Empty;

                    }

                    if (_dataContext.Person.AgentInfo.SkillLevels != null && _dataContext.Person.AgentInfo.SkillLevels.Count > 0)
                    {
                        var skillLevels = (ICollection<CfgSkillLevel>)_dataContext.Person.AgentInfo.SkillLevels;
                        if (_configContainer.AllKeys.Contains("enable.my-skills"))
                            if (skillLevels != null && skillLevels.Count > 0)
                            {
                                _dataContext.MySkills.Clear();
                                foreach (CfgSkillLevel skillLevel in skillLevels)
                                    _dataContext.MySkills.Add(new MySkills(skillLevel.Skill.Name.ToString(), skillLevel.Level));
                            }
                            else
                                _dataContext.MySkills.Clear();
                    }
                }

                if (_configContainer.AllKeys.Contains("QueueCollection") &&
                                                    _configContainer.GetValue("QueueCollection") != null)
                {
                    _dataContext.LoadCollection.Clear();
                    _dataContext.LoadCollection.Add("None");
                    foreach (var dn in ((List<string>)_configContainer.GetValue("QueueCollection")).Where(dn => !_dataContext.LoadCollection.Contains(dn)))
                        _dataContext.LoadCollection.Add(dn);
                }
                else
                {
                    _dataContext.LoadCollection.Clear();
                    _dataContext.LoadCollection.Add("None");
                    _logger.Warn("No queues added");
                }
                bool isautocompleteplace = true;
                if (_configContainer.AllKeys.Contains("login.enable-place-completion"))
                    isautocompleteplace = _configContainer.GetAsBoolean("login.enable-place-completion");
                _dataContext.AutoComplete.Clear();
                if (_configContainer.AllKeys.Contains("AllPlaces") &&
                                                    _configContainer.GetValue("AllPlaces") != null && isautocompleteplace)
                {
                    //foreach (var item in _configContainer.GetValue("AllPlaces"))
                    _dataContext.AutoComplete = (_configContainer.GetValue("AllPlaces") as List<string>).ToList();
                    _dataContext.AutoComplete.Sort();
                }
                else
                    _dataContext.AutoComplete = null;
            }
            catch (Exception ex)
            {
                _logger.Error("Error occurred while loading config values " + ex.ToString());
            }
        }

        [DllImport("user32.dll")]
        private static extern bool cInsertMenu(IntPtr hMenu, Int32 wPosition, Int32 wFlags, Int32 wIDNewItem,
            string lpNewItem);

        [System.Runtime.InteropServices.DllImport("user32", SetLastError = true)]
        private static extern bool CloseClipboard();

        [DllImport("user32.dll")]
        private static extern bool DeleteMenu(IntPtr hMenu, int uPosition, int uFlags);

        [System.Runtime.InteropServices.DllImport("user32", SetLastError = true)]
        private static extern bool EmptyClipboard();

        [DllImport("user32.dll")]
        private static extern IntPtr GetSystemMenu(IntPtr hWnd, bool bRevert);

        [System.Runtime.InteropServices.DllImport("user32", SetLastError = true)]
        private static extern bool OpenClipboard(System.IntPtr winHandle);

        /// <summary>
        /// Handles the Click event of the btnCancel control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="RoutedEventArgs"/> instance containing the event data.</param>
        private void btnCancel_Click(object sender, RoutedEventArgs e)
        {
            ClearClipboard();
            Environment.Exit(0);
        }

        /// <summary>
        /// Handles the Click event of the btnChangeLogin control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="RoutedEventArgs"/> instance containing the event data.</param>
        private void btnChangeLogin_Click(object sender, RoutedEventArgs e)
        {
            Process.Start(Assembly.GetExecutingAssembly().Location, "NoSplashScreen");
            IsChangeLoginCalled = true;
            Environment.Exit(0);
        }

        /// <summary>
        /// Handles the Click event of the btnOk control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="RoutedEventArgs"/> instance containing the event data.</param>
        private void btnOk_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                _dataContext.lstHoldingChannelSelection.Clear();
                if (CheckForLogin(true))
                {
                    _logger.Info("ChannelSelection:btnOk_Clicked");
                    string place;
                    if (_dataContext.AutoComplete != null)
                    {
                        place = _dataContext.AutoComplete.Find(s => s.Equals(txtPlace.Text, StringComparison.OrdinalIgnoreCase));
                        _dataContext.Place = place.Trim();
                    }
                    else if (string.IsNullOrEmpty(_dataContext.Place) && CheckPlaceValid(txtPlace.Text))
                        _dataContext.Place = txtPlace.Text.Trim();

                    _logger.Info("Place:" + _dataContext.Place);
                    _logger.Info("Queue:" + _dataContext.Queue);
                    //if (CheckPlaceStatus(_dataContext.Place))
                    //{

                    if (!string.IsNullOrEmpty(_dataContext.Place))
                    {
                        if (_dataContext.QueueSelectedValue != null)
                        {
                            if (_dataContext.QueueSelectedValue.Equals("None"))
                            {
                                _dataContext.QueueSelectedValue = "optional";
                            }
                            else
                                _dataContext.QueueSelectedValue = _dataContext.LoadCollection.Find(s => s.Equals(_dataContext.QueueSelectedValue, StringComparison.OrdinalIgnoreCase));
                        }
                        else
                        {
                            _dataContext.QueueSelectedValue = "optional";
                        }
                        _dataContext.Queue = _dataContext.QueueSelectedValue;
                        _logger.Info("QueueSelected:" + _dataContext.QueueSelectedValue);
                        _logger.Info("ChannelSelection:Create LogFile");

                        var keepChannels = new List<Datacontext.Channels>();
                        if (chkbxVoice.IsChecked == true)
                            keepChannels.Add(Datacontext.Channels.Voice);
                        if (chkbxEmail.IsChecked == true)
                            keepChannels.Add(Datacontext.Channels.Email);
                        if (chkbxChat.IsChecked == true)
                            keepChannels.Add(Datacontext.Channels.Chat);
                        if (chkbxOutbound.IsChecked == true)
                            keepChannels.Add(Datacontext.Channels.OutboundPreview);
                        try
                        {
                            XMLHandler _xmlHandler = new XMLHandler();
                            if (!_dataContext.KeepRecentPlace)
                                _xmlHandler.ModifyXmlData(_dataContext.SettingsXMLFile, _xmlHandler.ConfigKeys[XMLHandler.Keys.Place].ToString(), _dataContext.Place);
                            _xmlHandler.ModifyXmlData(_dataContext.SettingsXMLFile, _xmlHandler.ConfigKeys[XMLHandler.Keys.Queueselection].ToString(), _dataContext.QueueSelectedValue.ToString());
                            if (keepChannels != null && keepChannels.Count > 0)
                            {
                                string channels = string.Empty;
                                foreach (var item in keepChannels)
                                {
                                    channels += item.ToString() + ",";
                                }
                                channels = channels.Substring(0, channels.Length - 1);
                                if (!string.IsNullOrEmpty(channels))
                                    _xmlHandler.ModifyXmlData(_dataContext.SettingsXMLFile, _xmlHandler.ConfigKeys[XMLHandler.Keys.KeepChannels].ToString(), channels);
                            }
                            _logger.Info("ChannelSelection:Updated Config file");
                        }
                        catch (Exception ex)
                        {
                            _logger.Error("ChannelSelection : Exception while creating Config File " + ex.ToString());
                        }

                        _dataContext.channelCollection.Add("Voice");

                        //check for same switch in place and login id match
                        if (CheckSwitch_place_agentLogin())
                        {
                            if (_dataContext.IsVoiceChecked)
                            {
                                if (!_dataContext.lstHoldingChannelSelection.ContainsKey(chkbxVoice.Content.ToString()))
                                {
                                    _dataContext.lstHoldingChannelSelection.Add(chkbxVoice.Content.ToString(), chkbxVoice.IsChecked.Value);
                                }
                                else if (!_dataContext.lstHoldingChannelSelection.ContainsValue(chkbxVoice.IsChecked.Value))
                                {
                                    _dataContext.lstHoldingChannelSelection[chkbxVoice.Content.ToString()] = chkbxVoice.IsChecked.Value;
                                }
                            }
                            if (_dataContext.IsEmailChecked)
                            {
                                if (!_dataContext.lstHoldingChannelSelection.ContainsKey(chkbxEmail.Content.ToString()))
                                {
                                    _dataContext.lstHoldingChannelSelection.Add(chkbxEmail.Content.ToString(), chkbxEmail.IsChecked.Value);
                                }
                                else if (!_dataContext.lstHoldingChannelSelection.ContainsValue(chkbxEmail.IsChecked.Value))
                                {
                                    _dataContext.lstHoldingChannelSelection[chkbxEmail.Content.ToString()] = chkbxEmail.IsChecked.Value;
                                }
                            }
                            else
                            {
                                _dataContext.lstHoldingChannelSelection.Add(chkbxEmail.Content.ToString(), chkbxEmail.IsChecked.Value);
                            }
                            //Check chat checkbox is checked
                            if (_dataContext.IsChatChecked)
                            {
                                if (!_dataContext.lstHoldingChannelSelection.ContainsKey(chkbxChat.Content.ToString()))
                                {
                                    _dataContext.lstHoldingChannelSelection.Add(chkbxChat.Content.ToString(), chkbxChat.IsChecked.Value);
                                }
                                else if (!_dataContext.lstHoldingChannelSelection.ContainsValue(chkbxChat.IsChecked.Value))
                                {
                                    _dataContext.lstHoldingChannelSelection[chkbxChat.Content.ToString()] = chkbxChat.IsChecked.Value;
                                }
                            }
                            else
                            {
                                _dataContext.lstHoldingChannelSelection.Add(chkbxChat.Content.ToString(), chkbxChat.IsChecked.Value);
                            }
                            if (_dataContext.IsOutboundChecked)
                            {
                                if (!_dataContext.lstHoldingChannelSelection.ContainsKey(chkbxOutbound.Content.ToString()))
                                {
                                    _dataContext.lstHoldingChannelSelection.Add(chkbxOutbound.Content.ToString(), chkbxOutbound.IsChecked.Value);
                                }
                                else if (!_dataContext.lstHoldingChannelSelection.ContainsValue(chkbxOutbound.IsChecked.Value))
                                {
                                    _dataContext.lstHoldingChannelSelection[chkbxOutbound.Content.ToString()] = chkbxOutbound.IsChecked.Value;
                                }
                            }
                            else
                                _dataContext.lstHoldingChannelSelection.Add(chkbxOutbound.Content.ToString(), chkbxOutbound.IsChecked.Value);

                            _logger.Debug("Reading selected DN.");
                            try
                            {
                                _dataContext.SwitchType = _cfgSwitch;
                                _dataContext.SwitchName = _dataContext.SwitchType.Type == CfgSwitchType.CFGLucentDefinityG3 ? "avaya" : ((_dataContext.SwitchType.Type == CfgSwitchType.CFGNortelDMS100 || _dataContext.SwitchType.Type == CfgSwitchType.CFGNortelMeridianCallCenter) ? "nortel" : "avaya");
                                (new Pointel.Configuration.Manager.ConfigManager()).ReadAllDns((int)_cfgSwitch.DBID);

                                if (_configContainer.AllKeys.Contains("ForwardDns") &&
                                                                    _configContainer.GetValue("ForwardDns") != null)
                                {
                                    //ForwardDNs = _configContainer.GetValue("ForwardDns");
                                    _dataContext.ForwardDns.Clear();
                                    foreach (var item in _configContainer.GetValue("ForwardDns"))
                                        _dataContext.ForwardDns.Add(item);
                                    _dataContext.ForwardDns.Sort();
                                }
                            }
                            catch (Exception ex)
                            {
                                _logger.Warn("Error occurred while setting switch type and read dn's for forward. " + ex.Message);
                            }

                            if (Datacontext.DNsCollection.Count > 0)
                            {
                                if (Datacontext.DNsCollection.ContainsKey("Extension"))
                                {
                                    if (_dataContext.ForwardDns.Contains(Datacontext.DNsCollection["Extension"].ToString()))
                                        _dataContext.ForwardDns.Remove(Datacontext.DNsCollection["Extension"].ToString());
                                }
                                else if (Datacontext.DNsCollection.ContainsKey("ACD"))
                                {
                                    if (_dataContext.ForwardDns.Contains(Datacontext.DNsCollection["ACD"].ToString()))
                                        _dataContext.ForwardDns.Remove(Datacontext.DNsCollection["ACD"].ToString());
                                }
                            }
                            if (txtAgentPassword.Password.ToString() == "$AgentLoginID$")
                                txtAgentPassword.Password = _dataContext.AgentLoginId;
                            _dataContext.AgentPassword = txtAgentPassword.Password.ToString();
                            _dataContext.LongDistanceCode = txtLDCode.Password.ToString();

                            if (!Datacontext.AvailableServerDic.ContainsValue(CfgAppType.CFGChatServer.ToString()))
                            {
                                if (_dataContext.lstHoldingChannelSelection.ContainsKey(chkbxChat.Content.ToString()))
                                    _dataContext.lstHoldingChannelSelection.Remove(chkbxChat.Content.ToString());
                            }

                            if (!Datacontext.AvailableServerDic.ContainsValue(CfgAppType.CFGInteractionServer.ToString()))
                            {
                                if (_dataContext.lstHoldingChannelSelection.ContainsKey(chkbxChat.Content.ToString()))
                                    _dataContext.lstHoldingChannelSelection.Remove(chkbxChat.Content.ToString());
                                if (_dataContext.lstHoldingChannelSelection.ContainsKey(chkbxEmail.Content.ToString()))
                                    _dataContext.lstHoldingChannelSelection.Remove(chkbxEmail.Content.ToString());
                            }

                            try
                            {
                                #region 4232323232323 party CRM plugin

                                if (IsEnableCRMThirdParty)
                                {
                                    if (_configContainer.ConfServiceObject == null) return;
                                    InitializeThirdPartyIntegration(_configContainer.ConfServiceObject);
                                }

                                #endregion 4232323232323 party CRM plugin
                            }
                            catch (Exception ex)
                            {
                                _logger.Error("While loading Third party CRM plugin:" + ex.ToString());
                            }

                            cbQueue.Text = cbQueue.SelectedItem != null ? cbQueue.SelectedItem.ToString() : cbQueue.Items[0].ToString();

                            //Code added by Elango.T, on 11/01/2016
                            //Update Agent ID Wrap Time
                            bool closewindow = true;
                            if (_isEnableWrapUpTime)
                            {
                                if (string.IsNullOrEmpty(_dataContext.WrapTime))
                                    _dataContext.WrapTime = _dataContext.agentWrapUpTime.ToString();
                                if (_isMaxWrapTimeSet && Convert.ToInt32(_dataContext.WrapTime) > _agentWrapUpTimeMaxValue)
                                {
                                    using (MessageBox msgBox = new MessageBox("Warning", "Wrap-up Time is limited to " + _agentWrapUpTimeMaxValue.ToString() +
                                        " second . Click continue to proceed with new value or click edit to change.", "_Continue", "_Edit", false))
                                    {
                                        if (msgBox.ShowDialog() == true)
                                        {
                                            getAgentWrapUpTime(_dataContext.AgentLoginId, true);
                                        }
                                        else
                                            closewindow = false;
                                        goto CloseWindow;
                                    }
                                }
                                getAgentWrapUpTime(_dataContext.AgentLoginId, true);
                            }

                            #region ElavonLogin

                        CloseWindow:
                            if (closewindow)
                            {
                                var showorhideapps = ConfigContainer.Instance().GetAsString("thirdparty.integerations.apps");
                                if (!string.IsNullOrEmpty(showorhideapps.Trim()) && showorhideapps != "No key found")
                                {
                                    this.Hide();
                                    (new GvasLogin(this)).Show();
                                }
                                else
                                    this.Close();
                            }

                            #endregion ElavonLogin
                        }
                    }
                }
            }
            catch (Exception commonException)
            {
                _logger.Error("channelSelection:" + commonException.ToString());
            }
        }

        private void cbAgentLoginID_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (e.AddedItems.Count > 0)
            {
                _dataContext.AgentLoginId = e.AddedItems[0].ToString();
                _dataContext.UserLoginID = "Login ID " + e.AddedItems[0].ToString();
                if (_isEnableWrapUpTime)
                    _dataContext.WrapTime = getAgentWrapUpTime(e.AddedItems[0].ToString(), false);
            }
            else
            {
                _dataContext.AgentLoginId = string.Empty;
                _dataContext.UserLoginID = string.Empty;
                _dataContext.WrapTime = string.Empty;
            }
            if (!string.IsNullOrEmpty(_dataContext.AgentLoginId))
                _agentLoginID = _dataContext.AgentLoginId;
        }

        private void cbQueue_KeyUp(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
                btnOk_Click(null, null);
        }

        private void cbQueue_PreviewKeyUp(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
                btnOk_Click(null, null);
        }

        private void channelWindow_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            //this.DataContext = null;
            if (_timerforcloseError != null)
            {
                if (_timerforcloseError.IsEnabled)
                    _timerforcloseError.Stop();
                _timerforcloseError = null;
            }
            chkbxEmail.Checked -= chkbxEmail_Checked;
            chkbxEmail.Unchecked -= chkbxEmail_Unchecked;
            chkbxChat.Checked -= chkbxChat_Checked;
            chkbxChat.Unchecked -= chkbxChat_Unchecked;
            chkbxOutbound.Unchecked -= chkbxOutbound_Unchecked;
            chkbxOutbound.Checked -= chkbxOutbound_Checked;
            txtPlace.SelectionChanged -= txtPlace_SelectionChanged;
            txtPlace.TextChanged -= txtPlace_TextChanged;
            cbAgentLoginID.SelectionChanged -= cbAgentLoginID_SelectionChanged;
        }

        /// <summary>
        /// Handles the StateChanged event of the channelWindow control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="EventArgs"/> instance containing the event data.</param>
        private void channelWindow_StateChanged(object sender, EventArgs e)
        {
            WindowState = System.Windows.WindowState.Normal;
        }

        /// <summary>
        /// Handles the Unloaded event of the channelWindow control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="RoutedEventArgs"/> instance containing the event data.</param>
        private void channelWindow_Unloaded(object sender, RoutedEventArgs e)
        {
            try
            {
                if (!IsChangeLoginCalled)
                {
                    var splashScreen = new SplashScreen(System.Reflection.Assembly.GetExecutingAssembly(), "Images/SplashScreen.Loding.png");
                    splashScreen.Show(true);
                    _view = new SoftPhoneBar();
                    var login = IsWindowOpen<Window>("loginWindow");
                    if (login != null && login is Login)
                    {
                        login.Close();
                        login = null;
                    }
                    try
                    {
                        //StatisticsListener listener = new StatisticsListener();
                        //if (IsStatPluginEnabled && _dataContext.IsStatPluginAdded)
                        //{
                        //    _dataContext.IsStatTickerLoaded = listener.InitializeStatTickerPlugin();
                        //}
                        ////if (_dataContext.IsStatTickerLoaded)
                        //{
                        //    //Load statistics directly
                        //    listener.InitialiseStatTicker(listener);
                        //}
                    }
                    catch (Exception generalException) { _logger.Error("Error occurred while initializing statistics : " + generalException.ToString()); }
                    _view.Show();

                    try
                    {
                        _shadowEffect = null;
                        _agentLoginIDs = null;
                        _agentInfo = null;
                        _dataContext = null;
                        _configContainer = null;
                    }
                    catch { }

                    _view.ContentRendered += new EventHandler(view_ContentRendered);
                }
                else
                    IsChangeLoginCalled = false;
            }
            catch (Exception generalException)
            {
                _logger.Error("Error occurred while channelWindow_Unloaded() :" + generalException.ToString());
            }
        }

        private void chbxEpic_Checked(object sender, RoutedEventArgs e)
        {
            try
            {
                if (!_dataContext.lstHoldingChannelSelection.ContainsKey(chkbxEpic.Tag.ToString()))
                {
                    _dataContext.lstHoldingChannelSelection.Add(chkbxEpic.Tag.ToString(), chkbxEpic.IsChecked != null && chkbxEpic.IsChecked.Value);
                }
            }
            catch (Exception commonException)
            {
                _logger.Error("ChannelSelection:chbxEpic_Checked:" + commonException.ToString());
            }
        }

        private void chbxEpic_Unchecked(object sender, RoutedEventArgs e)
        {
            try
            {
                if (_dataContext.lstHoldingChannelSelection.ContainsKey(chkbxEpic.Tag.ToString()))
                {
                    _dataContext.lstHoldingChannelSelection.Remove(chkbxEpic.Tag.ToString());
                }
            }
            catch (Exception commonException)
            {
                _logger.Error("ChannelSelection:chbxEpic_Unchecked:" + commonException.ToString());
            }
        }

        private void chbxFacets_Checked(object sender, RoutedEventArgs e)
        {
            try
            {
                if (!_dataContext.lstHoldingChannelSelection.ContainsKey(chkbxFacets.Tag.ToString()))
                {
                    _dataContext.lstHoldingChannelSelection.Add(chkbxFacets.Tag.ToString(), chkbxFacets.IsChecked != null && chkbxFacets.IsChecked.Value);
                }
            }
            catch (Exception commonException)
            {
                _logger.Error("ChannelSelection:chbxFacets_Checked:" + commonException.ToString());
            }
        }

        private void chbxFacets_Unchecked(object sender, RoutedEventArgs e)
        {
            try
            {
                if (_dataContext.lstHoldingChannelSelection.ContainsKey(chkbxFacets.Tag.ToString()))
                {
                    _dataContext.lstHoldingChannelSelection.Remove(chkbxFacets.Tag.ToString());
                }
            }
            catch (Exception commonException)
            {
                _logger.Error("ChannelSelection:chbxFacets_Unchecked:" + commonException.ToString());
            }
        }

        private void chbxLawson_Checked(object sender, RoutedEventArgs e)
        {
            try
            {
                if (!_dataContext.lstHoldingChannelSelection.ContainsKey(chkbxLawson.Tag.ToString()))
                {
                    _dataContext.lstHoldingChannelSelection.Add(chkbxLawson.Tag.ToString(), chkbxLawson.IsChecked != null && chkbxLawson.IsChecked.Value);
                }
            }
            catch (Exception commonException)
            {
                _logger.Error("ChannelSelection:chbxLawson_Checked:" + commonException.ToString());
            }
        }

        private void chbxLawson_Unchecked(object sender, RoutedEventArgs e)
        {
            try
            {
                if (_dataContext.lstHoldingChannelSelection.ContainsKey(chkbxLawson.Tag.ToString()))
                {
                    _dataContext.lstHoldingChannelSelection.Remove(chkbxLawson.Tag.ToString());
                }
            }
            catch (Exception commonException)
            {
                _logger.Error("ChannelSelection:chbxLawson_Unchecked:" + commonException.ToString());
            }
        }

        private void chbxMediPac_Checked(object sender, RoutedEventArgs e)
        {
            try
            {
                if (!_dataContext.lstHoldingChannelSelection.ContainsKey(chkbxMediPac.Tag.ToString()))
                {
                    _dataContext.lstHoldingChannelSelection.Add(chkbxMediPac.Tag.ToString(), chkbxMediPac.IsChecked != null && chkbxMediPac.IsChecked.Value);
                }
            }
            catch (Exception commonException)
            {
                _logger.Error("ChannelSelection:chbxMediPac_Checked:" + commonException.ToString());
            }
        }

        private void chbxMediPac_Unchecked(object sender, RoutedEventArgs e)
        {
            try
            {
                if (_dataContext.lstHoldingChannelSelection.ContainsKey(chkbxMediPac.Tag.ToString()))
                {
                    _dataContext.lstHoldingChannelSelection.Remove(chkbxMediPac.Tag.ToString());
                }
            }
            catch (Exception commonException)
            {
                _logger.Error("ChannelSelection:chbxMediPac_Unchecked:" + commonException.ToString());
            }
        }

        private bool CheckForLogin(bool isLogEnabled)
        {
            try
            {
                if (txtPlace.Text == string.Empty)
                {
                    if (isLogEnabled)
                    {
                        _dataContext.CSErrorMessage = "Unable to login. Place can not be empty.";
                        StartTimerForError();
                        //_dataContext.CSErrorRowHeight = GridLength.Auto;
                        txtPlace.IsEnabled = true;
                        // btnOk.IsEnabled = false;
                    }
                    return false;
                }
                if (_dataContext.AutoComplete == null)
                {
                    if (!CheckPlaceValid(txtPlace.Text))
                    {
                        if (isLogEnabled)
                        {
                            _dataContext.CSErrorMessage = "Unable to login. There is no such place configured.";
                            StartTimerForError();
                            // _dataContext.CSErrorRowHeight = GridLength.Auto;
                            txtPlace.IsEnabled = true;
                            //btnOk.IsEnabled = false;
                        }
                        return false;
                    }

                }
                else if (_dataContext.AutoComplete.FindIndex(x => x.Equals(txtPlace.Text, StringComparison.OrdinalIgnoreCase)) == -1)
                {
                    if (isLogEnabled)
                    {
                        _dataContext.CSErrorMessage = "Unable to login. There is no such place configured.";
                        StartTimerForError();
                        // _dataContext.CSErrorRowHeight = GridLength.Auto;
                        txtPlace.IsEnabled = true;
                        //btnOk.IsEnabled = false;
                    }
                    return false;
                }
                if (isEnableVoiceUnactivateChannel && _dataContext.IsVoiceChecked)
                {
                    string tempQueue = cbQueue.SelectedItem == null ? cbQueue.Text : cbQueue.SelectedItem.ToString();
                    if (tempQueue == string.Empty)
                    {
                        if (isLogEnabled)
                        {
                            _dataContext.CSErrorMessage = "Unable to login. Queue can not be empty.";
                            //_dataContext.CSErrorRowHeight = GridLength.Auto;
                            StartTimerForError();
                            //btnOk.IsEnabled = false;
                        }
                        return false;
                    }
                    if (_dataContext.LoadCollection.FindIndex(x => x.Equals(tempQueue, StringComparison.OrdinalIgnoreCase)) == -1)
                    {
                        if (isLogEnabled)
                        {
                            _dataContext.CSErrorMessage = "Unable to login. There is no such queue configured.";
                            //_dataContext.CSErrorRowHeight = GridLength.Auto;
                            StartTimerForError();
                            //btnOk.IsEnabled = false;
                        }
                        return false;
                    }
                }
                if (_dataContext.AgentLoginId == string.Empty)
                {
                    _dataContext.CSErrorMessage = "Unable to login. There is no agent login id configured for selected switch type.";
                    // _dataContext.CSErrorRowHeight = GridLength.Auto;
                    StartTimerForError();
                    return false;
                }
                _dataContext.CSErrorMessage = string.Empty;
                //_dataContext.CSErrorRowHeight = new GridLength(0);
                return true;
            }
            catch
            {
                return false;
            }
        }

        /// <summary>
        /// Checks the switch_place_agent login.
        /// </summary>
        /// <returns></returns>
        private bool CheckSwitch_place_agentLogin()
        {
            try
            {
                _logger.Info("Checking Switch from place & agentLogin");
                Genesyslab.Platform.ApplicationBlocks.ConfigurationObjectModel.Queries.CfgPlaceQuery _cfgPlaceQuery = new Genesyslab.Platform.ApplicationBlocks.ConfigurationObjectModel.Queries.CfgPlaceQuery();
                _cfgPlaceQuery.Name = _dataContext.Place;
                _cfgPlaceQuery.TenantDbid = _configContainer.TenantDbId;
                Genesyslab.Platform.ApplicationBlocks.ConfigurationObjectModel.CfgObjects.CfgPlace _cfgplace = _configContainer.ConfServiceObject.RetrieveObject<Genesyslab.Platform.ApplicationBlocks.ConfigurationObjectModel.CfgObjects.CfgPlace>(_cfgPlaceQuery);
                ICollection<Genesyslab.Platform.ApplicationBlocks.ConfigurationObjectModel.CfgObjects.CfgDN> dn = _cfgplace.DNs;

                int dnDBID = 0;

                foreach (var item in dn)
                {
                    if (_dataContext.ThisDN == item.Number)
                    {
                        dnDBID = item.DBID;
                        break;
                    }
                }

                Genesyslab.Platform.ApplicationBlocks.ConfigurationObjectModel.Queries.CfgDNQuery _cfgDNQuery = new Genesyslab.Platform.ApplicationBlocks.ConfigurationObjectModel.Queries.CfgDNQuery();
                _cfgDNQuery.DnNumber = _dataContext.ThisDN;
                _cfgDNQuery.Dbid = dnDBID;
                _cfgDNQuery.TenantDbid = _configContainer.TenantDbId;
                Genesyslab.Platform.ApplicationBlocks.ConfigurationObjectModel.CfgObjects.CfgDN _cfgDN = _configContainer.ConfServiceObject.RetrieveObject<Genesyslab.Platform.ApplicationBlocks.ConfigurationObjectModel.CfgObjects.CfgDN>(_cfgDNQuery);

                Genesyslab.Platform.ApplicationBlocks.ConfigurationObjectModel.Queries.CfgAgentLoginQuery _cfgAgentLoginQuery = new Genesyslab.Platform.ApplicationBlocks.ConfigurationObjectModel.Queries.CfgAgentLoginQuery();
                _cfgAgentLoginQuery.LoginCode = _dataContext.AgentLoginId != string.Empty ? _dataContext.AgentLoginId : cbAgentLoginID.Text;
                _cfgAgentLoginQuery.TenantDbid = _configContainer.TenantDbId;
                var agentInfo = (ICollection<Genesyslab.Platform.ApplicationBlocks.ConfigurationObjectModel.CfgObjects.CfgAgentLoginInfo>)_dataContext.Person.AgentInfo.AgentLogins;
                foreach (var loginDetails in agentInfo)
                {
                    if (_tempAgentLoginIDs.Values.Contains(loginDetails.AgentLogin.DBID))
                    {
                        _cfgAgentLoginQuery.Dbid = loginDetails.AgentLogin.DBID;
                        break;
                    }
                }
                Genesyslab.Platform.ApplicationBlocks.ConfigurationObjectModel.CfgObjects.CfgAgentLogin _cfgAgentLogin = _configContainer.ConfServiceObject.RetrieveObject<Genesyslab.Platform.ApplicationBlocks.ConfigurationObjectModel.CfgObjects.CfgAgentLogin>(_cfgAgentLoginQuery);
                if (_cfgDN.Switch.Name != _cfgAgentLogin.Switch.Name)
                {
                    _dataContext.CSErrorMessage = "Unable to login. Switch is not same for place and login id.";
                    StartTimerForError();
                    //_dataContext.CSErrorRowHeight = GridLength.Auto;
                    return false;
                }
                else
                    _cfgSwitch = _cfgAgentLogin.Switch;
                return true;
            }
            catch (Exception ex)
            {
                _logger.Error("Checking Switch from place & agentLogin : " + ex.Message);
                return false;
            }
        }

        private void chkbxChat_Checked(object sender, RoutedEventArgs e)
        {
            try
            {
                bool a = chkbxChat.IsChecked != null && chkbxChat.IsChecked.Value;
                if (!_dataContext.lstHoldingChannelSelection.ContainsKey(chkbxChat.Content.ToString()))
                {
                    _dataContext.lstHoldingChannelSelection.Add(chkbxChat.Content.ToString(), chkbxChat.IsChecked != null && chkbxChat.IsChecked.Value);
                }
            }
            catch (Exception commonException)
            {
                _logger.Error("ChannelSelection:chkbxChat_Checked:" + commonException.ToString());
            }
        }

        private void chkbxChat_Unchecked(object sender, RoutedEventArgs e)
        {
            try
            {
                if (_dataContext.lstHoldingChannelSelection.ContainsKey(chkbxChat.Content.ToString()))
                {
                    _dataContext.lstHoldingChannelSelection.Remove(chkbxChat.Content.ToString());
                }
            }
            catch (Exception commonException)
            {
                _logger.Error("ChannelSelection:chkbxChat_Unchecked:" + commonException.ToString());
            }
        }

        private void chkbxEmail_Checked(object sender, RoutedEventArgs e)
        {
            try
            {
                bool a = chkbxEmail.IsChecked != null && chkbxEmail.IsChecked.Value;
                if (!_dataContext.lstHoldingChannelSelection.ContainsKey(chkbxEmail.Content.ToString()))
                {
                    _dataContext.lstHoldingChannelSelection.Add(chkbxEmail.Content.ToString(), chkbxEmail.IsChecked != null && chkbxEmail.IsChecked.Value);
                }
            }
            catch (Exception commonException)
            {
                _logger.Error("ChannelSelection:chkbxEmail_Checked:" + commonException.ToString());
            }
        }

        private void chkbxEmail_Unchecked(object sender, RoutedEventArgs e)
        {
            try
            {
                if (_dataContext.lstHoldingChannelSelection.ContainsKey(chkbxEmail.Content.ToString()))
                {
                    _dataContext.lstHoldingChannelSelection.Remove(chkbxEmail.Content.ToString());
                }
            }
            catch (Exception commonException)
            {
                _logger.Error("ChannelSelection:chkbxEmail_Unchecked:" + commonException.ToString());
            }
        }

        private void chkbxOutbound_Checked(object sender, RoutedEventArgs e)
        {
            try
            {
                bool a = chkbxOutbound.IsChecked != null && chkbxOutbound.IsChecked.Value;
                if (!_dataContext.lstHoldingChannelSelection.ContainsKey(chkbxOutbound.Content.ToString()))
                {
                    _dataContext.lstHoldingChannelSelection.Add(chkbxOutbound.Content.ToString(), chkbxOutbound.IsChecked != null && chkbxOutbound.IsChecked.Value);
                }
            }
            catch (Exception commonException)
            {
                _logger.Error("ChannelSelection:chkbxOutbound_Checked:" + commonException.ToString());
            }
        }

        private void chkbxOutbound_Unchecked(object sender, RoutedEventArgs e)
        {
            try
            {
                if (_dataContext.lstHoldingChannelSelection.ContainsKey(chkbxOutbound.Content.ToString()))
                {
                    _dataContext.lstHoldingChannelSelection.Remove(chkbxOutbound.Content.ToString());
                }
            }
            catch (Exception commonException)
            {
                _logger.Error("ChannelSelection:chkbxOutbound_Unchecked:" + commonException.ToString());
            }
        }

        /// <summary>
        /// Handles the Checked event of the chkbxVoice control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="RoutedEventArgs"/> instance containing the event data.</param>
        private void chkbxVoice_Checked(object sender, RoutedEventArgs e)
        {
            try
            {
                brdHide.Visibility = System.Windows.Visibility.Collapsed;
                lblQueue.IsEnabled = true;
                cbQueue.IsEnabled = true;
                lblAgentLogin.IsEnabled = true;
                lblAgentLoginID.IsEnabled = true;
                cbAgentLoginID.IsEnabled = true;
                lblAgentPassword.IsEnabled = true;
                txtAgentPassword.IsEnabled = true;
                lblLDCode.IsEnabled = true;
                txtLDCode.IsEnabled = true;
                if (txtPlace.Text != string.Empty && cbQueue.Text != string.Empty && isPromptVoiceAgentLoginPassword)
                    txtAgentPassword.Focus();
                if (_dataContext == null) return;
                if (_dataContext.lstHoldingChannelSelection != null && !_dataContext.lstHoldingChannelSelection.ContainsKey(chkbxVoice.Content.ToString()))
                {
                    _dataContext.lstHoldingChannelSelection.Add(chkbxVoice.Content.ToString(), chkbxVoice.IsChecked != null && chkbxVoice.IsChecked.Value);
                }
            }
            catch (Exception commonException)
            {
                _logger.Error("ChannelSelection:chkbxVoice_Checked:" + commonException.ToString());
            }
        }

        //end
        /// <summary>
        /// Handles the Unchecked event of the chkbxVoice control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="RoutedEventArgs"/> instance containing the event data.</param>
        private void chkbxVoice_Unchecked(object sender, RoutedEventArgs e)
        {
            try
            {
                brdHide.Visibility = System.Windows.Visibility.Visible;
                lblQueue.IsEnabled = false;
                cbQueue.IsEnabled = false;
                lblAgentLogin.IsEnabled = false;
                lblAgentLoginID.IsEnabled = false;
                cbAgentLoginID.IsEnabled = false;
                lblAgentPassword.IsEnabled = false;
                txtAgentPassword.IsEnabled = false;
                lblLDCode.IsEnabled = false;
                txtLDCode.IsEnabled = false;
                if (_dataContext == null) return;
                if (_dataContext.lstHoldingChannelSelection != null && _dataContext.lstHoldingChannelSelection.ContainsKey(chkbxVoice.Content.ToString()))
                {
                    _dataContext.lstHoldingChannelSelection.Remove(chkbxVoice.Content.ToString());
                }
            }
            catch (Exception commonException)
            {
                _logger.Error("ChannelSelection:chkbxVoice_Unchecked:" + commonException.ToString());
            }
        }

        private void CloseError(object sender, EventArgs e)
        {
            try
            {
                if (_dataContext != null)
                {
                    _dataContext.CSErrorRowHeight = new GridLength(0);
                    //btnCloseError.Visibility = Visibility.Collapsed;
                    bool isautocompleteplace = true;
                    if (_configContainer.AllKeys.Contains("login.enable-place-completion"))
                        isautocompleteplace = _configContainer.GetAsBoolean("login.enable-place-completion");

                    if (!isautocompleteplace)
                    {
                        if (CheckPlaceValid(txtPlace.Text))
                            goto Enable;
                        else
                            goto Escape;
                    }

                Enable:
                    btnOk.IsEnabled = true;

                Escape:
                    if (_timerforcloseError != null)
                    {
                        _timerforcloseError.Stop();
                        _timerforcloseError.Tick -= CloseError;
                        _timerforcloseError = null;
                    }
                }
            }
            catch (Exception ex)
            {
                _logger.Error("Error occurred as  " + ex.Message);
            }
        }

        private string GetAgentLoginIDPassword(string ADPwd)
        {
            if (ADPwd == "No key found")
                return "";
            AgentDetailAsPwd password;

            Enum.TryParse<AgentDetailAsPwd>(ADPwd, out password);

            var pwd = "";
            switch (password)
            {
                case AgentDetailAsPwd.agentloginid:
                    pwd = "$AgentLoginID$";
                    break;
                case AgentDetailAsPwd.agentpwd:
                    pwd = _dataContext.Password;
                    break;
                case AgentDetailAsPwd.empid:
                    pwd = _dataContext.Person.EmployeeID;
                    break;
                case AgentDetailAsPwd.username:
                    pwd = _dataContext.Person.UserName;
                    break;
            }
            return pwd;
        }

        /// <summary>
        /// Gets the wrap time.
        /// </summary>
        /// <param name="agentId">The agent identifier.</param>
        /// <param name="isUpdate">if set to <c>true</c> [is update].</param>
        /// <returns></returns>
        private string getAgentWrapUpTime(string agentId, bool isUpdate)
        {
            string WrapTime = string.Empty;
            try
            {
                CfgPerson person = _configContainer.GetValue("CfgPerson");
                ICollection<CfgAgentLoginInfo> loginCollection = person.AgentInfo.AgentLogins;

                foreach (CfgAgentLoginInfo infoLogin in loginCollection)
                {
                    if (infoLogin.AgentLogin.Switch.Name.ToString() == _dataContext.CurrentSwitchName && infoLogin.AgentLogin.LoginCode.ToString() == agentId)
                    {
                        if (!isUpdate)
                        {
                            WrapTime = infoLogin.WrapupTime.ToString();
                            _dataContext.agentWrapUpTime = infoLogin.WrapupTime;

                            int agentwraptime = 0;
                            if (int.TryParse(WrapTime, out agentwraptime) && _isMaxWrapTimeSet)
                            {
                                if (agentwraptime < _agentWrapUpTimeMaxValue)
                                    _isWrapTimeBeyondMaxValue = false;
                                else
                                    _isWrapTimeBeyondMaxValue = true;
                            }
                        }
                        else
                        {
                            infoLogin.WrapupTime = Convert.ToInt32(_dataContext.WrapTime);
                            _dataContext.agentWrapUpTime = Convert.ToInt32(_dataContext.WrapTime);
                        }
                    }
                }

                if (isUpdate)
                {
                    person.AgentInfo.AgentLogins = loginCollection;
                    person.Save();
                }
            }
            catch (Exception ex)
            {
                _logger.Error("Error occurred in GetWrapTime: " + ex.Message.ToString());
            }
            return WrapTime;
        }

        private int? GetAvailableLocation(Grid grid, string location)
        {
            int? rownum = null;
            int? colnum = null;
            for (int row = 0; row < grid.RowDefinitions.Count; row++)
            {
                for (int col = 0; col < grid.ColumnDefinitions.Count; col++)
                {
                    if (!grid.Children.Cast<UIElement>().Any(i => Grid.GetRow(i) == row && Grid.GetColumn(i) == col))
                    {
                        rownum = row;
                        colnum = col;
                        goto End;
                    }
                }
            }
        End:
            if (location == "row")
                return rownum;
            else if (location == "col")
                return colnum;
            return null;
        }

        private string GetWrapTime(string agentId, bool isUpdate)
        {
            string WrapTime = string.Empty;
            try
            {
                CfgPerson person = _configContainer.GetValue("CfgPerson");
                ICollection<CfgAgentLoginInfo> loginCollection = person.AgentInfo.AgentLogins;

                foreach (CfgAgentLoginInfo infoLogin in loginCollection)
                {
                    if (infoLogin.AgentLogin.Switch.Name.ToString() == _dataContext.CurrentSwitchName && infoLogin.AgentLogin.LoginCode.ToString() == agentId)
                    {
                        WrapTime = infoLogin.WrapupTime.ToString();
                        if (!isUpdate)
                        {
                            int agentwraptime = 0;
                            if (int.TryParse(WrapTime, out agentwraptime) && _isMaxWrapTimeSet)
                            {
                                if (agentwraptime < _agentWrapUpTimeMaxValue)
                                    _isWrapTimeBeyondMaxValue = false;
                                else
                                    _isWrapTimeBeyondMaxValue = true;
                            }
                        }
                        else
                        {
                            Dispatcher.Invoke((Action)delegate()
                            {
                                using (MessageBox msgBox = new MessageBox("Warning", "Please contact your administrator.", "_Ok", "_Cancel", false))
                                {
                                    if (msgBox.ShowDialog() == true)
                                    {
                                        infoLogin.WrapupTime = Convert.ToInt32(_dataContext.WrapTime);
                                    }
                                }
                            });
                        }
                    }
                }

                if (isUpdate)
                {
                    person.AgentInfo.AgentLogins = loginCollection;
                    person.Save();
                }
            }
            catch (Exception ex)
            {
                _logger.Error("Error occurred in GetWrapTime: " + ex.Message.ToString());
            }
            return WrapTime;
        }

        /// <summary>
        /// Initializes the third party integration.
        /// </summary>
        /// <param name="comObject">The COM object.</param>
        private void InitializeThirdPartyIntegration(ConfService comObject)
        {
            if (_dataContext.IsMediPacChecked)
            {
                if (!_dataContext.lstHoldingChannelSelection.ContainsKey(chkbxMediPac.Tag.ToString()))
                {
                    _dataContext.lstHoldingChannelSelection.Add(chkbxMediPac.Tag.ToString(), chkbxMediPac.IsChecked.Value);
                }
                else if (!_dataContext.lstHoldingChannelSelection.ContainsValue(chkbxMediPac.IsChecked.Value))
                {
                    _dataContext.lstHoldingChannelSelection[chkbxMediPac.Tag.ToString()] = chkbxMediPac.IsChecked.Value;
                }
            }
            else
                _dataContext.lstHoldingChannelSelection.Add(chkbxMediPac.Tag.ToString(), chkbxMediPac.IsChecked.Value);

            if (_dataContext.IsFacetsChecked)
            {
                if (!_dataContext.lstHoldingChannelSelection.ContainsKey(chkbxFacets.Tag.ToString()))
                {
                    _dataContext.lstHoldingChannelSelection.Add(chkbxFacets.Tag.ToString(), chkbxFacets.IsChecked.Value);
                }
                else if (!_dataContext.lstHoldingChannelSelection.ContainsValue(chkbxFacets.IsChecked.Value))
                {
                    _dataContext.lstHoldingChannelSelection[chkbxFacets.Tag.ToString()] = chkbxFacets.IsChecked.Value;
                }
            }
            else
                _dataContext.lstHoldingChannelSelection.Add(chkbxFacets.Tag.ToString(), chkbxFacets.IsChecked.Value);

            if (_dataContext.IsEpicChecked)
            {
                if (!_dataContext.lstHoldingChannelSelection.ContainsKey(chkbxEpic.Tag.ToString()))
                {
                    _dataContext.lstHoldingChannelSelection.Add(chkbxEpic.Tag.ToString(), chkbxEpic.IsChecked.Value);
                }
                else if (!_dataContext.lstHoldingChannelSelection.ContainsValue(chkbxEpic.IsChecked.Value))
                {
                    _dataContext.lstHoldingChannelSelection[chkbxEpic.Tag.ToString()] = chkbxEpic.IsChecked.Value;
                }
            }
            else
                _dataContext.lstHoldingChannelSelection.Add(chkbxEpic.Tag.ToString(), chkbxEpic.IsChecked.Value);

            if (_dataContext.IsLawsonChecked)
            {
                if (!_dataContext.lstHoldingChannelSelection.ContainsKey(chkbxLawson.Tag.ToString()))
                {
                    _dataContext.lstHoldingChannelSelection.Add(chkbxLawson.Tag.ToString(), chkbxLawson.IsChecked.Value);
                }
                else if (!_dataContext.lstHoldingChannelSelection.ContainsValue(chkbxLawson.IsChecked.Value))
                {
                    _dataContext.lstHoldingChannelSelection[chkbxLawson.Tag.ToString()] = chkbxLawson.IsChecked.Value;
                }
            }
            else
                _dataContext.lstHoldingChannelSelection.Add(chkbxLawson.Tag.ToString(), chkbxLawson.IsChecked.Value);

            //Code Added - For collect agent information
            //29-07-2014 - smoorhty

            #region Collect Agent Information to send integration part

            _agentInfo = new AgentInfo();
            if (_dataContext.Person.FirstName != null)
                _agentInfo.FirstName = _dataContext.Person.FirstName;
            else
                _agentInfo.FirstName = "";
            if (_dataContext.Person.LastName != null)
                _agentInfo.LastName = _dataContext.Person.LastName;
            else
                _agentInfo.LastName = "";
            _agentInfo.UserName = _dataContext.UserName;
            _agentInfo.EmployeeID = _dataContext.AgentID;
            _agentInfo.LoginID = _dataContext.AgentLoginId;
            _agentInfo.ExtensionDN = _dataContext.ThisDN;
            _agentInfo.Queue = _dataContext.Queue;

            #endregion Collect Agent Information to send integration part

            //end

            var file = Path.Combine(Path.Combine(System.Windows.Forms.Application.StartupPath, "Plugins"), "Pointel.Integration.Core.dll");
            if (File.Exists(file))
            {
                Assembly asm = Assembly.LoadFile(file);
                var thirdPartyInterfrace = (IDesktopMessenger)(from asmType in asm.GetTypes() where asmType.GetInterface("IDesktopMessenger") != null select (IDesktopMessenger)Activator.CreateInstance(asmType)).FirstOrDefault();
                thirdPartyInterfrace.InitializeIntegration(comObject, _dataContext.ApplicationName, _dataContext.lstHoldingChannelSelection);
                thirdPartyInterfrace.GetAgentInfo(_agentInfo);
            }
            else
                _logger.Warn("Integration.Core Plug-in dll not exist");
        }

        /// <summary>
        /// Medias the availability check.
        /// </summary>
        private void mediaAvailabilityCheck()
        {
            try
            {
                _dataContext.lstHoldingChannelSelection.Clear();
                _dataContext.lstHoldingChannelSelection.Add(chkbxVoice.Content.ToString(), chkbxVoice.IsChecked != null && chkbxVoice.IsChecked.Value);
                if (isEnableLoginQueue)
                {
                    cbQueue.SelectedIndex = 0;
                    lblQueue.Visibility = System.Windows.Visibility.Visible;
                    cbQueue.Visibility = System.Windows.Visibility.Visible;
                    _dataContext.LoginQueueRowHeight = GridLength.Auto;
                }
                else
                {
                    lblQueue.Visibility = System.Windows.Visibility.Hidden;
                    cbQueue.Visibility = System.Windows.Visibility.Hidden;
                    _dataContext.LoginQueueRowHeight = new GridLength(0);
                }

                if (isEnableVoiceUnactivateChannel)
                {
                    if (_dataContext.IsVoiceChecked)
                    {
                        chkbxVoice.IsEnabled = true;
                        lblQueue.IsEnabled = true;
                        cbQueue.IsEnabled = true;
                        lblAgentLogin.IsEnabled = true;
                        lblAgentLoginID.IsEnabled = true;
                        cbAgentLoginID.IsEnabled = true;
                        lblAgentPassword.IsEnabled = true;
                        txtAgentPassword.IsEnabled = true;
                    }
                    else
                    {
                        chkbxVoice.IsEnabled = true;
                        lblQueue.IsEnabled = false;
                        cbQueue.IsEnabled = false;
                        lblAgentLogin.IsEnabled = false;
                        lblAgentLoginID.IsEnabled = false;
                        cbAgentLoginID.IsEnabled = false;
                        lblAgentPassword.IsEnabled = false;
                        txtAgentPassword.IsEnabled = false;
                    }
                    //if (_dataContext.IsCMELoginEnable)
                    //_dataContext.IsVoiceChecked = true;
                }
                else
                {
                    _logger.Warn("Media Voice - Can Inactivate Channel is set to false");
                    chkbxVoice.IsEnabled = false;
                    _dataContext.IsVoiceChecked = true;
                    lblQueue.IsEnabled = true;
                    cbQueue.IsEnabled = true;
                    lblAgentLogin.IsEnabled = true;
                    lblAgentLoginID.IsEnabled = true;
                    cbAgentLoginID.IsEnabled = true;
                    lblAgentPassword.IsEnabled = true;
                    txtAgentPassword.IsEnabled = true;
                    _dataContext.IsVoiceChecked = true;
                }
                if (IsChatPluginEnabled && _dataContext.IsChatPluginAdded && Datacontext.AvailableServerDic.ContainsValue(CfgAppType.CFGInteractionServer.ToString()) && Datacontext.AvailableServerDic.ContainsValue(CfgAppType.CFGChatServer.ToString()))
                {
                    grdRowChat.Height = GridLength.Auto;
                    chkbxChat.Visibility = Visibility.Visible;
                    if (isEnableChatUnactivateChannel)
                        chkbxChat.IsEnabled = true;
                    else
                    {
                        chkbxChat.IsEnabled = false;
                        _dataContext.IsChatChecked = true;
                    }
                }
                else
                {
                    grdRowChat.Height = new GridLength(0);
                    chkbxChat.IsEnabled = false;
                    chkbxChat.Visibility = Visibility.Collapsed;
                }

                if (IsEmailPluginEnabled && _dataContext.IsEmailPluginAdded && Datacontext.AvailableServerDic.ContainsValue(CfgAppType.CFGInteractionServer.ToString()))
                {
                    grdRowEmail.Height = GridLength.Auto;
                    chkbxEmail.Visibility = System.Windows.Visibility.Visible;
                    if (isEnableEmailUnactivateChannel)
                        chkbxEmail.IsEnabled = true;
                    else
                    {
                        chkbxEmail.IsEnabled = false;
                        _dataContext.IsEmailChecked = true;
                    }
                }
                else
                {
                    grdRowEmail.Height = new GridLength(0);
                    chkbxEmail.IsEnabled = false;
                    chkbxEmail.Visibility = System.Windows.Visibility.Collapsed;
                }

                if (IsOutboundPluginEnabled && _dataContext.IsOutboundPluginAdded)
                {
                    grdRowOutbound.Height = GridLength.Auto;
                    chkbxOutbound.Visibility = System.Windows.Visibility.Visible;
                    if (isEnableOutboundUnactiveChannel)
                        chkbxOutbound.IsEnabled = true;
                    else
                    {
                        chkbxOutbound.IsEnabled = false;
                        _dataContext.IsOutboundChecked = true;
                    }
                }
                else
                {
                    grdRowOutbound.Height = new GridLength(0);
                    chkbxOutbound.IsEnabled = false;
                    chkbxOutbound.Visibility = System.Windows.Visibility.Collapsed;
                }

                if (!isPromptVoiceAgentLoginId && !isEnableLoginQueue)
                {
                    gbVoiceChannel.BorderBrush = new SolidColorBrush(Colors.Transparent);
                    gbVoiceChannel.BorderThickness = new Thickness(0);
                }

                if (isPromptVoiceAgentLoginId)
                {
                    grd_VoiceLoginDetails.Visibility = System.Windows.Visibility.Visible;
                    lblAgentLogin.Visibility = System.Windows.Visibility.Visible;
                    lblAgentLoginID.Visibility = System.Windows.Visibility.Visible;
                    _dataContext.AgentLoginIDRowHeight = GridLength.Auto;
                    if (_dataContext.UserLoginID != null && _dataContext.UserLoginID != string.Empty)
                    {
                        var splitValue = _dataContext.UserLoginID.Split(new char[] { ' ' });
                        lblAgentLoginID.Content = splitValue[2].ToString();
                    }
                    else if (_dataContext.AgentLoginIds.Count > 0)
                    {
                        //just to skip else in this case
                    }
                    else
                    {
                        _dataContext.AgentLoginIDRowHeight = new GridLength(0);
                        lblAgentLoginID.Content = string.Empty;
                    }
                }
                else
                {
                    if (_dataContext.AgentLoginIds.Count > 1)
                    {
                        grd_VoiceLoginDetails.Visibility = System.Windows.Visibility.Visible;
                        gbVoiceChannel.BorderBrush = new SolidColorBrush((Color)(ColorConverter.ConvertFromString("#ADAAAD")));
                        gbVoiceChannel.BorderThickness = new Thickness(0.5);
                        lblAgentLogin.Visibility = System.Windows.Visibility.Visible;
                        lblAgentLoginID.Visibility = System.Windows.Visibility.Visible;
                        _dataContext.AgentLoginIDRowHeight = GridLength.Auto;
                        if (_dataContext.UserLoginID != null && _dataContext.UserLoginID != string.Empty)
                        {
                            var splitValue = _dataContext.UserLoginID.Split(new char[] { ' ' });
                            lblAgentLoginID.Content = splitValue[2].ToString();
                        }
                        else
                        {
                            _dataContext.AgentLoginIDRowHeight = new GridLength(0);
                            lblAgentLoginID.Content = string.Empty;
                        }
                    }
                    else
                    {
                        grd_VoiceLoginDetails.Visibility = System.Windows.Visibility.Collapsed;
                        lblAgentLogin.Visibility = System.Windows.Visibility.Collapsed;
                        lblAgentLoginID.Visibility = System.Windows.Visibility.Collapsed;
                    }
                }
                if (isPromptVoiceAgentLoginPassword)
                {
                    lblAgentPassword.Visibility = System.Windows.Visibility.Visible;
                    txtAgentPassword.Visibility = System.Windows.Visibility.Visible;
                    _dataContext.AgentPasswordRowHeight = GridLength.Auto;
                    if (txtPlace.Text != string.Empty && cbQueue.Text != string.Empty)
                        txtAgentPassword.Focus();
                }
                else
                {
                    lblAgentPassword.Visibility = System.Windows.Visibility.Collapsed;
                    txtAgentPassword.Visibility = System.Windows.Visibility.Collapsed;
                    _dataContext.AgentPasswordRowHeight = new GridLength(0);
                }
                if (isEnableLDCode && IsEnableCRMThirdParty)
                {
                    _dataContext.LDCodeRowHeight = GridLength.Auto;
                    lblLDCode.Visibility = System.Windows.Visibility.Visible;
                    txtLDCode.Visibility = System.Windows.Visibility.Visible;
                }
                else
                {
                    _dataContext.LDCodeRowHeight = new GridLength(0);
                    lblLDCode.Visibility = System.Windows.Visibility.Collapsed;
                    txtLDCode.Visibility = System.Windows.Visibility.Collapsed;
                }

                if (_dataContext.AgentLoginIds.Count > 1)
                {
                    lblAgentLoginID.Visibility = System.Windows.Visibility.Hidden;
                    cbAgentLoginID.Visibility = System.Windows.Visibility.Visible;
                    //_dataContext.AgentLoginId = string.Empty;
                }
                else
                {
                    lblAgentLoginID.Visibility = System.Windows.Visibility.Visible;
                    cbAgentLoginID.Visibility = System.Windows.Visibility.Hidden;
                    _dataContext.AgentLoginIds.Clear();
                }

                if (_isEnableWrapUpTime)
                {
                    _dataContext.WrapTimeRowHeight = GridLength.Auto;
                    lblWrapTime.Visibility = System.Windows.Visibility.Visible;
                    txtWrapTime.Visibility = System.Windows.Visibility.Visible;
                }
                else
                {
                    _dataContext.WrapTimeRowHeight = new GridLength(0);
                    lblWrapTime.Visibility = System.Windows.Visibility.Collapsed;
                    txtWrapTime.Visibility = System.Windows.Visibility.Collapsed;
                }
            }
            catch (Exception generalExcption)
            {
                _logger.Error("Error occurred in mediaAvailabilityCheck() : " + generalExcption.ToString());
            }
        }

        /// <summary>
        /// Mouses the left button down.
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="e">The <see cref="MouseButtonEventArgs"/> instance containing the event data.</param>
        private void MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            try
            {
                this.DragMove();
                if (!(_configContainer.AllKeys.Contains("allow.system-draggable") &&
                        ((string)_configContainer.GetValue("allow.system-draggable")).ToLower().Equals("true")))
                {
                    if (Left < 0)
                        Left = 0;
                    if (Top < 0)
                        Top = 0;
                    if (Left > SystemParameters.WorkArea.Right - Width)
                        Left = SystemParameters.WorkArea.Right - Width;
                    if (Top > SystemParameters.WorkArea.Bottom - Height)
                        Top = SystemParameters.WorkArea.Bottom - Height; ;
                }
            }
            catch { }
        }

        private void PHSItegration()
        {
            if (!IsEnableCRMThirdParty) return;
            int count = (Convert.ToInt16(isEnableMediPac)
                + Convert.ToInt16(isEnableFacets)
                + Convert.ToInt16(isEnableLawson)
                + Convert.ToInt16(isEnableEpic));
            switch (count)
            {
                case 2:
                case 1:
                    grdPHS2.Height = new GridLength(0);
                    break;

                case 0:
                    grdPHS1.Height = new GridLength(0);
                    grdPHS2.Height = new GridLength(0);
                    break;

                default:
                    grdPHS1.Height = GridLength.Auto;
                    grdPHS2.Height = GridLength.Auto;
                    break;
            }
            int? row;
            int? col;
            if (isEnableMediPac)
            {
                row = GetAvailableLocation(grd_VoiceLoginDetails, "row");
                col = GetAvailableLocation(grd_VoiceLoginDetails, "col");
                if (row != null)
                    Grid.SetRow(chkbxMediPac, Convert.ToInt16(row));
                if (col != null)
                    Grid.SetColumn(chkbxMediPac, Convert.ToInt16(col));
            }
            if (isEnableFacets)
            {
                row = GetAvailableLocation(grd_VoiceLoginDetails, "row");
                col = GetAvailableLocation(grd_VoiceLoginDetails, "col");
                if (row != null)
                    Grid.SetRow(chkbxFacets, Convert.ToInt16(row));
                if (col != null)
                    Grid.SetColumn(chkbxFacets, Convert.ToInt16(col));
            }
            if (isEnableEpic)
            {
                row = GetAvailableLocation(grd_VoiceLoginDetails, "row");
                col = GetAvailableLocation(grd_VoiceLoginDetails, "col");
                if (row != null)
                    Grid.SetRow(chkbxEpic, Convert.ToInt16(row));
                if (col != null)
                    Grid.SetColumn(chkbxEpic, Convert.ToInt16(col));
            }
            if (isEnableLawson)
            {
                row = GetAvailableLocation(grd_VoiceLoginDetails, "row");
                col = GetAvailableLocation(grd_VoiceLoginDetails, "col");
                if (row != null)
                    Grid.SetRow(chkbxLawson, Convert.ToInt16(row));
                if (col != null)
                    Grid.SetColumn(chkbxLawson, Convert.ToInt16(col));
            }

            chkbxMediPac.Visibility = isEnableMediPac ? System.Windows.Visibility.Visible : System.Windows.Visibility.Collapsed;
            chkbxFacets.Visibility = isEnableFacets ? System.Windows.Visibility.Visible : System.Windows.Visibility.Collapsed;
            chkbxEpic.Visibility = isEnableEpic ? System.Windows.Visibility.Visible : System.Windows.Visibility.Collapsed;
            chkbxLawson.Visibility = isEnableLawson ? System.Windows.Visibility.Visible : System.Windows.Visibility.Collapsed;
        }

        /// <summary>
        /// Previews the key up.
        /// </summary>
        /// <param name="sender">The sender.</param>timer
        /// <param name="e">The <see cref="KeyEventArgs"/> instance containing the event data.</param>
        private void PreviewKeyUp(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
                btnOk_Click(null, null);
            else
            {
                var txtBox = sender as PasswordBox;
                //if (txtBox != null && txtBox.Name == "txtAgentPassword")
                //    btnOk.IsEnabled = true;
            }
        }

        /// <summary>
        /// Removes the switch_place_agent login.
        /// </summary>
        private void RemoveSwitch_place_agentLogin(bool checkMediaAvailability = true)
        {
            try
            {
                btnOk.IsEnabled = true;
                _dataContext.CSErrorRowHeight = new GridLength(0);
                if (checkMediaAvailability)
                    mediaAvailabilityCheck();
                _logger.Info("Retrieving agent info");
                SelectDN();
                Genesyslab.Platform.ApplicationBlocks.ConfigurationObjectModel.Queries.CfgPlaceQuery _cfgPlaceQuery = new Genesyslab.Platform.ApplicationBlocks.ConfigurationObjectModel.Queries.CfgPlaceQuery();
                _cfgPlaceQuery.Name = _dataContext.Place;
                _cfgPlaceQuery.TenantDbid = _configContainer.TenantDbId;
                Genesyslab.Platform.ApplicationBlocks.ConfigurationObjectModel.CfgObjects.CfgPlace _cfgplace =
                        _configContainer.ConfServiceObject.RetrieveObject<Genesyslab.Platform.ApplicationBlocks.ConfigurationObjectModel.CfgObjects.CfgPlace>(_cfgPlaceQuery);
                if (_cfgplace == null) return;
                ICollection<Genesyslab.Platform.ApplicationBlocks.ConfigurationObjectModel.CfgObjects.CfgDN> dn = _cfgplace.DNs;
                int dnDBID = 0;
                foreach (var item in dn)
                {
                    if (_dataContext.ThisDN == item.Number)
                    {
                        dnDBID = item.DBID;
                        break;
                    }
                }

                Genesyslab.Platform.ApplicationBlocks.ConfigurationObjectModel.Queries.CfgDNQuery _cfgDNQuery = new Genesyslab.Platform.ApplicationBlocks.
                    ConfigurationObjectModel.Queries.CfgDNQuery();
                _cfgDNQuery.DnNumber = _dataContext.ThisDN;
                _cfgDNQuery.Dbid = dnDBID;
                _cfgDNQuery.TenantDbid = _configContainer.TenantDbId;
                _logger.Info("Place DN Queried : Dn Number :" + _dataContext.ThisDN + " DBID : " + _cfgDNQuery.Dbid.ToString());
                Genesyslab.Platform.ApplicationBlocks.ConfigurationObjectModel.CfgObjects.CfgDN _cfgDN = _configContainer.ConfServiceObject.RetrieveObject<Genesyslab.Platform.
                    ApplicationBlocks.ConfigurationObjectModel.CfgObjects.CfgDN>(_cfgDNQuery);
                if (_cfgDN == null)
                    _logger.Warn("Unable to retrieve Place DN details");
                string a = _cfgDN.Switch.Name;
                _logger.Info("Place DN switch name : " + _cfgDN.Switch.Name + a);
                List<string> temp = new List<string>();
                if (_agentLoginIDs.Count > 0)
                {
                    _tempAgentLoginIDs = new Dictionary<string, int>();
                    foreach (var item in _agentLoginIDs)
                    {
                        Genesyslab.Platform.ApplicationBlocks.ConfigurationObjectModel.Queries.CfgAgentLoginQuery _cfgAgentLoginQuery = new Genesyslab.Platform.ApplicationBlocks.ConfigurationObjectModel.Queries.CfgAgentLoginQuery();
                        _cfgAgentLoginQuery.LoginCode = item.Key; //_dataContext.AgentLoginId != string.Empty ? _dataContext.AgentLoginId : cbAgentLoginID.Text;
                        _cfgAgentLoginQuery.TenantDbid = _configContainer.TenantDbId;
                        var agentInfo = (ICollection<Genesyslab.Platform.ApplicationBlocks.ConfigurationObjectModel.CfgObjects.CfgAgentLoginInfo>)_dataContext.Person.AgentInfo.AgentLogins;
                        foreach (var loginDetails in agentInfo)
                        {
                            if (loginDetails.AgentLogin == null) continue;
                            if (item.Key == loginDetails.AgentLogin.LoginCode && item.Value == loginDetails.AgentLogin.Switch.Name)
                            {
                                _cfgAgentLoginQuery.Dbid = loginDetails.AgentLogin.DBID;
                                break;
                            }
                        }
                        _logger.Info("Agent Queried : Login Code :" + _cfgAgentLoginQuery.LoginCode.ToString() + " DBID : " + _cfgAgentLoginQuery.Dbid.ToString());
                        Genesyslab.Platform.ApplicationBlocks.ConfigurationObjectModel.CfgObjects.CfgAgentLogin _cfgAgentLogin = _configContainer.ConfServiceObject.RetrieveObject<Genesyslab.Platform.ApplicationBlocks.ConfigurationObjectModel.CfgObjects.CfgAgentLogin>(_cfgAgentLoginQuery);
                        if (_cfgAgentLogin != null)
                        {
                            _logger.Info("Agent login ID switch type : " + _cfgAgentLogin.Switch.Name);
                            if (_cfgDN.Switch.Name == _cfgAgentLogin.Switch.Name)
                            {
                                _dataContext.CurrentSwitchName = _cfgAgentLogin.Switch.Name;
                                temp.Add(item.Key);
                                _tempAgentLoginIDs.Add(item.Key, _cfgAgentLogin.DBID);
                            }
                        }
                        else
                            _logger.Warn("Unable to retrieve agent details.");
                    }
                    _dataContext.AgentLoginIds = temp;
                    if (temp.Count > 1)
                    {
                        cbAgentLoginID.Visibility = System.Windows.Visibility.Visible;
                        lblAgentLoginID.Visibility = System.Windows.Visibility.Collapsed;
                        cbAgentLoginID.SelectedIndex = 0;
                        if (_isEnableWrapUpTime)
                            _dataContext.WrapTime = getAgentWrapUpTime(temp[0], false);
                    }
                    else if (temp.Count == 1)
                    {
                        _agentLoginID = temp[0];
                        cbAgentLoginID.Visibility = System.Windows.Visibility.Collapsed;
                        lblAgentLoginID.Visibility = System.Windows.Visibility.Visible;
                        _dataContext.AgentLoginId = _agentLoginID.ToString();
                        lblAgentLoginID.Content = _agentLoginID.ToString();
                        _dataContext.AgentLoginIDRowHeight = GridLength.Auto;
                        if (_isEnableWrapUpTime)
                            _dataContext.WrapTime = getAgentWrapUpTime(_agentLoginID, false);
                    }
                    else
                    {
                        cbAgentLoginID.Visibility = System.Windows.Visibility.Collapsed;
                        lblAgentLoginID.Visibility = System.Windows.Visibility.Visible;
                        _dataContext.AgentLoginId = "";
                        lblAgentLoginID.Content = "";
                        _dataContext.AgentLoginIDRowHeight = GridLength.Auto;
                    }
                }

                #region Old Code

                //if (_agentLoginIDs.Count < 0 || temp.Count <= 1)
                //{
                //    Genesyslab.Platform.ApplicationBlocks.ConfigurationObjectModel.Queries.CfgAgentLoginQuery _cfgAgentLoginQuery = new Genesyslab.Platform.ApplicationBlocks.ConfigurationObjectModel.Queries.CfgAgentLoginQuery();
                //    _cfgAgentLoginQuery.LoginCode = _agentLoginID;
                //    _cfgAgentLoginQuery.TenantDbid = _configContainer.TenantDbId;
                //    var agentInfo = (ICollection<Genesyslab.Platform.ApplicationBlocks.ConfigurationObjectModel.CfgObjects.CfgAgentLoginInfo>)_dataContext.Person.AgentInfo.AgentLogins;
                //    foreach (var loginDetails in agentInfo)
                //    {
                //        if (_cfgAgentLoginQuery.LoginCode == loginDetails.AgentLogin.LoginCode)
                //        {
                //            _cfgAgentLoginQuery.Dbid = loginDetails.AgentLogin.DBID;
                //            break;
                //        }
                //        //CfgPerson person = _configContainer.GetValue("CfgPerson");

                //    }
                //    _logger.Info("Agent Queried : Login Code :" + _cfgAgentLoginQuery.LoginCode.ToString() + " DBID : " + _cfgAgentLoginQuery.Dbid.ToString());
                //    Genesyslab.Platform.ApplicationBlocks.ConfigurationObjectModel.CfgObjects.CfgAgentLogin _cfgAgentLogin = _configContainer.ConfServiceObject.RetrieveObject<Genesyslab.Platform.ApplicationBlocks.ConfigurationObjectModel.CfgObjects.CfgAgentLogin>(_cfgAgentLoginQuery);
                //    if (_cfgAgentLogin == null)
                //    {
                //        _dataContext.AgentLoginId = string.Empty;
                //        lblAgentLoginID.Content = string.Empty;
                //        _logger.Warn("Unable to retrieve agent details.");
                //    }
                //    else if (_cfgDN.Switch.Name != _cfgAgentLogin.Switch.Name)
                //    {
                //        lblAgentLoginID.Content = string.Empty;
                //        _dataContext.AgentLoginId = string.Empty;
                //    }
                //    else
                //    {
                //        _logger.Info("Agent login ID switch type : " + _cfgAgentLogin.Switch.Name);
                //        _dataContext.AgentLoginId = _cfgAgentLogin.LoginCode.ToString();
                //        lblAgentLoginID.Content = _cfgAgentLogin.LoginCode.ToString();
                //    }
                //}

                #endregion Old Code

                CfgSwitchQuery switchQuery = new CfgSwitchQuery();
                switchQuery.Name = _cfgDN.Switch.Name;
                switchQuery.TenantDbid = _configContainer.TenantDbId;
                _cfgSwitch = _configContainer.ConfServiceObject.RetrieveObject<Genesyslab.Platform.ApplicationBlocks.ConfigurationObjectModel.CfgObjects.CfgSwitch>(switchQuery);
                if (_cfgSwitch != null && Datacontext.TServersSwitchDic != null && Datacontext.TServersSwitchDic.Count > 0)
                {
                    var media = (Datacontext.AvailableServerDic.Where(x => x.Value == Genesyslab.Platform.Configuration.Protocols.Types.CfgAppType.CFGTServer.ToString())).ToDictionary(x => x.Key, y => y.Value);
                    if (media == null)
                        return;
                    if (media.Count > 0)
                    {
                        if (Datacontext.TServersSwitchDic.ContainsKey(_cfgSwitch.DBID)
                            && media.Any(x => (x.Key.ToString().Split(','))[0].Trim() == Datacontext.TServersSwitchDic[_cfgSwitch.DBID]))
                        {
                            Datacontext.UsedTServerSwitchDBId = _cfgSwitch.DBID;
                            gbVoiceChannel.Visibility = Visibility.Visible;
                        }
                        else
                        {
                            //  Datacontext.UsedTServerSwitchDBId = 0;
                            gbVoiceChannel.Visibility = Visibility.Collapsed;
                        }
                    }
                }
                if ((Datacontext.AvailableServerDic != null &&
                    (Datacontext.AvailableServerDic.Count == 0 || (Datacontext.AvailableServerDic.Count == 1 && Datacontext.AvailableServerDic.ContainsValue(CfgAppType.CFGContactServer.ToString()))))
                    ||
                    (gbVoiceChannel.Visibility == System.Windows.Visibility.Collapsed &&
                    chkbxEmail.Visibility == System.Windows.Visibility.Collapsed &&
                    chkbxChat.Visibility == System.Windows.Visibility.Collapsed &&
                    chkbxOutbound.Visibility == System.Windows.Visibility.Collapsed)
                    )
                {
                    _dataContext.CSErrorMessage = "No available channel for this place.";
                    if (txtPlace.IsEnabled == false)
                        txtPlace.IsEnabled = true;
                    _dataContext.CSErrorRowHeight = GridLength.Auto;
                    btnOk.IsEnabled = false;
                    //StartTimerForError();

                }
            }
            catch (Exception ex)
            {
                _logger.Error("Error occurred in RemoveSwitch_place_agentLogin: " + ex.Message.ToString());
            }
        }

        private void SelectDN()
        {
            try
            {
                var selectedDNs = ReadDNs(_dataContext.Place);
                if (selectedDNs != null && selectedDNs.Count > 0)
                {
                    Datacontext.DNsCollection.Clear();
                    foreach (var dNs in selectedDNs)
                    {
                        Datacontext.DNsCollection.Add(dNs.Key, dNs.Value);
                    }
                    if (Datacontext.DNsCollection.ContainsKey("Extension") && !string.IsNullOrEmpty(Datacontext.DNsCollection["Extension"].ToString()))
                        _dataContext.ThisDN = Datacontext.DNsCollection["Extension"].ToString();
                    else if (Datacontext.DNsCollection.ContainsKey("ACD") && !string.IsNullOrEmpty(Datacontext.DNsCollection["ACD"].ToString()))
                        _dataContext.ThisDN = Datacontext.DNsCollection["ACD"].ToString();
                }
                else
                {
                    _dataContext.CSErrorMessage = "Unable to login. There is no such place configured.";
                    StartTimerForError();
                    // _dataContext.CSErrorRowHeight = GridLength.Auto;
                    txtPlace.IsEnabled = true;
                }
            }
            catch (Exception ex)
            {
                _logger.Error("Selecting DN : " + ex.Message);
            }
        }

        private void StartTimerForError()
        {
            try
            {
                _dataContext.CSErrorRowHeight = GridLength.Auto;
                //btnCloseError.Visibility = Visibility.Visible;
                _timerforcloseError = new DispatcherTimer();
                _timerforcloseError.Interval = TimeSpan.FromSeconds(10);
                _timerforcloseError.Tick += new EventHandler(CloseError);
                _timerforcloseError.Start();
                btnOk.IsEnabled = false;
            }
            catch (Exception ex)
            {
                _logger.Error("Error occurred as  " + ex.Message);
            }
        }

        private void txtLDCode_PreviewKeyDown(object sender, KeyEventArgs e)
        {
            CloseError(null, null);
            btnOk.IsEnabled = true;

            if ((Keyboard.Modifiers & ModifierKeys.Shift) == ModifierKeys.Shift)
                e.Handled = true;
            else
            {
                switch (e.Key)
                {
                    case Key.D0:
                    case Key.D1:
                    case Key.D2:
                    case Key.D3:
                    case Key.D4:
                    case Key.D5:
                    case Key.D6:
                    case Key.D7:
                    case Key.D8:
                    case Key.D9:
                    case Key.NumLock:
                    case Key.NumPad0:
                    case Key.NumPad1:
                    case Key.NumPad2:
                    case Key.NumPad3:
                    case Key.NumPad4:
                    case Key.NumPad5:
                    case Key.NumPad6:
                    case Key.NumPad7:
                    case Key.NumPad8:
                    case Key.NumPad9:
                    case Key.Multiply:
                    case Key.Left:
                    case Key.Right:
                    case Key.End:
                    case Key.Home:
                    case Key.Prior:
                    case Key.Next:
                    case Key.Delete:
                    case Key.Back:
                    case Key.Enter:
                    case Key.Tab:
                        break;

                    default:
                        e.Handled = true;
                        break;
                }
            }
        }

        //Event Added for the purpose of focus the Auto complete text box by Smoorthy
        //29-04-2014
        /// <summary>
        /// Handles the GotFocus event of the txtPlace control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="RoutedEventArgs"/> instance containing the event data.</param>
        private void txtPlace_GotFocus(object sender, RoutedEventArgs e)
        {
            try
            {
                if (istxtPlaceFocued == false)
                {
                    AutoCompleteBox autoBox = sender as AutoCompleteBox;
                    if (autoBox == null)
                        return;
                    TextBox textBox = autoBox.Template.FindName("Text", autoBox) as TextBox;
                    if (textBox != null)
                        textBox.Focus();
                    istxtPlaceFocued = true;
                }
            }
            catch (Exception generalException)
            {
                _logger.Error("ChannelSelection:txtPlace_GotFocus():" + generalException.ToString());
            }
        }

        private void txtPlace_PreviewKeyUp(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
                if (!_configContainer.GetAsBoolean("login.enable-place-completion"))
                {
                    if (CheckPlaceValid(txtPlace.Text))
                    {
                        _dataContext.Place = txtPlace.Text;
                        btnOk.IsEnabled = true;
                        RemoveSwitch_place_agentLogin();
                    }
                    else
                    {
                        _dataContext.CSErrorMessage = "Unable to login. There is no such place configured.";
                        StartTimerForError();
                        // _dataContext.CSErrorRowHeight = GridLength.Auto;
                        txtPlace.IsEnabled = true;
                        btnOk.IsEnabled = false;
                    }
                }
                else
                    btnOk_Click(null, null);
        }

        /// <summary>
        /// Handles the SelectionChanged event of the txtPlace control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="SelectionChangedEventArgs"/> instance containing the event data.</param>
        private void txtPlace_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            var currentPlace = sender as AutoCompleteBox;
            CloseError(null, null);
            gbVoiceChannel.Visibility = Visibility.Collapsed;
            grdRowEmail.Height = new GridLength(0);
            grdRowChat.Height = new GridLength(0);
            if (_dataContext.AutoComplete == null)
            {
                if (CheckPlaceValid(currentPlace.Text))
                    RemoveSwitch_place_agentLogin();
                else if (!_configContainer.GetAsBoolean("login.enable-place-completion"))
                    btnOk.IsEnabled = false;
            }
            else if (currentPlace != null && _dataContext.AutoComplete.FindIndex(x => x.Equals(currentPlace.Text, StringComparison.OrdinalIgnoreCase)) != -1)
            {
                //var state = CheckPlaceStatus(currentPlace.Text);
                RemoveSwitch_place_agentLogin();
            }
            //else
            //{
            //    gbVoiceChannel.Visibility = Visibility.Collapsed;
            //    grdRowEmail.Height = new GridLength(0);
            //    grdRowChat.Height = new GridLength(0);
            //}
            if (_dataContext.AgentLoginIds.Count > 0)
            {
                cbAgentLoginID.SelectedIndex = 0;
            }
        }

        /// <summary>
        /// Handles the TextChanged event of the txtPlace control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="RoutedEventArgs"/> instance containing the event data.</param>
        private void txtPlace_TextChanged(object sender, RoutedEventArgs e)
        {
            //if (_agentLoginIDs.Count == 0 && _dataContext.AgentLoginIds.Count > 0)
            //{
            //    foreach (var item in _dataContext.AgentLoginIds)
            //    {
            //        _agentLoginIDs.Add(item.ToString());
            //    }
            //}
            //if (string.IsNullOrEmpty(_agentLoginID) && !string.IsNullOrEmpty(_dataContext.AgentLoginId))
            //{
            //    _agentLoginID = _dataContext.AgentLoginId;
            //}

            //var currentPlace = sender as AutoCompleteBox;
            //if (currentPlace != null && _dataContext.AutoComplete.FindIndex(x => x.Equals(currentPlace.Text, StringComparison.OrdinalIgnoreCase)) != -1)
            //{
            //    //var state = CheckPlaceStatus(currentPlace.Text);
            //    _dataContext.Place = _dataContext.AutoComplete.FirstOrDefault(place => place.Equals(currentPlace.Text, StringComparison.OrdinalIgnoreCase));
            //    //_dataContext.Place = currentPlace.Text;
            //    RemoveSwitch_place_agentLogin();
            //}
            //if (_dataContext.AgentLoginIds.Count > 0)
            //{
            //    cbAgentLoginID.SelectedIndex = 0;
            //}
        }

        private void txtWrapTime_PreviewKeyDown(object sender, KeyEventArgs e)
        {
            if ((Keyboard.Modifiers & ModifierKeys.Shift) == ModifierKeys.Shift)
                e.Handled = true;
            else
            {
                Action checkInput = () =>
                    {
                        switch (e.Key)
                        {
                            case Key.D0:
                            case Key.D1:
                            case Key.D2:
                            case Key.D3:
                            case Key.D4:
                            case Key.D5:
                            case Key.D6:
                            case Key.D7:
                            case Key.D8:
                            case Key.D9:
                            case Key.NumLock:
                            case Key.NumPad0:
                            case Key.NumPad1:
                            case Key.NumPad2:
                            case Key.NumPad3:
                            case Key.NumPad4:
                            case Key.NumPad5:
                            case Key.NumPad6:
                            case Key.NumPad7:
                            case Key.NumPad8:
                            case Key.NumPad9:
                            case Key.Back:
                            case Key.Delete:
                            case Key.Left:
                            case Key.Right:
                            case Key.End:
                            case Key.Home:
                            case Key.Tab:
                            case Key.LeftAlt:
                            case Key.RightAlt:
                            case Key.Prior:
                            case Key.Next:
                            case Key.System:
                                e.Handled = false;
                                break;

                            default:
                                e.Handled = true;
                                break;
                        }
                    };
                checkInput();
                while (e.Key != Key.Back && e.Key != Key.Delete
                    && e.Key != Key.Left && e.Key != Key.Right
                    && e.Key != Key.End && e.Key != Key.Home
                    && e.Key != Key.Prior && e.Key != Key.Next && e.Key != Key.Tab && e.Key != Key.LeftAlt && e.Key != Key.RightAlt && e.Key != Key.System)
                {
                    if (string.IsNullOrEmpty(txtWrapTime.Text) && NumericKeyboardvalue(e.Key) != null)
                    {
                        e.Handled = false;
                        break;
                    }
                    else
                    {
                        var num = NumericKeyboardvalue(e.Key);
                        var portText = txtWrapTime.Text;
                        if (!string.IsNullOrEmpty(txtWrapTime.SelectedText))
                            portText = portText.Replace(txtWrapTime.SelectedText, "");
                        if (num != null)
                        {
                            portText += num;
                            if (Convert.ToInt32(portText) > _agentWrapUpTimeMaxValue ||
                                Convert.ToInt32(portText) == 0)
                            {
                                e.Handled = true;
                                break;
                            }
                            else
                            {
                                e.Handled = false;
                                break;
                            }
                        }
                    }

                    e.Handled = true;
                    break;
                }
            }
        }

        private void view_ContentRendered(object sender, EventArgs e)
        {
            _view.Show();
            _view.ContentRendered -= view_ContentRendered;

            var source = PresentationSource.FromVisual(this) as HwndSource;
            if (source != null)
                source.RemoveHook(WndProc);
            foreach (var item in Resources)
                Resources.Remove(item);

            #region Nulling variables

            _logger = null;
            _shadowEffect = null;
            _agentLoginIDs = null;
            _agentLoginID = null;
            _agentInfo = null;
            _view = null;

            #endregion Nulling variables

            this.Dispatcher.Invoke(DispatcherPriority.Render, GCDelegate);
        }

        /// <summary>
        /// Handles the Activated event of the Window control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="EventArgs"/> instance containing the event data.</param>
        private void Window_Activated(object sender, EventArgs e)
        {
            MainBorder.BorderBrush = new SolidColorBrush((Color)(ColorConverter.ConvertFromString("#0070C5")));
            MainBorder.BitmapEffect = _shadowEffect;
        }

        /// <summary>
        /// Handles the Deactivated event of the Window control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="EventArgs"/> instance containing the event data.</param>
        private void Window_Deactivated(object sender, EventArgs e)
        {
            MainBorder.BorderBrush = Brushes.Black;
            MainBorder.BitmapEffect = null;
        }

        /// <summary>
        /// Handles the KeyDown event of the Window control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="KeyEventArgs"/> instance containing the event data.</param>
        private void Window_KeyDown(object sender, KeyEventArgs e)
        {
            if (Keyboard.Modifiers == ModifierKeys.Alt && (e.SystemKey == Key.Space || e.SystemKey == Key.F4))
            {
                e.Handled = true;
            }
            else
            {
                base.OnKeyDown(e);
            }
            //Code added for the purpose of check caps lock is on alert - Smoorthy
            //03-04-2014
            if (System.Windows.Forms.Control.IsKeyLocked(System.Windows.Forms.Keys.CapsLock))
            {
                _dataContext.CSErrorMessage = "Caps Lock is On";
                _dataContext.CSErrorRowHeight = GridLength.Auto;
                //btnCloseError.Visibility = Visibility.Collapsed;
            }
            else
            {
                _dataContext.CSErrorRowHeight = new GridLength(0);
            }
            //end
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            try
            {
                try
                {
                    var path = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData).ToString() + @"\Pointel\AgentInteractionDesktop\" + _dataContext.UserName;
                    if (File.Exists(path + ".stat.config"))
                        System.IO.File.Move(path + ".stat.config", path + ".config");
                }
                catch (Exception ex)
                {
                    _logger.Info("apdapd : " + ex.ToString());
                }

                SystemMenu = GetSystemMenu(new WindowInteropHelper(this).Handle, false);
                DeleteMenu(SystemMenu, SC_Move, MF_BYCOMMAND);
                DeleteMenu(SystemMenu, SC_Size, MF_BYCOMMAND);
                DeleteMenu(SystemMenu, SC_Maximize, MF_BYCOMMAND);
                var source = PresentationSource.FromVisual(this) as HwndSource;
                source.AddHook(WndProc);

                if (_dataContext.Place == null && !_configContainer.GetAsBoolean("login.enable-place-completion"))
                    btnOk.IsEnabled = false;

                if (ApplicationDeployment.IsNetworkDeployed)
                    if (ConfigurationManager.AppSettings.AllKeys.Contains("application.version"))
                        if (!string.IsNullOrEmpty(ConfigurationManager.AppSettings["application.version"]))
                            channelTitleversion.Content = "V " + ConfigurationManager.AppSettings["application.version"].ToString();
                //Code added for the purpose of check caps lock is on alert - Smoorthy
                //03-04-2014
                if (System.Windows.Forms.Control.IsKeyLocked(System.Windows.Forms.Keys.CapsLock))
                {
                    _dataContext.CSErrorMessage = "Caps Lock is On";
                    _dataContext.CSErrorRowHeight = GridLength.Auto;
                    //btnCloseError.Visibility = Visibility.Collapsed;
                }
                //end

                lblUserName.Content = lblUserName.Content + " " + _dataContext.AgentFullName + ",";
                if (_dataContext.LoadCollection.Count > 0)
                    _dataContext.QueueSelectedValue = _dataContext.LoadCollection.Contains(_dataContext.Queue) ? _dataContext.Queue : _dataContext.LoadCollection[0].ToString();
                if (txtPlace.Text == string.Empty || txtPlace.Text == "")
                {
                    //btnOk.IsEnabled = false;
                    txtPlace.IsEnabled = true;
                    txtPlace.Focus();
                }
                else if (txtPlace.Text != string.Empty && cbQueue.Text == string.Empty || cbQueue.Text == "")
                {
                    cbQueue.Focus();
                }
                string loginId = string.Empty;
                if (_dataContext.UserLoginID != string.Empty)
                {
                    loginId = _dataContext.UserLoginID.Substring(9, _dataContext.UserLoginID.Length - 9);
                }
                //if (_dataContext.UserLoginID == string.Empty || loginId == string.Empty)
                //{
                //    //btnOk.IsEnabled = false;
                //    _dataContext.AgentPasswordRowHeight = new GridLength(0);
                //    _dataContext.CSErrorMessage = "Unable to login. Login id for the user name is not available.";
                //    StartTimerForError();
                //    //_dataContext.CSErrorRowHeight = GridLength.Auto;
                //    //_configSubscribe.DisconnectConfigServer();
                //}

                btnOk.Content = "_" + (string)FindResource("KeyOk");
                btnCancel.Content = "_" + (string)FindResource("KeyCancel");
                if (btnOk.IsEnabled && !isPromptVoiceAgentLoginPassword && !txtPlace.Focusable)
                    btnOk.Focus();

                if (!string.IsNullOrEmpty(_dataContext.Subversion))
                    channelTitleversion.Content += " (" + _dataContext.Subversion + ")";

                //if (_dataContext.AgentLoginIds.Count > 0)
                //{
                //    _agentLoginIDs.Clear();
                //    foreach (var item in _dataContext.AgentLoginIds)
                //    {
                //        _agentLoginIDs.Add(item.ToString());
                //    }
                //}
                if (!string.IsNullOrEmpty(_dataContext.AgentLoginId))
                {
                    _agentLoginID = _dataContext.AgentLoginId;
                }

                gbVoiceChannel.Visibility = Visibility.Collapsed;
                grdRowEmail.Height = new GridLength(0);
                grdRowChat.Height = new GridLength(0);

                if (!string.IsNullOrEmpty(_dataContext.Place))
                {

                    bool isautocompleteplace = true;
                    if (_configContainer.AllKeys.Contains("login.enable-place-completion"))
                        isautocompleteplace = _configContainer.GetAsBoolean("login.enable-place-completion");

                    RemoveSwitch_place_agentLogin(isautocompleteplace);
                }
                else
                    if (!txtPlace.Focusable)
                        txtPlace.Focus();
                //if (_dataContext.KeepRecentPlace)
                //    RemoveSwitch_place_agentLogin();
                PHSItegration();

                txtAgentPassword.Password = GetAgentLoginIDPassword(_configContainer.GetAsString("login.assign.agent.id-password"));
            }
            catch (Exception ex)
            {
                _logger.Error((ex.InnerException == null) ? ex.Message : ex.InnerException.ToString());
            }
        }

        private IntPtr WndProc(IntPtr hwnd, int msg, IntPtr wParam, IntPtr lParam, ref bool handled)
        {
            if (msg != WM_SYSCOMMAND) return IntPtr.Zero;
            //System.Windows.MessageBox.Show(wParam.ToInt32().ToString());
            //if (wParam.ToInt32().ToString() == "61536")
            //{
            //    string d = "d";
            //}
            switch (wParam.ToInt32())
            {
                case SC_Close: //close
                    btnCancel_Click(null, null);
                    handled = true;
                    break;

                case SC_Move: // move
                    MouseLeftButtonDown(null, null);
                    handled = true;
                    break;

                case CU_Minimize: // Minimize
                    WindowState = WindowState.Minimized;
                    handled = true;
                    break;

                default:
                    break;
            }
            return IntPtr.Zero;
        }

        #endregion Methods
    }
}