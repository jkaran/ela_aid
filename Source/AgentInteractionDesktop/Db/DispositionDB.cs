
using Pointel.Common.DataBase;
using Pointel.Configuration.Manager;
using System;
namespace Agent.Interaction.Desktop.Db
{
    public class DispositionDB : System.IDisposable
    {
        private Pointel.Logger.Core.ILog _logger = null;
        public DispositionDB()
        {
            _logger = Pointel.Logger.Core.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType,
           "AID");
        }
        public bool StorerDispositionDetail(DispositionDetail objDispositionDetail, out string ErrorMessage)
        {
            ErrorMessage = string.Empty;
            try
            {
                string _query = GenerateInserQuery(objDispositionDetail);
                _logger.Info("Query: " + _query);
                Database db = new Database();
                db.Provider = "System.Data.OracleClient";
                _logger.Info("Forming connection string for disposition db.");
                if (ConfigContainer.Instance().AllKeys.Contains("disposition.db.servicename"))
                    db.ConnectionString = "Data Source = (Description=(Address_list=(Address=(Protocol=TCP)(HOST=" + GetValue("disposition.db.host") + ")(PORT=" + GetValue("disposition.db.port") + ")))" +
                        "(CONNECT_DATA=(SERVICE_NAME=" + GetValue("disposition.db.servicename") + ")));User Id =" + GetValue("disposition.db.userid") + ";Password=" + GetValue("disposition.db.Password");
                else if (ConfigContainer.Instance().AllKeys.Contains("disposition.db.sid"))
                    db.ConnectionString = "Data Source = (Description=(Address_list=(Address=(Protocol=TCP)(HOST=" + GetValue("disposition.db.host") + ")(PORT=" + GetValue("disposition.db.port") + ")))" +
                    "(CONNECT_DATA=(SID=" + GetValue("disposition.db.sid") + ")));User Id =" + GetValue("disposition.db.userid") + ";Password=" + GetValue("disposition.db.Password");
               
                _logger.Info("Disposition db connection string : " + db.ConnectionString);

                if (string.IsNullOrEmpty(db.ConnectionString))
                    throw new Exception("The database connection string is not configured to disposition database.");
                db.CreateConnection(true);
                db.ExecuteNonQuery(_query);
                db.CloseConnection();
                db = null;
                ErrorMessage = "Success";
                return true;
            }
            catch (System.Exception _generalException)
            {
                ErrorMessage = _generalException.Message;
                _logger.Error("Error Occurred as " + _generalException.Message);
            }
            return false;
        }

        private string GetValue(string KeyName)
        {
            var value = string.Empty;
            value = ConfigContainer.Instance().AllKeys.Contains(KeyName) ? ConfigContainer.Instance().GetValue(KeyName) : string.Empty;
            return value;
        }
        private string GenerateInserQuery(DispositionDetail objDispositionDetail)
        {
            string query = string.Empty;

            //if (objDispositionDetail.TcCode==0)
            //query = "insert into DISPOSITION (ELV_LOB, ELV_MEDIA_TYPE, ELV_TERM_DESC, AGENTID, DBA, CHANNEL, ANI, CONNECTIONID, DNIS, TALKTIME, SEQUENCEID, MID, GENESYS_CALL_RESULT, CALLTYPE, USERID, APPID, NOTES,DATECREATED,ELV_SUBMIT_FLAG,ELV_CALLTYPE2) values(q'#" + objDispositionDetail.LOB+ "#',q'#" + objDispositionDetail.MediaType+ "#',q'#" + objDispositionDetail.TcDescription
            //   + "#',q'#" + objDispositionDetail.AgentId+ "#',q'#" + objDispositionDetail.NameValue+ "#',q'#" + objDispositionDetail.LOB+ "#',q'#" + objDispositionDetail.PhoneValue+ "#',q'#" + objDispositionDetail.ConnectionId+ "#',q'#" + objDispositionDetail.DNIS+ "#',q'#" + objDispositionDetail.CallDuration + "#',sequenceId.nextval,q'#" + objDispositionDetail.MID + "#',33,q'#"
            //+objDispositionDetail.CallTypeCode+ "#',q'#" + objDispositionDetail.Username+ "#',q'#" + objDispositionDetail.ApplicationId+ "#',q'#" + objDispositionDetail.Notes + "#',SYSDATE,q'#" + objDispositionDetail.ELV_SubmitFlag+ "#',q'#" + objDispositionDetail.CallType + "#')";

            query = "insert into DISPOSITION (ELV_LOB, ELV_MEDIA_TYPE, ELV_TERM_DESC, AGENTID, DBA, CHANNEL, ANI, CONNECTIONID, DNIS, TALKTIME, SEQUENCEID, MID, GENESYS_CALL_RESULT, CALLTYPE, USERID, APPID,TERM_CODE, NOTES,DATECREATED,ELV_SUBMIT_FLAG,ELV_CALLTYPE2) values(q'#" + objDispositionDetail.LOB + "#',q'#" + objDispositionDetail.MediaType + "#',q'#" + objDispositionDetail.TcDescription
           + "#',q'#" + objDispositionDetail.AgentId + "#',q'#" + objDispositionDetail.NameValue + "#',q'#" + objDispositionDetail.LOB + "#',q'#" + objDispositionDetail.PhoneValue + "#',q'#" + objDispositionDetail.ConnectionId + "#',q'#" + objDispositionDetail.DNIS + "#',q'#" + objDispositionDetail.CallDuration + "#',disposition_seq.nextval,q'#" + objDispositionDetail.MID + "#',33,q'#"
        + objDispositionDetail.CallTypeCode + "#',q'#" + objDispositionDetail.Username + "#',q'#" + objDispositionDetail.ApplicationId + "#',q'#" + objDispositionDetail.TcCode + "#',q'#" + objDispositionDetail.Notes + "#',SYSDATE,q'#" + objDispositionDetail.ELV_SubmitFlag + "#',q'#" + objDispositionDetail.CallType + "#')";
            return query;
        }


        public void Dispose()
        {
            _logger = null;
        }
    }
}
