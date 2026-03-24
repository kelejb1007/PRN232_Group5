using DAL.DTOs.Dashboard;

namespace BLL.Services.Admin.Interfaces
{
    public interface IDashboardService
    {
        Task<DashboardSummaryDTO> GetFullDashboardSummaryAsync();
    }
}
