using Bogus;
using GT.DataSync.Core.Data;

namespace GT.DataSync.Api.Services; 

public class TrackedSampleDataService
{
    private readonly List<DistributionGroup> _groups;
    private readonly List<Employee> _employees;
    private readonly PeriodicTimer _timer = new PeriodicTimer(TimeSpan.FromSeconds(7));
    private readonly Random _random;

    public TrackedSampleDataService()
    {
        _random = new Random(DateTime.UtcNow.Millisecond);
        Randomizer.Seed = new Random(8675309);

        var ids = 0;
        var testgroups = new Faker<DistributionGroup>()
            .RuleFor(o => o.Id, f => (++ids).ToString())
            .RuleFor(o => o.DisplayName, (f, u) => $"dg-{ids}")
            .RuleFor(o => o.CreatedOn, f => DateTimeOffset.UtcNow)
            .RuleFor(o => o.ModifiedOn, (f, u) => u.CreatedOn);

        _groups = testgroups.Generate(5);

        ids = 0;
        var testemployees = new Faker<Employee>()
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
                    var active = _employees.Where(x => x.Delete != true).ToList();
                    var toDelete = _random.Next(0, Math.Min(active.Count, 5));
                    for (int i = 0; i < toDelete; i++)
                    {
                        active = _employees.Where(x=> x.Delete != true).ToList();
                        if (active.Any())
                        {
                            DeleteEmployee(active[_random.Next(0, active.Count)]);
                        }
                    }

                    var toUpdate = _random.Next(0, 3);
                    for (int i = 0; i < toUpdate; i++)
                    {
                        _employees[_random.Next(0, _employees.Count)].ModifiedOn = DateTimeOffset.UtcNow;
                    }

                    _employees.AddRange(testemployees.Generate(_random.Next(0, 5)));

                    var cutoff = DateTimeOffset.UtcNow.AddHours(-1);
                    var expired = _employees.Where(x => x.Delete == true && x.ModifiedOn < cutoff).ToList();
                    foreach (var item in expired)
                    {
                        _employees.Remove(item);
                    }
                }
            }
        });
    }

    private void DeleteEmployee(Employee emp)
    {
        emp.Delete = true;
        emp.ModifiedOn = DateTimeOffset.UtcNow;
        emp.GivenName = null;
        emp.FamilyName = null;
        emp.DisplayName = null;
        emp.KeypadId = null;
        emp.Language = null;
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
