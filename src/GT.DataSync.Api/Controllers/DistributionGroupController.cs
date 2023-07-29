using GT.DataSync.Api.Services;
using GT.DataSync.Core.Data;
using LinqKit;
using Microsoft.AspNetCore.Mvc;

namespace GT.DataSync.Api.Controllers
{
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
        public Task<DistributionGroupSyncResponse> Get([FromQuery] string? token)
        {
            var predicate = PredicateBuilder.New<DistributionGroup>(true);

            if (DateTime.TryParse(token, out var checkpoint))
            {
                predicate = predicate.Or(b => b.ModifiedOn >= checkpoint);
            }

            var data = _employees.GetGroups().Where(predicate).OrderBy(x => x.ModifiedOn).Take(100).ToList();

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
}