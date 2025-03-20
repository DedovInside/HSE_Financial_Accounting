namespace HSE_financial_accounting.Repositories
{
    public class DataBaseRepository<T> : IRepository<T> where T : class
    {
        private readonly InMemoryRepository<T> _storage = new();
    
        public void Add(T entity)
        {
            Thread.Sleep(100); // Имитация медленной операции записи
            _storage.Add(entity);
        }

        public void Update(T entity)
        {
            Thread.Sleep(100);
            _storage.Update(entity);
        }

        public void Delete(Guid id)
        {
            Thread.Sleep(100);
            _storage.Delete(id);
        }

        public T GetById(Guid id)
        {
            Thread.Sleep(100);
            return _storage.GetById(id);
        }

        public IEnumerable<T> GetAll()
        {
            Thread.Sleep(100);
            return _storage.GetAll();
        }
    }
}