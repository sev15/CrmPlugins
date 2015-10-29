using Microsoft.Xrm.Sdk;
using Microsoft.Xrm.Sdk.Query;
using System;
using System.Linq;

namespace SEV.Crm.ServiceContext
{
    public interface ICrmServiceContext : IDisposable
    {
        IQueryable<TEntity> CreateQuery<TEntity>() where TEntity : Entity;
        IQueryable<Entity> CreateQuery(string entityLogicalName);

        Guid Create(Entity entity);
        void Update(Entity entity);
        void Delete(string entityName, Guid entityId);

        void AddObject(Entity entity);
        void DeleteObject(Entity entity);
        void UpdateObject(Entity entity);

        void Associate(string entityName, Guid entityId, Relationship relationship,
            EntityReferenceCollection relatedEntities);
        void Disassociate(string entityName, Guid entityId, Relationship relationship,
            EntityReferenceCollection relatedEntities);

        bool IsAttached(Entity entity);
        void Attach(Entity entity);
        SaveChangesResultCollection SaveChanges();
        bool Detach(Entity entity);
        void ClearChanges();

        EntityCollection RetrieveMultiple(QueryBase query);
        OrganizationResponse Execute(OrganizationRequest request);
    }
}