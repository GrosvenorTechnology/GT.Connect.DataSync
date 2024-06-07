using GT.DataSync.Api.Services;
using GT.DataSync.Core.Data;
using Microsoft.AspNetCore.Mvc;

namespace GT.DataSync.Api.Controllers;

[ApiController]
[Produces("application/json")]
[Route("api/authoritative")]
public class AuthoritativeEmployeeController : ControllerBase
{
    private readonly AuthoritativeSampleDataService _employees;

    public AuthoritativeEmployeeController(AuthoritativeSampleDataService employees)
    {
        _employees = employees;
    }

    [HttpGet]
    [Route("employee")]
    public Task<EmployeeSyncResponse> GetAuthoritative() //Token should be ignored here
    {
        var data = _employees.GetEmployees().OrderBy(x => x.ModifiedOn).ToList();

        return Task.FromResult(new EmployeeSyncResponse
        {
            Control = new()
            {
                AuthoritativeList = true,
            },
            Employees = data
        });
    }

    
}