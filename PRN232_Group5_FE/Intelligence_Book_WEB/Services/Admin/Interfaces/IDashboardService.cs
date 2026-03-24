using Intelligence_Book_WEB.Models.Dashboard;
using System.Threading.Tasks;

namespace Intelligence_Book_WEB.Services.Admin.Interfaces
{
    public interface IDashboardService
    {
        Task<DashboardSummaryDTO?> GetDashboardSummaryAsync(string token);
    }
}
