namespace SEV.Crm.Business
{
    public interface IBusinessConfiguratorFactory
    {
        SEV.Crm.Business.Configurators.BusinessConfigurator GetConfigurator(CrmPluginEvent pluginEvent);
    }
}