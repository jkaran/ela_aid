namespace Agent.Interaction.Desktop
{
    using System;
    using System.Collections.Generic;
    using System.Configuration;
    using System.Deployment.Application;
    using System.Diagnostics;
    using System.Globalization;
    using System.IO;
    using System.Linq;
    using System.Net;
    using System.Reflection;
    using System.Runtime.InteropServices;
    using System.Security.Principal;
    using System.Threading;
    using System.Windows;
    using System.Windows.Controls;
    using System.Windows.Forms;
    using System.Windows.Input;
    using System.Windows.Interop;
    using System.Windows.Markup;
    using System.Windows.Media;
    using System.Windows.Media.Effects;
    using System.Windows.Threading;

    using Agent.Interaction.Desktop.ApplicationReader;
    using Agent.Interaction.Desktop.Helpers;
    using Agent.Interaction.Desktop.Settings;

    using Genesyslab.Platform.ApplicationBlocks.ConfigurationObjectModel.CfgObjects;
    using Genesyslab.Platform.ApplicationBlocks.ConfigurationObjectModel.Queries;
    using Genesyslab.Platform.Commons.Collections;
    using Genesyslab.Platform.Configuration.Protocols.Types;

    using Pointel.Configuration.Manager;

    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class Login : Window
    {
        #region Fields

        public const Int32 MF_BYPOSITION = 0x400;

        public string CultureCode = "en-US";

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
        private IntPtr SystemMenu;
        private Uri _backupServerUri = null;

        //private string _clientName = string.Empty;
        private ConfigContainer _configContainer = ConfigContainer.Instance();
        private Datacontext _dataContext = Datacontext.GetInstance();
        private string _frmAdminpassword = string.Empty;
        private string _frmAdminuserName = string.Empty;
        private string _host = string.Empty;

        //private readonly ILog _logger = LogManager.GetLogger(typeof(Login));
        private Pointel.Logger.Core.ILog _logger = Pointel.Logger.Core.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType,
            "AID");
        private Dictionary<string, string> _matchingipport = new Dictionary<string, string>();
        private string _place = string.Empty;
        private string _port = string.Empty;
        private string _previouslang = string.Empty;
        private Uri _serverUri = null;
        private DropShadowBitmapEffect _shadowEffect = new DropShadowBitmapEffect();
        private DispatcherTimer _timerforcloseError;
        private XMLHandler _xmlHandler = new XMLHandler();

        #endregion Fields

        #region Constructors

        //Do not delete this code is to create the ghost window used for CME subscribe
        public Login(string empty)
        {
            this.Name = "loginWindow";
        }

        // A sample assembly reference class that would exist in the `Core` project.
        public Login()
        {
            try
            {
                InstallFont();
                //CaseWindow _caseWindow = new CaseWindow();
                //_caseWindow.Show();
                InitializeComponent();
                Closing += Login_Closing;
                CheckPlugins();
                this.DataContext = Datacontext.GetInstance();
                _dataContext.LoginWindow = this;
                _shadowEffect.ShadowDepth = 0;
                _shadowEffect.Opacity = 0.5;
                _shadowEffect.Softness = 0.5;
                _shadowEffect.Color = (Color)ColorConverter.ConvertFromString("#003660");
                // _dataContext.ErrorRowHeight = new GridLength(0);
                CloseError(null, null);
                btnLogin.IsEnabled = true;
                if (_dataContext.LanguageList != null) return;
                var languages = new List<string>(ConfigurationManager.AppSettings["Languages"].Split(new char[] { ';' }));
                _dataContext.LanguageList = new List<Languages>();
                foreach (var nameCode in languages.Select(item => item.Split(new char[] { '@' })))
                    _dataContext.LanguageList.Add(new Languages() { Code = nameCode[1], Name = nameCode[0] });
                _dataContext.SelectedLanguage = _dataContext.LanguageList[_dataContext.LanguageList.FindIndex(s => s.Code == CultureCode)];
                _previouslang = _dataContext.SelectedLanguage.Code;
            }
            catch (Exception ex)
            {
                _logger.Error(ex.Message.ToString());
            }
        }

        #endregion Constructors

        #region Methods

        [DllImport("gdi32.dll", EntryPoint = "AddFontResourceW", SetLastError = true)]
        public static extern int AddFontResource([In][MarshalAs(UnmanagedType.LPWStr)] string lpFileName);

        public static void ClearClipboard()
        {
            if (!OpenClipboard(System.IntPtr.Zero)) return;
            EmptyClipboard();
            CloseClipboard();
        }

        [DllImport("user32.dll")]
        public static extern int SendMessage(int hWnd, uint msg, int wParam, int lParam);

        /// <summary>
        /// Closes the error.
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="e">The <see cref="EventArgs"/> instance containing the event data.</param>
        public void CloseError(object sender, EventArgs e)
        {
            try
            {
                _dataContext.ErrorMessage = string.Empty;
                _dataContext.ErrorRowHeight = new GridLength(0);
                btnLogin.IsEnabled = true;
                if (_timerforcloseError != null)
                {
                    _timerforcloseError.Stop();
                    _timerforcloseError.Tick -= CloseError;
                    _timerforcloseError = null;
                }
            }
            catch (Exception ex)
            {
                _logger.Error("Error occurred at CloseSignatureError : " + ex.Message);
            }
        }

        /// <summary>
        /// Does the font exist.
        /// </summary>
        /// <param name="fontFamilyName">Name of the font family.</param>
        /// <returns></returns>
        public bool DoesFontExist(string fontFamilyName)
        {
            bool result;
            try
            {
                result = (from f in Fonts.SystemFontFamilies where f.Source.Equals(fontFamilyName) select f).Any();
            }
            catch (ArgumentException)
            {
                result = false;
            }
            return result;
        }

        /// <summary>
        /// Gets the XML value assign to control.
        /// </summary>
        public void GetXMLValueAssignToControl()
        {
            try
            {
                if (!Directory.Exists(_dataContext.TempFile))
                    Directory.CreateDirectory(_dataContext.TempFile);

                if (File.Exists(_dataContext.SettingsXMLFile))
                {
                    var xmldata = _xmlHandler.ReadAllXmlData(_dataContext.SettingsXMLFile);
                    if (xmldata != null)
                    {
                        _dataContext.ApplicationName = (xmldata.ContainsKey(_xmlHandler.ConfigKeys[XMLHandler.Keys.ApplicationName].ToString()) ?
                            (string.IsNullOrEmpty(xmldata[_xmlHandler.ConfigKeys[XMLHandler.Keys.ApplicationName].ToString()]) ? _dataContext.ApplicationName : xmldata[_xmlHandler.ConfigKeys[XMLHandler.Keys.ApplicationName].ToString()]) :
                            (string.IsNullOrEmpty(_dataContext.ApplicationName) ? string.Empty : _dataContext.ApplicationName));

                        _dataContext.HostNameText = _dataContext.HostNameSelectedValue = (xmldata.ContainsKey(_xmlHandler.ConfigKeys[XMLHandler.Keys.Host].ToString()) ?
                            (string.IsNullOrEmpty(xmldata[_xmlHandler.ConfigKeys[XMLHandler.Keys.Host].ToString()]) ? _dataContext.HostNameText : xmldata[_xmlHandler.ConfigKeys[XMLHandler.Keys.Host].ToString()]) :
                            (string.IsNullOrEmpty(_dataContext.HostNameText) ? string.Empty : _dataContext.HostNameText));

                        _dataContext.PortText = _dataContext.PortSelectedValue = (xmldata.ContainsKey(_xmlHandler.ConfigKeys[XMLHandler.Keys.Port].ToString()) ?
                            (string.IsNullOrEmpty(xmldata[_xmlHandler.ConfigKeys[XMLHandler.Keys.Port].ToString()]) ? _dataContext.PortText : xmldata[_xmlHandler.ConfigKeys[XMLHandler.Keys.Port].ToString()]) :
                            (string.IsNullOrEmpty(_dataContext.PortText) ? string.Empty : _dataContext.PortText));

                        _dataContext.UserName = (xmldata.ContainsKey(_xmlHandler.ConfigKeys[XMLHandler.Keys.UserName].ToString()) ? xmldata[_xmlHandler.ConfigKeys[XMLHandler.Keys.UserName].ToString()] : string.Empty);
                        string[] tempServeriphost = ((xmldata.ContainsKey(_xmlHandler.ConfigKeys[XMLHandler.Keys.ConfigServers].ToString()) ? xmldata[_xmlHandler.ConfigKeys[XMLHandler.Keys.ConfigServers].ToString()] : string.Empty)).Split(',');
                        foreach (var item in tempServeriphost)
                        {
                            if (item.Contains(":"))
                            {
                                if (!_matchingipport.ContainsKey((item.Split(':'))[0].ToString()))
                                    _matchingipport.Add(((item.Split(':'))[0].ToString()), ((item.Split(':'))[1].ToString()));
                                else
                                {
                                    if (!_matchingipport[(item.Split(':'))[0].ToString()].ToString().Contains((item.Split(':'))[1].ToString()))
                                        _matchingipport[(item.Split(':'))[0].ToString()] = _matchingipport[(item.Split(':'))[0].ToString()].ToString() + "," + (item.Split(':'))[1].ToString();
                                }
                                if (!_dataContext.HostNameItemSource.Contains((item.Split(':'))[0].ToString()))
                                    _dataContext.HostNameItemSource.Add((item.Split(':'))[0].ToString());
                                if (!_dataContext.PortItemSource.Contains((item.Split(':'))[1].ToString()))
                                    _dataContext.PortItemSource.Add((item.Split(':'))[1].ToString());
                            }
                        }
                        if (!string.IsNullOrEmpty(_dataContext.HostNameText) && !_dataContext.HostNameItemSource.Contains(_dataContext.HostNameText))
                            _dataContext.HostNameItemSource.Add(_dataContext.HostNameText);
                        if (!string.IsNullOrEmpty(_dataContext.PortText) && !_dataContext.PortItemSource.Contains(_dataContext.PortText))
                            _dataContext.PortItemSource.Add(_dataContext.PortText);
                        _place = (xmldata.ContainsKey(_xmlHandler.ConfigKeys[XMLHandler.Keys.Place].ToString()) ? xmldata[_xmlHandler.ConfigKeys[XMLHandler.Keys.Place].ToString()] : string.Empty);
                        _dataContext.Queue = (xmldata.ContainsKey(_xmlHandler.ConfigKeys[XMLHandler.Keys.Queueselection].ToString()) ? xmldata[_xmlHandler.ConfigKeys[XMLHandler.Keys.Queueselection].ToString()] : string.Empty);
                        _dataContext.Subversion = (xmldata.ContainsKey(_xmlHandler.ConfigKeys[XMLHandler.Keys.Subversion].ToString()) ? xmldata[_xmlHandler.ConfigKeys[XMLHandler.Keys.Subversion].ToString()] : string.Empty);
                        _dataContext.KeepRecentPlace = xmldata.ContainsKey(_xmlHandler.ConfigKeys[XMLHandler.Keys.KeepPlace].ToString()) ? (string.IsNullOrEmpty(xmldata[_xmlHandler.ConfigKeys[XMLHandler.Keys.KeepPlace].ToString()]) ? false : Convert.ToBoolean(xmldata[_xmlHandler.ConfigKeys[XMLHandler.Keys.KeepPlace].ToString()].ToString().ToLower())) : false;
                        var recentChannels = xmldata.ContainsKey(_xmlHandler.ConfigKeys[XMLHandler.Keys.KeepChannels].ToString()) ? (string.IsNullOrEmpty(xmldata[_xmlHandler.ConfigKeys[XMLHandler.Keys.KeepChannels].ToString()]) ? string.Empty : xmldata[_xmlHandler.ConfigKeys[XMLHandler.Keys.KeepChannels].ToString()]) : string.Empty;
                        if (!string.IsNullOrEmpty(recentChannels))
                        {
                            if (recentChannels.Contains("Voice"))
                                _dataContext.IsVoiceChecked = true;
                            if (recentChannels.Contains("Email"))
                                _dataContext.IsEmailChecked = true;
                            if (recentChannels.Contains("Chat"))
                                _dataContext.IsChatChecked = true;
                            if (recentChannels.Contains("Outbound"))
                                _dataContext.IsOutboundChecked = true;
                        }
                        _dataContext.GadgetState = xmldata.ContainsKey(_xmlHandler.ConfigKeys[XMLHandler.Keys.GadgetState].ToString()) ? (string.IsNullOrEmpty(xmldata[_xmlHandler.ConfigKeys[XMLHandler.Keys.GadgetState].ToString()]) ? "None" : xmldata[_xmlHandler.ConfigKeys[XMLHandler.Keys.GadgetState].ToString()]) : "None";
                    }
                }

                #region Read xml by old method

                //var storage = new XMLStorage();
                //var output = storage.LoadParameters();
                //if (output != null)
                //{
                //    #region Assign Values

                //    //Assinging values to controls
                //    //if (string.IsNullOrEmpty(_dataContext.ApplicationName))
                //    //{
                //    //    _dataContext.ApplicationName = (output.ContainsKey("applicationName") ? output["applicationName"] : "");
                //    //}
                //    //if (string.IsNullOrEmpty(_dataContext.Place))
                //    //{
                //    //    _dataContext.Place = _place = (output.ContainsKey("placeName") ? output["placeName"] : "");
                //    //}
                //    //if (string.IsNullOrEmpty(_dataContext.Queue))
                //    //{
                //    //    _dataContext.Queue = (output.ContainsKey("queueselection") ? output["queueselection"] : "");
                //    //}
                //    //_dataContext.Subversion = output.ContainsKey("subVersion") ? output["subVersion"] : string.Empty;
                //    //_dataContext.KeepRecentPlace = Convert.ToBoolean((output.ContainsKey("KeepRecentPlace") ? string.IsNullOrEmpty(output["KeepRecentPlace"]) ? false : Convert.ToBoolean(output["KeepRecentPlace"]) : false));
                //    //var recentChannels = output.ContainsKey("RecentChannels") ? (string.IsNullOrEmpty(output["RecentChannels"]) ? string.Empty : output["RecentChannels"]) : string.Empty;
                //    //if (!string.IsNullOrEmpty(recentChannels))
                //    //{
                //    //    if (recentChannels.Contains("Voice"))
                //    //        _dataContext.IsVoiceChecked = true;
                //    //    if (recentChannels.Contains("Email"))
                //    //        _dataContext.IsEmailChecked = true;
                //    //    if (recentChannels.Contains("Chat"))
                //    //        _dataContext.IsChatChecked = true;
                //    //}
                //    //_dataContext.GadgetState = output.ContainsKey("GadgetState") ? string.IsNullOrEmpty(output["GadgetState"]) ? Pointel.Statistics.Core.Utility.StatisticsEnum.GadgetState.None : (Pointel.Statistics.Core.Utility.StatisticsEnum.GadgetState)Enum.Parse(typeof(Pointel.Statistics.Core.Utility.StatisticsEnum.GadgetState), output["GadgetState"], true) : Pointel.Statistics.Core.Utility.StatisticsEnum.GadgetState.None;
                //    //_dataContext.HostNameItemSource = _dataContext.HostNameItemSource.Where(s => !string.IsNullOrEmpty(s)).Distinct().ToList();
                //    //if (_dataContext.HostNameItemSource.Count == 0)
                //    //{
                //    //    if (output.Count > 0)
                //    //    {
                //    //        foreach (var compareValue in output.Select(t => (output.ContainsKey("configHost") ? output["configHost"] : "")).Where(compareValue => !_dataContext.HostNameItemSource.Contains(compareValue) && !string.IsNullOrEmpty(compareValue)))
                //    //        {
                //    //            _dataContext.HostNameItemSource.Add((output.ContainsKey("configHost") ? output["configHost"] : ""));
                //    //        }
                //    //    }
                //    //    if (!string.IsNullOrEmpty(_dataContext.HostNameText))
                //    //        _dataContext.HostNameItemSource.Add(_dataContext.HostNameText);
                //    //}
                //    //_dataContext.PortItemSource = _dataContext.PortItemSource.Where(s => !string.IsNullOrEmpty(s)).Distinct().ToList();
                //    //if (_dataContext.PortItemSource.Count == 0)
                //    //{
                //    //    if (output.Count > 0)
                //    //    {
                //    //        foreach (var compareValue in output.Select(t => (output.ContainsKey("configPort") ? output["configPort"] : "")).Where(compareValue => !_dataContext.PortItemSource.Contains(compareValue) && !string.IsNullOrEmpty(compareValue)))
                //    //        {
                //    //            _dataContext.PortItemSource.Add((output.ContainsKey("configPort") ? output["configPort"] : ""));
                //    //        }
                //    //    }
                //    //    if (!string.IsNullOrEmpty(_dataContext.PortText))
                //    //        _dataContext.PortItemSource.Add(_dataContext.PortText);
                //    //}
                //    //Assinging values to controls
                //    //if (string.IsNullOrEmpty(_dataContext.UserName))
                //    //{
                //    //    _dataContext.UserName = (output.ContainsKey("userName") ? output["userName"] : "");
                //    //}
                //    //if (string.IsNullOrEmpty(_dataContext.HostNameSelectedValue) && _dataContext.HostNameItemSource.Count != 0)
                //    //{
                //    //    _dataContext.HostNameSelectedValue = _dataContext.HostNameItemSource[0];
                //    //}
                //    //if (_dataContext.HostNameItemSource.Count > 0)
                //    //    _dataContext.HostNameSelectedValue = _dataContext.HostNameItemSource[0].ToString();
                //    //if (_dataContext.PortItemSource.Count > 0)
                //    //    _dataContext.PortSelectedValue = _dataContext.PortItemSource[0].ToString();
                //if (string.IsNullOrEmpty(_dataContext.PortSelectedValue) && string.IsNullOrEmpty(_dataContext.HostNameSelectedValue) && string.IsNullOrEmpty(_dataContext.UserName) && string.IsNullOrEmpty(_dataContext.ApplicationName))
                //{
                //    hcontCtrl.IsExpanded = true;
                //    _dataContext.ConfDetails_SP_Height = new GridLength(28);
                //    _dataContext.ConfDetails_SP_Height_keepplace = new GridLength(0);
                //    KeyboardNavigation.SetIsTabStop(txtApplication, true);
                //    KeyboardNavigation.SetTabNavigation(cmbHostname, KeyboardNavigationMode.Local);
                //    KeyboardNavigation.SetTabNavigation(cmbPort, KeyboardNavigationMode.Local);
                //    KeyboardNavigation.SetIsTabStop(cbxKeepPlace, true);
                //}
                //else
                //{
                //    _dataContext.ConfDetails_SP_Height = new GridLength(0);
                //    _dataContext.ConfDetails_SP_Height_keepplace = new GridLength(0);
                //    KeyboardNavigation.SetIsTabStop(txtApplication, false);
                //    KeyboardNavigation.SetTabNavigation(cmbHostname, KeyboardNavigationMode.None);
                //    KeyboardNavigation.SetTabNavigation(cmbPort, KeyboardNavigationMode.None);
                //    KeyboardNavigation.SetIsTabStop(cbxKeepPlace, false);
                //}

                //    #endregion Assign Values
                //}

                #endregion Read xml by old method

                if (string.IsNullOrEmpty(_dataContext.ApplicationName) ||
                    string.IsNullOrEmpty(_dataContext.HostNameSelectedValue) ||
                    string.IsNullOrEmpty(_dataContext.PortSelectedValue))
                {
                    hcontCtrl.IsExpanded = true;
                    _dataContext.ConfDetails_SP_Height = new GridLength(28);
                    _dataContext.ConfDetails_SP_Height_keepplace = new GridLength(0);
                    KeyboardNavigation.SetIsTabStop(txtApplication, true);
                    KeyboardNavigation.SetTabNavigation(cmbHostname, KeyboardNavigationMode.Local);
                    KeyboardNavigation.SetTabNavigation(cmbPort, KeyboardNavigationMode.Local);
                    KeyboardNavigation.SetIsTabStop(cbxKeepPlace, true);
                }
                else
                {
                    hcontCtrl.IsExpanded = false;
                    _dataContext.ConfDetails_SP_Height = new GridLength(0);
                    _dataContext.ConfDetails_SP_Height_keepplace = new GridLength(0);
                    KeyboardNavigation.SetIsTabStop(txtApplication, false);
                    KeyboardNavigation.SetTabNavigation(cmbHostname, KeyboardNavigationMode.None);
                    KeyboardNavigation.SetTabNavigation(cmbPort, KeyboardNavigationMode.None);
                    KeyboardNavigation.SetIsTabStop(cbxKeepPlace, false);
                }
                if (string.IsNullOrEmpty(_dataContext.UserName) || string.IsNullOrWhiteSpace(_dataContext.UserName))
                {
                    txtUserName.Focus();
                }
                else if (string.IsNullOrEmpty(txtPassword.Password) || string.IsNullOrWhiteSpace(txtPassword.Password))
                {
                    txtPassword.Focus();
                }
            }
            catch (Exception error)
            {
                _logger.Warn("Error occurred while reading and assigning values from xml file : " + error.Message.ToString());
            }
        }

        /// <summary>
        /// Installs the font.
        /// </summary>
        public void InstallFont()
        {
            _logger.Info("------------------------------------------");
            _logger.Info("Checking for Calibri font in local system");
            _logger.Info("------------------------------------------");
            var windowsPath = Environment.GetEnvironmentVariable("windir") + @"\fonts";
            var path = Environment.CurrentDirectory.ToString() + @"\Calibri.ttf";
            _logger.Info("Preparing...");
            _logger.Info("Target Font directory: " + windowsPath);
            _logger.Info("Source Font directory: " + path);
            var f = Microsoft.Win32.Registry.GetValue(@"HKEY_LOCAL_MACHINE\SOFTWARE\Microsoft\Windows NT\CurrentVersion\Fonts",
            "MyriadPro-Semibold(TrueType)", "Not installed");
            if (f.ToString().Contains("Not installed") && !f.ToString().Contains("Calibri.ttf"))
            {
                _logger.Info("Installing Calibri(True Type) font to local system");
                var ifc = new System.Drawing.Text.InstalledFontCollection();
                if (ifc.Families.Any(item => item.Name == "Calibri"))
                {
                    _logger.Info("Fontalready exist");
                }
                else
                {
                    try
                    {
                        System.IO.File.Copy(path, windowsPath + @"\Calibri.ttf", true); const int WM_FONTCHANGE = 0x001D;
                        const int hwndBroadcast = 0xffff;
                        var added = AddFontResource(path);
                        SendMessage(hwndBroadcast, WM_FONTCHANGE, 0, 0);
                        Microsoft.Win32.Registry.SetValue(@"HKEY_LOCAL_MACHINE\SOFTWARE\Microsoft\Windows NT\CurrentVersion\Fonts",
                        "MyriadPro-Semibold(TrueType)", windowsPath + @"\Calibri.ttf", Microsoft.Win32.RegistryValueKind.String);
                        _logger.Info("Installation completed: " + added.ToString());
                    }
                    catch (Exception ex)
                    {
                        _logger.Info("Installation Terminated : " + ex.Message.ToString());
                    }
                }
            }
            else
            {
                _logger.Info("Font already exist.");
                _logger.Info("------------------------------------------");
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

        /// <summary>
        /// Shows the error.
        /// </summary>
        /// <param name="errMsg">The error MSG.</param>
        public void ShowError(string errMsg)
        {
            if (string.IsNullOrEmpty(errMsg)) return;
            _dataContext.ErrorMessage = errMsg;
            _dataContext.ErrorRowHeight = GridLength.Auto;
            starttimerforerror();
            borderContent.IsEnabled = true;
        }

        public IntPtr WndProc(IntPtr hwnd, int msg, IntPtr wParam, IntPtr lParam, ref bool handled)
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

                case SC_Move:
                    ResizeMode = ResizeMode.NoResize;
                    UpdateLayout();
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

        [System.Runtime.InteropServices.DllImport("user32", SetLastError = true)]
        private static extern bool CloseClipboard();

        [DllImport("user32.dll")]
        private static extern bool DeleteMenu(IntPtr hMenu, int uPosition, int uFlags);

        [System.Runtime.InteropServices.DllImport("user32", SetLastError = true)]
        private static extern bool EmptyClipboard();

        [DllImport("user32.dll")]
        private static extern IntPtr GetSystemMenu(IntPtr hWnd, bool bRevert);

        [DllImport("user32.dll")]
        private static extern bool InsertMenu(IntPtr hMenu, Int32 wPosition, Int32 wFlags, Int32 wIDNewItem,
            string lpNewItem);

        [System.Runtime.InteropServices.DllImport("user32", SetLastError = true)]
        private static extern bool OpenClipboard(System.IntPtr winHandle);

        /// <summary>
        /// Handles the Click event of the btnCancel control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="RoutedEventArgs"/> instance containing the event data.</param>
        private void btnCancel_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                ClearClipboard();
                Environment.Exit(0);
            }
            catch (Exception ex)
            {
                _logger.Error("Error occurred as " + ex.Message);
            }
        }

        /// <summary>
        /// Handles the Click event of the btnLogin control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="RoutedEventArgs"/> instance containing the event data.</param>
        private void btnLogin_Click(object sender, RoutedEventArgs e)
        {
            if (txtUserName.Text == "" || string.IsNullOrEmpty(txtUserName.Text))
            {
                //_dataContext.ErrorMessage = "Please enter the User Name";
                //_dataContext.ErrorRowHeight = GridLength.Auto;
                ShowError("Please enter the User Name");
                btnLogin.IsEnabled = false;
                txtUserName.Focus();
            }
            else if (txtApplication.Text == "" || string.IsNullOrEmpty(txtApplication.Text))
            {
                //_dataContext.ErrorMessage = "Please enter the Application Name";
                //_dataContext.ErrorRowHeight = GridLength.Auto;
                ShowError("Please enter the Application Name");
                btnLogin.IsEnabled = false;
                txtApplication.Focus();
            }
            else if (cmbHostname.Text == "" || string.IsNullOrEmpty(cmbHostname.Text))
            {
                //_dataContext.ErrorMessage = "Please enter the Host Name";
                //_dataContext.ErrorRowHeight = GridLength.Auto;
                ShowError("Please enter the Host Name");
                btnLogin.IsEnabled = false;
                cmbHostname.Focus();
            }
            else if (cmbPort.Text == "" || string.IsNullOrEmpty(cmbPort.Text))
            {
                //_dataContext.ErrorMessage = "Please enter the Port";
                //_dataContext.ErrorRowHeight = GridLength.Auto;
                ShowError("Please enter the Port");
                btnLogin.IsEnabled = false;
                cmbPort.Focus();
            }
            else
            {
                Thread sdr = new Thread(delegate() { LoginWithAgentCredential(); });
                sdr.Start();
            }
        }

        /// <summary>
        /// Handles the SelectionChanged event of the cbLanguage control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="SelectionChangedEventArgs"/> instance containing the event data.</param>
        private void cbLanguage_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            try
            {
                if (_dataContext != null && _dataContext.SelectedLanguage != null)
                {
                    var currentResourceDictionary = (from d in _dataContext.ImportCatalog.ResourceDictionaryList
                                                     where d.Metadata.ContainsKey("Culture")
                                                     && d.Metadata["Culture"].ToString().Equals(_dataContext.SelectedLanguage.Code)
                                                     select d).FirstOrDefault();
                    if (currentResourceDictionary != null)
                    {
                        var previousResourceDictionary = (from d in _dataContext.ImportCatalog.ResourceDictionaryList
                                                          where d.Metadata.ContainsKey("Culture")
                                                          && d.Metadata["Culture"].ToString().Equals(_previouslang)
                                                          select d).FirstOrDefault();
                        if (previousResourceDictionary != null && previousResourceDictionary != currentResourceDictionary)
                        {
                            System.Windows.Application.Current.Resources.MergedDictionaries.Remove(previousResourceDictionary.Value);
                            System.Windows.Application.Current.Resources.MergedDictionaries.Add(currentResourceDictionary.Value);
                            var cultureInfo = new CultureInfo(_dataContext.SelectedLanguage.Code);
                            Thread.CurrentThread.CurrentCulture = cultureInfo;
                            Thread.CurrentThread.CurrentUICulture = cultureInfo;
                            System.Windows.Application.Current.MainWindow.Language = XmlLanguage.GetLanguage(CultureInfo.CurrentCulture.IetfLanguageTag);
                        }
                    }
                    _previouslang = _dataContext.SelectedLanguage.Code;
                    if (!string.IsNullOrEmpty(_place))
                        cbxKeepPlace.Content = "_" + (string)FindResource("KeyKeepPlace") + " " + _place;
                    if (hcontCtrl.IsExpanded)
                        hcontCtrl.Header = FindResource("KeyLess");
                    if (!hcontCtrl.IsExpanded)
                        hcontCtrl.Header = FindResource("KeyMore");

                    btnLogin.Content = "_" + (string)FindResource("KeyLogIn");
                    btnCancel.Content = "_" + (string)FindResource("KeyCancel");
                }
            }
            catch (Exception ex)
            {
                _logger.Error("Error occurred as " + ex.Message);
            }
        }

        /// <summary>
        /// Checks the plugins.
        /// </summary>
        private void CheckPlugins()
        {
            try
            {
                var file = Path.Combine(Path.Combine(System.Windows.Forms.Application.StartupPath, "Plugins"), "StatTickerFive.dll");
                if (File.Exists(file))
                    _dataContext.IsStatPluginAdded = true;
                else
                    _logger.Warn("CheckPlugins : Stat Plug-in dll does not exist");

                file = Path.Combine(Path.Combine(System.Windows.Forms.Application.StartupPath, "Plugins"), "Pointel.Integration.Core.dll");
                if (File.Exists(file))
                    _dataContext.IsThirdPartyPluginAdded = true;
                else
                    _logger.Warn("CheckPlugins : Third Party Plugin dll does not exist");

                file = Path.Combine(Path.Combine(System.Windows.Forms.Application.StartupPath, "Plugins"), "Pointel.Interactions.Email.dll");
                if (File.Exists(file))
                    _dataContext.IsEmailPluginAdded = true;
                else
                    _logger.Warn("CheckPlugins : Email Plugin dll does not exist");

                file = Path.Combine(Path.Combine(System.Windows.Forms.Application.StartupPath, "Plugins"), "Pointel.Interactions.Chat.dll");
                if (File.Exists(file))
                    _dataContext.IsChatPluginAdded = true;
                else
                    _logger.Warn("CheckPlugins : Chat Plugin dll does not exist");

                file = Path.Combine(Path.Combine(System.Windows.Forms.Application.StartupPath, "Plugins"), "Pointel.Interactions.TeamCommunicator.dll");
                if (File.Exists(file))
                    _dataContext.IsTeamCommunicatorPluginAdded = true;
                else
                    _logger.Warn("CheckPlugins : TeamCommunicator Plugin dll does not exist");

                file = Path.Combine(Path.Combine(System.Windows.Forms.Application.StartupPath, "Plugins"), "Pointel.Interaction.Workbin.dll");
                if (File.Exists(file))
                    _dataContext.IsWorkbinPluginAdded = true;
                else
                    _logger.Warn("CheckPlugins : Workbin Plugin dll does not exist");

                //file = Path.Combine(Path.Combine(System.Windows.Forms.Application.StartupPath, "Plugins"), "Pointel.Interactions.Contact.dll");
                //if (File.Exists(file))
                //    _dataContext.IsContactsPluginAdded = true;
                //else
                //    _logger.Warn("CheckPlugins : Contact Plugin dll does not exist");

                file = Path.Combine(Path.Combine(System.Windows.Forms.Application.StartupPath, "Plugins"), "Pointel.Interactions.Outbound.dll");
                if (File.Exists(file))
                    _dataContext.IsOutboundPluginAdded = true;
                else
                    _logger.Warn("CheckPlugins : Outbound Plugin dll does not exist");

                file = Path.Combine(Path.Combine(System.Windows.Forms.Application.StartupPath, "Plugins"), "Pointel.Integration.Salesforce.dll");
                if (File.Exists(file))
                    Datacontext.GetInstance().IsSalesforcePluginAdded = true;
                else
                    _logger.Warn("CheckPlugins : Salesforce Plugin dll does not exist");
            }
            catch (Exception ex)
            {
                _logger.Error("CheckPlugins : Error occurred while checking the plugins " + ex.InnerException == null ? ex.Message : ex.InnerException.Message);
            }
        }

        /// <summary>
        /// Handles the KeyDown event of the cmbHostname control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="System.Windows.Input.KeyEventArgs"/> instance containing the event data.</param>
        private void cmbHostname_KeyDown(object sender, System.Windows.Input.KeyEventArgs e)
        {
            //_dataContext.ErrorRowHeight = new GridLength(0);
            CloseError(null, null);
            btnLogin.IsEnabled = true;
        }

        /// <summary>
        /// Handles the SelectionChanged event of the cmbHostname control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="SelectionChangedEventArgs"/> instance containing the event data.</param>
        private void cmbHostname_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (_matchingipport.Count > 0 && e.AddedItems.Count > 0)
            {
                if (!_matchingipport.ContainsKey(e.AddedItems[0].ToString())) return;
                if (!_matchingipport[e.AddedItems[0].ToString()].ToString().Contains(","))
                    cmbPort.Text = _matchingipport[e.AddedItems[0].ToString()].ToString();
            }
        }

        /// <summary>
        /// Handles the KeyDown event of the cmbPort control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="System.Windows.Input.KeyEventArgs"/> instance containing the event data.</param>
        private void cmbPort_KeyDown(object sender, System.Windows.Input.KeyEventArgs e)
        {
            // _dataContext.ErrorRowHeight = new GridLength(0);
            CloseError(null, null);
            btnLogin.IsEnabled = true;
            var portText = cmbPort.Text;

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
                while (portText.Length > 3 && e.Key != Key.Back && e.Key != Key.Delete
                    && e.Key != Key.Left && e.Key != Key.Right
                    && e.Key != Key.End && e.Key != Key.Home
                    && e.Key != Key.Prior && e.Key != Key.Next && e.Key != Key.Tab && e.Key != Key.LeftAlt && e.Key != Key.RightAlt && e.Key != Key.System)
                {
                    var editableTextBox = cmbPort.Template.FindName("PART_EditableTextBox", cmbPort) as System.Windows.Controls.TextBox;
                    if (editableTextBox != null)
                        if (!string.IsNullOrEmpty(editableTextBox.SelectedText))
                        {
                            e.Handled = false;
                            checkInput();
                            break;
                        }
                        else
                        {
                            var num = NumericKeyboardvalue(e.Key);
                            if (num != null)
                            {
                                portText += num;
                                if (Convert.ToInt32(portText) > 65535 ||
                                    Convert.ToInt32(portText) == 0 ||
                                    portText.Length > 5)
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
        /// Gets the application servers.
        /// </summary>
        /// <param name="appName">Name of the application.</param>
        private void getAppServers(string appName)
        {
            string primaryServerName = string.Empty;
            string secondaryServerName = string.Empty;
            try
            {
                Datacontext.TServersSwitchDic.Clear();
                Datacontext.AvailableServerDic.Clear();
                CfgApplicationQuery applicationQuery = new CfgApplicationQuery();
                applicationQuery.Name = appName;
                //applicationQuery.TenantDbid = _configContainer.TenantDbId;
                _logger.Debug("Reading application values from : " + appName);
                CfgApplication applicationObject = _configContainer.ConfServiceObject.RetrieveObject<CfgApplication>(applicationQuery);
                if (applicationObject != null && applicationObject.AppServers != null && applicationObject.AppServers.Count > 0)
                {
                    foreach (CfgConnInfo appServer in applicationObject.AppServers)
                    {
                        if (appServer.AppServer != null && appServer.AppServer.Type == CfgAppType.CFGTServer)
                        {
                            if (!Datacontext.AvailableServerDic.ContainsKey(appServer.AppServer.Name))
                                Datacontext.AvailableServerDic.Add(appServer.AppServer.Name, appServer.AppServer.Type.ToString());

                            CfgApplicationQuery tappQuery = new CfgApplicationQuery();
                            tappQuery.Name = appServer.AppServer.Name;
                            tappQuery.Dbid = appServer.AppServer.DBID;
                            CfgApplication tappObject = _configContainer.ConfServiceObject.RetrieveObject<CfgApplication>(tappQuery);
                            if (tappObject != null)
                            {
                                var _flexibleProperties = tappObject.FlexibleProperties;
                                if (_flexibleProperties != null && _flexibleProperties.Count > 0)
                                {
                                    if (_flexibleProperties.ContainsKey("CONN_INFO"))
                                    {
                                        var _connInfoCollection = _flexibleProperties["CONN_INFO"];
                                        if (_connInfoCollection != null)
                                        {
                                            var _switchCollection = (KeyValueCollection)_connInfoCollection;
                                            if (_switchCollection != null && _switchCollection.ContainsKey("CFGSwitch"))
                                            {
                                                var _switchCollection1 = _switchCollection["CFGSwitch"];
                                                if (_switchCollection1 != null)
                                                {
                                                    KeyValueCollection _switchDBIds = (KeyValueCollection)_switchCollection1;
                                                    if (_switchDBIds != null && _switchDBIds.Count > 0)
                                                        foreach (string id in _switchDBIds.AllKeys)
                                                            if (!Datacontext.TServersSwitchDic.ContainsKey(Convert.ToInt32(id)))
                                                                Datacontext.TServersSwitchDic.Add(Convert.ToInt32(id), appServer.AppServer.Name);
                                                }
                                            }
                                        }
                                    }
                                }
                            }
                        }
                        else if (appServer.AppServer != null && appServer.AppServer.Type == CfgAppType.CFGInteractionServer)
                        {
                            if (!Datacontext.AvailableServerDic.ContainsKey(appServer.AppServer.Name))
                                Datacontext.AvailableServerDic.Add(appServer.AppServer.Name, appServer.AppServer.Type.ToString());
                        }
                        else if (appServer.AppServer != null && appServer.AppServer.Type == CfgAppType.CFGContactServer)
                        {
                            if (!Datacontext.AvailableServerDic.ContainsKey(appServer.AppServer.Name))
                                Datacontext.AvailableServerDic.Add(appServer.AppServer.Name, appServer.AppServer.Type.ToString());
                        }
                    }
                }

                if (_configContainer.AllKeys.Contains("voice.primary-server-name") &&
                         ((string)_configContainer.GetValue("voice.primary-server-name")) != string.Empty)
                    primaryServerName = ((string)_configContainer.GetValue("voice.primary-server-name")).Trim();
                if (_configContainer.AllKeys.Contains("voice.secondary-server-name") &&
                        ((string)_configContainer.GetValue("voice.secondary-server-name")) != string.Empty)
                    secondaryServerName = ((string)_configContainer.GetValue("voice.secondary-server-name")).Trim();

                if (!string.IsNullOrEmpty(primaryServerName) && !string.IsNullOrEmpty(secondaryServerName))
                {
                    foreach (var item in Datacontext.AvailableServerDic.Where(kvp => kvp.Value == CfgAppType.CFGTServer.ToString()).ToList())
                    {
                        Datacontext.AvailableServerDic.Remove(item.Key);
                    }
                    if (!Datacontext.AvailableServerDic.ContainsKey(primaryServerName) && !Datacontext.AvailableServerDic.ContainsKey(secondaryServerName))
                        Datacontext.AvailableServerDic.Add(primaryServerName + "," + secondaryServerName, CfgAppType.CFGTServer.ToString());
                    else
                    {
                        if (Datacontext.AvailableServerDic.ContainsKey(primaryServerName))
                        {
                            Datacontext.AvailableServerDic.Remove(primaryServerName);
                            Datacontext.AvailableServerDic.Add(primaryServerName + "," + secondaryServerName, CfgAppType.CFGTServer.ToString());
                        }
                        if (Datacontext.AvailableServerDic.ContainsKey(secondaryServerName))
                        {
                            Datacontext.AvailableServerDic.Remove(secondaryServerName);
                            Datacontext.AvailableServerDic.Add(primaryServerName + "," + secondaryServerName, CfgAppType.CFGTServer.ToString());
                        }
                    }
                }
                else if (!string.IsNullOrEmpty(primaryServerName))
                {
                    foreach (var item in Datacontext.AvailableServerDic.Where(kvp => kvp.Value == CfgAppType.CFGTServer.ToString()).ToList())
                    {
                        Datacontext.AvailableServerDic.Remove(item.Key);
                    }
                    if (!Datacontext.AvailableServerDic.ContainsKey(primaryServerName))
                        Datacontext.AvailableServerDic.Add(primaryServerName + "," + secondaryServerName, CfgAppType.CFGTServer.ToString());
                    else
                    {
                        Datacontext.AvailableServerDic.Remove(primaryServerName);
                        Datacontext.AvailableServerDic.Add(primaryServerName + "," + secondaryServerName, CfgAppType.CFGTServer.ToString());
                    }
                }
                else if (!string.IsNullOrEmpty(secondaryServerName))
                {
                    foreach (var item in Datacontext.AvailableServerDic.Where(kvp => kvp.Value == CfgAppType.CFGTServer.ToString()).ToList())
                    {
                        Datacontext.AvailableServerDic.Remove(item.Key);
                    }
                    if (!Datacontext.AvailableServerDic.ContainsKey(secondaryServerName))
                        Datacontext.AvailableServerDic.Add(primaryServerName + "," + secondaryServerName, CfgAppType.CFGTServer.ToString());
                    else
                    {
                        Datacontext.AvailableServerDic.Remove(secondaryServerName);
                        Datacontext.AvailableServerDic.Add(primaryServerName + "," + secondaryServerName, CfgAppType.CFGTServer.ToString());
                    }
                }

                if (!string.IsNullOrEmpty(primaryServerName))
                {
                    CfgApplicationQuery tappQuery = new CfgApplicationQuery();
                    tappQuery.Name = primaryServerName;
                    CfgApplication tappObject = _configContainer.ConfServiceObject.RetrieveObject<CfgApplication>(tappQuery);
                    if (tappObject != null)
                    {
                        var _flexibleProperties = tappObject.FlexibleProperties;
                        if (_flexibleProperties != null && _flexibleProperties.Count > 0)
                        {
                            if (_flexibleProperties.ContainsKey("CONN_INFO"))
                            {
                                var _connInfoCollection = _flexibleProperties["CONN_INFO"];
                                if (_connInfoCollection != null)
                                {
                                    var _switchCollection = (KeyValueCollection)_connInfoCollection;
                                    if (_switchCollection != null && _switchCollection.ContainsKey("CFGSwitch"))
                                    {
                                        var _switchCollection1 = _switchCollection["CFGSwitch"];
                                        if (_switchCollection1 != null)
                                        {
                                            KeyValueCollection _switchDBIds = (KeyValueCollection)_switchCollection1;
                                            if (_switchDBIds != null && _switchDBIds.Count > 0)
                                                foreach (string id in _switchDBIds.AllKeys)
                                                    if (!Datacontext.TServersSwitchDic.ContainsKey(Convert.ToInt32(id)))
                                                        Datacontext.TServersSwitchDic.Add(Convert.ToInt32(id), primaryServerName);
                                        }
                                    }
                                }
                            }
                        }
                    }
                }
                primaryServerName = string.Empty;
                secondaryServerName = string.Empty;

                if (_configContainer.AllKeys.Contains("interaction.primary-server-name") &&
                        ((string)_configContainer.GetValue("interaction.primary-server-name")) != string.Empty)
                    primaryServerName = ((string)_configContainer.GetValue("interaction.primary-server-name")).Trim();
                if (_configContainer.AllKeys.Contains("interaction.secondary-server-name") &&
                        ((string)_configContainer.GetValue("interaction.secondary-server-name")) != string.Empty)
                    secondaryServerName = ((string)_configContainer.GetValue("interaction.secondary-server-name")).Trim();

                if (!string.IsNullOrEmpty(primaryServerName) && !string.IsNullOrEmpty(secondaryServerName))
                {
                    foreach (var item in Datacontext.AvailableServerDic.Where(kvp => kvp.Value == CfgAppType.CFGInteractionServer.ToString()).ToList())
                    {
                        Datacontext.AvailableServerDic.Remove(item.Key);
                    }
                    if (!Datacontext.AvailableServerDic.ContainsKey(primaryServerName) && !Datacontext.AvailableServerDic.ContainsKey(secondaryServerName))
                        Datacontext.AvailableServerDic.Add(primaryServerName + "," + secondaryServerName, CfgAppType.CFGInteractionServer.ToString());
                    else
                    {
                        if (Datacontext.AvailableServerDic.ContainsKey(primaryServerName))
                        {
                            Datacontext.AvailableServerDic.Remove(primaryServerName);
                            Datacontext.AvailableServerDic.Add(primaryServerName + "," + secondaryServerName, CfgAppType.CFGInteractionServer.ToString());
                        }
                        if (Datacontext.AvailableServerDic.ContainsKey(secondaryServerName))
                        {
                            Datacontext.AvailableServerDic.Remove(secondaryServerName);
                            Datacontext.AvailableServerDic.Add(primaryServerName + "," + secondaryServerName, CfgAppType.CFGInteractionServer.ToString());
                        }
                    }
                }
                else if (!string.IsNullOrEmpty(primaryServerName))
                {
                    foreach (var item in Datacontext.AvailableServerDic.Where(kvp => kvp.Value == CfgAppType.CFGInteractionServer.ToString()).ToList())
                    {
                        Datacontext.AvailableServerDic.Remove(item.Key);
                    }
                    if (!Datacontext.AvailableServerDic.ContainsKey(primaryServerName))
                        Datacontext.AvailableServerDic.Add(primaryServerName + "," + secondaryServerName, CfgAppType.CFGInteractionServer.ToString());
                    else
                    {
                        Datacontext.AvailableServerDic.Remove(primaryServerName);
                        Datacontext.AvailableServerDic.Add(primaryServerName + "," + secondaryServerName, CfgAppType.CFGInteractionServer.ToString());
                    }
                }
                else if (!string.IsNullOrEmpty(secondaryServerName))
                {
                    foreach (var item in Datacontext.AvailableServerDic.Where(kvp => kvp.Value == CfgAppType.CFGInteractionServer.ToString()).ToList())
                    {
                        Datacontext.AvailableServerDic.Remove(item.Key);
                    }
                    if (!Datacontext.AvailableServerDic.ContainsKey(secondaryServerName))
                        Datacontext.AvailableServerDic.Add(primaryServerName + "," + secondaryServerName, CfgAppType.CFGInteractionServer.ToString());
                    else
                    {
                        Datacontext.AvailableServerDic.Remove(secondaryServerName);
                        Datacontext.AvailableServerDic.Add(primaryServerName + "," + secondaryServerName, CfgAppType.CFGInteractionServer.ToString());
                    }
                }
                primaryServerName = string.Empty;
                secondaryServerName = string.Empty;

                if (_configContainer.AllKeys.Contains("contact.primary-server-name") &&
                       ((string)_configContainer.GetValue("contact.primary-server-name")) != string.Empty)
                    primaryServerName = ((string)_configContainer.GetValue("contact.primary-server-name")).Trim();
                if (_configContainer.AllKeys.Contains("contact.secondary-server-name") &&
                        ((string)_configContainer.GetValue("contact.secondary-server-name")) != string.Empty)
                    secondaryServerName = ((string)_configContainer.GetValue("contact.secondary-server-name")).Trim();

                if (!string.IsNullOrEmpty(primaryServerName) && !string.IsNullOrEmpty(secondaryServerName))
                {
                    foreach (var item in Datacontext.AvailableServerDic.Where(kvp => kvp.Value == CfgAppType.CFGContactServer.ToString()).ToList())
                    {
                        Datacontext.AvailableServerDic.Remove(item.Key);
                    }
                    if (!Datacontext.AvailableServerDic.ContainsKey(primaryServerName) && !Datacontext.AvailableServerDic.ContainsKey(secondaryServerName))
                        Datacontext.AvailableServerDic.Add(primaryServerName + "," + secondaryServerName, CfgAppType.CFGContactServer.ToString());
                    else
                    {
                        if (Datacontext.AvailableServerDic.ContainsKey(primaryServerName))
                        {
                            Datacontext.AvailableServerDic.Remove(primaryServerName);
                            Datacontext.AvailableServerDic.Add(primaryServerName + "," + secondaryServerName, CfgAppType.CFGContactServer.ToString());
                        }
                        if (Datacontext.AvailableServerDic.ContainsKey(secondaryServerName))
                        {
                            Datacontext.AvailableServerDic.Remove(secondaryServerName);
                            Datacontext.AvailableServerDic.Add(primaryServerName + "," + secondaryServerName, CfgAppType.CFGContactServer.ToString());
                        }
                    }
                }
                else if (!string.IsNullOrEmpty(primaryServerName))
                {
                    foreach (var item in Datacontext.AvailableServerDic.Where(kvp => kvp.Value == CfgAppType.CFGContactServer.ToString()).ToList())
                    {
                        Datacontext.AvailableServerDic.Remove(item.Key);
                    }
                    if (!Datacontext.AvailableServerDic.ContainsKey(primaryServerName))
                        Datacontext.AvailableServerDic.Add(primaryServerName + "," + secondaryServerName, CfgAppType.CFGContactServer.ToString());
                    else
                    {
                        Datacontext.AvailableServerDic.Remove(primaryServerName);
                        Datacontext.AvailableServerDic.Add(primaryServerName + "," + secondaryServerName, CfgAppType.CFGContactServer.ToString());
                    }
                }
                else if (!string.IsNullOrEmpty(secondaryServerName))
                {
                    foreach (var item in Datacontext.AvailableServerDic.Where(kvp => kvp.Value == CfgAppType.CFGContactServer.ToString()).ToList())
                    {
                        Datacontext.AvailableServerDic.Remove(item.Key);
                    }
                    if (!Datacontext.AvailableServerDic.ContainsKey(secondaryServerName))
                        Datacontext.AvailableServerDic.Add(primaryServerName + "," + secondaryServerName, CfgAppType.CFGContactServer.ToString());
                    else
                    {
                        Datacontext.AvailableServerDic.Remove(secondaryServerName);
                        Datacontext.AvailableServerDic.Add(primaryServerName + "," + secondaryServerName, CfgAppType.CFGContactServer.ToString());
                    }
                }
                primaryServerName = string.Empty;
                secondaryServerName = string.Empty;

                if (Datacontext.AvailableServerDic.ContainsValue(CfgAppType.CFGInteractionServer.ToString()))
                {
                    var ixnAppName = Datacontext.AvailableServerDic.FirstOrDefault(x => x.Value == CfgAppType.CFGInteractionServer.ToString()).Key;
                    if (ixnAppName != null)
                    {
                        string[] ixn = ixnAppName.Split(',');
                        if (ixn != null && ixn.Length > 0 && !string.IsNullOrEmpty(ixn[0]))
                        {
                            CfgApplicationQuery appQuery = new CfgApplicationQuery();
                            appQuery.Name = ixn[0].Trim();
                            appQuery.TenantDbid = _configContainer.TenantDbId;
                            _logger.Debug("Reading application values from : " + ixnAppName);
                            CfgApplication ixnappObject = _configContainer.ConfServiceObject.RetrieveObject<CfgApplication>(appQuery);
                            if (ixnappObject != null && ixnappObject.AppServers != null && ixnappObject.AppServers.Count > 0)
                            {
                                foreach (CfgConnInfo appServer in ixnappObject.AppServers)
                                {
                                    if (appServer.AppServer != null && appServer.AppServer.Type == CfgAppType.CFGChatServer)
                                    {
                                        if (!Datacontext.AvailableServerDic.ContainsKey(appServer.AppServer.Name))
                                            Datacontext.AvailableServerDic.Add(appServer.AppServer.Name, appServer.AppServer.Type.ToString());
                                    }
                                }
                            }
                            else
                            {
                                Datacontext.AvailableServerDic.Remove(ixnAppName);
                            }
                        }
                    }
                }
                else
                {
                }
            }
            catch (Exception generalException)
            {
                _logger.Error("Error occurred in getAppServers() : " + generalException.ToString());
            }
            finally
            {
                primaryServerName = null;
                secondaryServerName = null;
            }
        }

        private void GetCMEValues()
        {
            try
            {
                //Load all the skills in the List
                if (_configContainer.AllKeys.Contains("AllSkills"))
                {
                    var skills = (List<string>)_configContainer.GetValue("AllSkills");
                    if (skills != null && skills.Count > 0)
                    {
                        _dataContext.LoadAllSkills.Clear();
                        // foreach (var skillValue in skills)
                        for (int index = 0; index < skills.Count; index++)
                            if (!_dataContext.LoadAllSkills.Contains(skills[index]))
                                _dataContext.LoadAllSkills.Add(skills[index]);
                        _dataContext.LoadAllSkills.Sort();
                    }
                }
                if (_configContainer.AllKeys.Contains("voice.enable.add-case-data") &&
                        ((string)_configContainer.GetValue("voice.enable.add-case-data")).ToLower().Equals("true"))
                    _dataContext.IsVoiceEnabledAddCallData = Visibility.Visible;
                else
                    _dataContext.IsVoiceEnabledAddCallData = Visibility.Collapsed;

                _dataContext.IsAttachDataEnabled = _configContainer.GetAsBoolean("voice.enable.case-data", true);

                if (_configContainer.AllKeys.Contains("voice.dial-pad.number-digit"))
                    _dataContext.DialpadDigits = Convert.ToInt32((string)_configContainer.GetValue("voice.dial-pad.number-digit"));

                if (_configContainer.AllKeys.Contains("voice.consult-call.dial-digit"))
                    _dataContext.ConsultDialDigits = Convert.ToInt32((string)_configContainer.GetValue("voice.consult-call.dial-digit"));

                //if (_configContainer.AllKeys.Contains("VoiceNotReadyReasonCodes"))
                //{
                //    Datacontext.loadNotReadyReasonCodes = (Dictionary<string, string>)_configContainer.GetValue("VoiceNotReadyReasonCodes");
                //}
                LoadDialPadContacts();
                LoadFonts();
                LoadThresholdValues();
            }
            catch (Exception generalException)
            {
                _logger.Error((generalException.InnerException == null) ? generalException.Message : generalException.InnerException.ToString());
            }
        }

        private void GetTrialXML()
        {
            try
            {
                if (File.Exists(_dataContext.TrialXMLFile))
                {
                    var xmldata = _xmlHandler.ReadAllXmlData(_dataContext.TrialXMLFile);
                    if (xmldata != null)
                    {
                        string trailMessage = (xmldata.ContainsKey(_xmlHandler.ConfigKeys[XMLHandler.Keys.TrialMessage].ToString()) ? xmldata[_xmlHandler.ConfigKeys[XMLHandler.Keys.TrialMessage].ToString()] : string.Empty);
                        string trailNotifyMessage = (xmldata.ContainsKey(_xmlHandler.ConfigKeys[XMLHandler.Keys.TrialNotificationMessage].ToString()) ? xmldata[_xmlHandler.ConfigKeys[XMLHandler.Keys.TrialNotificationMessage].ToString()] : string.Empty);
                        string strailNotifyStartDate = (xmldata.ContainsKey(_xmlHandler.ConfigKeys[XMLHandler.Keys.TrialNotificationStartDate].ToString()) ? BasicEncryptionDecryption.Decrypt(xmldata[_xmlHandler.ConfigKeys[XMLHandler.Keys.TrialNotificationStartDate].ToString()]) : string.Empty);
                        string strialEndDate = (xmldata.ContainsKey(_xmlHandler.ConfigKeys[XMLHandler.Keys.TrialNotificationEndDate].ToString()) ? BasicEncryptionDecryption.Decrypt(xmldata[_xmlHandler.ConfigKeys[XMLHandler.Keys.TrialNotificationEndDate].ToString()]) : string.Empty);
                        if (!string.IsNullOrEmpty(strailNotifyStartDate))
                        {
                            DateTime trailNotifyStartDate = Convert.ToDateTime(strailNotifyStartDate);
                            if (!string.IsNullOrEmpty(strialEndDate))
                            {
                                DateTime trailtrialEndDate = Convert.ToDateTime(strialEndDate);
                                int days = Convert.ToInt32((trailtrialEndDate - DateTime.Now).TotalDays);
                                if (days <= 0)
                                {
                                    _dataContext.TrailMessage = trailMessage;
                                }
                                else
                                {
                                    if (trailNotifyStartDate <= DateTime.Now)
                                    {
                                        if (days == 1)
                                            _dataContext.TrailMessage = trailNotifyMessage + " " + days + " day";
                                        else
                                            _dataContext.TrailMessage = trailNotifyMessage + " " + days + " days";
                                    }
                                }
                            }

                            int days1 = Convert.ToInt32((trailNotifyStartDate - DateTime.Now).TotalDays);
                            if (days1 > 0)
                            {
                                _dataContext.TrialVisibility = System.Windows.Visibility.Collapsed;
                            }
                            else
                            {
                                _dataContext.TrialVisibility = System.Windows.Visibility.Visible;
                            }
                        }
                    }
                }
            }
            catch (Exception error)
            {
                _logger.Error("Error occurred while reading and assigning values from trial xml file : " + error.Message.ToString());
            }
        }

        /// <summary>
        /// Handles the Collapsed event of the hcontCtrl control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="RoutedEventArgs"/> instance containing the event data.</param>
        private void hcontCtrl_Collapsed(object sender, RoutedEventArgs e)
        {
            _dataContext.ConfDetails_SP_Height = new GridLength(0);
            hcontCtrl.Header = FindResource("KeyMore");
            KeyboardNavigation.SetIsTabStop(txtApplication, false);
            KeyboardNavigation.SetTabNavigation(cmbHostname, KeyboardNavigationMode.None);
            KeyboardNavigation.SetTabNavigation(cmbPort, KeyboardNavigationMode.None);
        }

        /// <summary>
        /// Handles the Expanded event of the hcontCtrl control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="RoutedEventArgs"/> instance containing the event data.</param>
        private void hcontCtrl_Expanded(object sender, RoutedEventArgs e)
        {
            _dataContext.ConfDetails_SP_Height = new GridLength(28);
            hcontCtrl.Header = FindResource("KeyLess");
            KeyboardNavigation.SetIsTabStop(txtApplication, true);
            KeyboardNavigation.SetTabNavigation(cmbHostname, KeyboardNavigationMode.Local);
            KeyboardNavigation.SetTabNavigation(cmbPort, KeyboardNavigationMode.Local);
        }

        private void LoadDialPadContacts()
        {
            try
            {
                //Load Global contacts
                if (_configContainer.AllKeys.Contains("GlobalContacts") && _configContainer.GetValue("GlobalContacts") != null)
                {
                    try
                    {
                        _dataContext.HshApplicationLevel.Clear();
                        foreach (var name in ((KeyValueCollection)_configContainer.GetValue("GlobalContacts")).AllKeys.Where(name => !_dataContext.HshApplicationLevel.ContainsKey(name)))
                        {
                            if (!_dataContext.HshApplicationLevel.ContainsKey(name))
                                _dataContext.HshApplicationLevel.Add(name, ((KeyValueCollection)_configContainer.GetValue("GlobalContacts"))[name].ToString());
                        }
                    }
                    catch (Exception ex)
                    {
                        _logger.Error("Error loading when Global contacts : " + ((ex.InnerException == null) ? ex.Message : ex.InnerException.ToString()));
                    }
                }

                //Load Group contacts
                //tempContacts.Clear();
                if (_configContainer.AllKeys.Contains("GroupContacts") && _configContainer.GetValue("GroupContacts") != null)
                {
                    Datacontext.hshLoadGroupContact.Clear();
                    try
                    {
                        foreach (string name in ((KeyValueCollection)_configContainer.GetValue("GroupContacts")).Keys)
                        {
                            if (!Datacontext.hshLoadGroupContact.ContainsKey(name))
                                Datacontext.hshLoadGroupContact.Add(name, ((KeyValueCollection)_configContainer.GetValue("GroupContacts"))[name].ToString());
                        }
                    }
                    catch (Exception ex)
                    {
                        _logger.Error("Error loading when Group contacts : " + ((ex.InnerException == null) ? ex.Message : ex.InnerException.ToString()));
                    }
                }

                //Load Group contacts
                //tempContacts.Clear();

                if (_configContainer.AllKeys.Contains("AgentContacts") && _configContainer.GetValue("AgentContacts") != null)
                {
                    try
                    {
                        _dataContext.AnnexContacts.Clear();
                        foreach (string name in ((Dictionary<string, string>)_configContainer.GetValue("AgentContacts")).Keys)
                        {
                            if (!_dataContext.AnnexContacts.ContainsKey(name))
                                _dataContext.AnnexContacts.Add(name, ((Dictionary<string, string>)_configContainer.GetValue("AgentContacts"))[name].ToString());
                        }
                    }
                    catch (Exception ex)
                    {
                        _logger.Error("Error loading when Group contacts : " + ((ex.InnerException == null) ? ex.Message : ex.InnerException.ToString()));
                    }
                }
            }
            catch (Exception generalException)
            {
                _logger.Error((generalException.InnerException == null) ? generalException.Message : generalException.InnerException.ToString());
            }
        }

        private void LoadFonts()
        {
            try
            {
                if (_configContainer.AllKeys.Contains("voice.call-data.font-family") &&
                    !string.IsNullOrEmpty((string)_configContainer.GetValue("voice.call-data.font-family")))
                {
                    if (DoesFontExist((string)_configContainer.GetValue("voice.call-data.font-family")))
                    {
                        object convertFromString = FontWeights.Normal;
                        if (_configContainer.AllKeys.Contains("voice.call-data.font-weight") &&
                                !string.IsNullOrEmpty((string)_configContainer.GetValue("voice.call-data.font-weight")))
                            convertFromString = new FontWeightConverter().ConvertFromString((string)_configContainer.GetValue("voice.call-data.font-weight"));
                        else
                            convertFromString = FontWeights.Normal;
                        _dataContext.KeyFontWeight = (FontWeight)convertFromString;
                        _dataContext.KeyFontFamily = new FontFamily((string)_configContainer.GetValue("voice.call-data.font-family"));
                    }
                    else
                    {
                        _dataContext.KeyFontWeight = FontWeights.Bold;
                        _dataContext.KeyFontFamily = new FontFamily("Calibri");
                    }
                }
                else
                {
                    _dataContext.KeyFontWeight = FontWeights.Bold;
                    _dataContext.KeyFontFamily = new FontFamily("Calibri");
                }
            }
            catch (Exception ex)
            {
                _logger.Error((ex.InnerException == null) ? ex.Message : ex.InnerException.ToString());
            }
        }

        private void LoadThresholdValues()
        {
            try
            {
                Dictionary<string, string> dicThreshold = new Dictionary<string, string>();
                //Loading Answer Threshold color
                if (_configContainer.AllKeys.Contains("answer.threshold1.color") &&
                            _configContainer.GetValue("answer.threshold1.color") != string.Empty)
                    dicThreshold.Add("answer.threshold1.color", (string)_configContainer.GetValue("answer.threshold1.color"));

                if (_configContainer.AllKeys.Contains("answer.threshold2.color") &&
                            _configContainer.GetValue("answer.threshold2.color") != string.Empty)
                    dicThreshold.Add("answer.threshold2.color", (string)_configContainer.GetValue("answer.threshold2.color"));

                if (_configContainer.AllKeys.Contains("answer.threshold3.color") &&
                            _configContainer.GetValue("answer.threshold3.color") != string.Empty)
                    dicThreshold.Add("answer.threshold3.color", (string)_configContainer.GetValue("answer.threshold3.color"));

                //End

                //Loading Hold Threshold Color

                if (_configContainer.AllKeys.Contains("hold.threshold1.color") &&
                            _configContainer.GetValue("hold.threshold1.color") != string.Empty)
                    dicThreshold.Add("hold.threshold1.color", (string)_configContainer.GetValue("hold.threshold1.color"));

                if (_configContainer.AllKeys.Contains("hold.threshold2.color") &&
                            _configContainer.GetValue("hold.threshold2.color") != string.Empty)
                    dicThreshold.Add("hold.threshold2.color", (string)_configContainer.GetValue("hold.threshold2.color"));

                if (_configContainer.AllKeys.Contains("hold.threshold3.color") &&
                            _configContainer.GetValue("hold.threshold3.color") != string.Empty)
                    dicThreshold.Add("hold.threshold3.color", (string)_configContainer.GetValue("hold.threshold3.color"));

                //End
                //Loading NotReady Threshold Color
                if (_configContainer.AllKeys.Contains("not-ready.threshold1.color") &&
                            _configContainer.GetValue("not-ready.threshold1.color") != string.Empty)
                    dicThreshold.Add("not-ready.threshold1.color", (string)_configContainer.GetValue("not-ready.threshold1.color"));

                if (_configContainer.AllKeys.Contains("not-ready.threshold2.color") &&
                            _configContainer.GetValue("not-ready.threshold2.color") != string.Empty)
                    dicThreshold.Add("not-ready.threshold2.color", (string)_configContainer.GetValue("not-ready.threshold2.color"));

                if (_configContainer.AllKeys.Contains("not-ready.threshold3.color") &&
                            _configContainer.GetValue("not-ready.threshold3.color") != string.Empty)
                    dicThreshold.Add("not-ready.threshold3.color", (string)_configContainer.GetValue("not-ready.threshold3.color"));

                //End
                //Loading Ready Threshold Color
                if (_configContainer.AllKeys.Contains("ready.threshold1.color") &&
                            _configContainer.GetValue("ready.threshold1.color") != string.Empty)
                    dicThreshold.Add("ready.threshold1.color", (string)_configContainer.GetValue("ready.threshold1.color"));

                if (_configContainer.AllKeys.Contains("ready.threshold2.color") &&
                            _configContainer.GetValue("ready.threshold2.color") != string.Empty)
                    dicThreshold.Add("ready.threshold2.color", (string)_configContainer.GetValue("ready.threshold2.color"));

                if (_configContainer.AllKeys.Contains("ready.threshold3.color") &&
                            _configContainer.GetValue("ready.threshold3.color") != string.Empty)
                    dicThreshold.Add("ready.threshold3.color", (string)_configContainer.GetValue("ready.threshold3.color"));

                var dic1 = dicThreshold.ToDictionary(r => r.Key, r => r.Value);
                Datacontext.loadThresholdColor = dic1;
                dicThreshold.Clear();
                //End
                //Loading Answer Threshold Time Value

                if (_configContainer.AllKeys.Contains("answer.threshold1.time") &&
                            _configContainer.GetValue("answer.threshold1.time") != string.Empty)
                {
                    DateTime green = Convert.ToDateTime((string)_configContainer.GetValue("answer.threshold1.time"));
                    dicThreshold.Add("answer.threshold1.time", green.TimeOfDay.TotalSeconds.ToString());
                }

                if (_configContainer.AllKeys.Contains("answer.threshold2.time") &&
                            _configContainer.GetValue("answer.threshold2.time") != string.Empty)
                {
                    DateTime green = Convert.ToDateTime((string)_configContainer.GetValue("answer.threshold2.time"));
                    dicThreshold.Add("answer.threshold2.time", green.TimeOfDay.TotalSeconds.ToString());
                }

                if (_configContainer.AllKeys.Contains("answer.threshold3.time") &&
                            _configContainer.GetValue("answer.threshold3.time") != string.Empty)
                {
                    DateTime green = Convert.ToDateTime((string)_configContainer.GetValue("answer.threshold3.time"));
                    dicThreshold.Add("answer.threshold3.time", green.TimeOfDay.TotalSeconds.ToString());
                }
                //End
                //Loading Hold Threshold Time Value

                if (_configContainer.AllKeys.Contains("hold.threshold1.time") &&
                            _configContainer.GetValue("hold.threshold1.time") != string.Empty)
                {
                    DateTime holdGreen = Convert.ToDateTime((string)_configContainer.GetValue("hold.threshold1.time"));
                    dicThreshold.Add("hold.threshold1.time", holdGreen.TimeOfDay.TotalSeconds.ToString());
                }

                if (_configContainer.AllKeys.Contains("hold.threshold2.time") &&
                            _configContainer.GetValue("hold.threshold2.time") != string.Empty)
                {
                    DateTime holdYellow = Convert.ToDateTime((string)_configContainer.GetValue("hold.threshold2.time"));
                    dicThreshold.Add("hold.threshold2.time", holdYellow.TimeOfDay.TotalSeconds.ToString());
                }

                if (_configContainer.AllKeys.Contains("hold.threshold3.time") &&
                            _configContainer.GetValue("hold.threshold3.time") != string.Empty)
                {
                    DateTime holdYellow = Convert.ToDateTime((string)_configContainer.GetValue("hold.threshold3.time"));
                    dicThreshold.Add("hold.threshold3.time", holdYellow.TimeOfDay.TotalSeconds.ToString());
                }
                //End
                //Loading NotReady Threshold Time Value
                if (_configContainer.AllKeys.Contains("not-ready.threshold1.time") &&
                            _configContainer.GetValue("not-ready.threshold1.time") != string.Empty)
                {
                    DateTime NRGreen = Convert.ToDateTime((string)_configContainer.GetValue("not-ready.threshold1.time"));
                    dicThreshold.Add("not-ready.threshold1.time", NRGreen.TimeOfDay.TotalSeconds.ToString());
                }

                if (_configContainer.AllKeys.Contains("not-ready.threshold2.time") &&
                            _configContainer.GetValue("not-ready.threshold2.time") != string.Empty)
                {
                    DateTime NRYellow = Convert.ToDateTime((string)_configContainer.GetValue("not-ready.threshold2.time"));
                    dicThreshold.Add("not-ready.threshold2.time", NRYellow.TimeOfDay.TotalSeconds.ToString());
                }

                if (_configContainer.AllKeys.Contains("not-ready.threshold3.time") &&
                            _configContainer.GetValue("not-ready.threshold3.time") != string.Empty)
                {
                    DateTime NRred = Convert.ToDateTime((string)_configContainer.GetValue("not-ready.threshold3.time"));
                    dicThreshold.Add("not-ready.threshold3.time", NRred.TimeOfDay.TotalSeconds.ToString());
                }
                //End
                //Loading Ready Threshold Value Time

                if (_configContainer.AllKeys.Contains("ready.threshold1.time") &&
                            _configContainer.GetValue("ready.threshold1.time") != string.Empty)
                {
                    DateTime RGreen = Convert.ToDateTime((string)_configContainer.GetValue("ready.threshold1.time"));
                    dicThreshold.Add("ready.threshold1.time", RGreen.TimeOfDay.TotalSeconds.ToString());
                }

                if (_configContainer.AllKeys.Contains("ready.threshold2.time") &&
                            _configContainer.GetValue("ready.threshold2.time") != string.Empty)
                {
                    DateTime RYellow = Convert.ToDateTime((string)_configContainer.GetValue("ready.threshold2.time"));
                    dicThreshold.Add("ready.threshold2.time", RYellow.TimeOfDay.TotalSeconds.ToString());
                }

                if (_configContainer.AllKeys.Contains("ready.threshold3.time") &&
                            _configContainer.GetValue("ready.threshold3.time") != string.Empty)
                {
                    DateTime Readyred = Convert.ToDateTime((string)_configContainer.GetValue("ready.threshold3.time"));
                    dicThreshold.Add("ready.threshold3.time", Readyred.TimeOfDay.TotalSeconds.ToString());
                }

                var dic = dicThreshold.ToDictionary(r => r.Key, r => Convert.ToInt32(r.Value));
                Datacontext.loadThresholdValues = dic;
                //End
            }
            catch (Exception ex)
            {
                _logger.Error((ex.InnerException == null) ? ex.Message : ex.InnerException.ToString());
            }
        }

        /// <summary>
        /// Logins with agent credential.
        /// </summary>
        private void LoginWithAgentCredential()
        {
            try
            {
                //_dataContext.IsVoiceChecked = true;
                Pointel.Configuration.Manager.Common.OutputValues responsefromLibrary = null;
                Dispatcher.Invoke((Action)delegate()
                {
                    borderContent.IsEnabled = false;
                    btnLogin.IsEnabled = false;
                    btnCancel.IsEnabled = false;
                    _host = cmbHostname.Text;
                    _port = cmbPort.Text;
                    _dataContext.Password = txtPassword.Password;
                });
                if (!string.IsNullOrEmpty(_host) && !_host.Contains(" ") && !_host.StartsWith(" ") && !_host.EndsWith(" "))
                {
                    if (!string.IsNullOrEmpty(_port) && !_port.Contains(" ") && !_port.StartsWith(" ") && !_port.EndsWith(" "))
                    {
                        if (Uri.TryCreate("tcp://" + this._host + ":" + this._port, UriKind.RelativeOrAbsolute, out _serverUri))
                            _backupServerUri = _serverUri;
                        else
                        {
                            Dispatcher.Invoke((Action)delegate()
                            {
                                ShowError("Please enter the valid Port");
                                btnLogin.IsEnabled = false;
                                btnCancel.IsEnabled = true;
                            });
                            goto End;
                        }
                    }
                    else
                    {
                        //_dataContext.ErrorMessage = "Please enter the valid Port";
                        //_dataContext.ErrorRowHeight = GridLength.Auto;
                        Dispatcher.Invoke((Action)delegate()
                        {
                            ShowError("Please enter the valid Port");
                            btnLogin.IsEnabled = false;
                            btnCancel.IsEnabled = true;
                        });
                        goto End;
                    }
                }
                else
                {
                    //_dataContext.ErrorMessage = "Please enter the valid Host Name";
                    //_dataContext.ErrorRowHeight = GridLength.Auto;
                    Dispatcher.Invoke((Action)delegate()
                    {
                        ShowError("Please enter the valid Host Name");
                        btnLogin.IsEnabled = false;
                        btnCancel.IsEnabled = true;
                    });
                    goto End;
                }
                Dispatcher.Invoke((Action)delegate()
                {
                    _frmAdminuserName = _dataContext.UserName = txtUserName.Text;
                    _frmAdminpassword = txtPassword.Password;
                    _dataContext.ApplicationName = txtApplication.Text;
                });
                //Connect with Configuration Server

                try
                {
                    if (_dataContext.IsCMELoginEnabled)
                        return;
                    Pointel.Configuration.Manager.ConfigManager _configManager = new Pointel.Configuration.Manager.ConfigManager();
                    string[] sectionToRead = { "_errors_", "_system_", "active-directory", "agent.ixn.desktop", "enable-disable-channels", "speed-dial.contacts" };
                    responsefromLibrary = _configManager.ConfigConnectionEstablish(_serverUri.Host, _serverUri.Port.ToString(),
                                 _dataContext.ApplicationName, _frmAdminuserName, _frmAdminpassword, _backupServerUri.Host, _backupServerUri.Port.ToString(),
                                 sectionToRead, "speed-dial.contacts", "", "");

                    if (responsefromLibrary != null && responsefromLibrary.MessageCode == "2001")
                    {
                        var splitErrorMessage = responsefromLibrary.Message.Split(new string[] { "Description:" }, StringSplitOptions.None);
                        if (splitErrorMessage.Count() > 1)
                        {
                            //_dataContext.ErrorMessage = splitErrorMessage[1].ToString();
                            //_dataContext.ErrorRowHeight = GridLength.Auto;
                            Dispatcher.Invoke((Action)delegate()
                            {
                                txtPassword.Password = string.Empty;
                                ShowError(splitErrorMessage[1].ToString());
                                btnLogin.IsEnabled = false;
                                btnCancel.IsEnabled = true;
                            });
                            _configManager.DisconnectConfigServer();
                        }
                        else
                        {
                            //_dataContext.ErrorMessage = splitErrorMessage[0].ToString();
                            //_dataContext.ErrorRowHeight = GridLength.Auto;
                            Dispatcher.Invoke((Action)delegate()
                            {
                                txtPassword.Password = string.Empty;
                                ShowError(splitErrorMessage[0].ToString());
                                btnLogin.IsEnabled = false;
                                btnCancel.IsEnabled = true;
                            });
                            _configManager.DisconnectConfigServer();
                        }
                    }
                    else
                    {
                        var isplaceenabled = true;
                        if (_configContainer.AllKeys.Contains("CfgPerson") &&
                                _configContainer.GetValue("CfgPerson") != null)
                        {
                            CfgPerson person = _configContainer.GetValue("CfgPerson");
                            if (person.IsAgent == CfgFlag.CFGFalse)
                            {
                                Dispatcher.Invoke((Action)delegate()
                                {
                                    ShowError("Person " + person.UserName + " is not defined as agent. Please contact your administrator");
                                    btnLogin.IsEnabled = false;
                                    btnCancel.IsEnabled = true;
                                });
                                return;
                            }
                            if (person.AgentInfo.Place != null && !_dataContext.KeepRecentPlace)
                            {
                                _dataContext.Place = person.AgentInfo.Place.Name.ToString();
                                _dataContext.AgentID = person.EmployeeID;
                            }
                        }
                        if (!string.IsNullOrEmpty(_place))
                            Dispatcher.Invoke((Action)delegate()
                                {
                                    isplaceenabled = !_dataContext.KeepRecentPlace;

                                    if (!_dataContext.HostNameItemSource.Contains(_dataContext.HostNameText))
                                        _dataContext.HostNameItemSource.Add(_dataContext.HostNameText);
                                    if (!_dataContext.PortItemSource.Contains(_dataContext.PortText))
                                        _dataContext.PortItemSource.Add(_dataContext.PortText);
                                });

                        #region Save Login details

                        if (!File.Exists(_dataContext.SettingsXMLFile))
                            _xmlHandler.CreateXMLFile(_dataContext.SettingsXMLFile);
                        _xmlHandler.ModifyXmlData(_dataContext.SettingsXMLFile, _xmlHandler.ConfigKeys[XMLHandler.Keys.ApplicationName].ToString(), _dataContext.ApplicationName);
                        _xmlHandler.ModifyXmlData(_dataContext.SettingsXMLFile, _xmlHandler.ConfigKeys[XMLHandler.Keys.UserName].ToString(), _dataContext.UserName);
                        _xmlHandler.ModifyXmlData(_dataContext.SettingsXMLFile, _xmlHandler.ConfigKeys[XMLHandler.Keys.Host].ToString(), _dataContext.HostNameText);
                        _xmlHandler.ModifyXmlData(_dataContext.SettingsXMLFile, _xmlHandler.ConfigKeys[XMLHandler.Keys.Port].ToString(), _dataContext.PortText);
                        if (_dataContext.KeepRecentPlace)
                            _xmlHandler.ModifyXmlData(_dataContext.SettingsXMLFile, _xmlHandler.ConfigKeys[XMLHandler.Keys.Place].ToString(), _dataContext.Place);
                        _xmlHandler.ModifyXmlData(_dataContext.SettingsXMLFile, _xmlHandler.ConfigKeys[XMLHandler.Keys.KeepPlace].ToString(), _dataContext.KeepRecentPlace.ToString());

                        #region Adding configured serve details to collection

                        string fg = _xmlHandler.ConfigKeys[XMLHandler.Keys.ConfigServers].ToString();
                        string[] temp = _xmlHandler.ReadXmlData(_dataContext.SettingsXMLFile, _xmlHandler.ConfigKeys[XMLHandler.Keys.ConfigServers].ToString());
                        string value = string.Empty;
                        if (temp != null)
                        {
                            var values = temp[1].Split(',');
                            string newValue = _dataContext.HostNameText + ":" + _dataContext.PortText;
                            if (!values.Contains(newValue))
                                value = temp[1] + "," + newValue;
                        }
                        else
                            value = _dataContext.HostNameText + ":" + _dataContext.PortText;
                        if (_matchingipport.ContainsKey(_dataContext.HostNameText))
                        {
                            if (!_matchingipport[_dataContext.HostNameText].ToString().Contains(_dataContext.PortText))
                            {
                                if (!string.IsNullOrEmpty(value))
                                    _xmlHandler.ModifyXmlData(_dataContext.SettingsXMLFile, _xmlHandler.ConfigKeys[XMLHandler.Keys.ConfigServers].ToString(), value);
                            }
                        }
                        else
                        {
                            if (!string.IsNullOrEmpty(value))
                                _xmlHandler.ModifyXmlData(_dataContext.SettingsXMLFile, _xmlHandler.ConfigKeys[XMLHandler.Keys.ConfigServers].ToString(), value);
                        }

                        #endregion Adding configured serve details to collection

                        //var storage = new XMLStorage();
                        //storage.SaveInitializeParameters(_dataContext.ApplicationName, _dataContext.UserName, "",
                        //    _dataContext.HostNameText, _dataContext.PortText, _dataContext.Place,
                        //    _dataContext.KeepRecentPlace.ToString(), _dataContext.QueueSelectedValue.ToString(), null);
                        _logger.Info("Login: Updated Config file");

                        #endregion Save Login details

                        //Get the switch type from system section from comclass
                        //ComClass.GetSwitchType();
                        GetCMEValues();

                        //Get the connection tab tserver name and switch DBId - Smoorthy
                        getAppServers(_dataContext.ApplicationName);

                        if (_dataContext.KeepRecentPlace)
                            _dataContext.Place = _place;
                        Dispatcher.Invoke((Action)delegate()
                        {
                            var channel = new ChannelSelection(isplaceenabled);
                            channel.Show();
                            //_dataContext.ErrorMessage = string.Empty;
                            // _dataContext.ErrorRowHeight = new GridLength(0);
                            CloseError(null, null);
                            btnLogin.IsEnabled = true;
                            btnCancel.IsEnabled = true;
                            this.Hide();
                        });
                    }
                }
                catch (Exception error)
                {
                    Dispatcher.Invoke((Action)delegate()
                        {
                            using (MessageBox msgBox = new MessageBox("Error", "Error : " + error.Message.ToString() + " Please contact your administrator.", "", "_Ok", false))
                            {
                                msgBox.ShowDialog();
                            }
                        });
                }
            End:
                return;
            }
            catch (Exception error)
            {
                Dispatcher.Invoke((Action)delegate()
                        {
                            using (MessageBox msgBox = new MessageBox("Error", error.Message.ToString() + " Please contact your administrator.", "", "_Ok", false))
                            {
                                msgBox.ShowDialog();
                            }
                        });
            }
        }

        private void Login_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            var source = PresentationSource.FromVisual(this) as HwndSource;
            source.RemoveHook(WndProc);
            foreach (var item in Resources)
                Resources.Remove(item);
            CultureCode = null;
            _logger = null;
            _host = null;
            _port = null;
            _previouslang = null;
            _frmAdminuserName = null;
            _frmAdminpassword = null;
            _place = null;
            _matchingipport = null;
            _serverUri = null;
            _backupServerUri = null;
            _shadowEffect = null;
            _xmlHandler = null;
            _dataContext = null;
            _configContainer = null;
            this.Dispatcher.Invoke(DispatcherPriority.Render, GCDelegate);
            //this.DataContext = null;
        }

        /// <summary>
        /// Handles the StateChanged event of the Login control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="EventArgs"/> instance containing the event data.</param>
        private void Login_StateChanged(object sender, EventArgs e)
        {
            WindowState = System.Windows.WindowState.Normal;
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
                if (this.Left < 0)
                    this.Left = 0;
                if (this.Top < 0)
                    this.Top = 0;
                if (this.Left > System.Windows.SystemParameters.WorkArea.Right - this.Width)
                    this.Left = System.Windows.SystemParameters.WorkArea.Right - this.Width;
                if (this.Top > System.Windows.SystemParameters.WorkArea.Bottom - this.Height)
                    this.Top = System.Windows.SystemParameters.WorkArea.Bottom - this.Height;
            }
            catch { }
        }

        /// <summary>
        /// Previews the key up.
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="e">The <see cref="System.Windows.Input.KeyEventArgs"/> instance containing the event data.</param>
        private void PreviewKeyUp(object sender, System.Windows.Input.KeyEventArgs e)
        {
            //if (e.Key == Key.Enter)
            //    btnLogin_Click(null, null);
            Key key = (e.Key == Key.System ? e.SystemKey : e.Key);
            if (key == Key.Enter)
                btnLogin_Click(null, null);
        }

        /// <summary>
        /// Start timer for error
        /// </summary>
        private void starttimerforerror()
        {
            try
            {
                if (_timerforcloseError == null)
                {
                    _timerforcloseError = new DispatcherTimer();
                    _timerforcloseError.Interval = TimeSpan.FromSeconds(10);
                    _timerforcloseError.Tick += new EventHandler(CloseError);
                    _timerforcloseError.Start();
                }
                else
                    _timerforcloseError.Interval = TimeSpan.FromSeconds(10);
            }
            catch (Exception ex)
            {
                _logger.Error("Error occurred at starttimerforerror : " + ex.Message);
            }
        }

        /// <summary>
        /// Handles the KeyDown event of the txtApplication control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="System.Windows.Input.KeyEventArgs"/> instance containing the event data.</param>
        private void txtApplication_KeyDown(object sender, System.Windows.Input.KeyEventArgs e)
        {
            // _dataContext.ErrorRowHeight = new GridLength(0);
            CloseError(null, null);
            btnLogin.IsEnabled = true;
        }

        /// <summary>
        /// Handles the KeyDown event of the txtUserName control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="System.Windows.Input.KeyEventArgs" /> instance containing the event data.</param>
        private void txtUserName_KeyDown(object sender, System.Windows.Input.KeyEventArgs e)
        {
            // _dataContext.ErrorRowHeight = new GridLength(0);
            CloseError(null, null);
            btnLogin.IsEnabled = true;
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
            //_dataContext.Place = _place;
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
        /// <param name="e">The <see cref="System.Windows.Input.KeyEventArgs"/> instance containing the event data.</param>
        private void Window_KeyDown(object sender, System.Windows.Input.KeyEventArgs e)
        {
            if (Keyboard.Modifiers == ModifierKeys.Alt && (e.SystemKey == Key.Space || e.SystemKey == Key.F4))
            {
                e.Handled = true;
            }
            //Code added for the purpose of check caps lock is on alert - Smoorthy
            //03-04-2014
            if (System.Windows.Forms.Control.IsKeyLocked(Keys.CapsLock))
            {
                //_dataContext.ErrorMessage = "Caps Lock is On";
                //_dataContext.ErrorRowHeight = GridLength.Auto;
                ShowError("Caps Lock is On");
            }
            else if (_dataContext.ErrorMessage == "Caps Lock is On")
            {
                //_dataContext.ErrorRowHeight = new GridLength(0);
                CloseError(null, null);
            }
            //end
        }

        /// <summary>
        /// Handles the Loaded event of the Window control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="RoutedEventArgs" /> instance containing the event data.</param>
        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            try
            {
                SystemMenu = GetSystemMenu(new WindowInteropHelper(this).Handle, false);
                DeleteMenu(SystemMenu, SC_Move, MF_BYCOMMAND);
                DeleteMenu(SystemMenu, SC_Size, MF_BYCOMMAND);
                DeleteMenu(SystemMenu, SC_Maximize, MF_BYCOMMAND);
                var source = PresentationSource.FromVisual(this) as HwndSource;
                source.AddHook(WndProc);
                _dataContext.IsCMELoginEnabled = Convert.ToBoolean(ConfigurationManager.AppSettings["CMELogin"].ToString());
                //Configurations Library
                //_configListener = new Login();

                _host = ConfigurationManager.AppSettings["Host"].ToString();
                _port = ConfigurationManager.AppSettings["Port"].ToString();

                if (ApplicationDeployment.IsNetworkDeployed)
                    if (ConfigurationManager.AppSettings.AllKeys.Contains("application.version"))
                        if (!string.IsNullOrEmpty(ConfigurationManager.AppSettings["application.version"]))
                            loginTitleversion.Content = "V " + ConfigurationManager.AppSettings["application.version"].ToString();

                #region CME Login

                if (_dataContext.IsCMELoginEnabled)
                {
                    #region error check

                    if (!string.IsNullOrEmpty(_host) && !_host.Contains(" ") && !_host.StartsWith(" ") && !_host.EndsWith(" "))
                    {
                        if (!string.IsNullOrEmpty(_port) && !_port.Contains(" ") && !_port.StartsWith(" ") && !_port.EndsWith(" "))
                        {
                            _serverUri = new Uri("tcp://" + this._host + ":" + this._port);
                            _backupServerUri = new Uri("tcp://" + this._host + ":" + this._port);
                        }
                        else
                        {
                            //System.Windows.Forms.MessageBox.Show("Please enter the valid Port ", "Login Error",
                            //                                         System.Windows.Forms.MessageBoxButtons.OK, System.Windows.Forms.MessageBoxIcon.Hand);
                            var message = new MessageBox("Login Error", "Please enter the valid Port." + Environment.NewLine + "The application will exit now.", "", "_Exit", false);
                            message.ShowDialog();

                            ClearClipboard();
                            Environment.Exit(0);
                        }
                    }
                    else
                    {
                        //System.Windows.Forms.MessageBox.Show("Please enter the valid Host ", "Login Error",
                        //                                      MessageBoxButtons.OK, MessageBoxIcon.Hand);
                        var message = new MessageBox("Login Error", "Please enter the valid Host." + Environment.NewLine + "The application will exit now.", "", "_Exit", false);
                        message.ShowDialog();
                        ClearClipboard();
                        Environment.Exit(0);
                    }

                    #endregion error check

                    _frmAdminuserName = _dataContext.UserName = Environment.UserName;
                    _frmAdminpassword = ConfigurationManager.AppSettings["CMEAgentPassword"].ToString();
                    _dataContext.ApplicationName = ConfigurationManager.AppSettings["AIDAppName"].ToString();
                    _dataContext.Password = ConfigurationManager.AppSettings["CMEAgentPassword"].ToString();
                    try
                    {
                        //Connect with Configuration Server
                        //_responseConfig = _configSubscribe.ConfigConnectionEstablish(_serverUri.Host, _serverUri.Port.ToString(), _clientName,
                        //                                                           _frmAdminuserName, _frmAdminpassword, _backupServerUri.Host,
                        //                                                           _backupServerUri.Port.ToString());
                        Pointel.Configuration.Manager.ConfigManager _configManager = new Pointel.Configuration.Manager.ConfigManager();
                        string[] sectionToRead = { "_errors_", "_system_", "active-directory", "agent.ixn.desktop", "db-integration", "enable-disable-channels", "file-integration", "pipe-integration", "port-integration", "salesforce-integration", "speed-dial.contacts", "url-integration", "ws-integration", "db-integration", "epic-integration", "facet-integration", "gud-options" };
                        Pointel.Configuration.Manager.Common.OutputValues output = _configManager.ConfigConnectionEstablish(_serverUri.Host, _serverUri.Port.ToString(), _dataContext.ApplicationName,
                                            _frmAdminuserName, _frmAdminpassword, _backupServerUri.Host, _backupServerUri.Port.ToString(), sectionToRead, "speed-dial.contacts");
                        //End
                        //End
                        string usernamewin = Environment.UserName;
                        if (output.MessageCode == "2001")
                        {
                            var message = new MessageBox("Configuration Error", output.Message + Environment.NewLine + "The application will exit now.", "", "_Exit", false);
                            message.ShowDialog();
                            ClearClipboard();
                            Environment.Exit(0);
                        }
                        if (output.MessageCode == "200")
                        {
                            if (_configContainer.AllKeys.Contains("login.enable.active-directory") &&
                                ((string)_configContainer.GetValue("login.enable.active-directory")).ToLower().Equals("true") ? true : false)
                            {
                                string _domainName = string.Empty;
                                if (_configContainer.AllKeys.Contains("domain-name"))
                                    _domainName = (string)_configContainer.GetValue("domain-name");

                                string _userGroup = string.Empty;
                                if (_configContainer.AllKeys.Contains("user-group"))
                                    _userGroup = (string)_configContainer.GetValue("user-group");

                                if (!_domainName.Equals(WindowsIdentity.GetCurrent().Name.Split('\\')[0]) || _domainName.Equals(null)
                                            || _domainName.Equals(string.Empty))
                                {
                                    var message = new MessageBox("Incorrect Domain", "This Agent Has Not Been Present At This Domain " + _domainName + "." + Environment.NewLine + "The application will exit now.", "", "_Exit", false);
                                    message.ShowDialog();
                                    ClearClipboard();
                                    Environment.Exit(0);
                                }
                                else if (_domainName.Equals(WindowsIdentity.GetCurrent().Name.Split('\\')[0]))
                                {
                                    AppDomain appDomain = Thread.GetDomain();
                                    appDomain.SetPrincipalPolicy(PrincipalPolicy.WindowsPrincipal);

                                    var windowsPrincipal = (WindowsPrincipal)Thread.CurrentPrincipal;
                                    try
                                    {
                                        if (windowsPrincipal.Identity.IsAuthenticated)
                                        {
                                            if (!windowsPrincipal.IsInRole(_userGroup))
                                            {
                                                var message = new MessageBox("Incorrect User Group", "This Agent Has Not Been Present At This User Group " +
                                                    _userGroup + "." + Environment.NewLine + "The application will exit now.", "", "_Exit", false);
                                                message.ShowDialog();
                                                ClearClipboard();
                                                Environment.Exit(0);
                                            }
                                            else
                                            {
                                                _dataContext.UserName = WindowsIdentity.GetCurrent().Name.Split('\\')[1];
                                            }
                                        }
                                        else
                                        {
                                            var message = new MessageBox("Access denied", usernamewin + " is not authenticated." + Environment.NewLine +
                                                "The application will exit now.", "", "_Exit", false);
                                            message.ShowDialog();
                                            ClearClipboard();
                                            Environment.Exit(0);
                                        }
                                    }
                                    catch
                                    {
                                        var message = new MessageBox("Access denied", usernamewin + " is not authenticated." + Environment.NewLine +
                                            "The application will exit now.", "", "_Exit", false);
                                        message.ShowDialog();
                                        ClearClipboard();
                                        Environment.Exit(0);
                                    }
                                }
                            }
                            else
                            {
                                var message = new MessageBox("Configuration Error", "Active directory is not enabled, please contact your Administrator"
                                    + Environment.NewLine + "The application will exit now.", "", "_Exit", false);
                                message.ShowDialog();
                                ClearClipboard();
                                Environment.Exit(0);
                            }

                            //Retrieving IP Address from Local System
                            try
                            {
                                var ipEntry = Dns.GetHostByName(Dns.GetHostName());
                                var addr = ipEntry.AddressList;
                                if (addr.Length == 1)
                                {
                                    _place = Convert.ToString(addr[0]);
                                }
                                else
                                {
                                    foreach (var t in addr.Where(t => !t.ToString().StartsWith("127.0")))
                                    {
                                        _place = Convert.ToString(t);
                                    }
                                }
                                if (!string.IsNullOrEmpty(_place))
                                    _dataContext.AutoComplete.Add(_place);
                                var isAgentAuthenticated = ComClass.AuthenticateUser(_dataContext.UserName, _dataContext.Password);
                                if (isAgentAuthenticated)
                                {
                                    // if (_dataContext.KeepRecentPlace)
                                    _dataContext.Place = _place;

                                    var channel = new ChannelSelection(false);
                                    channel.Show();
                                    //_dataContext.ErrorRowHeight = new GridLength(0);
                                    btnLogin.IsEnabled = true;
                                    // _dataContext.ErrorMessage = string.Empty;
                                    CloseError(null, null);
                                    this.ShowInTaskbar = false;
                                    this.Hide();
                                }
                                else
                                {
                                    var message = new MessageBox("Access denied", usernamewin + " is not authenticated." + Environment.NewLine + "The application will exit now.", "", "_Exit", false);
                                    message.ShowDialog();
                                    ClearClipboard();
                                    Environment.Exit(0);
                                }
                            }
                            catch { }
                        }
                        else
                        {
                            //Code added for the purpose of check caps lock is on alert - Smoorthy
                            //03-04-2014
                            if (System.Windows.Forms.Control.IsKeyLocked(Keys.CapsLock))
                            {
                                //_dataContext.ErrorMessage = "Caps Lock is On";
                                //_dataContext.ErrorRowHeight = GridLength.Auto;
                                ShowError("Caps Lock is On");
                            }
                            //end
                            GetXMLValueAssignToControl();
                        }
                    }
                    catch
                    {
                    }
                }

                #endregion CME Login

                else
                {
                    //Code added for the purpose of check caps lock is on alert - Smoorthy
                    //03-04-2014
                    if (System.Windows.Forms.Control.IsKeyLocked(Keys.CapsLock))
                    {
                        //_dataContext.ErrorMessage = "Caps Lock is On";
                        //_dataContext.ErrorRowHeight = GridLength.Auto;
                        ShowError("Caps Lock is On");
                    }
                    //end
                    GetXMLValueAssignToControl();
                }
                btnLogin.Content = "_" + (string)FindResource("KeyLogIn");
                btnCancel.Content = "_" + (string)FindResource("KeyCancel");
                if (!string.IsNullOrEmpty(_dataContext.Subversion))
                    loginTitleversion.Content += " (" + _dataContext.Subversion + ")";
                //GetTrialXML();

                _dataContext.ConfDetails_SP_Height_keepplace = !string.IsNullOrEmpty(_place) ? new GridLength(28) : new GridLength(0);
                if (_dataContext.ConfDetails_SP_Height_keepplace.Value == 0)
                    KeyboardNavigation.SetIsTabStop(cbxKeepPlace, false);
                else
                    KeyboardNavigation.SetIsTabStop(cbxKeepPlace, true);
            }
            catch (Exception _generalException)
            {
                _logger.Error("Error occurred as " + _generalException.Message);
            }
        }

        #endregion Methods
    }
}