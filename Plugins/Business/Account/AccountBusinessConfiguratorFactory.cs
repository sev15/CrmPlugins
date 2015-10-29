using System;
using System.Collections.Generic;
using Sample.Crm.Business.Configurators;
using SEV.Crm.Business;
using SEV.Crm.Business.Configurators;

namespace Sample.Crm.Business
{
    public class AccountBusinessConfiguratorFactory : BusinessConfiguratorFactory
    {
        protected override List<Tuple<CrmPluginEvent, BusinessConfigurator>> CreateConfiguratorEntries()
        {
            return new List<Tuple<CrmPluginEvent, BusinessConfigurator>>
            {
                new Tuple<CrmPluginEvent, BusinessConfigurator>(CrmPluginEvent.PostUpdate,
                                                                new PostUpdateAccountBusinessConfigurator())
            };
        }

        protected override string GetFactoryName()
        {
            return "AccountBusinessConfiguratorFactory";
        }
    }
}
