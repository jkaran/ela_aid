namespace Agent.Interaction.Desktop
{
    using System;
    using System.Runtime.InteropServices;
    using System.Windows;
    using System.Windows.Controls;
    using System.Windows.Input;
    using System.Windows.Interop;
    using System.Windows.Media;
    using System.Windows.Media.Effects;
    using System.Windows.Threading;

    using Agent.Interaction.Desktop.Helpers;
    using Agent.Interaction.Desktop.Settings;

    using Microsoft.Windows.Controls.Primitives;

    using Pointel.Configuration.Manager;
    using Pointel.Softphone.Voice.Core;
    using Pointel.TaskbarNotifier;

    /// <summary>
    /// Interaction logic for Notifier.xaml
    /// </summary>
    public partial class Notifier : TaskbarNotifier, INotifier
    {
        #region Fields

        public DropShadowBitmapEffect ShadowEffect = new DropShadowBitmapEffect();

        private const int GWL_STYLE = -16; //WPF's Message code for Title Bar's Style
        private const int WS_SYSMENU = 0x80000; //WPF's Message code for System Menu

        private bool isPlayTone = false;
        private Pointel.Logger.Core.ILog logger = Pointel.Logger.Core.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType,
            "AID");

        // Added for ringing bell functionality
        private MediaPlayer mediaPlayer;
        private DispatcherTimer timerforstopplayingtone;
        private ConfigContainer _configContainer = ConfigContainer.Instance();
        private Datacontext _dataContext = Datacontext.GetInstance();

        #endregion Fields

        #region Constructors

        /// <summary>
        /// Initializes a new instance of the <see cref="Notifier"/> class.
        /// </summary>
        public Notifier()
        {
            InitializeComponent();

            mediaPlayer = new System.Windows.Media.MediaPlayer();
            mediaPlayer.MediaEnded += mediaPlayer_MediaEnded;

            bool isEnableCallReject = false;
            bool isEnableCallAccept = false;

            EventManager.RegisterClassHandler(typeof(UIElement), AccessKeyManager.AccessKeyPressedEvent, new AccessKeyPressedEventHandler(OnAccessKeyPressed));
            this.DataContext = Datacontext.GetInstance();
            _dataContext.ErrorRowHeight = new GridLength(0);
            if (_configContainer.AllKeys.Contains("voice.enable.call-notify-reject") &&
                        ((string)_configContainer.GetValue("voice.enable.call-notify-reject")).ToLower().Equals("true"))
                isEnableCallReject = true;

            if ((_configContainer.AllKeys.Contains("voice.enable.call-notify-accept") &&
                    ((string)_configContainer.GetValue("voice.enable.call-notify-accept")).ToLower().Equals("true")))
                isEnableCallAccept = true;

            if (!isEnableCallReject && isEnableCallAccept)
            {
                btnLeft.Visibility = System.Windows.Visibility.Hidden;
                btnRight.Content = "_Accept";
                btnRight.Style = (Style)FindResource("CallButton");
            }
            if (isEnableCallReject && !isEnableCallAccept)
            {
                btnLeft.Visibility = System.Windows.Visibility.Hidden;
                btnRight.Content = "_Reject";
                btnRight.Style = (Style)FindResource("RejectButton");
            }
            if (isEnableCallReject && isEnableCallAccept)
            {
                btnLeft.Visibility = System.Windows.Visibility.Visible;
                btnLeft.Content = "_Accept";
                btnRight.Content = "_Reject";
                btnRight.Style = (Style)FindResource("RejectButton");
            }
            if (btnRight.Content == "_Reject")
            {
                if (string.IsNullOrEmpty(_dataContext.ThridpartyDN) || !_configContainer.AllKeys.Contains("voice.requeue.route-dn") ||
                        string.IsNullOrEmpty(_configContainer.GetValue("voice.requeue.route-dn")))
                    btnRight.Visibility = System.Windows.Visibility.Hidden;
            }
        }

        #endregion Constructors

        #region Methods

        /// <summary>
        /// Gets the cell.
        /// </summary>
        /// <param name="host">The host.</param>
        /// <param name="row">The row.</param>
        /// <param name="columnIndex">Index of the column.</param>
        /// <returns></returns>
        public static Microsoft.Windows.Controls.DataGridCell GetCell(Microsoft.Windows.Controls.DataGrid host, Microsoft.Windows.Controls.DataGridRow row, int columnIndex)
        {
            if (row == null) return null;

            var presenter = GetVisualChild<DataGridCellsPresenter>(row);
            if (presenter == null) return null;

            // try to get the cell but it may possibly be virtualized
            var cell = (Microsoft.Windows.Controls.DataGridCell)presenter.ItemContainerGenerator.ContainerFromIndex(columnIndex);
            if (cell == null)
            {
                // now try to bring into view and retreive the cell
                host.ScrollIntoView(row, host.Columns[columnIndex]);
                cell = (Microsoft.Windows.Controls.DataGridCell)presenter.ItemContainerGenerator.ContainerFromIndex(columnIndex);
            }
            return cell;
        }

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

        public void Notify()
        {
            Left_Click(null, null);
        }

        public void PlayTone()
        {
            logger.Info("PlayTone Method Entry");
            if (ConfigContainer.Instance().AllKeys.Contains("voice.ringing-bell") && !string.IsNullOrEmpty((string)ConfigContainer.Instance().GetValue("voice.ringing-bell")))
            {
                try
                {
                    // Assign path to mediaplayer
                    string path = System.IO.Path.GetFullPath((((string)ConfigContainer.Instance().GetValue("voice.ringing-bell")).Split('|')[0]));
                    mediaPlayer.Open(new Uri(path));

                    // Set Volume to mediaplayer in double valid values from 0.0 to 1.0
                    if (!string.IsNullOrEmpty(((string)ConfigContainer.Instance().GetValue("voice.ringing-bell")).Split('|')[1]))
                    {
                        double volume;
                        double.TryParse(((string)ConfigContainer.Instance().GetValue("voice.ringing-bell")).Split('|')[1], out volume);
                        if (volume > 0)
                            mediaPlayer.Volume = volume;
                        else
                            mediaPlayer.Volume = 1.0;
                    }
                    else
                        mediaPlayer.Volume = 1.0;

                    // Set duration -1 means plays and repeats until an notifier closes, 0 means play the whole sound one time and  > 0 means a time, in milliseconds, to play and repeat the sound.
                    if (!string.IsNullOrEmpty(((string)ConfigContainer.Instance().GetValue("voice.ringing-bell")).Split('|')[2]))
                    {
                        int secondsforPlaying;
                        int.TryParse(((string)ConfigContainer.Instance().GetValue("voice.ringing-bell")).Split('|')[2], out secondsforPlaying);
                        if (secondsforPlaying > 0)
                        {
                            // Timer to stop the mediaplayer in defined seconds example 10 seconds
                            timerforstopplayingtone = new DispatcherTimer();
                            timerforstopplayingtone.Interval = TimeSpan.FromSeconds(secondsforPlaying);
                            timerforstopplayingtone.Tick += timerforstopplayingtone_Tick;
                            timerforstopplayingtone.Start();
                            // plays the audio
                            mediaPlayer.Play();
                        }
                        else
                            mediaPlayer.Play();
                    }
                    else
                        mediaPlayer.Play();

                    // Assign true for stopping mediaplayer when notifier closes
                    isPlayTone = true;
                }
                catch (Exception ex)
                {
                    isPlayTone = false;
                    logger.Error("Error occurred while opening url for voice ringing bell " + ex.Message);
                }
            }
            else
                isPlayTone = false;
            logger.Info("PlayTone Method Exit");
        }

        /// <summary>
        /// Reloads the unique identifier.
        /// </summary>
        /// <param name="isrejectCallDisplayed">if set to <c>true</c> [is reject call displayed].</param>
        public void ReloadUI(bool isrejectCallDisplayed)
        {
            if (!isrejectCallDisplayed && !_dataContext.SwitchName.ToLower().Contains("nortel"))// request redirect not supported in Nortel1000
            {
                if (_configContainer.AllKeys.Contains("voice.enable.call-notify-reject") &&
                            _configContainer.GetValue("voice.enable.call-notify-reject").ToLower().Equals("true") &&
                    _configContainer.AllKeys.Contains("voice.enable.call-notify-accept") &&
                        _configContainer.GetAsString("voice.enable.call-notify-accept").ToLower().Equals("true"))
                {
                    btnRight.Visibility = System.Windows.Visibility.Visible;
                    btnRight.Content = "_Reject";
                    btnRight.Style = (Style)FindResource("RejectButton");
                    btnLeft.Visibility = System.Windows.Visibility.Visible;
                    btnLeft.Content = "_Accept";
                    btnLeft.Style = (Style)FindResource("CallButton");
                }
                else if (_configContainer.AllKeys.Contains("voice.enable.call-notify-accept") &&
                        _configContainer.GetAsString("voice.enable.call-notify-accept").ToLower().Equals("true"))
                {
                    btnLeft.Visibility = System.Windows.Visibility.Collapsed;
                    btnRight.Content = "_Accept";
                    btnRight.Style = (Style)FindResource("CallButton");
                    btnRight.Visibility = System.Windows.Visibility.Visible;
                }
                else if (_configContainer.AllKeys.Contains("voice.enable.call-notify-reject") &&
                            _configContainer.GetValue("voice.enable.call-notify-reject").ToLower().Equals("true"))
                {
                    btnLeft.Visibility = System.Windows.Visibility.Collapsed;
                    btnRight.Content = "_Reject";
                    btnRight.Style = (Style)FindResource("RejectButton");
                    btnRight.Visibility = System.Windows.Visibility.Visible;
                }
                else
                {
                    btnLeft.Visibility = System.Windows.Visibility.Collapsed;
                    btnRight.Visibility = System.Windows.Visibility.Collapsed;
                }
            }
            else
            {
                if (((_configContainer.AllKeys.Contains("voice.enable.call-notify-accept") &&
                        _configContainer.GetAsString("voice.enable.call-notify-accept").ToLower().Equals("true"))))
                {
                    btnLeft.Visibility = System.Windows.Visibility.Collapsed;
                    btnRight.Content = "_Accept";
                    btnRight.Style = (Style)FindResource("CallButton");
                    btnRight.Visibility = System.Windows.Visibility.Visible;
                }
                else
                {
                    btnLeft.Visibility = System.Windows.Visibility.Hidden;
                    btnRight.Visibility = System.Windows.Visibility.Hidden;
                }
            }
        }

        public void StopTone()
        {
            try
            {
                if (isPlayTone)
                {
                    mediaPlayer.Stop();
                    if (timerforstopplayingtone != null)
                    {
                        timerforstopplayingtone.Stop();
                        timerforstopplayingtone.Tick -= timerforstopplayingtone_Tick;
                        timerforstopplayingtone = null;
                    }
                }
            }
            catch (Exception ex)
            {
                logger.Error("Error occurred in media player stopping " + ex.Message);
            }
        }

        [DllImport("user32.dll", SetLastError = true)]
        private static extern int GetWindowLong(IntPtr hWnd, int nIndex);

        /// <summary>
        /// Called when [access key pressed].
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="e">The <see cref="AccessKeyPressedEventArgs"/> instance containing the event data.</param>
        private static void OnAccessKeyPressed(object sender, AccessKeyPressedEventArgs e)
        {
            if (!e.Handled && e.Scope == null && (e.Target == null))
            {
                //if alt key is not in use handle event to prevent behavior without alt key
                if ((Keyboard.Modifiers & ModifierKeys.Alt) != ModifierKeys.Alt)
                {
                    e.Target = null;
                    e.Handled = true;
                }
            }
        }

        [DllImport("user32.dll")]
        private static extern int SetWindowLong(IntPtr hWnd, int nIndex, int dwNewLong);

        /// <summary>
        /// Alters the row.
        /// </summary>
        /// <param name="e">The <see cref="Microsoft.Windows.Controls.DataGridRowEventArgs"/> instance containing the event data.</param>
        private void AlterRow(Microsoft.Windows.Controls.DataGridRowEventArgs e)
        {
            var cell = GetCell(DGCallData, e.Row, 1);
            if (cell == null) return;

            var item = e.Row.Item as CallData;
            if (item == null) return;
            else
            {
                var val = item.Value;
                if (val != null)
                {
                    if ((val.StartsWith("http") || val.StartsWith("www")) &&
                        (_configContainer.AllKeys.Contains("voice.enable.attach-data-popup-url") &&
                        ((string)_configContainer.GetValue("voice.enable.attach-data-popup-url")).ToLower().Equals("true")))
                    {
                        cell.Foreground = Brushes.Blue;
                    }
                }
            }
        }

        /// <summary>
        /// Handles the LoadingRow event of the DGCallData control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="Microsoft.Windows.Controls.DataGridRowEventArgs"/> instance containing the event data.</param>
        private void DGCallData_LoadingRow(object sender, Microsoft.Windows.Controls.DataGridRowEventArgs e)
        {
            //Dispatcher.BeginInvoke(DispatcherPriority.Render, new Action(() => AlterRow(e)));
            AlterRow(e);
        }

        /// <summary>
        /// Handles the Collapsed event of the Expander control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="RoutedEventArgs"/> instance containing the event data.</param>
        private void Expander_Collapsed(object sender, RoutedEventArgs e)
        {
            base.SetInitialLocations(true);
            this.Left = System.Windows.SystemParameters.FullPrimaryScreenWidth - this.Width;

            this.Top = System.Windows.SystemParameters.FullPrimaryScreenHeight - this.Height;
            this.Top = this.Top + 98;
        }

        /// <summary>
        /// Handles the Expanded event of the Expander control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="RoutedEventArgs"/> instance containing the event data.</param>
        private void Expander_Expanded(object sender, RoutedEventArgs e)
        {
            base.SetInitialLocations(true);
            this.Left = System.Windows.SystemParameters.FullPrimaryScreenWidth - this.Width;

            this.Top = System.Windows.SystemParameters.FullPrimaryScreenHeight - this.Height;
            this.Top = this.Top - 52;
        }

        /// <summary>
        /// Handles the Click event of the Left control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="RoutedEventArgs"/> instance containing the event data.</param>
        private void Left_Click(object sender, RoutedEventArgs e)
        {
            SoftPhone softAnswer = new SoftPhone();
            softAnswer.Answer();
            base.stayOpenTimer.Stop();
            base.DisplayState = Pointel.TaskbarNotifier.TaskbarNotifier.DisplayStates.Hiding;
        }

        void mediaPlayer_MediaEnded(object sender, EventArgs e)
        {
            try
            {
                mediaPlayer.Stop();

                //Check the duration is -1, then repeat the audio.
                if (!string.IsNullOrEmpty(((string)ConfigContainer.Instance().GetValue("voice.ringing-bell")).Split('|')[2]) && (((string)ConfigContainer.Instance().GetValue("voice.ringing-bell")).Split('|')[2]) == "-1")
                    mediaPlayer.Play();
                else
                    isPlayTone = false;
            }
            catch (Exception ex)
            {
                logger.Error("Error occurred in media player ended event " + ex.Message);
            }
        }

        /// <summary>
        /// Handles the Click event of the Right control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="RoutedEventArgs"/> instance containing the event data.</param>
        private void Right_Click(object sender, RoutedEventArgs e)
        {
            Button tempbtn = sender as Button;
            if (tempbtn.Content.ToString().Contains("Reject"))
            {
                string a = _dataContext.DialedNumbers;
                if (!string.IsNullOrEmpty(_dataContext.ThridpartyDN))
                {
                    if (_dataContext.ThisDN == _dataContext.ThridpartyDN)
                    {
                        logger.Warn("Redirect DN and This DN are same.");
                    }
                    else
                    {
                        SoftPhone softRedirect = new SoftPhone();
                        softRedirect.Redirect(_dataContext.ThridpartyDN);
                    }
                }
                _dataContext.CallTypeStatus = string.Empty;
            }
            else if (tempbtn.Content.ToString().Contains("Accept"))
            {
                SoftPhone softAnswer = new SoftPhone();
                softAnswer.Answer();
            }
            base.stayOpenTimer.Stop();
            base.DisplayState = Pointel.TaskbarNotifier.TaskbarNotifier.DisplayStates.Hiding;
        }

        private void TaskbarNotifier_Loaded(object sender, RoutedEventArgs e)
        {
            try
            {
                var hwnd = new WindowInteropHelper(this).Handle;
                SetWindowLong(hwnd, GWL_STYLE, GetWindowLong(hwnd, GWL_STYLE) & ~WS_SYSMENU);
            }
            catch (Exception _generalException)
            {
                logger.Error("Error occurred as " + _generalException.Message);
            }
        }

        void timerforstopplayingtone_Tick(object sender, EventArgs e)
        {
            try
            {
                mediaPlayer.Stop();
                mediaPlayer.Play();
            }
            catch (Exception ex)
            {
                logger.Error("Error occurred in timerforstopplayingtone_Tick stopping " + ex.Message);
            }
        }

        #endregion Methods
    }
}