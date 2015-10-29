
namespace SEV.Crm.ServiceContext
{
    public class CrmServiceContextFactory : ICrmServiceContextFactory
    {
        public ICrmServiceContext CreateServiceContext(Microsoft.Xrm.Sdk.IOrganizationService service)
        {
            return new CrmServiceContext(service);
        }

        public ICrmServiceContext CreateServiceContext(Microsoft.Xrm.Client.CrmConnection connection)
        {
            return new CrmServiceContext(connection);
        }

        public ICrmServiceContext CreateServiceContext()
        {
            return new CrmServiceContext();
        }
    }
}