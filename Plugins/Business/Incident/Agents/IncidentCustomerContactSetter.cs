using System.Linq;
using Sample.Crm.Entities;
using SEV.Crm.Business;
using SEV.Crm.Business.Agents;
using SEV.Crm.ServiceContext;

namespace Sample.Crm.Business.Agents
{
    public class IncidentCustomerContactSetter : CrmBusinessAgent
    {
        public override void Execute(IPluginExecutorContext context)
        {
            base.Execute(context);

            var targetEntity = context.GetTargetEntity<Incident>();
            var crmServiceContext = (ICrmServiceContext)Context[0];

            var customerAccount =
                        crmServiceContext.CreateQuery<Account>().Single(x => x.Id == targetEntity.CustomerId.Id);
            targetEntity.ResponsibleContactId = customerAccount.PrimaryContactId;
        }
    }
}