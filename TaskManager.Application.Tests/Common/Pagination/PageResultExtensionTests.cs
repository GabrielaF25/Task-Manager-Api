using NUnit.Framework;
using TaskManager.Application.Common.Pagination;

namespace TaskManager.Application.Tests.Common.Pagination;

[TestFixture]
public class PageResultExtensionTests
{
    [Test]
    public void TransformToPageList_Should_Return_First_Page_Correctly()
    {
        // Arrange
        var data = Enumerable.Range(1, 10);
        var pagination = new PaginationParam
        {
            PageNumber = 1,
            PageSize = 10
        };

        // Act
        var result = data.TransformToPageList(pagination, 25);

        // Assert
        Assert.That(result.PageNumber, Is.EqualTo(1));
        Assert.That(result.PageSize, Is.EqualTo(10));
        Assert.That(result.TotalCount, Is.EqualTo(25));
        Assert.That(result.TotalPages, Is.EqualTo(3));
        Assert.That(result.HasPreviousPage, Is.False);
        Assert.That(result.HasNextPage, Is.True);
        Assert.That(result.Items.Count(), Is.EqualTo(10));
    }

    [Test]
    public void TransformToPageList_Should_Return_Last_Page_Correctly()
    {
        // Arrange
        var data = Enumerable.Range(21, 5);
        var pagination = new PaginationParam
        {
            PageNumber = 3,
            PageSize = 10
        };

        // Act
        var result = data.TransformToPageList(pagination, 25);

        // Assert
        Assert.That(result.PageNumber, Is.EqualTo(3));
        Assert.That(result.PageSize, Is.EqualTo(10));
        Assert.That(result.TotalCount, Is.EqualTo(25));
        Assert.That(result.TotalPages, Is.EqualTo(3));
        Assert.That(result.HasPreviousPage, Is.True);
        Assert.That(result.HasNextPage, Is.False);
        Assert.That(result.Items.Count(), Is.EqualTo(5));
    }

    [Test]
    public void TransformToPageList_Should_Return_Single_Page_When_Total_Count_Is_Less_Than_Page_Size()
    {
        // Arrange
        var data = Enumerable.Range(1, 5);
        var pagination = new PaginationParam
        {
            PageNumber = 1,
            PageSize = 10
        };

        // Act
        var result = data.TransformToPageList(pagination, 5);

        // Assert
        Assert.That(result.TotalPages, Is.EqualTo(1));
        Assert.That(result.HasPreviousPage, Is.False);
        Assert.That(result.HasNextPage, Is.False);
        Assert.That(result.Items.Count(), Is.EqualTo(5));
    }

    [Test]
    public void TransformToPageList_Should_Calculate_Total_Pages_Correctly_When_Total_Count_Is_Divisible()
    {
        // Arrange
        var data = Enumerable.Range(1, 10);
        var pagination = new PaginationParam
        {
            PageNumber = 2,
            PageSize = 10
        };

        // Act
        var result = data.TransformToPageList(pagination, 30);

        // Assert
        Assert.That(result.TotalPages, Is.EqualTo(3));
        Assert.That(result.HasPreviousPage, Is.True);
        Assert.That(result.HasNextPage, Is.True);
    }

    [Test]
    public void TransformToPageList_Should_Calculate_Total_Pages_Correctly_When_Total_Count_Is_Not_Divisible()
    {
        // Arrange
        var data = Enumerable.Range(1, 10);
        var pagination = new PaginationParam
        {
            PageNumber = 2,
            PageSize = 10
        };

        // Act
        var result = data.TransformToPageList(pagination, 21);

        // Assert
        Assert.That(result.TotalPages, Is.EqualTo(3));
        Assert.That(result.HasPreviousPage, Is.True);
        Assert.That(result.HasNextPage, Is.True);
    }
}