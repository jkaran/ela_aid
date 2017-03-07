namespace Agent.Interaction.Desktop
{
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Linq;
    using System.Reflection;
    using System.Runtime.InteropServices;
    using System.Windows;
    using System.Windows.Forms;
    using System.Windows.Input;
    using System.Windows.Interop;
    using System.Windows.Media;
    using System.Windows.Media.Effects;
    using System.Windows.Threading;

    using Agent.Interaction.Desktop.Settings;

    using Pointel.Configuration.Manager;
    using Pointel.Integration.PlugIn;

    /// <summary>
    /// Interaction logic for GvasLogin.xaml
    /// </summary>
    public partial class GvasLogin : Window
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

        private IntPtr SystemMenu;
        private ConfigContainer _configContainer = ConfigContainer.Instance();
        private Datacontext _dataContext = Datacontext.GetInstance();
        private Pointel.Logger.Core.ILog _logger = Pointel.Logger.Core.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType,
            "AID");
        private Window _parentWindow = null;
        private DropShadowBitmapEffect _shadowEffect = new DropShadowBitmapEffect();
        private DispatcherTimer _timerforcloseError;

        #endregion Fields

        #region Constructors

        public GvasLogin(Window win)
        {
            InitializeComponent();
            _parentWindow = win;
            brd_Hide.Visibility = System.Windows.Visibility.Visible;
            var showorhideapps = ConfigContainer.Instance().GetAsString("thirdparty.integerations.apps");
            if (!string.IsNullOrEmpty(showorhideapps.Trim()) && showorhideapps != "No key found")
            {
                if (!showorhideapps.ToLower().Contains("gvas"))
                    chkb_gvas.Visibility = Visibility.Collapsed;
                if (!showorhideapps.ToLower().Contains("evas"))
                    chkb_evas.Visibility = Visibility.Collapsed;
                if (!showorhideapps.ToLower().Contains("nvas"))
                    chkb_nvas.Visibility = Visibility.Collapsed;
            }
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
                _dataContext.ErrorMessage = string.Empty;
                _dataContext.ErrorRowHeight = new GridLength(0);
                if (chkb_evas.IsChecked == false && chkb_gvas.IsChecked == false && chkb_nvas.IsChecked == false)
                    btnLogin.IsEnabled = false;
                else if (btnLogin.IsEnabled == false)
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

        private void btnCancel_Click(object sender, RoutedEventArgs e)
        {
            //if it is cancel
            //Environment.Exit(0);
            _parentWindow.Close();
            Close();
        }

        private void btnLogin_Click(object sender, RoutedEventArgs e)
        {
            var thirdParty = GetThirdParty();
            if (thirdParty != null)
            {
                var listAppDataDetails = new List<ApplicationDataDetails>();
                //checked for Gvas
                if (chkb_gvas.IsChecked == true)
                {
                    var gvasdetails = new ApplicationDataDetails();
                    gvasdetails.ApplicationName = "GVAS";
                    gvasdetails.DataToSent = new Dictionary<string, string>();
                    gvasdetails.DataToSent.Add("username", txtUserName.Text);
                    gvasdetails.DataToSent.Add("password", txtPassword.Password);
                    listAppDataDetails.Add(gvasdetails);
                }
                //checked for Evas
                if (chkb_evas.IsChecked == true)
                {
                    var gvasdetails = new ApplicationDataDetails();
                    gvasdetails.ApplicationName = "EVAS";
                    gvasdetails.DataToSent = new Dictionary<string, string>();
                    gvasdetails.DataToSent.Add("username", txtUserName.Text);
                    gvasdetails.DataToSent.Add("password", txtPassword.Password);
                    listAppDataDetails.Add(gvasdetails);

                }
                //checked for Nvas
                if (chkb_nvas.IsChecked == true)
                {
                    var gvasdetails = new ApplicationDataDetails();
                    gvasdetails.ApplicationName = "NVAS";
                    gvasdetails.DataToSent = new Dictionary<string, string>();
                    gvasdetails.DataToSent.Add("username", _dataContext.UserName);
                    gvasdetails.DataToSent.Add("password", _dataContext.Password);
                    listAppDataDetails.Add(gvasdetails);
                }

                thirdParty.StartWebIntegration(listAppDataDetails);

                // Hard coded by sakthi to test HIMMS integration.
                // thirdParty.StartHIMMSIntegration();
            }
            _parentWindow.Close();
            Close();
        }

        private void chkb_Checked(object sender, RoutedEventArgs e)
        {
            var chkBox = sender as System.Windows.Controls.CheckBox;
            if (chkBox.Name == "chkb_gvas")
            {
                chkb_evas.IsChecked = false;
                chkb_nvas.IsChecked = false;
                if (brd_Hide.Visibility != System.Windows.Visibility.Collapsed)
                    brd_Hide.Visibility = System.Windows.Visibility.Collapsed;
            }
            if (chkBox.Name == "chkb_evas")
            {
                chkb_gvas.IsChecked = false;
                if (brd_Hide.Visibility != System.Windows.Visibility.Collapsed)
                    brd_Hide.Visibility = System.Windows.Visibility.Collapsed;
            }
            if (chkBox.Name == "chkb_nvas")
            {
                chkb_gvas.IsChecked = false;
                if (chkb_evas.IsChecked == false)
                {
                    if (brd_Hide.Visibility != System.Windows.Visibility.Visible)
                        brd_Hide.Visibility = System.Windows.Visibility.Visible;
                }
                else if (brd_Hide.Visibility == System.Windows.Visibility.Collapsed)
                    brd_Hide.Visibility = System.Windows.Visibility.Collapsed;
            }
            EnableLogin();
        }

        private void chkb_Unchecked(object sender, RoutedEventArgs e)
        {
            var chkBox = sender as System.Windows.Controls.CheckBox;
            if (chkBox.Name == "chkb_gvas")
            {

            }
            else if (chkBox.Name == "chkb_evas")
            {
                if (brd_Hide.Visibility != System.Windows.Visibility.Visible)
                    brd_Hide.Visibility = System.Windows.Visibility.Visible;

            }
            else if (chkBox.Name == "chkb_nvas" && chkb_evas.IsChecked == true)
            {
                if (brd_Hide.Visibility != System.Windows.Visibility.Collapsed)
                    brd_Hide.Visibility = System.Windows.Visibility.Collapsed;
            }
            EnableLogin();
        }

        private void EnableLogin()
        {
            if (chkb_evas.IsChecked == false && chkb_gvas.IsChecked == false && chkb_nvas.IsChecked == false)
                btnLogin.IsEnabled = false;
            else if (btnLogin.IsEnabled == false)
                btnLogin.IsEnabled = true;
        }

        IDesktopMessenger GetThirdParty()
        {
            var file = Path.Combine(Path.Combine(System.Windows.Forms.Application.StartupPath, "Plugins"), "Pointel.Integration.Core.dll");
            if (File.Exists(file))
            {
                Assembly asm = Assembly.LoadFile(file);
                return (IDesktopMessenger)(from asmType in asm.GetTypes() where asmType.GetInterface("IDesktopMessenger") != null select (IDesktopMessenger)Activator.CreateInstance(asmType)).FirstOrDefault();
            }
            else
                _logger.Warn("Integration.Core Plug-in dll not exist");
            return null;
        }

        private void gvasloginWindow_Loaded(object sender, RoutedEventArgs e)
        {
            DataContext = _dataContext;
            SystemMenu = GetSystemMenu(new WindowInteropHelper(this).Handle, false);
            DeleteMenu(SystemMenu, SC_Move, MF_BYCOMMAND);
            DeleteMenu(SystemMenu, SC_Size, MF_BYCOMMAND);
            DeleteMenu(SystemMenu, SC_Maximize, MF_BYCOMMAND);
            var source = PresentationSource.FromVisual(this) as HwndSource;
            source.AddHook(WndProc);
            CloseError(null, null);

            btnLogin.Content = "_" + (string)FindResource("KeyLogIn");
            btnCancel.Content = "_" + (string)FindResource("KeySkip");
            if (!string.IsNullOrEmpty(_dataContext.Subversion))
                loginTitleversion.Content += " (" + _dataContext.Subversion + ")";
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
        /// Handles the KeyDown event of the txtUserName control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="System.Windows.Input.KeyEventArgs" /> instance containing the event data.</param>
        private void txtUserName_KeyDown(object sender, System.Windows.Input.KeyEventArgs e)
        {
            // _dataContext.ErrorRowHeight = new GridLength(0);
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
            else
            {
                base.OnKeyDown(e);
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

        #endregion Methods
    }
}