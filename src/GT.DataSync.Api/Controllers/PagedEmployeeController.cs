using GT.DataSync.Api.Services;
using GT.DataSync.Core.Data;
using LinqKit;
using Microsoft.AspNetCore.Mvc;

namespace GT.DataSync.Api.Controllers
{
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
            var predicate = PredicateBuilder.New<Employee>(true);

            if (DateTime.TryParse(token, out var checkpoint))
            {
                predicate = predicate.Or(b => b.ModifiedOn > checkpoint);
            }

            var data = _employees.GetEmployees().Where(predicate).OrderBy(x => x.ModifiedOn).Take(5).ToList();

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

        [HttpGet]
        [Route("employee/keys")]
        public Task<KeyListResponse> GetPagedWithReconcileKeys()
        {
            return Task.FromResult(new KeyListResponse
            {
                Keys = _employees.GetEmployees().Select(x => x.Id!).ToList()
            }) ;
        }
    }
}