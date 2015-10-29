using System;
using System.Collections.Generic;
using Sample.Crm.Business.Configurators;
using SEV.Crm.Business;
using SEV.Crm.Business.Configurators;

namespace Sample.Crm.Business
{
    public class IncidentBusinessConfiguratorFactory : BusinessConfiguratorFactory
    {
        protected override List<Tuple<CrmPluginEvent, BusinessConfigurator>> CreateConfiguratorEntries()
        {
            return new List<Tuple<CrmPluginEvent, BusinessConfigurator>>
            {
                new Tuple<CrmPluginEvent, BusinessConfigurator>(CrmPluginEvent.PreValidateCreate,
                                                                new PreValidateCreateIncidentBusinessConfigurator()),
                new Tuple<CrmPluginEvent, BusinessConfigurator>(CrmPluginEvent.PreCreate,
                                                                new PreCreateIncidentBusinessConfigurator()),
                new Tuple<CrmPluginEvent, BusinessConfigurator>(CrmPluginEvent.PreValidateUpdate,
                                                                new PreValidateUpdateIncidentBusinessConfigurator())
            };
        }

        protected override string GetFactoryName()
        {
            return "IncidentBusinessConfiguratorFactory";
        }
    }
}
