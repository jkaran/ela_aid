namespace Agent.Interaction.Desktop
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Windows;
    using System.Windows.Controls;
    using System.Windows.Documents;
    using System.Windows.Input;
    using System.Windows.Media.Animation;

    using Agent.Interaction.Desktop.ApplicationReader;
    using Agent.Interaction.Desktop.Settings;

    using Pointel.Configuration.Manager.Common;
    using System.Windows.Interop;
    using System.Runtime.InteropServices;

    /// <summary>
    /// Interaction logic for Skills.xaml
    /// </summary>
    public partial class Skills : Window
    {
        #region Fields

        private bool isSkillFocued = false;
        private Datacontext _dataContext = Datacontext.GetInstance();
        private Pointel.Logger.Core.ILog _logger = Pointel.Logger.Core.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType,
            "AID");
        private const int GWL_STYLE = -16; //WPF's Message code for Title Bar's Style 
        private const int WS_SYSMENU = 0x80000; //WPF's Message code for System Menu
        [DllImport("user32.dll", SetLastError = true)]
        private static extern int GetWindowLong(IntPtr hWnd, int nIndex);
        [DllImport("user32.dll")]
        private static extern int SetWindowLong(IntPtr hWnd, int nIndex, int dwNewLong);

        #endregion Fields

        #region Constructors

        public Skills()
        {
            InitializeComponent();
        }

        #endregion Constructors

        #region Methods

        public void Blink()
        {
            this.Topmost = true;
            BeginStoryboard((Storyboard)FindResource("BlinkBorder"));
        }

        private void AutoComplete_GotFocus(object sender, RoutedEventArgs e)
        {
            try
            {
                if (isSkillFocued == false)
                {
                    AutoCompleteBox autoBox = sender as AutoCompleteBox;
                    if (autoBox == null)
                        return;
                    TextBox textBox = autoBox.Template.FindName("Text", autoBox) as TextBox;
                    if (textBox != null)
                        textBox.Focus();
                    isSkillFocued = true;
                }
            }
            catch (Exception generalException)
            {
                _logger.Error("Skills:AutoComplete_GotFocus():" + generalException.ToString());
            }
        }

        /// <summary>
        /// Handles the Click event of the btnCancel control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="System.Windows.RoutedEventArgs"/> instance containing the event data.</param>
        private void btnCancel_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }

        /// <summary>
        /// Handles the Click event of the btnOk control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="System.Windows.RoutedEventArgs"/> instance containing the event data.</param>
        private void btnOk_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                Int32 EnteredIntValue = 0;
                if (cmbSkillLevel.Text == string.Empty && cmbSkillName.Text == string.Empty)
                {
                    txtError.Text = "Enter the Skill Name and Level for the Agent";
                    isSkillFocued = false;
                    cmbSkillName.Focus();
                    errorRowHeight.Height = GridLength.Auto;
                }
                else if (cmbSkillName.Text == string.Empty)
                {
                    txtError.Text = "Enter the Skill Name for the Agent";
                    isSkillFocued = false;
                    cmbSkillName.Focus();
                    errorRowHeight.Height = GridLength.Auto;
                }
                else if (cmbSkillLevel.Text == string.Empty)
                {
                    txtError.Text = "Enter the Skill Level for the Agent";
                    isSkillFocued = false;
                    cmbSkillLevel.Focus();
                    errorRowHeight.Height = GridLength.Auto;
                }
                else if (!(_dataContext.LoadAllSkills.FindIndex(x => x.Equals(cmbSkillName.Text, StringComparison.OrdinalIgnoreCase)) != -1))
                {
                    txtError.Text = "Enter the Valid Skill Name for the Agent";
                    isSkillFocued = false;
                    cmbSkillName.Focus();
                    errorRowHeight.Height = GridLength.Auto;
                }
                else if (!_dataContext.IsEditSkill &&
                    _dataContext.MySkills.Any(x => x.SkillName.Equals(cmbSkillName.Text, StringComparison.OrdinalIgnoreCase)))
                {
                    txtError.Text = "The entered skill is already present for the Agent";
                    isSkillFocued = false;
                    cmbSkillName.Focus();
                    errorRowHeight.Height = GridLength.Auto;
                }
                else
                {
                    Int32 skillValue;
                    try
                    {
                        skillValue = Convert.ToInt32(cmbSkillLevel.Text);
                        if (skillValue > 2000000000)
                        {
                            txtError.Text = "Skill Level should be between 0 to 2000000000";
                            isSkillFocued = false;
                            cmbSkillLevel.Focus();
                            errorRowHeight.Height = GridLength.Auto;
                        }
                        else
                        {
                            if (!_dataContext.IsEditSkill)//Adding a skill
                            {
                                Int32 skillLevel = Convert.ToInt32(cmbSkillLevel.Text);
                                if (!_dataContext.SkillLevelSource.Contains(skillLevel))
                                    _dataContext.SkillLevelSource.Add(skillLevel);
                                OutputValues outputValue = ComClass.AddUpdateSkillToAgent("Add", _dataContext.UserName,
                                    _dataContext.LoadAllSkills.Find(s => s.Equals(cmbSkillName.Text, StringComparison.OrdinalIgnoreCase)), skillLevel);
                                if (outputValue.MessageCode == "200")
                                {
                                    this.Close();
                                }
                                else
                                {
                                    txtError.Text = outputValue.Message;
                                    errorRowHeight.Height = GridLength.Auto;
                                }
                            }
                            else
                            {
                                Int32 skillLevel = Convert.ToInt32(cmbSkillLevel.Text);
                                if (!_dataContext.SkillLevelSource.Contains(skillLevel))
                                    _dataContext.SkillLevelSource.Add(skillLevel);
                                Pointel.Configuration.Manager.Common.OutputValues outputValue = ComClass.AddUpdateSkillToAgent("Edit", _dataContext.UserName,
                                    _dataContext.LoadAllSkills.Find(s => s.Equals(cmbSkillName.Text, StringComparison.OrdinalIgnoreCase)), skillLevel);
                                if (outputValue.MessageCode == "200")
                                    this.Close();
                                else
                                {
                                    txtError.Text = outputValue.Message;
                                    errorRowHeight.Height = GridLength.Auto;
                                }
                            }
                        }
                    }
                    catch (Exception ex)
                    {
                        txtError.Text = "Skill Level should be between 0 to 2000000000";
                        errorRowHeight.Height = GridLength.Auto;
                    }
                    //bool isIntOrNot = Int32.TryParse(cmbSkillLevel.Text, out EnteredIntValue);
                    //if (!isIntOrNot)
                    //{
                    //    txtError.Text = "Skill Level must be numerals";
                    //    errorRowHeight.Height = GridLength.Auto;
                    //}
                }
            }
            catch (Exception commonException)
            {
                _logger.Error("Skills:btnOk_click:" + commonException.ToString());
            }
        }

        /// <summary>
        /// Handles the PreviewKeyUp event of the cmbSkillLevel control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="KeyEventArgs"/> instance containing the event data.</param>
        private void cmbSkillLevel_PreviewKeyUp(object sender, KeyEventArgs e)
        {
            errorRowHeight.Height = new GridLength(0);
            if (e.Key == Key.Enter)
                btnOk_Click(null, null);
        }

        /// <summary>
        /// Handles the SelectionChanged event of the cmbSkillName control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="System.Windows.Controls.SelectionChangedEventArgs"/> instance containing the event data.</param>
        private void cmbSkillName_SelectionChanged(object sender, System.Windows.Controls.SelectionChangedEventArgs e)
        {
            var currentSkill = sender as AutoCompleteBox;
            if (currentSkill != null && _dataContext.LoadAllSkills.FindIndex(x => x.Equals(currentSkill.Text, StringComparison.OrdinalIgnoreCase)) != -1)
            {
                btnOk.IsEnabled = true;
            }
            else
            {
                btnOk.IsEnabled = false;
            }
        }

        /// <summary>
        /// Handles the KeyDown event of the cmbSkillValue control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="System.Windows.Input.KeyEventArgs"/> instance containing the event data.</param>
        private void cmbSkillValue_KeyDown(object sender, KeyEventArgs e)
        {
            errorRowHeight.Height = new GridLength(0);
            if ((Keyboard.Modifiers & ModifierKeys.Shift) == ModifierKeys.Shift)
            {
                e.Handled = true;
            }
            else
            {
                switch (e.Key)
                {
                    case Key.D0:
                    case Key.D1:
                    case Key.D2:
                    case Key.D3:
                    case Key.D4:
                    case Key.D5:
                    case Key.D6:
                    case Key.D7:
                    case Key.D8:
                    case Key.D9:
                    case Key.NumLock:
                    case Key.NumPad0:
                    case Key.NumPad1:
                    case Key.NumPad2:
                    case Key.NumPad3:
                    case Key.NumPad4:
                    case Key.NumPad5:
                    case Key.NumPad6:
                    case Key.NumPad7:
                    case Key.NumPad8:
                    case Key.NumPad9:
                    case Key.Back:
                    case Key.Delete:
                    case Key.Left:
                    case Key.Right:
                    case Key.End:
                    case Key.Home:
                    case Key.Prior:
                    case Key.Next:
                    case Key.LeftShift:
                    case Key.RightShift:
                    case Key.Tab:
                        e.Handled = false;
                        break;

                    default:
                        e.Handled = true;
                        break;
                }
                while (cmbSkillLevel.Text.Length > 9 && e.Key != Key.Back && e.Key != Key.Delete
                    && e.Key != Key.Left && e.Key != Key.Right
                    && e.Key != Key.End && e.Key != Key.Home
                    && e.Key != Key.Prior && e.Key != Key.Next && e.Key != Key.Tab && e.Key != Key.LeftShift && e.Key != Key.RightShift)
                {
                    e.Handled = true;
                    break;
                }
            }
        }

        /// <summary>
        /// Handles the MouseLeftButtonDown event of the lblTitle control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="System.Windows.Input.MouseButtonEventArgs"/> instance containing the event data.</param>
        private void lblTitle_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            this.DragMove();
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
        /// Previews the key up.
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="e">The <see cref="System.Windows.Input.KeyEventArgs"/> instance containing the event data.</param>
        private void PreviewKeyUp(object sender, System.Windows.Input.KeyEventArgs e)
        {
            errorRowHeight.Height = new GridLength(0);
            if (e.Key == Key.Enter)
            {
                isSkillFocued = false;
                cmbSkillLevel.Focus();
                //FocusNavigationDirection focusDirection = FocusNavigationDirection.Next;

                //// MoveFocus takes a TraveralReqest as its argument.
                //TraversalRequest request = new TraversalRequest(focusDirection);

                //// Gets the element with keyboard focus.
                //UIElement elementWithFocus = Keyboard.FocusedElement as UIElement;

                //// Change keyboard focus.
                //if (elementWithFocus != null)
                //{
                //    if (elementWithFocus.MoveFocus(request)) e.Handled = true;
                //}
            }
            //cmbSkillLevel.Focus();
            else
            {
                var currentSkill = sender as AutoCompleteBox;
                if (currentSkill != null && _dataContext.LoadAllSkills.FindIndex(x => x.Equals(currentSkill.Text, StringComparison.OrdinalIgnoreCase)) != -1)
                {
                    //_dataContext.MySkills.Any(x => x.SkillName.Equals(currentSkill.Text, StringComparison.OrdinalIgnoreCase));
                    //var skills = _dataContext.MySkills.Where(x => x.SkillName.Equals(currentSkill.Text, StringComparison.OrdinalIgnoreCase));
                    if (!_dataContext.MySkills.Any(x => x.SkillName.Equals(currentSkill.Text, StringComparison.OrdinalIgnoreCase)))
                        btnOk.IsEnabled = true;
                }
                else
                    btnOk.IsEnabled = false;
            }
        }

        private void Storyboard_Completed(object sender, EventArgs e)
        {
            this.Topmost = false;
        }

        private void UserSkills_Loaded(object sender, RoutedEventArgs e)
        {
            try
            {
                var hwnd = new WindowInteropHelper(this).Handle;
                SetWindowLong(hwnd, GWL_STYLE, GetWindowLong(hwnd, GWL_STYLE) & ~WS_SYSMENU);
                _dataContext.SkillLevelSource.Sort();
                cmbSkillLevel.ItemsSource = _dataContext.SkillLevelSource;
                if (!_dataContext.IsEditSkill)//Adding a skill
                {
                    lblTitle.Content = "Add Skill";
                    cmbSkillName.Focus();
                    List<string> tempList = new List<string>();
                    foreach (string value in _dataContext.LoadAllSkills)
                    {
                        tempList.Add(value);
                    }
                    foreach (var item in _dataContext.MySkills)
                    {
                        if (tempList.Contains(item.SkillName))
                        {
                            tempList.Remove(item.SkillName);
                        }
                    }
                    cmbSkillName.ItemsSource = tempList;
                    cmbSkillName.IsEnabled = true;
                    btnOk.IsEnabled = false;
                    //cmbSkillName.SelectedIndex = 0;
                }
                else//Editing a skill
                {
                    lblTitle.Content = "Edit Skill";
                    cmbSkillLevel.Focus();
                    List<string> skillNamesList = new List<string>();
                    foreach (string key in _dataContext.EditingSkill.Keys)
                    {
                        skillNamesList.Add(key);
                        _dataContext.CurrentEditingSkill = key;
                        cmbSkillLevel.Text = _dataContext.EditingSkill[key].ToString();
                        cmbSkillName.ItemsSource = skillNamesList;
                        cmbSkillName.Text = key;
                        break;
                    }
                    //cmbSkillName.SelectedIndex = 0;
                    cmbSkillName.IsEnabled = false;
                    btnOk.IsEnabled = true;
                }
            }
            catch (Exception _generalException)
            {
                _logger.Error("Error occurred as " + _generalException.Message);
            }
        }

        #endregion Methods

        private void UserSkills_PreviewKeyDown(object sender, KeyEventArgs e)
        {
            if ((Keyboard.IsKeyDown(Key.LeftAlt) || Keyboard.IsKeyDown(Key.RightAlt)) && Keyboard.IsKeyDown(Key.F4))
                e.Handled = true;

        }


    }
}