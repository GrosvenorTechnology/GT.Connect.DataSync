using System.ComponentModel.DataAnnotations;

namespace GT.DataSync.Core.Data;

public class EmployeeSyncResponse
{
    [Required]
    public ControlBlock? Control { get; set; }
    [Required]
    public List<Employee>? Employees { get; set; }
}

