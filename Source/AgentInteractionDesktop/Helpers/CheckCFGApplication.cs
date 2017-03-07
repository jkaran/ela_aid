using System;
using Pointel.Configuration.Manager;

namespace Agent.Interaction.Desktop
{
    class CheckCFGApplication
    {
        public static bool CheckCFGAppisExist(string appName, string[] appNames = null)
        {
            try
            {
                if (appNames != null && appNames.Length>0)
                    appName = appNames[0];

                if (!string.IsNullOrEmpty(appName))
                {
                    Genesyslab.Platform.ApplicationBlocks.ConfigurationObjectModel.Queries.CfgApplicationQuery appQuery =
                        new Genesyslab.Platform.ApplicationBlocks.ConfigurationObjectModel.Queries.CfgApplicationQuery();
                    appQuery.Name = appName.Trim();
                    appQuery.TenantDbid = ConfigContainer.Instance().TenantDbId;
                    Genesyslab.Platform.ApplicationBlocks.ConfigurationObjectModel.CfgObjects.CfgApplication appObject =
                        ConfigContainer.Instance().ConfServiceObject.RetrieveObject<Genesyslab.Platform.ApplicationBlocks.ConfigurationObjectModel.CfgObjects.CfgApplication>(appQuery);
                    return appObject != null;
                }
            }
            catch (Exception)
            {
            }
            return false;
        }
    }
}
