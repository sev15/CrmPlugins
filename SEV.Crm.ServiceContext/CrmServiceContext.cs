
namespace SEV.Crm.ServiceContext
{
    internal class CrmServiceContext : Microsoft.Xrm.Client.CrmOrganizationServiceContext, ICrmServiceContext
    {
        public CrmServiceContext(Microsoft.Xrm.Sdk.IOrganizationService service) : base(service)
        {
        }

        public CrmServiceContext(Microsoft.Xrm.Client.CrmConnection connection) : base(connection)
        {      
        }

        public CrmServiceContext() : base("Xrm")
        {
        }
    }
}