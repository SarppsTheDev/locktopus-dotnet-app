namespace passwordvault_domain.Repositories;

public interface IRepository<T>
{
    Task<int> Create(T entity);
    void Update(T entity);
    void Delete(T entity);
}