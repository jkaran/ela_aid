using System;
using System.ComponentModel;
using System.Configuration.Install;
using System.IO;
using System.Reflection;


namespace Agent.Interaction.Desktop
{
    /// <summary>
    /// 
    /// </summary>
    [RunInstaller(true)]
    public partial class Installer1 : System.Configuration.Install.Installer
    {
        private string CustomDictionary = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData).ToString() + @"\Pointel\dic\dictionary.lex";

        public Installer1()
        {
            InitializeComponent();
        }


        /// <summary>
        /// After the install event handler.
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="e">The <see cref="InstallEventArgs"/> instance containing the event data.</param>
        #region AfterInstall
        private void AfterInstallEventHandler(object sender, InstallEventArgs e)
        {
            try
            {
                //var assembly = Assembly.GetExecutingAssembly();
                //var fvi = System.Diagnostics.FileVersionInfo.GetVersionInfo(assembly.Location);

                //string fileName = @"\user.trial.config";
                //string filepath = Environment.GetFolderPath(Environment.SpecialFolder.CommonApplicationData).ToString() + @"\Pointel\AgentInteractionDesktopV" + fvi.FileVersion;
                //filepath = filepath + fileName;

                //if (!Directory.Exists(Path.GetDirectoryName(filepath)))
                //    Directory.CreateDirectory(Path.GetDirectoryName(filepath));
                //if (!File.Exists(filepath))
                //{
                //    XDocument config = new XDocument(new XDeclaration("1.0", "utf-8", ""), new XElement("UserSettings", (new XElement("Item", (new XElement("Key", "trial.display-message")), (new XElement("Value", "Trial Version expired"))))
                //        , (new XElement("Item", (new XElement("Key", "trial.notification-message")), (new XElement("Value", "Trial Version expires in"))))
                //        , (new XElement("Item", (new XElement("Key", "trial.notification.start-date")), (new XElement("Value", BasicEncryptionDecryption.Encrypt(DateTime.Now.AddDays(8).ToString())))))
                //        , (new XElement("Item", (new XElement("Key", "trial.end-date")), (new XElement("Value", BasicEncryptionDecryption.Encrypt(DateTime.Now.AddDays(15).ToString())))))));
                //    config.Save(filepath);
                //}
            }
            catch (ReflectionTypeLoadException rex)
            {
                string errormsg = rex.Message.ToString();
                if (rex.InnerException != null)
                    errormsg += "inner ex : " + rex.InnerException.ToString();
                System.Windows.MessageBox.Show("error occurred while committing : " + errormsg);
            }
            catch (Exception ex)
            {
                //System.Windows.MessageBox.Show("Error occurred as : " + ex.Message.ToString()); 
            }
        }
        #endregion

        /// <summary>
        /// Handles the AfterUninstall event of the Installer1 control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="InstallEventArgs"/> instance containing the event data.</param>
        #region AfterUninstall
        private void AfterUninstallEventHandler(object sender, InstallEventArgs e)
        {
            try
            {

                var assembly = Assembly.GetExecutingAssembly();
                var fvi = System.Diagnostics.FileVersionInfo.GetVersionInfo(assembly.Location);
                string fileName = @"\localuser.settings.config";
                string filepath = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData).ToString() + @"\Pointel\AgentInteractionDesktopV" + fvi.FileVersion;
                filepath = filepath + fileName;
                if (File.Exists(filepath))
                    File.Delete(filepath);
                if (Directory.Exists(Path.GetDirectoryName(filepath)))
                    Directory.Delete(Path.GetDirectoryName(filepath));
                string folder = "";
                folder = Assembly.GetExecutingAssembly().Location;
                var files = new DirectoryInfo(folder).Parent;
                if (files != null)
                {
                    DirectoryInfo[] allDirectories = new DirectoryInfo(Path.Combine(files.FullName, "")).GetDirectories();
                    if (allDirectories != null)
                    {
                        foreach (DirectoryInfo directory in allDirectories)
                        {
                            if (directory.Name.Equals("Plugins"))
                            {
                                Directory.Delete(Path.Combine(files.FullName, "Plugins"), true);
                            }
                        }
                    }
                }
            }
            catch (ReflectionTypeLoadException rex)
            {
                string errormsg = rex.Message.ToString();
                if (rex.InnerException != null)
                    errormsg += "inner ex : " + rex.InnerException.ToString();
                System.Windows.MessageBox.Show("error occurred while committing : " + errormsg);
            }
            catch (Exception ex)
            {
                //System.Windows.MessageBox.Show("Error " + ex.Message.ToString());
            }
        }
        #endregion

        /// <summary>
        /// Raises the <see cref="E:System.Configuration.Install.Installer.Committing" /> event.
        /// </summary>
        /// <param name="savedState">An <see cref="T:System.Collections.IDictionary" /> that contains the state of the computer before the installers in the <see cref="P:System.Configuration.Install.Installer.Installers" /> property are committed.</param>
        #region OnCommitting
        protected override void OnCommitting(System.Collections.IDictionary savedState)
        {
            try
            {
                if (!File.Exists(CustomDictionary))
                {
                    File.Create(CustomDictionary).Dispose();
                }
                string installedPath = string.Empty;
                installedPath = Context.Parameters["assemblypath"];
                installedPath = installedPath.Substring(0, installedPath.LastIndexOf('\\'));

                File.Delete(Path.Combine(installedPath, "Agent.Interaction.Desktop.InstallState"));

                base.OnCommitting(savedState);
            }
            catch (ReflectionTypeLoadException rex)
            {
                string errormsg = rex.Message.ToString();
                if (rex.InnerException != null)
                    errormsg += "inner ex : " + rex.InnerException.ToString();
                System.Windows.MessageBox.Show("error occurred while committing : " + errormsg);
            }
            catch (Exception ex)
            {

            }
        }
        #endregion
    }
}
