using Bogus;
using GT.DataSync.Core.Data;

namespace GT.DataSync.Api.Services; 

public class AuthoritativeSampleDataService
{
    private List<DistributionGroup> _groups;
    private List<Employee> _employees;
    private PeriodicTimer _timer = new PeriodicTimer(TimeSpan.FromSeconds(7));

    public AuthoritativeSampleDataService()
    {
        Randomizer.Seed = new Random(8675309);

        var ids = 0;
        var createdOn = DateTimeOffset.Parse("2021-01-02T03:04:05Z");
        var testgroups = new Faker<DistributionGroup>()
            //.StrictMode(true)
            .RuleFor(o => o.Id, f => (++ids).ToString())
            .RuleFor(o => o.DisplayName, (f, u) => $"dg-{ids}")
            .RuleFor(o => o.CreatedOn, f => createdOn.AddMinutes(ids))
            .RuleFor(o => o.ModifiedOn, (f, u) => u.CreatedOn);

        _groups = testgroups.Generate(5);

        ids = 0;
        createdOn = DateTimeOffset.Parse("2022-01-02T03:04:05Z");
        var testemployees = new Faker<Employee>()
            //.StrictMode(true)
            .RuleFor(o => o.Id, f => (++ids).ToString())
            .RuleFor(o => o.GivenName, (f, u) => f.Name.FirstName())
            .RuleFor(o => o.FamilyName, (f, u) => f.Name.LastName())
            .RuleFor(o => o.DisplayName, (f, u) => $"{u.GivenName} {u.FamilyName}")
            .RuleFor(o => o.KeypadId, (f, u) => ids.ToString("00000"))
            .RuleFor(o => o.Language, f => "en")
            .RuleFor(o => o.CreatedOn, f => createdOn.AddMinutes(ids))
            .RuleFor(o => o.ModifiedOn, (f, u) => u.CreatedOn);

        _employees = testemployees.Generate(5);

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
