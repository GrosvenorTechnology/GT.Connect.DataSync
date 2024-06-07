using System.ComponentModel.DataAnnotations;

namespace GT.DataSync.Core.Data;

public class EmployeeSyncResponse
{
    public required ControlBlock? Control { get; set; }
    [Required]
    public required List<Employee>? Employees { get; set; }
}

