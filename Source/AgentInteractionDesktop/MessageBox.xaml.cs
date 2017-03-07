#region System Namespace

using System;
using System.Windows;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Controls;
using System.Windows.Media.Effects;

#endregion System Namespace

#region AID Namesapce

using Agent.Interaction.Desktop.Settings;
using System.Windows.Interop;
using System.Runtime.InteropServices;

#endregion AID Namesapce

namespace Agent.Interaction.Desktop
{
    /// <summary>
    /// Interaction logic for MessageBox.xaml
    /// </summary>
    public partial class MessageBox : Window, IDisposable
    {
        #region Public Members

        public string btnleft_Content = string.Empty;
        public string btnright_Content = string.Empty;
        public bool IsUsedForForward = false;
        public DropShadowBitmapEffect ShadowEffect = new DropShadowBitmapEffect();
        public static string ContentValue = string.Empty;
        private Datacontext _dataContext = Datacontext.GetInstance();

        private const int GWL_STYLE = -16; //WPF's Message code for Title Bar's Style 
        private const int WS_SYSMENU = 0x80000; //WPF's Message code for System Menu
        [DllImport("user32.dll", SetLastError = true)]
        private static extern int GetWindowLong(IntPtr hWnd, int nIndex);
        [DllImport("user32.dll")]
        private static extern int SetWindowLong(IntPtr hWnd, int nIndex, int dwNewLong);

        #endregion Public Members

        #region Constructor

        /// <summary>
        /// Initializes a new instance of the <see cref="MessageBox"/> class.
        /// </summary>
        /// <param name="Msgbox_typename">The msgbox_typename.</param>
        /// <param name="content">The content.</param>
        /// <param name="btnleft_content">The btnleft_content.</param>
        /// <param name="btnright_content">The btnright_content.</param>
        /// <param name="isUsedForForward">if set to <c>true</c> [is used for forward].</param>
        public MessageBox(string Msgbox_typename, string content, string btnleft_content, string btnright_content, bool isUsedForForward)
        {
            btnleft_Content = btnleft_content;
            btnright_Content = btnright_content;
            IsUsedForForward = isUsedForForward;
            InitializeComponent();
            this.DataContext = Datacontext.GetInstance();
            ForwardError.Visibility = System.Windows.Visibility.Hidden;
            ForwardError.Content = "";
            if (Msgbox_typename == "close")
            {
                this.DialogResult = true;
                this.Close();
            }
            if (IsUsedForForward)
            {
                growFwd.Height = GridLength.Auto;
                btn_left.IsEnabled = false;
            }
            else
                growFwd.Height = new GridLength(0);

            if (IsUsedForForward)
            {
                if (Msgbox_typename.Contains("Cancel"))
                {
                    lblTitle.Content = Msgbox_typename;
                    txtForward.Visibility = System.Windows.Visibility.Hidden;
                    lblForward.Visibility = System.Windows.Visibility.Hidden;
                    btn_left.IsEnabled = true;
                }
                else
                {
                    lblTitle.Content = Msgbox_typename;
                    txtForward.Focus();
                    lblForward.Visibility = System.Windows.Visibility.Visible;
                    txtForward.Visibility = System.Windows.Visibility.Visible;
                }
            }
            else
                lblTitle.Content = Msgbox_typename + " - Agent Interaction Desktop";

            txtblockContent.Text = content;
            if (!string.IsNullOrEmpty(btnleft_content))
                btn_left.Content = btnleft_content;
            else
                btn_left.Visibility = System.Windows.Visibility.Hidden;
            btn_right.Content = btnright_content;
            ShadowEffect.ShadowDepth = 0;
            ShadowEffect.Opacity = 0.5;
            ShadowEffect.Softness = 0.5;
            ShadowEffect.Color = (Color)ColorConverter.ConvertFromString("#003660");
            ContentValue = content;
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
        /// Handles the MouseLeftButtonDown event of the Label control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="MouseButtonEventArgs"/> instance containing the event data.</param>
        private void Label_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            try { this.DragMove(); }
            catch { }
        }

        #region Controls Events

        /// <summary>
        /// Handles the SelectionChanged event of the txtForward control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="RoutedEventArgs"/> instance containing the event data.</param>
        private void txtForward_SelectionChanged(object sender, RoutedEventArgs e)
        {
            if (IsUsedForForward)
            {
                AutoCompleteBox currentPlace = sender as AutoCompleteBox;
                if (_dataContext.ForwardDns.Contains(currentPlace.Text))
                    btn_left.IsEnabled = true;
                else
                    btn_left.IsEnabled = false;
            }
        }

        /// <summary>
        /// Handles the Click event of the btn_left control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="RoutedEventArgs"/> instance containing the event data.</param>
        private void btn_left_Click(object sender, RoutedEventArgs e)
        {
            if (IsUsedForForward)
            {//_dataContext.ForwardDns.FindIndex(x => x.Equals(_dataContext.ForwardDN, StringComparison.OrdinalIgnoreCase)) != -1
                if (_dataContext.ForwardDns.FindIndex(x => x.Equals(_dataContext.ForwardDN, StringComparison.OrdinalIgnoreCase)) != -1 && !string.IsNullOrEmpty(_dataContext.ForwardDN.Trim()) && !string.IsNullOrWhiteSpace(_dataContext.ForwardDN))
                {
                    //_dataContext.ForwardDN = txtForward.Text;
                    ForwardError.Visibility = System.Windows.Visibility.Hidden;
                    ForwardError.Content = "";
                    this.DialogResult = true;
                    this.Close();
                }
                else
                {
                    ForwardError.Visibility = System.Windows.Visibility.Visible;
                    ForwardError.Content = "Enter the valid DN.";
                    txtForward.Focus();
                }
            }
            else
            {
                this.DialogResult = true;
                ForwardError.Content = "";
                this.Close();
            }
        }

        /// <summary>
        /// Handles the Click event of the btn_right control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="RoutedEventArgs"/> instance containing the event data.</param>
        private void btn_right_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                if (IsUsedForForward)
                {
                    if (!string.IsNullOrEmpty(btnleft_Content) && btn_right.Content.ToString() == "_Cancel")
                    {
                        _dataContext.ForwardDN = string.Empty;
                        this.DialogResult = false;
                    }
                }
                else if (!string.IsNullOrEmpty(btnleft_Content))
                    this.DialogResult = false;
                else
                    this.DialogResult = true;
                this.Close();
            }
            catch { this.Close(); }
        }

        #endregion Controls Events

        /// <summary>
        /// Handles the PreviewKeyDown event of the txtForward control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="KeyEventArgs"/> instance containing the event data.</param>
        private void txtForward_PreviewKeyDown(object sender, KeyEventArgs e)
        {
            ForwardError.Content = string.Empty;
            ForwardError.Visibility = System.Windows.Visibility.Hidden;
        }

        /// <summary>
        /// Handles the PreviewKeyUp event of the txtForward control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="KeyEventArgs"/> instance containing the event data.</param>
        private void txtForward_PreviewKeyUp(object sender, KeyEventArgs e)
        {
            var forwardedDN = sender as AutoCompleteBox;
            _dataContext.ForwardDN = forwardedDN.Text;
        }

        #endregion Window Events

        #region Dispose

        /// <summary>
        /// Performs application-defined tasks associated with freeing, releasing, or resetting unmanaged resources.
        /// </summary>
        public void Dispose()
        {
            btnleft_Content = null;
            btnright_Content = null;
            ShadowEffect = null;
            //GC.Collect();
        }

        #endregion Dispose

        private void Window_PreviewKeyDown(object sender, KeyEventArgs e)
        {
            if ((Keyboard.IsKeyDown(Key.LeftAlt) || Keyboard.IsKeyDown(Key.RightAlt)) && Keyboard.IsKeyDown(Key.F4))
                e.Handled = true;
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            var hwnd = new WindowInteropHelper(this).Handle;
            SetWindowLong(hwnd, GWL_STYLE, GetWindowLong(hwnd, GWL_STYLE) & ~WS_SYSMENU);
        }
    }
}