namespace Agent.Interaction.Desktop.Helpers
{
    using System.Collections.Generic;
    using System.Collections.ObjectModel;

    using Agent.Interaction.Desktop.Settings;

    using Genesyslab.Platform.ApplicationBlocks.ConfigurationObjectModel.CfgObjects;
    using Genesyslab.Platform.Commons.Collections;

    using Pointel.Configuration.Manager;

    public class RequeueSkills
    {
        #region Properties

        public string Category
        {
            get;
            set;
        }

        public string Skill
        {
            get;
            set;
        }

        public string Value
        {
            get;
            set;
        }

        #endregion Properties
    }

    public class RequeueSkillSet
    {
        #region Methods

        public List<RequeueSkills> RetrieveRequeueSkillSet(string businessAttributeFullPath)
        {
            var _dataContext = Datacontext.GetInstance();
            var listRequeueSkills = new List<RequeueSkills>();
            if (businessAttributeFullPath == "$AllSkills$")
            {
                foreach (var item in _dataContext.LoadAllSkills)
                {
                    listRequeueSkills.Add(new RequeueSkills() { Skill = item, Value = item });
                }

            }
            else if (!string.IsNullOrEmpty(businessAttributeFullPath) && businessAttributeFullPath.Contains("/"))
            {
                string[] attribName = businessAttributeFullPath.Split('/');
                if (attribName.Length == 2)
                {
                    ConfigManager objConfigManager = new ConfigManager();
                    CfgEnumeratorValue objBusinessAttribute = objConfigManager.GetBusinessAttribute(attribName[0], attribName[1]);
                    if (objBusinessAttribute != null && objBusinessAttribute.UserProperties != null)
                    {
                        foreach (var item in objBusinessAttribute.UserProperties.AllKeys)
                        {
                            var conditionData = objBusinessAttribute.UserProperties.GetAsKeyValueCollection(item.ToString());
                            foreach (var key in conditionData.AllKeys)
                            {
                                var requeueSkills = new RequeueSkills();
                                requeueSkills.Category = item.ToString();
                                requeueSkills.Value = key.ToString();
                                requeueSkills.Skill = conditionData[key].ToString();
                                listRequeueSkills.Add(requeueSkills);
                            }
                        }
                        //return listRequeueSkills;
                    }
                }
            }
            if (listRequeueSkills.Count == 0)
                listRequeueSkills.Add(new RequeueSkills() { Skill = "None", Value = "None", Category = "None" });
            return listRequeueSkills;
        }

        #endregion Methods
    }
}