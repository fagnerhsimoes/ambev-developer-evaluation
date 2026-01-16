using AutoMapper;
using Ambev.DeveloperEvaluation.Domain.Entities;

namespace Ambev.DeveloperEvaluation.Application.Sales.CreateSale;

public class CreateSaleProfile : Profile
{
    public CreateSaleProfile()
    {
        CreateMap<CreateSaleCommand, Sale>()
            .ForMember(dest => dest.SaleDate, opt => opt.MapFrom(src => src.Date))
            .ForMember(dest => dest.SaleItems, opt => opt.MapFrom(src => src.Items));
        CreateMap<CreateSaleCommand.CreateSaleItemDto, SaleItem>();
        CreateMap<Sale, CreateSaleResult>();
    }
}
