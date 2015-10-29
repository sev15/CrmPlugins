using System.Linq;
using Sample.Crm.Entities;
using SEV.Crm.Business;
using SEV.Crm.Business.Agents;
using SEV.Crm.ServiceContext;

namespace Sample.Crm.Business.Agents
{
    public class IncidentsCustomerContactUpdater : CrmBusinessAgent
    {
        public override void Execute(IPluginExecutorContext context)
        {
            base.Execute(context);

            var targetEntity = context.GetTargetEntity<Account>();
            var crmServiceContext = (ICrmServiceContext)Context[0];
            var incidents =
                    crmServiceContext.CreateQuery<Incident>().Where(x => x.CustomerId.Id == targetEntity.Id).ToArray();
            if (!incidents.Any())
            {
                return;
            }

            foreach (var incident in incidents)
            {
                incident.ResponsibleContactId = targetEntity.PrimaryContactId;
                crmServiceContext.UpdateObject(incident);
            }
            crmServiceContext.SaveChanges();
        }
    }
}