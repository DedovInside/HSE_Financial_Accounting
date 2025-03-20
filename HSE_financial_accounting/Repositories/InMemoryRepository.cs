using System.Reflection;

namespace HSE_financial_accounting.Repositories
{
    public class InMemoryRepository<T> : IRepository<T> where T : class
    {
        private readonly Dictionary<Guid, T> _entities = [];

        public virtual void Add(T entity)
        {
            if (entity == null)
            {
                throw new ArgumentNullException(nameof(entity));
            }

            PropertyInfo idProperty = entity.GetType().GetProperty("Id") 
                                      ?? throw new InvalidOperationException($"Entity of type {entity.GetType()} doesn't have an Id property");
                
            object idValue = idProperty.GetValue(entity) 
                             ?? throw new InvalidOperationException($"Id property of entity {entity.GetType()} is null");
                
            Guid id = (Guid)idValue;
            _entities[id] = entity;
        }

        public virtual void Update(T entity)
        {
            if (entity == null)
            {
                throw new ArgumentNullException(nameof(entity));
            }

            PropertyInfo idProperty = entity.GetType().GetProperty("Id") 
                                      ?? throw new InvalidOperationException($"Entity of type {entity.GetType()} doesn't have an Id property");
                
            object idValue = idProperty.GetValue(entity) 
                             ?? throw new InvalidOperationException($"Id property of entity {entity.GetType()} is null");
                
            Guid id = (Guid)idValue;
            _entities[id] = entity;
        }

        public virtual void Delete(Guid id)
        {
            _entities.Remove(id);
        }

        public virtual T? GetById(Guid id)
        {
            if (_entities.TryGetValue(id, out T? entity))
            {
                return entity;
            }
            return null;
        }

        public virtual IEnumerable<T> GetAll()
        {
            return _entities.Values;
        }
    }
}