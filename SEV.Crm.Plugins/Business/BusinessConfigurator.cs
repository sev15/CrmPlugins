using System.Collections.Generic;
using SEV.Crm.Business.Agents;
using SEV.Crm.ServiceContext;
using SEV.Crm.Services;

namespace SEV.Crm.Business.Configurators
{
    public abstract class BusinessConfigurator
    {
        protected ICrmServiceContext CreateCrmServiceContext(IPluginExecutorContext executorContext)
        {
            var serviceContextFactory = CrmServiceProvider.Current.GetService<ICrmServiceContextFactory>();

            return serviceContextFactory.CreateServiceContext(executorContext.OrganizationService);
        }

        public abstract IEnumerable<IBusinessAgent> Configure(IPluginExecutorContext context);
    }
}
