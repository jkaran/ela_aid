namespace AgentInteractrionDesktop
{
    using System;
    using System.ComponentModel.Composition;
    using System.ComponentModel.Composition.Hosting;
    using System.Configuration;
    using System.Deployment.Application;
    using System.Diagnostics;
    using System.Globalization;
    using System.IO;
    using System.Linq;
    using System.Reflection;
    using System.Text;
    using System.Threading;
    using System.Windows;
    using System.Windows.Input;
    using System.Windows.Media.Animation;

    using Agent.Interaction.Desktop;
    using Agent.Interaction.Desktop.Settings;

    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        #region Fields

        private Datacontext _dataContext = Datacontext.GetInstance();

        #endregion Fields

        #region Methods

        /// <summary>
        /// Sets the language.
        /// </summary>
        public void SetLanguage()
        {
            try
            {
                //switch (Thread.CurrentThread.CurrentCulture.ToString())
                //{
                //    case "en-US":
                //        Login.CultureCode = "en-US";
                //        break;

                //    //case "es-ES":
                //    //    Login.CultureCode = "es-ES";
                //    //    break;

                //    default:
                //        Login.CultureCode = "en-US";
                //        break;
                //}
                var cultureInfo = new CultureInfo("en-US");
                Thread.CurrentThread.CurrentCulture = cultureInfo;
                Thread.CurrentThread.CurrentUICulture = cultureInfo;
                var dictionary = (from d in _dataContext.ImportCatalog.ResourceDictionaryList
                                  where d.Metadata.ContainsKey("Culture")
                                  && d.Metadata["Culture"].ToString().Equals("en-US")
                                  select d).FirstOrDefault();
                if (dictionary != null && dictionary.Value != null)
                {
                    this.Resources.MergedDictionaries.Add(dictionary.Value);
                }
            }
            catch { }
        }

        /// <summary>
        /// Raises the <see cref="E:System.Windows.Application.Startup" /> event.
        /// </summary>
        /// <param name="e">A <see cref="T:System.Windows.StartupEventArgs" /> that contains the event data.</param>
        protected override void OnStartup(StartupEventArgs e)
        {
            try
            {
                EventManager.RegisterClassHandler(typeof(UIElement), AccessKeyManager.AccessKeyPressedEvent, new AccessKeyPressedEventHandler(OnAccessKeyPressed));
                _dataContext.IsDebug = (e.Args.Contains("debug"));
                bool IsSplashScreenEnabled = true;
                if (e.Args.Length > 0)
                    IsSplashScreenEnabled = !(e.Args.Contains("NoSplashScreen"));
                if (IsSplashScreenEnabled)
                {
                    var splashScreen = new SplashScreen(System.Reflection.Assembly.GetExecutingAssembly(), "Images/MainSplashScreen.png");
                    splashScreen.Show(true);
                }

                if (ApplicationDeployment.IsNetworkDeployed)
                {
                    // <add key="login.url" value="tcp://win2003se:2020/AID" />
                    if (!File.Exists(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData).ToString() + @"\Pointel\AgentInteractionDesktop\localuser.settings.config"))
                    {
                        if (ConfigurationManager.AppSettings.AllKeys.Contains("login.url"))
                        {
                            if (!string.IsNullOrEmpty(ConfigurationManager.AppSettings["login.url"]))
                            {
                                string uri = ConfigurationManager.AppSettings["login.url"];
                                Uri _uri = new Uri(uri);
                                _dataContext.ApplicationName = _uri.PathAndQuery.Replace("/", "");
                                _dataContext.HostNameText = _dataContext.HostNameSelectedValue = _uri.Host;
                                _dataContext.PortText = _dataContext.PortSelectedValue = (_uri.Port > 0 ? _uri.Port.ToString() : "");
                            }
                        }

                    }
                }
            }
            catch (Exception ex)
            {
                // System.Windows.MessageBox.Show("Error occurred while moving settings file to required path as : " + ex.ToString());
            }

            SetImportCatalog();
            SetLanguage();
            Application.Current.DispatcherUnhandledException += new System.Windows.Threading.DispatcherUnhandledExceptionEventHandler(AppDispatcherUnhandledException);
            AppDomain.CurrentDomain.UnhandledException += new UnhandledExceptionEventHandler(CurrentDomain_UnhandledException);

            //Code added to reduce cpu usage to minimal by reducing the application refresh frame per second.
            Timeline.DesiredFrameRateProperty.OverrideMetadata(typeof(Timeline), new FrameworkPropertyMetadata { DefaultValue = 21 });
            base.OnStartup(e);

            //System.Windows.MessageBox.Show("The application is deployed by " + (ApplicationDeployment.IsNetworkDeployed == true ? "Click once." : "Stand alone." ));
        }

        static void CurrentDomain_UnhandledException(object sender, UnhandledExceptionEventArgs e)
        {
            try
            {
                Exception ex = (Exception)e.ExceptionObject;
                Pointel.Logger.Core.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType,
                        "AID").Error("--- Unhandled exception ---");

                Pointel.Logger.Core.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType,
                        "AID").Error("Exception : " + ((ex.InnerException == null) ? ex.Message : ex.InnerException.ToString()));

                if (ex.TargetSite != null)
                    Pointel.Logger.Core.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType,
                        "AID").Error("Exception Module : " + ex.TargetSite.Module.Name + " - Exception Method : " + ex.TargetSite.Name);

                var showMessageBox = new Agent.Interaction.Desktop.MessageBox("Alert",
                        "Agent Interaction Desktop has encountered a problem.\nThe application will exit now, please contact your Administrator", "", "_OK", false);
                showMessageBox.ShowDialog();

                if (showMessageBox.DialogResult == true)
                {
                    Environment.Exit(0);
                }
            }
            catch { }
            finally
            {

            }
        }

        private static void OnAccessKeyPressed(object sender, AccessKeyPressedEventArgs e)
        {
            if (Keyboard.IsKeyDown(Key.Enter) || Keyboard.IsKeyDown(Key.Escape)) return;

            if (!e.Handled && e.Scope == null && (e.Target == null)) //|| e.Target == label
            {
                // If Alt key is not pressed - handle the event
                if ((Keyboard.Modifiers & ModifierKeys.Alt) != ModifierKeys.Alt)
                {
                    e.Target = null;
                    e.Handled = true;
                }
            }
        }

        void AppDispatcherUnhandledException(object sender, System.Windows.Threading.DispatcherUnhandledExceptionEventArgs e)
        {
            // System.Windows.MessageBox.Show(e.Exception.Message);
            try
            {
                Exception ex = (Exception)e.Exception;
                Pointel.Logger.Core.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType,
                        "AID").Fatal("--- Unhandled exception ---");

                Pointel.Logger.Core.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType,
                        "AID").Fatal("Exception : " + ((ex.InnerException == null) ? ex.Message : ex.InnerException.ToString()));

                if (ex.TargetSite != null)
                    Pointel.Logger.Core.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType,
                        "AID").Fatal("Exception Module : " + ex.TargetSite.Module.Name + " - Exception Method : " + ex.TargetSite.Name);

                var showMessageBox = new Agent.Interaction.Desktop.MessageBox("Alert",
                        "Agent Interaction Desktop has encountered a problem.\nThe application will exit now, please contact your Administrator", "", "_OK", false);
                showMessageBox.ShowDialog();

                if (showMessageBox.DialogResult == true)
                {
                    Environment.Exit(0);
                }
            }
            catch { }
            finally
            {

            }
        }

        /// <summary>
        /// Sets the import catalog.
        /// </summary>
        private void SetImportCatalog()
        {
            try
            {
                var path = AppDomain.CurrentDomain.BaseDirectory;
                var catalog = new DirectoryCatalog(path);
                var container = new CompositionContainer(catalog);
                container.ComposeParts(_dataContext.ImportCatalog);
            }
            catch (ReflectionTypeLoadException ex)
            {
                var sb = new StringBuilder();
                foreach (var exSub in ex.LoaderExceptions)
                {
                    sb.AppendLine(exSub.Message);
                    if (exSub is FileNotFoundException)
                    {
                        var exFileNotFound = exSub as FileNotFoundException;
                        if (!string.IsNullOrEmpty(exFileNotFound.FusionLog))
                        {
                            sb.AppendLine("Fusion Log:");
                            sb.AppendLine(exFileNotFound.FusionLog);
                        }
                    }
                    sb.AppendLine();
                }
                //Display or log the error based on your application.
            }
        }

        #endregion Methods
    }
}