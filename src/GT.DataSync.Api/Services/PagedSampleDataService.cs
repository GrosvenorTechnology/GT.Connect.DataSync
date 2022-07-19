using Bogus;
using GT.DataSync.Core.Data;

namespace GT.DataSync.Api.Services; 

public class PagedSampleDataService
{
    private List<DistributionGroup> _groups;
    private List<Employee> _employees;
    private PeriodicTimer _timer = new PeriodicTimer(TimeSpan.FromSeconds(7));

    public PagedSampleDataService()
    {
        Randomizer.Seed = new Random(8675309);

        var ids = 0;
        var testgroups = new Faker<DistributionGroup>()
            //.StrictMode(true)
            .RuleFor(o => o.Id, f => (++ids).ToString())
            .RuleFor(o => o.DisplayName, (f, u) => $"dg-{ids}")
            .RuleFor(o => o.CreatedOn, f => DateTimeOffset.UtcNow)
            .RuleFor(o => o.ModifiedOn, (f, u) => u.CreatedOn);

        _groups = testgroups.Generate(5);

        ids = 0;
        var testemployees = new Faker<Employee>()
            //.StrictMode(true)
            .RuleFor(o => o.Id, f => (++ids).ToString())
            .RuleFor(o => o.GivenName, (f, u) => f.Name.FirstName())
            .RuleFor(o => o.FamilyName, (f, u) => f.Name.LastName())
            .RuleFor(o => o.DisplayName, (f, u) => $"{u.GivenName} {u.FamilyName}")
            .RuleFor(o => o.KeypadId, (f, u) => ids.ToString("00000"))
            .RuleFor(o => o.Language, f => "en")
            .RuleFor(o => o.CreatedOn, f => DateTimeOffset.UtcNow)
            .RuleFor(o => o.ModifiedOn, (f, u) => u.CreatedOn);

        _employees = testemployees.Generate(15);

        _ = Task.Run(async () =>
        {
            while (await _timer.WaitForNextTickAsync())
            {
                lock(_employees)
                {
                    _employees.RemoveAt(0);
                    _employees.AddRange(testemployees.Generate(1));
                }
            }
        });
    }


    public List<Employee> GetEmployees()
    {
        lock (_employees)
        {
            return _employees;
        }
    }

    public List<DistributionGroup> GetGroups()
    {
        return _groups;
    }
}
