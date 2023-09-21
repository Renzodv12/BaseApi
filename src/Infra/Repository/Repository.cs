using System;
using Core.Interfaces;

namespace Infra.Repository
{
    public class Repository<T> : IRepository<T> where T : IEntity
    {
        IDbContext<T> _ctx;
        ICacheManager<T> _cache;
        public Repository(IDbContext<T> ctx, ICacheManager<T> cache)
        {
            _cache = cache;
            _ctx = ctx;
        }

        public void Delete(int id)
        {
            _ctx.Delete(id);
        }

        public void DeleteAsync(int id)
        {
            _ctx.DeleteAsync(id);
        }

        public IList<T> GetAll()
        {
            if (_cache.IsCacheableEntity())
            {
                var redisOne = _cache.GetList();
                if (redisOne.Result == null)
                {
                    var list = _ctx.GetAll();
                    _cache.SetList(list);
                    return list;
                }
                return redisOne.Result;
            }
            else
                return _ctx.GetAll();
        }

        public async Task<IList<T>> GetAllAsync()
        {
            if (_cache.IsCacheableEntity())
            {
                var redisOne = await _cache.GetList();
                if (redisOne == null)
                {
                    var list = await _ctx.GetAllAsync();
                    _cache.SetList(list);
                    return list;
                }
                return redisOne;
            }
            else
                return await _ctx.GetAllAsync();
        }

        public T GetById(int id)
        {

            if (_cache.IsCacheableEntity())
            {
                var redisOne = _cache.GetOne(id);
                if (redisOne.Result == null)
                {
                    var one = _ctx.GetById(id);
                    _cache.SetOne(one);
                    return one;
                }
                return redisOne.Result;
            }
            else
                return _ctx.GetById(id);
        }

        public async Task<T> GetByIdAsync(int id)
        {
            if (_cache.IsCacheableEntity())
            {
                var redisOne = await _cache.GetOne(id);
                if (redisOne == null)
                {
                    var one = await _ctx.GetByIdAsync(id);
                    _cache.SetOne(one);
                    return one;
                }
                return redisOne;
            }
            else
                return await _ctx.GetByIdAsync(id);
        }

        public T Save(T entity)
        {
            return _ctx.Save(entity);
        }

        public Task<T> SaveAsync(T entity)
        {
            return _ctx.SaveAsync(entity);
        }
    }
}
