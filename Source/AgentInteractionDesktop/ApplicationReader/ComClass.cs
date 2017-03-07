namespace Agent.Interaction.Desktop.ApplicationReader
{
    using System;
    using System.Collections.Generic;

    using Agent.Interaction.Desktop.Settings;

    using Genesyslab.Platform.ApplicationBlocks.ConfigurationObjectModel;
    using Genesyslab.Platform.ApplicationBlocks.ConfigurationObjectModel.CfgObjects;
    using Genesyslab.Platform.ApplicationBlocks.ConfigurationObjectModel.Queries;
    using Genesyslab.Platform.Commons.Protocols;
    using Genesyslab.Platform.Configuration.Protocols.ConfServer.Events;
    using Genesyslab.Platform.Configuration.Protocols.ConfServer.Requests.Security;
    using Genesyslab.Platform.Configuration.Protocols.Types;

    using Pointel.Configuration.Manager;
    using Pointel.Configuration.Manager.Common;
    using Pointel.Configuration.Manager.Core;

    /// <summary>
    ///
    /// </summary>
    public class ComClass
    {
        #region Fields

        private static ConfigContainer _configContainer = ConfigContainer.Instance();
        private static Pointel.Logger.Core.ILog _logger = Pointel.Logger.Core.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType,
            "AID");

        #endregion Fields

        #region Methods

        internal static OutputValues AddUpdateSkillToAgent(string operationType, string userName, string skillName, Int32 skillLevel)
        {
            OutputValues output = new OutputValues();
            try
            {
                CfgPersonQuery queryPerson = new CfgPersonQuery();
                queryPerson.UserName = userName;
                queryPerson.TenantDbid = _configContainer.TenantDbId;

                CfgPerson person = _configContainer.ConfServiceObject.RetrieveObject<CfgPerson>(queryPerson);
                if (operationType == "Add")
                {
                    if (person != null)
                    {
                        ICollection<CfgSkillLevel> agentSkills = person.AgentInfo.SkillLevels;
                        bool flag = false;
                        foreach (CfgSkillLevel checkSkill in agentSkills)
                        {
                            if (checkSkill.Skill.Name == skillName)
                            {
                                flag = true;
                                break;
                            }
                        }
                        if (!flag)
                        {
                            CfgSkillQuery querySkill = new CfgSkillQuery();
                            querySkill.TenantDbid = _configContainer.TenantDbId;
                            querySkill.Name = skillName;

                            CfgSkill skill = _configContainer.ConfServiceObject.RetrieveObject<CfgSkill>(querySkill);

                            CfgSkillLevel skillToAdd = new CfgSkillLevel(_configContainer.ConfServiceObject, person);
                            skillToAdd.Skill = skill;
                            skillToAdd.Level = Convert.ToInt32(skillLevel);

                            person.AgentInfo.SkillLevels.Add(skillToAdd);
                            person.Save();
                        }
                    }
                }
                else if (operationType == "Edit")
                {
                    if (person != null)
                    {
                        ICollection<CfgSkillLevel> agentSkills = person.AgentInfo.SkillLevels;
                        foreach (CfgSkillLevel editingSkill in agentSkills)
                        {
                            if (editingSkill.Skill.Name == skillName)
                            {
                                editingSkill.Level = Convert.ToInt32(skillLevel);
                                person.Save();
                                break;
                            }
                        }
                    }
                }
                output.MessageCode = "200";
                output.Message = "Skill " + operationType + "ed Successfully.";
                person = _configContainer.ConfServiceObject.RetrieveObject<CfgPerson>(queryPerson);

                if (person != null)
                {
                    //Update the skill level to MySkills
                    if (person.AgentInfo.SkillLevels != null && person.AgentInfo.SkillLevels.Count > 0)
                    {
                        Datacontext.GetInstance().MySkills.Clear();
                        foreach (CfgSkillLevel skill in person.AgentInfo.SkillLevels)
                            Datacontext.GetInstance().MySkills.Add(new Agent.Interaction.Desktop.Helpers.MySkills(skill.Skill.Name.ToString(), skill.Level));
                    }
                    else
                        Datacontext.GetInstance().MySkills.Clear();
                }
            }
            catch (Exception commonException)
            {
                output.MessageCode = "2001";
                output.Message = (commonException.InnerException == null ? commonException.Message : commonException.InnerException.Message);
            }
            return output;
        }

        internal static bool AuthenticateUser(string userName, string password)
        {
            bool output = false;
            try
            {
                RequestAuthenticate requestAuthenticate = RequestAuthenticate.Create(userName, password);
                IMessage response = _configContainer.ConfServiceObject.Protocol.Request(requestAuthenticate);
                if (response != null)
                {
                    switch (response.Id)
                    {
                        case EventAuthenticated.MessageId:
                            {
                                output = true;
                                _logger.Info("User " + userName + "  Authenticated ");
                                break;
                            }
                        case EventError.MessageId:
                            {
                                EventError eventError = (EventError)response;
                                if (eventError.Description != null)
                                {
                                    _logger.Warn("User " + userName + "  is not Authenticated   " + eventError.Description);
                                }
                                output = false;
                                _logger.Warn("User " + userName + "  is not Authenticated   ");
                                break;
                            }
                    }
                }
            }
            catch (Exception commonException)
            {
                _logger.Error("Error occurred while authenticating user " + userName + "  " + commonException.ToString());
            }
            return output;
        }

        internal static OutputValues DeleteSkillFromAgent(string userName, string skillName)
        {
            OutputValues output = new OutputValues();
            try
            {
                CfgPersonQuery queryPerson = new CfgPersonQuery();
                queryPerson.UserName = userName;
                queryPerson.TenantDbid = _configContainer.TenantDbId;

                CfgPerson person = _configContainer.ConfServiceObject.RetrieveObject<CfgPerson>(queryPerson);
                if (person != null)
                {
                    ICollection<CfgSkillLevel> agentSkills = person.AgentInfo.SkillLevels;
                    foreach (CfgSkillLevel skillToRemove in person.AgentInfo.SkillLevels)
                    {
                        if (skillToRemove.Skill.Name == skillName)
                        {
                            person.AgentInfo.SkillLevels.Remove(skillToRemove);
                            person.Save();
                            break;
                        }
                    }
                }
                output.MessageCode = "200";
                output.Message = "Skill deleted Successfully.";

                person = _configContainer.ConfServiceObject.RetrieveObject<CfgPerson>(queryPerson);

                if (person != null)
                {
                    //Update the skill level to MySkills
                    if (person.AgentInfo.SkillLevels != null && person.AgentInfo.SkillLevels.Count > 0)
                    {
                        Datacontext.GetInstance().MySkills.Clear();
                        foreach (CfgSkillLevel skillLevel in person.AgentInfo.SkillLevels)
                            Datacontext.GetInstance().MySkills.Add(new Agent.Interaction.Desktop.Helpers.MySkills(skillLevel.Skill.Name.ToString(), skillLevel.Level));
                    }
                    else
                        Datacontext.GetInstance().MySkills.Clear();
                }
            }
            catch (Exception commonException)
            {
                output.MessageCode = "2001";
                output.Message = (commonException.InnerException == null ? commonException.Message : commonException.InnerException.Message);
            }
            return output;
        }

        internal static void GetSwitchType()
        {
            if (_configContainer.AllKeys.Contains("switch-type") && _configContainer.GetValue("switch-type") != null)
            {

                var _switchQuery = new CfgSwitchQuery();
                _switchQuery.TenantDbid = _configContainer.TenantDbId;
                _switchQuery.Name = _configContainer.GetValue("switch-type");
                try
                {
                    var _switch = _configContainer.ConfServiceObject.RetrieveObject<CfgSwitch>(_switchQuery);
                    if (_switch != null)
                        Datacontext.GetInstance().SwitchType = _switch;
                    else
                    {
                        _switchQuery = new CfgSwitchQuery();
                        _switchQuery.TenantDbid = _configContainer.TenantDbId;
                        var _switchICollection = _configContainer.ConfServiceObject.RetrieveMultipleObjects<CfgSwitch>(_switchQuery);
                        foreach (CfgSwitch item in _switchICollection)
                        {
                            Datacontext.GetInstance().SwitchType = item;
                        }
                    }
                }
                catch
                {
                    _switchQuery = new CfgSwitchQuery();
                    _switchQuery.TenantDbid = _configContainer.TenantDbId;
                    var _switchICollection = _configContainer.ConfServiceObject.RetrieveMultipleObjects<CfgSwitch>(_switchQuery);
                    foreach (CfgSwitch item in _switchICollection)
                    {
                        Datacontext.GetInstance().SwitchType = item;
                    }
                }
                finally
                {
                    if (Datacontext.GetInstance().SwitchType != null)
                    {
                        _logger.Info("T-server switch type: " + Datacontext.GetInstance().SwitchType.Type.ToString());
                        Datacontext.GetInstance().SwitchName = Datacontext.GetInstance().SwitchType.Type == CfgSwitchType.CFGLucentDefinityG3 ? "avaya" : ((Datacontext.GetInstance().SwitchType.Type == CfgSwitchType.CFGNortelDMS100 || Datacontext.GetInstance().SwitchType.Type == CfgSwitchType.CFGNortelMeridianCallCenter) ? "nortel" : "avaya");
                    }
                }
            }
        }

        internal static CfgApplication ReadApplicationObjects(string applicationName)
        {
            CfgApplication application = null;
            try
            {
                application = new CfgApplication(_configContainer.ConfServiceObject);
                CfgApplicationQuery queryApp = new CfgApplicationQuery();
                //queryApp.TenantDbid = _configContainer.TenantDbId;
                queryApp.Name = applicationName;
                application = _configContainer.ConfServiceObject.RetrieveObject<CfgApplication>(queryApp);
            }
            catch (Exception commonException)
            {
                _logger.Error("Error occurred while reading " + applicationName + "  =  " + commonException.ToString());
            }
            return application;
        }

        #endregion Methods

        #region Other

        /// <summary>
        /// Gets the application values.
        /// </summary>
        //public void GetApplicationValues()
        //{
        //    try
        //    {
        //        CfgApplication application = new CfgApplication(Datacontext.comObject);
        //        CfgApplicationQuery queryApp = new CfgApplicationQuery();
        //        queryApp.Name = _dataContext.ApplicationName;
        //        application = Datacontext.comObject.RetrieveObject<CfgApplication>(queryApp);
        //        if (application != null)
        //        {
        //            string[] appkey = application.Options.AllKeys;
        //            foreach (string section in appkey)
        //            {
        //                if (string.Compare(section, "TServer", true) == 0)
        //                {
        //                    KeyValueCollection applicationValues = (KeyValueCollection)application.Options[section];
        //                    foreach (string key in applicationValues.AllKeys)
        //                    {
        //                        if (string.Compare(key, "PrimaryTServerName", true) == 0)
        //                        {
        //                            _dataContext.primaryTServer = ReadApplicationObjects(Convert.ToString(applicationValues[key]));
        //                            logger.Info("---------------Read Transaction Object------------------");
        //                            logger.Info("Transaction Object Name:" + "TServer");
        //                            logger.Info("Primary Server Name:" + applicationValues[key].ToString());
        //                            logger.Info("--------------------------------------------");
        //                        }
        //                        else if (string.Compare(key, "SecondaryTServerName", true) == 0)
        //                        {
        //                            _dataContext.secondaryTServer = ReadApplicationObjects(Convert.ToString(applicationValues[key]));
        //                            logger.Info("---------------Read Transaction Object------------------");
        //                            logger.Info("Transaction Object Name:" + "TServer");
        //                            logger.Info("Secondary Server Name:" + applicationValues[key].ToString());
        //                            logger.Info("--------------------------------------------");
        //                        }
        //                    }
        //                    if (_dataContext.primaryTServer == null)
        //                    {
        //                        _dataContext.primaryTServer = _dataContext.secondaryTServer;
        //                    }
        //                    if (_dataContext.secondaryTServer == null)
        //                    {
        //                        _dataContext.secondaryTServer = _dataContext.primaryTServer;
        //                    }
        //                }
        //                if (string.Compare(section, "Log", true) == 0)
        //                {
        //                    KeyValueCollection applicationValues = (KeyValueCollection)application.Options[section];
        //                    foreach (string key in applicationValues.AllKeys)
        //                    {
        //                        if (string.Compare(key, "ConversionPattern", true) == 0)
        //                        {
        //                            _dataContext.conversionPattern = applicationValues[key].ToString();
        //                        }
        //                        else if (string.Compare(key, "DatePattern", true) == 0)
        //                        {
        //                            _dataContext.datePattern = applicationValues[key].ToString();
        //                        }
        //                        else if (string.Compare(key, "Level", true) == 0)
        //                        {
        //                            _dataContext.logLevel = applicationValues[key].ToString();
        //                        }
        //                        else if (string.Compare(key, "LogFileName", true) == 0)
        //                        {
        //                            _dataContext.logFileName = applicationValues[key].ToString();
        //                        }
        //                        else if (string.Compare(key, "MaxFileSize", true) == 0)
        //                        {
        //                            _dataContext.maxFileSize = applicationValues[key].ToString();
        //                        }
        //                        else if (string.Compare(key, "MaxSizeRoll", true) == 0)
        //                        {
        //                            _dataContext.maxSizeRoll = applicationValues[key].ToString();
        //                        }
        //                    }
        //                }
        //                if (string.Compare(section, "StatServer", true) == 0)
        //                {
        //                    KeyValueCollection applicationValues = (KeyValueCollection)application.Options[section];
        //                    foreach (string key in applicationValues.AllKeys)
        //                    {
        //                        if (string.Compare(key, "PrimaryStatServer", true) == 0)
        //                        {
        //                            _dataContext.primaryStatServer = ReadApplicationObjects(Convert.ToString(applicationValues[key]));
        //                            logger.Info("---------------Read Transaction Object------------------");
        //                            logger.Info("Transaction Object Name:" + "StatServer");
        //                            logger.Info("Primary Server Name:" + applicationValues[key].ToString());
        //                            logger.Info("--------------------------------------------");
        //                        }
        //                        else if (string.Compare(key, "SecondaryStatServer", true) == 0)
        //                        {
        //                            _dataContext.secondaryStatServer = ReadApplicationObjects(Convert.ToString(applicationValues[key]));
        //                            logger.Info("---------------Read Transaction Object------------------");
        //                            logger.Info("Transaction Object Name:" + "StatServer");
        //                            logger.Info("Secondary Server Name:" + applicationValues[key].ToString());
        //                            logger.Info("--------------------------------------------");
        //                        }
        //                    }
        //                    if (_dataContext.primaryStatServer == null)
        //                    {
        //                        _dataContext.primaryStatServer = _dataContext.secondaryStatServer;
        //                    }
        //                    if (_dataContext.secondaryStatServer == null)
        //                    {
        //                        _dataContext.secondaryStatServer = _dataContext.primaryStatServer;
        //                    }
        //                }
        //                else if (string.Compare(section, "Set Call Status Timer", true) == 0)
        //                {
        //                    KeyValueCollection systemValues = (KeyValueCollection)application.Options[section];
        //                    foreach (string key in systemValues.AllKeys)
        //                    {
        //                        if (string.Compare(key, "Ans_Green", true) == 0)
        //                        {
        //                            DateTime green = Convert.ToDateTime(systemValues[key]);
        //                            _dataContext.loadThresholdValues.Add("Ans_Green", Convert.ToInt32(green.TimeOfDay.TotalSeconds));
        //                        }
        //                        else if (string.Compare(key, "Ans_Yellow", true) == 0)
        //                        {
        //                            DateTime yellow = Convert.ToDateTime(systemValues[key]);
        //                            _dataContext.loadThresholdValues.Add("Ans_Yellow", Convert.ToInt32(yellow.TimeOfDay.TotalSeconds));
        //                        }
        //                        else if (string.Compare(key, "Hold_Green", true) == 0)
        //                        {
        //                            DateTime holdGreen = Convert.ToDateTime(systemValues[key]);
        //                            _dataContext.loadThresholdValues.Add("Hold_Green", Convert.ToInt32(holdGreen.TimeOfDay.TotalSeconds));
        //                        }
        //                        else if (string.Compare(key, "Hold_Yellow", true) == 0)
        //                        {
        //                            DateTime holdYellow = Convert.ToDateTime(systemValues[key]);
        //                            _dataContext.loadThresholdValues.Add("Hold_Yellow", Convert.ToInt32(holdYellow.TimeOfDay.TotalSeconds));
        //                        }
        //                        else if (string.Compare(key, "NR_Green", true) == 0)
        //                        {
        //                            DateTime NRGreen = Convert.ToDateTime(systemValues[key]);
        //                            _dataContext.loadThresholdValues.Add("NR_Green", Convert.ToInt32(NRGreen.TimeOfDay.TotalSeconds));
        //                        }
        //                        else if (string.Compare(key, "NR_Yellow", true) == 0)
        //                        {
        //                            DateTime NRYellow = Convert.ToDateTime(systemValues[key]);
        //                            _dataContext.loadThresholdValues.Add("NR_Yellow", Convert.ToInt32(NRYellow.TimeOfDay.TotalSeconds));
        //                        }
        //                        else if (string.Compare(key, "Ready_Green", true) == 0)
        //                        {
        //                            DateTime RGreen = Convert.ToDateTime(systemValues[key]);
        //                            _dataContext.loadThresholdValues.Add("Ready_Green", Convert.ToInt32(RGreen.TimeOfDay.TotalSeconds));
        //                        }
        //                        else if (string.Compare(key, "Ready_Yellow", true) == 0)
        //                        {
        //                            DateTime RYellow = Convert.ToDateTime(systemValues[key]);
        //                            _dataContext.loadThresholdValues.Add("Ready_Yellow", Convert.ToInt32(RYellow.TimeOfDay.TotalSeconds));
        //                        }
        //                    }
        //                }
        //                else if (string.Compare(section, "StatisticsObject", true) == 0)
        //                {
        //                    KeyValueCollection systemValues = (KeyValueCollection)application.Options[section];
        //                    foreach (string key in systemValues.AllKeys)
        //                    {
        //                        if (string.Compare(key, "InboundCalls", true) == 0)
        //                        {
        //                            _dataContext.loadStatisticsObject.Add("InboundCalls", systemValues[key].ToString());
        //                        }
        //                        else if (string.Compare(key, "LoginTime", true) == 0)
        //                        {
        //                            _dataContext.loadStatisticsObject.Add("LoginTime", systemValues[key].ToString());
        //                        }
        //                        else if (string.Compare(key, "NotReady", true) == 0)
        //                        {
        //                            _dataContext.loadStatisticsObject.Add("NotReady", systemValues[key].ToString());
        //                        }
        //                    }
        //                }
        //                else if (string.Compare(section, "Not Ready Codes", true) == 0)
        //                {
        //                    KeyValueCollection notReadyCodeValues = (KeyValueCollection)application.Options[section];
        //                    foreach (string codes in notReadyCodeValues.AllKeys)
        //                    {
        //                        _dataContext.loadNotReadyReasonCodes.Add(codes);
        //                    }
        //                }
        //                else if (string.Compare(section, "Neocase", true) == 0)
        //                {
        //                    KeyValueCollection Neocase = (KeyValueCollection)application.Options[section];
        //                    foreach (string codes in Neocase.AllValues)
        //                    {
        //                        _dataContext.loadURL.Add(codes);
        //                    }
        //                }
        //                else if (string.Compare(section, "_system_", true) == 0)
        //                {
        //                    _dataContext.systemValuesCollection = (KeyValueCollection)application.Options[section];
        //                }
        //                else if (string.Compare(section, "Decision", true) == 0)
        //                {
        //                    _dataContext.keyValueDecisionCollection = (KeyValueCollection)application.Options[section];
        //                    foreach (string decisionKey in _dataContext.keyValueDecisionCollection.AllKeys)
        //                    {
        //                        if (decisionKey.Equals("EnableActiveDirectory"))
        //                        {
        //                            _dataContext.IsEnableActiveDirectory = Convert.ToBoolean(_dataContext.keyValueDecisionCollection[decisionKey]);
        //                            break;
        //                        }
        //                    }
        //                }
        //                else if (string.Compare(section, "Log", true) == 0)
        //                {
        //                    _dataContext.keyValueLogCollection = (KeyValueCollection)application.Options[section];
        //                }
        //                else if (string.Compare(section, "Image Path", true) == 0)
        //                {
        //                    _dataContext.keyValueImagePathCollection = (KeyValueCollection)application.Options[section];
        //                }
        //                else if (string.Compare(section, "ActiveDirectory", true) == 0)
        //                {
        //                    KeyValueCollection keyActiveDirectoryCollection = new KeyValueCollection();
        //                    keyActiveDirectoryCollection = (KeyValueCollection)application.Options[section];
        //                    foreach (string strActiveDirectory in keyActiveDirectoryCollection.AllKeys)
        //                    {
        //                        if (strActiveDirectory.Contains("Domain"))
        //                        {
        //                            _dataContext.domainName = keyActiveDirectoryCollection[strActiveDirectory].ToString();
        //                        }
        //                        if (strActiveDirectory.Contains("UserGroup"))
        //                        {
        //                            _dataContext.userGroup = keyActiveDirectoryCollection[strActiveDirectory].ToString();
        //                        }
        //                    }
        //                }
        //            }
        //        }
        //        else
        //        {
        //            logger.Error("Application with name :" + _dataContext.ApplicationName + " " + "doesn't exists in the CME");
        //        }
        //    }
        //    catch (Exception commonException)
        //    {
        //        logger.Error("Error occurred while ReadQueuesCollection " + commonException.ToString());
        //    }
        //    finally
        //    {
        //        // GC.Collect();
        //    }
        //}
        /// <summary>
        /// Reads the application objects.
        /// </summary>
        /// <param name="applicationName">Name of the application.</param>
        /// <returns></returns>
        /// <summary>
        /// Authenticates the user.
        /// </summary>
        /// <param name="userName">Name of the user.</param>
        /// <param name="password">The password.</param>
        /// <returns></returns>
        /// <summary>
        /// Add/update skill to agent.
        /// </summary>
        /// <param name="operationType">Type of the operation.</param>
        /// <param name="userName">Name of the user.</param>
        /// <param name="skillName">Name of the skill.</param>
        /// <param name="skillLevel">The skill level.</param>
        /// <summary>
        /// Deletes the skill from agent.
        /// </summary>
        /// <param name="userName">Name of the user.</param>
        /// <param name="skillName">Name of the skill.</param>
        /// <returns></returns>

        #endregion Other
    }
}