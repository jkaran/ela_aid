#region System namespace

using System.Windows;
using System.Linq;

#endregion System namespace

#region AID namespace

using Pointel.TaskbarNotifier;
using Agent.Interaction.Desktop.Settings;
using System.Windows.Input;
using System.Runtime.InteropServices;
using System;
using System.Windows.Interop;

#endregion AID namespace

namespace Agent.Interaction.Desktop
{
    /// <summary>
    /// Interaction logic for BroadCastNotifier.xaml
    /// </summary>
    public partial class BroadCastNotifier : TaskbarNotifier
    {

        #region Private Fields

        private const int GWL_STYLE = -16; //WPF's Message code for Title Bar's Style 
        private const int WS_SYSMENU = 0x80000; //WPF's Message code for System Menu
        [DllImport("user32.dll", SetLastError = true)]
        private static extern int GetWindowLong(IntPtr hWnd, int nIndex);
        [DllImport("user32.dll")]
        private static extern int SetWindowLong(IntPtr hWnd, int nIndex, int dwNewLong);

        #endregion


        #region Constructor

        /// <summary>
        /// Initializes a new instance of the <see cref="BroadCastNotifier"/> class.
        /// </summary>
        public BroadCastNotifier()
        {
            InitializeComponent();
            this.DataContext = Datacontext.GetInstance();
        }

        #endregion Constructor

        #region Window Events

        /// <summary>
        /// Handles the Loaded event of the TaskbarNotifier control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="RoutedEventArgs"/> instance containing the event data.</param>
        private void BroadCastNotifier_Loaded(object sender, RoutedEventArgs e)
        {
            var hwnd = new WindowInteropHelper(this).Handle;
            SetWindowLong(hwnd, GWL_STYLE, GetWindowLong(hwnd, GWL_STYLE) & ~WS_SYSMENU);
            EventManager.RegisterClassHandler(typeof(UIElement), AccessKeyManager.AccessKeyPressedEvent, new AccessKeyPressedEventHandler(OnAccessKeyPressed));

        }

        #endregion Window Events

        #region Controls Events

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

        /// <summary>
        /// Handles the Click event of the Show control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="RoutedEventArgs"/> instance containing the event data.</param>
        private void Show_Click(object sender, RoutedEventArgs e)
        {
            base.DisplayState = Pointel.TaskbarNotifier.TaskbarNotifier.DisplayStates.Hiding;
            var window = IsWindowOpen<Window>("MessageSummary");
            if (window != null)
                window.Close();
            MyMessageSummary summary = new MyMessageSummary(this);
            summary.LoadGrid("Notifier");
            summary.Show();
        }

        /// <summary>
        /// Handles the Click event of the Dismiss control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="RoutedEventArgs"/> instance containing the event data.</param>
        private void Dismiss_Click(object sender, RoutedEventArgs e)
        {
            base.DisplayState = Pointel.TaskbarNotifier.TaskbarNotifier.DisplayStates.Hiding;
        }

        #endregion Controls Events

        #region Callable Functions
        public static T IsWindowOpen<T>(string name = null) where T : Window
        {
            var windows = Application.Current.Windows.OfType<T>();
            return string.IsNullOrEmpty(name) ? windows.FirstOrDefault() : windows.FirstOrDefault(w => w.Name.Equals(name));
        }
        #endregion
    }
}