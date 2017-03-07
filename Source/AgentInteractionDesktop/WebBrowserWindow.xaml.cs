namespace Agent.Interaction.Desktop
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel;
    using System.IO;
    using System.Linq;
    using System.Linq.Expressions;
    using System.Runtime.InteropServices;
    using System.Text;
    using System.Windows;
    using System.Windows.Controls;
    using System.Windows.Data;
    using System.Windows.Documents;
    using System.Windows.Input;
    using System.Windows.Interop;
    using System.Windows.Media;
    using System.Windows.Media.Effects;
    using System.Windows.Media.Imaging;
    using System.Windows.Resources;
    using System.Windows.Shapes;

    using Agent.Interaction.Desktop.WebBrowserOverlay;

    using mshtml;

    using Pointel.Configuration.Manager;
    using Pointel.Tools;

    /// <summary>
    /// Interaction logic for WebBrowserWindow.xaml
    /// </summary>
    public partial class WebBrowserWindow : Window, INotifyPropertyChanged
    {
        #region Fields

        public const Int32 MF_BYPOSITION = 0x400;

        private const int CU_Close = 1002;
        private const int CU_Maximize = 1004;
        private const int CU_Minimize = 1000;
        private const int CU_Normal = 1001;
        private const int CU_Restore = 1003;
        private const int MF_BYCOMMAND = 0x00000000;
        private const int MF_DISABLED = 0x2;
        private const int MF_ENABLED = 0x0;
        private const int MF_GRAYED = 0x1;
        private const int SC_Close = 0x0000f060;
        private const int SC_Maximize = 0x0000f030;
        private const int SC_Minimize = 0x0000f020;
        private const int SC_Move = 0x0000f010;
        private const int SC_Restore = 0x0000f120;
        private const int SC_Size = 0x0000f000;
        private const Int32 WM_SYSCOMMAND = 0x112;

        bool firstTime = false;
        private Image imgPinClose = new Image();
        private Image imgPinOpen = new Image();
        private Image imgPin_EnterClose = new Image();
        private Image imgPin_EnterOpen = new Image();
        private IntPtr SystemMenu;
        private ConfigContainer _configContainer = ConfigContainer.Instance();
        private Brush _mainBorderBrush;
        private DropShadowBitmapEffect _shadowEffect = new DropShadowBitmapEffect();
        private double _tempHeight;
        private double _tempLeft;
        private double _tempTop;
        private double _tempWidth;
        private WebBrowserWindowState _webBrowserWindowState;

        #endregion Fields

        #region Constructors

        public WebBrowserWindow()
        {
            InitializeComponent();
            WindowResizer winResize = new WindowResizer(this);
            winResize.addResizerDown(BottomSideRect);
            winResize.addResizerRight(RightSideRect);
            winResize.addResizerRightDown(RightbottomSideRect);
            winResize = null;
            DataContext = this;
            HelperWindow helperWindow = new HelperWindow(BottomWindowGrid, this, null);
            webBrowser = helperWindow.WebBrowser;
            webBrowser.LoadCompleted += new System.Windows.Navigation.LoadCompletedEventHandler(webBrowser_LoadCompleted);
        }

        #endregion Constructors

        #region Enumerations

        private enum WebBrowserWindowState
        {
            Normal, Minimized, Maximized
        }

        #endregion Enumerations

        #region Events

        public event PropertyChangedEventHandler PropertyChanged;

        #endregion Events

        #region Properties

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

        #endregion Properties

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
        ///     Handles the Activated event of the Window control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="EventArgs" /> instance containing the event data.</param>
        public void Window_Activated(object sender, EventArgs e)
        {
            MainBorder.BitmapEffect = _shadowEffect;
            MainBorderBrush = (Brush)(new BrushConverter().ConvertFromString("#0070C5"));
            if (!firstTime)
            {
                //Width = grdTools_Buttons.ActualWidth + btn_Menu.Width + 20;
                //this.MinWidth = Width;
                firstTime = true;
            }
        }

        /// <summary>
        ///     Handles the Deactivated event of the Window control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="EventArgs" /> instance containing the event data.</param>
        public void Window_Deactivated(object sender, EventArgs e)
        {
            MainBorderBrush = Brushes.Black;
            MainBorder.BitmapEffect = null;
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

        [DllImport("user32.dll", SetLastError = true)]
        private static extern bool BringWindowToTop(IntPtr hWnd);

        [DllImport("user32", SetLastError = true)]
        private static extern bool CloseClipboard();

        [DllImport("user32.dll")]
        private static extern bool DeleteMenu(IntPtr hMenu, int uPosition, int uFlags);

        [DllImport("user32", SetLastError = true)]
        private static extern bool EmptyClipboard();

        [DllImport("user32.dll")]
        private static extern bool EnableMenuItem(IntPtr hMenu, Int32 uIDEnableItem, Int32 uEnable);

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

        [DllImport("user32.dll")]
        private static extern IntPtr GetSystemMenu(IntPtr hWnd, bool bRevert);

        [DllImport("user32.dll")]
        private static extern bool InsertMenu(IntPtr hMenu, Int32 wPosition, Int32 wFlags, Int32 wIDNewItem, string lpNewItem);

        /// <summary>
        ///     Handles the Click event of the btnExit control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="RoutedEventArgs" /> instance containing the event data.</param>
        private void btnExit_Click(object sender, RoutedEventArgs e)
        {
            //Dictionary<Datacontext.Channels, string> _dict = new Dictionary<Datacontext.Channels, string>();
            //_dict = _dataContext.htMediaCurrentState.Cast<DictionaryEntry>().ToDictionary(kvp => (Datacontext.Channels)kvp.Key, kvp => (string)kvp.Value);
            //var keyValue = _dict.Where(x => x.Value.Contains("Pending"));
            //if (keyValue.Count() == 0)
            //{
            this.Close();
            //}
        }

        /// <summary>
        ///     Handles the Click event of the btnPin control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="RoutedEventArgs" /> instance containing the event data.</param>
        private void btnMaximize_Click(object sender, RoutedEventArgs e)
        {
            if (WindowState == System.Windows.WindowState.Minimized)
            {
                WindowState = System.Windows.WindowState.Normal;
            }
            if (_webBrowserWindowState == WebBrowserWindowState.Normal)
            {
                btnMaximize.Style = FindResource("RestoreButton") as Style;
                _tempWidth = this.ActualWidth;
                _tempHeight = this.ActualHeight;
                _tempLeft = this.Left;
                _tempTop = this.Top;
                _webBrowserWindowState = WebBrowserWindowState.Maximized;

                DeleteMenu(SystemMenu, CU_Maximize, MF_BYCOMMAND);
                DeleteMenu(SystemMenu, CU_Minimize, MF_BYCOMMAND);
                DeleteMenu(SystemMenu, CU_Restore, MF_BYCOMMAND);
                InsertMenu(SystemMenu, 0, MF_BYPOSITION, CU_Restore, "Restore");
                InsertMenu(SystemMenu, 1, MF_BYPOSITION, CU_Minimize, "Minimize");
                MainBorder.Margin = new Thickness(0);

                this.Width = System.Windows.SystemParameters.WorkArea.Width;
                this.Height = System.Windows.SystemParameters.WorkArea.Height;
                this.Left = 0;
                this.Top = 0;

                RightSideRect.Visibility = Visibility.Hidden;
                RightbottomSideRect.Visibility = Visibility.Hidden;
                BottomSideRect.Visibility = Visibility.Hidden;
            }
            else if (_webBrowserWindowState == WebBrowserWindowState.Maximized)
            {
                _webBrowserWindowState = WebBrowserWindowState.Normal;
                DeleteMenu(SystemMenu, CU_Restore, MF_BYCOMMAND);
                DeleteMenu(SystemMenu, CU_Minimize, MF_BYCOMMAND);
                DeleteMenu(SystemMenu, CU_Maximize, MF_BYCOMMAND);
                InsertMenu(SystemMenu, 0, MF_BYPOSITION, CU_Minimize, "Minimize");
                InsertMenu(SystemMenu, 1, MF_BYPOSITION, CU_Maximize, "Maximize");
                WindowState = System.Windows.WindowState.Normal;
                MainBorder.Margin = new Thickness(8);
                btnMaximize.Style = FindResource("MaximizeButton") as Style;
                this.Width = _tempWidth;
                this.Height = _tempHeight;
                this.Left = _tempLeft;
                this.Top = _tempTop;

                RightSideRect.Visibility = Visibility.Visible;
                RightbottomSideRect.Visibility = Visibility.Visible;
                BottomSideRect.Visibility = Visibility.Visible;
            }
        }

        /// <summary>
        ///     Handles the Click event of the btnMinimize control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="RoutedEventArgs" /> instance containing the event data.</param>
        private void btnMinimize_Click(object sender, RoutedEventArgs e)
        {
            WindowState = WindowState.Minimized;
            Topmost = false;
        }

        /// <summary>
        ///     Mouses the left button down.
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="e">The <see cref="MouseButtonEventArgs" /> instance containing the event data.</param>
        private void MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            try
            {
                if (e.ClickCount > 1)
                    e.Handled = true;
                DragMove();
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
                        Top = SystemParameters.WorkArea.Bottom - Height;
                }
            }
            catch (Exception commonException)
            {
                //_logger.Warn("Error occurred as " + commonException.Message.ToString());
            }
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

        /// <summary>
        ///     Handles the StateChanged event of the Window1 control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="System.EventArgs" /> instance containing the event data.</param>
        private void WebBrowserWin_StateChanged(object sender, EventArgs e)
        {
            StateChanged -= WebBrowserWin_StateChanged;
            if (WindowState == System.Windows.WindowState.Minimized)
            {
                DeleteMenu(SystemMenu, CU_Maximize, MF_BYCOMMAND);
                DeleteMenu(SystemMenu, CU_Minimize, MF_BYCOMMAND);
                DeleteMenu(SystemMenu, CU_Restore, MF_BYCOMMAND);
                InsertMenu(SystemMenu, 0, MF_BYPOSITION, CU_Restore, "Restore");
                InsertMenu(SystemMenu, 1, MF_BYPOSITION, CU_Maximize, "Maximize");
            }
            if ((WindowState == System.Windows.WindowState.Maximized))
            {
                WindowState = System.Windows.WindowState.Normal;
                btnMaximize_Click(null, null);
                DeleteMenu(SystemMenu, CU_Maximize, MF_BYCOMMAND);
                DeleteMenu(SystemMenu, CU_Minimize, MF_BYCOMMAND);
                DeleteMenu(SystemMenu, CU_Restore, MF_BYCOMMAND);
                InsertMenu(SystemMenu, 0, MF_BYPOSITION, CU_Restore, "Restore");
                InsertMenu(SystemMenu, 1, MF_BYPOSITION, CU_Minimize, "Minimize");
            }
            if (WindowState == System.Windows.WindowState.Normal)
            {
                DeleteMenu(SystemMenu, CU_Restore, MF_BYCOMMAND);
                DeleteMenu(SystemMenu, CU_Minimize, MF_BYCOMMAND);
                DeleteMenu(SystemMenu, CU_Maximize, MF_BYCOMMAND);
                InsertMenu(SystemMenu, 0, MF_BYPOSITION, CU_Minimize, "Minimize");
                InsertMenu(SystemMenu, 1, MF_BYPOSITION, CU_Maximize, "Maximize");
            }
            if (_webBrowserWindowState == WebBrowserWindowState.Maximized)
            {
                if (WindowState == System.Windows.WindowState.Minimized)
                {
                    DeleteMenu(SystemMenu, CU_Maximize, MF_BYCOMMAND);
                    DeleteMenu(SystemMenu, CU_Minimize, MF_BYCOMMAND);
                    DeleteMenu(SystemMenu, CU_Restore, MF_BYCOMMAND);
                    InsertMenu(SystemMenu, 0, MF_BYPOSITION, CU_Restore, "Restore");
                }
                else
                {
                    DeleteMenu(SystemMenu, CU_Maximize, MF_BYCOMMAND);
                    DeleteMenu(SystemMenu, CU_Minimize, MF_BYCOMMAND);
                    DeleteMenu(SystemMenu, CU_Restore, MF_BYCOMMAND);
                    InsertMenu(SystemMenu, 0, MF_BYPOSITION, CU_Restore, "Restore");
                    InsertMenu(SystemMenu, 1, MF_BYPOSITION, CU_Minimize, "Minimize");
                }
            }
            if (_webBrowserWindowState == WebBrowserWindowState.Maximized && WindowState == System.Windows.WindowState.Minimized)
            {

            }

            StateChanged += WebBrowserWin_StateChanged;
        }

        private void webBrowser_LoadCompleted(object sender, System.Windows.Navigation.NavigationEventArgs e)
        {
            try
            {
                var docs = webBrowser.Document as HTMLDocument;
                if (docs != null)
                {

                    IHTMLElementCollection elementCollection = docs.getElementsByTagName("title");
                    if (elementCollection != null && elementCollection.length > 0)
                    {
                        foreach (IHTMLElement element in elementCollection)
                        {
                            lblTitleStatus.Text = element.innerText;
                            break;
                        }
                    }
                }
            }
            catch (Exception generalException)
            {
                System.Windows.MessageBox.Show(generalException.Message);
            }
        }

        /// <summary>
        /// Handles the Closing event of the Window control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="System.ComponentModel.CancelEventArgs"/> instance containing the event data.</param>
        private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            try
            {
                //Environment.Exit(0);
            }
            catch { }
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
                DeleteMenu(SystemMenu, SC_Close, MF_BYCOMMAND);
                DeleteMenu(SystemMenu, SC_Restore, MF_BYCOMMAND);
                DeleteMenu(SystemMenu, SC_Minimize, MF_BYCOMMAND);
                InsertMenu(SystemMenu, 0, MF_BYPOSITION, CU_Minimize, "Minimize");
                InsertMenu(SystemMenu, 1, MF_BYPOSITION, CU_Maximize, "Maximize");
                InsertMenu(SystemMenu, 3, MF_BYPOSITION, CU_Close, "Close");
                var source = PresentationSource.FromVisual(this) as HwndSource;
                source.AddHook(WndProc);

                _shadowEffect.ShadowDepth = 0;
                _shadowEffect.Opacity = 0.5;
                _shadowEffect.Softness = 0.5;
                _shadowEffect.Color = (System.Windows.Media.Color)ColorConverter.ConvertFromString("#003660");
            }
            catch
            {

            }
        }

        private IntPtr WndProc(IntPtr hwnd, int msg, IntPtr wParam, IntPtr lParam, ref bool handled)
        {
            if (msg != WM_SYSCOMMAND) return IntPtr.Zero;
            //if (wParam.ToInt32().ToString() != "0" && wParam.ToInt32().ToString() != "1" && wParam.ToInt32().ToString() != "32")
            //    System.Windows.MessageBox.Show(wParam.ToInt32().ToString());
            //if (wParam.ToInt32().ToString() == "61536")
            //{
            //string d = "d";
            //}
            switch (wParam.ToInt32())
            {
                case CU_Minimize:
                    if (WindowState != System.Windows.WindowState.Minimized)
                        WindowState = System.Windows.WindowState.Minimized;
                    break;
                case CU_Restore:
                    if (WindowState != System.Windows.WindowState.Normal)
                        WindowState = System.Windows.WindowState.Normal;
                    else if (_webBrowserWindowState == WebBrowserWindowState.Maximized)
                    {
                        WindowState = System.Windows.WindowState.Normal;
                        btnMaximize_Click(null, null);
                        handled = true;
                    }
                    break;
                case CU_Maximize:
                    if (WindowState != System.Windows.WindowState.Maximized)
                    {
                        if (WindowState == System.Windows.WindowState.Minimized)
                        {
                            WindowState = System.Windows.WindowState.Normal;
                        }
                        btnMaximize_Click(null, null);
                        handled = true;
                    }
                    break;
                case CU_Close: //close
                    btnExit_Click(null, null);
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