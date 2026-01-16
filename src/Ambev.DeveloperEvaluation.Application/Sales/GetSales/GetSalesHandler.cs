using AutoMapper;
using MediatR;
using Ambev.DeveloperEvaluation.Domain.Repositories;
using Ambev.DeveloperEvaluation.Application.Sales.GetSale;

namespace Ambev.DeveloperEvaluation.Application.Sales.GetSales;

public class GetSalesHandler(ISaleRepository saleRepository, IMapper mapper) : IRequestHandler<GetSalesCommand, GetSalesResult>
{
    public async Task<GetSalesResult> Handle(GetSalesCommand command, CancellationToken cancellationToken)
    {
        var (sales, totalCount) = await saleRepository.GetAllAsync(command.Page, command.Size, command.Order, cancellationToken);
        
        var results = mapper.Map<List<GetSaleResult>>(sales);
        
        return new GetSalesResult
        {
            Data = results,
            TotalCount = totalCount,
            CurrentPage = command.Page,
            PageSize = command.Size
        };
    }
}
