namespace Agent.Interaction.Desktop
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
    using System.Text;
    using System.Threading;
    using System.Windows;
    using System.Windows.Controls;
    using System.Windows.Data;
    using System.Windows.Documents;
    using System.Windows.Input;
    using System.Windows.Interop;
    using System.Windows.Media;
    using System.Windows.Media.Effects;
    using System.Windows.Media.Imaging;
    using System.Windows.Threading;

    using Agent.Interaction.Desktop.ApplicationReader;
    using Agent.Interaction.Desktop.Helpers;
    using Agent.Interaction.Desktop.Settings;

    using Genesyslab.Platform.ApplicationBlocks.ConfigurationObjectModel;
    using Genesyslab.Platform.ApplicationBlocks.ConfigurationObjectModel.CfgObjects;
    using Genesyslab.Platform.ApplicationBlocks.ConfigurationObjectModel.Queries;
    using Genesyslab.Platform.Commons.Protocols;
    using Genesyslab.Platform.Configuration.Protocols.Types;
    using Genesyslab.Platform.Voice.Protocols.TServer.Events;

    using Pointel.Configuration.Manager;
    using Pointel.Integration.PlugIn;
    using Pointel.Integration.PlugIn.Common;
    using Pointel.Softphone.Voice.Core;

    /// <summary>
    /// Interaction logic for RefinePlace.xaml
    /// </summary>
    public partial class RefinePlace : Window
    {
        #region Fields

        private const int GWL_STYLE = -16; //WPF's Message code for Title Bar's Style
        private const int WS_SYSMENU = 0x80000; //WPF's Message code for System Menu

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
        private bool isEnableLoginQueue = false;
        private bool isEnableOutboundUnactiveChannel = false;
        private bool isEnableVoiceUnactivateChannel = false;
        private bool IsOutboundPluginEnabled = false;
        private bool isPromptVoiceAgentLoginId = false;
        private bool isPromptVoiceAgentLoginPassword = false;
        private bool isStartErrorTimer = false;
        private bool IsStatPluginEnabled = false;
        private bool istxtPlaceFocued = false;
        private int selectedTServerSwitchDBId;
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
        private Pointel.Logger.Core.ILog _logger = Pointel.Logger.Core.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType, "AID");
        private Dictionary<string, bool> _lstHoldingLocalChannelSelection = new Dictionary<string, bool>();
        private string _previousAgentLoginId = string.Empty;
        private string _previousPlace = string.Empty;
        private string _previousQueue = string.Empty;
        private DropShadowBitmapEffect _shadowEffect = new DropShadowBitmapEffect();
        private Dictionary<string, int> _tempAgentLoginIDs;
        private DispatcherTimer _timerforcloseError = null;
        private SoftPhoneBar _view;

        #endregion Fields

        #region Constructors

        public RefinePlace()
        {
            InitializeComponent();
            this.DataContext = Datacontext.GetInstance();
            selectedTServerSwitchDBId = Datacontext.UsedTServerSwitchDBId;
            Datacontext.isCalledRefinePlace = false;
            _shadowEffect.ShadowDepth = 0;
            _shadowEffect.Opacity = 0.5;
            _shadowEffect.Softness = 0.5;
            _shadowEffect.Color = (Color)ColorConverter.ConvertFromString("#003660");
            _dataContext.CSErrorMessage = string.Empty;
            _dataContext.CSErrorRowHeight = new GridLength(0);
            _isMaxWrapTimeSet = int.TryParse(_configContainer.GetAsString("voice.agent.wrapup.time-max-value"), out _agentWrapUpTimeMaxValue);
            LoadConfigValues();
            var window = IsWindowOpen<Window>("SoftphoneBar");
            if (window != null)
            {
                _view = (SoftPhoneBar)window;
            }
        }

        #endregion Constructors

        #region Methods

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

        /// <summary>
        /// Checks the place status.
        /// </summary>
        /// <param name="placeName">Name of the place.</param>
        /// <returns></returns>
        public bool CheckPlaceStatus(string placeName)
        {
            bool returnValue = true;
            try
            {
                var placeQuery = new CfgPlaceQuery();
                placeQuery.Name = placeName;
                var place = _configContainer.ConfServiceObject.RetrieveObject<CfgPlace>(placeQuery);
                var dnCollection = (IList<CfgDN>)place.DNs;
                if (dnCollection != null && dnCollection.Count > 0)
                {
                    foreach (CfgDN item in dnCollection)
                    {
                        if ((item.Switch.Type.ToString().Contains("nortel") && item.Type == CfgDNType.CFGACDPosition) ||
                            (!item.Switch.Type.ToString().Contains("nortel") && item.Type == CfgDNType.CFGExtension))
                        {
                            var regDN = new SoftPhone();
                            IMessage result = regDN.RegisterDNRequest(item.Number);
                            if (result is EventRegistered && ((EventRegistered)result).AgentID != null && ((EventRegistered)result).AgentID != _dataContext.AgentLoginId)
                            {
                                CfgAgentLoginQuery cfgALQ = new CfgAgentLoginQuery() { LoginCode = ((EventRegistered)result).AgentID, SwitchDbid = item.Switch.DBID };
                                cfgALQ.TenantDbid = _configContainer.TenantDbId;
                                CfgAgentLogin cfgAL = _configContainer.ConfServiceObject.RetrieveObject<CfgAgentLogin>(cfgALQ);
                                CfgPersonQuery cfgPQ = new CfgPersonQuery() { LoginDbid = cfgAL.DBID };
                                CfgPerson cfgP = _configContainer.ConfServiceObject.RetrieveObject<CfgPerson>(cfgPQ);
                                txtPlace.IsEnabled = true;
                                _dataContext.CSErrorMessage = "Unable to login. Place is already taken by " + cfgP.LastName + " " + cfgP.FirstName + ".";
                                StartTimerForError();
                                _dataContext.CSErrorRowHeight = GridLength.Auto;
                                regDN.RequestUnRegisterDN();
                                returnValue = false;
                                break;
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                _logger.Error("Error occurred while checking the place status as : " + ex.ToString());
            }
            return returnValue;
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

                if (_configContainer.AllKeys.Contains("voice.enable.modify.agent-wrapup-time") && _configContainer.GetValue("voice.enable.modify.agent-wrapup-time") != null)
                    _isEnableWrapUpTime = ((string)(_configContainer.GetValue("voice.enable.modify.agent-wrapup-time"))).ToLower().Equals("true");

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
                            _dataContext.AgentLoginId = loginDetails.AgentLogin.LoginCode.ToString();
                            _agentLoginIDs.Add(new KeyValuePair<string, string>(loginDetails.AgentLogin.LoginCode.ToString(), loginDetails.AgentLogin.Switch.Name));
                            if (!_dataContext.AgentLoginIds.Contains(loginDetails.AgentLogin.LoginCode.ToString()))
                                _dataContext.AgentLoginIds.Add(loginDetails.AgentLogin.LoginCode.ToString());
                            if (loginDetails.WrapupTime != 0)
                                _dataContext.agentWrapUpTime = loginDetails.WrapupTime;
                        }
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
                //if (_configContainer.AllKeys.Contains("AllPlaces") &&
                //                                    _configContainer.GetValue("AllPlaces") != null)
                //{
                //    _dataContext.AutoComplete.Clear();
                //    //foreach (var item in _configContainer.GetValue("AllPlaces"))
                //    _dataContext.AutoComplete = _configContainer.GetValue("AllPlaces");
                //    _dataContext.AutoComplete.Sort();
                //}
            }
            catch (Exception ex)
            {
                _logger.Error("Error occurred while loading config values " + ex.ToString());
            }
        }

        [DllImport("user32.dll", SetLastError = true)]
        private static extern int GetWindowLong(IntPtr hWnd, int nIndex);

        [DllImport("user32.dll")]
        private static extern int SetWindowLong(IntPtr hWnd, int nIndex, int dwNewLong);

        private void btnCancel_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
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
                if (CheckForLogin(true))
                {
                    Datacontext.isCalledRefinePlace = true;
                    _logger.Info("ChannelSelection:btnOk_Clicked");
                    string place;
                    if (_dataContext.AutoComplete != null)
                    {
                        place = _dataContext.AutoComplete.Find(s => s.Equals(txtPlace.Text, StringComparison.OrdinalIgnoreCase));
                        _dataContext.Place = place.Trim();
                    }
                    else if (string.IsNullOrEmpty(_dataContext.Place) && CheckPlaceValid(txtPlace.Text))
                        _dataContext.Place = txtPlace.Text.Trim();
                    _dataContext.UserPlace = "Using " + _dataContext.Place;
                    _logger.Info("Place:" + _dataContext.Place);
                    _logger.Info("Queue:" + _dataContext.Queue);
                    if (CheckPlaceStatus(_dataContext.Place))
                    {
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
                                if (txtPlace.Text != _previousPlace)
                                {
                                    _xmlHandler.ModifyXmlData(_dataContext.SettingsXMLFile, _xmlHandler.ConfigKeys[XMLHandler.Keys.KeepPlace].ToString(), "False");
                                    _xmlHandler.ModifyXmlData(_dataContext.SettingsXMLFile, _xmlHandler.ConfigKeys[XMLHandler.Keys.Place].ToString(), _dataContext.Place);
                                }
                                if (cbQueue.SelectedItem != null && cbQueue.SelectedItem.ToString() != _previousQueue)
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
                            if (_dataContext.AgentPassword != txtAgentPassword.Password)
                                _dataContext.AgentPassword = txtAgentPassword.Password;
                            //check for same switch in place and login id match
                            if (CheckSwitch_place_agentLogin())
                            {
                                //if (_dataContext.IsVoiceChecked)
                                //{
                                if (!_dataContext.lstHoldingChannelSelection.ContainsKey(chkbxVoice.Content.ToString()))
                                {
                                    _dataContext.lstHoldingChannelSelection.Add(chkbxVoice.Content.ToString(), chkbxVoice.IsChecked.Value);
                                }
                                else if (_dataContext.lstHoldingChannelSelection[chkbxVoice.Content.ToString()] != chkbxVoice.IsChecked.Value)
                                {
                                    _dataContext.lstHoldingChannelSelection[chkbxVoice.Content.ToString()] = chkbxVoice.IsChecked.Value;
                                }
                                //}
                                //else
                                //{
                                //_dataContext.lstHoldingChannelSelection.Add(chkbxVoice.Content.ToString(), chkbxVoice.IsChecked.Value);
                                //}
                                //if (_dataContext.IsEmailChecked)
                                //{
                                if (!_dataContext.lstHoldingChannelSelection.ContainsKey(chkbxEmail.Content.ToString()))
                                {
                                    _dataContext.lstHoldingChannelSelection.Add(chkbxEmail.Content.ToString(), chkbxEmail.IsChecked.Value);
                                }
                                else if (_dataContext.lstHoldingChannelSelection[chkbxEmail.Content.ToString()] != chkbxEmail.IsChecked.Value)
                                {
                                    _dataContext.lstHoldingChannelSelection[chkbxEmail.Content.ToString()] = chkbxEmail.IsChecked.Value;
                                }
                                //}
                                //else
                                //{
                                //    _dataContext.lstHoldingChannelSelection.Add(chkbxEmail.Content.ToString(), chkbxEmail.IsChecked.Value);
                                //}
                                //Check chat checkbox is checked
                                //if (_dataContext.IsChatChecked)
                                //{
                                if (!_dataContext.lstHoldingChannelSelection.ContainsKey(chkbxChat.Content.ToString()))
                                {
                                    _dataContext.lstHoldingChannelSelection.Add(chkbxChat.Content.ToString(), chkbxChat.IsChecked.Value);
                                }
                                else if (_dataContext.lstHoldingChannelSelection[chkbxChat.Content.ToString()] != chkbxChat.IsChecked.Value)
                                {
                                    _dataContext.lstHoldingChannelSelection[chkbxChat.Content.ToString()] = chkbxChat.IsChecked.Value;
                                }
                                //}
                                //else
                                //{
                                //    _dataContext.lstHoldingChannelSelection.Add(chkbxChat.Content.ToString(), chkbxChat.IsChecked.Value);
                                //}
                                //if (_dataContext.IsOutboundChecked)
                                //{
                                if (!_dataContext.lstHoldingChannelSelection.ContainsKey(chkbxOutbound.Content.ToString()))
                                {
                                    _dataContext.lstHoldingChannelSelection.Add(chkbxOutbound.Content.ToString(), chkbxOutbound.IsChecked.Value);
                                }
                                else if (_dataContext.lstHoldingChannelSelection[chkbxOutbound.Content.ToString()] != chkbxOutbound.IsChecked.Value)
                                {
                                    _dataContext.lstHoldingChannelSelection[chkbxOutbound.Content.ToString()] = chkbxOutbound.IsChecked.Value;
                                }
                                //}
                                //else
                                //    _dataContext.lstHoldingChannelSelection.Add(chkbxOutbound.Content.ToString(), chkbxOutbound.IsChecked.Value);
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
                                _logger.Debug("Reading selected DN.");
                                try
                                {
                                    (new ConfigManager()).ReadAllDns(_cfgSwitch.DBID);

                                    if (_configContainer.AllKeys.Contains("ForwardDns") &&
                                                                        _configContainer.GetValue("ForwardDns") != null)
                                    {
                                        _dataContext.ForwardDns.Clear();
                                        foreach (var item in _configContainer.GetValue("ForwardDns"))
                                            _dataContext.ForwardDns.Add(item);
                                        _dataContext.ForwardDns.Sort();
                                    }
                                    _logger.Debug("Read DN and forward collection set.");
                                }
                                catch (Exception ex)
                                {
                                    _logger.Error("Error occurred as : " + ex.Message);
                                }

                                _dataContext.AgentPassword = txtAgentPassword.Password.ToString();
                                //voice refine place
                                if (txtPlace.Text != _previousPlace || (cbQueue.SelectedItem != null && cbQueue.SelectedItem.ToString() != _previousQueue) || _dataContext.AgentLoginId != _previousAgentLoginId)
                                {
                                    if (!_dataContext.htMediaCurrentState[Datacontext.Channels.Voice].Contains("Logout"))
                                    {
                                        var softPohne = new SoftPhone();
                                        //softPohne.Logout();
                                        IMessage message = softPohne.RequestLogoutAgent();
                                        if (message != null)
                                        {
                                            switch (message.Id)
                                            {
                                                case EventAgentLogout.MessageId:
                                                    EventAgentLogout eventAgentLogout = (EventAgentLogout)message;
                                                    softPohne.RequestUnRegisterDN();
                                                    break;

                                                case EventError.MessageId:
                                                    EventError eventError = (EventError)message;
                                                    break;
                                            }
                                        }
                                    }
                                    try
                                    {
                                        _dataContext.SwitchType = _cfgSwitch;
                                        _dataContext.SwitchName = _dataContext.SwitchType.Type == CfgSwitchType.CFGLucentDefinityG3 ? "avaya" : ((_dataContext.SwitchType.Type == CfgSwitchType.CFGNortelDMS100 || _dataContext.SwitchType.Type == CfgSwitchType.CFGNortelMeridianCallCenter) ? "nortel" : "avaya");
                                        //(new Pointel.Configuration.Manager.ConfigManager()).ReadAllDns((int)_cfgSwitch.DBID);

                                        //if (_configContainer.AllKeys.Contains("ForwardDns") &&
                                        //                                    _configContainer.GetValue("ForwardDns") != null)
                                        //{
                                        //    //ForwardDNs = _configContainer.GetValue("ForwardDns");
                                        //    _dataContext.ForwardDns.Clear();
                                        //    foreach (var item in _configContainer.GetValue("ForwardDns"))
                                        //        _dataContext.ForwardDns.Add(item);
                                        //    _dataContext.ForwardDns.Sort();
                                        //}
                                    }
                                    catch (Exception ex)
                                    {
                                        _logger.Warn("Error occurred while setting switch type and read dn's for forward. " + ex.Message);
                                    }

                                    if (_view != null)
                                    {
                                        if (Datacontext.UsedTServerSwitchDBId != 0 && Datacontext.UsedTServerSwitchDBId == selectedTServerSwitchDBId)
                                            _view.InitializeRefinePlaceforVoice();
                                        else
                                        {
                                            selectedTServerSwitchDBId = Datacontext.UsedTServerSwitchDBId;
                                            _view.InitializeVoiceMedia();
                                        }
                                    }
                                }
                                else
                                {
                                    if (_dataContext.lstHoldingChannelSelection.ContainsKey("voice") && !Convert.ToBoolean(_dataContext.lstHoldingChannelSelection["voice"]))
                                    {
                                        _dataContext.VoiceNotReadyReasonCode = string.Empty;
                                        _logger.Debug("Agent Logout is in process");
                                        _view.VoiceStateChange(Agent.Interaction.Desktop.SoftPhoneBar.AgentStateType.Channel, "Logout");
                                    }
                                    else if (_dataContext.lstHoldingChannelSelection.ContainsKey("voice") && Convert.ToBoolean(_dataContext.lstHoldingChannelSelection["voice"]))
                                    {
                                        if (_view != null && _dataContext.htMediaCurrentState.ContainsKey(Datacontext.Channels.Voice) && _dataContext.htMediaCurrentState[Datacontext.Channels.Voice].ToString().Contains("Logout"))
                                            _view.VoiceStateChange(SoftPhoneBar.AgentStateType.Channel, "Log On");
                                    }
                                }
                                bool isInitializedIXNMedia = false;
                                //ixn medias refine place
                                if (txtPlace.Text != _previousPlace && ((IsChatPluginEnabled && _dataContext.IsChatPluginAdded) || (IsEmailPluginEnabled && _dataContext.IsEmailPluginAdded)))
                                {
                                    if (_view != null)
                                    {
                                        _view.InitializeIXNMedias();
                                        isInitializedIXNMedia = true;
                                    }
                                }
                                else
                                {
                                    if (_dataContext.lstHoldingChannelSelection.ContainsKey("email") && !Convert.ToBoolean(_dataContext.lstHoldingChannelSelection["email"]))
                                    {
                                        if (_view != null)
                                            _view.InteractionsMediaStateChange(Datacontext.Channels.Email, "Logout");
                                    }
                                    else if (!isInitializedIXNMedia)
                                    {
                                        if (_view != null && _dataContext.htMediaCurrentState.ContainsKey(Datacontext.Channels.Email) && _dataContext.htMediaCurrentState[Datacontext.Channels.Email].ToString().Contains("Logout"))
                                            _view.InteractionsMediaStateChange(Datacontext.Channels.Email, "Log On");
                                    }
                                    if (_dataContext.lstHoldingChannelSelection.ContainsKey("chat") && !Convert.ToBoolean(_dataContext.lstHoldingChannelSelection["chat"]))
                                    {
                                        if (_view != null)
                                            _view.InteractionsMediaStateChange(Datacontext.Channels.Chat, "Logout");
                                    }
                                    else if (!isInitializedIXNMedia)
                                    {
                                        if (_view != null && _dataContext.htMediaCurrentState.ContainsKey(Datacontext.Channels.Chat) && _dataContext.htMediaCurrentState[Datacontext.Channels.Chat].ToString().Contains("Logout"))
                                            _view.InteractionsMediaStateChange(Datacontext.Channels.Chat, "Log On");
                                    }
                                }

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
                            CloseWindow:
                                if (closewindow)
                                {
                                    this.Close();
                                }
                            }
                        }
                    }
                    else
                    {
                        _logger.Error("Unable to login. Place is already taken by another agent");
                        goto End;
                    }
                }
                cbQueue.Text = cbQueue.SelectedItem != null ? cbQueue.SelectedItem.ToString() : cbQueue.Items[0].ToString();
            }
            catch (Exception commonException)
            {
                _logger.Error("channelSelection:" + commonException.ToString());
            }
        End:
            return;
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

        private void channelWindow_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            //DataContext = null;
        }

        private void channelWindow_KeyDown(object sender, KeyEventArgs e)
        {
        }

        private void channelWindow_Loaded(object sender, RoutedEventArgs e)
        {
            try
            {
                var hwnd = new WindowInteropHelper(this).Handle;
                SetWindowLong(hwnd, GWL_STYLE, GetWindowLong(hwnd, GWL_STYLE) & ~WS_SYSMENU);
                if (ApplicationDeployment.IsNetworkDeployed)
                    if (ConfigurationManager.AppSettings.AllKeys.Contains("application.version"))
                        if (!string.IsNullOrEmpty(ConfigurationManager.AppSettings["application.version"]))
                            channelTitleversion.Content = "V " + ConfigurationManager.AppSettings["application.version"].ToString();

                if (_dataContext.Place == null && !_configContainer.GetAsBoolean("login.enable-place-completion"))
                    btnOk.IsEnabled = false;
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

                btnOk.Content = "_" + (string)FindResource("KeyOk");
                btnCancel.Content = "_" + (string)FindResource("KeyCancel");
                if (btnOk.IsEnabled && !isPromptVoiceAgentLoginPassword)
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
                    txtPlace.Focus();
                //if (_dataContext.KeepRecentPlace)
                //    RemoveSwitch_place_agentLogin();
                _previousPlace = _dataContext.Place;
                _previousQueue = _dataContext.Queue;
                _previousAgentLoginId = _dataContext.AgentLoginId;

                if (_lstHoldingLocalChannelSelection != null)
                    _lstHoldingLocalChannelSelection.Clear();
            }
            catch (Exception ex)
            {
                _logger.Error((ex.InnerException == null) ? ex.Message : ex.InnerException.ToString());
            }
        }

        private void channelWindow_PreviewKeyDown(object sender, KeyEventArgs e)
        {
            if ((Keyboard.IsKeyDown(Key.LeftAlt) || Keyboard.IsKeyDown(Key.RightAlt)) && Keyboard.IsKeyDown(Key.F4))
                e.Handled = true;
        }

        private void channelWindow_StateChanged(object sender, EventArgs e)
        {
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
            }
            catch (Exception generalException)
            {
                _logger.Error("Error occurred while channelWindow_Unloaded() :" + generalException.ToString());
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
                //if (isStartErrorTimer)
                //    CloseError(null, null);
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
                lblQueue.IsEnabled = true;
                cbQueue.IsEnabled = true;
                lblAgentLogin.IsEnabled = true;
                lblAgentLoginID.IsEnabled = true;
                cbAgentLoginID.IsEnabled = true;
                lblAgentPassword.IsEnabled = true;
                txtAgentPassword.IsEnabled = true;
                if (txtPlace.Text != string.Empty && cbQueue.Text != string.Empty && isPromptVoiceAgentLoginPassword)
                    txtAgentPassword.Focus();
                if (!_dataContext.lstHoldingChannelSelection.ContainsKey(chkbxVoice.Content.ToString()))
                {
                    _dataContext.lstHoldingChannelSelection.Add(chkbxVoice.Content.ToString(), chkbxVoice.IsChecked != null && chkbxVoice.IsChecked.Value);
                }
            }
            catch (Exception commonException)
            {
                _logger.Error("ChannelSelection:chkbxVoice_Checked:" + commonException.ToString());
            }
        }

        /// <summary>
        /// Handles the Unchecked event of the chkbxVoice control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="RoutedEventArgs"/> instance containing the event data.</param>
        private void chkbxVoice_Unchecked(object sender, RoutedEventArgs e)
        {
            try
            {
                lblQueue.IsEnabled = false;
                cbQueue.IsEnabled = false;
                lblAgentLogin.IsEnabled = false;
                lblAgentLoginID.IsEnabled = false;
                cbAgentLoginID.IsEnabled = false;
                lblAgentPassword.IsEnabled = false;
                txtAgentPassword.IsEnabled = false;
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
                    isStartErrorTimer = false;
                    _timerforcloseError.Stop();
                    _timerforcloseError.Tick -= CloseError;
                    _timerforcloseError = null;
                }
            }
            catch (Exception ex)
            {
                _logger.Error("Error occurred as  " + ex.Message);
            }
        }

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

        /// <summary>
        /// Medias the availability check.
        /// </summary>
        private void mediaAvailabilityCheck()
        {
            try
            {
                if (!_dataContext.lstHoldingChannelSelection.ContainsKey(chkbxVoice.Content.ToString()))
                    _dataContext.lstHoldingChannelSelection.Add(chkbxVoice.Content.ToString(), chkbxVoice.IsChecked != null && chkbxVoice.IsChecked.Value);
                else
                    _dataContext.lstHoldingChannelSelection[chkbxVoice.Content.ToString()] = chkbxVoice.IsChecked != null && chkbxVoice.IsChecked.Value;
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
                if (IsChatPluginEnabled && _dataContext.IsChatPluginAdded && Datacontext.AvailableServerDic.ContainsValue(CfgAppType.CFGInteractionServer.ToString())
                    && Datacontext.AvailableServerDic.ContainsValue(CfgAppType.CFGChatServer.ToString()))
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
                        //grd_VoiceLoginDetails.Visibility = System.Windows.Visibility.Collapsed;
                        //lblAgentLogin.Visibility = System.Windows.Visibility.Hidden;
                        //lblAgentLoginID.Visibility = System.Windows.Visibility.Hidden;
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
                    lblAgentPassword.Visibility = System.Windows.Visibility.Hidden;
                    txtAgentPassword.Visibility = System.Windows.Visibility.Hidden;
                    _dataContext.AgentPasswordRowHeight = new GridLength(0);
                }
                if (_isEnableWrapUpTime)
                {
                    _dataContext.WrapTimeRowHeight = GridLength.Auto;
                    lblWrapupTime.Visibility = System.Windows.Visibility.Visible;
                    txtWrapupTime.Visibility = System.Windows.Visibility.Visible;
                }
                else
                {
                    _dataContext.WrapTimeRowHeight = new GridLength(0);
                    lblWrapupTime.Visibility = System.Windows.Visibility.Collapsed;
                    txtWrapupTime.Visibility = System.Windows.Visibility.Collapsed;
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
            }
            catch (Exception generalExcption)
            {
                _logger.Error("Error occurred in mediaAvailabilityCheck() : " + generalExcption.ToString());
            }
        }

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
                _logger.Info("Place DN switch name : " + _cfgDN.Switch.Name);
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

                //if (_agentLoginIDs.Count < 0 || temp.Count == 1)
                //{
                //    Genesyslab.Platform.ApplicationBlocks.ConfigurationObjectModel.Queries.CfgAgentLoginQuery _cfgAgentLoginQuery = new Genesyslab.Platform.ApplicationBlocks.ConfigurationObjectModel.Queries.CfgAgentLoginQuery();
                //    _cfgAgentLoginQuery.LoginCode = _agentLoginID;
                //    _cfgAgentLoginQuery.Dbid = _tempAgentLoginIDs[_agentLoginID];
                //    _cfgAgentLoginQuery.TenantDbid = _configContainer.TenantDbId;
                //    //var agentInfo = (ICollection<Genesyslab.Platform.ApplicationBlocks.ConfigurationObjectModel.CfgObjects.CfgAgentLoginInfo>)_dataContext.Person.AgentInfo.AgentLogins;
                //    //foreach (var loginDetails in agentInfo)
                //    //{
                //    //    if (_cfgAgentLoginQuery.LoginCode == loginDetails.AgentLogin.LoginCode)
                //    //    {
                //    //        _cfgAgentLoginQuery.Dbid = loginDetails.AgentLogin.DBID;
                //    //        break;
                //    //    }
                //    //}
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
                isStartErrorTimer = true;
                _dataContext.CSErrorRowHeight = GridLength.Auto;
                //btnCloseError.Visibility = Visibility.Visible;
                DispatcherTimer _timerforcloseError = new DispatcherTimer();
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
            // btnOk.IsEnabled = true;
            switch (e.Key)
            {
                case Key.D0:
                    if (Keyboard.Modifiers == ModifierKeys.Shift)
                        e.Handled = true;
                    break;

                case Key.D1:
                    if (Keyboard.Modifiers == ModifierKeys.Shift)
                        e.Handled = true;
                    break;

                case Key.D2:
                    if (Keyboard.Modifiers == ModifierKeys.Shift)
                        e.Handled = true;
                    break;

                case Key.D3:
                    if (Keyboard.Modifiers == ModifierKeys.Shift)
                        e.Handled = true;
                    break;

                case Key.D4:
                    if (Keyboard.Modifiers == ModifierKeys.Shift)
                        e.Handled = true;
                    break;

                case Key.D5:
                    if (Keyboard.Modifiers == ModifierKeys.Shift)
                        e.Handled = true;
                    break;

                case Key.D6:
                    if (Keyboard.Modifiers == ModifierKeys.Shift)
                        e.Handled = true;
                    break;

                case Key.D7:
                    if (Keyboard.Modifiers == ModifierKeys.Shift)
                        e.Handled = true;
                    break;

                case Key.D8:
                    if (Keyboard.Modifiers == ModifierKeys.Shift)
                        e.Handled = true;
                    break;

                case Key.D9:
                    if (Keyboard.Modifiers == ModifierKeys.Shift)
                        e.Handled = true;
                    break;

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
                    if (string.IsNullOrEmpty(txtWrapupTime.Text) && NumericKeyboardvalue(e.Key) != null)
                    {
                        e.Handled = false;
                        break;
                    }
                    else
                    {
                        var num = NumericKeyboardvalue(e.Key);
                        var portText = txtWrapupTime.Text;
                        if (!string.IsNullOrEmpty(txtWrapupTime.SelectedText))
                            portText = portText.Replace(txtWrapupTime.SelectedText, "");
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

        #endregion Methods
    }
}