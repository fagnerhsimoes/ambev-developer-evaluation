using AutoMapper;
using Ambev.DeveloperEvaluation.Domain.Entities;

namespace Ambev.DeveloperEvaluation.Application.Sales.UpdateSale;

public class UpdateSaleProfile : Profile
{
    public UpdateSaleProfile()
    {
        CreateMap<UpdateSaleCommand, Sale>()
            .ForMember(dest => dest.SaleDate, opt => opt.MapFrom(src => src.Date))
            .ForMember(dest => dest.SaleItems, opt => opt.MapFrom(src => src.Items));
        CreateMap<UpdateSaleCommand.UpdateSaleItemDto, SaleItem>();
        CreateMap<Sale, UpdateSaleResult>();
    }
}
