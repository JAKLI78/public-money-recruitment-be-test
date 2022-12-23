using AutoMapper;
using VacationRental.Api.Models;
using VacationRental.Data.Entities;
using VacationRental.Services.Models;

namespace VacationRental.Api.MapperProfiles;

public class MapperProfile : Profile
{
    public MapperProfile()
    {
        CreateMap<CalendarModel, CalendarViewModel>();
        CreateMap<CalendarDateModel, CalendarDateViewModel>();
        CreateMap<CalendarBookingModel, CalendarBookingViewModel>();
        CreateMap<CalendarPreparationModel, CalendarPreparationViewModel>();

        CreateMap<Rental, RentalViewModel>();
        CreateMap<RentalBindingModel, Rental>()
            .ForMember(dest => dest.Id, options => options.Ignore());
        CreateMap<RentalBindingUpdateModel, Rental>();

        CreateMap<Booking, BookingViewModel>();
        CreateMap<BookingBindingModel, Booking>()
            .ForMember(dest => dest.Id, options => options.Ignore());
    }
}