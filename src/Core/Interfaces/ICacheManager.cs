using System;
namespace Core.Interfaces
{
    public interface ICacheManager<T> where T : IEntity
    {
        bool IsCacheableEntity();
        Task<T> GetOne(int id);
        void SetOne(T entity);

        Task<IList<T>> GetList();
        void SetList(IList<T> entity);

        Task<T> GetAsync<T>(string key, Func<Task<T>> acquire);

        Task<T> GetAsync<T>(string key, Func<Task<T>> acquire, long cacheTime);
        Task RemoveAsync(string key);
    }
}

