namespace Agent.Interaction.Desktop.WebBrowserOverlay
{
    using System;
    using System.Windows;
    using System.Windows.Controls;
    using System.Windows.Interop;
    using System.Windows.Media;

    /// <summary>
    /// Displays a WebBrowser control over a given placement target element in a WPF Window.
    /// The owner window can be transparent, but not this one, due mixing DirectX and GDI drawing. 
    /// WebBrowserOverlayWF uses WinForms to avoid this limitation.
    /// </summary>
    public partial class HelperWindow : Window
    {
        #region Fields

        Point offset;

        //        SoftPhoneBar win;
        FrameworkElement _placementTarget;

        #endregion Fields

        #region Constructors

        public HelperWindow(FrameworkElement placementTarget, Window ownerWindow, TabControl objParenttab)
        {
            InitializeComponent();
            try
            {
                _placementTarget = placementTarget;
                try
                {
                    Owner = ownerWindow;
                }
                catch
                {

                }

                //Window owner = Window.GetWindow(placementTarget);
                //Debug.Assert(owner != null);
                //  win = ownerWindow as SoftPhoneBar;
                if (objParenttab != null)
                {
                    objParenttab.SelectionChanged += new SelectionChangedEventHandler(DataTabControl_SelectionChanged);
                    objParenttab.IsVisibleChanged += new DependencyPropertyChangedEventHandler(DataTabControl_IsVisibleChanged);
                }

                ownerWindow.SizeChanged += delegate { OnSizeLocationChanged(); };
                ownerWindow.LocationChanged += delegate { OnSizeLocationChanged(); };
                _placementTarget.SizeChanged += delegate { OnSizeLocationChanged(); };
                ownerWindow.LayoutUpdated += delegate { OnSizeLocationChanged(); };
                //this.Activated += new EventHandler(HelperWindow_Activated);
                //this.Deactivated += new EventHandler(HelperWindow_Deactivated);

                if (ownerWindow.IsVisible)
                {
                    this.Show();
                }
                else
                    ownerWindow.IsVisibleChanged += delegate
                    {
                        if (ownerWindow.IsVisible)
                        {
                            Owner = ownerWindow;
                            Show();
                        }
                    };

            }
            catch (Exception error)
            {
                string message = error.Message;
                message = message + "Error";
            }
            //owner.LayoutUpdated += new EventHandler(OnOwnerLayoutUpdated);
        }

        #endregion Constructors

        #region Properties

        public WebBrowser WebBrowser
        {
            get { return _wb; }
        }

        #endregion Properties

        #region Methods

        protected override void OnClosing(System.ComponentModel.CancelEventArgs e)
        {
            try
            {
                base.OnClosing(e);
                if (!e.Cancel)
                    // Delayed call to avoid crash due to Window bug.
                    Dispatcher.BeginInvoke((Action)delegate
                    {
                        Owner.Close();
                    });
            }
            catch
            {

            }
        }

        void DataTabControl_IsVisibleChanged(object sender, DependencyPropertyChangedEventArgs e)
        {
            try
            {
                if ((e.NewValue as bool?) == true)
                {
                    if (IsChild(((sender as TabControl).SelectedItem as TabItem), _placementTarget))
                    {
                        Show();
                        Point _offset = _placementTarget.TranslatePoint(new Point(), Owner);
                        if (offset.X < _offset.X || offset.Y < _offset.Y)
                        {
                            offset = _offset;
                            Point size = new Point(_placementTarget.ActualWidth, _placementTarget.ActualHeight);
                            HwndSource hwndSource = (HwndSource)HwndSource.FromVisual(Owner);
                            CompositionTarget ct = hwndSource.CompositionTarget;
                            offset = ct.TransformToDevice.Transform(offset);
                            size = ct.TransformToDevice.Transform(size);

                            Win32.POINT screenLocation = new Win32.POINT(offset);
                            Win32.ClientToScreen(hwndSource.Handle, ref screenLocation);
                            Win32.POINT screenSize = new Win32.POINT(size);

                            Win32.MoveWindow(((HwndSource)HwndSource.FromVisual(this)).Handle, screenLocation.X, screenLocation.Y, screenSize.X, screenSize.Y, true);
                        }
                    }
                    else
                    {
                        Hide();
                    }
                }
                else
                {
                    Hide();
                }
            }
            catch
            {

            }
        }

        void DataTabControl_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            try
            {
                if (e.AddedItems.Count > 0)
                {
                    if ((e.AddedItems[0] as TabItem) != null)
                    {
                        if (IsChild((e.AddedItems[0] as TabItem), _placementTarget))
                        {
                            Show();
                            Point _offset = _placementTarget.TranslatePoint(new Point(), Owner);
                            if (offset.X < _offset.X || offset.Y < _offset.Y)
                            {
                                offset = _offset;

                                Point size = new Point(_placementTarget.ActualWidth, _placementTarget.ActualHeight);
                                HwndSource hwndSource = (HwndSource)HwndSource.FromVisual(Owner);
                                CompositionTarget ct = hwndSource.CompositionTarget;
                                offset = ct.TransformToDevice.Transform(offset);
                                size = ct.TransformToDevice.Transform(size);

                                Win32.POINT screenLocation = new Win32.POINT(offset);
                                Win32.ClientToScreen(hwndSource.Handle, ref screenLocation);
                                Win32.POINT screenSize = new Win32.POINT(size);

                                Win32.MoveWindow(((HwndSource)HwndSource.FromVisual(this)).Handle, screenLocation.X, screenLocation.Y, screenSize.X, screenSize.Y, true);
                            }
                        }
                        else
                            Hide();
                    }
                    else
                    {
                        Hide();
                    }
                }
            }
            catch
            {

            }
        }

        //void HelperWindow_Activated(object sender, EventArgs e)
        //{
        //    win.Window_Deactivated(null, null);
        //}
        //void HelperWindow_Deactivated(object sender, EventArgs e)
        //{
        //    win.Window_Activated(null, null);
        //}
        bool IsChild(FrameworkElement parent, FrameworkElement child)
        {
            if (child.Parent == parent)
                return true;
            else
                return false;
        }

        void OnSizeLocationChanged()
        {
            try
            {
                if (this.Visibility == System.Windows.Visibility.Visible)
                {
                    Point offset = _placementTarget.TranslatePoint(new Point(), Owner);
                    Point size = new Point(_placementTarget.ActualWidth, _placementTarget.ActualHeight);
                    HwndSource hwndSource = (HwndSource)HwndSource.FromVisual(Owner);
                    CompositionTarget ct = hwndSource.CompositionTarget;
                    offset = ct.TransformToDevice.Transform(offset);
                    size = ct.TransformToDevice.Transform(size);

                    Win32.POINT screenLocation = new Win32.POINT(offset);
                    Win32.ClientToScreen(hwndSource.Handle, ref screenLocation);
                    Win32.POINT screenSize = new Win32.POINT(size);
                    try
                    {
                        Win32.MoveWindow(((HwndSource)HwndSource.FromVisual(this)).Handle, screenLocation.X, screenLocation.Y, screenSize.X, screenSize.Y, true);
                    }
                    catch
                    {

                    }

                }
            }
            catch
            {

            }
        }

        #endregion Methods
    }
}