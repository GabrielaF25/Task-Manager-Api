using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TaskManager.Application.Features.Projects.Queries;

public class QueryParamProject
{
    public string? Search { get; set; }
    public string? SortBy { get; set; }
    public string? SortDirection {  get; set; }
}
