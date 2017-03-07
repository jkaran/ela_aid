using System;
using System.Windows.Controls;
using Agent.Interaction.Desktop.Settings;

namespace Agent.Interaction.Desktop
{
    /// <summary>
    /// Interaction logic for AgetStatus.xaml
    /// </summary>
    public partial class AgentState : UserControl
    {
        #region Constructor
        private Datacontext _dataContext = Datacontext.GetInstance();
        /// <summary>
        /// Initializes a new instance of the <see cref="AgentState"/> class.
        /// </summary>
        public AgentState()
        {
            InitializeComponent();
            this.DataContext = _dataContext;
        }

        #endregion Constructor

        #region Window Event

        /// <summary>
        /// Handles the Loaded event of the UserControl control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="System.Windows.RoutedEventArgs"/> instance containing the event data.</param>
        private void UserControl_Loaded(object sender, System.Windows.RoutedEventArgs e)
        {
            lblLoggedIN_time.Text = "Logged in since " + DateTime.Now.ToShortTimeString();
            lblLoginID.Text = "Login ID " + _dataContext.AgentLoginId;
        }

        #endregion Window Event
    }
}