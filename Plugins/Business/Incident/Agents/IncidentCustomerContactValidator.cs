using System;
using System.Linq;
using Microsoft.Xrm.Sdk;
using Sample.Crm.Entities;
using SEV.Crm.Business;
using SEV.Crm.Business.Agents;
using SEV.Crm.ServiceContext;

namespace Sample.Crm.Business.Agents
{
    public class IncidentCustomerContactValidator : CrmBusinessAgent
    {
        public override void Execute(IPluginExecutorContext context)
        {
            base.Execute(context);

            var targetEntity = context.GetTargetEntity<Incident>();
            var crmServiceContext = (ICrmServiceContext)Context[0];

            Guid customerId = targetEntity.Attributes.ContainsKey("customerid") ? targetEntity.CustomerId.Id
                                                            : context.GetPreEntityImage<Incident>().CustomerId.Id;
            var customerAccount = crmServiceContext.CreateQuery<Account>().Single(x => x.Id == customerId);

            if (targetEntity.ResponsibleContactId.Id != customerAccount.PrimaryContactId.Id)
            {
                throw new InvalidPluginExecutionException("Invalid Responsible Contact");
            }
        }
    }
}