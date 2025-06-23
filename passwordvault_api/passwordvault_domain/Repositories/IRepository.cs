namespace passwordvault_domain.Repositories;

public interface IRepository<T>
{
    Task<long> Create(T entity);
    Task<T> Update(T entity);
    Task<long> Delete(long id);
}