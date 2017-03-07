using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Effects;
using System.Xml.Linq;
using Agent.Interaction.Desktop.Db;
using System.Runtime.InteropServices;
using System.Windows.Interop;

namespace Agent.Interaction.Desktop
{
    /// <summary>
    /// Interaction logic for DispositionForm.xaml
    /// </summary>
    public partial class DispositionForm : Window
    {
        #region Member Declaration

        public DropShadowBitmapEffect ShadowEffect = new DropShadowBitmapEffect();
        private Pointel.Logger.Core.ILog _logger = Pointel.Logger.Core.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType,
           "AID");
        DispositionDetail dispositionDetail;
        private const int GWL_STYLE = -16; //WPF's Message code for Title Bar's Style 
        private const int WS_SYSMENU = 0x80000; //WPF's Message code for System Menu
        [DllImport("user32.dll", SetLastError = true)]
        private static extern int GetWindowLong(IntPtr hWnd, int nIndex);
        [DllImport("user32.dll")]
        private static extern int SetWindowLong(IntPtr hWnd, int nIndex, int dwNewLong);
        #endregion Member Declaration

        #region Constructor

        /// <summary>
        /// Initializes a new instance of the <see cref="DispositionForm"/> class.
        /// </summary>
        /// <param name="callData">The call data.</param>
        public DispositionForm(Dictionary<string, string> callData)
        {
            InitializeComponent();
            ShadowEffect.ShadowDepth = 0;
            ShadowEffect.Opacity = 0.5;
            ShadowEffect.Softness = 0.5;
            ShadowEffect.Color = (Color)ColorConverter.ConvertFromString("#003660");
            txtError.Text = string.Empty;
            ReadXML();
            if (callData != null)
                AssignDispositionValues(callData);
            this.Topmost = true;

        }

        #endregion Constructor

        #region Window Events

        private void Window_Activated(object sender, EventArgs e)
        {
            try
            {
                MainBorder.BorderBrush = new SolidColorBrush((Color)(ColorConverter.ConvertFromString("#0070C5")));
                MainBorder.BitmapEffect = ShadowEffect;
            }
            catch (Exception _generalException)
            {
                _logger.Error("Error occurred as " + _generalException.Message);
            }
        }

        private void Window_Deactivated(object sender, EventArgs e)
        {
            try
            {
                MainBorder.BorderBrush = Brushes.Black;
                MainBorder.BitmapEffect = null;
            }
            catch (Exception _generalException)
            {
                _logger.Error("Error occurred as " + _generalException.Message);
            }
        }

        private void Border_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            try
            {
                this.DragMove();
            }
            catch (Exception _generalException)
            {
                _logger.Error("Error occurred as " + _generalException.Message);
            }
        }

        #endregion  Window Events

        private void AssignDispositionValues(Dictionary<string, string> callData)
        {
            dispositionDetail = new DispositionDetail();
            float duration = 0;
            foreach (var keyvalue in callData)
            {
                switch (keyvalue.Key.ToLower())
                {
                    case "agentloginid": dispositionDetail.AgentId = !string.IsNullOrEmpty(keyvalue.Value) ? keyvalue.Value.Trim() : string.Empty; break;
                    case "contactname": dispositionDetail.NameValue = !string.IsNullOrEmpty(keyvalue.Value) ? keyvalue.Value.Trim() : string.Empty; txtCallerName.Text = dispositionDetail.NameValue; break;
                    case "ani": dispositionDetail.PhoneValue = !string.IsNullOrEmpty(keyvalue.Value) ? keyvalue.Value.Trim() : string.Empty; txtPhone.Text = dispositionDetail.PhoneValue; break;
                    case "connectionid": dispositionDetail.ConnectionId = !string.IsNullOrEmpty(keyvalue.Value) ? keyvalue.Value.Trim() : string.Empty; break;
                    case "dnis": dispositionDetail.DNIS = !string.IsNullOrEmpty(keyvalue.Value) ? keyvalue.Value.Trim() : string.Empty; break;
                    case "duration": dispositionDetail.CallDuration = !string.IsNullOrEmpty(keyvalue.Value) ? (float.TryParse(keyvalue.Value.Trim(), out duration) ? dispositionDetail.CallDuration = (int)duration : 0) : 0; break;
                    case "mid": dispositionDetail.MID = !string.IsNullOrEmpty(keyvalue.Value) ? keyvalue.Value.Trim() : string.Empty; txtMID.Text = dispositionDetail.MID; break;
                    case "calltype": dispositionDetail.CallType = !string.IsNullOrEmpty(keyvalue.Value) ? keyvalue.Value.Trim() : string.Empty; break;
                    case "username": dispositionDetail.Username = !string.IsNullOrEmpty(keyvalue.Value) ? keyvalue.Value.Trim() : string.Empty; break;
                }
            }
        }

        private void ReadXML()
        {
            try
            {
                ComboBoxItem cbItem;
                var file = System.IO.Path.Combine(System.Windows.Forms.Application.StartupPath, "Disposition.xml");
                if (File.Exists(file))
                {
                    XDocument xdoc = XDocument.Load(file);
                    foreach (var childElements in xdoc.Root.Elements())
                    {
                        switch (childElements.Name.ToString().ToLower())
                        {
                            case "lob": if (childElements.Elements().Any())
                                {
                                    foreach (var childElement in childElements.Elements())
                                    {
                                        cbItem = new ComboBoxItem();

                                        //Insert Key and Value for LOB
                                        if (childElement.Attribute("key") != null && !string.IsNullOrEmpty(childElement.Attribute("key").Value))
                                        {
                                            //Inserting Key
                                            cbItem.Tag = childElement.Attribute("key").Value;

                                            if (childElement.Attribute("value") != null && !string.IsNullOrEmpty(childElement.Attribute("value").Value))
                                            {
                                                //Inserting Value
                                                cbItem.Content = childElement.Attribute("value").Value;
                                                cbLOB.Items.Add(cbItem);
                                            }
                                        }
                                    }
                                    cbLOB.SelectedIndex = 0;
                                }
                                break;

                            case "terminationcode": if (childElements.Elements().Any())
                                {
                                    foreach (var childElement in childElements.Elements())
                                    {
                                        cbItem = new ComboBoxItem();

                                        //Insert Key and Value for Termination Code
                                        if (childElement.Attribute("value") != null && !string.IsNullOrEmpty(childElement.Attribute("value").Value))
                                        {
                                            //Inserting Key
                                            cbItem.Tag = childElement.Attribute("value").Value;

                                            if (childElement.Attribute("xdesc") != null && !string.IsNullOrEmpty(childElement.Attribute("xdesc").Value))
                                            {
                                                //Inserting Value
                                                if (childElement.Attribute("value").Value.ToLower().Equals("n/a"))
                                                    cbItem.Content = childElement.Attribute("xdesc").Value;
                                                else
                                                    cbItem.Content = childElement.Attribute("value").Value + " " + childElement.Attribute("xdesc").Value;
                                                cbTerminationCode.Items.Add(cbItem);
                                            }
                                        }
                                    }
                                    cbTerminationCode.SelectedIndex = 0;
                                }
                                break;

                            default: break;
                        }
                    }
                }
            }
            catch (Exception exception)
            {
                _logger.Error("Error while reading XML" + exception.ToString());
            }

        }
        Thread st;
        private void btnSave_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                // Check Caller Name is Empty or not
                if (!string.IsNullOrEmpty(txtCallerName.Text.Trim()) && txtCallerName.Text.ToLower() != "n/a")
                {
                    // Check LOB value is selected or not
                    if (cbLOB.SelectedIndex > 0)
                    {
                        // Check Termination code value is selected or not
                        if (cbTerminationCode.SelectedIndex > 0)
                        {
                            txtError.Text = string.Empty;
                            // Update Values from UI
                            dispositionDetail.MID = txtMID.Text.Trim();
                            dispositionDetail.NameValue = txtCallerName.Text.Trim();
                            dispositionDetail.PhoneValue = txtPhone.Text.Trim();
                            ComboBoxItem cbItem = cbLOB.SelectedValue as ComboBoxItem;
                            dispositionDetail.LOB = cbItem.Content.ToString();
                            cbItem = cbTerminationCode.SelectedValue as ComboBoxItem;
                            dispositionDetail.TcCode = Convert.ToInt32(cbItem.Tag);
                            dispositionDetail.TcDescription = cbItem.Content.ToString().Substring((cbItem.Content.ToString().IndexOf(' ')) + 1);
                            dispositionDetail.ApplicationId = txtAWD.Text.Trim();
                            dispositionDetail.Notes = txtNotes.Text.Trim();

                            _logger.Info("ELV_LOB - " + dispositionDetail.LOB);
                            _logger.Info("ELV_MEDIA_TYPE - " + dispositionDetail.MediaType);
                            _logger.Info("ELV_TERM_DESC - " + dispositionDetail.TcDescription);
                            _logger.Info("AGENTID - " + dispositionDetail.AgentId);
                            _logger.Info("DBA - " + dispositionDetail.NameValue);
                            _logger.Info("CHANNEL - " + dispositionDetail.LOB);
                            _logger.Info("ANI - " + dispositionDetail.PhoneValue);
                            _logger.Info("CONNECTIONID - " + dispositionDetail.ConnectionId);
                            _logger.Info("DNIS - " + dispositionDetail.DNIS);
                            _logger.Info("TALKTIME - " + dispositionDetail.CallDuration);
                            _logger.Info("MID - " + dispositionDetail.MID);
                            _logger.Info("GENESYS_CALL_RESULT - " + dispositionDetail.CallResult);
                            _logger.Info("CALLTYPE - " + dispositionDetail.CallTypeCode);
                            _logger.Info("USERID - " + dispositionDetail.Username);
                            _logger.Info("APPID - " + dispositionDetail.ApplicationId);
                            _logger.Info("TERM_CODE - " + dispositionDetail.TcCode);
                            _logger.Info("NOTES - " + dispositionDetail.Notes);
                            _logger.Info("ELV_SUBMIT_FLAG - " + dispositionDetail.SubmitFlag);
                            _logger.Info("ELV_CALLTYPE2 - " + dispositionDetail.CallType);

                            string errorMessage;
                            DispositionDB dispositionDB = new DispositionDB();
                            if (dispositionDB.StorerDispositionDetail(dispositionDetail, out errorMessage))
                                this.Close();
                            else
                            {
                                _logger.Error(errorMessage);
                                txtError.Text = errorMessage;
                                st = new Thread(delegate() { Thread.Sleep(3000); Dispatcher.Invoke((Action)delegate() { this.Close(); }); });
                                st.Start();
                            }
                        }
                        else
                            txtError.Text = "Please select Termination Code";
                    }
                    else
                        txtError.Text = "Please select LOB";
                }
                else
                    txtError.Text = "Please provide Caller Name";
            }
            catch (Exception exception)
            {
                _logger.Error("Error while saving disposition data" + exception.ToString());
            }
        }

        private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {

            try
            {
                if (st != null)
                    st.Abort();
            }
            catch (Exception _generalException)
            {
                _logger.Error("Error occurred as " + _generalException.Message);
            }
        }

        private void Window_PreviewKeyDown(object sender, KeyEventArgs e)
        {
            try
            {
                if ((Keyboard.IsKeyDown(Key.LeftAlt) || Keyboard.IsKeyDown(Key.RightAlt)) && Keyboard.IsKeyDown(Key.F4))
                    e.Handled = true;
            }
            catch (Exception _generalException)
            {
                _logger.Error("Error occurred as " + _generalException.Message);
            }
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
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




