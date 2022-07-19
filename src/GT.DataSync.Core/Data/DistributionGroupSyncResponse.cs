namespace GT.DataSync.Core.Data;

public class DistributionGroupSyncResponse
{
    public ControlBlock? Control { get; set; }
    public List<DistributionGroup>? DistributionGroups { get; set; }
}
