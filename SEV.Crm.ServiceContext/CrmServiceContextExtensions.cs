using System;
using Microsoft.Xrm.Client;
using Microsoft.Xrm.Client.Services;

namespace SEV.Crm.ServiceContext.Extensions
{
    public static class CrmServiceContextExtensions
    {
        public static void RemoveCachedData(this ICrmServiceContext context, string entityLogicalName, Guid? id)
        {
            var serviceContainer = context as IOrganizationServiceContainer;
            if (serviceContainer == null)
            {
                return;
            }
            var cachedOrgService = serviceContainer.Service as CachedOrganizationService;
            if (cachedOrgService == null)
            {
                return;
            }
            var orgServiceCache = cachedOrgService.Cache;
            if (orgServiceCache == null)
            {
                return;
            }

            orgServiceCache.Remove(entityLogicalName, id);
        }
    }
}