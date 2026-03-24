using DAL.Data;
using DAL.Models.Enums;
using Microsoft.EntityFrameworkCore;

namespace Intelligence_Book_API.Services
{
    public class OrderCompletionHostedService : BackgroundService
    {
        private readonly IServiceProvider _serviceProvider;
        private readonly ILogger<OrderCompletionHostedService> _logger;

        public OrderCompletionHostedService(IServiceProvider serviceProvider, ILogger<OrderCompletionHostedService> logger)
        {
            _serviceProvider = serviceProvider;
            _logger = logger;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            _logger.LogInformation("OrderCompletionHostedService is starting.");

            while (!stoppingToken.IsCancellationRequested)
            {
                try
                {
                    using var scope = _serviceProvider.CreateScope();
                    var db = scope.ServiceProvider.GetRequiredService<Intelligence_Book_APIContext>();

                    // Retrieve orders that have been waiting > 5 days (Processing or Shipped)
                    var thresholdDate = DateTime.Now.AddDays(-5);

                    var ordersToComplete = await db.Orders
                        .Where(o => (o.Status == OrderStatus.Processing || o.Status == OrderStatus.Shipped) 
                                    && o.OrderDate <= thresholdDate)
                        .ToListAsync(stoppingToken);

                    if (ordersToComplete.Any())
                    {
                        foreach (var order in ordersToComplete)
                        {
                            order.Status = OrderStatus.Delivered;
                            _logger.LogInformation($"Auto-completed Order #{order.OrderId} as it has been processing for > 5 days.");
                        }
                        
                        await db.SaveChangesAsync(stoppingToken);
                    }
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Error occurred while auto-completing orders.");
                }

                // Check again in 1 hour
                await Task.Delay(TimeSpan.FromHours(1), stoppingToken);
            }
        }
    }
}
