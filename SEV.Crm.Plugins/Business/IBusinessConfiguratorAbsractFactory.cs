using System;

namespace SEV.Crm.Business
{
    public interface IBusinessConfiguratorAbsractFactory
    {
        IBusinessConfiguratorFactory GetFactory<TEntity>() where TEntity : Microsoft.Xrm.Sdk.Entity;
        void AddFactory(Type factoryType, IBusinessConfiguratorFactory factory);
    }
}