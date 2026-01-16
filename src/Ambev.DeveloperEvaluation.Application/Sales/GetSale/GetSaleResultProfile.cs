using AutoMapper;
using Ambev.DeveloperEvaluation.Domain.Entities;

namespace Ambev.DeveloperEvaluation.Application.Sales.GetSale;

public class GetSaleResultProfile : Profile
{
    public GetSaleResultProfile()
    {
        CreateMap<Sale, GetSaleResult>()
            .ForMember(d => d.CustomerId, o => o.MapFrom(s => s.CustomerId))
            .ForMember(d => d.CustomerName, o => o.MapFrom(s => s.CustomerName))
            .ForMember(d => d.SaleDate, o => o.MapFrom(s => s.SaleDate));
        CreateMap<SaleItem, GetSaleItemDto>();
    }
}
