namespace Agent.Interaction.Desktop.ApplicationReader
{
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using System.IO;
    using System.Linq;
    using System.Text;
    using System.Xml;
    using System.Xml.Linq;

    internal class XMLHandler
    {
        #region Fields

        public Hashtable ConfigKeys = new Hashtable();

        private const string rootElement = "UserSettings";

        #endregion Fields

        #region Constructors

        public XMLHandler()
        {
            ConfigKeys.Add(Keys.UserName, "login.user-name");
            ConfigKeys.Add(Keys.Place, "login.place-name");
            ConfigKeys.Add(Keys.Host, "login.host");
            ConfigKeys.Add(Keys.Port, "login.port");
            ConfigKeys.Add(Keys.KeepPlace, "login.keep-recent-place");
            ConfigKeys.Add(Keys.KeepChannels, "login.keep-recent-channels");
            ConfigKeys.Add(Keys.ApplicationName, "login.application-name");
            ConfigKeys.Add(Keys.Queueselection, "login.queue-selection");
            ConfigKeys.Add(Keys.GadgetState, "softphone.gadget-state");
            ConfigKeys.Add(Keys.ConfigServers, "login.configuration-servers");
            ConfigKeys.Add(Keys.Subversion, "application.subversion");
            ConfigKeys.Add(Keys.TrialMessage, "trial.display-message");
            ConfigKeys.Add(Keys.TrialNotificationMessage, "trial.notification-message");
            ConfigKeys.Add(Keys.TrialNotificationStartDate, "trial.notification.start-date");
            ConfigKeys.Add(Keys.TrialNotificationEndDate, "trial.end-date");
        }

        #endregion Constructors

        #region Enumerations

        public enum Keys
        {
            UserName, Place, KeepPlace, Subversion,
            KeepChannels, ApplicationName, Host, Port,
            Queueselection, GadgetState, ConfigServers,
            TrialMessage, TrialNotificationMessage,
            TrialNotificationStartDate, TrialNotificationEndDate
        }

        #endregion Enumerations

        #region Methods

        public void CDPW_ReadXmlData(string location, Dictionary<string, string> nodes)
        {
            try
            {
                CDPW_CreateXmlFile(location);
                XmlDocument xDocument = new XmlDocument();
                xDocument.Load(location);
                XmlNode root = xDocument.DocumentElement;

                XmlNode cdwp = root.SelectSingleNode("CallDataWindowPopUp");
                XmlNode child = null;
                if (cdwp == null)
                {
                    child = xDocument.CreateNode(XmlNodeType.Element, "CallDataWindowPopUp", null);
                    root.AppendChild(child);
                }
                cdwp = root.SelectSingleNode("CallDataWindowPopUp");

                foreach (var item in nodes.Keys.ToList())
                {
                    var myNode = cdwp.SelectSingleNode("descendant::" + item);
                    if (myNode != null)
                    {
                        nodes[item] = myNode.InnerText;
                    }
                }
            }
            catch
            {

            }
        }

        public void CDPW_UpdateXmlData(string location, Dictionary<string, string> nodes)
        {
            try
            {
                CDPW_CreateXmlFile(location);
                XmlDocument xDocument = new XmlDocument();
                xDocument.Load(location);
                XmlNode root = xDocument.DocumentElement;

                XmlNode cdwp = root.SelectSingleNode("CallDataWindowPopUp");
                XmlNode child = null;
                if (cdwp == null)
                {
                    child = xDocument.CreateNode(XmlNodeType.Element, "CallDataWindowPopUp", null);
                    root.AppendChild(child);
                }
                cdwp = root.SelectSingleNode("CallDataWindowPopUp");

                foreach (var item in nodes.Keys)
                {
                    var myNode = cdwp.SelectSingleNode("descendant::" + item);
                    if (myNode == null)
                    {
                        var attr1 = xDocument.CreateNode(XmlNodeType.Element, item, null);
                        attr1.InnerText = nodes[item];
                        cdwp.AppendChild(attr1);
                    }
                    else
                    {
                        myNode.InnerText = nodes[item];
                    }
                }
                xDocument.Save(location);
            }
            catch { }
        }

        public int CreateXmlData(string XMLFile, string Key, string value)
        {
            try
            {
                if (File.Exists(XMLFile))
                {
                    System.Xml.Linq.XDocument resultsDoc = System.Xml.Linq.XDocument.Parse((System.Xml.Linq.XDocument.Load(XMLFile)).ToString());
                    var query = resultsDoc.Root;
                    query.Add(new System.Xml.Linq.XElement("Item", (new System.Xml.Linq.XElement("Key", Key)), (new System.Xml.Linq.XElement("Value", value))));
                    resultsDoc.Save(XMLFile);
                    return 1;
                }
            }
            catch (Exception ErrorEventArgs)
            { }
            return 0;
        }

        public void CreateXMLFile(string XMLFile)
        {
            if (!Directory.Exists(Path.GetDirectoryName(XMLFile)))
                Directory.CreateDirectory(Path.GetDirectoryName(XMLFile));
            if (!File.Exists(XMLFile))
            {
                XDocument config = new XDocument(
                       new XDeclaration("1.0", "utf-8", ""),
                       new XElement(rootElement));

                config.Save(XMLFile);
            }
        }

        //Code added by Manikandan on 26/11/2013
        //For getting the XML Contacts
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Reliability", "CA2000:Dispose objects before losing scope")]
        public Dictionary<string, string> LoadXmlContacts(string XMLFile)
        {
            Dictionary<string, string> xmlContacts = new Dictionary<string, string>();
            StreamReader readUserDetails = null;
            XmlDocument configXml = new XmlDocument();
            try
            {
                if (!File.Exists(XMLFile))
                    return xmlContacts;
                if (readUserDetails == null)
                    readUserDetails = new StreamReader(XMLFile);
                configXml.LoadXml(readUserDetails.ReadToEnd());
                XmlNodeList nodeList = configXml.GetElementsByTagName("Users");
                if (nodeList.Count > 0)
                    foreach (XmlNode node in nodeList)
                    {
                        string[] tr = node.InnerText.ToString().Split('/');
                        xmlContacts.Add(tr[0].ToString(), tr[1].ToString());
                    }
                readUserDetails.Dispose();
            }
            catch (Exception commonException)
            {
                //System.Windows.MessageBox.Show("XML File Not found");
                // readUserDetails.Dispose();
            }
            return xmlContacts;
        }

        public int ModifyXmlData(string XMLFile, string key, string value)
        {
            try
            {
                System.Xml.Linq.XDocument resultsDoc = System.Xml.Linq.XDocument.Parse((System.Xml.Linq.XDocument.Load(XMLFile)).ToString());
                string query = resultsDoc.Root.Descendants("Item").Where(x => x.Element("Key").Value.ToString() == key).Select(i => i.Element("Key").Value.ToString()).FirstOrDefault();
                if (query == null)
                {
                    return CreateXmlData(XMLFile, key, value);
                }
                else
                {
                    return UpdateXmlData(XMLFile, key, value);
                }
            }
            catch { }
            return 0;
        }

        public Dictionary<string, string> ReadAllXmlData(string XMLFile)
        {
            Dictionary<string, string> configData = new Dictionary<string, string>();
            try
            {
                System.Xml.Linq.XDocument resultsDoc = System.Xml.Linq.XDocument.Parse((System.Xml.Linq.XDocument.Load(XMLFile)).ToString());
                var query = resultsDoc.Root.Descendants("Item");
                foreach (System.Xml.Linq.XElement e in query)
                {
                    if (!configData.ContainsKey(e.Element("Key").Value.ToString()))
                        configData.Add(e.Element("Key").Value.ToString(), e.Element("Value").Value.ToString());
                    else
                        configData[e.Element("Key").Value.ToString()] = e.Element("Value").Value.ToString();
                }
            }
            catch (Exception ex)
            { return null; }
            return configData;
        }

        public string[] ReadXmlData(string XMLFile, string key)
        {
            string[] selectedValue = null;
            try
            {
                System.Xml.Linq.XDocument resultsDoc = System.Xml.Linq.XDocument.Parse((System.Xml.Linq.XDocument.Load(XMLFile)).ToString());
                var query = resultsDoc.Root.Descendants("Item");
                foreach (System.Xml.Linq.XElement e in query)
                {
                    if (e.Element("Key").Value.ToString() == key)
                    {
                        selectedValue = new string[] { e.Element("Key").Value.ToString(), e.Element("Value").Value.ToString() };
                    }
                }
            }
            catch (Exception error) { return null; }
            return selectedValue;
        }

        public int UpdateXmlData(string XMLFile, string Key, string value)
        {
            try
            {
                System.Xml.Linq.XDocument resultsDoc = System.Xml.Linq.XDocument.Parse((System.Xml.Linq.XDocument.Load(XMLFile)).ToString());
                var query = resultsDoc.Root.Descendants("Item");
                foreach (System.Xml.Linq.XElement e in query)
                {
                    if (e.Element("Key").Value.ToString() == Key)
                    {
                        e.Element("Value").Value = value;
                    }
                }
                resultsDoc.Save(XMLFile);
                return 1;
            }
            catch (Exception error) { }
            return 0;
        }

        private void CDPW_CreateXmlFile(string location)
        {
            if (!Directory.Exists(Path.GetDirectoryName(location)))
                Directory.CreateDirectory(Path.GetDirectoryName(location));
            if (!File.Exists(location))
            {
                XmlTextWriter writeUserDetails = new XmlTextWriter(location, Encoding.Default);
                writeUserDetails.Formatting = Formatting.Indented;

                writeUserDetails.WriteStartElement("AgentConfig");
                writeUserDetails.WriteStartElement("CallDataWindowPopUp");
                writeUserDetails.WriteElementString("voice.calldata.popup-position", "");
                writeUserDetails.WriteElementString("voice.calldata.popup-size", "");
                writeUserDetails.WriteEndElement();
                writeUserDetails.Close();
                writeUserDetails = null;
            }
        }

        #endregion Methods
    }
}