namespace HSE_financial_accounting.Repositories
{
    public class CachedRepository<T> : IRepository<T> where T : class
    {
        private readonly IRepository<T> _mainRepository;
        private readonly InMemoryRepository<T> _cache = new();

        public CachedRepository(IRepository<T> mainRepository)
        {
            _mainRepository = mainRepository;
            // При создании загружаем все данные в кэш
            foreach (T entity in _mainRepository.GetAll())
            {
                _cache.Add(entity);
            }
        }

        public void Add(T entity)
        {
            _mainRepository.Add(entity);
            _cache.Add(entity);
        }

        public void Update(T entity)
        {
            _mainRepository.Update(entity);
            _cache.Update(entity);
        }

        public void Delete(Guid id)
        {
            _mainRepository.Delete(id);
            _cache.Delete(id);
        }

        public T GetById(Guid id)
        {
            // Сначала проверяем кэш
            T cachedEntity = _cache.GetById(id);
            if (cachedEntity != null)
            {
                return cachedEntity;
            }

            // Если в кэше нет, берём из основного хранилища
            T entity = _mainRepository.GetById(id);
            if (entity != null)
            {
                _cache.Add(entity);
            }

            return entity;
        }

        public IEnumerable<T> GetAll()
        {
            return _cache.GetAll();
        }
    }
}