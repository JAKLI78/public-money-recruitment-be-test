using VacationRental.Data.Entities;
using VacationRental.Data.Interfaces;

namespace VacationRental.Data.Repositories;

public abstract class RepositoryBase<TEntity> : IRepositoryBase<TEntity> where TEntity : EntityBase
{
    private readonly IDictionary<int, TEntity> _entities;

    public RepositoryBase(IDictionary<int, TEntity> entities)
    {
        _entities = entities;
    }

    public int Add(TEntity entity)
    {
        var id = _entities.Keys.Count + 1;

        entity.Id = id;

        _entities.Add(id, entity);

        return id;
    }

    public TEntity GetById(int id)
    {
        if (!_entities.ContainsKey(id))
        {
            return null;
        }

        return _entities[id];
    }

    public IDictionary<int, TEntity> GetAll()
    {
        return _entities;
    }

    public void Remove(TEntity entity)
    {
        _entities.Remove(entity.Id);
    }

    public void Update(TEntity entity)
    {
        _entities[entity.Id] = entity;
    }
}