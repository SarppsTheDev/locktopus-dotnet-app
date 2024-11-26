namespace passwordvault_domain.Repositories;

public interface IQueryRepository<T>
{
    Task<T> GetById(int id);
}