
namespace SEV.Crm.ServiceContext
{
    public interface ICrmServiceContextFactory
    {
        ICrmServiceContext CreateServiceContext(Microsoft.Xrm.Sdk.IOrganizationService service);
        ICrmServiceContext CreateServiceContext(Microsoft.Xrm.Client.CrmConnection connection);
        ICrmServiceContext CreateServiceContext();
    }
}