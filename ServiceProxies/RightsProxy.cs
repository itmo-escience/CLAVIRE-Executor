using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ServiceProxies.ResourceBaseService;
using Config = System.Web.Configuration.WebConfigurationManager;

namespace MITP
{
    public class RightsProxy
    {
        public const string RIGHTS_ENABLED_PARAM = "Rights.Enabled";

        public static bool IsRightsIgnored() // todo : m.b. property?
        {
            string rightsEnabledStr = Config.AppSettings[RIGHTS_ENABLED_PARAM];
            if (rightsEnabledStr != null && rightsEnabledStr.ToLowerInvariant() == "false")
            {
                Log.Warn("Rights check ignored");
                return true;
            }

            return false;
        }

        public static string[] GetAllowedResourceNames(string userId, IEnumerable<Resource> resources)
        {
            var rightsService = Discovery.GetRightsService();
            string[] allowedResources = new string[0];

            try
            {
                allowedResources = rightsService.GetUserResourcesFromList(userId, resources.Select(r => r.ResourceName).ToArray());
                rightsService.Close();
            }
            catch (Exception e)
            {
                rightsService.Abort();
                Log.Error("Exception on getting allowed resources from RightsService for user " + userId + ": " + e.ToString());
            }

            if (IsRightsIgnored())    
                allowedResources = resources.Select(r => r.ResourceName).ToArray();

            Log.Info(String.Format("Allowed resources for user '{0}': [{1}]", userId, String.Join(", ", allowedResources)));
            return allowedResources;
        }

        public static bool CanExecutePackage(string userId, string packageName) // todo : use CanExecutePackage
        {
            var rightsService = Discovery.GetRightsService();
            bool isPackageAllowed = false;

            try
            {
                isPackageAllowed = rightsService.CanExecutePackage(userId, packageName);
                rightsService.Close();
            }
            catch (Exception e)
            {
                rightsService.Abort();
                Log.Error("Exception on check from RightsService if user " + userId + " can use package " + packageName + ": " + e.ToString());
            }

            if (IsRightsIgnored())
                isPackageAllowed = true;

            return isPackageAllowed;
        }

        public static bool HasMoney(string userId)
        {
            //Log.Info(String.Format("Checking money deposit for user '{0}'", userId));
            var accountingService = Discovery.GetAccountingService();
            double money = 0;

            try
            {
                money = accountingService.GetUserQuotingInfo(userId);
                accountingService.Close();
            }
            catch (Exception e)
            {
                accountingService.Abort();
                Log.Error("Exception on checking money deposit for user " + userId + ": " + e.ToString());
            }

            Log.Info(String.Format("User's '{0}' money deposit = {1}", userId, money));

            bool hasMoney = (money > 0);
            if (IsRightsIgnored())
                hasMoney = true;

            return hasMoney;
        }
    }
}