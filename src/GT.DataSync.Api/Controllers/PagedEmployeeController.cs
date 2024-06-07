using GT.DataSync.Api.Services;
using GT.DataSync.Core.Data;
using Microsoft.AspNetCore.Mvc;

namespace GT.DataSync.Api.Controllers;

[ApiController]
[Produces("application/json")]
[Route("api/paged")]
public class PagedEmployeeController : ControllerBase
{
    private readonly PagedSampleDataService _employees;

    public PagedEmployeeController(PagedSampleDataService employees)
    {
        _employees = employees;
    }

    [HttpGet]
    [Route("employee")]
    public Task<EmployeeSyncResponse> GetPaged([FromQuery] string? token)
    {
        var query = _employees.GetEmployees().AsQueryable();

        if (DateTime.TryParse(token, out var checkpoint))
        {
            query = query.Where(x => x.ModifiedOn > checkpoint);
        }

        var data = query.OrderBy(x => x.ModifiedOn).Take(5).ToList();

        if (data.Any())
        {
            token = data.Max(x => x.ModifiedOn).ToString("O");
        }

        return Task.FromResult(new EmployeeSyncResponse
        {
            Control = new()
            {
                ContinuationToken = token
            },
            Employees = data
        });
    }
}