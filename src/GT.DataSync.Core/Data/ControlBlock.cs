namespace GT.DataSync.Core.Data;

public class ControlBlock
{
    public bool? AuthoritativeList { get; set; }
    public int? TotalItems { get; set; }
    public bool? TriggerReconcile { get; set; }
    public string? ContinuationToken { get; set; }
    public bool? NoMoreData { get; set; }
}
