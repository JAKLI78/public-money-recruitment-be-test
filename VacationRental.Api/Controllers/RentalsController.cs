using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using VacationRental.Api.Models;
using VacationRental.Core.Exceptions;
using VacationRental.Data.Entities;
using VacationRental.Services.Interfaces;

namespace VacationRental.Api.Controllers;

[Route("api/v1/rentals")]
[ApiController]
public class RentalsController : ControllerBase
{
    private readonly IMapper _mapper;
    private readonly IRentalService _rentalService;

    public RentalsController(IMapper mapper, IRentalService rentalService)
    {
        _mapper = mapper;
        _rentalService = rentalService;
    }

    [HttpGet]
    [Route("{rentalId:int}")]
    public RentalViewModel Get(int rentalId)
    {
        var rental = _rentalService.GetRentalById(rentalId);

        if (rental == null)
        {
            throw new EntityNotFoundException("Rental not found");
        }

        return _mapper.Map<RentalViewModel>(rental);
    }

    [HttpPost]
    public ResourceIdViewModel Post(RentalBindingModel model)
    {
        var rental = _mapper.Map<Rental>(model);

        var id = _rentalService.AddRental(rental);

        var key = new ResourceIdViewModel { Id = id };

        return key;
    }

    [HttpPut]
    public ResourceIdViewModel Put(RentalBindingUpdateModel model)
    {
        var rentalToUpdate = _mapper.Map<Rental>(model);

        _rentalService.UpdateRental(rentalToUpdate);

        return new ResourceIdViewModel { Id = model.Id };
    }
}