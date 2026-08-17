using TaskManager.Application.Features.Projects.GetProjects;
using TaskManager.Application.Features.Todos.GetTodos;
using TaskManager.Domain.Entities;
using TaskManager.Infrastructure.ExtensionMethods;

namespace TaskManager.Infrastructure.UnitTests.ExtensionMethods;

public class QueryFeaturesExtensionsTests
{
    [Test]
    public void ApplyTodoFilters_Should_Filter_By_Search()
    {
        // Arrange
        var list = new List<TodoItem>
        {
            TodoItem.Create("Test1", "Description1", Guid.NewGuid()),
            TodoItem.Create("Test2", "Description1", Guid.NewGuid()),
            TodoItem.Create("Test3", "Description1", Guid.NewGuid()),
            TodoItem.Create("Test4", "Description1", Guid.NewGuid()),
            TodoItem.Create("Test5", "Description1", Guid.NewGuid()),
            TodoItem.Create("Test6", "Description1", Guid.NewGuid()),
            TodoItem.Create("Test7", "Description1", Guid.NewGuid()),
        }.AsQueryable();

        var filer = new QueryParamTodo()
        {
            Search = "Test1"
        };

        // Act 

        var listFiltered = list.ApplyTodoFilters(filer);

        // Assert

        Assert.That(listFiltered, Is.Not.Empty);
        Assert.Multiple(() =>
        {
            Assert.That(listFiltered.Count, Is.EqualTo(1));
            Assert.That(listFiltered.FirstOrDefault()?.Title, Is.EqualTo("Test1"));
        });
    }

    [Test]
    public void ApplyTodoFilters_Should_Filter_By_IsComplete_True()
    {
        // Arrange

        var todo1 = TodoItem.Create("Test1", "Description1", Guid.NewGuid());
        var todo2 = TodoItem.Create("Test2", "Description1", Guid.NewGuid());
        var todo3 = TodoItem.Create("Test3", "Description1", Guid.NewGuid() );
        var todo4 = TodoItem.Create("Test4", "Description1", Guid.NewGuid());
        var todo5 = TodoItem.Create("Test5", "Description1", Guid.NewGuid());
        var todo6 = TodoItem.Create("Test6", "Description1", Guid.NewGuid());
        var todo7 = TodoItem.Create("Test7", "Description1", Guid.NewGuid());

        todo1.Complete();
        todo3.Complete();
        todo7 .Complete();

        var list = new List<TodoItem>
        {
             todo1, todo2 , todo3 , todo4 , todo5 , todo6, todo7
        }.AsQueryable();

        var filer = new QueryParamTodo()
        {
            IsCompleted = true
        };

        // Act 

        var listFiltered = list.ApplyTodoFilters(filer);

        // Assert

        Assert.That(listFiltered, Is.Not.Empty);
        Assert.That(listFiltered.Count, Is.EqualTo(3));
    }

    [Test]
    public void ApplyTodoFilters_Should_Filter_By_IsComplete_False()
    {
        // Arrange

        var todo1 = TodoItem.Create("Test1", "Description1", Guid.NewGuid());
        var todo2 = TodoItem.Create("Test2", "Description1", Guid.NewGuid());
        var todo3 = TodoItem.Create("Test3", "Description1", Guid.NewGuid());
        var todo4 = TodoItem.Create("Test4", "Description1", Guid.NewGuid());
        var todo5 = TodoItem.Create("Test5", "Description1", Guid.NewGuid());
        var todo6 = TodoItem.Create("Test6", "Description1", Guid.NewGuid());
        var todo7 = TodoItem.Create("Test7", "Description1", Guid.NewGuid());

        todo1.Complete();
        todo3.Complete();
        todo7.Complete();

        var list = new List<TodoItem>
        {
             todo1, todo2 , todo3 , todo4 , todo5 , todo6, todo7
        }.AsQueryable();

        var filer = new QueryParamTodo()
        {
            IsCompleted = false
        };

        // Act 

        var listFiltered = list.ApplyTodoFilters(filer);

        // Assert

        Assert.That(listFiltered, Is.Not.Empty);
        Assert.That(listFiltered.Count, Is.EqualTo(4));
    }

    [Test]
    public void ApplyTodoSorting_Should_Sort_By_Title_Ascending()
    {
        var todos = new List<TodoItem>
        {
            TodoItem.Create("Charlie", null, Guid.NewGuid()),
            TodoItem.Create("Alpha", null, Guid.NewGuid()),
            TodoItem.Create("Bravo", null, Guid.NewGuid())
        }.AsQueryable();

        var sorting = new QueryParamTodo
        {
            SortBy = "title",
            SortDirection = "asc"
        };

        var result = todos
            .ApplyTodoSorting(sorting)
            .ToList();

        Assert.That(result[0].Title, Is.EqualTo("Alpha"));
        Assert.That(result[1].Title, Is.EqualTo("Bravo"));
        Assert.That(result[2].Title, Is.EqualTo("Charlie"));
    }

    [Test]
    public void ApplyTodoSorting_Should_Sort_By_Title_Descending()
    {
        var todos = new List<TodoItem>
        {
            TodoItem.Create("Charlie", null, Guid.NewGuid()),
            TodoItem.Create("Alpha", null, Guid.NewGuid()),
            TodoItem.Create("Bravo", null, Guid.NewGuid())
        }.AsQueryable();

        var sorting = new QueryParamTodo
        {
            SortBy = "title",
            SortDirection = "desc"
        };

        var result = todos
            .ApplyTodoSorting(sorting)
            .ToList();

        Assert.That(result[0].Title, Is.EqualTo("Charlie"));
        Assert.That(result[1].Title, Is.EqualTo("Bravo"));
        Assert.That(result[2].Title, Is.EqualTo("Alpha"));
    }

    [Test]
    public void ApplyProjectFilters_Should_Filter_By_Name()
    {
        var projects = new List<Project>
        {
            Project.Create("Task Manager", null, Guid.NewGuid()),
            Project.Create("Shop App", null, Guid.NewGuid()),
            Project.Create("Task Tracker", null, Guid.NewGuid())
        }.AsQueryable();

        var filter = new QueryParamProject
        {
            Search = "task"
        };

        var result = projects
            .ApplyProjectFilters(filter)
            .ToList();

        Assert.That(result.Count, Is.EqualTo(2));

        Assert.That(
            result.All(p => p.Name.ToLower().Contains("task")),
            Is.True);
    }

    [Test]
    public void ApplyProjectSorting_Should_Sort_By_Name_Descending()
    {
        var projects = new List<Project>
        {
            Project.Create("Bravo", null, Guid.NewGuid()),
            Project.Create("Alpha", null, Guid.NewGuid()),
            Project.Create("Charlie", null, Guid.NewGuid())
        }.AsQueryable();

        var filter = new QueryParamProject
        {
            SortBy = "name",
            SortDirection = "desc"
        };

        var result = projects
            .ApplyProjectSorting(filter)
            .ToList();

        Assert.That(result[0].Name, Is.EqualTo("Charlie"));
        Assert.That(result[1].Name, Is.EqualTo("Bravo"));
        Assert.That(result[2].Name, Is.EqualTo("Alpha"));
    }
}
