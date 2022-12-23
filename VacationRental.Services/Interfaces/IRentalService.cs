using VacationRental.Data.Entities;

namespace VacationRental.Services.Interfaces;

public interface IRentalService
{
    int AddRental(Rental rental);

    Rental GetRentalById(int id);

    void UpdateRental(Rental rental);
}