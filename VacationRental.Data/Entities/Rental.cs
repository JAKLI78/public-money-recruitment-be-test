namespace VacationRental.Data.Entities;

public class Rental : EntityBase
{
    public int Units { get; set; }

    public int PreparationTimeInDays { get; set; }
}