using VacationRental.Data.Entities;
using VacationRental.Data.Interfaces;

namespace VacationRental.Data.Repositories;

public class RentalRepository : RepositoryBase<Rental>, IRentalRepository
{
    public RentalRepository(IDictionary<int, Rental> entities) : base(entities)
    {
    }
}