namespace VacationRental.Data.Entities;

public class Booking : EntityBase
{
    public int RentalId { get; set; }
    public DateTime Start { get; set; }
    public int Nights { get; set; }
    public int PreparationTimeInDays { get; set; }
    public int Unit { get; set; }
}