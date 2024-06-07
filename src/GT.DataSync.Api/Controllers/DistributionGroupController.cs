using GT.DataSync.Api.Services;
using GT.DataSync.Core.Data;
using Microsoft.AspNetCore.Mvc;

namespace GT.DataSync.Api.Controllers;

[ApiController]
[Route("api/distributiongroup")]
public class DistributionGroupController : ControllerBase
{
    private readonly AuthoritativeSampleDataService _employees;

    public DistributionGroupController(AuthoritativeSampleDataService employees)
    {
        _employees = employees;
    }

    [HttpGet]
    public Task<DistributionGroupSyncResponse> Get()
    {
        var data = _employees.GetGroups().OrderBy(x => x.ModifiedOn).Take(100).ToList();

        return Task.FromResult(new DistributionGroupSyncResponse
        {
            Control = new()
            {
                AuthoritativeList = true,
                ContinuationToken = data.Max(x => x.ModifiedOn).ToString("O")
            },
            DistributionGroups = data
        });
    }
}