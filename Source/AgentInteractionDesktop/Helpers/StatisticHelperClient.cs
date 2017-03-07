namespace Agent.Interaction.Desktop.Helpers
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel;
    using System.Diagnostics;
    using System.IO;
    using System.IO.Pipes;
    using System.Linq;
    using System.Threading;

    using Agent.Interaction.Desktop.Settings;
    using Pointel.Statistics.Core;

    #region Enumerations

    public enum StatRequest
    {
        Open = 0,
        Show = 1,
        Hide = 2,
        Close = 3
    }

    #endregion Enumerations

    public class StatisticHelperClient
    {
        #region Methods

        public static bool SendRequest(StatRequest request)
        {
            Thread namedPipeThread = new Thread(() =>
            {
                NamedPipeDataTransfer(request);
            });
            namedPipeThread.Start();
            return false;
        }

        //private static void NamedPipeDataTransfer(StatRequest request)
        //{
        //    Pointel.Logger.Core.ILog logger = Pointel.Logger.Core.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType, "AID");
        //    NamedPipeClientStream namedPipeClient = null;
        //    Datacontext _dataContext = Datacontext.GetInstance();
        //    bool sendData = true;
        //    try
        //    {
        //        if (request == StatRequest.Show || request == StatRequest.Open)
        //        {
        //            var statprocess = Process.GetProcessesByName("StatTickerFive");
        //            if (statprocess == null || statprocess.Length == 0 || (statprocess != null && statprocess.Length != 0 && !statprocess.Any(x => x.Id.ToString().Trim().Contains(_dataContext.StatProcessId.ToString()))))
        //            {
        //                string[] args = { "UserName:" + _dataContext.UserName,
        //                                       "Password:" + _dataContext.Password,
        //                                       "AppName:" + _dataContext.ApplicationName,
        //                                       "Host:" + (string.IsNullOrEmpty(_dataContext.HostNameSelectedValue) ? _dataContext.HostNameText : _dataContext.HostNameSelectedValue),
        //                                       "Port:" + (string.IsNullOrEmpty(_dataContext.PortSelectedValue) ? _dataContext.PortText : _dataContext.PortSelectedValue),
        //                                       "Place:" + _dataContext.Place };
        //                var statProcess = Process.Start(Environment.CurrentDirectory + @"\StatTickerFive.exe", string.Join(",", args));
        //                //var statProcess = Process.Start(@"C:\Program Files\Pointel Inc.,\Agent Interaction Desktop V5.0.3.11\StatTickerFive.exe", string.Join(",", args));
        //                _dataContext.StatProcessId = statProcess.Id;
        //                sendData = false;
        //            }
        //        }
        //        if (sendData)
        //        {
        //            namedPipeClient = new NamedPipeClientStream(".", "AIDPipe_" + _dataContext.UserName, PipeDirection.Out, PipeOptions.Asynchronous);
        //            namedPipeClient.Connect(1000);
        //            if (namedPipeClient.IsConnected)
        //            {
        //                byte[] msgBuff = System.Text.Encoding.UTF8.GetBytes(((int)request).ToString());

        //                namedPipeClient.Write(msgBuff, 0, msgBuff.Length);
        //                namedPipeClient.Flush();
        //                namedPipeClient.Close();
        //                namedPipeClient.Dispose();
        //            }
        //        }
        //    }
        //    catch (Exception generalException)
        //    {
        //        //System.Windows.MessageBox.Show("Stat Request : " + request.ToString() + "Error occurred as " + generalException.Message);
        //        logger.Error("Error occurred as " + generalException.Message);
        //    }
        //    finally
        //    {
        //        if (namedPipeClient != null)
        //        {
        //            if (namedPipeClient.IsConnected)
        //            {
        //                namedPipeClient.Close();
        //            }
        //            namedPipeClient.Dispose();
        //            namedPipeClient = null;
        //        }
        //        logger = null;
        //    }
        //}

        private static void NamedPipeDataTransfer(StatRequest request)
        {
            Pointel.Logger.Core.ILog logger = Pointel.Logger.Core.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType, "AID");
            NamedPipeClientStream namedPipeClient = null;
            Datacontext _dataContext = Datacontext.GetInstance();
            bool sendData = true;

            try
            {
                if (request == StatRequest.Show || request == StatRequest.Open)
                {
                    var statprocess = Process.GetProcessesByName("StatTickerFive");
                    if (statprocess == null || statprocess.Length == 0 || (statprocess != null && statprocess.Length != 0 && !statprocess.Any(x => x.Id.ToString().Trim().Contains(_dataContext.StatProcessId.ToString()))))
                    {
                        //string isGadgetShow = "true";

                        //if (request == StatRequest.QueueConfig)
                        //    isGadgetShow = "false";

                        string[] args = { "UserName:" + _dataContext.UserName,
                                               "Password:" + _dataContext.Password,
                                               "AppName:" + _dataContext.ApplicationName,
                                               "Host:" + (string.IsNullOrEmpty(_dataContext.HostNameSelectedValue) ? _dataContext.HostNameText : _dataContext.HostNameSelectedValue),
                                               "Port:" + (string.IsNullOrEmpty(_dataContext.PortSelectedValue) ? _dataContext.PortText : _dataContext.PortSelectedValue),
                                               "Place:" + _dataContext.Place  ,
                                               "AddpServerTimeOut:"+0,
                                                "AddpClientTimeOut:"+0,
                                               "IsConfigQueue:"+false,
                                               "IsGadgetShow:"+true
                                        };
                        var statProcess = Process.Start(Environment.CurrentDirectory + @"\StatTickerFive.exe", string.Join(",", args));
                        //var statProcess = Process.Start(@"C:\Program Files\Pointel Inc.,\Agent Interaction Desktop V5.0.3.11\StatTickerFive.exe", string.Join(",", args));
                        _dataContext.StatProcessId = statProcess.Id;
                        sendData = false;
                    }
                }
                if (sendData)
                {
                    namedPipeClient = new NamedPipeClientStream(".", "AIDPipe_" + _dataContext.UserName, PipeDirection.Out, PipeOptions.Asynchronous);
                    namedPipeClient.Connect();
                    if (namedPipeClient.IsConnected)
                    {
                        byte[] msgBuff = System.Text.Encoding.UTF8.GetBytes(((int)request).ToString());

                        namedPipeClient.Write(msgBuff, 0, msgBuff.Length);
                        namedPipeClient.Flush();
                        namedPipeClient.Close();
                        namedPipeClient.Dispose();
                    }
                }
            }
            catch (Exception generalException)
            {
                //System.Windows.MessageBox.Show("Stat Request : " + request.ToString() + "Error occurred as " + generalException.Message);
                logger.Error("Error occurred as " + generalException.Message);
            }
            finally
            {
                //if (namedPipeClient != null)
                //{
                //    if (namedPipeClient.IsConnected)
                //    {
                //        namedPipeClient.Close();
                //    }
                //    namedPipeClient.Dispose();
                //    namedPipeClient = null;
                //}
                logger = null;
            }
        }

        public static void CloseStatProtocol()
        {
            StatisticsBase.GetInstance().CloseStatistics();
        }
        #endregion Methods
    }
}