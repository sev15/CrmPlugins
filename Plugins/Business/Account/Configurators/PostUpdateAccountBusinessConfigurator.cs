using System.Collections.Generic;
using Sample.Crm.Business.Agents;
using Sample.Crm.Entities;
using SEV.Crm.Business;
using SEV.Crm.Business.Agents;
using SEV.Crm.Business.Configurators;

namespace Sample.Crm.Business.Configurators
{
    public class PostUpdateAccountBusinessConfigurator : BusinessConfigurator
    {
        public override IEnumerable<IBusinessAgent> Configure(IPluginExecutorContext context)
        {
            var targetEntity = context.GetTargetEntity<Account>();
            if (!targetEntity.Attributes.ContainsKey("primarycontactid"))
            {
                return new List<IBusinessAgent>();
            }

            var businessAgents = new List<IBusinessAgent>
            {
                new IncidentsCustomerContactUpdater
                {
                    ExecutionOrder = 1, Context = new object[] { CreateCrmServiceContext(context) }
                }
            };
            return businessAgents;
        }
    }
}