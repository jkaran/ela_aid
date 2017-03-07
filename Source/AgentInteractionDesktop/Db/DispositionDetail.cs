using System.Text;
namespace Agent.Interaction.Desktop.Db
{
    public class DispositionDetail
    {
        #region Properties
        private bool xsubmitFlag;
        public string ConnectionId { get; set; }
        public string AgentId { get; set; }
        public string DNIS { get; set; }
        public string PhoneValue { get; set; }
        public string MID { get; set; }
        public string Username { get; set; }
        public string NameValue { get; set; }
        public string LOB { get; set; }
        public int TcCode { get; set; }
        public string TcDescription { get; set; }
        public string ApplicationId { get; set; }
        public string Notes { get; set; }
        public int CallDuration { get; set; }
        public byte CallTypeCode { get; set; }
        private string callType = null;
        public string CallType
        {
            get
            {
                return callType;
            }
            set
            {
                callType = value;
                if (callType != null && callType.ToLower().Equals("inbound"))
                {
                    CallTypeCode = 2;
                }
                else if (callType != null && callType.ToLower().Equals("outbound"))
                {
                    CallTypeCode = 3;
                }
            }
        }
        public int SubmitFlag { get; set; }
        public string MediaType { get; set; }
        public string CallResult { get; set; }
        public string ELV_SubmitFlag
        {
            get;
            private set;
        }
        public bool X_SubmitFlag
        {
            get
            {
                return xsubmitFlag;
            }
            set
            {
                xsubmitFlag = value;
                if (xsubmitFlag)
                    ELV_SubmitFlag = "N";
                else
                    ELV_SubmitFlag = "Y";
            }
        }
        #endregion

        public DispositionDetail()
        {
            MediaType = "Voice";
            ELV_SubmitFlag = "Y";
            CallResult = "33";
            CallDuration = 0;
            MID = string.Empty;
            ApplicationId = string.Empty;
            PhoneValue = string.Empty;
            DNIS = string.Empty;
            Notes = string.Empty;
        }

        public override string ToString()
        {
            StringBuilder objStringBuilder = new StringBuilder();
            if (string.IsNullOrEmpty(ConnectionId))
                objStringBuilder.Append("ConnectionId=null");
            else
                objStringBuilder.Append("ConnectionId=" + ConnectionId);

            if (string.IsNullOrEmpty(AgentId))
                objStringBuilder.Append("AgentId=null");
            else
                objStringBuilder.Append("AgentId=" + AgentId);

            if (string.IsNullOrEmpty(DNIS))
                objStringBuilder.Append("DNI=null");
            else
                objStringBuilder.Append("DNI=" + DNIS);

            if (string.IsNullOrEmpty(PhoneValue))
                objStringBuilder.Append("PhoneValue=null");
            else
                objStringBuilder.Append("PhoneValue=" + PhoneValue);
            
            objStringBuilder.Append("MID=" + ConnectionId);
            
            if (string.IsNullOrEmpty(Username))
                objStringBuilder.Append("Username=null");
            else
                objStringBuilder.Append("Username=" + Username);
            
            if (string.IsNullOrEmpty(NameValue))
                objStringBuilder.Append("NameValue=null");
            else
                objStringBuilder.Append("NameValue=" + NameValue);
            
            objStringBuilder.Append("LOB=" + LOB);
            
            objStringBuilder.Append("TcCode=" + TcCode);
            
            objStringBuilder.Append("TcDescription=" + TcDescription);
            
            if (string.IsNullOrEmpty(ApplicationId))
                objStringBuilder.Append("ApplicationId=null");
            else
                objStringBuilder.Append("ApplicationId=" + ApplicationId);
            
            if (string.IsNullOrEmpty(Notes))
                objStringBuilder.Append("Notes=null");
            else
                objStringBuilder.Append("Notes=" + Notes);

            //if (string.IsNullOrEmpty(CallDuration))
            //    objStringBuilder.Append("CallDuration=null");
            //else
                objStringBuilder.Append("CallDuration=" + CallDuration);
            
            objStringBuilder.Append("CallTypeCode=" + CallTypeCode);

            if (string.IsNullOrEmpty(CallType))
                objStringBuilder.Append("CallType=null");
            else
                objStringBuilder.Append("CallType=" + CallType);
            
            objStringBuilder.Append("SubmitFlag=" + SubmitFlag);
            
            if (string.IsNullOrEmpty(MediaType))
                objStringBuilder.Append("MediaType=null");
            else
                objStringBuilder.Append("MediaType=" + MediaType);
            
            if (string.IsNullOrEmpty(CallResult))
                objStringBuilder.Append("CallResult=null");
            else
                objStringBuilder.Append("CallResult=" + CallResult);
           
            if (string.IsNullOrEmpty(ELV_SubmitFlag))
                objStringBuilder.Append("ELV_SubmitFlag=null");
            else
                objStringBuilder.Append("ELV_SubmitFlag=" + ELV_SubmitFlag);
           
            objStringBuilder.Append("X_SubmitFlag=" + X_SubmitFlag);
            return objStringBuilder.ToString();
        }
    }
}
