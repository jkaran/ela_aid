#region System Namespaces

using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Media.Effects;

#endregion System Namespaces

#region AID Namespaces

using Agent.Interaction.Desktop.Settings;
using Pointel.Softphone.Voice.Core;
using System.Windows.Interop;
using System.Runtime.InteropServices;
using System.Globalization;


#endregion AID Namespaces

namespace Agent.Interaction.Desktop
{
    /// <summary>
    /// Interaction logic for CallInfo
    /// </summary>
    public partial class CallInfo : Window
    {
        #region Member Declaration

        private Pointel.Logger.Core.ILog _logger = Pointel.Logger.Core.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType,
            "AID");
        public DropShadowBitmapEffect ShadowEffect = new DropShadowBitmapEffect();
        public Image imgPinOpen = new Image();
        public Image imgPin_EnterOpen = new Image();
        public Image imgPinClose = new Image();
        public Image imgPin_EnterClose = new Image();
        private List<string> MemTopValidated = new List<string>();
        private Dictionary<string, string> callData = new Dictionary<string, string>();
        private Datacontext _dataContext = Datacontext.GetInstance();

        private const int GWL_STYLE = -16; //WPF's Message code for Title Bar's Style 
        private const int WS_SYSMENU = 0x80000; //WPF's Message code for System Menu
        [DllImport("user32.dll", SetLastError = true)]
        private static extern int GetWindowLong(IntPtr hWnd, int nIndex);
        [DllImport("user32.dll")]
        private static extern int SetWindowLong(IntPtr hWnd, int nIndex, int dwNewLong);


        #endregion Member Declaration

        #region Constructor

        /// <summary>
        /// Initializes a new instance of the <see cref="CallInfo"/> class.
        /// </summary>
        public CallInfo()
        {
            try
            {
                InitializeComponent();
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
                callData.Clear();
                //LoadComboBoxValues(); 
                _dataContext.CurrentMemberId = 0;
                LoadMemberID();
                LoadCallData();
                //btnOk.IsEnabled = false;
               
            }
            catch (Exception generalException)
            {
                _logger.Error("CallInfo:Constructor():" + generalException.ToString());
            }
        }

        #endregion Constructor

        #region Window Events

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
        private void UserCallInfo_Loaded(object sender, RoutedEventArgs e)
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
            }
            catch (Exception _generalException)
            {
                _logger.Error("Error occurred as " + _generalException.Message);

            }
            
        }

        #endregion Window Events

        #region Control Events

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
        /// Handles the Click event of the btnOk control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="System.Windows.RoutedEventArgs"/> instance containing the event data.</param>
        private void btnOk_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                if (btnOk.IsEnabled == false) return;

                if (_dataContext.isOnCall)
                {
                    if (txtCSCI_PHONE.Text != string.Empty && txtMEMBER_ID.Text!=string.Empty)// && txtExtension.Text != string.Empty)
                    {
                        
                        if(_dataContext.MemberId.ContainsValue(txtMEMBER_ID.Text) && _dataContext.CurrentMemberId==0)
                        {
                            stkp_Error.Visibility = Visibility.Visible;
                            lblInformation.Content = " Member Id is already added";
                        }
                        else
                        {
                            if (txtMEMBER_DOB.Text != string.Empty)
                            {
                                if (ValidateDate(txtMEMBER_DOB.Text))
                                {
                                    if (Convert.ToDateTime(txtMEMBER_DOB.Text).Date > DateTime.Now.Date)
                                    {
                                        stkp_Error.Visibility = Visibility.Visible;
                                        lblInformation.Content = "Entered date of birth is greater than today";
                                    }
                                    else
                                    {
                                        UpdateUserData();
                                        this.Close();
                                    }
                                }
                                else
                                {
                                    stkp_Error.Visibility = Visibility.Visible;
                                    lblInformation.Content = "Date must be in a valid format mm/dd/yyyy";
                                }
                            }
                            else
                            {
                                UpdateUserData();
                                this.Close();
                            }
                        }
                    }
                    else
                    {
                        stkp_Error.Visibility = Visibility.Visible;
                        lblInformation.Content = " * Fields are mandatory";
                    }
                }
            }
            catch (Exception generalException)
            {
                _logger.Error("CallInfo:Ok Button Click event:" + generalException.ToString());
            }
        }

        /// <summary>
        /// Handles the KeyDown event of the TextOnly control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="System.Windows.Input.KeyEventArgs"/> instance containing the event data.</param>
        private void TextOnly_KeyDown(object sender, System.Windows.Input.KeyEventArgs e)
        {
            stkp_Error.Visibility = Visibility.Hidden;
            if (e.Key >= Key.A && e.Key <= Key.Z || e.Key == Key.Home
                    || e.Key == Key.Back || e.Key == Key.End || e.Key == Key.Tab ||
                    e.Key == Key.Delete || e.Key == Key.Left || e.Key == Key.Right)
            {
                e.Handled = false;
            }
            else
                e.Handled = true;
        }

        /// <summary>
        /// Handles the KeyDown event of the TextBox control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="System.Windows.Input.KeyEventArgs"/> instance containing the event data.</param>
        private void TextBox_KeyDown(object sender, System.Windows.Input.KeyEventArgs e)
        {
            stkp_Error.Visibility = Visibility.Hidden;
        }

        /// <summary>
        /// Handles the KeyDown event of the DOB TextBox control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="System.Windows.Input.KeyEventArgs"/> instance containing the event data.</param>
        private void TextDOB_KeyDown(object sender, System.Windows.Input.KeyEventArgs e)
        {
            stkp_Error.Visibility = Visibility.Hidden;
            if ((Keyboard.Modifiers & ModifierKeys.Shift) == ModifierKeys.Shift)
            {
                e.Handled = true;
            }
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
                    case Key.Left:
                    case Key.Right:
                    case Key.End:
                    case Key.Tab:
                    case Key.Home:
                    case Key.Next:
                    case Key.Delete:
                    case Key.Back:
                    case Key.Divide:
                    case Key.OemQuestion:

                        break;

                    default:
                        e.Handled = true;
                        break;
                }
            }
        }

        /// <summary>
        /// Handles the KeyDown event of the NumeralsOnly TextBox control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="System.Windows.Input.KeyEventArgs"/> instance containing the event data.</param>
        private void NumeralsOnly_KeyDown(object sender, System.Windows.Input.KeyEventArgs e)
        {
            stkp_Error.Visibility = Visibility.Hidden;
            if ((Keyboard.Modifiers & ModifierKeys.Shift) == ModifierKeys.Shift)
            {
                e.Handled = true;
            }
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
                    case Key.Left:
                    case Key.Right:
                    case Key.End:
                    case Key.Tab:
                    case Key.Home:
                    case Key.Next:
                    case Key.Delete:
                    case Key.Back:
                        break;

                    default:
                        e.Handled = true;
                        break;
                }
            }
        }

        /// <summary>
        /// Previews the key up.
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="e">The <see cref="System.Windows.Input.KeyEventArgs"/> instance containing the event data.</param>
        private void PreviewKeyUp(object sender, System.Windows.Input.KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
                btnOk_Click(null, null);
        }

        /// <summary>
        /// Handles the Click event of the btnCancel control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="System.Windows.RoutedEventArgs"/> instance containing the event data.</param>
        private void btnCancel_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }

        /// <summary>
        /// Handles the Click event of the btnExit control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="System.Windows.RoutedEventArgs"/> instance containing the event data.</param>
        private void btnExit_Click(object sender, RoutedEventArgs e)
        {
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
        /// Handles the MouseRightButtonUp event of the txtSBSB_ID control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="System.Windows.Input.MouseButtonEventArgs"/> instance containing the event data.</param>
        private void txtSBSB_ID_MouseRightButtonUp(object sender, MouseButtonEventArgs e)
        {
            e.Handled = true;
        }

        /// <summary>
        /// Handles the Click event of the btnBackward control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="RoutedEventArgs"/> instance containing the event data.</param>
        private void btnBackward_Click(object sender, RoutedEventArgs e)
        {
            //txtContent.Text = "";
            stkp_Error.Visibility = Visibility.Hidden;
            lblInformation.Content = "";
            if (_dataContext.MemberId.Count < 1) return;

            if (_dataContext.CurrentMemberId - 1 < 0) return;

            if (!_dataContext.MemberId.ContainsKey(_dataContext.CurrentMemberId - 1)) return;
            string memberId = _dataContext.MemberId[_dataContext.CurrentMemberId - 1].ToString();
            if (memberId == null) return;
            else
            {
                txtMEMBER_ID.Text = memberId;
                _dataContext.CurrentMemberId--;
                txtContent.Text = (_dataContext.CurrentMemberId).ToString() + " of " + _dataContext.MemberId.Count.ToString();
                btnForward.IsEnabled = true;
                if (_dataContext.CurrentMemberId == 0)
                    btnBackward.IsEnabled = false;
                LoadCallData();
            }
        }

        /// <summary>
        /// Handles the Click event of the btnForward control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="RoutedEventArgs"/> instance containing the event data.</param>
        private void btnForward_Click(object sender, RoutedEventArgs e)
        {
            //txtContent.Text = "";
            stkp_Error.Visibility = Visibility.Hidden;
            lblInformation.Content = "";
            if (_dataContext.MemberId.Count < 1) return;

            if ((_dataContext.CurrentMemberId > _dataContext.MemberId.Count)) return;

            if (!_dataContext.MemberId.ContainsKey(_dataContext.CurrentMemberId + 1)) return;
            string memberId = _dataContext.MemberId[_dataContext.CurrentMemberId + 1].ToString();
            if (memberId == null) return;
            else
            {
                txtMEMBER_ID.Text = memberId;
                _dataContext.CurrentMemberId++;
                txtContent.Text = (_dataContext.CurrentMemberId).ToString() + " of " + _dataContext.MemberId.Count.ToString();
                btnBackward.IsEnabled = true;
                if (_dataContext.CurrentMemberId == _dataContext.MemberId.Count)
                    btnForward.IsEnabled = false;
                LoadCallData();
            }
        }

        /// <summary>
        /// Handles the Click event of the btnUpdate control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="RoutedEventArgs"/> instance containing the event data.</param>
        private void btnUpdate_Click(object sender, RoutedEventArgs e)
        {
            txtCSCI_FIRST_NAME.Text = "";
            txtCSCI_LAST_NAME.Text = "";
            txtCSCI_MID_INIT.Text = "";
            txtCSTK_MCTR_CATG.Text = "";
            txtCSTK_SUMMARY.Text = "";
            txtGRGR_ID.Text = "";
            txtMEMBER_DOB.Text = "";
            txtMEMBER_ID.Text = "";
            txtMEMBER_INFO.Text = "";
            txtMemZIP.Text = "";
            txtPRPR_ID.Text = "";
            txtTarget_VDN.Text = "";
            txtContent.Text = "";
            _dataContext.CurrentMemberId = 0;
            btnBackward.IsEnabled = true;
            btnForward.IsEnabled = true;
            btnOk.IsEnabled = true;
        }

        /// <summary>
        /// Handles the PreviewKeyDown event of the txtMEMBER_ID control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="KeyEventArgs"/> instance containing the event data.</param>
        private void txtMEMBER_ID_PreviewKeyDown(object sender, KeyEventArgs e)
        {
            txtContent.Text = "";
            stkp_Error.Visibility = Visibility.Hidden;
            lblInformation.Content = "";
        }

        #endregion Control Events

        #region Method Definition

        /// <summary>
        /// Loads the call data.
        /// </summary>
        public void LoadCallData()
        {
            try
            {
                callData.Clear();
                string[] tempValues;
                foreach (string key in _dataContext.userAttachData.Keys)
                    callData.Add(key, _dataContext.userAttachData[key]); 
                string memberValue = "";
                if (callData.Count > 0)
                {
                    foreach (string key in callData.Keys)
                    {
                        if (_dataContext.configuredAttachData.ContainsValue(key))
                        {
                            string tag = _dataContext.configuredAttachData.Where(kvp => kvp.Value == key).Select(kvp => kvp.Key).FirstOrDefault();
                            if (tag == "SBSB_ID" || tag == "MEME_SFX")
                            {
                                txtSBSB_ID.Text = "";
                                txtMEME_SFX.Text = "";
                                tempValues = null;
                                string callValue = "";
                                tempValues = callData[key].Split('|');
                                if (tempValues != null && tempValues.Length >= 1 && _dataContext.CurrentMemberId - 1 < tempValues.Length &&
                                        _dataContext.CurrentMemberId <= _dataContext.MemberId.Count)
                                {
                                    callValue = tempValues[_dataContext.CurrentMemberId - 1];
                                }
                                if (_dataContext.configuredAttachData.ContainsKey("SBSB_ID") && callValue != "")
                                {
                                    if (callValue.Length >= 11)
                                    {
                                        txtSBSB_ID.Text = callValue.Substring(0, 9);
                                    }
                                    else
                                    {
                                        txtSBSB_ID.Text = callValue;
                                    }
                                }
                                if (_dataContext.configuredAttachData.ContainsKey("MEME_SFX") && callValue != "")
                                {
                                    if (callValue.Length >= 11)
                                    {
                                        txtMEME_SFX.Text = callValue.Substring(9, 2);
                                    }
                                }
                            }
                            else if (tag == "MEMBER_INFO")
                            {
                                tempValues = null;
                                if (callData[key] != string.Empty)
                                {
                                    txtMEMBER_INFO.Text = "";
                                    memberValue = callData[key];
                                    tempValues = memberValue.Split('|');
                                    int currmemId = _dataContext.CurrentMemberId - 1;
                                    if (tempValues.Length >= 1 && _dataContext.CurrentMemberId - 1 < tempValues.Length &&
                                        _dataContext.CurrentMemberId <=  _dataContext.MemberId.Count)
                                        txtMEMBER_INFO.Text = tempValues[_dataContext.CurrentMemberId - 1];
                                }
                            }

                            else if (tag == "CSCI_FIRST_NAME" || tag == "CSCI_MID_INIT" || tag == "CSCI_LAST_NAME")
                            {
                                tempValues = null;
                                if (callData[key] != string.Empty)
                                {
                                    string nameValue = "";
                                    memberValue = callData[key];
                                    tempValues = memberValue.Split('|');
                                    if (tempValues.Length >= 1 && _dataContext.CurrentMemberId - 1 < tempValues.Length &&
                                        _dataContext.CurrentMemberId <= _dataContext.MemberId.Count)
                                        nameValue = tempValues[_dataContext.CurrentMemberId - 1];
                                    txtCSCI_FIRST_NAME.Text = "";
                                    txtCSCI_MID_INIT.Text = "";
                                    txtCSCI_LAST_NAME.Text = "";
                                    if (nameValue != "")
                                    {
                                        string[] name = nameValue.Split(' ');
                                        if (name.Count() > 2)
                                        {
                                            txtCSCI_FIRST_NAME.Text = name[0];
                                            txtCSCI_MID_INIT.Text = name[2];
                                            txtCSCI_LAST_NAME.Text = name[1];
                                        }
                                        else if (name.Count() > 1)
                                        {
                                            txtCSCI_FIRST_NAME.Text = name[0];
                                            txtCSCI_LAST_NAME.Text = name[1];
                                        }
                                        else
                                            txtCSCI_FIRST_NAME.Text = callData[key].ToString();
                                    }
                                }
                            }

                            else if (tag == "MEMBER_DOB")
                            {
                                DateTime result = new DateTime();
                                tempValues = null;
                                string dob = "";
                                txtMEMBER_DOB.Text = "";
                                memberValue = callData[key];
                                if (memberValue != string.Empty && memberValue != "")
                                {
                                    try
                                    {
                                        int value;
                                        tempValues = memberValue.Split('|');
                                        if (tempValues.Length >= 1 && _dataContext.CurrentMemberId - 1 < tempValues.Length &&
                                            _dataContext.CurrentMemberId <= _dataContext.MemberId.Count)
                                            dob = tempValues[_dataContext.CurrentMemberId - 1];
                                        if (int.TryParse(dob, out value) && dob.Length == 8)
                                        {
                                            CultureInfo provider = CultureInfo.InvariantCulture;
                                            string format = "MMddyyyy";
                                            result = DateTime.ParseExact(dob, format, provider);
                                            if (result != null)
                                                txtMEMBER_DOB.Text = result.Date.ToString("MM/dd/yyyy");
                                        }
                                        else
                                            txtMEMBER_DOB.Text = dob;
                                    }
                                    catch (Exception ex)
                                    {
                                        try
                                        {
                                            CultureInfo provider = CultureInfo.InvariantCulture;
                                            string format = "ddMMyyyy";
                                            result = DateTime.ParseExact(dob, format, provider);
                                            if (result != null)
                                                txtMEMBER_DOB.Text = result.Date.ToString("MM/dd/yyyy");
                                        }
                                        catch (Exception ex1)
                                        {
                                            _logger.Error("CallInfo:LoadCallData():" + ex.ToString());
                                            txtMEMBER_DOB.Text = dob;
                                        }
                                    }
                                }
                            }
                            else if (tag == "MemZIP")
                            {
                                tempValues = null;
                                txtMemZIP.Text = "";
                                if (callData[key] != string.Empty)
                                {
                                    memberValue = callData[key];
                                    tempValues = memberValue.Split('|');
                                    if (tempValues.Length >= 1 && _dataContext.CurrentMemberId - 1 < tempValues.Length &&
                                        _dataContext.CurrentMemberId <= _dataContext.MemberId.Count)   
                                        txtMemZIP.Text = tempValues[_dataContext.CurrentMemberId - 1];
                                }
                            }
                            else if (tag == "GRGR_ID")
                            {
                                tempValues = null;
                                txtGRGR_ID.Text = "";
                                if (callData[key] != string.Empty)
                                {
                                    memberValue = callData[key];
                                    tempValues = memberValue.Split('|');
                                    if (tempValues.Length >= 1 && _dataContext.CurrentMemberId-1 < tempValues.Length &&
                                        _dataContext.CurrentMemberId <= _dataContext.MemberId.Count)   
                                        txtGRGR_ID.Text = tempValues[_dataContext.CurrentMemberId - 1];
                                }
                            }

                            else if (tag == "CSTK_MCTR_CATG")
                            {
                                tempValues = null;
                                txtCSTK_MCTR_CATG.Text = "";
                                if (callData[key] != string.Empty)
                                {
                                    memberValue = callData[key];
                                    tempValues = memberValue.Split('|');
                                    if (tempValues.Length >= 1 && _dataContext.CurrentMemberId - 1 < tempValues.Length &&
                                        _dataContext.CurrentMemberId <= _dataContext.MemberId.Count)   
                                        txtCSTK_MCTR_CATG.Text = tempValues[_dataContext.CurrentMemberId - 1];
                                }
                            }
                            else if (tag == "CSTK_SUMMARY")
                            {
                                txtCSTK_SUMMARY.Text = callData[key]; 
                                tempValues = null;
                                txtCSTK_SUMMARY.Text = "";
                                if (callData[key] != string.Empty)
                                {
                                    memberValue = callData[key];
                                    tempValues = memberValue.Split('|');
                                    if (tempValues.Length >= 1 && _dataContext.CurrentMemberId - 1 < tempValues.Length &&
                                        _dataContext.CurrentMemberId <= _dataContext.MemberId.Count)
                                        txtCSTK_SUMMARY.Text = tempValues[_dataContext.CurrentMemberId - 1];
                                }
                            }

                            else if (tag == "PRPR_ID")
                            {
                                tempValues = null;
                                txtPRPR_ID.Text = "";
                                txtPRPR_ID.Text = callData[key];
                            }

                            else
                            {
                                TextBox txtBox = (TextBox)DGCalldata.FindName("txt" + _dataContext.configuredAttachData.Where(kvp => kvp.Value == key).Select(kvp => kvp.Key).FirstOrDefault());
                                if (txtBox != null)
                                {
                                    if(txtBox.Name!="txtMEMBER_ID")
                                        txtBox.Text = callData[key];
                                }
                            }
                        }
                    }
                }
                //Code added by Manikandan on 25/06/2014 for new changes from client
                if (txtCSTK_CUST_IND.Text != "")
                {
                    if (txtCSTK_CUST_IND.Text.ToLower() == "m")
                        txtPRPR_ID.Text = "";
                }
                //End
            }
            catch (Exception generalException)
            {
                _logger.Error("CallInfo:LoadCallData():" + generalException.ToString());
            }
        }

        /// <summary>
        /// Loads the member unique identifier.
        /// </summary>
        public void LoadMemberID()
        {
            try
            {
                _dataContext.MemberId.Clear();

                if (_dataContext.userAttachData.ContainsKey("ConnectionId"))
                    txtConnID.Text = _dataContext.userAttachData["ConnectionId"].ToString();
                string userDataValue = "";
                if (_dataContext.configuredAttachData.ContainsKey("MEMBER_ID"))
                    userDataValue = _dataContext.configuredAttachData["MEMBER_ID"];
                if (userDataValue == null || userDataValue == string.Empty) return;
                if (!_dataContext.userAttachData.ContainsKey(userDataValue)) return;
                string memberIds = _dataContext.userAttachData[userDataValue];
                string[] keys = memberIds.Split('|');
                for (int i = 1; i <= keys.Length; i++)
                    _dataContext.MemberId.Add(i, keys[i - 1]);
                if (_dataContext.MemberId.Count >= 1 && _dataContext.CurrentMemberId == 0)
                {
                    txtMEMBER_ID.Text = _dataContext.MemberId[1];
                    _dataContext.CurrentMemberId = 1;
                    btnBackward.IsEnabled = false;
                    txtContent.Text = "1 of " + _dataContext.MemberId.Count.ToString();
                    btnBackward.IsEnabled = true;
                    btnForward.IsEnabled = true;
                }
                else if (_dataContext.CurrentMemberId != 0 && _dataContext.CurrentMemberId <= _dataContext.MemberId.Count)
                {
                    txtMEMBER_ID.Text = _dataContext.MemberId[_dataContext.CurrentMemberId];
                    txtContent.Text = _dataContext.CurrentMemberId + " of " + _dataContext.MemberId.Count.ToString();
                    btnBackward.IsEnabled = true;
                    btnForward.IsEnabled = true;
                }
                else
                {
                    btnBackward.IsEnabled = false;
                    btnForward.IsEnabled = false;
                }
            }
            catch (Exception ex)
            {
                _logger.Error("LoadMemberID() " + ex.Message.ToString());
                string s = ex.Message;
            }
        }

        /// <summary>
        /// Updates the user data.
        /// </summary>
        private void UpdateUserData()
        {
            try
            {
                SoftPhone updateCallData = new SoftPhone();
                string[] tempValues=null;
                Dictionary<string, string> childKeys = new Dictionary<string, string>();
                childKeys.Clear();
                foreach (TextBox tb in FindVisualChildren<TextBox>(DGCalldata))
                {
                    TextBox textBox = (TextBox)tb;
                    if (textBox.Text != null)//&& textBox.Text != string.Empty)
                    {
                        if (!childKeys.ContainsKey(textBox.Name.Replace("txt", string.Empty)))
                            childKeys.Add(textBox.Name.Replace("txt", string.Empty), textBox.Text);
                    }
                }


                foreach (string key in childKeys.Keys)
                {
                    if (_dataContext.configuredAttachData.ContainsKey(key) && (_dataContext.configuredAttachData[key] != string.Empty))
                    {
                        //if (key == "SBSB_ID" )//|| key == "MEME_SFX")
                        //{
                        //    string MemIDTranslated = "";
                        //    if (_dataContext.configuredAttachData.ContainsKey("SBSB_ID"))
                        //        MemIDTranslated = txtSBSB_ID.Text;
                        //    if (_dataContext.configuredAttachData.ContainsKey("MEME_SFX"))
                        //        MemIDTranslated += txtMEME_SFX.Text;
                        //    if (_dataContext.userAttachData.ContainsKey(_dataContext.configuredAttachData[key]))
                        //    {
                        //        string value = "";
                        //        string originalValue = _dataContext.userAttachData[_dataContext.configuredAttachData[key]];
                        //        if(originalValue!="")
                        //            value="|" + MemIDTranslated;
                        //        else
                        //            value=MemIDTranslated;
                        //        if (value != originalValue)
                        //        {
                        //            originalValue += value;
                        //            _dataContext.userAttachData.Remove(_dataContext.configuredAttachData[key]);
                        //            _dataContext.userAttachData.Add(_dataContext.configuredAttachData[key], originalValue);
                        //            updateCallData.UpdateUserData(_dataContext.userAttachData);
                        //        }
                        //    }
                        //    else
                        //    {
                        //        _dataContext.userAttachData.Add(_dataContext.configuredAttachData[key], "|" + MemIDTranslated);
                        //        updateCallData.UpdateUserData(_dataContext.userAttachData);
                        //    }
                        //}

                        if (key=="CSCI_FIRST_NAME")// || key.Contains("MID_INIT"))
                        {
                            string MemName = txtCSCI_FIRST_NAME.Text + " " + txtCSCI_LAST_NAME.Text + " " + txtCSCI_MID_INIT.Text;
                            if (_dataContext.userAttachData.ContainsKey(_dataContext.configuredAttachData[key]))
                            {
                                string value = "";
                                string originalValue = _dataContext.userAttachData[_dataContext.configuredAttachData[key]];
                                tempValues = null;
                                tempValues = originalValue.Split('|');
                                //if (originalValue == "")
                                //    value = MemName;
                                //else
                                {
                                    if (_dataContext.CurrentMemberId == 0)
                                    {
                                        string tempvalue = "";
                                        foreach (KeyValuePair<int, string> val in _dataContext.MemberId)
                                        {
                                            int memberidKey = val.Key;
                                            if ((memberidKey - 1) < tempValues.Length)
                                                tempvalue += tempValues[memberidKey - 1] + "|";
                                            else
                                                tempvalue += "|";
                                        }
                                        //foreach (var item in tempValues)
                                        //    tempvalue += item + "|";
                                        value = tempvalue + MemName;
                                    }
                                    else if (tempValues.Length >= 1 && _dataContext.CurrentMemberId - 1 < tempValues.Length &&
                                        _dataContext.CurrentMemberId <= tempValues.Length)   
                                    {
                                        var i = tempValues[_dataContext.CurrentMemberId - 1];
                                        if (i != MemName)
                                            tempValues[_dataContext.CurrentMemberId - 1] = MemName;
                                        value = "";
                                        foreach (KeyValuePair<int, string> val in _dataContext.MemberId)
                                        {
                                            int memberidKey = val.Key;
                                            if ((memberidKey - 1) < tempValues.Length)
                                                value += tempValues[memberidKey - 1] + "|";
                                            else
                                                value += "|";
                                        }
                                        //foreach (var item in tempValues)
                                        //    value += item + "|";
                                        value = value.Substring(0, value.Length - 1);
                                    }
                                }
                                if (value != originalValue)
                                {
                                    //originalValue += value;
                                    _dataContext.userAttachData.Remove(_dataContext.configuredAttachData[key]);
                                    _dataContext.userAttachData.Add(_dataContext.configuredAttachData[key], value);
                                    updateCallData.UpdateUserData(_dataContext.userAttachData);
                                }
                            }
                            else
                            {
                                _dataContext.userAttachData.Add(_dataContext.configuredAttachData[key], MemName);
                                updateCallData.UpdateUserData(_dataContext.userAttachData);
                            }
                        }

                        else if (key == "MEMBER_DOB")
                        {
                            DateTime dob = new DateTime();
                            if (txtMEMBER_DOB.Text != string.Empty)
                            {
                                dob = Convert.ToDateTime(txtMEMBER_DOB.Text);
                            }

                            if (_dataContext.userAttachData.ContainsKey(_dataContext.configuredAttachData[key]))
                            {
                                string value = "";
                                string originalValue = _dataContext.userAttachData[_dataContext.configuredAttachData[key]];
                                if (dob != null && txtMEMBER_DOB.Text != string.Empty && originalValue!="")
                                    value +=dob.ToString("MM") + dob.ToString("dd") + dob.Year.ToString();
                                else if (dob != null && txtMEMBER_DOB.Text != string.Empty)
                                    value += dob.ToString("MM") + dob.ToString("dd") + dob.Year.ToString();


                                tempValues = null;
                                tempValues = originalValue.Split('|');
                                //if (originalValue != "")
                                //{
                                    if (_dataContext.CurrentMemberId == 0)
                                    {
                                        string tempvalue = "";
                                        foreach (KeyValuePair<int, string> val in _dataContext.MemberId)
                                        {
                                            int memberidKey = val.Key;
                                            if ((memberidKey - 1) < tempValues.Length)
                                                tempvalue += tempValues[memberidKey - 1] + "|";
                                            else
                                                tempvalue += "|";
                                        }
                                        //foreach (var item in tempValues)
                                        //    tempvalue += item + "|";
                                        value = tempvalue + value;
                                    }
                                    else if (tempValues.Length >= 1 && _dataContext.CurrentMemberId - 1 < tempValues.Length &&
                                        _dataContext.CurrentMemberId <= tempValues.Length)
                                    {
                                        var i = tempValues[_dataContext.CurrentMemberId - 1];
                                        if (i != value)
                                            tempValues[_dataContext.CurrentMemberId - 1] = value;
                                        value = "";
                                        foreach (KeyValuePair<int, string> val in _dataContext.MemberId)
                                        {
                                            int memberidKey = val.Key;
                                            if ((memberidKey - 1) < tempValues.Length)
                                                value += tempValues[memberidKey - 1] + "|";
                                            else
                                                value += "|";
                                        }
                                        //foreach (var item in tempValues)
                                        //    value += item + "|";
                                        value = value.Substring(0, value.Length - 1);
                                    }
                                //}

                                if (value != originalValue && value!=string.Empty)
                                {
                                    _dataContext.userAttachData.Remove(_dataContext.configuredAttachData[key]);
                                    _dataContext.userAttachData.Add(_dataContext.configuredAttachData[key], value);
                                    updateCallData.UpdateUserData(_dataContext.userAttachData);
                                }
                            }
                            else
                            {
                                string value = "|";
                                if (dob != null && txtMEMBER_DOB.Text != string.Empty)
                                {
                                    value += dob.Month.ToString() + dob.Day.ToString() + dob.Year.ToString();
                                    _dataContext.userAttachData.Add(_dataContext.configuredAttachData[key], value);
                                    updateCallData.UpdateUserData(_dataContext.userAttachData);
                                }
                            }
                        }
                        else if (key == "MEMBER_ID")
                        {
                            string memberId = txtMEMBER_ID.Text;
                            if (_dataContext.userAttachData.ContainsKey(_dataContext.configuredAttachData[key]))
                            {
                                string value = "";
                                string originalValue = _dataContext.userAttachData[_dataContext.configuredAttachData[key]];
                                tempValues = null;
                                tempValues = originalValue.Split('|');
                                //if(originalValue=="")
                                //    value= memberId;
                                //else
                                {
                                    if (_dataContext.CurrentMemberId == 0)
                                    {
                                        string tempvalue = "";
                                        foreach (KeyValuePair<int, string> val in _dataContext.MemberId)
                                        {
                                            int memberidKey = val.Key;
                                            if ((memberidKey - 1) < tempValues.Length)
                                                tempvalue += tempValues[memberidKey - 1] + "|";
                                            else
                                                tempvalue += "|";
                                        }
                                        //foreach (var item in tempValues)
                                        //    tempvalue += item + "|";
                                        value = tempvalue + memberId;
                                    }
                                    else if (tempValues.Length >= 1 && _dataContext.CurrentMemberId - 1 < tempValues.Length &&
                                        _dataContext.CurrentMemberId <= tempValues.Length)
                                    {
                                        var i = tempValues[_dataContext.CurrentMemberId - 1];
                                        if (i != memberId)
                                            tempValues[_dataContext.CurrentMemberId - 1] = memberId;
                                        value = "";
                                        foreach (KeyValuePair<int, string> val in _dataContext.MemberId)
                                        {
                                            int memberidKey = val.Key;
                                            if ((memberidKey - 1) < tempValues.Length)
                                                value += tempValues[memberidKey - 1] + "|";
                                            else
                                                value += "|";
                                        }
                                        //foreach (var item in tempValues)
                                        //    value += item + "|";
                                        value = value.Substring(0, value.Length-1);
                                    }
                                }

                                if (value != originalValue)
                                {
                                    //originalValue += value;
                                    _dataContext.userAttachData.Remove(_dataContext.configuredAttachData[key]);
                                    _dataContext.userAttachData.Add(_dataContext.configuredAttachData[key], value);
                                    updateCallData.UpdateUserData(_dataContext.userAttachData);
                                }
                            }
                            else
                            {
                                _dataContext.userAttachData.Add(_dataContext.configuredAttachData[key], memberId);
                                updateCallData.UpdateUserData(_dataContext.userAttachData);
                            }
                        }
                        else if (key == "MEMBER_INFO")
                        {
                            string memValidated = txtMEMBER_INFO.Text;
                            if (_dataContext.userAttachData.ContainsKey(_dataContext.configuredAttachData[key]))
                            {
                                string value = "";
                                string originalValue = _dataContext.userAttachData[_dataContext.configuredAttachData[key]];

                                tempValues = null;
                                tempValues = originalValue.Split('|');
                                //if (originalValue == "")
                                //    value = memValidated;
                                //else
                                {
                                    if (_dataContext.CurrentMemberId == 0)
                                    {
                                        string tempvalue = "";
                                        foreach (KeyValuePair<int, string> val in _dataContext.MemberId)
                                        {
                                            int memberidKey = val.Key;
                                            if ((memberidKey - 1) < tempValues.Length)
                                                tempvalue += tempValues[memberidKey - 1] + "|";
                                            else
                                                tempvalue += "|";
                                        }
                                        //foreach (var item in tempValues)
                                        //    tempvalue += item + "|";
                                        value = tempvalue + memValidated;
                                    }
                                    else if (tempValues.Length >= 1 && _dataContext.CurrentMemberId - 1 < tempValues.Length &&
                                        _dataContext.CurrentMemberId <= tempValues.Length)
                                    {
                                        var i = tempValues[_dataContext.CurrentMemberId - 1];
                                        if (i != memValidated)
                                            tempValues[_dataContext.CurrentMemberId - 1] = memValidated;
                                        value = "";
                                        foreach (KeyValuePair<int, string> val in _dataContext.MemberId)
                                        {
                                            int memberidKey = val.Key;
                                            if ((memberidKey - 1) < tempValues.Length)
                                                value += tempValues[memberidKey - 1] + "|";
                                            else
                                                value += "|";
                                        }
                                        //foreach (var item in tempValues)
                                        //    value += item + "|";
                                        value = value.Substring(0, value.Length - 1);
                                    }
                                }
                                if (value != originalValue)
                                {
                                    //originalValue += value;
                                    _dataContext.userAttachData.Remove(_dataContext.configuredAttachData[key]);
                                    _dataContext.userAttachData.Add(_dataContext.configuredAttachData[key], value);
                                    updateCallData.UpdateUserData(_dataContext.userAttachData);
                                }
                            }
                            else
                            {
                                _dataContext.userAttachData.Add(_dataContext.configuredAttachData[key], memValidated);
                                updateCallData.UpdateUserData(_dataContext.userAttachData);
                            }
                        }
                        else if (key == "GRGR_ID")
                        {
                            string groupId = txtGRGR_ID.Text;
                            if (_dataContext.userAttachData.ContainsKey(_dataContext.configuredAttachData[key]))
                            {
                                string value ="";
                                string originalValue = _dataContext.userAttachData[_dataContext.configuredAttachData[key]];

                                tempValues = null;
                                tempValues = originalValue.Split('|');
                                //if (originalValue == "")
                                //    value = groupId;
                                //else
                                {
                                    if (_dataContext.CurrentMemberId == 0)
                                    {
                                        string tempvalue = "";
                                        foreach (KeyValuePair<int, string> val in _dataContext.MemberId)
                                        {
                                            int memberidKey = val.Key;
                                            if ((memberidKey - 1) < tempValues.Length)
                                                tempvalue += tempValues[memberidKey - 1] + "|";
                                            else
                                                tempvalue += "|";
                                        }
                                        //foreach (var item in tempValues)
                                        //    tempvalue += item + "|";
                                        value = tempvalue + groupId;
                                    }
                                    else if (tempValues.Length >= 1 && _dataContext.CurrentMemberId - 1 < tempValues.Length &&
                                        _dataContext.CurrentMemberId <= tempValues.Length)
                                    {
                                        var i = tempValues[_dataContext.CurrentMemberId - 1];
                                        if (i != groupId)
                                            tempValues[_dataContext.CurrentMemberId - 1] = groupId;
                                        value = "";
                                        foreach (KeyValuePair<int, string> val in _dataContext.MemberId)
                                        {
                                            int memberidKey = val.Key;
                                            if ((memberidKey - 1) < tempValues.Length)
                                                value += tempValues[memberidKey - 1] + "|";
                                            else
                                                value += "|";
                                        }
                                        //foreach (var item in tempValues)
                                        //    value += item + "|";
                                        value = value.Substring(0, value.Length - 1);
                                    }
                                }
                                if (value != originalValue)
                                {
                                    //originalValue += value;
                                    _dataContext.userAttachData.Remove(_dataContext.configuredAttachData[key]);
                                    _dataContext.userAttachData.Add(_dataContext.configuredAttachData[key], value);
                                    updateCallData.UpdateUserData(_dataContext.userAttachData);
                                }
                            }
                            else
                            {
                                _dataContext.userAttachData.Add(_dataContext.configuredAttachData[key], groupId);
                                updateCallData.UpdateUserData(_dataContext.userAttachData);
                            }
                        }
                        else if (key == "MemZIP")
                        {
                            string memZip = txtMemZIP.Text;
                            if (_dataContext.userAttachData.ContainsKey(_dataContext.configuredAttachData[key]))
                            {
                                string value ="";
                                string originalValue = _dataContext.userAttachData[_dataContext.configuredAttachData[key]];
                                tempValues = null;
                                tempValues = originalValue.Split('|');
                                //if (originalValue == "")
                                //    value = memZip;
                                //else
                                {
                                    if (_dataContext.CurrentMemberId == 0)
                                    {
                                        string tempvalue = "";

                                        foreach(KeyValuePair<int,string> val in _dataContext.MemberId)
                                        {
                                            int memberidKey = val.Key;
                                            if ((memberidKey - 1) < tempValues.Length)
                                                tempvalue += tempValues[memberidKey - 1] + "|";
                                            else
                                                tempvalue += "|";
                                        }

                                        //foreach (var item in tempValues)
                                        //    tempvalue += item + "|";
                                        value = tempvalue + memZip;
                                    }
                                    else if (tempValues.Length >= 1 && _dataContext.CurrentMemberId - 1 < tempValues.Length &&
                                        _dataContext.CurrentMemberId <= tempValues.Length)
                                    {
                                        var i = tempValues[_dataContext.CurrentMemberId - 1];
                                        if (i != memZip)
                                            tempValues[_dataContext.CurrentMemberId - 1] = memZip;
                                        value = "";

                                        foreach (KeyValuePair<int, string> val in _dataContext.MemberId)
                                        {
                                            int memberidKey = val.Key;
                                            if ((memberidKey - 1) < tempValues.Length)
                                                value += tempValues[memberidKey - 1] + "|";
                                            else
                                                value += "|";
                                        }

                                        //foreach (var item in tempValues)
                                        //    value += item + "|";
                                        value = value.Substring(0, value.Length - 1);
                                    }
                                }
                                if (value != originalValue)
                                {
                                    //originalValue += value;
                                    _dataContext.userAttachData.Remove(_dataContext.configuredAttachData[key]);
                                    _dataContext.userAttachData.Add(_dataContext.configuredAttachData[key], value);
                                    updateCallData.UpdateUserData(_dataContext.userAttachData);
                                }
                            }
                            else
                            {
                                _dataContext.userAttachData.Add(_dataContext.configuredAttachData[key], memZip);
                                updateCallData.UpdateUserData(_dataContext.userAttachData);
                            }
                        }
                        else if (key == "CSTK_MCTR_CATG")
                        {
                            string callReason = txtCSTK_MCTR_CATG.Text;
                            if (_dataContext.userAttachData.ContainsKey(_dataContext.configuredAttachData[key]))
                            {
                                string value = "";
                                string originalValue = _dataContext.userAttachData[_dataContext.configuredAttachData[key]];
                                tempValues = null;
                                tempValues = originalValue.Split('|');
                                //if (originalValue == "")
                                //    value = callReason;
                                //else
                                {
                                    if (_dataContext.CurrentMemberId == 0)
                                    {
                                        string tempvalue = "";
                                        foreach (KeyValuePair<int, string> val in _dataContext.MemberId)
                                        {
                                            int memberidKey = val.Key;
                                            if ((memberidKey - 1) < tempValues.Length)
                                                tempvalue += tempValues[memberidKey - 1] + "|";
                                            else
                                                tempvalue += "|";
                                        }
                                        //foreach (var item in tempValues)
                                        //    tempvalue += item + "|";
                                        value = tempvalue + callReason;
                                    }
                                    else //if (tempValues.Length >= 1 && _dataContext.CurrentMemberId <= tempValues.Length)
                                    {
                                        var i = tempValues[_dataContext.CurrentMemberId - 1];
                                        if (i != callReason)
                                            tempValues[_dataContext.CurrentMemberId - 1] = callReason;
                                        value = "";
                                        foreach (KeyValuePair<int, string> val in _dataContext.MemberId)
                                        {
                                            int memberidKey = val.Key;
                                            if ((memberidKey - 1) < tempValues.Length)
                                                value += tempValues[memberidKey - 1] + "|";
                                            else
                                                value += "|";
                                        }
                                        //foreach (var item in tempValues)
                                        //    value += item + "|";
                                        value = value.Substring(0, value.Length - 1);
                                    }
                                }
                                if (value != originalValue)
                                {
                                    //originalValue += value;
                                    _dataContext.userAttachData.Remove(_dataContext.configuredAttachData[key]);
                                    _dataContext.userAttachData.Add(_dataContext.configuredAttachData[key], value);
                                    updateCallData.UpdateUserData(_dataContext.userAttachData);
                                }
                            }
                            else
                            {
                                _dataContext.userAttachData.Add(_dataContext.configuredAttachData[key], callReason);
                                updateCallData.UpdateUserData(_dataContext.userAttachData);
                            }
                        }
                        else if (key == "CSTK_SUMMARY")
                        {
                            string benDisc = txtCSTK_SUMMARY.Text;
                            if (_dataContext.userAttachData.ContainsKey(_dataContext.configuredAttachData[key]))
                            {
                                string value = "";
                                string originalValue = _dataContext.userAttachData[_dataContext.configuredAttachData[key]];
                                tempValues = null;
                                tempValues = originalValue.Split('|');
                                //if (originalValue == "")
                                //    value = benDisc;
                                //else
                                {
                                    if (_dataContext.CurrentMemberId == 0)
                                    {
                                        string tempvalue = "";
                                        foreach (KeyValuePair<int, string> val in _dataContext.MemberId)
                                        {
                                            int memberidKey = val.Key;
                                            if ((memberidKey - 1) < tempValues.Length)
                                                tempvalue += tempValues[memberidKey - 1] + "|";
                                            else
                                                tempvalue += "|";
                                        }
                                        //foreach (var item in tempValues)
                                        //    tempvalue += item + "|";
                                        value = tempvalue + benDisc;
                                    }
                                    else if (tempValues.Length >= 1 && _dataContext.CurrentMemberId -1 <= tempValues.Length)
                                    {
                                        var i = tempValues[_dataContext.CurrentMemberId - 1];
                                        if (i != benDisc)
                                            tempValues[_dataContext.CurrentMemberId - 1] = benDisc;
                                        value = "";

                                        foreach (KeyValuePair<int, string> val in _dataContext.MemberId)
                                        {
                                            int memberidKey = val.Key;
                                            if ((memberidKey - 1) < tempValues.Length)
                                                value += tempValues[memberidKey - 1] + "|";
                                            else
                                                value += "|";
                                        }
                                        //foreach (var item in tempValues)
                                        //    value += item + "|";
                                        value = value.Substring(0, value.Length - 1);
                                    }
                                    //else
                                    //{
                                    //    string tempvalue = "";
                                    //    foreach (var item in tempValues)
                                    //        tempvalue += item + "|";
                                    //    value = tempvalue + benDisc;
                                    //}
                                }
                                if (value != originalValue)
                                {
                                    //originalValue += value;
                                    _dataContext.userAttachData.Remove(_dataContext.configuredAttachData[key]);
                                    _dataContext.userAttachData.Add(_dataContext.configuredAttachData[key], value);
                                    updateCallData.UpdateUserData(_dataContext.userAttachData);
                                }
                            }
                            else
                            {
                                _dataContext.userAttachData.Add(_dataContext.configuredAttachData[key], benDisc);
                                updateCallData.UpdateUserData(_dataContext.userAttachData);
                            }
                        }
                            //Code added by Manikandan on 25the June for new change by client
                        else if (key == "PRPR_ID")
                        {
                            string value = txtPRPR_ID.Text;
                            string originalValue = "";
                            if (_dataContext.userAttachData.ContainsKey(_dataContext.configuredAttachData[key]))
                            {
                                originalValue = _dataContext.userAttachData[_dataContext.configuredAttachData[key]];
                                if (txtCSTK_CUST_IND.Text.ToLower()=="m")
                                {
                                    if (originalValue == value) continue;
                                    if (value == "") continue;
                                    _dataContext.userAttachData.Remove(_dataContext.configuredAttachData[key]);
                                    _dataContext.userAttachData.Add(_dataContext.configuredAttachData[key], value);
                                    updateCallData.UpdateUserData(_dataContext.userAttachData);
                                }
                                else if (value != originalValue && txtCSTK_CUST_IND.Text.ToLower() != "m")
                                {
                                    _dataContext.userAttachData.Remove(_dataContext.configuredAttachData[key]);
                                    _dataContext.userAttachData.Add(_dataContext.configuredAttachData[key], value);
                                    updateCallData.UpdateUserData(_dataContext.userAttachData);
                                }
                            }
                            else if (value != "")
                            {
                                _dataContext.userAttachData.Add(_dataContext.configuredAttachData[key], value);
                                updateCallData.UpdateUserData(_dataContext.userAttachData);
                            }
                        }
                            //End
                        else
                        {
                            //Check other Key values and add in User Data if configured in CME
                            if (key != "CSCI_LAST_NAME" && key != "CSCI_MID_INIT" && key != "MEME_SFX" && key != "SBSB_ID")
                            {
                                if (_dataContext.configuredAttachData.ContainsKey(key))
                                {
                                    string keyValue = _dataContext.configuredAttachData[key];
                                    if (_dataContext.userAttachData.ContainsKey(keyValue))
                                    {
                                        string value = childKeys[key];
                                        string originalValue = _dataContext.userAttachData[keyValue];
                                        if (value != originalValue)
                                        {
                                            _dataContext.userAttachData.Remove(keyValue);
                                            _dataContext.userAttachData.Add(keyValue, value);
                                            updateCallData.UpdateUserData(_dataContext.userAttachData);
                                        }
                                    }
                                    else if (childKeys[key] != string.Empty)
                                    {
                                        _dataContext.userAttachData.Add(keyValue, childKeys[key]);
                                        updateCallData.UpdateUserData(_dataContext.userAttachData);
                                    }
                                }
                            }
                        }
                    }
                }
            }
            catch (Exception generalException)
            {
                _logger.Error("CallInfo:UpdateUserData():" + generalException.ToString());
            }
        }

        /// <summary>
        /// Finds the visual children.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="depObj">The dep obj.</param>
        /// <returns></returns>
        public static IEnumerable<T> FindVisualChildren<T>(DependencyObject depObj) where T : DependencyObject
        {
            if (depObj != null)
            {
                for (int i = 0; i < VisualTreeHelper.GetChildrenCount(depObj); i++)
                {
                    DependencyObject child = VisualTreeHelper.GetChild(depObj, i);
                    if (child != null && child is T)
                    {
                        yield return (T)child;
                    }

                    foreach (T childOfChild in FindVisualChildren<T>(child))
                    {
                        yield return childOfChild;
                    }
                }
            }
        }

        /// <summary>
        /// Validates the date.
        /// </summary>
        /// <param name="stringDateValue">The string date value.</param>
        /// <returns></returns>
        private bool ValidateDate(string stringDateValue)
        {
            try
            {
                CultureInfo CultureInfoDateCulture = new CultureInfo("en-US");
                DateTime d = DateTime.ParseExact(stringDateValue, "MM/dd/yyyy", CultureInfoDateCulture);
                return true;
            }
            catch
            {
                return false;
            }
        }

        #endregion Method Definition

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

        #endregion System Menu

        private void txtCSCI_TITLE_TextChanged(object sender, TextChangedEventArgs e)
        {

        }
    }

    #region ComboBoxPairs

    /// <summary>
    /// ComboBoxPairs Class Instantiation
    /// class used to assign Text and Value for Combobox Control
    /// </summary>
    public class ComboBoxPairs
    {
        public int id { get; set; }

        public string value { get; set; }

        /// <summary>
        /// Initializes a new instance of the <see cref="ComboBoxPairs"/> class.
        /// </summary>
        /// <param name="Value">The value.</param>
        /// <param name="Id">The id.</param>
        public ComboBoxPairs(string Value,
                             int Id)
        {
            id = Id;
            value = Value;
        }
    }

    #endregion ComboBoxPairs
}