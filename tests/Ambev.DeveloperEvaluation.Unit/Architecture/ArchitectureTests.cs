using Ambev.DeveloperEvaluation.Application;
using Ambev.DeveloperEvaluation.Domain.Entities;
using FluentAssertions;
using NetArchTest.Rules;
using Xunit;

namespace Ambev.DeveloperEvaluation.Unit.Architecture;

public class ArchitectureTests
{
    private const string DomainNamespace = "Ambev.DeveloperEvaluation.Domain";
    private const string ApplicationNamespace = "Ambev.DeveloperEvaluation.Application";
    private const string WebApiNamespace = "Ambev.DeveloperEvaluation.WebApi";
    private const string InfrastructureNamespace = "Ambev.DeveloperEvaluation.ORM";

    [Fact(DisplayName = "Domain Layer should not depend on Application Layer")]
    public void Domain_Should_Not_Depend_On_Application()
    {
        var result = Types.InAssembly(typeof(Sale).Assembly)
            .ShouldNot()
            .HaveDependencyOn(ApplicationNamespace)
            .GetResult();

        result.IsSuccessful.Should().BeTrue();
    }

    [Fact(DisplayName = "Domain Layer should not depend on WebApi Layer")]
    public void Domain_Should_Not_Depend_On_WebApi()
    {
        var result = Types.InAssembly(typeof(Sale).Assembly)
            .ShouldNot()
            .HaveDependencyOn(WebApiNamespace)
            .GetResult();

        result.IsSuccessful.Should().BeTrue();
    }

    [Fact(DisplayName = "Domain Layer should not depend on Infrastructure Layer")]
    public void Domain_Should_Not_Depend_On_Infrastructure()
    {
        var result = Types.InAssembly(typeof(Sale).Assembly)
            .ShouldNot()
            .HaveDependencyOn(InfrastructureNamespace)
            .GetResult();

        result.IsSuccessful.Should().BeTrue();
    }

    [Fact(DisplayName = "Application Layer should not depend on WebApi Layer")]
    public void Application_Should_Not_Depend_On_WebApi()
    {
        var result = Types.InAssembly(typeof(ApplicationLayer).Assembly)
            .ShouldNot()
            .HaveDependencyOn(WebApiNamespace)
            .GetResult();

        result.IsSuccessful.Should().BeTrue();
    }

    [Fact(DisplayName = "Handlers should have Handler suffix")]
    public void Handlers_Should_Have_Handler_Suffix()
    {
        var result = Types.InAssembly(typeof(ApplicationLayer).Assembly)
            .That()
            .ImplementInterface(typeof(MediatR.IRequestHandler<,>))
            .Should()
            .HaveNameEndingWith("Handler")
            .GetResult();

        result.IsSuccessful.Should().BeTrue();
    }
}
