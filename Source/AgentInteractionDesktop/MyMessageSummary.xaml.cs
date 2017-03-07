namespace Agent.Interaction.Desktop
{
    using System;
    using System.Linq;
    using System.Runtime.InteropServices;
    using System.Windows;
    using System.Windows.Input;
    using System.Windows.Interop;
    using System.Windows.Media;
    using System.Windows.Media.Animation;
    using System.Windows.Media.Effects;
    using System.Windows.Media.Imaging;

    using Agent.Interaction.Desktop.Helpers;
    using Agent.Interaction.Desktop.Settings;

    /// <summary>
    /// Interaction logic for MyMessageSummary.xaml
    /// </summary>
    public partial class MyMessageSummary : Window
    {
        #region Fields

        public SoftPhoneBar bottom;
        public DropShadowBitmapEffect ShadowEffect = new DropShadowBitmapEffect();

        private const int GWL_STYLE = -16; //WPF's Message code for Title Bar's Style
        private const int WS_SYSMENU = 0x80000; //WPF's Message code for System Menu

        private Datacontext _dataContext = Datacontext.GetInstance();
        private Pointel.Logger.Core.ILog _logger = Pointel.Logger.Core.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType,
            "AID");

        #endregion Fields

        #region Constructors

        /// <summary>
        /// Initializes a new instance of the <see cref="MyMessageSummary"/> class.
        /// </summary>
        /// <param name="owner">The owner.</param>
        public MyMessageSummary(Window owner)
        {
            EventManager.RegisterClassHandler(typeof(UIElement), AccessKeyManager.AccessKeyPressedEvent, new AccessKeyPressedEventHandler(OnAccessKeyPressed));
            if (owner is SoftPhoneBar)
                bottom = owner as SoftPhoneBar;
            InitializeComponent();
            this.DataContext = Datacontext.GetInstance();

            ShadowEffect.ShadowDepth = 0;
            ShadowEffect.Opacity = 0.5;
            ShadowEffect.Softness = 0.5;
            ShadowEffect.Color = (Color)ColorConverter.ConvertFromString("#003660");
        }

        #endregion Constructors

        #region Methods

        public void Blink()
        {
            this.Topmost = true;
            BeginStoryboard((Storyboard)FindResource("BlinkBorder"));
            this.Focus();
        }

        /// <summary>
        /// Loads the grid.
        /// </summary>
        /// <param name="calledby">The called by.</param>
        public void LoadGrid(string calledby)
        {
            try
            {
                string[] data = _dataContext.Msgsummary;

                #region Called by Bottom

                if (calledby == "Bottom")
                {
                    int index = 0;
                    MyMessages selectedMessage = bottom.DGMyMessages.SelectedItem as MyMessages;
                    if (_dataContext.MyMessages.Count >= 0 && selectedMessage != null)
                    {
                        if (_dataContext.MyMessages.Any(p => p.Index == selectedMessage.Index && p.ISread == false))
                        {
                            index = _dataContext.MyMessages.IndexOf(_dataContext.MyMessages.Where(p => p.Index == selectedMessage.Index).FirstOrDefault());
                            _dataContext.MyMessages.RemoveAt(index);
                            _dataContext.MyMessages.Insert(index, new MyMessages(selectedMessage.Index, selectedMessage.MessageIconImageSource, selectedMessage.MessageType,
                                            selectedMessage.MessageSender, selectedMessage.MessageSubject, selectedMessage.MessagePriority,
                                            selectedMessage.MessageDate, selectedMessage.MessageAudience, selectedMessage.MessageBody, true));
                            _dataContext.UnreadMsgCount--;
                            _dataContext.UnreadMessageCount = (_dataContext.UnreadMsgCount) >= 9 ? "9" : (_dataContext.UnreadMsgCount).ToString();
                            _dataContext.MessageCountRange = _dataContext.UnreadMsgCount <= 0 ? "-1" : (_dataContext.UnreadMsgCount <= 9 ? "0" : "1");
                        }
                    }
                    if (selectedMessage != null)
                    {
                        if (data.Length > 0)
                        {
                            if (data.Any(s => s.Contains("Subject")))
                            {
                                data[(data.Select((c, i) => new { c, i }).Where(x => x.c.StartsWith("Subject")).FirstOrDefault()).i] = "Subject:$" + selectedMessage.MessageSubject.ToString();
                            }
                            if (data.Any(s => s.Contains("Sender")))
                            {
                                data[(data.Select((c, i) => new { c, i }).Where(x => x.c.StartsWith("Sender")).FirstOrDefault()).i] = "Sender:$" + selectedMessage.MessageSender.ToString();
                            }
                            if (data.Any(s => s.Contains("Priority")))
                            {
                                data[(data.Select((c, i) => new { c, i }).Where(x => x.c.StartsWith("Priority")).FirstOrDefault()).i] = "Priority:$" + selectedMessage.MessagePriority.ToString();
                            }
                            if (data.Any(s => s.Contains("Date")))
                            {
                                data[(data.Select((c, i) => new { c, i }).Where(x => x.c.StartsWith("Date")).FirstOrDefault()).i] = "Date:$" + selectedMessage.MessageDate.ToString();
                            }
                            if (data.Any(s => s.Contains("Topic")))
                            {
                                data[(data.Select((c, i) => new { c, i }).Where(x => x.c.StartsWith("Topic")).FirstOrDefault()).i] = "Audience:$" + selectedMessage.MessageAudience.ToString();
                            }
                        }
                        int item = 0;
                        foreach (string value in data)
                        {
                            string[] temp = value.Split('$');
                            if (temp.Count() == 2)
                            {
                                if (item == 0)
                                {
                                    _dataContext.Message11 = temp[0];
                                    _dataContext.Message111 = temp[1];
                                }
                                else if (item == 1)
                                {
                                    _dataContext.Message22 = temp[0];
                                    _dataContext.Message222 = temp[1];
                                }
                                else if (item == 2)
                                {
                                    _dataContext.Message33 = temp[0];
                                    _dataContext.Message333 = temp[1];
                                }
                                else if (item == 3)
                                {
                                    _dataContext.Message44 = temp[0];
                                    _dataContext.Message444 = temp[1];
                                }
                                else if (item == 4)
                                {
                                    _dataContext.Message55 = temp[0];
                                    _dataContext.Message555 = temp[1];
                                }
                            }
                            item++;
                        }
                        _dataContext.MessageBodyMsg = selectedMessage.MessageBody;
                        if (!string.IsNullOrEmpty(selectedMessage.MessageBody))
                            _dataContext.MsgRowHeight = GridLength.Auto;
                        else
                            _dataContext.MsgRowHeight = new GridLength(0);
                        _dataContext.MessageIconImageSource = selectedMessage.MessageIconImageSource;
                        _dataContext.MessageType = selectedMessage.MessageType;
                        _dataContext.BroadCastBackgroundBrush = (Brush)(new BrushConverter().ConvertFromString(_dataContext.BroadCastAttributes[selectedMessage.MessagePriority.ToString()]));
                        var color = (Color)ColorConverter.ConvertFromString(_dataContext.BroadCastAttributes[selectedMessage.MessagePriority.ToString()]);
                        //  var brush = new SolidColorBrush(color);
                        _dataContext.BroadCastForegroundBrush = color;
                        show();
                        if (_dataContext.OpenedMessage != index.ToString())
                            _dataContext.OpenedMessage = index.ToString();
                        else
                            this.Blink();
                    }
                }

                #endregion Called by Bottom

                #region Called by Notifier

                else if (calledby == "Notifier")
                {
                    if (data.Length > 0)
                    {
                        if (data.Any(s => s.Contains("Subject")))
                        {
                            data[(data.Select((c, i) => new { c, i }).Where(x => x.c.StartsWith("Subject")).FirstOrDefault()).i] = "Subject:$" + _dataContext.MessageSubject.ToString();
                        }
                        if (data.Any(s => s.Contains("Sender")))
                        {
                            data[(data.Select((c, i) => new { c, i }).Where(x => x.c.StartsWith("Sender")).FirstOrDefault()).i] = "Sender:$" + _dataContext.MessageSender.ToString();
                        }
                        if (data.Any(s => s.Contains("Priority")))
                        {
                            data[(data.Select((c, i) => new { c, i }).Where(x => x.c.StartsWith("Priority")).FirstOrDefault()).i] = "Priority:$" + _dataContext.MessagePriority.ToString();
                        }
                        if (data.Any(s => s.Contains("Date")))
                        {
                            data[(data.Select((c, i) => new { c, i }).Where(x => x.c.StartsWith("Date")).FirstOrDefault()).i] = "Date:$" + _dataContext.MessageDate.ToString();
                        }
                        if (data.Any(s => s.Contains("Topic")))
                        {
                            data[(data.Select((c, i) => new { c, i }).Where(x => x.c.StartsWith("Topic")).FirstOrDefault()).i] = "Audience:$" + _dataContext.MessageAudience.ToString();
                        }
                    }

                    int item1 = 0;
                    foreach (string value in data)
                    {
                        string[] temp = value.Split('$');
                        if (temp.Count() == 2)
                        {
                            if (item1 == 0)
                            {
                                _dataContext.Message11 = temp[0];
                                _dataContext.Message111 = temp[1];
                            }
                            else if (item1 == 1)
                            {
                                _dataContext.Message22 = temp[0];
                                _dataContext.Message222 = temp[1];
                            }
                            else if (item1 == 2)
                            {
                                _dataContext.Message33 = temp[0];
                                _dataContext.Message333 = temp[1];
                            }
                            else if (item1 == 3)
                            {
                                _dataContext.Message44 = temp[0];
                                _dataContext.Message444 = temp[1];
                            }
                            else if (item1 == 4)
                            {
                                _dataContext.Message55 = temp[0];
                                _dataContext.Message555 = temp[1];
                            }
                        }
                        item1++;
                    }
                    _dataContext.MessageBodyMsg = _dataContext.MessageBodyMsg;
                    if (!string.IsNullOrEmpty(_dataContext.MessageBodyMsg))
                        _dataContext.MsgRowHeight = GridLength.Auto;
                    else
                        _dataContext.MsgRowHeight = new GridLength(0);
                    _dataContext.MessageIconImageSource = _dataContext.MessageIconImageSource;
                    _dataContext.MessageType = _dataContext.MessageType;
                    _dataContext.BroadCastBackgroundBrush = (Brush)(new BrushConverter().ConvertFromString(_dataContext.BroadCastAttributes[_dataContext.MessagePriority.ToString()]));
                    var color = (Color)ColorConverter.ConvertFromString(_dataContext.BroadCastAttributes[_dataContext.MessagePriority.ToString()]);
                    // var brush = new SolidColorBrush(color);
                    _dataContext.BroadCastForegroundBrush = color;
                    //this.Title = _dataContext.MessageType + " - " + _dataContext.MessageSubject.ToString();
                    //this.Icon = _dataContext.MessageIconImageSource;
                    show();
                    if (_dataContext.OpenedMessage != _dataContext.OpenedNotifyMessage)
                        _dataContext.OpenedMessage = _dataContext.OpenedNotifyMessage;
                    else
                        this.Blink();
                }

                #endregion Called by Notifier
            }
            catch (Exception commonException)
            {
                _logger.Error("Error occurred while loading Grid : " + commonException.Message.ToString());
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
        /// Handles the Click event of the btn_right control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="RoutedEventArgs"/> instance containing the event data.</param>
        private void btn_right_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }

        /// <summary>
        /// Handles the MouseLeftButtonDown event of the Label control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="MouseButtonEventArgs"/> instance containing the event data.</param>
        private void Label_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            try { this.DragMove(); }
            catch { }
        }

        private void MessageSummary_Loaded(object sender, RoutedEventArgs e)
        {
            var hwnd = new WindowInteropHelper(this).Handle;
            SetWindowLong(hwnd, GWL_STYLE, GetWindowLong(hwnd, GWL_STYLE) & ~WS_SYSMENU);
        }

        private void MessageSummary_PreviewKeyDown(object sender, KeyEventArgs e)
        {
            if ((Keyboard.IsKeyDown(Key.LeftAlt) || Keyboard.IsKeyDown(Key.RightAlt)) && Keyboard.IsKeyDown(Key.F4))
                e.Handled = true;
        }

        private void show()
        {
            this.Show();
            MessageType.Content = _dataContext.MessageType;
            BroadCastBackgroundBrush.Background = _dataContext.BroadCastBackgroundBrush;
            MessageIconImageSource.Source = _dataContext.MessageIconImageSource;
            Message11.Text = _dataContext.Message11;
            Message111.Text = _dataContext.Message111;
            Message22.Text = _dataContext.Message22;
            Message222.Text = _dataContext.Message222;
            Message33.Text = _dataContext.Message33;
            Message333.Text = _dataContext.Message333;
            Message44.Text = _dataContext.Message44;
            Message444.Text = _dataContext.Message444;
            Message55.Text = _dataContext.Message55;
            Message555.Text = _dataContext.Message555;
            MessageBodyMsg.Text = _dataContext.MessageBodyMsg;
        }

        private void Storyboard_Completed(object sender, EventArgs e)
        {
            this.Topmost = false;
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

        #endregion Methods
    }
}