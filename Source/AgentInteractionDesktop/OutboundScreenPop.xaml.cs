namespace Agent.Interaction.Desktop
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using System.Windows;
    using System.Windows.Controls;
    using System.Windows.Data;
    using System.Windows.Documents;
    using System.Windows.Input;
    using System.Windows.Media;
    using System.Windows.Media.Effects;
    using System.Windows.Media.Imaging;
    using System.Windows.Shapes;

    using Agent.Interaction.Desktop.Settings;
    using Pointel.Configuration.Manager;
    using Agent.Interaction.Desktop.Helpers;
    using Genesyslab.Platform.Commons.Collections;
    using System.Globalization;
    using Pointel.Softphone.Voice.Core;
    using Pointel.Softphone.Voice.Common;
    using System.Windows.Interop;
    using System.Runtime.InteropServices;

    /// <summary>
    /// Interaction logic for OutboundScreenPop.xaml
    /// </summary>
    public partial class OutboundScreenPop : Window
    {
        #region Fields

        public DropShadowBitmapEffect ShadowEffect = new DropShadowBitmapEffect();
        private ConfigContainer _configContainer = ConfigContainer.Instance();
        Datacontext _datacontext = null;
        private Pointel.Logger.Core.ILog _logger = Pointel.Logger.Core.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType,
           "AID");
        KeyValueCollection _updateUserEventData = new KeyValueCollection();

        private const int GWL_STYLE = -16; //WPF's Message code for Title Bar's Style 
        private const int WS_SYSMENU = 0x80000; //WPF's Message code for System Menu
        [DllImport("user32.dll", SetLastError = true)]
        private static extern int GetWindowLong(IntPtr hWnd, int nIndex);
        [DllImport("user32.dll")]
        private static extern int SetWindowLong(IntPtr hWnd, int nIndex, int dwNewLong);

        #endregion Fields

        #region Constructors

        public OutboundScreenPop()
        {
            _datacontext = Datacontext.GetInstance();
            InitializeComponent();
            this.DataContext = _datacontext;
            ShadowEffect.ShadowDepth = 0;
            ShadowEffect.Opacity = 0.5;
            ShadowEffect.Softness = 0.5;
            ShadowEffect.Color = (Color)ColorConverter.ConvertFromString("#003660");
        }

        #endregion Constructors

        #region Methods

        private void Window_Activated(object sender, EventArgs e)
        {
            MainBorder.BorderBrush = new SolidColorBrush((Color)(ColorConverter.ConvertFromString("#0070C5")));
            MainBorder.BitmapEffect = ShadowEffect;
        }

        private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {

        }

        private void Window_Deactivated(object sender, EventArgs e)
        {
            MainBorder.BorderBrush = Brushes.Black;
            MainBorder.BitmapEffect = null;
        }

        #endregion Methods

        private void btnUpdate_Click(object sender, RoutedEventArgs e)
        {
            PrepareUserData();

            DataContext = null;
            this.Close();
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            try
            {
                var hwnd = new WindowInteropHelper(this).Handle;
                SetWindowLong(hwnd, GWL_STYLE, GetWindowLong(hwnd, GWL_STYLE) & ~WS_SYSMENU);
                chkbxPersonalCallback.IsEnabled = false;
                dpRescheduledate.IsEnabled = false;
                dpRescheduletime.IsEnabled = false;
                cmbCallResult.SelectedIndex = 0;
                _datacontext.CallResultItemSource.Clear();
                if (_configContainer.AllKeys.Contains("OutboundCallResult") && _configContainer.GetValue("OutboundCallResult") != null)
                    foreach (var item in ((Dictionary<string, string>)_configContainer.GetValue("OutboundCallResult")))
                        _datacontext.CallResultItemSource.Add(new OutboundCallResult(item.Key.ToString(), item.Value.ToString()));
            }
            catch (Exception generalException)
            {
                _logger.Error("Error occurred as Window_Loaded : " + generalException.ToString());
            }
        }

        private void cmbCallResult_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            var selectedCallResult = cmbCallResult.SelectedItem as OutboundCallResult;
            if (selectedCallResult != null)
            {

            }
        }

        private void Border_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
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

        private void chkbxRescheduleCall_Checked(object sender, RoutedEventArgs e)
        {
            chkbxPersonalCallback.IsEnabled = true;
            dpRescheduledate.IsEnabled = true;
            dpRescheduletime.IsEnabled = true;
        }

        private void chkbxRescheduleCall_Unchecked(object sender, RoutedEventArgs e)
        {
            chkbxPersonalCallback.IsEnabled = false;
            dpRescheduledate.IsEnabled = false;
            dpRescheduletime.IsEnabled = false;
        }

        /// <summary>
        /// Updates the title.
        /// </summary>
        private void UpdateTitle()
        {
            try
            {
                string temp = "";
                if (_datacontext.userAttachData.ContainsKey("PHSPCB") && _datacontext.userAttachData.ContainsKey("agent_id") && !string.IsNullOrEmpty(_datacontext.userAttachData["agent_id"].ToString()))
                {
                    if (_datacontext.userAttachData["PHSPCB"].ToString() == "1")
                    {
                        temp = "Personal CallBack with ";
                    }
                    else
                    {
                        if (_datacontext.userAttachData.ContainsKey("GSW_CAMPAIGN_NAME"))
                        {
                            temp = _datacontext.userAttachData["GSW_CAMPAIGN_NAME"].ToString() + " CallBack with ";
                        }
                        else
                        {
                            temp = "Campaign CallBack with ";
                        }


                    }
                }
                else
                {
                    if (_datacontext.userAttachData.ContainsKey("GSW_CAMPAIGN_NAME"))
                    {
                        temp = _datacontext.userAttachData["GSW_CAMPAIGN_NAME"].ToString() + " Call With  : ";
                    }
                    else
                    {
                        temp = "Outbound Call";
                    }
                }
                if (_datacontext.userAttachData.ContainsKey("GSW_PHONE"))
                {
                    temp += _datacontext.userAttachData["GSW_PHONE"].ToString();
                }
                this.Title += " - " + temp;
            }
            catch (Exception generalException)
            {
                _logger.Error("Error occurred in UpdateTitle() : " + generalException.ToString());
            }
        }

        /// <summary>
        /// Prepares the user data.
        /// </summary>
        private void PrepareUserData()
        {
            string finalDateTime = string.Empty;
            try
            {
                _updateUserEventData.Clear();
                if (_datacontext.userAttachData.ContainsKey("GSW_RECORD_HANDLE"))
                    if (!_updateUserEventData.ContainsKey("GSW_RECORD_HANDLE"))
                        _updateUserEventData.Add("GSW_RECORD_HANDLE", Convert.ToInt32(_datacontext.userAttachData["GSW_RECORD_HANDLE"]));
                    else
                        _logger.Debug("PrepareUserData : GSW_RECORD_HANDLE is null");

                if (_datacontext.userAttachData.ContainsKey("GSW_APPLICATION_ID"))
                    if (!_updateUserEventData.ContainsKey("GSW_APPLICATION_ID"))
                        _updateUserEventData.Add("GSW_APPLICATION_ID", Convert.ToInt32(_datacontext.userAttachData["GSW_APPLICATION_ID"]));
                    else
                        _logger.Debug("PrepareUserData : GSW_APPLICATION_ID is null");

                if (_datacontext.userAttachData.ContainsKey("GSW_CAMPAIGN_NAME"))
                    if (!_updateUserEventData.ContainsKey("GSW_CAMPAIGN_NAME"))
                        _updateUserEventData.Add("GSW_CAMPAIGN_NAME", Convert.ToInt32(_datacontext.userAttachData["GSW_CAMPAIGN_NAME"]));
                    else
                        _logger.Debug("PrepareUserData : GSW_CAMPAIGN_NAME is null");

                if (chkbxPersonalCallback.IsChecked == true)
                {
                    if (!_updateUserEventData.ContainsKey("PHSPCB"))
                        _updateUserEventData.Add("PHSPCB", "1");
                }
                else
                {
                    if (!_updateUserEventData.ContainsKey("PHSPCB"))
                        _updateUserEventData.Add("PHSPCB", "0");
                }
                if (!_updateUserEventData.ContainsKey("GSW_CALLBACK_TYPE"))
                    _updateUserEventData.Add("GSW_CALLBACK_TYPE", "Campaign");

                if (_datacontext.userAttachData.ContainsKey("GSW_CALLING_LIST"))
                    if (!_updateUserEventData.ContainsKey("GSW_CALLING_LIST"))
                        _updateUserEventData.Add("GSW_CALLING_LIST", Convert.ToInt32(_datacontext.userAttachData["GSW_CALLING_LIST"]));
                    else
                        _logger.Debug("PrepareUserData : GSW_CALLING_LIST is null");

                if (chkbxRescheduleCall.IsChecked == true)
                {
                    DateTime dt = new DateTime();
                    dt = Convert.ToDateTime(dpRescheduledate.Text);
                    DateTime dtTime = new DateTime();
                    dtTime = Convert.ToDateTime(dpRescheduletime.Text);

                    if (_configContainer.AllKeys.Contains("voice.ocs.time-format") && !string.IsNullOrEmpty(_configContainer.GetAsString("voice.ocs.time-format")))
                    {
                        try
                        {
                            finalDateTime = dt.Date.ToShortDateString() + " " + dtTime.ToString(_configContainer.GetAsString("voice.ocs.time-format"), CultureInfo.InstalledUICulture);
                        }
                        catch (Exception generalException)
                        {
                            _logger.Error("Error occurred while converting time in the given format : " + _configContainer.GetAsString("voice.ocs.time-format") + " : " + generalException.ToString());
                            finalDateTime = dt.Date.ToShortDateString() + " " + dtTime.ToString("HH:mm", CultureInfo.InstalledUICulture);
                        }
                    }

                    if (!_updateUserEventData.ContainsKey("GSW_DATE_TIME"))
                        _updateUserEventData.Add("GSW_DATE_TIME", finalDateTime);
                    if (!_updateUserEventData.ContainsKey("GSW_AGENT_REQ_TYPE"))
                        _updateUserEventData.Add("GSW_AGENT_REQ_TYPE", "RecordReschedule");
                }
                if (!string.IsNullOrEmpty(cmbCallResult.Text.Trim()) && cmbCallResult.SelectedIndex >= 0)
                    if (!_updateUserEventData.ContainsKey("GSW_CALL_RESULT"))
                        _updateUserEventData.Add("GSW_CALL_RESULT", Convert.ToInt32(cmbCallResult.SelectedValue.ToString()));

                if (_datacontext.userAttachData.ContainsKey("GSW_PHONE"))
                    if (!_updateUserEventData.ContainsKey("GSW_PHONE"))
                        _updateUserEventData.Add("GSW_PHONE", _datacontext.userAttachData["GSW_PHONE"].ToString());

                if (!_updateUserEventData.ContainsKey("GSW_AGENT_REQ_TYPE"))
                    _updateUserEventData.Add("GSW_AGENT_REQ_TYPE", "UpdateCallCompletionStats");

                var soft = new SoftPhone();
                OutputValues output = soft.UpdateOCSCallData(_updateUserEventData);
                if (output.MessageCode == "200")
                {
                    _logger.Info("OCS Call Data Updated Successfully....");
                }
                SendRecordProcessed(_updateUserEventData);
            }
            catch (Exception generalException)
            {
                _logger.Error("Error occurred in PrepareUserData() : " + generalException.ToString());
            }
        }

        /// <summary>
        /// Sends the record processed.
        /// </summary>
        private void SendRecordProcessed(KeyValueCollection updateUserEventData)
        {
            try
            {
                if (updateUserEventData == null || _datacontext.userAttachData == null)
                    return;
                if (_datacontext.userAttachData.ContainsKey("GSW_AGENT_REQ_TYPE") && updateUserEventData.ContainsKey("GSW_AGENT_REQ_TYPE"))
                {
                    _datacontext.userAttachData["GSW_AGENT_REQ_TYPE"] = "RecordProcessed";
                    updateUserEventData["GSW_AGENT_REQ_TYPE"] = "RecordProceed";
                    var soft = new SoftPhone();
                    OutputValues output = soft.UpdateOCSCallData(_updateUserEventData);
                    if (output.MessageCode == "200")
                    {
                        _logger.Info("OCS Call Data Updated Successfully....");
                    }
                }
            }
            catch (Exception generalException)
            {
                _logger.Error("Error occurred SendRecordProcessed() : " + generalException.ToString());
            }
        }
    }
}