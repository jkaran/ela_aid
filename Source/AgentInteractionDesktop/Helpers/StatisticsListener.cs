using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Windows;
using Agent.Interaction.Desktop.Settings;
using Pointel.Interactions.IPlugins;
using Pointel.IPlugin.Interface;
using Pointel.Statistics.Core;
using System.Windows.Media;
using System.Windows.Threading;
using Genesyslab.Platform.Reporting.Protocols.StatServer;
using Pointel.Configuration.Manager;
using Genesyslab.Platform.ApplicationBlocks.ConfigurationObjectModel.Queries;
using Genesyslab.Platform.ApplicationBlocks.ConfigurationObjectModel.CfgObjects;

namespace Agent.Interaction.Desktop.Helpers
{
    public class StatisticsListener : IStatTicker
    {
        #region Delegate Members

        public delegate void NotifyCCStatistics(bool isCCStatistics);
        public delegate void NotifyMyStatistics(bool isMyStatistics);

        #endregion

        #region Events Declaration

        public static event NotifyCCStatistics _notifyCCStatistics;
        public static event NotifyMyStatistics _notifyShowMyStatistics;

        #endregion

        #region Private Members Declaration

        private Pointel.Logger.Core.ILog _logger = Pointel.Logger.Core.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType,
            "AID");
        public StatisticsBase statSubscribe = new StatisticsBase();
        public IStatTicker statTickerListener;
        int state;
        Pointel.Interactions.IPlugins.AgentMediaStatus status;
        private Datacontext _dataContext = Datacontext.GetInstance();
        private ConfigContainer _configContainer = ConfigContainer.Instance();
        private PluginCollection _plugins = PluginCollection.GetInstance();
        #endregion

        /// <summary>
        /// Initializes a new instance of the <see cref="StatisticsListener"/> class.
        /// </summary>
        public StatisticsListener()
        {

        }

        /// <summary>
        /// Initialises the stat ticker.
        /// </summary>
        /// <param name="statListener">The stat listener.</param>
        public void InitialiseStatTicker()
        {
            try
            {
                Pointel.Statistics.Core.Utility.OutputValues Output = new Pointel.Statistics.Core.Utility.OutputValues();

                if (((_configContainer.AllKeys.Contains("statistics.enable-mystat-aid") &&
                    ((string)_configContainer.GetValue("statistics.enable-mystat-aid")).ToLower().Equals("true")) ||
                    (_configContainer.AllKeys.Contains("statistics.enable-ccstat-aid") &&
                    ((string)_configContainer.GetValue("statistics.enable-ccstat-aid")).ToLower().Equals("true"))))
                {
                    _logger.Debug("Window_Loaded : StatTicker Plugin Subscription Started");
                    statTickerListener = this;
                    statSubscribe.Subscribe("AID", statTickerListener);
                    _logger.Debug("Window_Loaded : StatTicker Plugin Subscriptn Completed");

                        List<CfgAgentGroup> agentGroup=new List<CfgAgentGroup>();
                    if (_configContainer.AllKeys.Contains("CfgAgentGroup"))
                    {
                        agentGroup = _configContainer.GetValue("CfgAgentGroup");
                    }
                    Output = statSubscribe.ConfigConnectionEstablish(_dataContext.ApplicationName, _configContainer.ConfServiceObject, _dataContext.UserName, _dataContext.UserName, "StatServer", agentGroup);

                   if (Output.MessageCode == "200")
                   {
                       statSubscribe.StartStatistics(_dataContext.UserName, _dataContext.ApplicationName,0,0, true, false);

                   }
                }
            }
            catch (Exception commonException)
            {
                _logger.Error("InitialiseStatTicker : " + commonException.Message.ToString());
            }
        }

        #region IStatTicker Interface Members

        public void NotifyAgentGroupStatistics(string refid, string statisticsName, string statisticsValue, string toolTip, System.Drawing.Color statColor, string statType, bool isThresholdBreach, bool isLevelTwo)
        {
            SoftPhoneBar_StatDisplayMessage(statisticsName, statisticsValue, statType, toolTip, statColor);
        }


        public void NotifyAgentStatistics(string refid, string statisticsName, string statisticsValue, string toolTip, System.Drawing.Color statColor, string statType, bool isThresholdBreach, bool isLevelTwo)
        {
            SoftPhoneBar_StatDisplayMessage(statisticsName, statisticsValue, statType, toolTip, statColor);
        }

        public void NotifyDBStatistics(string refid, string statisticsName, string statisticsValue, string toolTip, System.Drawing.Color statColor, bool isThresholdBreach, string dbStatName, bool isLevelTwo)
        {
        }

        public void NotifyQueueStatistics(string refid, string statisticsName, string statisticsValue, string toolTip, System.Drawing.Color statColor, string statType, bool isThresholdBreach, bool isLevelTwo)
        {
            SoftPhoneBar_StatDisplayMessage(statisticsName, statisticsValue, statType, toolTip, statColor);
        }

        public void NotifyGadgetStatus(Pointel.Statistics.Core.Utility.StatisticsEnum.GadgetState gadgetstate)
        {
            _logger.Debug("NotifyGadgetStatus : " + gadgetstate.ToString());
            _dataContext.GadgetState = gadgetstate.ToString();
            if (gadgetstate == Pointel.Statistics.Core.Utility.StatisticsEnum.GadgetState.Opened)
                _dataContext.NotifyGadgetDisplayName = "Close Stat Gadget";
            else if (gadgetstate == Pointel.Statistics.Core.Utility.StatisticsEnum.GadgetState.Closed || gadgetstate == Pointel.Statistics.Core.Utility.StatisticsEnum.GadgetState.Ended)
                _dataContext.NotifyGadgetDisplayName = "Show Stat Gadget";
        }

        public void NotifyShowCCStatistics(bool isCCStatistics)
        {
            _notifyCCStatistics.Invoke(isCCStatistics);
        }

        public void NotifyShowMyStatistics(bool isMyStatistics)
        {
            _notifyShowMyStatistics.Invoke(isMyStatistics);
        }

        public void NotifyStatErrorMessage(Pointel.Statistics.Core.Utility.OutputValues errorMessage)
        {
            try
            {
                _logger.Debug("NotifyStatErrorMessage : " + "error code " + errorMessage.MessageCode.ToString() + " error message" + errorMessage.Message.ToString());

                if (_plugins.PluginCollections.ContainsKey(Pointel.Interactions.IPlugins.Plugins.TeamCommunicator))
                    ((ITeamCommunicatorPlugin)_plugins.PluginCollections[Pointel.Interactions.IPlugins.Plugins.TeamCommunicator]).NotifyStatServerStatus(errorMessage.Message.ToString());
            }
            catch (Exception ex)
            {
                _logger.Debug("NotifyStatErrorMessage : " + ex.Message.ToString());
            }
        }

        public void NotifyAIDStatistics(Genesyslab.Platform.Reporting.Protocols.StatServer.Events.EventInfo StatisticsEvents)
        {
            try
            {
                if (StatisticsEvents != null)
                {
                    _logger.Trace("Response from Stat Server : " + StatisticsEvents.ToString());
                    if (StatisticsEvents.StateValue != null)
                    {
                        if (_plugins.PluginCollections.ContainsKey(Pointel.Interactions.IPlugins.Plugins.TeamCommunicator))
                            ((ITeamCommunicatorPlugin)_plugins.PluginCollections[Pointel.Interactions.IPlugins.Plugins.TeamCommunicator]).NotifyStatistics(StatisticsEvents);

                        #region Old

                        //Application.Current.Dispatcher.Invoke((Action)(delegate
                        ////{
                        //    #region Agent Statistics

                        //    if (StatisticsEvents.StateValue.ObjectType == StatisticObjectType.Agent && StatisticsEvents.StateValue is AgentStatus)
                        //    {
                        //        AgentStatus agentStatus = (AgentStatus)StatisticsEvents.StateValue;
                        //        state = Convert.ToInt32(agentStatus.Status);
                        //        status = GetStatus(state);
                        //        DnCollection dnCollection = agentStatus.Place.DnStatuses;
                        //        string dn = string.Empty;
                        //        string place = string.Empty;
                        //        Dictionary<string, object> agentStatistics = new Dictionary<string, object>();
                        //        agentStatistics.Clear();
                        //        if (agentStatus.Place != null)
                        //            place = agentStatus.Place.PlaceId;
                        //        agentStatistics.Add("AgentStatus", status);
                        //        status = AgentMediaStatus.LoggedOut;
                        //        agentStatistics.Add("VoiceStatus", status);
                        //        agentStatistics.Add("DN", string.Empty);
                        //        agentStatistics.Add("Place", string.Empty);
                        //        agentStatistics.Add("ChatStatus", status);
                        //        agentStatistics.Add("EmailStatus", status);
                        //        if (dnCollection != null && dnCollection.Count > 0)
                        //        {
                        //            foreach (DnStatus dnStatus in dnCollection)
                        //            {
                        //                try
                        //                {
                        //                    if (dnStatus != null)
                        //                    {
                        //                        if (dnStatus.SwitchId != null)  //For media Voice
                        //                        {
                        //                            status = GetStatus(dnStatus.Status.Value);
                        //                            dn = dnStatus.DnId;
                        //                            agentStatistics["VoiceStatus"] = status;
                        //                            agentStatistics["DN"] = !(string.IsNullOrEmpty(dn)) ? dn : string.Empty;
                        //                            agentStatistics["Place"] = !(string.IsNullOrEmpty(place)) ? place : string.Empty;
                        //                        }
                        //                        else
                        //                        {
                        //                            if (dnStatus.DnId.ToString().ToLower().Contains("chat"))  //For media Chat
                        //                            {
                        //                                status = GetStatus(dnStatus.Status.Value);
                        //                                agentStatistics["ChatStatus"] = status;
                        //                            }
                        //                            else if (dnStatus.DnId.ToString().ToLower().Contains("email"))  //For media Email
                        //                            {
                        //                                status = GetStatus(dnStatus.Status.Value);
                        //                                agentStatistics["EmailStatus"] = status;
                        //                            }
                        //                        }
                        //                    }
                        //                }
                        //                catch (Exception ex)
                        //                {
                        //                    _logger.Error("Error occurred as " + ex.Message);
                        //                }
                        //            }
                        //        }
                        //        if (_plugins.PluginCollections.ContainsKey(Pointel.Interactions.IPlugins.Plugins.TeamCommunicator))
                        //            ((ITeamCommunicatorPlugin)_plugins.PluginCollections[Pointel.Interactions.IPlugins.Plugins.TeamCommunicator]).NotifyStatistics(agentStatus.AgentId.ToString(), "Agent", agentStatistics);
                        //        agentStatistics = null;
                        //    }
                        //    #endregion

                        //    #region Agent Group Statistics

                        //    else if (StatisticsEvents.StateValue.ObjectType == StatisticObjectType.GroupAgents)
                        //    {
                        //        Dictionary<string, object> agentGroupStatistics = new Dictionary<string, object>();
                        //        agentGroupStatistics.Clear();
                        //        AgentGroup aGroup = StatisticsEvents.StateValue as AgentGroup;
                        //        state = Convert.ToInt32(aGroup.Status);
                        //        status = GetStatus(state);
                        //        agentGroupStatistics.Add("Status", status);
                        //        if (_plugins.PluginCollections.ContainsKey(Pointel.Interactions.IPlugins.Plugins.TeamCommunicator))
                        //            ((ITeamCommunicatorPlugin)_plugins.PluginCollections[Pointel.Interactions.IPlugins.Plugins.TeamCommunicator]).NotifyStatistics(aGroup.GroupId, "AgentGroup", agentGroupStatistics);
                        //        agentGroupStatistics = null;
                        //    }

                        //    #endregion
                        //})); 
                        #endregion
                    }
                }
            }
            catch (Exception commonException)
            {
                _logger.Error("NotifyAIDStatistics : " + commonException.Message.ToString());
            }
            finally
            {
                StatisticsEvents = null;
            }
        }

        #endregion

        private Pointel.Interactions.IPlugins.AgentMediaStatus GetStatus(int state)
        {
            Pointel.Interactions.IPlugins.AgentMediaStatus status = AgentMediaStatus.None;
            switch (state)
            {
                case 23:
                    status = Pointel.Interactions.IPlugins.AgentMediaStatus.LoggedOut;
                    break;
                case 8:
                    status = Pointel.Interactions.IPlugins.AgentMediaStatus.NotReady;
                    break;
                case 4:
                    status = Pointel.Interactions.IPlugins.AgentMediaStatus.Ready;
                    break;
                case 2:
                    status = Pointel.Interactions.IPlugins.AgentMediaStatus.LoggedIn;
                    break;
                case 6:
                    status = Pointel.Interactions.IPlugins.AgentMediaStatus.Busy;
                    break;
                case 7:
                    status = Pointel.Interactions.IPlugins.AgentMediaStatus.Busy;
                    break;
                case 13:
                    status = Pointel.Interactions.IPlugins.AgentMediaStatus.Busy;
                    break;
                case 20:
                    status = Pointel.Interactions.IPlugins.AgentMediaStatus.Busy;
                    break;
                case 19:
                    status = Pointel.Interactions.IPlugins.AgentMediaStatus.Busy;
                    break;
                case 21:
                    status = Pointel.Interactions.IPlugins.AgentMediaStatus.Busy;
                    break;
                case 22:
                    status = Pointel.Interactions.IPlugins.AgentMediaStatus.Busy;
                    break;
                case 18:
                    status = Pointel.Interactions.IPlugins.AgentMediaStatus.Busy;
                    break;
                default:
                    break;
            }
            return status;

        }

        /// <summary>
        ///     Softs the phone bar__ stat display message.
        /// </summary>
        /// <param name="statisticName">Name of the statistic.</param>
        /// <param name="statisticValue">The statistic value.</param>
        /// <param name="statType">Type of the stat.</param>
        /// <param name="toolTip">The tool tip.</param>
        /// <param name="color">The color.</param>
        public void SoftPhoneBar_StatDisplayMessage(string statisticName, string statisticValue,
          string statType, string toolTip, System.Drawing.Color color)
        {
            try
            {
                _logger.Trace("SofphoneBar_From StatServer:" + statisticName + ":" + statisticValue);
                if (statType == "GroupAgents" || statType == "Agent")
                {
                    if (_dataContext.MyStatistics.Count < 0) return;
                    //below code added for display agent group name
                    //Smoorthy 08-01-2014
                    if (statType == "GroupAgents")
                    {
                        var groupName = toolTip.Substring(0, toolTip.IndexOf('\n'));
                        //Code added by Manikandan on 24-03-2014 to remove group in agent group statistics in my statistics tab
                        statisticName = statisticName.Replace("Group", "") + " - " + groupName;
                        groupName = string.Empty;
                    }
                    //Code added by Manikandan on 24-03-2014 to remove agent in agent statistics in my statistics tab
                    if (statType == "Agent")
                    {
                        string statName = statisticName.Replace("Agent", "");
                        statisticName = statName;
                    }
                    //End
                    //Code added by Manikandan added to update my statistics only when my statistics is enabled
                    if (_configContainer.AllKeys.Contains("statistics.enable-mystat-aid") &&
                    ((string)_configContainer.GetValue("statistics.enable-mystat-aid")).ToLower().Equals("true"))
                    {
                        Application.Current.Dispatcher.BeginInvoke((Action)(delegate
                        {
                            var item = new MyStatistics(statisticName, statisticValue,
                                              new SolidColorBrush(System.Windows.Media.Color.FromArgb(color.A, color.R,
                                                  color.G, color.B)), statType.ToString());
                            if (_dataContext.MyStatistics.Any(p => p.StatisticName == statisticName))
                            {
                                var i =
                                    _dataContext
                                        .MyStatistics.IndexOf(
                                            _dataContext.MyStatistics.Where(p => p.StatisticName == statisticName)
                                                .FirstOrDefault());
                                _dataContext.MyStatistics[i] = item;
                            }
                            else
                            {
                                if (statType == "Agent" || statType == "GroupAgents")
                                    _dataContext.MyStatistics.Add(item);
                            }
                        }), DispatcherPriority.DataBind);

                    }
                }
                if (statType == "Queue" || statType == "GroupPlaces")
                {
                    if (_dataContext.ContactCenterStatistics.Count < 0) return;
                    //Code added by Manikandan added to update contact center statistics only when contact center statistics is enabled
                    if (_configContainer.AllKeys.Contains("statistics.enable-ccstat-aid") &&
                    ((string)_configContainer.GetValue("statistics.enable-ccstat-aid")).ToLower().Equals("true"))
                    {
                        Application.Current.Dispatcher.BeginInvoke((Action)(delegate
                        {
                            var item = new ContactCenterStatistics(statisticName, toolTip, statisticValue,
                                            new SolidColorBrush(System.Windows.Media.Color.FromArgb(color.A, color.R,
                                                color.G, color.B)), statType.ToString());
                            if (_dataContext.ContactCenterStatistics.Any(p => p.ContactStatisticName == statisticName))
                            {
                                int i =
                                    _dataContext
                                        .ContactCenterStatistics.IndexOf(
                                            _dataContext
                                                .ContactCenterStatistics.Where(
                                                    p => p.ContactStatisticName == statisticName)
                                                .FirstOrDefault());
                                _dataContext.ContactCenterStatistics[i] = item;
                            }
                            else
                                _dataContext.ContactCenterStatistics.Add(item);
                        }), DispatcherPriority.DataBind);
                    }
                }
            }
            catch (Exception commonException)
            {
                _logger.Error("Error occurred as " + commonException.Message);
            }
        }

        public void NotifyAgentStatus(string agentid, string status)
        {
            //throw new NotImplementedException();
        }



        public void NotifyStatServerStatustoTC(bool status, string serverName)
        {
            throw new NotImplementedException();
        }
    }
}
