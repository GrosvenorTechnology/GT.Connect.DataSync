using GT.DataSync.Api.Services;
using GT.DataSync.Core.Data;
using LinqKit;
using Microsoft.AspNetCore.Mvc;
using Swashbuckle.AspNetCore.Annotations;
using System;

namespace GT.DataSync.Api.Controllers
{
    [ApiController]
    [Produces("application/json")]
    [Route("pagedWithReconcile")]

    public class PagedWithReconcileEmployeeController : ControllerBase
    {
        private readonly PagedSampleDataService _employees;

        public PagedWithReconcileEmployeeController(PagedSampleDataService employees)
        {
            _employees = employees;
        }

        [HttpGet]
        [Route("employee")]
        public Task<EmployeeSyncResponse> GetPagedWithReconcile([FromQuery] string? token)
        {
            var predicate = PredicateBuilder.New<Employee>(true);

            if (DateTimeOffset.TryParse(token, out var checkpoint))
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
                    ContinuationToken = token,
                    TriggerReconcile = data.Count < 5 ? true : null 
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