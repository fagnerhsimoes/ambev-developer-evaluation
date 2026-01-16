using AutoMapper;
using MediatR;
using FluentValidation;
using Ambev.DeveloperEvaluation.Domain.Repositories;

namespace Ambev.DeveloperEvaluation.Application.Sales.GetSale;

public class GetSaleHandler(ISaleRepository saleRepository, IMapper mapper)
    : IRequestHandler<GetSaleCommand, GetSaleResult>
{
    public async Task<GetSaleResult> Handle(GetSaleCommand command, CancellationToken cancellationToken)
    {
        // Add minimal validation for ID
        if (command.Id == Guid.Empty)
           throw new ValidationException("Sale ID is required");

        var sale = await saleRepository.GetByIdAsync(command.Id, cancellationToken);
        if (sale == null)
            throw new KeyNotFoundException($"Sale with ID {command.Id} not found");

        return mapper.Map<GetSaleResult>(sale);
    }
}
