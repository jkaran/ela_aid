namespace Agent.Interaction.Desktop
{
    using System;
    using System.Windows;
    using System.Windows.Input;
    using System.Windows.Media;
    using System.Windows.Media.Effects;
    using System.Windows.Threading;

    using Agent.Interaction.Desktop.Settings;

    using Genesyslab.Platform.Configuration.Protocols;
    using Genesyslab.Platform.Configuration.Protocols.ConfServer.Events;
    using Genesyslab.Platform.Configuration.Protocols.ConfServer.Requests.Security;
    using Genesyslab.Platform.Configuration.Protocols.Exceptions;

    using Pointel.Configuration.Manager;
    using Pointel.Connection.Manager;
    using System.Runtime.InteropServices;
    using System.Windows.Interop;

    /// <summary>
    /// Interaction logic for ChangePassword.xaml
    /// </summary>
    public partial class ChangePassword : Window
    {
        #region Fields

        private ConfigContainer _configContainer = ConfigContainer.Instance();
        private Datacontext _dataContext = Datacontext.GetInstance();
        private Pointel.Logger.Core.ILog _logger = Pointel.Logger.Core.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType,
               "AID");
        private DropShadowBitmapEffect _shadowEffect = new DropShadowBitmapEffect();
        private DispatcherTimer _timerforcloseError;

        private const int GWL_STYLE = -16; //WPF's Message code for Title Bar's Style 
        private const int WS_SYSMENU = 0x80000; //WPF's Message code for System Menu
        [DllImport("user32.dll", SetLastError = true)]
        private static extern int GetWindowLong(IntPtr hWnd, int nIndex);
        [DllImport("user32.dll")]
        private static extern int SetWindowLong(IntPtr hWnd, int nIndex, int dwNewLong);

        #endregion Fields

        #region Constructors

        public ChangePassword()
        {
            InitializeComponent();
            this.DataContext = Datacontext.GetInstance();
            _shadowEffect.ShadowDepth = 0;
            _shadowEffect.Opacity = 0.5;
            _shadowEffect.Softness = 0.5;
            _shadowEffect.Color = (Color)ColorConverter.ConvertFromString("#003660");
            _dataContext.CSErrorMessage = string.Empty;
            _dataContext.CSErrorRowHeight = new GridLength(0);
            txtOldPass.Focus();
        }

        #endregion Constructors

        #region Methods

        /// <summary>
        /// Closes the error.
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="e">The <see cref="EventArgs"/> instance containing the event data.</param>
        public void CloseError(object sender, EventArgs e)
        {
            try
            {
                _dataContext.CSErrorMessage = "";
                _dataContext.CSErrorRowHeight = new GridLength(0);
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
        /// Handles the Click event of the btnCancel control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="RoutedEventArgs"/> instance containing the event data.</param>
        private void btnCancel_Click(object sender, RoutedEventArgs e)
        {
            this.DialogResult = false;
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
                bool sendRequest = true;
                string error = "";
                if (txtNewPass.Password != txtConfPass.Password)
                {
                    sendRequest = false;
                    error = "The confirmation password that you entered does not match the password that you entered."
                    + "Please ensure that the password and confirmation match exactly";
                }

                if (sendRequest)
                {
                    var changepassword = (ProtocolManagers.Instance().ProtocolManager[ServerType.Configserver.ToString()] as ConfServerProtocol).Request(
                        RequestChangePassword.Create(_dataContext.UserName, txtOldPass.Password, txtConfPass.Password));
                    if (changepassword is EventPasswordChanged)
                    {
                        _logger.Info((changepassword as EventPasswordChanged).ToString());
                        this.DialogResult = true;
                        this.Close();
                    }
                    else if (changepassword is Genesyslab.Platform.Configuration.Protocols.ConfServer.Events.EventError)
                    {
                        error = (changepassword as Genesyslab.Platform.Configuration.Protocols.ConfServer.Events.EventError).Description;
                        _dataContext.CSErrorMessage = error;
                        _dataContext.CSErrorRowHeight = GridLength.Auto;
                        StartTimerForError();
                        _logger.Info(error);
                    }
                }
                else
                {
                    _dataContext.CSErrorMessage = error;
                    _dataContext.CSErrorRowHeight = GridLength.Auto;
                    StartTimerForError();
                }
            }
            catch (ChangePasswordException chaPEx)
            {
                _dataContext.CSErrorMessage = chaPEx.Message;
                _dataContext.CSErrorRowHeight = GridLength.Auto;
                StartTimerForError();
                _logger.Error(chaPEx.Message);
            }
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

        /// <summary>
        /// Start timer for error
        /// </summary>
        private void StartTimerForError()
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
        /// Handles the KeyDown event of the txtUserName control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="System.Windows.Input.KeyEventArgs" /> instance containing the event data.</param>
        private void txtUserName_KeyDown(object sender, System.Windows.Input.KeyEventArgs e)
        {
            CloseError(null, null);
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
                StartTimerForError();
            }
            else
            {
                _dataContext.CSErrorRowHeight = new GridLength(0);
            }
            //end
        }

        #endregion Methods

        private void channelWindow_Loaded(object sender, RoutedEventArgs e)
        {
            try
            {
                var hwnd = new WindowInteropHelper(this).Handle;
                SetWindowLong(hwnd, GWL_STYLE, GetWindowLong(hwnd, GWL_STYLE) & ~WS_SYSMENU);
            }
            catch (Exception _generalException)
            {
                _logger.Error("Error occurred as " + _generalException.Message);
            }
        }
    }
}