using System.Collections.Generic;
using Sample.Crm.Business.Agents;
using SEV.Crm.Business;
using SEV.Crm.Business.Agents;
using SEV.Crm.Business.Configurators;
using Sample.Crm.Entities;

namespace Sample.Crm.Business.Configurators
{
    public class PreValidateCreateIncidentBusinessConfigurator : BusinessConfigurator
    {
        public override IEnumerable<IBusinessAgent> Configure(IPluginExecutorContext context)
        {
            var targetEntity = context.GetTargetEntity<Incident>();
            if (targetEntity.ResponsibleContactId == null)
            {
                return new List<IBusinessAgent>();
            }

            var businessAgents = new List<IBusinessAgent>
            {
                new IncidentCustomerContactValidator
                {
                    ExecutionOrder = 1, Context = new object[] { CreateCrmServiceContext(context) }
                }
            };
            return businessAgents;
        }
    }
}