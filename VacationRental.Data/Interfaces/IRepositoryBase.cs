using VacationRental.Data.Entities;

namespace VacationRental.Data.Interfaces;

public interface IRepositoryBase<TEntity> where TEntity : EntityBase
{
    IDictionary<int, TEntity> GetAll();

    TEntity GetById(int id);

    int Add(TEntity entity);

    void Remove(TEntity entity);

    void Update(TEntity entity);
}