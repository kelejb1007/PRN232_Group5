using BLL.Services.Admin.Interfaces;
using DAL.DTOs.Dashboard;
using DAL.Repositories.Admin.Interfaces;

namespace BLL.Services.Admin
{
    public class DashboardService : IDashboardService
    {
        private readonly IDashboardRepository _dashboardRepo;

        public DashboardService(IDashboardRepository dashboardRepo)
        {
            _dashboardRepo = dashboardRepo;
        }

        public async Task<DashboardSummaryDTO> GetFullDashboardSummaryAsync()
        {
            try 
            {
                var summary = new DashboardSummaryDTO
                {
                    TotalRevenue = await _dashboardRepo.GetTotalRevenueAsync(),
                    TotalOrders = await _dashboardRepo.GetTotalOrdersAsync(),
                    TotalCustomers = await _dashboardRepo.GetTotalCustomersAsync(),
                    TotalBooks = await _dashboardRepo.GetTotalBooksAsync(),
                    RevenueByMonth = await _dashboardRepo.GetRevenueByMonthAsync(DateTime.UtcNow.Year),
                    RevenueByDay = await _dashboardRepo.GetRevenueByDayAsync(DateTime.UtcNow.Date.AddDays(-7), DateTime.UtcNow.Date),
                    TopSellingBooks = await _dashboardRepo.GetTopSellingBooksAsync(5),
                    LowStockBooks = await _dashboardRepo.GetLowStockBooksAsync(10), // Threshold = 10
                    RegistrationsByDay = await _dashboardRepo.GetRegistrationsByDayAsync(7)
                };

                return summary;
            }
            catch (Exception ex)
            {
                // This will be caught by Controller and detailed error returned
                throw;
            }
        }
    }
}
