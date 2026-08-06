using FluentValidation.TestHelper;
using TaskManager.Application.Common.Pagination;
using TaskManager.Application.Pagination.Validation;

namespace TaskManager.Application.Tests.Common.Pagination;

[TestFixture]
public class PaginationParamValidationTests
{
    private PaginationParamValidation _validator = null!;

    [SetUp]
    public void SetUp()
    {
        _validator = new PaginationParamValidation();
    }

    [Test]
    public async Task Should_Not_Have_Error_When_Pagination_Is_Valid()
    {
        // Arrange
        var pagination = new PaginationParam
        {
            PageNumber = 1,
            PageSize = 10
        };

        // Act
        var result = await _validator.TestValidateAsync(pagination);

        // Assert
        result.ShouldNotHaveAnyValidationErrors();
    }

    [TestCase(0)]
    [TestCase(101)]
    public async Task Should_Have_Error_When_PageSize_Is_Out_Of_Range(int pageSize)
    {
        // Arrange
        var pagination = new PaginationParam
        {
            PageNumber = 1,
            PageSize = pageSize
        };

        // Act
        var result = await _validator.TestValidateAsync(pagination);

        // Assert
        result.ShouldHaveValidationErrorFor(x => x.PageSize)
            .WithErrorMessage("The PageSize cannot be less than 1 or greater than 100");
    }

    [TestCase(0)]
    [TestCase(-1)]
    [TestCase(-10)]
    public async Task Should_Have_Error_When_PageNumber_Is_Less_Than_One(int pageNumber)
    {
        // Arrange
        var pagination = new PaginationParam
        {
            PageNumber = pageNumber,
            PageSize = 10
        };

        // Act
        var result = await _validator.TestValidateAsync(pagination);

        // Assert
        result.ShouldHaveValidationErrorFor(x => x.PageNumber)
            .WithErrorMessage("The PageNumber cannot be less than 1");
    }

    [TestCase(1, 1)]
    [TestCase(1, 100)]
    [TestCase(10, 50)]
    public async Task Should_Not_Have_Error_For_Valid_Boundary_Values(int pageNumber, int pageSize)
    {
        // Arrange
        var pagination = new PaginationParam
        {
            PageNumber = pageNumber,
            PageSize = pageSize
        };

        // Act
        var result = await _validator.TestValidateAsync(pagination);

        // Assert
        result.ShouldNotHaveAnyValidationErrors();
    }
}