using System;
using System.Security.Principal;
using Core.Interfaces;

namespace Business.Application
{
    public class Application<T> : IApplication<T> where T : IEntity
    {
        IRepository<T> _repository;
        public Application(IRepository<T> repository)
        {
            _repository = repository;
        }
        public void Delete(int id)
        {
            _repository.Delete(id);
        }

        public void DeleteAsync(int id)
        {
            _repository.DeleteAsync(id);
        }

        public IList<T> GetAll()
        {
            return _repository.GetAll();
        }

        public Task<IList<T>> GetAllAsync()
        {
            return _repository.GetAllAsync();
        }

        public T GetById(int id)
        {
            return _repository.GetById(id);
        }

        public Task<T> GetByIdAsync(int id)
        {
            return _repository.GetByIdAsync(id);
        }

        public T Save(T entity)
        {
            return _repository.Save(entity);
        }

        public Task<T> SaveAsync(T entity)
        {
            return _repository.SaveAsync(entity);
        }
    }
}