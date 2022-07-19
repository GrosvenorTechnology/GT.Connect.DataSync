using GT.DataSync.Core.Data;
using Refit;

namespace GT.DataSync.Core.Service
{
    public interface IDataSyncServiceApi
    {
        [Get("/distributiongroup")]
        Task<ApiResponse<DistributionGroupSyncResponse>> GetGroups([Query] string? token);

        [Get("/distributiongroup/keys")]
        Task<ApiResponse<KeyListResponse>> GetGroupKeys();

        [Get("/employee")]
        Task<ApiResponse<EmployeeSyncResponse>> GetEmployees([Query] string? token);

        [Get("/employee/keys")]
        Task<ApiResponse<KeyListResponse>> GetEmployeeKeys();
    }
}
