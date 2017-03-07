namespace Agent.Interaction.Desktop
{
    using System;
    using System.Linq;
    using System.Windows;
    using System.Windows.Controls;
    using System.Windows.Input;

    using Agent.Interaction.Desktop.Settings;

    using Pointel.Configuration.Manager;
    using Pointel.Softphone.Voice.Common;
    using Pointel.Softphone.Voice.Core;
    using System.Windows.Interop;
    using System.Runtime.InteropServices;

    /// <summary>
    /// Interaction logic for LogonInfo.xaml
    /// </summary>
    public partial class LogonInfo : Window
    {
        #region Fields

        private ConfigContainer _configContainer = ConfigContainer.Instance();
        private Datacontext _dataContext = Datacontext.GetInstance();
        private Pointel.Logger.Core.ILog _logger = Pointel.Logger.Core.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType,
            "AID");
        private SoftPhoneBar _view;

        private const int GWL_STYLE = -16; //WPF's Message code for Title Bar's Style 
        private const int WS_SYSMENU = 0x80000; //WPF's Message code for System Menu
        [DllImport("user32.dll", SetLastError = true)]
        private static extern int GetWindowLong(IntPtr hWnd, int nIndex);
        [DllImport("user32.dll")]
        private static extern int SetWindowLong(IntPtr hWnd, int nIndex, int dwNewLong);
        #endregion Fields

        #region Constructors

        /// <summary>
        /// Initializes a new instance of the <see cref="LogonInfo"/> class.
        /// </summary>
        public LogonInfo()
        {
            InitializeComponent();
            this.DataContext = Datacontext.GetInstance();
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
        /// Handles the Click event of the btnCancel control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="RoutedEventArgs"/> instance containing the event data.</param>
        private void btnCancel_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
            _dataContext.IsLogonInfoOpened = false;
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
                if (cbQueue.Text != "" && _dataContext.LoadCollection.FindIndex(x => x.Equals(cbQueue.Text, StringComparison.OrdinalIgnoreCase)) != -1)
                {
                    OutputValues loginResponse = null;
                    _dataContext.AgentPassword = txtAgentPassword.Password.ToString();
                    string workMode = "optional";
                    string switchtype = "avaya";
                    if (_dataContext.SwitchName.ToLower().Contains("nortel"))
                        switchtype = "nortel";
                    var softLogin = new SoftPhone();
                    if (!_dataContext.IsTserverConnected)
                    {
                        var media = (Datacontext.AvailableServerDic.Where(x => x.Value == Genesyslab.Platform.Configuration.Protocols.Types.CfgAppType.CFGTServer.ToString())).ToDictionary(x => x.Key, y => y.Value);
                        if (media == null)
                            return;
                        if (media.Count > 0)
                        {
                            if (Datacontext.TServersSwitchDic.ContainsKey(Datacontext.UsedTServerSwitchDBId)
                                && media.Any(x => (x.Key.ToString().Split(','))[0].Trim() == Datacontext.TServersSwitchDic[Datacontext.UsedTServerSwitchDBId]))
                            {
                                var tserverName = media.Where(x => (x.Key.ToString().Split(','))[0].Trim() == Datacontext.TServersSwitchDic[Datacontext.UsedTServerSwitchDBId]).FirstOrDefault().Key;
                                loginResponse = softLogin.Initialize(_dataContext.Place, _dataContext.UserName, _configContainer.ConfServiceObject, tserverName, _dataContext.AgentLoginId, _dataContext.AgentPassword, _dataContext.SwitchType);
                                if (loginResponse.MessageCode != "200") goto End;
                            }
                        }
                        else
                        {
                            _logger.Error("LogonInfo btnOk_Click: " + "T-server not configured");
                            goto End;
                        }
                    }
                    loginResponse = softLogin.Login(_dataContext.Place, _dataContext.UserName, workMode, _dataContext.Queue, _dataContext.AgentLoginId, _dataContext.AgentPassword);
                    if (loginResponse.MessageCode == "200")
                    {
                        if (_configContainer.AllKeys.Contains("login.voice.enable.auto-ready") &&
                                        ((string)_configContainer.GetValue("login.voice.enable.auto-ready")).ToLower().Equals("true"))
                        {
                            var softReady = new SoftPhone();
                            softReady.Ready();
                        }
                        _logger.Debug("Agent Login Successful");
                    }
                    else if (loginResponse.MessageCode == "2001" || loginResponse.MessageCode == "2002")
                    {
                        _logger.Error(" LogonInfo btnOk_Click:" + loginResponse.Message);
                    }
                    else if (loginResponse.MessageCode == "2004")
                    {
                        if (_view != null)
                            _view.PlaceAlreadyTaken(loginResponse);
                    }
                End:
                    _logger.Debug("LogonInfo btnok_click : Login using Place : " + _dataContext.Place + " Username : " + _dataContext.UserName +
                        " Queue : " + _dataContext.QueueSelectedValue + " Application : " + _dataContext.ApplicationName + " LoginID : " +
                        _dataContext.AgentLoginId);

                    this.Close();
                    _dataContext.IsLogonInfoOpened = false;
                }
            }
            catch (Exception commonException)
            {
                _logger.Error("btnOk_Click:" + commonException.ToString());
            }
        }

        /// <summary>
        /// Handles the KeyUp event of the cbQueue control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="KeyEventArgs"/> instance containing the event data.</param>
        private void cbQueue_KeyUp(object sender, KeyEventArgs e)
        {
            try
            {
                var cbSelected = sender as ComboBox;
                if (cbSelected != null)
                    btnOk.IsEnabled = _dataContext.LoadCollection.Contains(cbSelected.Text);
            }
            catch (Exception commonException)
            {
                _logger.Error("LogonInfo:cbQueue_KeyUp" + commonException.ToString());
            }
        }

        /// <summary>
        /// Handles the SelectionChanged event of the cbQueue control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="SelectionChangedEventArgs"/> instance containing the event data.</param>
        private void cbQueue_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            try
            {
                var selectedItem = cbQueue.SelectedItem;
                if (selectedItem == null) return;
                btnOk.IsEnabled = _dataContext.LoadCollection.FindIndex(x => x.Equals(cbQueue.SelectedItem.ToString(), StringComparison.OrdinalIgnoreCase)) != -1;
            }
            catch (Exception commonException)
            {
                _logger.Error("LogonInfo:cbQueue_SelectionChanged" + commonException.ToString());
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
                lblAgentPassword.IsEnabled = true;
                txtAgentPassword.IsEnabled = true;
                if (cbQueue.Text != string.Empty && _configContainer.AllKeys.Contains("login.voice.enable.prompt-dn-password")
                        && ((string)_configContainer.GetValue("login.voice.enable.prompt-dn-password")).ToLower().Equals("true"))
                    txtAgentPassword.Focus();
            }
            catch (Exception commonException)
            {
                _logger.Error("LogonInfo:chkbxVoice_Checked:" + commonException.ToString());
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
                lblAgentPassword.IsEnabled = false;
                txtAgentPassword.IsEnabled = false;
            }
            catch (Exception commonException)
            {
                _logger.Error("LogonInfo:chkbxVoice_Unchecked:" + commonException.ToString());
            }
        }

        /// <summary>
        /// Handles the MouseLeftButtonDown event of the lblTitle control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="MouseButtonEventArgs"/> instance containing the event data.</param>
        private void lblTitle_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
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

        /// <summary>
        /// Previews the key up.
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="e">The <see cref="KeyEventArgs"/> instance containing the event data.</param>
        private void PreviewKeyUp(object sender, KeyEventArgs e)
        {
            try
            {
                if (e.Key == Key.Enter)
                {
                    btnOk_Click(null, null);
                }
            }
            catch (Exception commonException)
            {
                _logger.Error("LogonInfo:PreviewKeyUp" + commonException.ToString());
            }
        }

        /// <summary>
        /// Handles the Loaded event of the Window control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="RoutedEventArgs"/> instance containing the event data.</param>
        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            try
            {
                var hwnd = new WindowInteropHelper(this).Handle;
                SetWindowLong(hwnd, GWL_STYLE, GetWindowLong(hwnd, GWL_STYLE) & ~WS_SYSMENU);

                lblAgentLoginID.Content = _dataContext.AgentLoginId;
                if (_configContainer.AllKeys.Contains("login.voice.enable.prompt-queue") &&
                        ((string)_configContainer.GetValue("login.voice.enable.prompt-queue")).ToLower().Equals("true"))
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
                if (_dataContext.LoadCollection.Count > 0)
                    _dataContext.QueueSelectedValue = _dataContext.LoadCollection.Contains(_dataContext.Queue) ? _dataContext.Queue : _dataContext.LoadCollection[0].ToString();
                if (cbQueue.Text != string.Empty || cbQueue.Text != "")
                    txtAgentPassword.Focus();
                else
                    cbQueue.Focus();
            }
            catch (Exception commonException)
            {
                _logger.Error("LogonInfo:Window_Loaded" + commonException.ToString());
            }
        }

        #endregion Methods

        private void Window_PreviewKeyDown(object sender, KeyEventArgs e)
        {
            if ((Keyboard.IsKeyDown(Key.LeftAlt) || Keyboard.IsKeyDown(Key.RightAlt)) && Keyboard.IsKeyDown(Key.F4))
                e.Handled = true;
        }
    }
}