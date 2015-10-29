namespace SEV.Crm.Services
{
    public static class CrmServiceProviderExtensions
    {
        public static TService GetService<TService>(this ICrmServiceProvider provider)
        {
            return (TService)provider.GetService(typeof(TService));
        }

        public static void LoadService<TService>(this ICrmServiceProvider provider, object service)
        {
            provider.LoadService(typeof(TService), service);
        }
    }
}