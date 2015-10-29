namespace SEV.Crm.Services
{
    public interface ICrmCache
    {
        bool Contains(string key);
        void Add(string key, object value);
        object Get(string key);
        void Remove(string key);
    }
}