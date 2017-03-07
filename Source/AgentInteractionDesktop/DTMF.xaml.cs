namespace Agent.Interaction.Desktop
{
    using System.Text;
    using System.Windows;
    using System.Windows.Controls;
    using System.Windows.Input;

    using Agent.Interaction.Desktop.Settings;

    using Pointel.Softphone.Voice.Common;
    using Pointel.Softphone.Voice.Core;

    /// <summary>
    /// Interaction logic for DTMF.xaml
    /// </summary>
    public partial class DTMF : UserControl
    {
        #region Fields

        //public StringBuilder typednumber = new StringBuilder();
        private Datacontext _dataContext = Datacontext.GetInstance();

        #endregion Fields

        #region Constructors

        /// <summary>
        /// Initializes a new instance of the <see cref="DTMF"/> class.
        /// </summary>
        public DTMF()
        {
            InitializeComponent();
            if (_dataContext.DialedDigits.Length > 9 && txtNumbers.FontSize > 26.75)
            {
                txtNumbers.FontSize = txtNumbers.FontSize - 2.75;
            }
            else if (_dataContext.DialedDigits.Length <= 9)
            {
                if (txtNumbers.FontSize != 35)
                {
                    txtNumbers.FontSize = 35;
                }
            }
            txtNumbers.Focus();
        }

        #endregion Constructors

        #region Methods

        /// <summary>
        /// Keyboardvalues the specified key.
        /// </summary>
        /// <param name="key">The key.</param>
        /// <returns></returns>
        public string Keyboardvalue(Key key)
        {
            string value = string.Empty;
            if ((Keyboard.Modifiers & ModifierKeys.Shift) == ModifierKeys.Shift)
            {
                if (key == Key.D3)
                {
                    value = "#";
                }
                else if (key == Key.D8)
                {
                    value = "*";
                }
            }
            else
            {
                if (key == Key.D0 || key == Key.NumPad0)
                    value = "0";
                if (key == Key.D1 || key == Key.NumPad1)
                    value = "1";
                if (key == Key.D2 || key == Key.NumPad2)
                    value = "2";
                if (key == Key.D3 || key == Key.NumPad3)
                    value = "3";
                if (key == Key.D4 || key == Key.NumPad4)
                    value = "4";
                if (key == Key.D5 || key == Key.NumPad5)
                    value = "5";
                if (key == Key.D6 || key == Key.NumPad6)
                    value = "6";
                if (key == Key.D7 || key == Key.NumPad7)
                    value = "7";
                if (key == Key.D8 || key == Key.NumPad8)
                    value = "8";
                if (key == Key.D9 || key == Key.NumPad9)
                    value = "9";
                if (key == Key.Multiply)
                    value = "*";
            }
            return value;
        }

        /// <summary>
        /// Handles the Click event of the Number control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="RoutedEventArgs"/> instance containing the event data.</param>
        private void Number_Click(object sender, RoutedEventArgs e)
        {
            Button number = sender as Button;
            string dialedDigit = number.Content.ToString();
            //typednumber.Append(dialedDigit);
            _dataContext.DialedDigits = _dataContext.DialedDigits.Trim();
            if (string.IsNullOrEmpty(_dataContext.DialedDigits))
                _dataContext.DialedDigits = dialedDigit;
            else
                _dataContext.DialedDigits += dialedDigit;
            if (_dataContext.DialedDigits.Length > 9 && txtNumbers.FontSize > 26.75)
            {
                txtNumbers.FontSize = txtNumbers.FontSize - 2.75;
            }
            else if (_dataContext.DialedDigits.Length <= 9)
            {
                if (txtNumbers.FontSize != 35)
                {
                    txtNumbers.FontSize = 35;
                }
            }
            else
            {
                _dataContext.DialedDigits += " ";
            }
            SoftPhone softPhone = new SoftPhone();
            OutputValues output = softPhone.DtmfSend(dialedDigit);
            if (output.MessageCode == "200")
            {
                //_dataContext.StatusMessage = output.Message.ToString();
            }
            else if (output.MessageCode == "2001")
            {
                //_dataContext.StatusMessage = output.Message.ToString();
                //lblStatus.Foreground = Brushes.Red;
            }
            if (txtNumbers.Text.Length >= 0)
            {
                txtNumbers.SelectionStart = txtNumbers.Text.Length - 1;
                txtNumbers.SelectionLength = 0;
                txtNumbers.UpdateLayout();
            }
        }

        /// <summary>
        /// Handles the PreviewKeyDown event of the TabItem control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="KeyEventArgs"/> instance containing the event data.</param>
        private void TabItem_PreviewKeyDown(object sender, KeyEventArgs e)
        {
            if (Keyboard.Modifiers == ModifierKeys.Shift && e.Key == Key.D3)
            {
                btnh.RaiseEvent(new RoutedEventArgs(Button.ClickEvent));
            }
            else if (Keyboard.Modifiers == ModifierKeys.Shift && e.Key == Key.D8)
            {
                btns.RaiseEvent(new RoutedEventArgs(Button.ClickEvent));
            }
            else if (e.Key == Key.D0 || e.Key == Key.NumPad0)
                btn0.RaiseEvent(new RoutedEventArgs(Button.ClickEvent));
            else if (e.Key == Key.D1 || e.Key == Key.NumPad1)
                btn1.RaiseEvent(new RoutedEventArgs(Button.ClickEvent));
            else if (e.Key == Key.D2 || e.Key == Key.NumPad2)
                btn2.RaiseEvent(new RoutedEventArgs(Button.ClickEvent));
            else if (e.Key == Key.D3 || e.Key == Key.NumPad3)
                btn3.RaiseEvent(new RoutedEventArgs(Button.ClickEvent));
            else if (e.Key == Key.D4 || e.Key == Key.NumPad4)
                btn4.RaiseEvent(new RoutedEventArgs(Button.ClickEvent));
            else if (e.Key == Key.D5 || e.Key == Key.NumPad5)
                btn5.RaiseEvent(new RoutedEventArgs(Button.ClickEvent));
            else if (e.Key == Key.D6 || e.Key == Key.NumPad6)
                btn6.RaiseEvent(new RoutedEventArgs(Button.ClickEvent));
            else if (e.Key == Key.D7 || e.Key == Key.NumPad7)
                btn7.RaiseEvent(new RoutedEventArgs(Button.ClickEvent));
            else if (e.Key == Key.D8 || e.Key == Key.NumPad8)
                btn8.RaiseEvent(new RoutedEventArgs(Button.ClickEvent));
            else if (e.Key == Key.D9 || e.Key == Key.NumPad9)
                btn9.RaiseEvent(new RoutedEventArgs(Button.ClickEvent));
            else if (e.Key == Key.Multiply)
                btns.RaiseEvent(new RoutedEventArgs(Button.ClickEvent));
        }

        /// <summary>
        /// Handles the Loaded event of the UserControl control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="RoutedEventArgs"/> instance containing the event data.</param>
        private void UserControl_Loaded(object sender, RoutedEventArgs e)
        {
            txtNumbers.Focus();
            _dataContext.StatusMessageHeight = new GridLength(0);
        }

        #endregion Methods
    }
}