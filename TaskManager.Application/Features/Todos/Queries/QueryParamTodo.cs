namespace TaskManager.Application.Features.Todo.Queries;

public class QueryParamTodo
{
  
    public string? Search {  get; set; }

    public bool? IsCompleted { get; set; }

    public string? SortBy { get; set; }

    public string? SortDirection { get; set; }
}
