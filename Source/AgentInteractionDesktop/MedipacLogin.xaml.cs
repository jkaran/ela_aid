using Pointel.Configuration.Manager;
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

namespace Agent.Interaction.Desktop
{
    /// <summary>
    /// Interaction logic for MedipacLogin.xaml
    /// </summary>
    public partial class MedipacLogin : Window
    {
        #region Member Declaration

        public DropShadowBitmapEffect ShadowEffect = new DropShadowBitmapEffect();

        #endregion Member Declaration

        #region Constructor
        public MedipacLogin()
        {
            InitializeComponent();
            btnOk.Content = "_" + (string)FindResource("KeyOk");
            btnCancel.Content = "_" + (string)FindResource("KeyCancel");
        }
        #endregion Constructor

        #region Window Events

        private void Window_Activated(object sender, EventArgs e)
        {
            MainBorder.BorderBrush = new SolidColorBrush((Color)(ColorConverter.ConvertFromString("#0070C5")));
            MainBorder.BitmapEffect = ShadowEffect;
        }

        private void Window_Deactivated(object sender, EventArgs e)
        {
            MainBorder.BorderBrush = Brushes.Black;
            MainBorder.BitmapEffect = null;
        }

        #endregion  Window Events


        private void btnOk_Click(object sender, RoutedEventArgs e)
        {

        }

        private void btnCancel_Click(object sender, RoutedEventArgs e)
        {

        }


        /// <summary>
        /// Mouses the left button down.
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="e">The <see cref="MouseButtonEventArgs"/> instance containing the event data.</param>
        private void MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            try
            {
                this.DragMove();
                if (!(ConfigContainer.Instance().AllKeys.Contains("allow.system-draggable") &&
                        ((string)ConfigContainer.Instance().GetValue("allow.system-draggable")).ToLower().Equals("true")))
                {
                    if (Left < 0)
                        Left = 0;
                    if (Top < 0)
                        Top = 0;
                    if (Left > SystemParameters.WorkArea.Right - Width)
                        Left = SystemParameters.WorkArea.Right - Width;
                    if (Top > SystemParameters.WorkArea.Bottom - Height)
                        Top = SystemParameters.WorkArea.Bottom - Height; ;
                }
            }
            catch { }
        }

        private void cbxKeepPlace_Checked(object sender, RoutedEventArgs e)
        {
            try
            {
                txtMediPacUserName.Text = string.Empty;
                txtMediPacPassword.Password = string.Empty;
                txtMediPacUserName.IsEnabled = false;
                txtMediPacPassword.IsEnabled = false;
            }
            catch (Exception ex)
            {
                
               
            }
        }

        private void cbxKeepPlace_Unchecked(object sender, RoutedEventArgs e)
        {
            try
            {
                txtMediPacUserName.Background = Brushes.White;
                txtMediPacPassword.Background = Brushes.White;
                txtMediPacUserName.IsEnabled = true;
                txtMediPacPassword.IsEnabled = true;
            }
            catch (Exception ex)
            {


            }
        }
    }
}
