using System;
using System.Windows;
using System.Windows.Controls;
using Agent.Interaction.Desktop.Settings;

namespace Agent.Interaction.Desktop
{
    /// <summary>
    /// Interaction logic for ConsulCallSelection.xaml
    /// </summary>
    public partial class ConsulCallSelection : UserControl
    {
        #region Public Members

        public Agent.Interaction.Desktop.Settings.Datacontext.DialPadType WindowType;

        public delegate void FireBackNum(string callBy);

        public event FireBackNum eventFireBackNum;

        #endregion Public Members
        private Datacontext _dataContext = Datacontext.GetInstance();

        #region Constructor

        /// <summary>
        /// Initializes a new instance of the <see cref="ConsulCallSelection"/> class.
        /// </summary>
        /// <param name="windowType">Type of the window.</param>
        public ConsulCallSelection(Agent.Interaction.Desktop.Settings.Datacontext.DialPadType windowType)
        {
            WindowType = windowType;
            InitializeComponent();
        }

        #endregion Constructor

        #region Window Event

        /// <summary>
        /// Handles the Click event of the Single control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="RoutedEventArgs"/> instance containing the event data.</param>
        private void Single_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                if (_dataContext.ThisDN != _dataContext.DialedNumbers)
                {
                    if (WindowType == Datacontext.DialPadType.Transfer)
                    {
                        _dataContext.UserSetTransType = Datacontext.ConsultType.OneStep;
                        _dataContext.cmshow.IsOpen = false;
                        eventFireBackNum.Invoke("transfer");
                    }
                    else if (WindowType == Datacontext.DialPadType.Conference)
                    {
                        _dataContext.UserSetConfType = Datacontext.ConsultType.OneStep;
                        _dataContext.cmshow.IsOpen = false;
                        eventFireBackNum.Invoke("conference");
                    }
                }
                else
                {
                    _dataContext.UserSetConfType = Datacontext.ConsultType.None;
                    _dataContext.UserSetTransType = Datacontext.ConsultType.None;
                }
            }
            catch (Exception commonException)
            {
            }
        }

        /// <summary>
        /// Handles the Click event of the Double control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="RoutedEventArgs"/> instance containing the event data.</param>
        private void Double_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                if (_dataContext.ThisDN != _dataContext.DialedNumbers)
                {
                    if (WindowType == Datacontext.DialPadType.Transfer)
                    {
                        _dataContext.UserSetTransType = Datacontext.ConsultType.DualStep;
                        _dataContext.cmshow.IsOpen = false;
                        eventFireBackNum.Invoke("transfer");
                    }
                    else if (WindowType == Datacontext.DialPadType.Conference)
                    {
                        _dataContext.UserSetConfType = Datacontext.ConsultType.DualStep;
                        _dataContext.cmshow.IsOpen = false;
                        eventFireBackNum.Invoke("conference");
                    }
                }
                else
                {
                    _dataContext.UserSetConfType = Datacontext.ConsultType.None;
                    _dataContext.UserSetTransType = Datacontext.ConsultType.None;
                }
            }
            catch (Exception commonException)
            {
            }
        }

        #endregion Window Event
    }
}