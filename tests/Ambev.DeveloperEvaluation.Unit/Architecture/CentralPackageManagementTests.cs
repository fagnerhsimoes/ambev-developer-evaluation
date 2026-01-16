using System.Xml.Linq;
using FluentAssertions;
using Xunit;

namespace Ambev.DeveloperEvaluation.Unit.Architecture;

/// <summary>
/// Validates that Central Package Management (CPM) is correctly applied across the solution.
/// Ensures that individual projects do not define explicit package versions, 
/// relying instead on Directory.Packages.props.
/// </summary>
public class CentralPackageManagementTests
{
    [Fact]
    public void ProjectFiles_ShouldNotHaveExplicitPackageVersions()
    {
        // Arrange
        var solutionDirectory = GetSolutionDirectory();
        Assert.NotNull(solutionDirectory);

        var projectFiles = GetAllProjectFiles(solutionDirectory);

        // Act
        var violations = new List<string>();

        foreach (var projectFile in projectFiles)
        {
            var projectXml = XDocument.Load(projectFile);
            var fileName = Path.GetFileName(projectFile);

            var invalidReferences = projectXml.Descendants("PackageReference")
                .Where(HasVersionAttribute)
                .Select(reference => new
                {
                    Package = reference.Attribute("Include")?.Value,
                    Version = reference.Attribute("Version")?.Value,
                    File = fileName
                });

            violations.AddRange(invalidReferences.Select(invalid => $"Project '{invalid.File}' defines explicit version '{invalid.Version}' for package '{invalid.Package}'. Remove the Version attribute to use Central Package Management."));
        }

        // Assert
        violations.Should()
            .BeEmpty("all projects should inherit package versions from Directory.Packages.props via CPM.");
    }

    private static bool HasVersionAttribute(XElement reference)
    {
        // We allow Version override ONLY if it contains a property variable (e.g. Version="$(MyVar)") 
        // ALTHOUGH in pure CPM usually even that is moved to Directory.Packages.props.
        // For strict CPM, ANY Version attribute in .csproj is a "local override" which we want to avoid.
        return reference.Attribute("Version") != null;
    }

    private static IEnumerable<string> GetAllProjectFiles(string solutionDirectory)
    {
        // Adjusting to look for 'src' and 'tests' based on your folder structure
        var srcPath = Path.Combine(solutionDirectory, "src");
        var testsPath = Path.Combine(solutionDirectory, "tests");

        var srcProjects = Directory.Exists(srcPath)
            ? Directory.GetFiles(srcPath, "*.csproj", SearchOption.AllDirectories)
            : [];

        var testProjects = Directory.Exists(testsPath)
            ? Directory.GetFiles(testsPath, "*.csproj", SearchOption.AllDirectories)
            : [];

        return srcProjects.Concat(testProjects);
    }

    private static string? GetSolutionDirectory()
    {
        var currentDirectory = new DirectoryInfo(Directory.GetCurrentDirectory());

        // Traverse up until we find the .sln file
        while (currentDirectory?.GetFiles("*.sln").Length == 0)
        {
            currentDirectory = currentDirectory.Parent;
        }

        return currentDirectory?.FullName;
    }
}