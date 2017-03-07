namespace Agent.Interaction.Desktop
{
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using System.Collections.ObjectModel;
    using System.Configuration;
    using System.Linq;
    using System.Text;
    using System.Threading;
    using System.Windows;
    using System.Windows.Controls;
    using System.Windows.Data;
    using System.Windows.Input;
    using System.Windows.Media;
    using System.Windows.Media.Imaging;

    using Agent.Interaction.Desktop.ApplicationReader;
    using Agent.Interaction.Desktop.Helpers;
    using Agent.Interaction.Desktop.Settings;

    using Genesyslab.Platform.Commons.Collections;

    using Pointel.Configuration.Manager;
    using Pointel.Softphone.Voice.Core;

    /// <summary>
    /// Interaction logic for DialPad.xaml
    /// </summary>
    public partial class DialPad : UserControl
    {
        #region Fields

        private Hashtable _agentContact = new Hashtable();
        private Hashtable _applcationContact = new Hashtable();
        private ConfigContainer _configContainer = ConfigContainer.Instance();
        private Datacontext _dataContext = Datacontext.GetInstance();
        private Hashtable _groupContact = new Hashtable();
        private bool _isDialClicked = false;
        private bool _isGlobalContactChecked = false;
        private bool _isGroupContactChecked = false;
        private bool _isLocalContactChecked = false;
        private Pointel.Logger.Core.ILog _logger = Pointel.Logger.Core.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType,
            "AID");
        private ContextMenu _phoneBookMenu = new ContextMenu();
        private string _phoneBookText = string.Empty;
        private string _searchContact = string.Empty;
        private string _searchText = string.Empty;
        private StringBuilder _typednumber = new StringBuilder();
        private Agent.Interaction.Desktop.Settings.Datacontext.DialPadType _windowType;

        #endregion Fields

        #region Constructors

        public DialPad(Agent.Interaction.Desktop.Settings.Datacontext.DialPadType windowType)
        {
            InitializeComponent();
            _dataContext.IsPhoneBookEnabled = _configContainer.GetAsBoolean("voice.enable.phone-book") ? Visibility.Visible : Visibility.Collapsed;
            _windowType = windowType;
            txtNumbers.Focus();
            _dataContext.Contacts.Clear();
            _dataContext.ContactsFilter.Clear();
            this.DataContext = Datacontext.GetInstance();

            //Code added by Manikandan on 18-03-2014 to implement the contextmenu in phonebook
            MenuItem mnuItem = new MenuItem();
            mnuItem.Header = "Dial";
            mnuItem.VerticalContentAlignment = VerticalAlignment.Center;
            mnuItem.Height = 18;
            mnuItem.Background = System.Windows.Media.Brushes.Transparent;
            mnuItem.Icon = new System.Windows.Controls.Image { Height = 12, Width = 12, Source = new BitmapImage(new Uri("/Agent.Interaction.Desktop;component/Images/Voice.Short.png", UriKind.Relative)) };
            mnuItem.Click += new RoutedEventHandler(mnuItem_Click);
            _phoneBookMenu.Items.Add(mnuItem);
            //End

            _dataContext.DialedNumbers = string.Empty;
            if (_dataContext.DialedNumbers.Length >= 9 && _dataContext.ModifiedTextSize != 0)
            {
                txtNumbers.FontSize = _dataContext.ModifiedTextSize;
            }
            if (_configContainer.AllKeys.Contains("voice.enable.phonebook.double-click-to-call") && ((string)_configContainer.GetValue("voice.enable.phonebook.double-click-to-call")).ToLower().Equals("true"))
            {
                dgvContact.MouseDoubleClick += new MouseButtonEventHandler(dgvContact_MouseDoubleClick);
            }

            //if (_dataContext.LoadAllSkills.Count > 0)
            //    cmbSkills.SelectedIndex = 0;

            if (windowType == Datacontext.DialPadType.Normal)
            {
                if (SelectionTab.Items.Contains(tabRequeue))
                    SelectionTab.Items.Remove(tabRequeue);
            }
            else
            {
                if (!_configContainer.AllKeys.Contains("voice.enable.requeue") || !((string)_configContainer.GetValue("voice.enable.requeue")).ToLower().Equals("true"))
                {
                    if (SelectionTab.Items.Contains(tabRequeue))
                    {
                        SelectionTab.Items.Remove(tabRequeue);
                    }
                }
                else
                {
                    FillRequeueData();
                    #region Old Code
                    //if (!_configContainer.AllKeys.Contains("voice.enable.requeue-single-step") && !_configContainer.AllKeys.Contains("voice.enable.requeue-dual-step"))
                    //{
                    //    if (windowType == Datacontext.DialPadType.Conference && _dataContext.UserSetConfType != Datacontext.ConsultType.DualStep)
                    //    {
                    //        if (SelectionTab.Items.Contains(tabRequeue))
                    //            SelectionTab.Items.Remove(tabRequeue);
                    //    }
                    //    else if (windowType == Datacontext.DialPadType.Transfer && _dataContext.UserSetTransType != Datacontext.ConsultType.DualStep)
                    //    {
                    //        if (SelectionTab.Items.Contains(tabRequeue))
                    //            SelectionTab.Items.Remove(tabRequeue);
                    //    }
                    //}
                    //else
                    //{
                    //    if (windowType == Datacontext.DialPadType.Conference)
                    //    {
                    //        if (_dataContext.UserSetConfType == Datacontext.ConsultType.OneStep)
                    //        {
                    //            if (!_configContainer.AllKeys.Contains("voice.enable.requeue-single-step") || !((string)_configContainer.GetValue("voice.enable.requeue-single-step")).ToLower().Equals("true"))
                    //            {
                    //                if (SelectionTab.Items.Contains(tabRequeue))
                    //                    SelectionTab.Items.Remove(tabRequeue);
                    //            }
                    //            else
                    //                _isNeedToLoadRequeue = true;

                    //        }
                    //        else if (_dataContext.UserSetConfType == Datacontext.ConsultType.DualStep)
                    //        {
                    //            btnClick.IsEnabled = false;
                    //            _isNeedToLoadRequeue = true;
                    //            //if (_dataContext.IsRequeueDualStepEnabled == null || _dataContext.IsRequeueDualStepEnabled == false)
                    //            //    if (SelectionTab.Items.Contains(tabRequeue))
                    //            //        SelectionTab.Items.Remove(tabRequeue);
                    //        }
                    //    }
                    //    else if (windowType == Datacontext.DialPadType.Transfer)
                    //    {
                    //        if (_dataContext.UserSetTransType == Datacontext.ConsultType.OneStep)
                    //        {
                    //            if (!_configContainer.AllKeys.Contains("voice.enable.requeue-single-step") || !((string)_configContainer.GetValue("voice.enable.requeue-single-step")).ToLower().Equals("true"))
                    //            {
                    //                if (SelectionTab.Items.Contains(tabRequeue))
                    //                    SelectionTab.Items.Remove(tabRequeue);
                    //            }
                    //            else
                    //                _isNeedToLoadRequeue = true;
                    //        }
                    //        else if (_dataContext.UserSetTransType == Datacontext.ConsultType.DualStep)
                    //        {
                    //            btnClick.IsEnabled = false;
                    //            _isNeedToLoadRequeue = true;
                    //            //if (_dataContext.IsRequeueDualStepEnabled == null || _dataContext.IsRequeueDualStepEnabled == false)
                    //            //    if (SelectionTab.Items.Contains(tabRequeue))
                    //            //        SelectionTab.Items.Remove(tabRequeue);
                    //        }
                    //    }
                    //}
                    #endregion Old Code
                }
            }
        }

        #endregion Constructors

        #region Delegates

        public delegate void FireBackNum(string value);

        public delegate void LoadContacts(object contacts);

        #endregion Delegates

        #region Events

        public event FireBackNum eventFireBackNum;

        #endregion Events

        #region Methods

        public static T FindAncestor<T>(DependencyObject current)
            where T : DependencyObject
        {
            current = VisualTreeHelper.GetParent(current);

            while (current != null)
            {
                if (current is T)
                {
                    return (T)current;
                }
                current = VisualTreeHelper.GetParent(current);
            };
            return null;
        }

        /// <summary>
        /// Keyboardvalues the specified key.
        /// </summary>
        /// <param name="key">The key.</param>
        /// <returns></returns>
        public string Keyboardvalue(Key key)
        {
            var value = string.Empty;
            if ((Keyboard.Modifiers & ModifierKeys.Shift) == ModifierKeys.Shift)
            {
                switch (key)
                {
                    case Key.D3:
                        value = "#";
                        break;

                    case Key.D8:
                        value = "*";
                        break;
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

        private void btn_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                string _requeueDn = string.Empty;
                if (_configContainer.AllKeys.Contains("voice.requeue.route-dn"))
                    _requeueDn = _configContainer.GetAsString("voice.requeue.route-dn");
                if (_requeueDn == _dataContext.ThisDN)
                    DisplayOwnDNCallErrorMessage();
                else if (!string.IsNullOrEmpty(_requeueDn))
                {
                    var _softphone = new SoftPhone();
                    var userData = new KeyValueCollection();
                    userData = BuildRequeueUserData(userData);

                    if (_windowType == Datacontext.DialPadType.Conference)
                    {
                        userData.Add("OperationMode", "Conference");
                        if (_configContainer.GetAsBoolean("voice.enable.requeue.two-step-conference", false))
                        {
                            _softphone.InitiateConference(_requeueDn, "", userData, null, null);
                            _dataContext.IsInitiateTransClicked = false;
                            _dataContext.IsInitiateConfClicked = true;
                        }
                        else
                            _softphone.SingleStepConference(_requeueDn, "", userData);
                    }
                    else if (_windowType == Datacontext.DialPadType.Transfer)
                    {
                        userData.Add("OperationMode", "Transfer");
                        if (_configContainer.GetAsBoolean("voice.enable.requeue.two-step-transfer", false))
                        {
                            _softphone.InitiateTransfer(_requeueDn, "", userData);
                            _dataContext.IsInitiateTransClicked = true;
                            _dataContext.IsInitiateConfClicked = false;
                        }
                        else
                            _softphone.MuteTransfer(_requeueDn, "", userData, null, null);
                    }

                    _dataContext.cmshow.StaysOpen = false;
                    _dataContext.cmshow.IsOpen = false;
                }
            }
            catch (Exception ex)
            {
                _logger.Error((ex.InnerException == null) ? ex.Message : ex.InnerException.ToString());
            }
        }

        private KeyValueCollection BuildRequeueUserData(KeyValueCollection keyValueCollection)
        {
            var skillKey = _configContainer.GetAsString("voice.requeue.attach-data.skill.key-name");
            if (skillKey == "No key found" || string.IsNullOrEmpty(skillKey))
                skillKey = "SkillSet";
            var selectedItem = cmbSkills.SelectedItem;
            if (selectedItem is RequeueSkills)
                selectedItem = (selectedItem as RequeueSkills).Value;
            keyValueCollection.Add(skillKey, selectedItem);

            var commentkey = _configContainer.GetAsString("voice.requeue.attach-data.comments.key-name");
            if (commentkey == "No key found" || string.IsNullOrEmpty(commentkey))
                commentkey = "Comments";
            keyValueCollection.Add(commentkey, _dataContext.RequeueComments);

            keyValueCollection.Add("FromWhom", _dataContext.UserName);
            keyValueCollection.Add("Requeue", "Requeued");

            return keyValueCollection;
        }

        /// <summary>
        /// Handles the Checked event of the ChkAgentLevel control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="RoutedEventArgs"/> instance containing the event data.</param>
        private void ChkAgentLevel_Checked(object sender, RoutedEventArgs e)
        {
            Thread contactControl;
            try
            {
                //Code added by Manikandan on 26-Nov-2013
                //if (_dataContext.IsAnnexContactsEnabled)
                if (_configContainer.AllKeys.Contains("voice.enable.read-agent-annex-value") &&
                        ((string)_configContainer.GetValue("voice.enable.read-agent-annex-value")).ToLower().Equals("true"))
                {
                    //if (_configContainer.AllKeys.Contains("AgentContacts") && _configContainer.GetValue("AgentContacts") != null)
                    //        _dataContext.AnnexContacts = _configContainer.GetValue("AgentContacts");
                    _agentContact.Clear();
                    if (_dataContext.AnnexContacts.Count > 0)
                    {
                        foreach (string key in _dataContext.AnnexContacts.Keys)
                        {
                            if (!_agentContact.ContainsKey(key))
                            {
                                _agentContact.Add(key, _dataContext.AnnexContacts[key].ToString());
                            }
                        }
                    }
                }
                else
                {
                    XMLHandler xmlHandler = new XMLHandler();
                    Dictionary<string, string> xmlContacts = xmlHandler.LoadXmlContacts(_dataContext.SpeedDialXMLFile);
                    _agentContact.Clear();
                    if (xmlContacts != null && xmlContacts.Count > 0)
                    {
                        foreach (string key in xmlContacts.Keys)
                        {
                            if (!_agentContact.ContainsKey(key))
                            {
                                _agentContact.Add(key, xmlContacts[key].ToString());
                            }
                        }
                    }
                }
                //code Ended
                //agentContact = (Hashtable)_dataContext.ConfiguredSpeedDial;
                if (!_agentContact.ContainsKey("type"))
                {
                    _agentContact.Add("type", "11");
                }
                else
                {
                    _agentContact["type"] = "11";
                }
                contactControl = new Thread(new ParameterizedThreadStart(ContactController));
                contactControl.Start((object)_agentContact);
                _isLocalContactChecked = true;
            }
            catch (Exception commonException)
            {
                _logger.Error("chkAgentLevel_CheckedChanged : Error occurred while getting contact details from agent level " +
                    commonException.ToString());
            }
            finally
            {
                contactControl = null;
                //GC.Collect();
            }
        }

        /// <summary>
        /// Handles the Unchecked event of the ChkAgentLevel control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="RoutedEventArgs"/> instance containing the event data.</param>
        private void ChkAgentLevel_Unchecked(object sender, RoutedEventArgs e)
        {
            Thread contactControl;
            try
            {
                _agentContact["type"] = 21;
                contactControl = new Thread(new ParameterizedThreadStart(ContactController));
                contactControl.Start((object)_agentContact);
                _isLocalContactChecked = false;
            }
            catch { }
        }

        /// <summary>
        /// Handles the Checked event of the ChkApplicationLevel control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="RoutedEventArgs"/> instance containing the event data.</param>
        private void ChkApplicationLevel_Checked(object sender, RoutedEventArgs e)
        {
            Thread contactControl;
            try
            {
                _applcationContact = (Hashtable)_dataContext.HshApplicationLevel;
                if (!_applcationContact.ContainsKey("type"))
                {
                    _applcationContact.Add("type", 12);
                }
                else
                {
                    _applcationContact["type"] = 12;
                }
                contactControl = new Thread(new ParameterizedThreadStart(ContactController));
                contactControl.Start((object)_applcationContact);
                _isGlobalContactChecked = true;
            }
            catch (Exception commonException)
            {
                _logger.Error("chkApplicationLevel_CheckedChanged : Error occurred while getting contact details from application level " +
                    commonException.ToString());
            }
            finally
            {
                contactControl = null;
            }
        }

        /// <summary>
        /// Handles the Unchecked event of the ChkApplicationLevel control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="RoutedEventArgs"/> instance containing the event data.</param>
        private void ChkApplicationLevel_Unchecked(object sender, RoutedEventArgs e)
        {
            Thread contactControl;
            try
            {
                _applcationContact["type"] = 22;
                contactControl = new Thread(new ParameterizedThreadStart(ContactController));
                contactControl.Start((object)_applcationContact);
                _isGlobalContactChecked = false;
            }
            catch
            {
            }
        }

        /// <summary>
        /// Handles the Checked event of the ChkGroupLevel control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="RoutedEventArgs"/> instance containing the event data.</param>
        private void ChkGroupLevel_Checked(object sender, RoutedEventArgs e)
        {
            Thread contactControl;
            try
            {
                if (Datacontext.hshLoadGroupContact != null && !Datacontext.hshLoadGroupContact.Count.Equals(0))
                {
                    //if (_configContainer.AllKeys.Contains("GroupContacts"))
                    //    if (_configContainer.GetValue("GroupContacts") != null)
                    //        _groupContact = _configContainer.GetValue("GroupContacts");
                    //    if (_groupContact != null && _groupContact.Count > 0)
                    //    {
                    //        foreach (string key in _groupContact.Keys)
                    //        {
                    //            if (!_groupContact.ContainsKey(key))
                    //            {
                    //                _groupContact.Add(key, _groupContact[key].ToString());
                    //            }
                    //            if (!Datacontext.hshLoadGroupContact.ContainsKey(key))
                    //            {
                    //                Datacontext.hshLoadGroupContact.Add(key, _groupContact[key].ToString());
                    //            }
                    //        }
                    //    }
                    //    _groupContact = Datacontext.hshLoadGroupContact;
                    //}
                    //else
                    _groupContact = (Hashtable)Datacontext.hshLoadGroupContact;
                    if (!_groupContact.ContainsKey("type"))
                    {
                        _groupContact.Add("type", 13);
                    }
                    else
                    {
                        _groupContact["type"] = 13;
                    }
                    contactControl = new Thread(new ParameterizedThreadStart(ContactController));
                    contactControl.Start((object)_groupContact);
                    _isGroupContactChecked = true;
                }
            }
            catch (Exception commonException)
            {
                _logger.Error("chkGroup_CheckedChanged : Error occurred while getting contact details from agent group level " +
                 commonException.ToString());
            }
            finally
            {
                contactControl = null;
                //GC.Collect();
            }
        }

        /// <summary>
        /// Handles the Unchecked event of the ChkGroupLevel control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="RoutedEventArgs"/> instance containing the event data.</param>
        private void ChkGroupLevel_Unchecked(object sender, RoutedEventArgs e)
        {
            Thread contactControl;
            try
            {
                _groupContact["type"] = 23;
                contactControl = new Thread(new ParameterizedThreadStart(ContactController));
                contactControl.Start((object)_groupContact);
                _isGroupContactChecked = false;
            }
            catch { }
        }

        private void cmbSkills_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            string s = e.AddedItems[0] is RequeueSkills ? (e.AddedItems[0] as RequeueSkills).Skill : e.AddedItems[0].ToString();
            if (cmbSkills.SelectedIndex == 0 || s == "None")
                btnClick.IsEnabled = false;
            else
                btnClick.IsEnabled = true;
        }

        private void ContactController(object contacts)
        {
            try
            {
                System.Windows.Application.Current.Dispatcher.Invoke((Action)(delegate
                {
                    if (contacts != null)
                    {
                        Hashtable contact = (Hashtable)contacts;

                        if (contact.ContainsKey("type"))
                        {
                            //Add agent/application/global contact in Grid

                            if (string.Compare(contact["type"].ToString(), "11", true) == 0 ||
                                string.Compare(contact["type"].ToString(), "12", true) == 0 ||
                                string.Compare(contact["type"].ToString(), "13", true) == 0)
                            {
                                IDictionaryEnumerator keys = contact.GetEnumerator();
                                while (keys.MoveNext())
                                {
                                    if (keys.Key.ToString() != "type")
                                    {
                                        _dataContext.Contacts.Add(new Contacts(keys.Key.ToString(), keys.Value.ToString(), contact["type"].ToString()));
                                    }
                                }
                                if (string.IsNullOrEmpty(txtContactSearch.Text))
                                {
                                    dgvContact.ItemsSource = _dataContext.Contacts;
                                    dgvContact.UpdateLayout();
                                    if (dgvContact.Items.Count > 1)
                                        dgvContact.ScrollIntoView(dgvContact.Items[dgvContact.Items.Count - 1]);
                                    lblStatus.Content = dgvContact.Items.Count.ToString() + " contacts available ";
                                    _dataContext.ContactsFilter.Clear();
                                }
                                else
                                {
                                    ContactSearch(txtContactSearch.Text);
                                }
                            }
                            else if (string.Compare(contact["type"].ToString(), "21", true) == 0 ||
                                string.Compare(contact["type"].ToString(), "22", true) == 0 ||
                                string.Compare(contact["type"].ToString(), "23", true) == 0)
                            {
                                ObservableCollection<IContacts> temp = null;
                                ObservableCollection<IContacts> temp1 = null;
                                if (string.IsNullOrEmpty(txtContactSearch.Text))
                                {
                                    temp = _dataContext.Contacts;
                                    temp1 = _dataContext.ContactsFilter;
                                }
                                else
                                {
                                    temp = _dataContext.ContactsFilter;
                                    temp1 = _dataContext.Contacts;
                                }
                                if (temp.Count > 0)
                                {
                                    if (string.Compare(contact["type"].ToString(), "21", true) == 0)
                                    {
                                        var toRemove = temp.Where(x => x.Type == "11").ToList();
                                        foreach (var item in toRemove)
                                            temp.Remove(item);
                                        var toRemove1 = temp1.Where(x => x.Type == "11").ToList();
                                        foreach (var item in toRemove1)
                                            temp1.Remove(item);
                                    }
                                    else if (string.Compare(contact["type"].ToString(), "22", true) == 0)
                                    {
                                        var toRemove = temp.Where(x => x.Type == "12").ToList();
                                        foreach (var item in toRemove)
                                            temp.Remove(item);
                                        var toRemove1 = temp1.Where(x => x.Type == "12").ToList();
                                        foreach (var item in toRemove1)
                                            temp1.Remove(item);
                                    }
                                    else if (string.Compare(contact["type"].ToString(), "23", true) == 0)
                                    {
                                        var toRemove = temp.Where(x => x.Type == "13").ToList();
                                        foreach (var item in toRemove)
                                            temp.Remove(item);
                                        var toRemove1 = temp1.Where(x => x.Type == "13").ToList();
                                        foreach (var item in toRemove1)
                                            temp1.Remove(item);
                                    }
                                    dgvContact.ItemsSource = temp;
                                    dgvContact.UpdateLayout();
                                    if (dgvContact.Items.Count > 1)
                                        dgvContact.ScrollIntoView(dgvContact.Items[dgvContact.Items.Count - 1]);
                                }
                                if (string.IsNullOrEmpty(txtContactSearch.Text))
                                {
                                    lblStatus.Content = dgvContact.Items.Count.ToString() + " contacts available ";
                                }
                                else if (!string.IsNullOrEmpty(txtContactSearch.Text))
                                {
                                    lblStatus.Content = dgvContact.Items.Count.ToString() + " matches found ";
                                }
                            }
                        }
                    }
                    else
                    {
                        _logger.Info("No contacts available");
                    }
                }));
            }
            catch (ThreadAbortException threadException)
            {
                _logger.Error("DialPad:ContactController:" + threadException.ToString());
            }
            catch (Exception commonException)
            {
                _logger.Error("Error occurred while Add/Remove contacts from grid " + commonException.ToString());
            }
            finally
            {
            }
        }

        //Below code added for implement contact search
        //SMoorthy - 07-01-2014
        private void ContactSearch(object value)
        {
            string searchString = string.Empty;
            try
            {
                System.Windows.Application.Current.Dispatcher.Invoke((Action)(delegate
                {
                    if (value != null)
                    {
                        searchString = Convert.ToString(value).ToLower();
                        _dataContext.ContactsFilter.Clear();
                        if (!string.IsNullOrEmpty(searchString))
                        {
                            for (int index = 0; index < _dataContext.Contacts.Count; index++)
                            {
                                if (_dataContext.Contacts[index].Name.ToString().ToLower().StartsWith(searchString))
                                {
                                    _dataContext.ContactsFilter.Add(_dataContext.Contacts[index]);
                                }
                            }
                            dgvContact.ItemsSource = _dataContext.ContactsFilter;
                            dgvContact.UpdateLayout();
                            if (dgvContact.Items.Count > 1)
                                dgvContact.ScrollIntoView(dgvContact.Items[dgvContact.Items.Count - 1]);
                        }
                        else if (string.IsNullOrEmpty(searchString))
                        {
                            dgvContact.ItemsSource = _dataContext.Contacts;
                            dgvContact.UpdateLayout();
                            if (dgvContact.Items.Count > 1)
                                dgvContact.ScrollIntoView(dgvContact.Items[dgvContact.Items.Count - 1]);
                        }

                        //Notify total contact
                        if (!string.IsNullOrEmpty(searchString))
                        {
                            lblStatus.Content = dgvContact.Items.Count.ToString() + " matches found ";
                        }
                        else if (string.IsNullOrEmpty(searchString))
                        {
                            lblStatus.Content = dgvContact.Items.Count.ToString() + " contacts available ";
                        }
                    }
                }));
            }
            catch (Exception commonException)
            {
                _logger.Error("Error occurred while searching contact in data grid " +
                    commonException.ToString());
            }
            finally
            {
            }
        }

        /// <summary>
        /// Handles the MouseDoubleClick event of the dgvContact control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="MouseButtonEventArgs"/> instance containing the event data.</param>
        private void dgvContact_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            if (dgvContact.SelectedIndex >= 0)
                Dial_Click(null, null);
        }

        /// <summary>
        /// Handles the SelectionChanged event of the dgvContact control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="SelectionChangedEventArgs"/> instance containing the event data.</param>
        private void dgvContact_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            try
            {
                if (dgvContact.SelectedItem is Contacts)
                {
                    Contacts temp = dgvContact.SelectedItem as Contacts;
                    if (!_dataContext.isOnCall)
                        _phoneBookText = temp.Number.ToString();
                    else if (temp.Number.ToString().Length <= _dataContext.ConsultDialDigits)
                        _phoneBookText = temp.Number.ToString();
                    //added after removing textbox and dial button in phone book
                    txtNumbersBook_TextChanged(null, null);
                }
            }
            catch
            {
            }
        }

        /// <summary>
        /// Handles the Click event of the Dial control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="RoutedEventArgs"/> instance containing the event data.</param>
        private void Dial_Click(object sender, RoutedEventArgs e)
        {
            if (_windowType == Datacontext.DialPadType.Transfer)
            {
                if (_dataContext.UserSetTransType == Datacontext.ConsultType.OneStep)
                {
                    _dataContext.IsInitiateConfClicked = false;
                    _dataContext.IsInitiateTransClicked = true;
                }
                if (_dataContext.UserSetTransType == Datacontext.ConsultType.DualStep)
                {
                    _dataContext.IsInitiateConfClicked = false;
                    _dataContext.IsInitiateTransClicked = true;
                }
            }
            if (_windowType == Datacontext.DialPadType.Conference)
            {
                if (_dataContext.UserSetConfType == Datacontext.ConsultType.OneStep)
                {
                    _dataContext.IsInitiateConfClicked = true;
                    _dataContext.IsInitiateTransClicked = false;
                }
                if (_dataContext.UserSetConfType == Datacontext.ConsultType.DualStep)
                {
                    _dataContext.IsInitiateConfClicked = true;
                    _dataContext.IsInitiateTransClicked = false;
                }
            }

            #region Old Code

            // if (_dataContext.Singleclick && !_dataContext.Dualclick)
            // {
            //     if (WindowType == Datacontext.DialPadType.Transfer)
            //     {
            //         _dataContext.UserSetTransType = Datacontext.ConsultType.OneStep;
            //         //Code Added for set initiate transfer is clicked - smoorthy
            //         //15-11-2013
            //         _dataContext.IsInitiateConfClicked = false;
            //         _dataContext.IsInitiateTransClicked = true;
            //         //end
            //     }
            //     else if (WindowType == Datacontext.DialPadType.Conference)
            //     {
            //         _dataContext.UserSetConfType = Datacontext.ConsultType.OneStep;
            //         //Code Added for set initiate conference is clicked - smoorthy
            //         //15-11-2013
            //         _dataContext.IsInitiateConfClicked = true;
            //         _dataContext.IsInitiateTransClicked = false;
            //         //end
            //     }
            //     _dataContext.Singleclick = false;
            // }
            //if (!_dataContext.Singleclick && _dataContext.Dualclick)
            //{
            //    if (WindowType == Datacontext.DialPadType.Transfer)
            //    {
            //        _dataContext.UserSetTransType = Datacontext.ConsultType.DualStep;
            //        //Code Added for set initiate transfer is clicked - smoorthy
            //        //15-11-2013
            //        _dataContext.IsInitiateConfClicked = false;
            //        _dataContext.IsInitiateTransClicked = true;
            //        //end
            //    }
            //    else if (WindowType == Datacontext.DialPadType.Conference)
            //    {
            //        _dataContext.UserSetConfType = Datacontext.ConsultType.DualStep;
            //        //Code Added for set initiate conference is clicked - smoorthy
            //        //15-11-2013
            //        _dataContext.IsInitiateConfClicked = true;
            //        _dataContext.IsInitiateTransClicked = false;
            //        //end
            //    }
            //    _dataContext.Dualclick = false;
            //}

            #endregion Old Code

            //if (_dataContext.ThisDN != _dataContext.DialedNumbers)
            //{
            if (_dataContext.IsInitiateTransClicked)
            {
                _dataContext.cmshow.IsOpen = false;
                _dataContext.IsTransDialPadOpen = false;
                eventFireBackNum.Invoke("transfer");
            }
            else if (_dataContext.IsInitiateConfClicked)
            {
                _dataContext.cmshow.IsOpen = false;
                _dataContext.IsConfDialPadOpen = false;
                eventFireBackNum.Invoke("conference");
            }
            else
            {
                _dataContext.cmshow.IsOpen = false;
                eventFireBackNum.Invoke("DIAL");
            }
            _isDialClicked = true;
            //}
            if (_dataContext.ThisDN != _dataContext.DialedNumbers)
            {
                _dataContext.UserSetConfType = Datacontext.ConsultType.None;
                _dataContext.UserSetTransType = Datacontext.ConsultType.None;
            }
        }

        private void DisplayOwnDNCallErrorMessage()
        {
            try
            {
                KeyValueCollection errorMessage = new KeyValueCollection();
                errorMessage.Add("IWS_Message",
                                "Voice (" + _dataContext.ThisDN + "@" + _dataContext.SwitchType.Name + ")");
                errorMessage.Add("IWS_Subject", "Call origination and destination are same");
                errorMessage.Add("IWS_Sender", "System");
                errorMessage.Add("IWS_Priority", "4");
                errorMessage.Add("IWS_MessageType", "System");
                errorMessage.Add("IWS_Date", DateTime.Now.ToString());
                errorMessage.Add("IWS_Topic", _dataContext.UserName);
            }
            catch (Exception ex)
            {
                _logger.Error(ex.Message.ToString());
            }
        }

        /// <summary>
        /// Handles the Click event of the Double control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="RoutedEventArgs"/> instance containing the event data.</param>
        private void Double_Click(object sender, RoutedEventArgs e)
        {
            _dataContext.Dualclick = true;
            _dataContext.Singleclick = false;
        }

        private void FillRequeueData()
        {
            try
            {
                var value = _configContainer.GetAsString("voice.requeue.attach-data.skill.object-name");
                if (value == "No key found" || string.IsNullOrEmpty(value))
                    value = "$AllSkills$";

                var list = (new RequeueSkillSet()).RetrieveRequeueSkillSet(value);
                System.Windows.Data.ListCollectionView requeuedata = null;
                if (list[0].Skill != "None")
                {
                    var defaultdata = new RequeueSkills();
                    defaultdata.Category = null;
                    defaultdata.Skill = "-- Select Re-Queue --";
                    defaultdata.Value = null;
                    list.Insert(0, defaultdata);
                    txt_Error.Visibility = Visibility.Collapsed;
                }
                else
                    txt_Error.Visibility = Visibility.Visible;
                requeuedata = new System.Windows.Data.ListCollectionView(list);
                if (value != "$AllSkills$")
                {
                    requeuedata.GroupDescriptions.Add(new PropertyGroupDescription("Category"));
                    requeuedata.SortDescriptions.Add(new System.ComponentModel.SortDescription("Skill", System.ComponentModel.ListSortDirection.Ascending));
                    requeuedata.SortDescriptions.Add(new System.ComponentModel.SortDescription("Category", System.ComponentModel.ListSortDirection.Ascending));
                }
                if (requeuedata != null)
                    _dataContext.RequeueData = requeuedata;
            }
            catch (Exception ex)
            {
                _logger.Error("Error occurred while filling the Requeue data as : " + ex.Message);
            }
        }

        /// <summary>
        /// Handles the Click event of the mnuItem control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="RoutedEventArgs"/> instance containing the event data.</param>
        private void mnuItem_Click(object sender, RoutedEventArgs e)
        {
            txtNumbersBook_TextChanged(null, null);
            Dial_Click(null, null);
        }

        /// <summary>
        /// Handles the Click event of the Number control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="RoutedEventArgs"/> instance containing the event data.</param>
        private void Number_Click(object sender, RoutedEventArgs e)
        {
            Button number = sender as Button;
            if (_dataContext.DialedNumbers.Length < _dataContext.MaxDialDigits)
            {
                if (_dataContext.DialedNumbers.Length > 9 && txtNumbers.FontSize > 26.75)
                {
                    _dataContext.ModifiedTextSize = txtNumbers.FontSize = txtNumbers.FontSize - 2.75;
                }
                else if (_dataContext.DialedNumbers.Length <= 9)
                {
                    if (txtNumbers.FontSize != 35)
                    {
                        txtNumbers.FontSize = 35;
                    }
                }
                _dataContext.DialedNumbers = _dataContext.DialedNumbers + number.Content.ToString();
                _typednumber.Append(number.Content.ToString());
            }

            Call.Focus();

            #region Old code

            //if (number.Content.ToString().Contains("*") || number.Content.ToString().Contains("#") || _dataContext.DialedNumbers.Contains("*")
            //    || _dataContext.DialedNumbers.Contains("#"))
            //{
            //    if (_dataContext.DialedNumbers.Length < _dataContext.MaxDialDigits)
            //    {
            //        if (_dataContext.DialedNumbers.Length >= 9 && txtNumbers.FontSize > 26.75)
            //        {
            //            _dataContext.ModifiedTextSize = txtNumbers.FontSize = txtNumbers.FontSize - 2.75;
            //        }
            //        _dataContext.DialedNumbers = _dataContext.DialedNumbers + number.Content.ToString();
            //        typednumber.Append(number.Content.ToString());
            //    }
            //}
            //else if (_dataContext.DialedNumbers.Length <= 9)
            //{
            //    if (txtNumbers.FontSize != 35)
            //    {
            //        txtNumbers.FontSize = 35;
            //    }
            //    _dataContext.DialedNumbers = _dataContext.DialedNumbers + number.Content.ToString();
            //    typednumber.Append(number.Content.ToString());
            //}

            #endregion Old code
        }

        /// <summary>
        /// Handles the PreviewMouseRightButtonDown event of the PhoneBook control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="MouseButtonEventArgs"/> instance containing the event data.</param>
        private void PhoneBook_PreviewMouseRightButtonDown(object sender, MouseButtonEventArgs e)
        {
            if (sender is Microsoft.Windows.Controls.DataGridCell)
            {
                Microsoft.Windows.Controls.DataGridCell tempcell = sender as Microsoft.Windows.Controls.DataGridCell;
                Microsoft.Windows.Controls.DataGridRow row = null;
                var parent = VisualTreeHelper.GetParent(tempcell);
                while (parent != null && parent.GetType() != typeof(Microsoft.Windows.Controls.DataGridRow))
                {
                    parent = VisualTreeHelper.GetParent(parent);
                    if (parent is Microsoft.Windows.Controls.DataGridRow)
                    {
                        row = parent as Microsoft.Windows.Controls.DataGridRow;
                        break;
                    }
                }
                if (row != null)
                {
                    Agent.Interaction.Desktop.Helpers.Contacts selectedContact = (Agent.Interaction.Desktop.Helpers.Contacts)row.Item;
                    dgvContact.SelectedItem = (Agent.Interaction.Desktop.Helpers.Contacts)row.Item;
                    _phoneBookText = selectedContact.Number;
                    _phoneBookMenu.PlacementTarget = row;
                    _phoneBookMenu.IsOpen = true;
                }
            }
        }

        /// <summary>
        /// Handles the Click event of the RemoveNumber control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="RoutedEventArgs"/> instance containing the event data.</param>
        private void RemoveNumber_Click(object sender, RoutedEventArgs e)
        {
            if (_dataContext.DialedNumbers.Length != 0)
            {
                if (_typednumber.Length > 0)
                    _typednumber.Length -= 1;
                _dataContext.DialedNumbers = _dataContext.DialedNumbers.Remove(_dataContext.DialedNumbers.Length - 1, 1);
            }
            if (_dataContext.DialedNumbers.Length < _dataContext.MaxDialDigits && txtNumbers.FontSize < 35)
            {
                _dataContext.ModifiedTextSize = txtNumbers.FontSize = txtNumbers.FontSize + 2.75;
            }
            if (eventFireBackNum != null)
            {
                eventFireBackNum.Invoke("CLEAR");
            }

            Call.Focus();
        }

        /// <summary>
        /// Handles the SelectionChanged event of the SelectionTab control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="SelectionChangedEventArgs"/> instance containing the event data.</param>
        private void SelectionTab_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            foreach (System.Windows.Controls.TabItem item in (this.SelectionTab as System.Windows.Controls.TabControl).Items)
            {
                if (item == (this.SelectionTab as System.Windows.Controls.TabControl).SelectedItem && item.Name == "tabDial")
                {
                    item.Foreground = (Brush)(new BrushConverter().ConvertFromString("#0071C6"));
                }
                else if (item != (this.SelectionTab as System.Windows.Controls.TabControl).SelectedItem && item.Name == "tabDial")
                {
                    item.Foreground = System.Windows.Media.Brushes.Black;
                }
                else if (item != (this.SelectionTab as System.Windows.Controls.TabControl).SelectedItem && item.Name == "tabPhoneBook")
                {
                    txtNumbers.Text = string.Empty;
                    _typednumber.Clear();
                    _dataContext.DiallingNumber = string.Empty;
                    _dataContext.DialedNumbers = string.Empty;
                }
                //else if (item != (this.SelectionTab as System.Windows.Controls.TabControl).SelectedItem && item.Name == "tabTeamCommunicator")
                //{
                //    TabItem_MouseLeftButtonDown(null, null);
                //}
            }
            //TabItem t = ((sender as TabControl).SelectedItem as TabItem);
            //if(t!=null)
            //    if(t.Name=="tabTeamCommunicator")
            //        TabItem_MouseLeftButtonDown(null, null);
        }

        /// <summary>
        /// Handles the Click event of the Single control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="RoutedEventArgs"/> instance containing the event data.</param>
        private void Single_Click(object sender, RoutedEventArgs e)
        {
            _dataContext.Singleclick = true;
            _dataContext.Dualclick = false;
        }

        /// <summary>
        /// Handles the MouseLeftClick event of the DialTabItem control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="System.Windows.Input.MouseButtonEventArgs"/> instance containing the event data.</param>
        private void TabItem_MouseLeftClick(object sender, MouseButtonEventArgs e)
        {
            if (sender is TabItem)
            {
                var tabItem = sender as TabItem;
                if (tabItem.Name == "tabDial")
                {
                    ChkAgentLevel.IsChecked = false;
                    ChkApplicationLevel.IsChecked = false;
                    ChkGroupLevel.IsChecked = false;
                    _phoneBookText = string.Empty;
                }
                else //if (tabItem.Name == "tabPhoneBook")
                {
                    txtNumbers.Text = string.Empty;
                    _typednumber.Clear();
                    _dataContext.DiallingNumber = string.Empty;
                    _dataContext.DialedNumbers = string.Empty;
                }
            }
            //txtSearch.Text = string.Empty;
        }

        /// <summary>
        /// Handles the PreviewKeyDown event of the TabItem control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="KeyEventArgs"/> instance containing the event data.</param>
        private void TabItem_PreviewKeyDown(object sender, KeyEventArgs e)
        {
            if (_dataContext.DialedNumbers.Length < _dataContext.MaxDialDigits)
            {
                if (Keyboard.Modifiers == ModifierKeys.Control && e.Key == Key.V)
                {
                    if (Clipboard.ContainsText(TextDataFormat.Text))
                    {
                        string returnHtmlText = Clipboard.GetText(TextDataFormat.Text);
                        var letters = returnHtmlText.Count(char.IsLetter);
                        var digits = returnHtmlText.Count(char.IsDigit);
                        if (letters == 0 && digits > 0)
                        {
                            foreach (var item in returnHtmlText.ToCharArray())
                            {
                                if (_dataContext.DialedNumbers.Length < _dataContext.MaxDialDigits)
                                {
                                    switch (item)
                                    {
                                        case '0':
                                            btn0.RaiseEvent(new RoutedEventArgs(Button.ClickEvent));
                                            break;
                                        case '1':
                                            btn1.RaiseEvent(new RoutedEventArgs(Button.ClickEvent));
                                            break;
                                        case '2':
                                            btn2.RaiseEvent(new RoutedEventArgs(Button.ClickEvent));
                                            break;
                                        case '3':
                                            btn3.RaiseEvent(new RoutedEventArgs(Button.ClickEvent));
                                            break;
                                        case '4':
                                            btn4.RaiseEvent(new RoutedEventArgs(Button.ClickEvent));
                                            break;
                                        case '5':
                                            btn5.RaiseEvent(new RoutedEventArgs(Button.ClickEvent));
                                            break;
                                        case '6':
                                            btn6.RaiseEvent(new RoutedEventArgs(Button.ClickEvent));
                                            break;
                                        case '7':
                                            btn7.RaiseEvent(new RoutedEventArgs(Button.ClickEvent));
                                            break;
                                        case '8':
                                            btn8.RaiseEvent(new RoutedEventArgs(Button.ClickEvent));
                                            break;
                                        case '9':
                                            btn9.RaiseEvent(new RoutedEventArgs(Button.ClickEvent));
                                            break;
                                    }
                                }

                            }
                        }
                    }
                }

                else if (Keyboard.Modifiers == ModifierKeys.Shift && e.Key == Key.D3)
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
            if (e.Key == Key.Back)
                btnClear.RaiseEvent(new RoutedEventArgs(Button.ClickEvent));
            else if ((e.Key == Key.Enter || e.Key == Key.Space) && txtNumbers.Text != string.Empty && !Call.IsFocused)
                Call.RaiseEvent(new RoutedEventArgs(Button.ClickEvent));
        }

        /// <summary>
        /// Handles the TextChanged event of the txtContactSearch control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="TextChangedEventArgs"/> instance containing the event data.</param>
        private void txtContactSearch_TextChanged(object sender, TextChangedEventArgs e)
        {
            TextBox txtSearch = sender as TextBox;
            _searchText = txtSearch.Text.ToString();
            Thread searchContacts = null;
            try
            {
                object searchString = (object)_searchText;
                searchContacts = new Thread(new ParameterizedThreadStart(ContactSearch));
                searchContacts.Start(searchString);
            }
            catch (Exception commonException)
            {
                _logger.Error("Error occurred while searching contacts at keyup " + commonException.ToString());
            }
            finally
            {
                _searchContact = null;
            }
        }

        /// <summary>
        /// Handles the TextChanged event of the txtNumbersBook control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="TextChangedEventArgs"/> instance containing the event data.</param>
        private void txtNumbersBook_TextChanged(object sender, TextChangedEventArgs e)
        {
            try
            {
                if (_phoneBookText.Length <= _dataContext.MaxDialDigits && eventFireBackNum != null)
                {
                    eventFireBackNum.Invoke(_phoneBookText);
                }
            }
            catch { }
        }

        /// <summary>
        /// Handles the TextChanged event of the txtNumbers control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="TextChangedEventArgs"/> instance containing the event data.</param>
        private void txtNumbers_TextChanged(object sender, TextChangedEventArgs e)
        {
            try
            {
                if (txtNumbers.Text.Length <= _dataContext.MaxDialDigits && eventFireBackNum != null)
                {
                    eventFireBackNum.Invoke(txtNumbers.Text);
                }
            }
            catch
            {
            }
        }

        /// <summary>
        /// Handles the Loaded event of the UserControl control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="RoutedEventArgs"/> instance containing the event data.</param>
        private void UserControl_Loaded(object sender, RoutedEventArgs e)
        {
            try
            {
                txtNumbers.Focus();
                this.DataContext = Datacontext.GetInstance();
                //if (_dataContext.isOnCall)
                //{
                //    _dataContext.MaxDialDigits = _dataContext.ConsultDialDigits;
                //}
                //else
                //{
                //    _dataContext.MaxDialDigits = _dataContext.DialpadDigits;
                //}
            }
            catch (Exception commonException)
            {
                _logger.Error("DialPad:UsercontrolLoaded:" + commonException.ToString());
            }
        }

        /// <summary>
        /// Handles the Unloaded event of the UserControl control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="RoutedEventArgs"/> instance containing the event data.</param>
        private void UserControl_Unloaded(object sender, RoutedEventArgs e)
        {
            if (_windowType == Datacontext.DialPadType.Transfer)
            {
                _dataContext.IsTransDialPadOpen = false;
                _dataContext.UserSetTransType = Datacontext.ConsultType.None;
            }
            else if (_windowType == Datacontext.DialPadType.Conference)
            {
                _dataContext.IsConfDialPadOpen = false;
                _dataContext.UserSetConfType = Datacontext.ConsultType.None;
            }
            _logger = null;
        }

        #endregion Methods

        #region Other

        // private static ILog _logger = LogManager.GetLogger(typeof(DialPad));
        /// <summary>
        /// Initializes a new instance of the <see cref="DialPad"/> class.
        /// </summary>
        //ComboBox cmb_Requeue = null;
        //TextBox txtbox_Requeue = null;
        /// <summary>
        /// Contacts the controller.
        /// </summary>
        /// <param name="contacts">The contacts.</param>
        /// <summary>
        /// Contacts the search.
        /// </summary>
        /// <param name="value">The value.</param>
        //end

        #endregion Other
    }
}