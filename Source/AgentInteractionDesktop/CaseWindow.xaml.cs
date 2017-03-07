using System;
using System.Runtime.InteropServices;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Interop;
using System.Windows.Media;
using System.Windows.Media.Effects;
using Agent.Interaction.Desktop.Settings;
using Pointel.Configuration.Manager;
using System.Windows.Media.Imaging;

namespace Agent.Interaction.Desktop
{
    /// <summary>
    /// Interaction logic for CaseWindow.xaml
    /// </summary>
    public partial class CaseWindow : Window
    {
        private Pointel.Logger.Core.ILog _logger = Pointel.Logger.Core.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType,
            "AID");
        public DropShadowBitmapEffect ShadowEffect = new DropShadowBitmapEffect();
        public Image imgPinOpen = new Image();
        public Image imgPin_EnterOpen = new Image();
        public Image imgPinClose = new Image();
        public Image imgPin_EnterClose = new Image();
        Datacontext _dataContext;
        ConfigContainer _configContainer;

        private const int GWL_STYLE = -16; //WPF's Message code for Title Bar's Style 
        private const int WS_SYSMENU = 0x80000; //WPF's Message code for System Menu
        [DllImport("user32.dll", SetLastError = true)]
        private static extern int GetWindowLong(IntPtr hWnd, int nIndex);
        [DllImport("user32.dll")]
        private static extern int SetWindowLong(IntPtr hWnd, int nIndex, int dwNewLong);

        public CaseWindow()
        {
            InitializeComponent();
            this.DataContext = Datacontext.GetInstance();
            this._configContainer = ConfigContainer.Instance();
            ShadowEffect.ShadowDepth = 0;
            ShadowEffect.Opacity = 0.5;
            ShadowEffect.Softness = 0.5;
            ShadowEffect.Color = (Color)ColorConverter.ConvertFromString("#003660");
            //webBrowser.Visibility = System.Windows.Visibility.Visible;
            //webBrowser.Navigate(new Uri("https://www.google.com"));
        }
        /// <summary>
        /// Handles the Activated event of the Window control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="EventArgs"/> instance containing the event data.</param>
        private void Window_Activated(object sender, EventArgs e)
        {
            MainBorder.BorderBrush = new SolidColorBrush((Color)(ColorConverter.ConvertFromString("#0070C5")));
            MainBorder.BitmapEffect = ShadowEffect;
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
        /// Handles the Loaded event of the UserCallInfo control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="System.Windows.RoutedEventArgs"/> instance containing the event data.</param>
        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            var hwnd = new WindowInteropHelper(this).Handle;
            SetWindowLong(hwnd, GWL_STYLE, GetWindowLong(hwnd, GWL_STYLE) & ~WS_SYSMENU);
            _dataContext = Datacontext.GetInstance();
            HwndSource source = PresentationSource.FromVisual(this) as HwndSource;
            source.AddHook(WndProc);
            SystemMenu = GetSystemMenu(new WindowInteropHelper(this).Handle, false);
            //EnableMenuItem(SystemMenu, SC_Minimize, (uint)(MF_ENABLED | (true ? MF_ENABLED : MF_GRAYED)));
            DeleteMenu(SystemMenu, SC_Move, MF_BYCOMMAND);
            DeleteMenu(SystemMenu, SC_Size, MF_BYCOMMAND);
            DeleteMenu(SystemMenu, SC_Minimize, MF_BYCOMMAND);
            DeleteMenu(SystemMenu, SC_Maximize, MF_BYCOMMAND);
            DeleteMenu(SystemMenu, SC_Restore, MF_BYCOMMAND);
            InsertMenu(SystemMenu, 0, MF_BYPOSITION, CU_Minimize, "Minimize");
            InsertMenu(SystemMenu, 0, MF_BYPOSITION, CU_TopMost, "Topmost");
        }

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
            catch
            {
            }
        }

        private void btnMinimize_Click(object sender, RoutedEventArgs e)
        {
            WindowState = System.Windows.WindowState.Minimized;
        }

        private void btnExit_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }

        #region System Menu

        private IntPtr SystemMenu;
        public const Int32 WM_SYSCOMMAND = 0x112;
        private const int CU_Minimize = 1000;
        private const int CU_TopMost = 1001;
        private const int SC_Minimize = 0x0000f020;
        private const int SC_Maximize = 0x0000f030;
        private const int SC_Restore = 0x0000f120;
        private const int SC_Close = 0x0000f060;
        private const int SC_Size = 0x0000f000;
        private const int MF_BYCOMMAND = 0x00000000;
        private const int SC_Move = 0x0000f010;
        public const Int32 MF_BYPOSITION = 0x400;
        internal const UInt32 MF_ENABLED = 0x00000000;
        internal const UInt32 MF_GRAYED = 0x00000001;
        internal const UInt32 MF_DISABLED = 0x00000002;

        [DllImport("user32.dll")]
        private static extern IntPtr GetSystemMenu(IntPtr hWnd, bool bRevert);

        [DllImport("user32.dll")]
        private static extern bool DeleteMenu(IntPtr hMenu, int uPosition, int uFlags);

        [DllImport("user32.dll")]
        private static extern bool InsertMenu(IntPtr hMenu, Int32 wPosition, Int32 wFlags, Int32 wIDNewItem, string lpNewItem);

        [DllImport("user32.dll")]
        private static extern bool EnableMenuItem(IntPtr hMenu, uint uIDEnableItem, uint uEnable);

        private IntPtr WndProc(IntPtr hwnd, int msg, IntPtr wParam, IntPtr lParam, ref bool handled)
        {
            if (msg == WM_SYSCOMMAND)
            {
                //System.Windows.MessageBox.Show(wParam.ToInt32().ToString());
                switch (wParam.ToInt32())
                {
                    case SC_Close://close
                        //btnExit_Click(null, null);
                        handled = true;
                        break;

                    case SC_Move: // move
                        //MouseLeftButtonDown(null, null);
                        handled = true;
                        break;

                    case CU_Minimize: // Minimize
                        this.WindowState = System.Windows.WindowState.Minimized;
                        handled = true;
                        break;

                    case CU_TopMost:
                        // btnPin_Click(null, null);
                        handled = true;
                        break;

                    default:
                        break;
                }
            }

            return IntPtr.Zero;
        }

        #endregion System Menu

        private void btnDone_Click(object sender, RoutedEventArgs e)
        {

        }

        private void Window_SizeChanged(object sender, SizeChangedEventArgs e)
        {

        }

        private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {

        }

        private void Window_StateChanged(object sender, EventArgs e)
        {

        }

        private void btnMaximize_Click(object sender, RoutedEventArgs e)
        {

        }

        private void btnShowHideUrlPanel_Click(object sender, RoutedEventArgs e)
        {
            if (imgShowHideUrlPanel.Source.ToString().Contains("Show_Left.png"))
            {
                HideUserDataPanel();
            }
            else
            {
                ShowUserDataPanel();
            }
        }

        private void HideUserDataPanel()
        {
            try
            {
                imgShowHideUrlPanel.Source = new BitmapImage(new Uri("/Agent.Interaction.Desktop;component/Images/Hide_Left.png", UriKind.Relative));
                ToolHeading.Text = "Show";
                ToolContent.Text = "Agent can view the data panel";
                columnData.Width = new GridLength(0);
                columnData.MinWidth = 0;
                stkDataPanel.Visibility = System.Windows.Visibility.Collapsed;
            }
            catch (Exception ex)
            {

            }
        }

        private void ShowUserDataPanel()
        {
            try
            {
                imgShowHideUrlPanel.Source = new BitmapImage(new Uri("/Agent.Interaction.Desktop;component/Images/Show_Left.png", UriKind.Relative));
                ToolHeading.Text = "Hide";
                ToolContent.Text = "Agent can hide the data panel";
                columnData.Width = new GridLength(1, GridUnitType.Auto);
                columnData.MinWidth = 400;
                stkDataPanel.Visibility = System.Windows.Visibility.Visible;
            }
            catch (Exception ex)
            {

            }
        }


        private void btnData_Click(object sender, RoutedEventArgs e)
        {
            if (btnData.IsChecked == true)
            {
                btnContacts.IsEnabled = true;
                btnContacts.IsChecked = false;
                grdCaseData.Visibility = System.Windows.Visibility.Visible;
                grdContacts.Visibility = System.Windows.Visibility.Collapsed;
            }
        }

        private void btnContacts_Click(object sender, RoutedEventArgs e)
        {
            if (btnContacts.IsChecked == true)
            {
                btnData.IsEnabled = true;
                btnData.IsChecked = false;
                grdCaseData.Visibility = System.Windows.Visibility.Collapsed;
                grdContacts.Visibility = System.Windows.Visibility.Visible;
            }
        }

        private void Storyboard_Completed(object sender, EventArgs e)
        {
            this.Topmost = false;
        }

        public void Blink()
        {
            this.Topmost = true;
            BeginStoryboard((System.Windows.Media.Animation.Storyboard)FindResource("BlinkBorder"));
            this.Focus();
        }

        private void txtNotes_TextChanged(object sender, TextChangedEventArgs e)
        {

        }

        private void btnSaveNote_Click(object sender, RoutedEventArgs e)
        {

        }

        private void DGAttachData_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {

        }

        private void DGAttachData_BeginningEdit(object sender, Microsoft.Windows.Controls.DataGridBeginningEditEventArgs e)
        {

        }

        private void DGAttachData_PreparingCellForEdit(object sender, Microsoft.Windows.Controls.DataGridPreparingCellForEditEventArgs e)
        {

        }

        private void DGAttachData_RowEditEnding(object sender, Microsoft.Windows.Controls.DataGridRowEditEndingEventArgs e)
        {

        }

        private void btnUpdate_Click(object sender, RoutedEventArgs e)
        {

        }

        private void btnCancel_Click(object sender, RoutedEventArgs e)
        {

        }

        private void btnAddCallData_Click(object sender, RoutedEventArgs e)
        {

        }

        private void webBrowser_LoadCompleted(object sender, System.Windows.Navigation.NavigationEventArgs e)
        {

        }

        private void DataTabControl_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {

        }
    }
}
