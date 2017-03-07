using System;
using System.Configuration;
using System.Deployment.Application;
using System.Linq;
using System.Windows;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Media.Effects;
using Agent.Interaction.Desktop.Settings;

namespace Agent.Interaction.Desktop
{
    /// <summary>
    /// Interaction logic for About.xaml
    /// </summary>
    public partial class About : Window
    {
        public DropShadowBitmapEffect ShadowEffect = new DropShadowBitmapEffect();
        private Datacontext _dataContext = Datacontext.GetInstance();

        /// <summary>
        /// Initializes a new instance of the <see cref="About"/> class.
        /// </summary>

        #region Constructor

        public About()
        {
            InitializeComponent();
            this.DataContext = Datacontext.GetInstance();
            MainBorder.BorderBrush = new SolidColorBrush((Color)(ColorConverter.ConvertFromString("#0070C5")));
            MainBorder.BitmapEffect = ShadowEffect;
            RenderOptions.SetCachingHint(MainBorder.BorderBrush, CachingHint.Cache);
            RenderOptions.SetBitmapScalingMode(MainBorder.BorderBrush, BitmapScalingMode.LowQuality);
            ShadowEffect.ShadowDepth = 0;
            ShadowEffect.Opacity = 0.5;
            ShadowEffect.Softness = 0.5;
            ShadowEffect.Color = (Color)ColorConverter.ConvertFromString("#003660");
            if (ApplicationDeployment.IsNetworkDeployed)
                if (ConfigurationManager.AppSettings.AllKeys.Contains("application.version"))
                    if (!string.IsNullOrEmpty(ConfigurationManager.AppSettings["application.version"]))
                        Version.Content = "V " + ConfigurationManager.AppSettings["application.version"].ToString();
        }

        #endregion Constructor

        #region Window event

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
        /// Handles the Click event of the btnLogin control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="RoutedEventArgs"/> instance containing the event data.</param>
        private void btnLogin_Click(object sender, RoutedEventArgs e)
        {
            Close();
            _dataContext.IsAboutOpened = false;
        }

        private void MainBorder_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            try
            {
                this.DragMove(); if (this.Left < 0)
                    this.Left = 0;
                if (this.Top < 0)
                    this.Top = 0;
                if (this.Left > System.Windows.SystemParameters.WorkArea.Right - this.Width)
                    this.Left = System.Windows.SystemParameters.WorkArea.Right - this.Width;
                if (this.Top > System.Windows.SystemParameters.WorkArea.Bottom - this.Height)
                    this.Top = System.Windows.SystemParameters.WorkArea.Bottom - this.Height;
            }
            catch { }
        }

        /// <summary>
        /// Handles the Completed event of the Storyboard control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="EventArgs"/> instance containing the event data.</param>
        private void Storyboard_Completed(object sender, EventArgs e)
        {
            this.Topmost = false;
        }

        /// <summary>
        /// Handles the PreviewKeyDown event of the Window control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="KeyEventArgs"/> instance containing the event data.</param>
        private void Window_PreviewKeyDown(object sender, KeyEventArgs e)
        {
            if (Keyboard.Modifiers == ModifierKeys.Alt && (e.SystemKey == Key.Space || e.SystemKey == Key.F4))
            {
                e.Handled = true;
            }
            else
            {
                base.OnKeyDown(e);
            }
        }

        #endregion Window event

        #region Callable functions

        /// <summary>
        /// Blinks this instance.
        /// </summary>
        public void Blink()
        {
            this.Topmost = true;
            BeginStoryboard((Storyboard)FindResource("BlinkBorder"));
        }

        #endregion Callable functions
    }
}