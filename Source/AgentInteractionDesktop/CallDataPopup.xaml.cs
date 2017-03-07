namespace Agent.Interaction.Desktop
{
    using System;
    using System.Collections.Generic;
    using System.Collections.ObjectModel;
    using System.Diagnostics;
    using System.Linq;
    using System.Runtime.InteropServices;
    using System.Text.RegularExpressions;
    using System.Windows;
    using System.Windows.Controls;
    using System.Windows.Controls.Primitives;
    using System.Windows.Input;
    using System.Windows.Interop;
    using System.Windows.Media;
    using System.Windows.Media.Effects;
    using System.Windows.Media.Imaging;
    using System.Windows.Shapes;

    using Agent.Interaction.Desktop.Helpers;
    using Agent.Interaction.Desktop.Settings;

    using Pointel.Configuration.Manager;
    using Pointel.Softphone.Voice.Core;
    using Pointel.Tools;

    /// <summary>
    /// Interaction logic for CallDataPopup.xaml
    /// </summary>
    public partial class CallDataPopup : Window
    {
        #region Fields

        public const Int32 MF_BYPOSITION = 0x400;
        public const Int32 WM_SYSCOMMAND = 0x112;

        public Image imgPinClose = new Image();
        public Image imgPinOpen = new Image();
        public Image imgPin_EnterClose = new Image();
        public Image imgPin_EnterOpen = new Image();
        public DropShadowBitmapEffect ShadowEffect = new DropShadowBitmapEffect();

        internal const UInt32 MF_DISABLED = 0x00000002;
        internal const UInt32 MF_ENABLED = 0x00000000;
        internal const UInt32 MF_GRAYED = 0x00000001;

        private const int CU_Minimize = 1000;
        private const int CU_TopMost = 1001;
        private const int GWL_STYLE = -16; //WPF's Message code for Title Bar's Style
        private const int MF_BYCOMMAND = 0x00000000;
        private const int SC_Close = 0x0000f060;
        private const int SC_Maximize = 0x0000f030;
        private const int SC_Minimize = 0x0000f020;
        private const int SC_Move = 0x0000f010;
        private const int SC_Restore = 0x0000f120;
        private const int SC_Size = 0x0000f000;
        private const int WS_SYSMENU = 0x80000; //WPF's Message code for System Menu

        private GridLength baseheight = new GridLength(380);
        private Dictionary<string, string> callData = new Dictionary<string, string>();
        private List<string> MemTopValidated = new List<string>();
        private IntPtr SystemMenu;
        private ConfigContainer _configContainer = ConfigContainer.Instance();
        private Datacontext _dataContext = Datacontext.GetInstance();
        private double _height;
        private bool _isCaseDataManualBeginEdit;
        private double _left;
        private Pointel.Logger.Core.ILog _logger = Pointel.Logger.Core.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType,
            "AID");
        private double _top;
        private double _width;

        #endregion Fields

        #region Constructors

        public CallDataPopup(string position, string size)
        {
            InitializeComponent();
            WindowResizer winResize = new WindowResizer(this);
            winResize.addResizerDown(BottomSideRect);
            winResize.addResizerRight(RightSideRect);
            winResize.addResizerRightDown(RightbottomSideRect);
            winResize = null;
            DataContext = Datacontext.GetInstance();
            ShadowEffect.ShadowDepth = 0;
            ShadowEffect.Opacity = 0.5;
            ShadowEffect.Softness = 0.5;
            ShadowEffect.Color = (Color)ColorConverter.ConvertFromString("#003660");
            imgPinClose.Source = new BitmapImage(new Uri("/Agent.Interaction.Desktop;component/Images/Pin.Open.png", UriKind.Relative));
            imgPin_EnterClose.Source = new BitmapImage(new Uri("/Agent.Interaction.Desktop;component/Images/Pin.Open.Selected.png", UriKind.Relative));
            imgPinClose.Width = 18;
            imgPinClose.Height = 18;
            imgPin_EnterClose.Width = 18;
            imgPin_EnterClose.Height = 18;
            imgPinOpen.Source = new BitmapImage(new Uri("/Agent.Interaction.Desktop;component/Images/Pin.Close.png", UriKind.Relative));
            imgPin_EnterOpen.Source = new BitmapImage(new Uri("/Agent.Interaction.Desktop;component/Images/Pin.Close.Selected.png", UriKind.Relative));
            imgPinOpen.Width = 18;
            imgPinOpen.Height = 18;
            imgPin_EnterOpen.Width = 18;
            imgPin_EnterOpen.Height = 18;
            btnPin.Content = imgPinOpen;
            BindGrid();

            var _posi = position.Split(',');
            var _size = size.Split(',');
            if (_posi.Count() > 1)
            {
                _left = double.Parse(_posi[0]);
                _top = double.Parse(_posi[1]);
            }
            if (_size.Count() > 1)
            {
                _height = double.Parse(_size[0]);
                _width = double.Parse(_size[1]);
            }
        }

        #endregion Constructors

        #region Methods

        /// <summary>
        /// Gets the visual child.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="parent">The parent.</param>
        /// <returns></returns>
        public static T GetVisualChild<T>(Visual parent)
            where T : Visual
        {
            T child = default(T);
            int numVisuals = VisualTreeHelper.GetChildrenCount(parent);
            var vw = (Visual)VisualTreeHelper.GetChild(parent, 0);
            for (int i = 0; i < numVisuals; i++)
            {
                var v = (Visual)VisualTreeHelper.GetChild(parent, i);
                child = v as T ?? GetVisualChild<T>(v);
                if (child != null)
                {
                    break;
                }
            }
            return child;
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

        /// <summary>
        ///     Binds the grid.
        /// </summary>
        public void BindGrid()
        {
            try
            {
                if (_dataContext.userAttachData.Count != 0)
                {
                    _dataContext.NotifyCallData.Clear();
                    foreach (var pair in _dataContext.userAttachData)
                    {
                        //commented by vinoth on 06th Nov, but need to check
                        //if (pair.Key == "ANI" || pair.Key == "OtherDN")
                        //_dataContext.TitleText = pair.Value + " - Agent Interaction Desktop";
                        //end
                        if (_dataContext.NotifyCallData.Count(p => p.Key == pair.Key) == 0)
                        {
                            Datacontext.GetInstance()
                                .NotifyCallData.Add(new CallData(pair.Key, pair.Value,
                                    _dataContext.KeyFontFamily, _dataContext.KeyFontWeight));
                        }
                    }
                    _dataContext.NotifyCallData = new ObservableCollection<ICallData>(_dataContext.NotifyCallData.OrderBy(callData => callData.Key));
                }
                if (_dataContext.NotifyCallData.Count <= 0)
                {
                    DGAttachData.Visibility = Visibility.Collapsed;
                    txtAttachDataInfo.Visibility = Visibility.Visible;
                }
                else
                {
                    DGAttachData.Visibility = Visibility.Visible;
                    txtAttachDataInfo.Visibility = Visibility.Collapsed;
                }
            }
            catch (Exception ex)
            {
                _logger.Error("Error occurred as " + ex.Message);
            }
        }

        /// <summary>
        /// Handles the Click event of the CallDataAddMenuitem control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="RoutedEventArgs"/> instance containing the event data.</param>
        public void CallDataAddMenuitem1_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                MenuItem menuitem = sender as MenuItem;

                DGAttachData.UpdateLayout();
                if (DGAttachData.Items.Count > 2)
                    DGAttachData.ScrollIntoView(DGAttachData.Items[DGAttachData.Items.Count - 2]);
                int rowIndex =
                            Datacontext.GetInstance()
                                .NotifyCallData.IndexOf(
                                    Datacontext.GetInstance()
                                        .NotifyCallData.Where(p => p.Key == menuitem.Header.ToString())
                                        .FirstOrDefault());
                var dataGridCellInfo = new Microsoft.Windows.Controls.DataGridCellInfo(DGAttachData.Items[rowIndex], DGAttachData.Columns[1]);
                var cell = TryToFindGridCell(DGAttachData, dataGridCellInfo);
                if (cell == null) return;
                cell.Focus();
                _isCaseDataManualBeginEdit = true;
                DGAttachData.BeginEdit();

                if (_dataContext.NotifyCallData.Count <= 0)
                {
                    DGAttachData.Visibility = Visibility.Collapsed;
                    txtAttachDataInfo.Visibility = Visibility.Visible;
                }
                else
                {
                    DGAttachData.Visibility = Visibility.Visible;
                    txtAttachDataInfo.Visibility = Visibility.Collapsed;
                }
            }
            catch (Exception commonException)
            {
                _logger.Error("CallDataAddMenuitem_Click: " + commonException.Message.ToString());
            }
        }

        [DllImport("user32.dll")]
        private static extern bool DeleteMenu(IntPtr hMenu, int uPosition, int uFlags);

        [DllImport("user32.dll")]
        private static extern bool EnableMenuItem(IntPtr hMenu, uint uIDEnableItem, uint uEnable);

        [DllImport("user32.dll")]
        private static extern IntPtr GetSystemMenu(IntPtr hWnd, bool bRevert);

        [DllImport("user32.dll", SetLastError = true)]
        private static extern int GetWindowLong(IntPtr hWnd, int nIndex);

        [DllImport("user32.dll")]
        private static extern bool InsertMenu(IntPtr hMenu, Int32 wPosition, Int32 wFlags, Int32 wIDNewItem, string lpNewItem);

        [DllImport("user32.dll")]
        private static extern int SetWindowLong(IntPtr hWnd, int nIndex, int dwNewLong);

        /// <summary>
        /// Tries the automatic find grid cell.
        /// </summary>
        /// <param name="grid">The grid.</param>
        /// <param name="cellInfo">The cell information.</param>
        /// <returns></returns>
        private static Microsoft.Windows.Controls.DataGridCell TryToFindGridCell(Microsoft.Windows.Controls.DataGrid grid, Microsoft.Windows.Controls.DataGridCellInfo cellInfo)
        {
            Microsoft.Windows.Controls.DataGridCell result = null;
            Microsoft.Windows.Controls.DataGridRow row = null;
            grid.ScrollIntoView(cellInfo.Item);
            grid.UpdateLayout();
            row = (Microsoft.Windows.Controls.DataGridRow)grid.ItemContainerGenerator.ContainerFromItem(cellInfo.Item);
            if (row != null)
            {
                int columnIndex = grid.Columns.IndexOf(cellInfo.Column);
                if (columnIndex > -1)
                {
                    Microsoft.Windows.Controls.Primitives.DataGridCellsPresenter presenter = GetVisualChild<Microsoft.Windows.Controls.Primitives.DataGridCellsPresenter>(row);
                    result = presenter.ItemContainerGenerator.ContainerFromIndex(columnIndex) as Microsoft.Windows.Controls.DataGridCell;
                }
            }
            return result;
        }

        private void btnAddCallData_Click(object sender, RoutedEventArgs e)
        {
            _dataContext.CallDataAdd.PlacementTarget = btnAddCallData;
            _dataContext.CallDataAdd.IsOpen = true;
        }

        /// <summary>
        ///     Handles the Click event of the btnClear control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="RoutedEventArgs" /> instance containing the event data.</param>
        private void btnClear_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                DGAttachData.CancelEdit();
                BindGrid();
            }
            catch (Exception commonException)
            {
                _logger.Error("Error occurred as " + commonException.Message);
            }
        }

        /// <summary>
        /// Handles the Click event of the btnExit control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="System.Windows.RoutedEventArgs"/> instance containing the event data.</param>
        private void btnExit_Click(object sender, RoutedEventArgs e)
        {
            var window1 = IsWindowOpen<Window>("SoftphoneBar");
            if (window1 != null && window1 is SoftPhoneBar)
                (window1 as SoftPhoneBar).CloseCallDataWindow(this);
            this.Close();
        }

        /// <summary>
        /// Handles the Click event of the btnMinimize control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="System.Windows.RoutedEventArgs"/> instance containing the event data.</param>
        private void btnMinimize_Click(object sender, RoutedEventArgs e)
        {
            WindowState = System.Windows.WindowState.Minimized;
        }

        /// <summary>
        /// Handles the Click event of the btnPin control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="System.Windows.RoutedEventArgs"/> instance containing the event data.</param>
        private void btnPin_Click(object sender, RoutedEventArgs e)
        {
            Image temp = (Image)btnPin.Content;
            if (temp.Source == imgPinOpen.Source)
            {
                btnPin.Content = imgPinClose;
                //btnPinStatus = "Show";
            }
            if (temp.Source == imgPinClose.Source)
            {
                btnPin.Content = imgPinOpen;
                //btnPinStatus = "Show";
            }
            if (temp.Source == imgPin_EnterOpen.Source)
            {
                btnPin.Content = imgPin_EnterClose;
                //btnPinStatus = "Hide";
            }
            if (temp.Source == imgPin_EnterClose.Source)
            {
                btnPin.Content = imgPin_EnterOpen;
                //btnPinStatus = "Show";
            }
            if (!this.Topmost)
            {
                this.Topmost = true;
                DeleteMenu(SystemMenu, CU_TopMost, MF_BYCOMMAND);
                InsertMenu(SystemMenu, 0, MF_BYPOSITION, CU_TopMost, "Normal");
            }
            else
            {
                this.Topmost = false;
                DeleteMenu(SystemMenu, CU_TopMost, MF_BYCOMMAND);
                InsertMenu(SystemMenu, 0, MF_BYPOSITION, CU_TopMost, "Topmost");
            }
        }

        /// <summary>
        /// Handles the MouseEnter event of the btnPin control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="MouseEventArgs"/> instance containing the event data.</param>
        private void btnPin_MouseEnter(object sender, MouseEventArgs e)
        {
            Image temp = (Image)btnPin.Content;
            if (temp.Source == imgPinOpen.Source)
                btnPin.Content = imgPin_EnterOpen;
            if (temp.Source == imgPinClose.Source)
                btnPin.Content = imgPin_EnterClose;
        }

        /// <summary>
        /// Handles the MouseLeave event of the btnPin control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="MouseEventArgs"/> instance containing the event data.</param>
        private void btnPin_MouseLeave(object sender, MouseEventArgs e)
        {
            Image temp = (Image)btnPin.Content;
            if (temp.Source == imgPin_EnterOpen.Source)
                btnPin.Content = imgPinOpen;
            if (temp.Source == imgPin_EnterClose.Source)
                btnPin.Content = imgPinClose;
        }

        /// <summary>
        ///     Handles the Click event of the btnUpdate control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="RoutedEventArgs" /> instance containing the event data.</param>
        private void btnUpdate_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                var selectedCallData = DGAttachData.SelectedCells[0].Item as CallData;
                string key = selectedCallData.Key.ToString().Trim();
                string value = selectedCallData.Value.ToString().Trim();
                var updateCallData = new SoftPhone();
                if (_dataContext.userAttachData.ContainsKey(key))
                {
                    string originalValue = _dataContext.userAttachData[key];
                    if (value != originalValue)
                    {
                        _dataContext.userAttachData.Remove(key);
                        _dataContext.userAttachData.Add(key, value);
                        updateCallData.UpdateUserData(_dataContext.userAttachData);
                    }
                }
                BindGrid();
            }
            catch (Exception commonException)
            {
                _logger.Error("Error occurred as " + commonException.Message);
            }
        }

        private void CallDataWin_Loaded(object sender, RoutedEventArgs e)
        {
            try
            {
                var hwnd = new WindowInteropHelper(this).Handle;
                SetWindowLong(hwnd, GWL_STYLE, GetWindowLong(hwnd, GWL_STYLE) & ~WS_SYSMENU);

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

                Top = _top;
                Left = _left;
                Height = _height;
                Width = _width;

            }
            catch (Exception _generalException)
            {
                _logger.Error("Error occurred as " + _generalException.Message);

            }
        }

        private void CallDataWin_StateChanged(object sender, EventArgs e)
        {
            if (WindowState == System.Windows.WindowState.Maximized)
                WindowState = System.Windows.WindowState.Normal;
        }

        /// <summary>
        ///     Handles the BeginningEdit event of the DGAttachData control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">
        ///     The <see cref="Microsoft.Windows.Controls.DataGridBeginningEditEventArgs" /> instance containing the
        ///     event data.
        /// </param>
        private void DGAttachData_BeginningEdit(object sender, Microsoft.Windows.Controls.DataGridBeginningEditEventArgs e)
        {
            var selectedCallData = e.Row.Item as CallData;
            if (selectedCallData != null)
            {
                if (selectedCallData.Value.StartsWith("http") || selectedCallData.Value.StartsWith("www"))
                {
                    e.Cancel = true;
                    if (_configContainer.AllKeys.Contains("voice.enable.attach-data-popup-url") && ((string)_configContainer.GetValue("voice.enable.attach-data-popup-url")).ToLower().Equals("true"))
                    {
                        string urlString = selectedCallData.Value.Contains("http") ? selectedCallData.Value : "http://" + selectedCallData.Value;
                        Uri uriResult;
                        if (Uri.TryCreate(urlString, UriKind.Absolute, out uriResult)
                            && (uriResult.Scheme == Uri.UriSchemeHttp || uriResult.Scheme == Uri.UriSchemeHttps))
                            Process.Start(uriResult.AbsoluteUri);
                    }
                }

                //Code added - hide the editable option in above key in call data datagrid
                //Smoorthy - 07-11-2013
                else if (_configContainer.AllKeys.Contains("VoiceAttachDataKey") &&
                    _configContainer.GetValue("VoiceAttachDataKey") != null)
                {
                    //ANI CallID CallType ConnID OtherDN ThisDN
                    //if (!string.IsNullOrEmpty(selectedCallData.Value))
                    //{
                    if (_isCaseDataManualBeginEdit)
                    {
                        e.Cancel = false;
                        _isCaseDataManualBeginEdit = false;
                    }
                    else if (((List<string>)_configContainer.GetValue("VoiceAttachDataKey")).Any(
                                x =>
                                    Regex.Replace(x, @"\s+", "").ToString().Trim().ToLower() == selectedCallData.Key.ToString().Trim().ToLower()) &&
                                    (_configContainer.AllKeys.Contains("voice.enable.modify-case-data") && ((string)_configContainer.GetValue("voice.enable.modify-case-data")).ToLower().Equals("true")))
                    {
                        e.Cancel = false;
                    }
                    else
                    {
                        e.Cancel = true;
                    }
                    //}
                }
                else
                {
                    e.Cancel = true;
                }
                //End
            }
        }

        private void DGAttachData_KeyUp(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.C && Keyboard.Modifiers == ModifierKeys.Control)
            {
                try
                {
                    if (DGAttachData.SelectedCells != null && DGAttachData.SelectedCells[0].Item != null)
                    {
                        var data = (DGAttachData.SelectedCells[0].Item as CallData).Value;
                        data = Uri.UnescapeDataString(data);
                        Clipboard.Clear();
                        Clipboard.SetText(data);
                        return;
                    }
                    _logger.Warn("warning clipboard data not updated.");

                }
                catch (Exception ex)
                {
                    _logger.Warn("Warning occurred while copying data as " + ex.Message);
                }

            }
        }

        //void SetDataToClipboard(string data)
        //{
        //    Thread.Sleep(300);
        //    this.Dispatcher.Invoke((Action)(delegate
        //    {
        //        Clipboard.Clear();
        //        Clipboard.SetData(DataFormats.Text, data);
        //    }));
        //}
        //private void DGAttachData_CopyingRowClipboardContent(object sender, Microsoft.Windows.Controls.DataGridRowClipboardEventArgs e)
        //{
        //    Thread thread = new Thread(() => SetDataToClipboard(e.ClipboardRowContent[0].Content.ToString()));
        //    thread.Start();
        //    //var content = e.ClipboardRowContent[0].Content.ToString();
        //    //content = content.Substring(content.Length - 1, 1);
        //    //e.ClipboardRowContent.Clear();
        //    //e.ClipboardRowContent.Add(new Microsoft.Windows.Controls.DataGridClipboardCellContent(e.Item, (sender as Microsoft.Windows.Controls.DataGrid).Columns[1], content));
        //}
        /// <summary>
        /// Handles the PreparingCellForEdit event of the DGAttachData control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="Microsoft.Windows.Controls.DataGridPreparingCellForEditEventArgs"/> instance containing the event data.</param>
        private void DGAttachData_PreparingCellForEdit(object sender, Microsoft.Windows.Controls.DataGridPreparingCellForEditEventArgs e)
        {
            var contentPresenter = e.EditingElement as ContentPresenter;
            var editingTemplate = contentPresenter.ContentTemplate;
            var textBox = (editingTemplate as DataTemplate).FindName("txtValue", (contentPresenter as ContentPresenter));
            if (textBox == null) return;
            if (!(textBox is TextBox)) return;
            (textBox as TextBox).Focus();
        }

        /// <summary>
        /// Handles the RowEditEnding event of the DGAttachData control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="Microsoft.Windows.Controls.DataGridRowEditEndingEventArgs"/> instance containing the event data.</param>
        private void DGAttachData_RowEditEnding(object sender, Microsoft.Windows.Controls.DataGridRowEditEndingEventArgs e)
        {
            try
            {
                Microsoft.Windows.Controls.DataGridRow dgRow = e.Row;
                if (dgRow != null)
                {
                    var selectedCallData = dgRow.Item as CallData;
                    string key = selectedCallData.Key.ToString().Trim();
                    string value = selectedCallData.Value.ToString().Trim();
                    var updateCallData = new SoftPhone();
                    if (_dataContext.userAttachData.ContainsKey(key))
                    {
                        string originalValue = _dataContext.userAttachData[key];
                        if (value != originalValue)
                        {
                            _dataContext.userAttachData.Remove(key);
                            _dataContext.userAttachData.Add(key, value);
                            updateCallData.UpdateUserData(_dataContext.userAttachData);
                        }
                    }
                    BindGrid();
                }
            }
            catch (Exception ex)
            {
                _logger.Error("Error occurred as " + ex.Message);
            }
        }

        private void DGCasedataValue_Copy_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                if (DGAttachData.SelectedCells != null && DGAttachData.SelectedCells[0].Item != null)
                {
                    var data = (DGAttachData.SelectedCells[0].Item as CallData).Value;
                    data = Uri.UnescapeDataString(data);
                    Clipboard.Clear();
                    Clipboard.SetText(data);
                    return;
                }
                _logger.Warn("warning clipboard data not updated.");

            }
            catch (Exception ex)
            {
                _logger.Warn("Warning occurred while copying data as " + ex.Message);
            }
        }

        private void Himss_Click(object sender, RoutedEventArgs e)
        {
            var window = IsWindowOpen<Window>("SoftphoneBar");
            if (window != null && window is SoftPhoneBar)
            {
                var bar = (SoftPhoneBar)window;
                if (bar != null)
                    bar.btnHimss.RaiseEvent(new RoutedEventArgs(ButtonBase.ClickEvent));
            }
        }

        /// <summary>
        /// Mouses the left button down.
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="e">The <see cref="System.Windows.Input.MouseButtonEventArgs"/>instance containing the event data.</param>
        private void MouseLeftButtonDown(object sender, System.Windows.Input.MouseButtonEventArgs e)
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

        private IntPtr WndProc(IntPtr hwnd, int msg, IntPtr wParam, IntPtr lParam, ref bool handled)
        {
            if (msg == WM_SYSCOMMAND)
            {
                //System.Windows.MessageBox.Show(wParam.ToInt32().ToString());
                switch (wParam.ToInt32())
                {
                    case SC_Close://close
                        btnExit_Click(null, null);
                        handled = true;
                        break;

                    case SC_Move: // move
                        MouseLeftButtonDown(null, null);
                        handled = true;
                        break;

                    case CU_Minimize: // Minimize
                        this.WindowState = System.Windows.WindowState.Minimized;
                        handled = true;
                        break;

                    case CU_TopMost:
                        btnPin_Click(null, null);
                        handled = true;
                        break;

                    default:
                        break;
                }
            }

            return IntPtr.Zero;
        }

        #endregion Methods
    }
}