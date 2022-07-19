using GT.DataSync.Core.Data;
using GT.DataSync.Core.Service;
using Refit;

Console.WriteLine("Hello, World!");

//var baseUrl = "https://localhost:7020/authoritative";
//var baseUrl = "https://localhost:7020/paged";
//var baseUrl = "https://localhost:7020/pagedWithReconcile";
var baseUrl = "https://localhost:7020/tracked";
var username = "stephen";
var password = "p@ssw0rd";

var api = RestService.For<IDataSyncServiceApi>(
    new HttpClient(new BasicAuthHandler(username, password))
    {
        BaseAddress = new Uri(baseUrl)
    },
    new RefitSettings
    {
        ContentSerializer = new SystemTextJsonContentSerializer(new System.Text.Json.JsonSerializerOptions
        {
            DefaultIgnoreCondition = System.Text.Json.Serialization.JsonIgnoreCondition.WhenWritingNull,
            PropertyNamingPolicy = System.Text.Json.JsonNamingPolicy.CamelCase
        })
    });

string? token = "";
var employees = new List<Employee>();

do
{
    Console.WriteLine("Starting import run...");
    var totalItems = 0;
    do
    {
        Console.WriteLine($"Requesting employees, token: {token}");
        var response = await api.GetEmployees(token);
        Console.WriteLine($"Request response code: {response.StatusCode}");
        if (response.IsSuccessStatusCode && response?.Content?.Employees is not null)
        {
            token = response.Content.Control?.ContinuationToken ?? "";
            totalItems = response.Content.Control?.TotalItems ?? -1;

            foreach (var emp in response.Content.Employees)
            {
                if (emp.Delete == true)
                {
                    Console.WriteLine($"Deleteing User: {emp.Id}");
                    var existing = employees.FirstOrDefault(x => x.Id == emp.Id);
                    if (existing is not null)
                    {
                        employees.Remove(existing);
                    }
                }
                else if (employees.Any(x => x.Id == emp.Id))
                {
                    Console.WriteLine($"Updating User: {emp.Id}");
                }
                else
                {
                    Console.WriteLine($"Adding User: {emp.Id}");
                    employees.Add(emp);
                }
            }

            if (response.Content.Control?.AuthoritativeList == true)
            {
                AuthoritativeListReconcile(employees, response.Content.Employees);
                break;
            }
            else if (response.Content.Control?.TriggerReconcile == true)
            {
                await Reconcile(api, employees);
                break;
            }
            else if (response.Content.Employees.Count == 0 || response.Content.Control?.NoMoreData == true)
            {
                break;
            }
            else if (string.IsNullOrEmpty(response.Content?.Control?.ContinuationToken))
            {
                Console.WriteLine("ERROR: ContinuationToken is null or empty, protocol violation, infinite loop");
                break;
            }
        }
        else
        {
            Console.WriteLine("ERROR: Bad Request");
            break;
        }
    } while (true);

    Console.WriteLine($"Import run complete, Employees: {employees.Count}/{totalItems}, sleeping for 5 seconds...");
    await Task.Delay(5000);

} while (true);


static void AuthoritativeListReconcile(List<Employee> employees, List<Employee> RequestEmployees)
{
    Console.WriteLine("Authoritative List, starting internal reconcile...");

    var keys = RequestEmployees.Where(x => x.Delete != true).Select(x => x.Id).ToList();
    var toDelete = employees.Select(x => x.Id).Except(keys).ToList();
    foreach (var id in toDelete)
    {
        var emp = employees.Single(x => x.Id == id);
        employees.Remove(emp);
        Console.WriteLine($"Deleteing User: {emp.Id}");
    }

    Console.WriteLine("Internal reconcile complete...");
}


static async Task Reconcile(IDataSyncServiceApi api, List<Employee> employees)
{
    Console.WriteLine("Starting internal reconcile...");

    var keyListResponse = await api.GetEmployeeKeys();
    if (keyListResponse.IsSuccessStatusCode && keyListResponse?.Content?.Keys is not null)
    {
        var keys = keyListResponse.Content.Keys;
        var toDelete = employees.Select(x => x.Id).Except(keys).ToList();
        foreach (var id in toDelete)
        {
            var emp = employees.Single(x => x.Id == id);
            employees.Remove(emp);
            Console.WriteLine($"Deleteing User: {emp.Id}");
        }
    }
    Console.WriteLine("Internal reconcile complete...");
}