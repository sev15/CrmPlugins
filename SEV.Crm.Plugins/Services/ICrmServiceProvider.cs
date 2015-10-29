using System;

namespace SEV.Crm.Services
{
    public interface ICrmServiceProvider : IServiceProvider
    {
        void LoadService(Type key, object service);
    }
}
